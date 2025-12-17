using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Category;
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

        public async Task<PagedResult<CategoryResponse>> GetAllAsync(CategoryQueryRequest req)
        {
            var query = _context.Categories.AsNoTracking();

            // Search theo Title
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                string keyword = req.Search.Trim().ToLower();
                query = query.Where(x => x.Title.ToLower().Contains(keyword));
            }

            // Filter theo Status
            if (req.Status.HasValue)
            {
                query = query.Where(x => x.Status == req.Status.Value);
            }

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt) // nếu không có CreatedAt thì x.Id
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(x => new CategoryResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    ImageUrl = x.ImageUrl,
                    Status = x.Status,
                    PublicId = x.PublicId
                })
                .ToListAsync();

            return new PagedResult<CategoryResponse>(items, totalItems, req.Page, req.PageSize);
        }

        public async Task<List<CategoryShortResponse>> GetAllAsync()
        {
            return await _context.Categories.AsNoTracking()
                .Select(x => new CategoryShortResponse
                {
                    Id = x.Id,
                    Title = x.Title
                })
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
