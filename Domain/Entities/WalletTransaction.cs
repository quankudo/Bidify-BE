using bidify_be.Domain.Enums;

namespace bidify_be.Domain.Entities
{
    public class WalletTransaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public WalletTransactionType Type { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public Guid? ReferenceId { get; set; }
        public string Description {  get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
