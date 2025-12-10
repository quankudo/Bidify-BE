using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Gift;

namespace bidify_be.Repository.Interfaces
{
    public interface IGiftRepository
    {
        //Task<IEnumerable<GiftResponse>> GetAllAsync();
        Task<PagedResult<GiftResponse>> SearchAsync(GiftQueryRequest req);
        Task<Gift?> GetByIdAsync(Guid id);
        Task<GiftResponse?> GetByIdAsyncResponse(Guid id);
        Task CreateAsync(Gift gift);
        Task<bool> ExistsByCodeAsync(string code);
        Task<bool> ExistsByCodeAsync(Guid id, string code);
        void UpdateAsync(Gift gift);
        void DeleteAsync(Gift gift);
        void ToggleActiveAsync(Gift gift);
    }
}
