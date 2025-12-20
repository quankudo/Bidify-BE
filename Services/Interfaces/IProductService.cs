using bidify_be.Domain.Contracts;
using bidify_be.DTOs.Product;

namespace bidify_be.Services.Interfaces
{
    public interface IProductService
    {
        // Create
        Task<ProductResponse> AddProductAsync(AddProductRequest request);
        Task<ProductResponse> GetProductDetailAsync(Guid id);

        // Delete
        Task<bool> DeleteProductAsyncByUser(Guid id);
        Task<bool> DeleteProductAsyncByAdmin(Guid id);

        Task<bool> ApproveProductAsync(Guid id);
        Task<bool> RejectProductAsync(RejectProductRequest req);

        Task<ProductResponse> UpdateProductAsync(Guid id, UpdateProductRequest request);

        Task<PagedResult<ProductShortResponse>> FilterProductsAsync(ProductFilterRequest request);
    }
}
