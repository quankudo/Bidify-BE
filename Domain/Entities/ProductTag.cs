namespace bidify_be.Domain.Entities
{
    public class ProductTag
    {
        public Guid TagId { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public Tag Tag { get; set; }
    }
}
