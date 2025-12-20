using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using bidify_be.DTOs;

namespace bidify_be.Services.Interfaces
{
    public interface IWalletService
    {
        Task CreditAsync(
            ApplicationUser user,
            decimal amount,
            WalletTransactionType type,
            Guid referenceId,
            string description);

        Task<List<WalletTransaction>> GetAllByUserIdAsync(WalletTransactionQuery req);
    }

}
