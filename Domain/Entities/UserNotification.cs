using bidify_be.Domain.Abstractions;
using bidify_be.Domain.Abstractions.Entities;

namespace bidify_be.Domain.Entities
{
    public class UserNotification : EntityBase<Guid>, ISoftDelete
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        public ApplicationUser User { get; set; }
        public Notification Notification { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
    }
}
