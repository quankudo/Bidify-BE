using bidify_be.Domain.Contracts;
using bidify_be.DTOs.Gift;

namespace bidify_be.Services.Interfaces
{
    public interface IGiftService
    {
        Task<GiftResponse> GetByIdAsync(Guid id);
        //Task<IEnumerable<GiftResponse>> GetAllAsync();
        Task<PagedResult<GiftResponse>> SearchAsync(GiftQueryRequest req);
        Task<GiftResponse> CreateAsync(AddGiftRequest request);
        Task<GiftResponse> UpdateAsync(Guid id, UpdateGiftRequest request);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ToggleActiveAsync(Guid id);
    }
}
