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

        public ProductServiceImpl(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ILogger<ProductServiceImpl> logger, 
            IValidator<AddProductRequest> validatorAdd,
            IValidator<UpdateProductRequest> validatorUpdate,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _validatorAdd = validatorAdd;
            _validatorUpdate = validatorUpdate;
            _currentUserService = currentUserService;
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

            // 1. Lấy User ID
            var userId = _currentUserService.GetUserId();

            EnsureAuthenticatedUser(userId);

            // 2. Validate
            await ValidateAsync(request, _validatorAdd);

            // 3. Mapping AddProductRequest -> Product
            var product = _mapper.Map<Product>(request);
            product.UserId = userId; // set owner
            product.Status = ProductStatus.Pending;

            // 4. Insert
            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product created successfully with Id: {Id}", product.Id);

            // 5. Map to response
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

            // 1. Validate
            await ValidateAsync(request, _validatorUpdate);

            // 2. Load product + navigation
            var product = await _unitOfWork.ProductRepository.GetProductWithDetailsAsync(id);
            if (product == null)
                throw new ProductNotFoundException($"Product {id} not found");

            // 3. Check permission
            if (product.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to update product {ProductId} but is not the owner", userId, id);
                throw new UnauthorizedAccessException("You do not have permission to update this product.");
            }

            // 4. Map fields đơn giản
            _mapper.Map(request, product);

            UpdateCollections(product, request);

            // 5. Update timestamp
            product.UpdatedAt = DateTime.UtcNow;

            // 6. Save
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

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
