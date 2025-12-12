using bidify_be.Domain.Enums;

namespace bidify_be.DTOs.Tags
{
    public class TagResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public TagType Type { get; set; }
        public bool Status { get; set; }
    }
}
