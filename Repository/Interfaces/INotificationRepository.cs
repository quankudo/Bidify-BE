using bidify_be.Domain.Entities;

namespace bidify_be.Repository.Interfaces
{
    public interface INotificationRepository
    {
        // Notification
        Task AddNotificationAsync(Notification notification);

        // UserNotification
        Task AddUserNotificationsAsync(IEnumerable<UserNotification> userNotifications);

        Task<IEnumerable<UserNotification>> GetUserNotificationsAsync(
            int skip, int take, string userId
        );

        Task<int> CountUnreadAsync(string userId);

        Task MarkAsReadAsync(Guid userNotificationId, string userId);

        Task MarkAllAsReadAsync(string userId);

        Task SoftDeleteAsync(Guid userNotificationId, string userId);

        Task SoftDeleteAllAsync(string userId);
    }
}
