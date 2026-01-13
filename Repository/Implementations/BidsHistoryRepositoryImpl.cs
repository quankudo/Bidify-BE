using bidify_be.Domain.Entities;
using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bidify_be.Repository.Implementations
{
    public class BidsHistoryRepositoryImpl : IBidsHistoryRepository
    {
        private readonly ApplicationDbContext _context;
        public BidsHistoryRepositoryImpl(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<BidsHistory> AddBidsHistoryAsync(BidsHistory bidsHistory)
        {
            var result = await _context.BidsHistories.AddAsync(bidsHistory);
            return result.Entity;
        }

        public async Task<List<BidsHistory>> GetBidsHistoriesByAuctionIdAsync(Guid auctionId, int skip, int take)
        {
            return await _context.BidsHistories.AsNoTracking().Where(x=>x.AuctionId == auctionId)
                .OrderByDescending(x=>x.CreatedAt)
                .Include(x=>x.User)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<BidsHistory>> GetBidsHistoriesByUserIdAsync(string userId, int skip, int take)
        {
            return await _context.BidsHistories.AsNoTracking().Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Include(x => x.User)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }
}
