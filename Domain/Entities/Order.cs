using bidify_be.Domain.Enums;

namespace bidify_be.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid AuctionId { get; set; }
        public string WinnerId { get; set; } = null!;
        public string SellerId { get; set; } = null!;
        public decimal FinalPrice { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? ReceiverName { get; set; } 
        public string? ReceiverPhone { get; set; } 
        public string? ShippingAddress { get; set; } 
        public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;
        public string? CancelReason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ApplicationUser Seller { get; set; } = null!;
        public ApplicationUser Winner { get; set; } = null!;
        public Auction Auction { get; set; } = null!;
    }
}
