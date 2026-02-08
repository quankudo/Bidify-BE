using bidify_be.Domain.Abstractions;
using bidify_be.Domain.Abstractions.Entities;
using bidify_be.Domain.Enums;

namespace bidify_be.Domain.Entities
{
    public class Product : EntityBase<Guid>, IDateTracking
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public string? Brand { get; set; }
        public ProductStatus Status { get; set; }
        public ProductCondition Condition { get; set; }
        public string Thumbnail {  get; set; } = string.Empty;
        public string ThumbnailPublicId { get; set; } = string.Empty;
        public string? Note { get; set; }

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
        public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
