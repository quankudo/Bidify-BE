namespace bidify_be.Domain.Entities
{
    public class UserNotification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid NotificationId { get; set; }
        public string UserId { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        public ApplicationUser User { get; set; }
        public Notification Notification { get; set; }
    }
}
