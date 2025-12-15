namespace bidify_be.DTOs.Category
{
    public class CategoryResponse
    {
        public Guid Id { get; set; } 
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
