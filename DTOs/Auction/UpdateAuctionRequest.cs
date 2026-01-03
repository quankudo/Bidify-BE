namespace bidify_be.DTOs.Auction
{
    public class UpdateAuctionRequest
    {
        public Guid ProductId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public decimal StepPrice { get; set; }
        public decimal StartPrice { get; set; }
        public ICollection<AuctionTagRequest> Tags { get; set; } = new List<AuctionTagRequest>();
    }
}
