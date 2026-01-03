using bidify_be.Domain.Enums;

namespace bidify_be.DTOs.Auction
{
    public class AuctionQueryRequest
    {
        // SEARCH
        public string? Search { get; set; }

        // FILTER
        public AuctionStatus? Status { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ProductId { get; set; }

        public DateTime? StartFrom { get; set; }
        public DateTime? EndTo { get; set; }

        // SORT
        public string? SortBy { get; set; } = "createdAt";
        public bool IsDescending { get; set; } = true;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
