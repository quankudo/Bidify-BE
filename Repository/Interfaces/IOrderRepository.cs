using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Order;

namespace bidify_be.Repository.Interfaces
{
    public interface IOrderRepository
    {
        Task<bool> CreateOrderAsync(Order order);
        void UpdateOrderAsync(Order order);
        Task<PagedResult<OrderResponseForAdmin>> GetOrderForAdminAsync(OrderFilterRequest orderFilter);
        Task<PagedResult<OrderResponseForSeller>> GetOrderForSellerAsync(OrderFilterRequest orderFilter, string userId);
        Task<PagedResult<OrderResponseForWinner>> GetOrderForWinnerAsync(OrderFilterRequest orderFilter, string userId);

        Task<bool> ExistOrderByAuctionId(Guid auctionId);

        Task<Order?> GetOrderByIdAndWinnerIdForPaid(Guid orderId, string winnerId);
    }
}
