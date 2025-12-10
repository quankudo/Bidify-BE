using bidify_be.Domain.Contracts;
using bidify_be.DTOs.PackageBid;

namespace bidify_be.Services.Interfaces
{
    public interface IPackageBidService
    {
        public Task<PackageBidResponse> GetByIdAsync(Guid id);
        public Task<PagedResult<PackageBidResponse>> GetAllAsync(PackageBidQueryRequest req);
        public Task<PackageBidResponse> CreateAsync(AddPackageBidRequest request);
        public Task<PackageBidResponse> UpdateAsync(Guid id, UpdatePackageBidRequest request);
        public Task<bool> DeleteAsync(Guid id);
        public Task<bool> ToggleActiveAsync(Guid id);
    }
}
