using bidify_be.DTOs.Users;

namespace bidify_be.DTOs.BidsHistory
{
    public class BidsHistoryResponse
    {
        public Guid Id { get; set; }
        public UserShortResponse User { get; set; }
        public Guid AuctionId { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
