using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs;
using bidify_be.DTOs.Category;
using bidify_be.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bidify_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletTransactionController : ControllerBase
    {
        private readonly IWalletService _walletService;
        public WalletTransactionController(IWalletService walletService)
        {
            _walletService = walletService;
        }
        [HttpGet("user")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<WalletTransaction>>>> GetAllPagingAsync([FromQuery] WalletTransactionQuery req)
        {
            var result = await _walletService.GetAllByUserIdAsync(req);
            return Ok(ApiResponse<List<WalletTransaction>>.SuccessResponse(result));
        }
    }
}
