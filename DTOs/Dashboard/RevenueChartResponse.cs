namespace bidify_be.DTOs.Dashboard
{
    public class RevenueChartResponse
    {
        public string Label { get; set; }     // "08h", "Mon", "W1", "Jan"
        public decimal Topup { get; set; }    // topup
        public decimal Bid { get; set; }      // bid
    }
}
