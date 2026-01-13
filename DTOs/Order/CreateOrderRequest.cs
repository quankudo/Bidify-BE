using bidify_be.Domain.Enums;

namespace bidify_be.DTOs.Order
{
    public class CreateOrderRequest
    {
        public Guid AuctionId { get; set; }
        public string WinnerId { get; set; } = null!;
        public string SellerId { get; set; } = null!;
        public decimal FinalPrice { get; set; }
    }
}
