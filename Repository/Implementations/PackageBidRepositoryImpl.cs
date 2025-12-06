using bidify_be.Domain.Entities;
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

        public async Task<IEnumerable<PackageBid>> GetAllAsync()
        {
            return await _context.PackageBid.AsNoTracking().ToListAsync();
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
