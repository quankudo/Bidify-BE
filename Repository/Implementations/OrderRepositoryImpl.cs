using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Order;
using bidify_be.DTOs.Product;
using bidify_be.DTOs.Users;
using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bidify_be.Repository.Implementations
{
    public class OrderRepositoryImpl : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderRepositoryImpl(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            return true;
        }

        private IQueryable<Order> ApplyFilter(
            IQueryable<Order> query,
            OrderFilterRequest filter)
        {
            if (filter.Status.HasValue)
            {
                query = query.Where(o => o.Status == filter.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim().ToLower();

                query = query.Where(o =>
                    o.Id.ToString().Contains(search) ||
                    o.SellerId.ToString().Contains(search) ||
                    o.WinnerId.ToString().Contains(search) ||
                    o.ReceiverName.ToLower().Contains(search) ||
                    o.ReceiverPhone.Contains(search)
                );
            }

            return query;
        }


        public async Task<PagedResult<OrderResponseForAdmin>> GetOrderForAdminAsync(
            OrderFilterRequest filter)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Include(o => o.Seller)
                .Include(o => o.Winner)
                .AsQueryable();

            query = ApplyFilter(query, filter);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(o => new OrderResponseForAdmin
                {
                    Id = o.Id,
                    AuctionId = o.AuctionId,
                    SellerId = o.SellerId,
                    WinnerId = o.WinnerId,
                    FinalPrice = o.FinalPrice,
                    PaidAt = o.PaidAt,
                    ReceiverName = o.ReceiverName,
                    ReceiverPhone = o.ReceiverPhone,
                    ShippingAddress = o.ShippingAddress,
                    Status = o.Status,
                    CancelReason = o.CancelReason,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    Seller = new UserShortResponse
                    {
                        Avatar = o.Seller.Avatar,
                        Id = o.Seller.Id,
                        RateStar = o.Seller.RateStar,
                        UserName = o.Seller.UserName,
                    },
                    Winner = new UserShortResponse
                    {
                        Avatar = o.Winner.Avatar,
                        Id = o.Winner.Id,
                        RateStar = o.Winner.RateStar,
                        UserName = o.Winner.UserName,
                    },
                    Product = new ProductShortForOrderResponse
                    {
                        Id = o.Auction.ProductId,
                        Name = o.Auction.Product.Name,
                        Thumbnail = o.Auction.Product.Thumbnail
                    }
                })
                .ToListAsync();

            return new PagedResult<OrderResponseForAdmin>(
                items,
                totalCount,
                filter.PageNumber,
                filter.PageSize
            );
        }


        public async Task<PagedResult<OrderResponseForSeller>> GetOrderForSellerAsync(OrderFilterRequest filter, string userId)
        {

            var query = _context.Orders
                .AsNoTracking()
                .Include(o => o.Winner)
                .Where(o => o.SellerId == userId)
                .AsQueryable();

            query = ApplyFilter(query, filter);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(o => new OrderResponseForSeller
                {
                    Id = o.Id,
                    AuctionId = o.AuctionId,
                    SellerId = o.SellerId,
                    WinnerId = o.WinnerId,
                    FinalPrice = o.FinalPrice,
                    PaidAt = o.PaidAt,
                    ReceiverName = o.ReceiverName,
                    ReceiverPhone = o.ReceiverPhone,
                    ShippingAddress = o.ShippingAddress,
                    Status = o.Status,
                    CancelReason = o.CancelReason,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    Winner = new UserShortResponse
                    {
                        Avatar = o.Winner.Avatar,
                        Id = o.Winner.Id,
                        RateStar = o.Winner.RateStar,
                        UserName = o.Winner.UserName,
                    },
                    Product = new ProductShortForOrderResponse
                    {
                        Id = o.Auction.ProductId,
                        Name = o.Auction.Product.Name,
                        Thumbnail = o.Auction.Product.Thumbnail
                    }
                })
                .ToListAsync();

            return new PagedResult<OrderResponseForSeller>(
                items,
                totalCount,
                filter.PageNumber,
                filter.PageSize
            );
        }


        public async Task<PagedResult<OrderResponseForWinner>> GetOrderForWinnerAsync(OrderFilterRequest filter, string userId)
        {

            var query = _context.Orders
                .AsNoTracking()
                .Include(o => o.Seller)
                .Where(o => o.WinnerId == userId)
                .AsQueryable();

            query = ApplyFilter(query, filter);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(o => new OrderResponseForWinner
                {
                    Id = o.Id,
                    AuctionId = o.AuctionId,
                    SellerId = o.SellerId,
                    WinnerId = o.WinnerId,
                    FinalPrice = o.FinalPrice,
                    PaidAt = o.PaidAt,
                    ReceiverName = o.ReceiverName,
                    ReceiverPhone = o.ReceiverPhone,
                    ShippingAddress = o.ShippingAddress,
                    Status = o.Status,
                    CancelReason = o.CancelReason,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    Seller = new UserShortResponse
                    {
                        Avatar = o.Seller.Avatar,
                        Id = o.Seller.Id,
                        RateStar = o.Seller.RateStar,
                        UserName = o.Seller.UserName,
                    },
                    Product = new ProductShortForOrderResponse
                    {
                        Id = o.Auction.ProductId,
                        Name = o.Auction.Product.Name,
                        Thumbnail = o.Auction.Product.Thumbnail
                    }
                })
                .ToListAsync();

            return new PagedResult<OrderResponseForWinner>(
                items,
                totalCount,
                filter.PageNumber,
                filter.PageSize
            );
        }


        public void UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
        }

        public async Task<bool> ExistOrderByAuctionId(Guid auctionId)
        {
            return await _context.Orders.AsNoTracking().AnyAsync(x=>x.AuctionId == auctionId);
        }

        public async Task<Order?> GetOrderByIdAndWinnerIdForPaid(Guid orderId, string winnerId)
        {
            return await _context.Orders
                .Include(o=>o.Winner)
                    .ThenInclude(w=>w.Addresses)
                .FirstOrDefaultAsync(x=>x.WinnerId==winnerId && x.Id==orderId);
        }
    }
}
