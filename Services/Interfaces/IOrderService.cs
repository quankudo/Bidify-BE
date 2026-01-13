using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Order;

namespace bidify_be.Services.Interfaces
{
    public interface IOrderService
    {
        Task<PagedResult<OrderResponseForAdmin>> GetOrderForAdminAsync(OrderFilterRequest orderFilter);
        Task<PagedResult<OrderResponseForSeller>> GetOrderForSellerAsync(OrderFilterRequest orderFilter);
        Task<PagedResult<OrderResponseForWinner>> GetOrderForWinnerAsync(OrderFilterRequest orderFilter);

        Task<bool> ExistOrderByAuctionId(Guid auctionId);

        Task<Order> CreateOrderAsync(CreateOrderRequest order);
        Task PaidOrderAsync(PaidOrderRequest order);
        //Task CanceldOrderAsync(CanceldOrderRequest);
    }
}
