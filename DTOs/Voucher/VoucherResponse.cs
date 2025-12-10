using bidify_be.Domain.Enums;
using bidify_be.DTOs.GiftType;
using bidify_be.DTOs.PackageBid;

namespace bidify_be.DTOs.Voucher
{
    public class VoucherResponse
    {
        public Guid Id { get; set; }
        public PackageBidShortResponse PackageBid { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public VoucherStatus Status { get; set; } = VoucherStatus.Active;
        public GiftTypeShortResponse VoucherType { get; set; }
        public decimal Discount { get; set; }
        public DiscountType DiscountType { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
