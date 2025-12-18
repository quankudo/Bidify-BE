using bidify_be.Domain.Enums;

namespace bidify_be.DTOs.Topup
{
    public class CreateTopupRequest
    {
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
