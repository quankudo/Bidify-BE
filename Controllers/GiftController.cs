using Microsoft.AspNetCore.Mvc;
using bidify_be.Services.Interfaces;
using bidify_be.DTOs.Gift;
using bidify_be.Domain.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace bidify_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : ControllerBase
    {
        private readonly IGiftService _giftService;

        public GiftController(IGiftService giftService)
        {
            _giftService = giftService;
        }

        // GET: api/gift
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<GiftResponse>>>> GetAllAsync()
        {
            var result = await _giftService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<GiftResponse>>.SuccessResponse(result, "Fetched all gifts successfully"));
        }

        // GET: api/gift/{id}
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<GiftResponse>>> GetByIdAsync(Guid id)
        {
            var result = await _giftService.GetByIdAsync(id);
            return Ok(ApiResponse<GiftResponse>.SuccessResponse(result, "Fetched gift successfully"));
        }

        // POST: api/gift
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<GiftResponse>>> CreateAsync([FromBody] AddGiftRequest request)
        {
            var result = await _giftService.CreateAsync(request);

            return Ok(ApiResponse<GiftResponse>.SuccessResponse(result, "Gift created successfully"));
        }

        // PUT: api/gift/{id}
        [HttpPut("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<GiftResponse>>> UpdateAsync(Guid id, [FromBody] UpdateGiftRequest request)
        {
            var result = await _giftService.UpdateAsync(id, request);
            return Ok(ApiResponse<GiftResponse>.SuccessResponse(result, "Gift updated successfully"));
        }

        // DELETE: api/gift/{id}
        [HttpDelete("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAsync(Guid id)
        {
            var result = await _giftService.DeleteAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Gift deleted successfully"));
        }

        // PATCH: api/gift/{id}/toggle-active
        [HttpPatch("{id:guid}/toggle-active")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> ToggleActiveAsync(Guid id)
        {
            var result = await _giftService.ToggleActiveAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Gift status toggled successfully"));
        }
    }
}
