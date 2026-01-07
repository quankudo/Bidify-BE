using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using bidify_be.DTOs.Notification;

namespace bidify_be.Services.Interfaces
{
    public interface INotificationService
    {
        Task<List<UserNotification>> SendAsync(
            NotificationType type,
            string title,
            string message,
            IEnumerable<string> userIds,
            Guid? relatedAuctionId = null
        );

        Task<List<UserNotification>> SendWithSeparateTransactionAsync(
            NotificationType type,
            string title,
            string message,
            IEnumerable<string> userIds,
            Guid? relatedAuctionId = null);

        Task MarkAsReadAsync(Guid userNotificationId);
        Task MarkAllAsReadAsync();

        Task SoftDeleteAsync(Guid userNotificationId);

        Task SoftDeleteAllAsync();

        Task<int> GetUnreadCountAsync();

        Task<IEnumerable<NotificationResponse>> GetUserNotificationsAsync(
            int skip,
            int take
        );
    }
}
