using bidify_be.Domain.Enums;

namespace bidify_be.DTOs.Product
{
    public class ProductFilterRequest
    {
        // Paging
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Search
        public string? Search { get; set; }   // tìm theo name, id, brand

        // Filters
        public Guid? CategoryId { get; set; }
        public ProductStatus? Status { get; set; }
        public ProductCondition? Condition { get; set; }

        // For normal user (filter theo user)
        public string? UserId { get; set; }
    }

}
