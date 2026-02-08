using bidify_be.Domain.Abstractions;
using bidify_be.Domain.Abstractions.Entities;

namespace bidify_be.Domain.Entities
{
    public class Category : EntityBase<Guid>, IDateTracking
    {
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}