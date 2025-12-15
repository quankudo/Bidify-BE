using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Address;
using bidify_be.DTOs.Upload;
using bidify_be.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bidify_be.Controllers
{
    [Route("api/")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly ICloudStorageService _cloudStorageService;
        private readonly IFileStorageService _fileStorageService;

        public UploadController (ICloudStorageService cloudStorageService, IFileStorageService fileStorageService)
        {
            _cloudStorageService = cloudStorageService;
            _fileStorageService = fileStorageService;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<CloudUploadResult>>> Upload([FromForm] UploadSingleFileRequest request)
        {
            var file = request.File;

            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<CloudUploadResult>
                    .FailResponse("File is required"));

            var uploadResult = await _cloudStorageService.UploadAsync(file, "bidify");

            await _fileStorageService.CreateTempAsync(uploadResult.PublicId);

            return Ok(ApiResponse<CloudUploadResult>.SuccessResponse(
                uploadResult,
                "Upload avatar successfully"
            ));
        }



        [HttpPost("upload-multiple")]
        [Consumes("multipart/form-data")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<CloudUploadResult>>>> UploadMultiple([FromForm] UploadMultipleFileRequest request)
        {
            if (request.Files == null || request.Files.Count == 0)
                return BadRequest(ApiResponse<List<CloudUploadResult>>
                    .FailResponse("No files uploaded"));

            var uploadResults = await _cloudStorageService.UploadManyAsync(
                request.Files,
                "bidify"
            );

            foreach (var result in uploadResults)
            {
                await _fileStorageService.CreateTempAsync(result.PublicId);
            }

            return Ok(ApiResponse<List<CloudUploadResult>>.SuccessResponse(
                uploadResults,
                "Upload product images successfully"
            ));
        }



    }
}
