using bidify_be.Domain.Enums;

namespace bidify_be.DTOs.Notification
{
    public class NotificationResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public NotificationType NotificationType { get; set; }
        public Guid? RelatedAuctionId { get; set; }
    }
}
