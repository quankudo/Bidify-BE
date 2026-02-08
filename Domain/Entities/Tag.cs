using bidify_be.Domain.Abstractions;
using bidify_be.Domain.Abstractions.Entities;
using bidify_be.Domain.Enums;

namespace bidify_be.Domain.Entities
{
    public class Tag : EntityBase<Guid>, IDateTracking
    {
        public string Title { get; set; } = string.Empty;
        public TagType Type { get; set; }
        public bool Status { get; set; } = true;
        public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
        public ICollection<AuctionTag> AuctionTags { get; set; } = new List<AuctionTag>();
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
