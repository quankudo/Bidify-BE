using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Product;

namespace bidify_be.Repository.Interfaces
{
    public interface IProductRepository
    {
        // Create
        Task AddAsync(Product product);

        // Get by id (kèm include cho detail)
        Task<Product?> GetProductByUser(string userId, Guid id);

        Task<List<ProductShortResponseForList>> GetProductShortListAsync(string userId);

        Task<Product?> GetProductByAdmin(Guid id);


        // Delete
        void DeleteAsyncByUser(Product product);
        void DeleteAsyncByAdmin(Product product);

        Task<Product?> GetProductWithDetailsAsync(Guid id);

        void Update(Product product);

        Task<PagedResult<ProductShortResponse>> FilterProductsAsync(ProductFilterRequest request);
        Task<PagedResult<ProductForTableResponse>> FilterProductsForAdminAsync(ProductFilterRequest request);
        

        Task<ProductResponse?> GetProductDetailAsync(Guid id);

        Task<bool> existsProductByUser(string userId, Guid id);
    }
}
