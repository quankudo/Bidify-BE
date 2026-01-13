using bidify_be.Domain.Enums;
using bidify_be.DTOs.Dashboard;

namespace bidify_be.Repository.Interfaces
{
    public interface IDashboardRepository
    {
        // Stat Cards
        public Task<ListStatCardResponse> GetListStatCardAsync(DateTime start, DateTime end, DateTime prevStart, DateTime prevEnd, TimeRange range);
        // Charts
        public Task<List<RevenueChartResponse>> GetRevenueChartAsync(DateTime start, DateTime end, TimeRange range);
        public Task<List<CategoryChartResponse>> GetCategoryChartAsync(DateTime start, DateTime end, TimeRange range);
        public Task<List<AuctionHotChartResponse>> GetAuctionHotChartAsync(DateTime start, DateTime end, TimeRange range);
        public Task<List<AuctionCompletionStat>> GetAuctionCompletionStatsAsync(DateTime start, DateTime end, TimeRange range);
        // Table without filter
        public Task<List<PendingAuctionTableResponse>> GetPendingAuctionTableNoFilterAsync();
        public Task<List<PendingProductTableResponse>> GetPendingProductTableNoFilterAsync();
        public Task<List<PendingDisputeTableResponse>> GetPendingDisputeTableNoFilterAsync();
        public Task<int> CountPendingAuctionsAsync();
        public Task<int> CountPendingProductsAsync();
        public Task<int> CountPendingDisputesAsync();

        // Table with filter
        public Task<List<TopAuctionParticipantTableResponse>> GetTopAuctionParticipantTable(DateTime start, DateTime end, TimeRange range);

    }
}
