namespace bidify_be.DTOs.PackageBid
{
    public class PackageBidShortResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string BgColor { get; set; } = string.Empty;
        public int BidQuantity { get; set; }
    }

    public class PackageBidResponse : PackageBidShortResponse
    {
        public decimal Price { get; set; }
        public bool Status { get; set; }
    }
}
