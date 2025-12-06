using bidify_be.Domain.Entities;
using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bidify_be.Repository.Implementations
{
    public class TagRepositoryImpl : ITagRepository
    {
        private readonly ApplicationDbContext _context;

        public TagRepositoryImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateTagAsync(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
        }

        public void DeleteTagAsync(Tag tag)
        {
            tag.Status = false;
            _context.Tags.Update(tag);
        }

        public Task<bool> ExistsByTitleAsync(string title)
        {
            return _context.Tags
                .AsNoTracking()
                .AnyAsync(t => t.Title.ToLower() == title.ToLower());
        }

        public Task<bool> ExistsOtherWithTitleAsync(Guid id, string title)
        {
            return _context.Tags
                .AsNoTracking()
                .AnyAsync(t => t.Id != id && t.Title.ToLower() == title.ToLower());
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await _context.Tags
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Tag?> GetTagByIdAsync(Guid id)
        {
            return await _context.Tags
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public void ToggleActiveAsync(Tag tag)
        {
            tag.Status = true;
            _context.Tags.Update(tag);
        }

        public void UpdateTagAsync(Tag tag)
        {
            _context.Tags.Update(tag);
        }
    }
}
