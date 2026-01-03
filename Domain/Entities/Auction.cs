using bidify_be.Domain.Enums;

namespace bidify_be.Domain.Entities
{
    public class Auction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public Guid ProductId { get; set; }
        public int BidCount { get; set; } = 0;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public decimal? BuyNowPrice { get; set; }
        public decimal StepPrice { get; set; }
        public decimal StartPrice { get; set; }
        public AuctionStatus Status { get; set; } = AuctionStatus.Pending;
        public string? Note { get; set; }
        public string? WinnerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ApplicationUser User { get; set; }
        public Product Product { get; set; }

        public ICollection<AuctionTag> AuctionTags { get; set; } = new List<AuctionTag>();
    }
}
