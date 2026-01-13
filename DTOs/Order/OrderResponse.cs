using bidify_be.Domain.Enums;
using bidify_be.DTOs.Product;
using bidify_be.DTOs.Users;

namespace bidify_be.DTOs.Order
{
    public class OrderResponse
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
        public OrderStatus Status { get; set; }
        public string? CancelReason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ProductShortForOrderResponse Product { get; set; }
    }

    public class OrderResponseForAdmin : OrderResponse
    {
        public UserShortResponse Seller { get; set; } = null!;
        public UserShortResponse Winner { get; set; } = null!;
    }

    public class OrderResponseForSeller : OrderResponse
    {
        public UserShortResponse Winner { get; set; } = null!;
    }

    public class OrderResponseForWinner : OrderResponse
    {
        public UserShortResponse Seller { get; set; } = null!;
    }
}
