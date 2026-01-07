using bidify_be.Domain.Enums;

namespace bidify_be.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public NotificationType NotificationType { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public Guid? RelatedAuctionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserNotification> UserNotifications { get; set; }
    }
}
