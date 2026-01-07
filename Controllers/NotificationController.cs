using bidify_be.Domain.Contracts;
using bidify_be.DTOs.Notification;
using bidify_be.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bidify_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Đánh dấu tất cả thông báo là đã đọc
        /// </summary>
        [HttpPatch("mark-all-read")]
        public async Task<ActionResult<ApiResponse<bool>>> MarkAllAsRead()
        {
            await _notificationService.MarkAllAsReadAsync();

            return Ok(ApiResponse<bool>.SuccessResponse(
                true, "All notifications marked as read"
            ));
        }

        /// <summary>
        /// Đánh dấu một thông báo là đã đọc
        /// </summary>
        [HttpPatch("{id:guid}/mark-read")]
        public async Task<ActionResult<ApiResponse<bool>>> MarkAsRead(Guid id)
        {
            await _notificationService.MarkAsReadAsync(id);

            return Ok(ApiResponse<bool>.SuccessResponse(
                true, "Notification marked as read"
            ));
        }

        /// <summary>
        /// Xóa mềm một thông báo
        /// </summary>
        [HttpPatch("{id:guid}/soft-delete")]
        public async Task<ActionResult<ApiResponse<bool>>> SoftDelete(Guid id)
        {
            await _notificationService.SoftDeleteAsync(id);

            return Ok(ApiResponse<bool>.SuccessResponse(
                true, "Notification deleted"
            ));
        }

        /// <summary>
        /// Xóa mềm tất cả thông báo của user hiện tại
        /// </summary>
        [HttpPatch("soft-delete-all")]
        public async Task<ActionResult<ApiResponse<bool>>> SoftDeleteAll()
        {
            await _notificationService.SoftDeleteAllAsync();

            return Ok(ApiResponse<bool>.SuccessResponse(
                true, "All notifications deleted"
            ));
        }

        /// <summary>
        /// Lấy số lượng thông báo chưa đọc
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount()
        {
            var count = await _notificationService.GetUnreadCountAsync();

            return Ok(ApiResponse<int>.SuccessResponse(
                count, "Unread notification count retrieved successfully"
            ));
        }

        /// <summary>
        /// Lấy danh sách thông báo của user (có phân trang)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<NotificationResponse>>>> GetUserNotifications(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 10
        )
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(skip, take);

            return Ok(ApiResponse<IEnumerable<NotificationResponse>>.SuccessResponse(
                notifications, "Notifications retrieved successfully"
            ));
        }
    }
}
