namespace bidify_be.DTOs.Category
{
    public class CategoryShortResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }

    public class CategoryResponse : CategoryShortResponse
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
