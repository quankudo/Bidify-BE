using bidify_be.Domain.Abstractions;

namespace bidify_be.Domain.Entities
{
    public class ProductImage : EntityBase<Guid>
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
    }
}
