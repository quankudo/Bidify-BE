using bidify_be.Domain.Enums;

namespace bidify_be.DTOs.Voucher
{
    public class VoucherResponse
    {
        public Guid Id { get; set; }
        public Guid PackageBidId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public VoucherStatus Status { get; set; } = VoucherStatus.Active;
        public Guid VoucherTypeId { get; set; }
        public decimal Discount { get; set; }
        public DiscountType DiscountType { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
