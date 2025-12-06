using bidify_be.Domain.Contracts;
using bidify_be.DTOs.PackageBid;
using bidify_be.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bidify_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageBidController : ControllerBase
    {
        private readonly IPackageBidService _packageBidService;

        public PackageBidController(IPackageBidService packageBidService)
        {
            _packageBidService = packageBidService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<PackageBidResponse>>>> GetAllPackageBids()
        {
            var response = await _packageBidService.GetAllAsync();

            return Ok(ApiResponse<IEnumerable<PackageBidResponse>>.SuccessResponse(
                response, "Package bids retrieved successfully"
            ));
        }

        [HttpPost]
        //[Authorize(Roles = "admin")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PackageBidResponse>>> CreatePackageBid([FromBody] AddPackageBidRequest request)
        {
            var response = await _packageBidService.CreateAsync(request);
            return Ok(ApiResponse<PackageBidResponse>.SuccessResponse(
                response, "Package bid created successfully"
            ));
        }

        [HttpPut("{id:guid}")]
        //[Authorize(Roles = "admin")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PackageBidResponse>>> UpdatePackageBid([FromRoute] Guid id, [FromBody] UpdatePackageBidRequest request)
        {
            var response = await _packageBidService.UpdateAsync(id, request);
            return Ok(ApiResponse<PackageBidResponse>.SuccessResponse(
                response, "Package bid updated successfully"
            ));
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PackageBidResponse>>> GetPackageBidById([FromRoute] Guid id)
        {
            var response = await _packageBidService.GetByIdAsync(id);
            return Ok(ApiResponse<PackageBidResponse>.SuccessResponse(
                response, "Package bid retrieved successfully"
            ));
        }

        [HttpDelete("{id:guid}")]
        //[Authorize(Roles = "admin")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> DeletePackageBid([FromRoute] Guid id)
        {
            var result = await _packageBidService.DeleteAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(
                result, "Package bid deleted successfully"
            ));
        }

        [HttpPatch("toggle-active/{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> TogglePackageBidActiveStatus([FromRoute] Guid id)
        {
            var result = await _packageBidService.ToggleActiveAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(
                result, "Package bid active status toggled successfully"
            ));
        }
    }
}
