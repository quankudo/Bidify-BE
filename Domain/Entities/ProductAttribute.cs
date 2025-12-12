namespace bidify_be.Domain.Entities
{
    public class ProductAttribute
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
    }
}
