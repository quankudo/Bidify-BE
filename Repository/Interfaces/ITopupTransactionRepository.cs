using bidify_be.Domain.Entities;

namespace bidify_be.Repository.Interfaces
{
    public interface ITopupTransactionRepository
    {
        Task<TopupTransaction?> GetByClientOrderIdAsync(string clientOrderId);

        Task AddAsync(TopupTransaction transaction);
        Task UpdateAsync(TopupTransaction transaction);
    }
}
