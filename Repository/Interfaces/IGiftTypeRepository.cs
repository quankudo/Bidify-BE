using bidify_be.Domain.Entities;

namespace bidify_be.Repository.Interfaces
{
    public interface IGiftTypeRepository
    {
        Task<IEnumerable<GiftType>> GetAllAsync();
        Task<GiftType?> GetByIdAsync(Guid id);
        Task<bool> ExistsWithCodeAsync(string code);
        Task<bool> ExistsWithCodeAsync(Guid id, string code);

        Task CreateAsync(GiftType giftType);

        void Update(GiftType giftType);

        void Delete(GiftType giftType);
        void ToggleActive(GiftType giftType);
    }
}
