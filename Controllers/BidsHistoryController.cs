using bidify_be.Domain.Contracts;
using bidify_be.DTOs.BidsHistory;
using bidify_be.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace bidify_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BidsHistoryController : ControllerBase
    {
        private readonly IBidsHistoryService _bidsHistoryService;
        public BidsHistoryController(IBidsHistoryService bidsHistoryService)
        {
            _bidsHistoryService = bidsHistoryService;
        }

        [HttpGet("auction/{auctionId:guid}")]
        public async Task<ActionResult<ApiResponse<List<BidsHistoryResponse>>>> GetBidsHistoryByAuctionId(Guid auctionId, [FromQuery] int skip = 0,
            [FromQuery] int take = 10)
        {
            var result = await _bidsHistoryService.GetBidsHistoriesByAuctionIdAsync(auctionId, skip, take);
            return Ok(ApiResponse<List<BidsHistoryResponse>>.SuccessResponse(
                result,
                "Bids histories retrieved successfully"));
        }

        [HttpGet("user")]
        public async Task<ActionResult<ApiResponse<List<BidsHistoryResponse>>>> GetBidsHistoryByAuctionId([FromQuery] int skip = 0,
            [FromQuery] int take = 10)
        {
            var result = await _bidsHistoryService.GetBidsHistoriesByUserIdAsync(skip, take);
            return Ok(ApiResponse<List<BidsHistoryResponse>>.SuccessResponse(
                result,
                "Bids histories retrieved successfully"));
        }
    }
}
