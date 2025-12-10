using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Gift;
using bidify_be.DTOs.GiftType;
using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bidify_be.Repository.Implementations
{
    public class GiftRepositoryImpl : IGiftRepository
    {
        private readonly ApplicationDbContext _context;
        public GiftRepositoryImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Gift gift)
        {
            await _context.Gifts.AddAsync(gift);
        }

        public void DeleteAsync(Gift gift)
        {
            gift.Status = false;
            _context.Gifts.Update(gift);
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _context.Gifts.AsNoTracking().AnyAsync(x=>x.Code.ToLower() == code.ToLower());
        }

        public async Task<bool> ExistsByCodeAsync(Guid id, string code)
        {
            return await _context.Gifts.AsNoTracking().AnyAsync(x => x.Code.ToLower() == code.ToLower() && x.Id != id);
        }

        public async Task<PagedResult<GiftResponse>> SearchAsync(GiftQueryRequest req)
        {
            var query =
                from g in _context.Gifts.AsNoTracking()
                join t in _context.GiftTypes.AsNoTracking()
                    on g.GiftTypeId equals t.Id
                select new { g, t };

            // SEARCH
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                string keyword = req.Search.Trim();

                query = query.Where(x =>
                    x.g.Code.Contains(keyword) ||
                    x.g.Description.Contains(keyword) ||
                    x.g.QuantityBid.ToString().Contains(keyword)
                );
            }

            // FILTER
            if (req.Status.HasValue)
                query = query.Where(x => x.g.Status == req.Status);

            if (!string.IsNullOrWhiteSpace(req.GiftTypeCode))
                query = query.Where(x => x.t.Code == req.GiftTypeCode);

            // TOTAL COUNT
            var total = await query.CountAsync();

            // PAGINATION
            var data = await query
                .OrderByDescending(x => x.g.CreatedAt)
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(x => new GiftResponse
                {
                    Id = x.g.Id,
                    Code = x.g.Code,
                    QuantityBid = x.g.QuantityBid,
                    Description = x.g.Description,
                    Status = x.g.Status,

                    GiftType = new GiftTypeShortResponse
                    {
                        Id = x.t.Id,
                        Code = x.t.Code
                    }
                })
                .ToListAsync();

            return new PagedResult<GiftResponse>(data, total, req.Page, req.PageSize);
        }



        public async Task<Gift?> GetByIdAsync(Guid id)
        {
            return await _context.Gifts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<GiftResponse?> GetByIdAsyncResponse(Guid id)
        {
            return await _context.Gifts
                .AsNoTracking()
                .Where(g => g.Id == id)
                .Join(
                    _context.GiftTypes,
                    g => g.GiftTypeId,
                    t => t.Id,
                    (g, t) => new GiftResponse
                    {
                        Id = g.Id,
                        Code = g.Code,
                        QuantityBid = g.QuantityBid,
                        Description = g.Description,
                        Status = g.Status,

                        GiftType = new GiftTypeShortResponse
                        {
                            Id = t.Id,
                            Code = t.Code
                        }
                    }
                )
                .FirstOrDefaultAsync();
        }

        public void ToggleActiveAsync(Gift gift)
        {
            gift.Status = true;
            _context.Gifts.Update(gift);
        }

        public void UpdateAsync(Gift gift)
        {
            _context.Gifts.Update(gift);
        }
    }
}
