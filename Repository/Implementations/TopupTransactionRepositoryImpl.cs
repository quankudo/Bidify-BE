using bidify_be.Domain.Entities;
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
    }

}
