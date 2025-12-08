using bidify_be.Domain.Entities;

namespace bidify_be.Repository.Interfaces
{
    public interface IGiftRepository
    {
        Task<IEnumerable<Gift>> GetAllAsync();
        Task<Gift?> GetByIdAsync(Guid id);
        Task CreateAsync(Gift gift);
        Task<bool> ExistsByCodeAsync(string code);
        Task<bool> ExistsByCodeAsync(Guid id, string code);
        void UpdateAsync(Gift gift);
        void DeleteAsync(Gift gift);
        void ToggleActiveAsync(Gift gift);
    }
}
