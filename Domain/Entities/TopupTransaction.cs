using bidify_be.Domain.Enums;

namespace bidify_be.Domain.Entities
{
    public class TopupTransaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public TopupTransactionsStatus Status { get; set; } = TopupTransactionsStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
        public string ClientOrderId { get; set; }
        public string RequestPayload { get; set; } = string.Empty;
        public string ResponsePayload { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
