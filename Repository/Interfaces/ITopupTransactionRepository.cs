using bidify_be.Domain.Entities;
using bidify_be.DTOs.Topup;

namespace bidify_be.Repository.Interfaces
{
    public interface ITopupTransactionRepository
    {
        Task<TopupTransaction?> GetByClientOrderIdAsync(string clientOrderId);

        Task AddAsync(TopupTransaction transaction);
        Task UpdateAsync(TopupTransaction transaction);
        Task<List<TopupTransactionResponse>> GetAllByUserIdAsync(string userId, TopupRequestQuery req);
    }
}
