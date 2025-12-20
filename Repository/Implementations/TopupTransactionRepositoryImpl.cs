using bidify_be.Domain.Entities;
using bidify_be.DTOs.Topup;
using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace bidify_be.Repository.Implementations
{
    public class TopupTransactionRepositoryImpl : ITopupTransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TopupTransactionRepositoryImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TopupTransaction?> GetByClientOrderIdAsync(string clientOrderId)
        {
            return await _context.TopupTransactions
                .FirstOrDefaultAsync(x => x.ClientOrderId == clientOrderId);
        }

        public async Task AddAsync(TopupTransaction transaction)
        {
            await _context.TopupTransactions.AddAsync(transaction);
        }

        public Task UpdateAsync(TopupTransaction transaction)
        {
            _context.TopupTransactions.Update(transaction);
            return Task.CompletedTask;
        }

        public async Task<List<TopupTransactionResponse>> GetAllByUserIdAsync(string userId, TopupRequestQuery req)
        {
            var query = _context.TopupTransactions
                .AsNoTracking()
                .Where(t => t.UserId == userId);

            if (req.PaymentMethod.HasValue)
            {
                query = query.Where(t => t.PaymentMethod == req.PaymentMethod.Value);
            }

            if (req.Status.HasValue)
            {
                query = query.Where(t => t.Status == req.Status.Value);
            }

            return await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip(req.Skip)
                .Take(req.Take)
                .Select(t => new TopupTransactionResponse
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Status = t.Status,
                    PaymentMethod = t.PaymentMethod,
                    TransactionCode = t.TransactionCode,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

    }

}
