using bidify_be.Domain.Abstractions;

namespace bidify_be.Domain.Entities
{
    public class AuctionTag : EntityBase<Guid>
    {
        public Guid AuctionId { get; set; }
        public Auction Auction { get; set; }
        public Tag Tag { get; set; }
    }
}
