using bidify_be.Domain.Contracts;
using bidify_be.Domain.Enums;
using bidify_be.DTOs.Voucher;
using bidify_be.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bidify_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        // GET: api/voucher
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<VoucherResponse>>>> GetAllAsync()
        {
            var result = await _voucherService.GetAllVouchersAsync();
            return Ok(ApiResponse<IEnumerable<VoucherResponse>>.SuccessResponse(result, "Fetched all vouchers successfully"));
        }

        // GET: api/voucher/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<VoucherResponse>>> GetByIdAsync(Guid id)
        {
            var result = await _voucherService.GetVoucherByIdAsync(id);
            return Ok(ApiResponse<VoucherResponse>.SuccessResponse(result, "Fetched voucher successfully"));
        }

        // POST: api/voucher
        [HttpPost]
        public async Task<ActionResult<ApiResponse<VoucherResponse>>> CreateAsync([FromBody] AddVoucherRequest request)
        {
            var result = await _voucherService.AddVoucherAsync(request);
            return Ok(ApiResponse<VoucherResponse>.SuccessResponse(result, "Voucher created successfully"));
        }

        // PUT: api/voucher/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse<VoucherResponse>>> UpdateAsync(Guid id, [FromBody] UpdateVoucherRequest request)
        {
            var result = await _voucherService.UpdateVoucherAsync(id, request);
            return Ok(ApiResponse<VoucherResponse>.SuccessResponse(result, "Voucher updated successfully"));
        }

        // DELETE: api/voucher/{id}
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAsync(Guid id)
        {
            var result = await _voucherService.DeleteVoucherAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Voucher deleted successfully"));
        }

        // PATCH: api/voucher/{id}/toggle-active
        [HttpPatch("{id:guid}/toggle-active")]
        public async Task<ActionResult<ApiResponse<bool>>> ToggleActiveAsync(Guid id)
        {
            var result = await _voucherService.ToggleActiveAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Voucher status toggled successfully"));
        }

        // GET: api/voucher/status/{status}
        [HttpGet("status/{status}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<VoucherResponse>>>> GetByStatus(VoucherStatus status)
        {
            var result = await _voucherService.GetVouchersByStatusAsync(status);
            return Ok(ApiResponse<IEnumerable<VoucherResponse>>.SuccessResponse(result, "Fetched vouchers by status successfully"));
        }

        // GET: api/voucher/package-bid/{packageBidId}
        [HttpGet("package-bid/{packageBidId:guid}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<VoucherResponse>>>> GetByPackageBidId(Guid packageBidId)
        {
            var result = await _voucherService.GetVouchersByPackageBidIdAsync(packageBidId);
            return Ok(ApiResponse<IEnumerable<VoucherResponse>>.SuccessResponse(result, "Fetched vouchers by package bid successfully"));
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<PagedResult<VoucherResponse>>>> GetAllAsync([FromQuery] VoucherQueryRequest request)
        {
            var result = await _voucherService.QueryAsync(request);
            return Ok(ApiResponse<PagedResult<VoucherResponse>>.SuccessResponse(
                result,
                "Fetched vouchers successfully"
            ));
        }

    }
}
