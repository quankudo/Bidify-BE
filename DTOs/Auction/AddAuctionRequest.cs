
namespace bidify_be.DTOs.Auction
{
    public class AddAuctionRequest
    {
        public Guid ProductId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public decimal StepPrice { get; set; }
        public decimal StartPrice { get; set; }
        public ICollection<AuctionTagRequest> Tags { get; set; } = new List<AuctionTagRequest>();
    }

    public class AuctionTagRequest
    {
        public Guid TagId { get; set; }
    }
}
