using bidify_be.Domain.Entities;
using bidify_be.DTOs;
using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace bidify_be.Repository.Implementations
{
    public class WalletTransactionRepositoryImpl : IWalletTransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public WalletTransactionRepositoryImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(WalletTransaction transaction)
        {
            await _context.WalletTransactions.AddAsync(transaction);
        }

        public async Task<List<WalletTransaction>> GetAllByUserIdAsync(WalletTransactionQuery req, string userId)
        {
            var query = _context.WalletTransactions
                .AsNoTracking()
                .Where(t => t.UserId == userId);

            // Filter theo type (nếu có)
            if (req.Type.HasValue)
            {
                query = query.Where(t => t.Type == req.Type.Value);
            }

            // Order bắt buộc cho pagination
            query = query.OrderByDescending(t => t.CreatedAt);

            // Guard skip / take
            var skip = req.Skip < 0 ? 0 : req.Skip;
            var take = req.Take <= 0 ? 20 : req.Take;

            return await query
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

    }

}
