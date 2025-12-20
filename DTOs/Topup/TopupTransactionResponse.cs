using bidify_be.Domain.Enums;

namespace bidify_be.DTOs.Topup
{
    public class TopupTransactionResponse
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal Amount { get; set; }
        public TopupTransactionsStatus Status { get; set; } = TopupTransactionsStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
