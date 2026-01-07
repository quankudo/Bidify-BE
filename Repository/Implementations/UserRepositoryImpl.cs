using bidify_be.Domain.Entities;
using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bidify_be.Repository.Implementations
{
    public class UserRepositoryImpl : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepositoryImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string userId)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<ApplicationUser?> GetByIdWithLockAsync(string userId)
        {
            return await _context.Users
                .FromSqlRaw("SELECT * FROM AspNetUsers WHERE Id = {0} FOR UPDATE", userId)
                .FirstOrDefaultAsync();
        }

        public void Update(ApplicationUser user)
        {
            _context.Users.Update(user);
        }
    }
}
