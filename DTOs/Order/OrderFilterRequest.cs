using bidify_be.Domain.Enums;

namespace bidify_be.DTOs.Order
{
    public class OrderFilterRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Search
        public string? Search { get; set; }   // tìm theo name, id, brand

        // Filters
        public OrderStatus? Status { get; set; }

    }
}
