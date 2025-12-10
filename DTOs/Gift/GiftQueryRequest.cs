namespace bidify_be.DTOs.Gift
{
    public class GiftQueryRequest
    {
        public string? Search { get; set; }   // gom code + description + quantityBid

        public bool? Status { get; set; }
        public string? GiftTypeCode { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
