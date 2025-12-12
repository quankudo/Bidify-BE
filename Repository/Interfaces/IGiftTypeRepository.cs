using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.GiftType;

namespace bidify_be.Repository.Interfaces
{
    public interface IGiftTypeRepository
    {
        Task<PagedResult<GiftTypeResponse>> GetAllAsync(GiftTypeQueryRequest req);
        Task<GiftType?> GetByIdAsync(Guid id);
        Task<bool> ExistsWithCodeAsync(string code);
        Task<bool> ExistsWithCodeAsync(Guid id, string code);

        Task CreateAsync(GiftType giftType);

        void Update(GiftType giftType);

        void Delete(GiftType giftType);
        void ToggleActive(GiftType giftType);
    }
}
