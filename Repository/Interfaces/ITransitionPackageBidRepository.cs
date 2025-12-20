using bidify_be.Domain.Entities;
using bidify_be.DTOs.TransitionPackageBid;

namespace bidify_be.Repository.Interfaces
{
    public interface ITransitionPackageBidRepository
    {
        Task<List<TransitionPackageBidResponse>> GetByUserIdAsync(string userId, int skip = 0, int take = 20);
        Task CreateAsync(TransitionPackageBid entity);
    }
}
