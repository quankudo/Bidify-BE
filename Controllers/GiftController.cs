using bidify_be.Domain.Contracts;
using bidify_be.DTOs.Gift;
using bidify_be.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace bidify_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "admin")]
    [AllowAnonymous]
    public class GiftController : ControllerBase
    {
        private readonly IGiftService _giftService;

        public GiftController(IGiftService giftService)
        {
            _giftService = giftService;
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<PagedResult<GiftResponse>>>> SearchAsync([FromQuery] GiftQueryRequest req)
        {
            var result = await _giftService.SearchAsync(req);
            return Ok(ApiResponse<PagedResult<GiftResponse>>.SuccessResponse(result, "Fetched gifts successfully"));
        }

        // GET: api/gift/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<GiftResponse>>> GetByIdAsync(Guid id)
        {
            var result = await _giftService.GetByIdAsync(id);
            return Ok(ApiResponse<GiftResponse>.SuccessResponse(result, "Fetched gift successfully"));
        }

        // POST: api/gift
        [HttpPost]
        public async Task<ActionResult<ApiResponse<GiftResponse>>> CreateAsync([FromBody] AddGiftRequest request)
        {
            var result = await _giftService.CreateAsync(request);

            return Ok(ApiResponse<GiftResponse>.SuccessResponse(result, "Gift created successfully"));
        }

        // PUT: api/gift/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse<GiftResponse>>> UpdateAsync(Guid id, [FromBody] UpdateGiftRequest request)
        {
            var result = await _giftService.UpdateAsync(id, request);
            return Ok(ApiResponse<GiftResponse>.SuccessResponse(result, "Gift updated successfully"));
        }

        // DELETE: api/gift/{id}
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAsync(Guid id)
        {
            var result = await _giftService.DeleteAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Gift deleted successfully"));
        }

        // PATCH: api/gift/{id}/toggle-active
        [HttpPatch("{id:guid}/toggle-active")]
        public async Task<ActionResult<ApiResponse<bool>>> ToggleActiveAsync(Guid id)
        {
            var result = await _giftService.ToggleActiveAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Gift status toggled successfully"));
        }
    }
}
