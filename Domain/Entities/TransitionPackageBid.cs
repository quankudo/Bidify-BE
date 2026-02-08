using bidify_be.Domain.Abstractions;

namespace bidify_be.Domain.Entities
{
    public class TransitionPackageBid : EntityBase<Guid>
    {
        public Guid UserId { get; set; }
        public Guid PackageBidId { get; set; }
        public decimal Price { get; set; }
        public int BidCount { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
