namespace bidify_be.DTOs.Dashboard
{
    public class ChartResponse
    {
        public List<CategoryChartResponse> CategoryChart { get; set; }
        public List<RevenueChartResponse> RevenueChart { get; set; }
        public List<AuctionHotChartResponse> AuctionHotChart { get; set; }
        public List<AuctionCompletionStat> AuctionCompletionStats { get; set; }
    }
}
