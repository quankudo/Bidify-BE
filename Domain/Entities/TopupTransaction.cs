using bidify_be.Domain.Abstractions;
using bidify_be.Domain.Abstractions.Entities;
using bidify_be.Domain.Enums;

namespace bidify_be.Domain.Entities
{
    public class TopupTransaction : EntityBase<Guid>, IDateTracking
    {
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public TopupTransactionsStatus Status { get; set; } = TopupTransactionsStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
        public string ClientOrderId { get; set; }
        public string RequestPayload { get; set; } = string.Empty;
        public string ResponsePayload { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
