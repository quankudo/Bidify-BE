namespace bidify_be.DTOs.BidsHistory
{
    public class CreateBidsHistoryRequest
    {
        public string UserId { get; set; }
        public Guid AuctionId { get; set; }
        public decimal Price { get; set; }
    }
}
