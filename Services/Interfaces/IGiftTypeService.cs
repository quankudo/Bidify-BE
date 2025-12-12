using bidify_be.Domain.Contracts;
using bidify_be.DTOs.GiftType;

namespace bidify_be.Services.Interfaces
{
    public interface IGiftTypeService
    {
        Task<PagedResult<GiftTypeResponse>> GetAllAsync(GiftTypeQueryRequest req);
        Task<GiftTypeResponse> GetByIdAsync(Guid id);
        Task<GiftTypeResponse> CreateAsync(AddGiftTypeRequest request);
        Task<GiftTypeResponse> UpdateAsync(Guid id, UpdateGiftTypeRequest request);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ToggleActiveAsync(Guid id);
    }
}
