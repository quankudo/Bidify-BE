using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.PackageBid;

namespace bidify_be.Repository.Interfaces
{
    public interface IPackageBidRepository
    {
        Task<PackageBid?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id, string title);
        Task<bool> ExistsAsync(string title);
        Task<PagedResult<PackageBidResponse>> GetAllAsync(PackageBidQueryRequest req);
        Task AddAsync(PackageBid packageBid);
        void Update(PackageBid packageBid);
        void Delete(PackageBid packageBid);
        void ToggleActive(PackageBid packageBid);
    }
}
