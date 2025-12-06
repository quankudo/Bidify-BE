namespace bidify_be.Domain.Entities
{
    public class Gift
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public int QuantityBid { get; set; }
        public Guid GiftTypeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
