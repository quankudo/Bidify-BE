namespace bidify_be.DTOs.Order
{
    public class PaidOrderRequest
    {
        public Guid OrderId { get; set; }
        public Guid AddressId { get; set; }
    }
}
