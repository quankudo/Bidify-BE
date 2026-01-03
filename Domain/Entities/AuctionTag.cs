namespace bidify_be.Domain.Entities
{
    public class AuctionTag
    {
        public Guid TagId { get; set; }
        public Guid AuctionId { get; set; }
        public Auction Auction { get; set; }
        public Tag Tag { get; set; }
    }
}
