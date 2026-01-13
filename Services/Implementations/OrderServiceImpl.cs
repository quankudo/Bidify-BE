using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using bidify_be.DTOs.Order;
using bidify_be.Exceptions;
using bidify_be.Hubs;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using UnauthorizedAccessException = bidify_be.Exceptions.UnauthorizedAccessException;

namespace bidify_be.Services.Implementations
{
    public class OrderServiceImpl : IOrderService
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<AppHub> _hub;

        public OrderServiceImpl(IUnitOfWork uow, ICurrentUserService currentUserService, 
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService,
            IHubContext<AppHub> hub)
        {
            _uow = uow;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _notificationService = notificationService;
            _hub = hub;
        }
        public Task<Order> CreateOrderAsync(CreateOrderRequest req)
        {
            var order = new Order
            {
                AuctionId = req.AuctionId,
                CreatedAt = DateTime.UtcNow,
                FinalPrice = req.FinalPrice,
                SellerId = req.SellerId,
                WinnerId = req.WinnerId,
                Status = OrderStatus.PendingPayment,
                UpdatedAt = DateTime.UtcNow,
            };

            _uow.OrderRepository.CreateOrderAsync(order);
            return Task.FromResult(order);
        }

        public async Task<bool> ExistOrderByAuctionId(Guid auctionId)
        {
            return await _uow.OrderRepository.ExistOrderByAuctionId(auctionId);
        }

        public async Task<PagedResult<OrderResponseForAdmin>> GetOrderForAdminAsync(OrderFilterRequest orderFilter)
        {
            return await _uow.OrderRepository.GetOrderForAdminAsync(orderFilter);
        }

        public async Task<PagedResult<OrderResponseForSeller>> GetOrderForSellerAsync(OrderFilterRequest orderFilter)
        {
            var userId = _currentUserService.GetUserId();
            return await _uow.OrderRepository.GetOrderForSellerAsync(orderFilter, userId);
        }

        public async Task<PagedResult<OrderResponseForWinner>> GetOrderForWinnerAsync(OrderFilterRequest orderFilter)
        {
            var userId = _currentUserService.GetUserId();
            return await _uow.OrderRepository.GetOrderForWinnerAsync(orderFilter, userId);
        }

        public async Task PaidOrderAsync(PaidOrderRequest req)
        {
            var userId = _currentUserService.GetUserId();
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new UnauthorizedAccessException("User is unauthorized!");

            Order order;

            // =========================
            // TRANSACTION CHÍNH
            // =========================
            await using var tx = await _uow.BeginTransactionAsync();
            try
            {
                order = await _uow.OrderRepository
                    .GetOrderByIdAndWinnerIdForPaid(req.OrderId, userId)
                    ?? throw new CategoryNotFoundException("Order not found");

                // 1. Check trạng thái
                if (order.Status != OrderStatus.PendingPayment)
                    throw new InvalidOperationException("Order is not payable");

                // 2. Check số dư
                if (user.Balance < order.FinalPrice)
                    throw new InsufficientBalanceException("Insufficient balance");

                // 3. Check address
                var address = order.Winner.Addresses
                    .FirstOrDefault(a => a.Id == req.AddressId)
                    ?? throw new AddressNotFoundException("Invalid address");

                // 4. Snapshot address
                order.ShippingAddress = $"{address.LineOne} - {address.LineTwo}";
                order.ReceiverName = address.UserName;
                order.ReceiverPhone = address.PhoneNumber;

                // 5. Anti double-payment (wallet)
                if (await _uow.WalletTransactionRepository.ExistsAsync(
                    WalletTransactionType.PayFinalProduct, order.Id))
                {
                    throw new InvalidOperationException("Order already paid");
                }

                // 6. Wallet transaction
                var balanceBefore = user.Balance;
                var balanceAfter = balanceBefore - order.FinalPrice;

                var walletTransaction = new WalletTransaction
                {
                    UserId = userId,
                    Amount = -order.FinalPrice,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = balanceAfter,
                    CreatedAt = DateTime.UtcNow,
                    ReferenceId = order.Id,
                    Type = WalletTransactionType.PayFinalProduct,
                    Description = $"Thanh toán đơn hàng {order.Id}",
                };

                await _uow.WalletTransactionRepository.AddAsync(walletTransaction);

                // 7. Update state
                user.Balance = balanceAfter;
                order.Status = OrderStatus.Paid;

                await _uow.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

            // =========================
            // NOTIFICATION (SEPARATE TX)
            // =========================
            var notifications = await _notificationService
                .SendWithSeparateTransactionAsync(
                    NotificationType.ORDER_PAID,
                    "Đơn hàng đã được thanh toán",
                    $"{user.UserName} đã thanh toán đơn hàng",
                    new[] { order.SellerId },
                    order.Id
                );

            // =========================
            // SIGNALR PUSH
            // =========================
            var notify = notifications?.FirstOrDefault();
            if (notify != null)
            {
                await _hub.Clients.User(order.SellerId)
                    .SendAsync("ReceiveNotification", new NotificationDto
                    {
                        Id = notify.Id,
                        NotificationType = NotificationType.ORDER_PAID,
                        Title = "Đơn hàng đã được thanh toán",
                        Message = $"{user.UserName} đã thanh toán đơn hàng",
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false,
                        IsDeleted = false,
                        Mode = NotificationMode.Personal,
                    });
            }
        }

    }
}
