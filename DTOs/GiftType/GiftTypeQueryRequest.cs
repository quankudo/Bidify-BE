namespace bidify_be.DTOs.GiftType
{
    public class GiftTypeQueryRequest
    {
        public string? Search { get; set; }
        public bool? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
