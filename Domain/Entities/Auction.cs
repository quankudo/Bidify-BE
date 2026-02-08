using bidify_be.Domain.Abstractions;
using bidify_be.Domain.Enums;

namespace bidify_be.Domain.Entities
{
    public class Auction : EntityAuditBase<Guid>
    {
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
        public ApplicationUser User { get; set; }
        public ApplicationUser Winner { get; set; }
        public Product Product { get; set; }

        public ICollection<AuctionTag> AuctionTags { get; set; } = new List<AuctionTag>();
        public ICollection<BidsHistory> BidsHistories { get; set; } = new List<BidsHistory>();
    }
}
