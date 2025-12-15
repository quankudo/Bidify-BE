using bidify_be.DTOs.Upload;
using bidify_be.Services.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace bidify_be.Services.Implementations
{
    public class CloudStorageServiceImpl : ICloudStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudStorageServiceImpl(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<CloudUploadResult> UploadAsync(
            IFormFile file,
            string? folder = null,
            CancellationToken cancellationToken = default)
        {
            if (file.Length == 0)
                throw new ArgumentException("File is empty");

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                UseFilename = false,
                UniqueFilename = true,
                Overwrite = false
            };

            var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

            if (result.Error != null)
                throw new Exception(result.Error.Message);

            return new CloudUploadResult
            {
                PublicId = result.PublicId,
                Url = result.SecureUrl.ToString()
            };
        }

        public async Task<List<CloudUploadResult>> UploadManyAsync(
            IEnumerable<IFormFile> files,
            string? folder = null,
            CancellationToken cancellationToken = default)
        {
            var results = new List<CloudUploadResult>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                    continue;

                // Reuse upload đơn
                var result = await UploadAsync(file, folder, cancellationToken);
                results.Add(result);
            }

            return results;
        }


        public async Task DeleteAsync(string publicId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return;

            var deleteParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image
            };

            var result = await _cloudinary.DestroyAsync(deleteParams);

            // "ok" hoặc "not found" đều coi là xoá thành công
            if (result.Result != "ok" && result.Result != "not found")
            {
                throw new Exception($"Delete failed: {result.Result}");
            }
        }

    }
}
