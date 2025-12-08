using bidify_be.Domain.Enums;

namespace bidify_be.DTOs.Voucher
{
    public class UpdateVoucherRequest
    {
        public Guid PackageBidId { get; set; }
        public string Description { get; set; } = string.Empty;
        public VoucherStatus Status { get; set; }
        public Guid VoucherTypeId { get; set; }
        public decimal Discount { get; set; }
        public DiscountType DiscountType { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
