using bidify_be.Domain.Entities;
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

        public async Task<IEnumerable<GiftType>> GetAllAsync()
        {
            return await _context.GiftTypes.AsNoTracking().ToListAsync();
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
