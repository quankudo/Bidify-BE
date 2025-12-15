using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Services.Interfaces;

namespace bidify_be.Services.Implementations
{
    public class FileStorageServiceImpl : IFileStorageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudStorageService _cloudStorageService;

        public FileStorageServiceImpl(
            IUnitOfWork unitOfWork,
            ICloudStorageService cloudStorageService)
        {
            _unitOfWork = unitOfWork;
            _cloudStorageService = cloudStorageService;
        }

        // Tạo record TEMP sau khi upload thành công
        public async Task CreateTempAsync(string publicId)
        {
            var existing = await _unitOfWork.FileStorageRepository.GetByPublicIdAsync(publicId);
            if (existing != null) return;

            var file = new FileStorage
            {
                PublicId = publicId,
                Status = FileStatus.Temp,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.FileStorageRepository.AddAsync(file);
            await _unitOfWork.SaveChangesAsync();
        }

        // Đánh dấu file đã được sử dụng
        public async Task MarkAsUsedAsync(string publicId)
        {
            var file = await _unitOfWork.FileStorageRepository.GetByPublicIdAsync(publicId);
            if (file == null)
                throw new InvalidOperationException("File not found");

            if (file.Status == FileStatus.Used) return;

            _unitOfWork.FileStorageRepository.MarkAsUsed(file);
        }

        public async Task RequestDeleteAsync(string publicId)
        {
            var file = await _unitOfWork.FileStorageRepository.GetByPublicIdAsync(publicId);
            if (file == null) return;

            // Đánh dấu intent xoá
            file.DeletedAt = DateTime.UtcNow;

            _unitOfWork.FileStorageRepository.Update(file);
        }


        // Lấy danh sách TEMP quá hạn
        public async Task<List<FileStorage>> GetExpiredTempFilesAsync(TimeSpan ttl)
        {
            var expiredAt = DateTime.UtcNow.Subtract(ttl);
            return await _unitOfWork.FileStorageRepository.GetExpiredTempFilesAsync(expiredAt);
        }

        // Cleanup TEMP quá hạn
        public async Task CleanupExpiredTempFilesAsync(TimeSpan ttl)
        {
            var expiredFiles = await GetExpiredTempFilesAsync(ttl);

            if (!expiredFiles.Any()) return;

            foreach (var file in expiredFiles)
            {
                await _cloudStorageService.DeleteAsync(file.PublicId);
            }

            _unitOfWork.FileStorageRepository.DeleteRange(expiredFiles);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
