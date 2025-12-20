using bidify_be.Domain.Enums;

namespace bidify_be.DTOs.Topup
{
    public class TopupRequestQuery
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 20;
        public PaymentMethod? PaymentMethod { get; set; }
        public TopupTransactionsStatus? Status { get; set; }
    }
}
