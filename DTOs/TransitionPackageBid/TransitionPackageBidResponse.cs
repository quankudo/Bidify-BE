namespace bidify_be.DTOs.TransitionPackageBid
{
    public class TransitionPackageBidResponse
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public Guid PackageBidId { get; set; }
        public decimal Price { get; set; }
        public int BidCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Title { get; set; } = string.Empty;
        public string BgColor { get; set; } = string.Empty;
    }
}
