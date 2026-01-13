using bidify_be.Domain.Entities;

namespace bidify_be.Repository.Interfaces
{
    public interface IBidsHistoryRepository
    {
        Task<BidsHistory> AddBidsHistoryAsync(BidsHistory bidsHistory);
        Task<List<BidsHistory>> GetBidsHistoriesByAuctionIdAsync(Guid auctionId, int skip, int take);
        Task<List<BidsHistory>> GetBidsHistoriesByUserIdAsync(string userId, int skip, int take);
    }
}
