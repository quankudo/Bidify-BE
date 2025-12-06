using bidify_be.Domain.Enums;

namespace bidify_be.DTOs.Tags
{
    public class AddTagRequest
    {
        public string Title { get; set; } = string.Empty;
        public TagType Type { get; set; }
    }
}
