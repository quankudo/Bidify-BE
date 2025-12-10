using bidify_be.Domain.Enums;

namespace bidify_be.DTOs.Voucher
{
    public class VoucherQueryRequest
    {
        public string? Search { get; set; }           // code + description

        public VoucherStatus? Status { get; set; }    // filter
        public string? PackageBidTitle { get; set; }  // filter
        public string? GiftTypeCode { get; set; }     // filter

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
