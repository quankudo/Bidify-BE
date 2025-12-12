using bidify_be.Domain.Enums;

namespace bidify_be.DTOs.Tags
{
    public class TagQueryRequest
    {
        public string? Search {  get; set; }
        public TagType? Type { get; set; }
    }
}
