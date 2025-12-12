using Microsoft.AspNetCore.Mvc;
using bidify_be.Services.Interfaces;
using bidify_be.DTOs.GiftType;
using bidify_be.Domain.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace bidify_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftTypeController : ControllerBase
    {
        private readonly IGiftTypeService _giftTypeService;

        public GiftTypeController(IGiftTypeService giftTypeService)
        {
            _giftTypeService = giftTypeService;
        }

        // GET: api/gifttype
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PagedResult<GiftTypeResponse>>>> GetAllAsync([FromQuery] GiftTypeQueryRequest req)
        {
            var result = await _giftTypeService.GetAllAsync(req);
            return Ok(ApiResponse<PagedResult<GiftTypeResponse>>.SuccessResponse(result, "Fetched gift types successfully"));
        }


        // GET: api/gifttype/{id}
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<GiftTypeResponse>>> GetByIdAsync(Guid id)
        {
            var result = await _giftTypeService.GetByIdAsync(id);
            return Ok(ApiResponse<GiftTypeResponse>.SuccessResponse(result, "Fetched gift type successfully"));
        }

        // POST: api/gifttype
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<GiftTypeResponse>>> CreateAsync([FromBody] AddGiftTypeRequest request)
        {
            var result = await _giftTypeService.CreateAsync(request);
            return Ok(
                ApiResponse<GiftTypeResponse>.SuccessResponse(result, "Gift type created successfully")
            );
        }

        // PUT: api/gifttype/{id}
        [HttpPut("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<GiftTypeResponse>>> UpdateAsync(Guid id, [FromBody] UpdateGiftTypeRequest request)
        {
            var result = await _giftTypeService.UpdateAsync(id, request);
            return Ok(ApiResponse<GiftTypeResponse>.SuccessResponse(result, "Gift type updated successfully"));
        }

        // DELETE: api/gifttype/{id}
        [HttpDelete("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAsync(Guid id)
        {
            var result = await _giftTypeService.DeleteAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Gift type deleted successfully"));
        }

        // PATCH: api/gifttype/{id}/toggle-active
        [HttpPatch("{id:guid}/toggle-active")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> ToggleActiveAsync(Guid id)
        {
            var result = await _giftTypeService.ToggleActiveAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Gift type status toggled successfully"));
        }
    }
}
