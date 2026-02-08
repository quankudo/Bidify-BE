using bidify_be.Domain.Abstractions;

namespace bidify_be.Domain.Entities
{
    public class ProductAttribute : EntityBase<Guid>
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
    }
}
