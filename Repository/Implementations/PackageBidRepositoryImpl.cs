using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.PackageBid;
using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bidify_be.Repository.Implementations
{
    public class PackageBidRepositoryImpl : IPackageBidRepository
    {
        private readonly ApplicationDbContext _context;

        public PackageBidRepositoryImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PackageBid packageBid)
        {
            await _context.PackageBid.AddAsync(packageBid);
        }

        public void Delete(PackageBid packageBid)
        {
            packageBid.status = false;
            _context.PackageBid.Update(packageBid);
        }

        public Task<bool> ExistsAsync(Guid id, string title)
        {
            return _context.PackageBid
                           .AsNoTracking()
                           .AnyAsync(c => c.Title.ToLower() == title.ToLower() && c.Id != id);
        }

        public Task<bool> ExistsAsync(string title)
        {
            return _context.PackageBid
                           .AsNoTracking()
                           .AnyAsync(c => c.Title.ToLower() == title.ToLower());
        }

        public async Task<PagedResult<PackageBidResponse>> GetAllAsync(PackageBidQueryRequest req)
        {
            var query = _context.PackageBid.AsNoTracking();

            // Search theo Title
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                string keyword = req.Search.Trim().ToLower();
                query = query.Where(x => x.Title.ToLower().Contains(keyword));
            }

            // Filter theo Status
            if (req.Status.HasValue)
            {
                query = query.Where(x => x.status == req.Status.Value);
            }

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(x => new PackageBidResponse
                {
                    Id = x.Id,
                    Price = x.Price,
                    BidQuantity = x.BidQuantity,
                    BgColor = x.BgColor,
                    Title = x.Title,
                    Status = x.status
                })
                .ToListAsync();

            return new PagedResult<PackageBidResponse>(
                items,
                totalItems,
                req.Page,
                req.PageSize
            );
        }


        public async Task<PackageBid?> GetByIdAsync(Guid id)
        {
            return await _context.PackageBid
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }

        public void ToggleActive(PackageBid packageBid)
        {
            packageBid.status = true;
            _context.PackageBid.Update(packageBid);
        }

        public void Update(PackageBid packageBid)
        {
            _context.PackageBid.Update(packageBid);
        }
    }
}
