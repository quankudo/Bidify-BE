using bidify_be.Domain.Abstractions;

namespace bidify_be.Domain.Entities
{
    public class BidsHistory : EntityBase<Guid>
    {
        public Guid UserId { get; set; }
        public Guid AuctionId { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public ApplicationUser User { get; set; }
    }
}
