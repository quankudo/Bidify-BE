using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using bidify_be.Infrastructure.Context;
using bidify_be.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bidify_be.Repository.Implementations
{
    public class FileStorageRepository : IFileStorageRepository
    {
        private readonly ApplicationDbContext _context;

        public FileStorageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(FileStorage file)
        {
            await _context.FileStorages.AddAsync(file);
        }

        public async Task<FileStorage?> GetByPublicIdAsync(string publicId)
        {
            return await _context.FileStorages.FindAsync(publicId);
        }

        public void MarkAsUsed(FileStorage file)
        {
            file.Status = FileStatus.Used;
            _context.FileStorages.Update(file);
        }

        public async Task<List<FileStorage>> GetExpiredTempFilesAsync(DateTime expiredAt)
        {
            return await _context.FileStorages
                .Where(x => x.Status == FileStatus.Temp && x.CreatedAt < expiredAt)
                .ToListAsync();
        }

        public void Update(FileStorage file)
        {
            _context.FileStorages.Update(file);
        }

        public void DeleteRange(IEnumerable<FileStorage> files)
        {
            _context.FileStorages.RemoveRange(files);
        }
    }

}
