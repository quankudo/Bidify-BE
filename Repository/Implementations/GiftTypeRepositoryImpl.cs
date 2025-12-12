using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.GiftType;
using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bidify_be.Repository.Implementations
{
    public class GiftTypeRepositoryImpl : IGiftTypeRepository
    {
        private readonly ApplicationDbContext _context;
        public GiftTypeRepositoryImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(GiftType giftType)
        {
            await _context.GiftTypes.AddAsync(giftType);
        }

        public void Delete(GiftType giftType)
        {
            giftType.Status = false;
            _context.GiftTypes.Update(giftType);
        }

        public async Task<bool> ExistsWithCodeAsync(string code)
        {
            return await _context.GiftTypes.AsNoTracking().AnyAsync(x => x.Code.ToLower() == code.ToLower());
        }

        public async Task<bool> ExistsWithCodeAsync(Guid id, string code)
        {
            return await _context.GiftTypes.AsNoTracking().AnyAsync(x => x.Code.ToLower() == code.ToLower() && x.Id != id);
        }

        public async Task<PagedResult<GiftTypeResponse>> GetAllAsync(GiftTypeQueryRequest req)
        {
            var query = _context.GiftTypes.AsNoTracking().AsQueryable();

            // SEARCH theo Code + Name
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                string keyword = req.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.Code.ToLower().Contains(keyword) ||
                    x.Name.ToLower().Contains(keyword)
                );
            }

            // FILTER theo Status
            if (req.Status.HasValue)
            {
                query = query.Where(x => x.Status == req.Status.Value);
            }

            // Đếm tổng
            int totalItems = await query.CountAsync();

            // Paging
            var items = await query
                .OrderByDescending(x => x.CreatedAt) // tùy chọn
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(x => new GiftTypeResponse
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    Status = x.Status
                })
                .ToListAsync();

            return new PagedResult<GiftTypeResponse>
            (items, totalItems, req.Page, req.PageSize);
        }


        public async Task<GiftType?> GetByIdAsync(Guid id)
        {
            return await _context.GiftTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public void ToggleActive(GiftType giftType)
        {
            giftType.Status = true;
            _context.GiftTypes.Update(giftType);
        }

        public void Update(GiftType giftType)
        {
            _context.GiftTypes.Update(giftType);
        }
    }
}
