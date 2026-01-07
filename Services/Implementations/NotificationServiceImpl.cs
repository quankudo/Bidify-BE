using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using bidify_be.DTOs.Notification;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Services.Interfaces;

namespace bidify_be.Services.Implementations
{
    public class NotificationServiceImpl : INotificationService
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUserService;

        public NotificationServiceImpl(IUnitOfWork uow, ICurrentUserService currentUserService)
        {
            _uow = uow;
            _currentUserService = currentUserService;
        }

        /* -------------------- SEND -------------------- */

        //Dùng chung transaction với transaction chính
        public async Task<List<UserNotification>> SendAsync(
            NotificationType type,
            string title,
            string message,
            IEnumerable<string> userIds,
            Guid? relatedAuctionId = null)
        {
            if (userIds == null || !userIds.Any())
                return new List<UserNotification>();

            var notification = new Notification
            {
                NotificationType = type,
                Title = title,
                Message = message,
                RelatedAuctionId = relatedAuctionId
            };

            await _uow.NotificationRepository.AddNotificationAsync(notification);

            var userNotifications = userIds.Select(userId => new UserNotification
            {
                NotificationId = notification.Id,
                UserId = userId,
                IsRead = false
            }).ToList();

            await _uow.NotificationRepository.AddUserNotificationsAsync(userNotifications);
            return userNotifications;
        }

        //Dùng transaction riêng để gửi notification, tránh bị rollback khi có lỗi ở transaction chính
        public async Task<List<UserNotification>> SendWithSeparateTransactionAsync(
            NotificationType type,
            string title,
            string message,
            IEnumerable<string> userIds,
            Guid? relatedAuctionId = null)
        {
            if (userIds == null || !userIds.Any())
                return [];

            // Bắt đầu transaction riêng
            await using var transaction = await _uow.BeginTransactionAsync();
            try
            {
                // 1. Tạo Notification
                var notification = new Notification
                {
                    NotificationType = type,
                    Title = title,
                    Message = message,
                    RelatedAuctionId = relatedAuctionId
                };
                await _uow.NotificationRepository.AddNotificationAsync(notification);

                // 2. Tạo UserNotifications
                var userNotifications = userIds.Select(userId => new UserNotification
                {
                    NotificationId = notification.Id,
                    UserId = userId,
                    IsRead = false,
                }).ToList();
                await _uow.NotificationRepository.AddUserNotificationsAsync(userNotifications);

                // 3. Commit transaction riêng
                await _uow.SaveChangesAsync();
                await transaction.CommitAsync();

                return userNotifications;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }



        /* -------------------- READ -------------------- */

        public async Task<IEnumerable<NotificationResponse>> GetUserNotificationsAsync(
            int skip,
            int take)
        {
            var userId = _currentUserService.GetUserId();
            var result = await _uow.NotificationRepository
                .GetUserNotificationsAsync(skip, take, userId);

            return result.Select(un => new NotificationResponse
            {
                Id = un.Id,
                Title = un.Notification.Title,
                Message = un.Notification.Message,
                CreatedAt = un.Notification.CreatedAt,
                IsRead = un.IsRead,
                NotificationType = un.Notification.NotificationType,
                RelatedAuctionId = un.Notification.RelatedAuctionId,
            }).ToList();
        }

        public async Task<int> GetUnreadCountAsync()
        {
            var userId = _currentUserService.GetUserId();
            return await _uow.NotificationRepository.CountUnreadAsync(userId);
        }

        /* -------------------- UPDATE -------------------- */

        public async Task MarkAllAsReadAsync()
        {
            var userId = _currentUserService.GetUserId();
            await _uow.NotificationRepository.MarkAllAsReadAsync(userId);
            await _uow.SaveChangesAsync();
        }

        public async Task MarkAsReadAsync(Guid userNotificationId)
        {
            var userId = _currentUserService.GetUserId();

            await _uow.NotificationRepository
                .MarkAsReadAsync(userNotificationId, userId);

            await _uow.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Guid userNotificationId)
        {
            var userId = _currentUserService.GetUserId();

            await _uow.NotificationRepository
                .SoftDeleteAsync(userNotificationId, userId);

            await _uow.SaveChangesAsync();
        }

        public async Task SoftDeleteAllAsync()
        {
            var userId = _currentUserService.GetUserId();

            await _uow.NotificationRepository
                .SoftDeleteAllAsync(userId);

            await _uow.SaveChangesAsync();
        }
    }
}
