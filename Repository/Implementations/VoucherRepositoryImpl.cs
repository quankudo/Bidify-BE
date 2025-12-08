using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
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
