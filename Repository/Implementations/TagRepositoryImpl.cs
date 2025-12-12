using bidify_be.Domain.Entities;
using bidify_be.DTOs.Tags;
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

        public async Task<IEnumerable<TagResponse>> GetAllTagsAsync(TagQueryRequest req)
        {
            var query = _context.Tags.AsNoTracking().AsQueryable();

            // SEARCH theo Title
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                string keyword = req.Search.Trim().ToLower();
                query = query.Where(x => x.Title.ToLower().Contains(keyword));
            }

            // FILTER theo TagType
            if (req.Type.HasValue)
            {
                query = query.Where(x => x.Type == req.Type.Value);
            }

            return await query
                .Select(x => new TagResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    Type = x.Type,
                    Status = x.Status
                })
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
