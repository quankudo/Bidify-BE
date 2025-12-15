namespace bidify_be.DTOs.Category
{
    public class UpdateCategoryRequest
    {
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
    }
}
