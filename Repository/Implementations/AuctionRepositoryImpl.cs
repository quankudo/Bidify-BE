using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using bidify_be.DTOs.Auction;
using bidify_be.DTOs.Product;
using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bidify_be.Repository.Implementations
{
    public class AuctionRepositoryImpl : IAuctionRepository
    {
        private readonly ApplicationDbContext _context;

        public AuctionRepositoryImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= CREATE =================

        public async Task AddAsync(Auction auction)
        {
            await _context.Auctions.AddAsync(auction);
        }

        // ================= GET =================

        public async Task<Auction?> GetByIdAsync(Guid auctionId, string userId)
        {
            return await _context.Auctions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == auctionId && x.UserId == userId);
        }

        public async Task<Auction?> GetByIdAsync(Guid auctionId)
        {
            return await _context.Auctions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == auctionId);
        }

        public async Task<Auction?> GetByIdIncludeTagsAsync(Guid auctionId)
        {
            return await _context.Auctions
                .Include(a => a.AuctionTags)
                    .ThenInclude(at => at.Tag) // nếu AuctionTag có navigation Tag
                .FirstOrDefaultAsync(a => a.Id == auctionId);
        }

        /// <summary>
        /// Dùng khi PlaceBid (lock row)
        /// MySQL: SELECT ... FOR UPDATE
        /// </summary>
        public async Task<Auction?> GetByIdWithLockAsync(Guid auctionId)
        {
            return await _context.Auctions
                .FromSqlRaw(
                    "SELECT * FROM Auctions WHERE Id = {0} FOR UPDATE",
                    auctionId)
                .FirstOrDefaultAsync();
        }

        // ================= QUERY =================

        public async Task<PagedResult<AuctionShortResponse>> GetActiveAuctionsAsync(
            AuctionQueryRequest request)
        {
            var now = DateTime.UtcNow;

            var query = _context.Auctions
                .AsNoTracking()
                .Where(x =>
                    x.Status == AuctionStatus.Approved &&
                    x.StartAt <= now &&
                    x.EndAt >= now);

            query = ApplyFilter(query, request);

            return await ToPagedAuctionShortAsync(query, request);
        }

        public async Task<PagedResult<AuctionShortResponse>> GetAuctionsByUserAsync(
            string userId,
            AuctionQueryRequest request)
        {
            var query = _context.Auctions
                .AsNoTracking()
                .Where(x => x.UserId == userId);

            query = ApplyFilter(query, request);

            return await ToPagedAuctionShortAsync(query, request);
        }

        public async Task<PagedResult<AuctionShortResponse>> GetAuctionsForAdminAsync(
            AuctionQueryRequest request)
        {
            var query = _context.Auctions
                .AsNoTracking();

            query = ApplyFilter(query, request);

            return await ToPagedAuctionShortAsync(query, request);
        }

        // ================= UPDATE =================

        public void Update(Auction auction)
        {
            _context.Auctions.Update(auction);
        }

        // ================= PRIVATE =================

        private IQueryable<Auction> ApplyFilter(
            IQueryable<Auction> query,
            AuctionQueryRequest request)
        {
            // SEARCH (tên sản phẩm)
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.Product.Name.ToLower().Contains(s));
            }

            // FILTER
            if (request.Status.HasValue)
                query = query.Where(x => x.Status == request.Status);

            if (request.ProductId.HasValue)
                query = query.Where(x => x.ProductId == request.ProductId);

            if (request.StartFrom.HasValue)
                query = query.Where(x => x.StartAt >= request.StartFrom);

            if (request.EndTo.HasValue)
                query = query.Where(x => x.EndAt <= request.EndTo);

            return query;
        }

        private async Task<PagedResult<AuctionShortResponse>> ToPagedAuctionShortAsync(
            IQueryable<Auction> query,
            AuctionQueryRequest request)
        {
            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new AuctionShortResponse
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    ProductId = x.ProductId,
                    BidCount = x.BidCount,
                    StartAt = x.StartAt,
                    EndAt = x.EndAt,
                    BuyNowPrice = x.BuyNowPrice ?? 0,
                    StepPrice = x.StepPrice,
                    StartPrice = x.StartPrice,
                    Status = x.Status,
                    Note = x.Note ?? "",
                    WinnerId = x.WinnerId ?? "",
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,

                    Product = new ProductShortResponse
                    {
                        Id = x.Product.Id,
                        Name = x.Product.Name,
                        Description = x.Product.Description,
                        CategoryId = x.Product.CategoryId,
                        Brand = x.Product.Brand,
                        Status = x.Product.Status,
                        Condition = x.Product.Condition,
                        Thumbnail = x.Product.Thumbnail,
                    }
                })
                .ToListAsync();

            return new PagedResult<AuctionShortResponse>(
                items,
                totalItems,
                request.PageNumber,
                request.PageSize
            );
        }

        public async Task<Auction?> GetAuctionForUpdateAsync(Guid auctionId)
        {
            return await _context.Auctions
                .Include(a => a.AuctionTags)
                    .ThenInclude(at => at.Tag)
                .FirstOrDefaultAsync(a => a.Id == auctionId);
        }

        public async Task<Auction?> GetAuctionDetailAsync(Guid auctionId)
        {
            return await _context.Auctions
                .Include(a => a.Product)
                .Include(a => a.User)
                .Include(a => a.AuctionTags)
                    .ThenInclude(at => at.Tag)
                .FirstOrDefaultAsync(a => a.Id == auctionId);
        }
    }
}
