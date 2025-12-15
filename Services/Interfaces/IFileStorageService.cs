using bidify_be.Domain.Entities;

namespace bidify_be.Services.Interfaces
{
    public interface IFileStorageService
    {
        // Tạo record TEMP sau khi upload thành công lên cloud
        Task CreateTempAsync(string publicId);

        // Khi User / Product sử dụng ảnh
        Task MarkAsUsedAsync(string publicId);

        // Xoá file hoàn toàn (cloud + db)
        Task RequestDeleteAsync(string publicId);

        // Lấy danh sách TEMP quá hạn (phục vụ cron)
        Task<List<FileStorage>> GetExpiredTempFilesAsync(TimeSpan ttl);

        // Cleanup TEMP files (cron job)
        Task CleanupExpiredTempFilesAsync(TimeSpan ttl);
    }
}
