using bidify_be.Domain.Entities;
using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bidify_be.Repository.Implementations
{
    public class NotificationRepositoryImpl : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepositoryImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        /* -------------------- CREATE -------------------- */

        public async Task AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        public async Task AddUserNotificationsAsync(IEnumerable<UserNotification> userNotifications)
        {
            await _context.UserNotifications.AddRangeAsync(userNotifications);
        }

        /* -------------------- READ -------------------- */

        public async Task<int> CountUnreadAsync(string userId)
        {
            return await _context.UserNotifications
                .AsNoTracking()
                .CountAsync(x =>
                    x.UserId == userId &&
                    !x.IsRead &&
                    !x.IsDeleted
                );
        }

        public async Task<IEnumerable<UserNotification>> GetUserNotificationsAsync(
            int skip,
            int take,
            string userId
        )
        {
            return await _context.UserNotifications
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId &&
                    !x.IsDeleted
                )
                .Include(x => x.Notification)
                .OrderByDescending(x => x.Notification.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        /* -------------------- UPDATE -------------------- */

        public async Task MarkAsReadAsync(Guid userNotificationId, string userId)
        {
            var entity = await _context.UserNotifications
                .FirstOrDefaultAsync(x => x.Id == userNotificationId && x.UserId == userId);

            if (entity == null || entity.IsRead)
                return;

            entity.IsRead = true;
            entity.ReadAt = DateTime.UtcNow;
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            await _context.UserNotifications
                .Where(x =>
                    x.UserId == userId &&
                    !x.IsRead &&
                    !x.IsDeleted
                )
                .ExecuteUpdateAsync(setters =>
                    setters
                        .SetProperty(n => n.IsRead, true)
                        .SetProperty(n => n.ReadAt, DateTime.UtcNow)
                );
        }


        /* -------------------- DELETE (SOFT) -------------------- */

        public async Task SoftDeleteAsync(Guid userNotificationId, string userId)
        {
            var entity = await _context.UserNotifications
                .FirstOrDefaultAsync(x => x.Id == userNotificationId && x.UserId == userId);

            if (entity == null || entity.IsDeleted)
                return;

            entity.IsDeleted = true;
        }

        public async Task SoftDeleteAllAsync(string userId)
        {
            await _context.UserNotifications
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(n => n.IsDeleted, true)
                );
        }
    }
}
