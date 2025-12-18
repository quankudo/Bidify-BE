namespace bidify_be.Domain.Entities
{
    public class TransitionPackageBid
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public Guid PackageBidId { get; set; }
        public decimal Price { get; set; }
        public int BidCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
