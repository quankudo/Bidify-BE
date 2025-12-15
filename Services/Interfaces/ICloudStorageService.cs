using bidify_be.DTOs.Upload;
using Microsoft.AspNetCore.Http;

namespace bidify_be.Services.Interfaces
{
    public interface ICloudStorageService
    {
        // Upload file lên cloud
        Task<CloudUploadResult> UploadAsync(
            IFormFile file,
            string? folder = null,
            CancellationToken cancellationToken = default);

        Task<List<CloudUploadResult>> UploadManyAsync(
            IEnumerable<IFormFile> files,
            string? folder = null,
            CancellationToken cancellationToken = default);

        // Xoá file khỏi cloud
        Task DeleteAsync(
            string publicId,
            CancellationToken cancellationToken = default);
    }
}
