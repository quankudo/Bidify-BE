using bidify_be.Domain.Enums;

namespace bidify_be.Domain.Entities
{
    public class Voucher
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PackageBidId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public VoucherStatus Status { get; set; } = VoucherStatus.Active;
        public Guid VoucherTypeId { get; set; }
        public decimal Discount { get; set; }
        public DiscountType DiscountType { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
