using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using bidify_be.DTOs.GiftType;
using bidify_be.DTOs.PackageBid;
using bidify_be.DTOs.Voucher;
using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bidify_be.Repository.Implementations
{
    public class VoucherRepositoryImpl : IVoucherRepository
    {
        private readonly ApplicationDbContext _context;
        public VoucherRepositoryImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Voucher voucher)
        {
            await _context.Vouchers.AddAsync(voucher);
        }

        public void DeleteAsync(Voucher voucher)
        {
            voucher.Status = VoucherStatus.Deleted;
            _context.Vouchers.Update(voucher);
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _context.Vouchers.AsNoTracking().AnyAsync(x=>x.Code.ToLower() == code.ToLower());
        }

        public async Task<IEnumerable<Voucher>> GetAllAsync()
        {
            return await _context.Vouchers.AsNoTracking().ToListAsync();
        }

        public async Task<Voucher?> GetByIdAsync(Guid id)
        {
            return await _context.Vouchers.AsNoTracking().FirstOrDefaultAsync(x=>x.Id == id);
        }

        public async Task<IEnumerable<Voucher>> GetByPackageBidIdAsync(Guid packageBidId)
        {
            return await _context.Vouchers.AsNoTracking().Where(x=>x.PackageBidId == packageBidId).ToListAsync();
        }

        public async Task<IEnumerable<Voucher>> GetByStatusAsync(VoucherStatus status)
        {
            return await _context.Vouchers.AsNoTracking().Where(x=>x.Status == status).ToListAsync();
        }

        public async Task<PagedResult<VoucherResponse>> QueryAsync(VoucherQueryRequest req)
        {
            var query =
                from v in _context.Vouchers.AsNoTracking()
                join p in _context.PackageBid on v.PackageBidId equals p.Id
                join g in _context.GiftTypes on v.VoucherTypeId equals g.Id
                select new { v, p, g };

            // 🔍 SEARCH (code + description)
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                string keyword = req.Search.Trim();
                query = query.Where(x =>
                    x.v.Code.Contains(keyword) ||
                    x.v.Description.Contains(keyword)
                );
            }

            // 🎯 FILTER
            if (req.Status.HasValue)
                query = query.Where(x => x.v.Status == req.Status.Value);

            if (!string.IsNullOrWhiteSpace(req.PackageBidTitle))
                query = query.Where(x => x.p.Title.Contains(req.PackageBidTitle));

            if (!string.IsNullOrWhiteSpace(req.GiftTypeCode))
                query = query.Where(x => x.g.Code == req.GiftTypeCode);

            // 📌 Total Count for pagination
            int total = await query.CountAsync();

            // 📌 Pagination
            var data = await query
                .OrderByDescending(x => x.v.CreatedAt)
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(x => new VoucherResponse
                {
                    Id = x.v.Id,
                    Code = x.v.Code,
                    Description = x.v.Description,
                    Status = x.v.Status,
                    Discount = x.v.Discount,
                    DiscountType = x.v.DiscountType,
                    ExpiryDate = x.v.ExpiryDate,

                    PackageBid = new PackageBidShortResponse
                    {
                        Id = x.p.Id,
                        Title = x.p.Title,
                        BgColor = x.p.BgColor,
                        BidQuantity = x.p.BidQuantity
                    },

                    VoucherType = new GiftTypeShortResponse
                    {
                        Id = x.g.Id,
                        Code = x.g.Code
                    }
                })
                .ToListAsync();

            // Replace the return statement in QueryAsync to use the correct constructor for PagedResult<VoucherResponse>
            return new PagedResult<VoucherResponse>(
                data,
                total,
                req.Page,
                req.PageSize
            );
        }


        public void ToggleActiveAsync(Voucher voucher)
        {
            voucher.Status = VoucherStatus.Active;
            _context.Vouchers.Update(voucher);
        }

        public void UpdateAsync(Voucher voucher)
        {
            _context.Vouchers.Update(voucher);
        }
    }
}
