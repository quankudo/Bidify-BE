using bidify_be.Domain.Entities;
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

        public async Task<IEnumerable<Gift>> GetAllAsync()
        {
            return await _context.Gifts.AsNoTracking().ToListAsync();
        }

        public async Task<Gift?> GetByIdAsync(Guid id)
        {
            return await _context.Gifts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
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
