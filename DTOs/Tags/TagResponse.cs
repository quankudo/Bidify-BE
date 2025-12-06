namespace bidify_be.DTOs.Tags
{
    public class TagResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
