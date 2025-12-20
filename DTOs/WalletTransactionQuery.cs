using bidify_be.Domain.Enums;

namespace bidify_be.DTOs
{
    public class WalletTransactionQuery
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 20;
        public WalletTransactionType? Type { get; set; }
    }
}
