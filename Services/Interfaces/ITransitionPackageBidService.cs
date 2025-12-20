using bidify_be.Domain.Entities;
using bidify_be.DTOs.TransitionPackageBid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bidify_be.Services.Interfaces
{
    public interface ITransitionPackageBidService
    {
        Task<TransitionPackageBid> CreateAsync(TransitionPackageBidRequest request);
        Task<List<TransitionPackageBidResponse>> GetAllByUserIdAsync(TransitionPackageBidQuery req);
    }
}
