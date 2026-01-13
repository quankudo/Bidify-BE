using bidify_be.Domain.Entities;
using bidify_be.DTOs.BidsHistory;

namespace bidify_be.Services.Interfaces
{
    public interface IBidsHistoryService
    {
        Task<BidsHistory> CreateBidsHistoryAsync(CreateBidsHistoryRequest request);
        Task<List<BidsHistoryResponse>> GetBidsHistoriesByAuctionIdAsync(Guid auctionId, int skip, int take);
        Task<List<BidsHistoryResponse>> GetBidsHistoriesByUserIdAsync(int skip, int take);
    }
}
