namespace bidify_be.DTOs.PackageBid
{
    public class AddPackageBidRequest
    {
        public decimal Price { get; set; }
        public int BidQuantity { get; set; }
        public string BgColor { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
}
