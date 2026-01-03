namespace bidify_be.DTOs.Auction
{
    public class PlaceBidRequest
    {
        public Guid AuctionId { get; set; }
        public decimal BidPrice { get; set; }
    }
}
