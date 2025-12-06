using bidify_be.Domain.Entities;

namespace bidify_be.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(Guid id);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<bool> ExistsAsyncByName(string title);
        Task<bool> ExistsAsyncById(Guid id);
        Task<bool> ExistsAsyncByName(string title, Guid id);
        Task AddAsync(Category category);
        void Update(Category category);
        void Delete(Category category);
        void ToggleActive(Category category);
    }
}
