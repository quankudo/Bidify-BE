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
using Microsoft.EntityFrameworkCore;
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
                    await _fileStorageService.MarkAsUsedAsync(request.ThumbnailPublicId);

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
                    var oldPublicIds = product.Images
                        .Where(x => !string.IsNullOrEmpty(x.PublicId))
                        .Select(x => x.PublicId!)
                        .ToHashSet();

                    var newPublicIds = request.Images
                        .Where(x => !string.IsNullOrEmpty(x.PublicId))
                        .Select(x => x.PublicId!)
                        .ToHashSet();

                    foreach (var publicId in newPublicIds.Except(oldPublicIds))
                    {
                        await _fileStorageService.MarkAsUsedAsync(publicId);
                    }

                    foreach (var publicId in oldPublicIds.Except(newPublicIds))
                    {
                        await _fileStorageService.RequestDeleteAsync(publicId);
                    }

                    UpdateImages(product, request);
                }

                /* ===================== ATTRIBUTES ===================== */
                if (request.Attributes != null)
                {
                    UpdateAttributes(product, request);
                }

                /* ===================== TAGS ===================== */
                if (request.Tags != null)
                {
                    UpdateTags(product, request);
                }

                /* ===================== SIMPLE FIELDS ===================== */
                _mapper.Map(request, product);

                product.UserId = userId;
                product.Status = ProductStatus.Pending;
                product.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Concurrency error when updating product {Id}", id);
                throw new ProductNotFoundException("Product has been modified or deleted.");
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


        private void UpdateImages(Product product, UpdateProductRequest request)
        {
            var reqImages = request.Images ?? [];

            var dbByPublicId = product.Images
                .Where(x => !string.IsNullOrEmpty(x.PublicId))
                .ToDictionary(x => x.PublicId!);

            var reqPublicIds = reqImages
                .Where(x => !string.IsNullOrEmpty(x.PublicId))
                .Select(x => x.PublicId!)
                .ToHashSet();

            // REMOVE
            foreach (var img in product.Images.ToList())
            {
                if (string.IsNullOrEmpty(img.PublicId) || !reqPublicIds.Contains(img.PublicId))
                {
                    product.Images.Remove(img);
                }
            }

            // ADD / UPDATE
            foreach (var req in reqImages)
            {
                if (dbByPublicId.TryGetValue(req.PublicId!, out var existing))
                {
                    existing.ImageUrl = req.ImageUrl;
                }
                else
                {
                    product.Images.Add(new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ImageUrl = req.ImageUrl,
                        PublicId = req.PublicId,
                        ProductId = product.Id
                    });
                }
            }
        }

        private void UpdateAttributes(Product product, UpdateProductRequest request)
        {
            var reqAttrs = request.Attributes ?? [];

            var dbByKey = product.Attributes
                .ToDictionary(x => x.Key);

            var reqKeys = reqAttrs
                .Select(x => x.Key)
                .ToHashSet();

            foreach (var attr in product.Attributes.ToList())
            {
                if (!reqKeys.Contains(attr.Key))
                {
                    product.Attributes.Remove(attr);
                }
            }

            foreach (var req in reqAttrs)
            {
                if (dbByKey.TryGetValue(req.Key, out var existing))
                {
                    existing.Value = req.Value;
                }
                else
                {
                    product.Attributes.Add(new ProductAttribute
                    {
                        Id = Guid.NewGuid(),
                        Key = req.Key,
                        Value = req.Value,
                        ProductId = product.Id
                    });
                }
            }
        }

        private void UpdateTags(Product product, UpdateProductRequest request)
        {
            var reqTagIds = request.Tags?
                .Select(x => x.TagId)
                .ToHashSet() ?? [];

            var dbTagIds = product.ProductTags
                .Select(x => x.TagId)
                .ToHashSet();

            foreach (var tag in product.ProductTags.ToList())
            {
                if (!reqTagIds.Contains(tag.TagId))
                {
                    product.ProductTags.Remove(tag);
                }
            }

            foreach (var tagId in reqTagIds)
            {
                if (!dbTagIds.Contains(tagId))
                {
                    product.ProductTags.Add(new ProductTag
                    {
                        ProductId = product.Id,
                        TagId = tagId
                    });
                }
            }
        }



        public async Task<PagedResult<ProductShortResponse>> FilterProductsAsync(ProductFilterRequest request)
        {
            _logger.LogInformation("Filtering products for UserId={UserId}",
                request.UserId);
            var userId = _currentUserService.GetUserId();
            request.UserId = userId;

            var result = await _unitOfWork.ProductRepository.FilterProductsAsync(request);

            _logger.LogInformation("Filter return {Count} items", result.Items.Count());

            return result;
        }

        public async Task<PagedResult<ProductForTableResponse>> FilterProductsForAdminAsync(ProductFilterRequest request)
        {
            _logger.LogInformation(
                "Admin filtering products. Page={PageNumber}, Size={PageSize}, Status={Status}, Category={CategoryId}",
                request.PageNumber,
                request.PageSize,
                request.Status,
                request.CategoryId);

            var result = await _unitOfWork.ProductRepository
                .FilterProductsForAdminAsync(request);

            _logger.LogInformation(
                "Admin filter returned {Count} items (Total={TotalItems})",
                result.Items.Count(),
                result.TotalItems);

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

        public async Task<bool> ApproveProductAsync(Guid id)
        {
            _logger.LogInformation("Approving product {ProductId}", id);

            var product = await _unitOfWork.ProductRepository.GetProductByAdmin(id);
            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found for approval", id);
                throw new ProductNotFoundException("Product not found");
            }

            if (product.Status != ProductStatus.Pending)
            {
                _logger.LogWarning(
                    "Invalid status transition for product {ProductId}. Current: {Status}",
                    id, product.Status);

                throw new InvalidProductStateException("Invalid product status");
            }


            if (product.Status == ProductStatus.Active)
            {
                _logger.LogInformation(
                    "Product {ProductId} is already approved (Status = Active)", id);
                return true;
            }

            product.Status = ProductStatus.Active;

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Product {ProductId} approved successfully", id);

            return true;
        }


        public async Task<bool> RejectProductAsync(RejectProductRequest req)
        {
            _logger.LogInformation(
                "Rejecting product {ProductId} with reason: {Reason}",
                req.Id, req.Reason);

            var product = await _unitOfWork.ProductRepository.GetProductByAdmin(req.Id);
            if (product == null)
            {
                _logger.LogWarning(
                    "Product {ProductId} not found for rejection", req.Id);
                throw new ProductNotFoundException("Product not found");
            }

            if (product.Status != ProductStatus.Pending)
            {
                _logger.LogWarning(
                    "Invalid status transition for product {ProductId}. Current: {Status}",
                    product.Id, product.Status);

                throw new InvalidProductStateException("Invalid product status");
            }


            if (product.Status == ProductStatus.Cancelled)
            {
                _logger.LogInformation(
                    "Product {ProductId} already rejected (Status = Cancelled)", req.Id);
                return true;
            }

            product.Status = ProductStatus.Cancelled;
            product.Note = req.Reason;

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Product {ProductId} rejected successfully", req.Id);

            return true;
        }

        public async Task<List<ProductShortResponseForList>> GetProductShortListAsync()
        {
            var userId = _currentUserService.GetUserId();
            EnsureAuthenticatedUser(userId);

            return await _unitOfWork.ProductRepository.GetProductShortListAsync(userId);
        }
    }
}
