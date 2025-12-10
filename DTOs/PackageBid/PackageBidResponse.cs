namespace bidify_be.DTOs.PackageBid
{
    public class PackageBidResponse
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public int BidQuantity { get; set; }
        public string BgColor { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool Status { get; set; }
    }

    public class PackageBidShortResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string BgColor { get; set; } = string.Empty;
        public int BidQuantity { get; set; }
    }
}
