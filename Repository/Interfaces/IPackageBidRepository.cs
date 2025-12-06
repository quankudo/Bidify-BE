using bidify_be.Domain.Entities;

namespace bidify_be.Repository.Interfaces
{
    public interface IPackageBidRepository
    {
        Task<PackageBid?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id, string title);
        Task<bool> ExistsAsync(string title);
        Task<IEnumerable<PackageBid>> GetAllAsync();
        Task AddAsync(PackageBid packageBid);
        void Update(PackageBid packageBid);
        void Delete(PackageBid packageBid);
        void ToggleActive(PackageBid packageBid);
    }
}
