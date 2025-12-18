using bidify_be.Domain.Entities;
using bidify_be.DTOs.TransitionPackageBid;

namespace bidify_be.Repository.Interfaces
{
    public interface ITransitionPackageBidRepository
    {
        Task<List<TransitionPackageBidResponse>> GetByUserIdAsync(string userId);
        Task CreateAsync(TransitionPackageBid entity);
    }
}
