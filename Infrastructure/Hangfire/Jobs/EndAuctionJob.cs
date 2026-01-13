using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using bidify_be.DTOs.Order;
using bidify_be.Hubs;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.SignalR;

namespace bidify_be.Infrastructure.Hangfire.Jobs
{
    public class EndAuctionJob
    {
        private readonly IUnitOfWork _uow;
        private readonly IOrderService _orderService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<AppHub> _hub;
        private readonly ILogger<EndAuctionJob> _logger;

        public EndAuctionJob(
            IUnitOfWork uow,
            IOrderService orderService,
            INotificationService notificationService,
            IHubContext<AppHub> hub,
            ILogger<EndAuctionJob> logger)
        {
            _uow = uow;
            _orderService = orderService;
            _notificationService = notificationService;
            _hub = hub;
            _logger = logger;
        }

        [DisableConcurrentExecution(300)]
        public async Task EndAuctionAsync(Guid auctionId)
        {
            Auction auction;

            await using var tx = await _uow.BeginTransactionAsync();
            try
            {
                auction = await _uow.AuctionRepository.GetByIdForBackgroudJobAsync(auctionId); 
                if (auction == null || auction.Status != AuctionStatus.Approved)
                    return;

                // ===== CASE 1: NO BID =====
                if (auction.Winner == null)
                {
                    auction.Status = AuctionStatus.EndedNoBids;
                    auction.UpdatedAt = DateTime.UtcNow;

                    _uow.AuctionRepository.Update(auction);
                    await _uow.SaveChangesAsync();
                    await tx.CommitAsync();
                }
                // ===== CASE 2: HAS BID =====
                else
                {
                    if (await _orderService.ExistOrderByAuctionId(auctionId))
                        return;

                    await _orderService.CreateOrderAsync(new CreateOrderRequest
                    {
                        AuctionId = auction.Id,
                        WinnerId = auction.Winner.Id,
                        SellerId = auction.UserId,
                        FinalPrice = auction.BuyNowPrice ?? 0
                    });

                    auction.Status = AuctionStatus.EndedWithBids;
                    auction.UpdatedAt = DateTime.UtcNow;

                    _uow.AuctionRepository.Update(auction);
                    await _uow.SaveChangesAsync();
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "EndAuctionJob failed. AuctionId={AuctionId}", auctionId);
                throw;
            }

            // ======================================================
            // 🔔 NOTIFICATION + SIGNALR (TRANSACTION RIÊNG)
            // ======================================================

            // ---------- NO BID ----------
            if (auction.Winner == null)
            {
                await _notificationService.SendWithSeparateTransactionAsync(
                    NotificationType.AUCTION_ENDED,
                    "Phiên đấu giá kết thúc",
                    "Phiên đấu giá của bạn đã kết thúc nhưng không có lượt đấu giá nào.",
                    new[] { auction.UserId },
                    auction.Id
                );

                await _hub.Clients.User(auction.UserId)
                    .SendAsync("ReceiveNotification", new NotificationDto
                    {
                        NotificationType = NotificationType.AUCTION_ENDED,
                        Title = "Phiên đấu giá kết thúc",
                        Message = "Không có lượt đấu giá nào.",
                        RelatedAuctionId = auction.Id,
                        CreatedAt = DateTime.UtcNow,
                        Mode = NotificationMode.Personal
                    });

                return;
            }

            // ---------- HAS BID ----------
            // 🎉 Winner
            await _notificationService.SendWithSeparateTransactionAsync(
                NotificationType.AUCTION_WON,
                "Chúc mừng bạn đã thắng đấu giá 🎉",
                $"Bạn đã thắng phiên đấu giá với giá {auction.BuyNowPrice:N0}₫",
                new[] { auction.Winner.Id },
                auction.Id
            );

            // 📦 Seller
            await _notificationService.SendWithSeparateTransactionAsync(
                NotificationType.AUCTION_ENDED,
                "Phiên đấu giá đã kết thúc",
                $"Sản phẩm của bạn đã được đấu giá thành công với giá {auction.BuyNowPrice:N0}₫",
                new[] { auction.UserId },
                auction.Id
            );

            // ---------- SIGNALR ----------
            var winnerPayload = new NotificationDto
            {
                NotificationType = NotificationType.AUCTION_WON,
                Title = "Bạn đã thắng đấu giá 🎉",
                Message = $"Giá cuối: {auction.BuyNowPrice:N0}₫",
                RelatedAuctionId = auction.Id,
                CreatedAt = DateTime.UtcNow,
                Mode = NotificationMode.Personal
            };

            var sellerPayload = new NotificationDto
            {
                NotificationType = NotificationType.AUCTION_ENDED,
                Title = "Phiên đấu giá kết thúc",
                Message = $"Đã bán với giá {auction.BuyNowPrice:N0}₫",
                RelatedAuctionId = auction.Id,
                CreatedAt = DateTime.UtcNow,
                Mode = NotificationMode.Personal
            };

            await _hub.Clients.User(auction.Winner.Id)
                .SendAsync("ReceiveNotification", winnerPayload);

            await _hub.Clients.User(auction.UserId)
                .SendAsync("ReceiveNotification", sellerPayload);

            _logger.LogInformation(
                "Auction ended successfully. AuctionId={AuctionId}, Winner={WinnerId}",
                auction.Id,
                auction.Winner.Id
            );
        }
    }

}
