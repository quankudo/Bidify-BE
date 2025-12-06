using bidify_be.Domain.Entities;
using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bidify_be.Repository.Implementations
{
    public class CategoryRepositoryImpl : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepositoryImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public void Delete(Category category)
        {
            category.Status = false;
            _context.Categories.Update(category);
        }

        public Task<bool> ExistsAsyncById(Guid id)
        {
            return _context.Categories
                           .AsNoTracking()
                           .AnyAsync(c => c.Id == id);
        }

        public Task<bool> ExistsAsyncByName(string title)
        {
            return _context.Categories
                           .AsNoTracking()
                           .AnyAsync(c => c.Title.ToLower() == title.ToLower());
        }

        public Task<bool> ExistsAsyncByName(string title, Guid id)
        {
            return _context.Categories
                           .AsNoTracking()
                           .AnyAsync(c => c.Title.ToLower() == title.ToLower() && c.Id != id);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _context.Categories
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }

        public void ToggleActive(Category category)
        {
            category.Status = true;
            _context.Categories.Update(category);
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }
    }
}
