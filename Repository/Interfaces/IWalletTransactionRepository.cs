using bidify_be.Domain.Entities;
using bidify_be.DTOs;

namespace bidify_be.Repository.Interfaces
{
    public interface IWalletTransactionRepository
    {
        Task AddAsync(WalletTransaction transaction);
        Task<List<WalletTransaction>> GetAllByUserIdAsync(WalletTransactionQuery req, string userId);
    }
}
