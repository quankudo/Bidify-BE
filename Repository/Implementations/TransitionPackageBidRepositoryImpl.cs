using bidify_be.Domain.Entities;
using bidify_be.DTOs.TransitionPackageBid;
using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bidify_be.Repository.Implementations
{
    public class TransitionPackageBidRepositoryImpl : ITransitionPackageBidRepository
    {
        private readonly ApplicationDbContext _context;
        public TransitionPackageBidRepositoryImpl(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(TransitionPackageBid entity)
        {
            await _context.AddAsync(entity);
        }

        public async Task<List<TransitionPackageBidResponse>> GetByUserIdAsync(string userId)
        {
            var query = from t in _context.TransitionPackagesBids.AsNoTracking()
                        join p in _context.PackageBid.AsNoTracking()
                            on t.PackageBidId equals p.Id
                        where t.UserId == userId
                        select new TransitionPackageBidResponse
                        {
                            Id = t.Id,
                            UserId = t.UserId,
                            PackageBidId = t.PackageBidId,
                            Price = t.Price,
                            BidCount = t.BidCount,
                            CreatedAt = t.CreatedAt,
                            Title = p.Title,
                            BgColor = p.BgColor
                        };

            return await query.ToListAsync();
        }
    }
}
