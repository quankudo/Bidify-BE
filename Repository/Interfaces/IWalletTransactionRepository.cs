using bidify_be.Domain.Entities;

namespace bidify_be.Repository.Interfaces
{
    public interface IWalletTransactionRepository
    {
        Task AddAsync(WalletTransaction transaction);
    }
}
