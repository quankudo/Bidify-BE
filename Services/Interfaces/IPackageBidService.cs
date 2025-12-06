using bidify_be.DTOs.PackageBid;

namespace bidify_be.Services.Interfaces
{
    public interface IPackageBidService
    {
        public Task<PackageBidResponse> GetByIdAsync(Guid id);
        public Task<IEnumerable<PackageBidResponse>> GetAllAsync();
        public Task<PackageBidResponse> CreateAsync(AddPackageBidRequest request);
        public Task<PackageBidResponse> UpdateAsync(Guid id, UpdatePackageBidRequest request);
        public Task<bool> DeleteAsync(Guid id);
        public Task<bool> ToggleActiveAsync(Guid id);
    }
}
