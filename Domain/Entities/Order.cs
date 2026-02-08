using bidify_be.Domain.Abstractions;
using bidify_be.Domain.Abstractions.Entities;
using bidify_be.Domain.Enums;

namespace bidify_be.Domain.Entities
{
    public class Order : EntityBase<Guid>, IDateTracking
    {
        public Guid AuctionId { get; set; }
        public Guid WinnerId { get; set; }
        public Guid SellerId { get; set; }
        public decimal FinalPrice { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? ReceiverName { get; set; } 
        public string? ReceiverPhone { get; set; } 
        public string? ShippingAddress { get; set; } 
        public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;
        public string? CancelReason { get; set; }

        public ApplicationUser Seller { get; set; } = null!;
        public ApplicationUser Winner { get; set; } = null!;
        public Auction Auction { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
