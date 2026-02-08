using bidify_be.Domain.Abstractions;
using bidify_be.Domain.Enums;

namespace bidify_be.Domain.Entities
{
    public class Notification : EntityBase<Guid>
    {
        public NotificationType NotificationType { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public Guid? RelatedAuctionId { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public ICollection<UserNotification> UserNotifications { get; set; }
    }
}
