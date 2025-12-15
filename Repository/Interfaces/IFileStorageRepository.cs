using bidify_be.Domain.Entities;

namespace bidify_be.Repository.Interfaces
{
    public interface IFileStorageRepository
    {
        // Tạo record khi upload file
        Task AddAsync(FileStorage file);

        // Lấy file theo publicId
        Task<FileStorage?> GetByPublicIdAsync(string publicId);

        // Đánh dấu file đã được dùng
        void MarkAsUsed(FileStorage file);

        // Lấy danh sách file TEMP quá hạn
        Task<List<FileStorage>> GetExpiredTempFilesAsync(DateTime expiredAt);

        // Xoá record (sau khi xoá file cloud)
        void Update(FileStorage file);

        // Bulk delete
        void DeleteRange(IEnumerable<FileStorage> files);
    }
}
