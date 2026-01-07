using bidify_be.Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
namespace bidify_be.Hubs
{
    public class AppHub : Hub
    {
        private const string AdminGroup = "Admins";

        public override async Task OnConnectedAsync()
        {
            if (Context.User?.IsInRole("admin") == true)
            {
                await Groups.AddToGroupAsync(
                    Context.ConnectionId,
                    AdminGroup
                );
            }

            Console.WriteLine($"Connected: {Context.UserIdentifier}, Roles: {string.Join(',', Context.User?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value) ?? new string[0])}");

            await base.OnConnectedAsync();
        }

        // -------- Notification --------
        public async Task SendNotificationToUser(string userId, NotificationDto notification)
        {
            await Clients.User(userId)
                .SendAsync("ReceiveNotification", notification);
        }

        public async Task SendNotificationToAdmins(NotificationDto notification)
        {
            await Clients.Group(AdminGroup)
                .SendAsync("ReceiveNotification", notification);
        }

        // ❌ KHÔNG CẦN JoinAdminGroup / LeaveAdminGroup

        // -------- Auction --------
        public async Task JoinAuctionGroup(Guid auctionId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                auctionId.ToString()
            );
        }

        public async Task LeaveAuctionGroup(Guid auctionId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                auctionId.ToString()
            );
        }

        //public async Task BroadcastNewBid(Guid auctionId, BidDto bid)
        //{
        //    await Clients.Group(auctionId.ToString())
        //        .SendAsync("NewBid", bid);
        //}

        //public async Task BroadcastAuctionEnded(Guid auctionId, AuctionEndedDto auctionInfo)
        //{
        //    await Clients.Group(auctionId.ToString())
        //        .SendAsync("AuctionEnded", auctionInfo);
        //}
    }

    public record NotificationDto
    {
        public Guid? Id { get; init; }
        public NotificationType NotificationType { get; init; }
        public string Title { get; init; } = null!;
        public string Message { get; init; } = null!;
        public DateTime CreatedAt { get; init; }
        public Guid? RelatedAuctionId { get; init; }
        public bool IsRead { get; init; }
        public bool IsDeleted { get; init; }
        public NotificationMode Mode { get; init; } = NotificationMode.Broadcast;
    }

    public enum NotificationMode
    {
        Broadcast,
        Personal
    }
}