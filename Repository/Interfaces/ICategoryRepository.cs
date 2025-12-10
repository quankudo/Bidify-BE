using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Category;

namespace bidify_be.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(Guid id);
        Task<PagedResult<CategoryResponse>> GetAllAsync(CategoryQueryRequest req);
        Task<bool> ExistsAsyncByName(string title);
        Task<bool> ExistsAsyncById(Guid id);
        Task<bool> ExistsAsyncByName(string title, Guid id);
        Task AddAsync(Category category);
        void Update(Category category);
        void Delete(Category category);
        void ToggleActive(Category category);
    }
}
