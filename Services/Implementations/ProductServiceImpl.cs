using AutoMapper;
using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using bidify_be.DTOs.Product;
using bidify_be.Exceptions;
using bidify_be.Helpers;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Services.Interfaces;
using FluentValidation;
using Org.BouncyCastle.Bcpg;
using UnauthorizedAccessException = bidify_be.Exceptions.UnauthorizedAccessException;

namespace bidify_be.Services.Implementations
{
    public class ProductServiceImpl : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductServiceImpl> _logger;
        private readonly IValidator<AddProductRequest> _validatorAdd;
        private readonly IValidator<UpdateProductRequest> _validatorUpdate;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileStorageService _fileStorageService;

        public ProductServiceImpl(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ILogger<ProductServiceImpl> logger, 
            IValidator<AddProductRequest> validatorAdd,
            IValidator<UpdateProductRequest> validatorUpdate,
            ICurrentUserService currentUserService,
            IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _validatorAdd = validatorAdd;
            _validatorUpdate = validatorUpdate;
            _currentUserService = currentUserService;
            _fileStorageService = fileStorageService;
        }

        private void EnsureAuthenticatedUser(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User is not authenticated");
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
        }

        private void EnsureAdmin()
        {
            if (!_currentUserService.IsAdmin())
            {
                _logger.LogWarning("Unauthorized admin operation attempted");
                throw new UnauthorizedAccessException("You do not have permission to perform this operation.");
            }
        }

        private async Task ValidateAsync<T>(T request, IValidator<T> validator)
        {
            var result = await validator.ValidateAsync(request);
            ValidationHelper.ThrowIfInvalid(result, _logger);
        }

        public async Task<ProductResponse> AddProductAsync(AddProductRequest request)
        {
            _logger.LogInformation("Creating product with name: {Name}", request.Name);

            // 1. Auth
            var userId = _currentUserService.GetUserId();
            EnsureAuthenticatedUser(userId);

            // 2. Validate
            await ValidateAsync(request, _validatorAdd);

            // 3. Mapping
            var product = _mapper.Map<Product>(request);
            product.UserId = userId;
            product.Status = ProductStatus.Pending;
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            using var tx = await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 4. Mark thumbnail as used
                if (!string.IsNullOrWhiteSpace(request.ThumbnailPublicId))
                {
                    await _fileStorageService.MarkAsUsedAsync(request.ThumbnailPublicId);
                }

                // 5. Mark product images as used
                if (request.Images?.Any() == true)
                {
                    foreach (var img in request.Images)
                    {
                        if (!string.IsNullOrWhiteSpace(img.PublicId))
                        {
                            await _fileStorageService.MarkAsUsedAsync(img.PublicId);
                        }
                    }
                }

                // 6. Insert product
                await _unitOfWork.ProductRepository.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();

                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error while creating product");
                throw;
            }

            _logger.LogInformation("Product created successfully with Id: {Id}", product.Id);

            return _mapper.Map<ProductResponse>(product);
        }



        public async Task<bool> DeleteProductAsyncByUser(Guid id)
        {
            _logger.LogInformation("User attempting to hidden product {ProductId}", id);

            var userId = _currentUserService.GetUserId();

            EnsureAuthenticatedUser(userId);

            var product = await _unitOfWork.ProductRepository.GetProductByUser(userId, id);
            if (product == null) {
                _logger.LogWarning("User {UserId} attempted to access product {ProductId} without permission", userId, id);
                throw new UnauthorizedAccessException("You do not have permission to access this product.");
            }

            _unitOfWork.ProductRepository.DeleteAsyncByUser(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<ProductResponse> UpdateProductAsync(Guid id, UpdateProductRequest request)
        {
            _logger.LogInformation("Updating Product {Id}", id);

            var userId = _currentUserService.GetUserId();
            EnsureAuthenticatedUser(userId);

            await ValidateAsync(request, _validatorUpdate);

            var product = await _unitOfWork.ProductRepository.GetProductWithDetailsAsync(id);
            if (product == null)
                throw new ProductNotFoundException($"Product {id} not found");

            if (product.UserId != userId)
                throw new UnauthorizedAccessException("You do not have permission to update this product.");

            using var tx = await _unitOfWork.BeginTransactionAsync();

            try
            {
                /* ===================== THUMBNAIL ===================== */
                if (!string.IsNullOrWhiteSpace(request.ThumbnailPublicId) &&
                    request.ThumbnailPublicId != product.ThumbnailPublicId)
                {
                    // Mark thumbnail mới
                    await _fileStorageService.MarkAsUsedAsync(request.ThumbnailPublicId);

                    // Soft delete thumbnail cũ
                    if (!string.IsNullOrWhiteSpace(product.ThumbnailPublicId))
                    {
                        await _fileStorageService.RequestDeleteAsync(product.ThumbnailPublicId);
                    }

                    product.Thumbnail = request.Thumbnail;
                    product.ThumbnailPublicId = request.ThumbnailPublicId;
                }

                /* ===================== PRODUCT IMAGES ===================== */
                if (request.Images != null)
                {
                    var oldImagePublicIds = product.Images
                        .Select(x => x.PublicId)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToHashSet();

                    var newImagePublicIds = request.Images
                        .Select(x => x.PublicId)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToHashSet();

                    // 1. Mark ảnh mới
                    foreach (var publicId in newImagePublicIds.Except(oldImagePublicIds))
                    {
                        await _fileStorageService.MarkAsUsedAsync(publicId);
                    }

                    // 2. Soft delete ảnh bị remove
                    foreach (var publicId in oldImagePublicIds.Except(newImagePublicIds))
                    {
                        await _fileStorageService.RequestDeleteAsync(publicId);
                    }

                    // 3. Update collection
                    UpdateCollections(product, request);
                }

                /* ===================== SIMPLE FIELDS ===================== */
                _mapper.Map(request, product);

                product.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.ProductRepository.Update(product);
                await _unitOfWork.SaveChangesAsync();

                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Update product failed");
                throw;
            }

            _logger.LogInformation("Updated Product {Id} successfully", id);

            return _mapper.Map<ProductResponse>(product);
        }


        private void UpdateCollections(Product product, UpdateProductRequest request)
        {
            product.Images = request.Images
                .Select(img => new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = img.ImageUrl,
                    PublicId = img.PublicId,
                    ProductId = product.Id
                }).ToList();

            product.Attributes = request.Attributes
                .Select(attr => new ProductAttribute
                {
                    Id = Guid.NewGuid(),
                    Key = attr.Key,
                    Value = attr.Value,
                    ProductId = product.Id
                }).ToList();

            product.ProductTags = request.Tags
                .Select(tag => new ProductTag
                {
                    ProductId = product.Id,
                    TagId = tag.TagId
                }).ToList();
        }


        public async Task<PagedResult<ProductShortResponse>> FilterProductsAsync(ProductFilterRequest request)
        {
            _logger.LogInformation("Filtering products for UserId={UserId}, Admin={IsAdmin}",
                request.UserId, request.IsAdmin);

            var userId = _currentUserService.GetUserId();
            request.UserId = userId;

            var result = await _unitOfWork.ProductRepository.FilterProductsAsync(request);

            _logger.LogInformation("Filter return {Count} items", result.Items.Count());

            return result;
        }

        public async Task<ProductResponse> GetProductDetailAsync(Guid id)
        {
            _logger.LogInformation("Getting product detail for {Id}", id);

            var userId = _currentUserService.GetUserId();

            EnsureAuthenticatedUser(userId);

            // Kiểm tra quyền: sản phẩm có thuộc user không
            var exists = await _unitOfWork.ProductRepository.existsProductByUser(userId, id);

            // Nếu user không phải admin và sản phẩm không thuộc user → throw
            if (!exists && !_currentUserService.IsAdmin())
            {
                _logger.LogWarning("User {UserId} attempted to access product {ProductId} without permission", userId, id);
                throw new UnauthorizedAccessException("You do not have permission to access this product.");
            }

            // Lấy product chi tiết
            var product = await _unitOfWork.ProductRepository.GetProductDetailAsync(id);

            if (product == null)
            {
                _logger.LogWarning("Product {Id} not found", id);
                throw new ProductNotFoundException($"Product {id} not found");
            }

            return product;
        }

        public async Task<bool> DeleteProductAsyncByAdmin(Guid id)
        {
            _logger.LogInformation("Admin attempting to delete product {ProductId}", id);

            EnsureAdmin();

            var product = await _unitOfWork.ProductRepository.GetProductByAdmin(id);
            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found for deletion by admin", id);
                throw new ProductNotFoundException($"Product {id} not found");
            }

            _unitOfWork.ProductRepository.DeleteAsyncByAdmin(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product {ProductId} deleted successfully by admin", id);
            return true;
        }

    }
}
