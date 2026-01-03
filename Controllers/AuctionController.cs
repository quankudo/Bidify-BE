using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using bidify_be.Services.Interfaces;
using bidify_be.DTOs.Auction;
using bidify_be.Domain.Contracts;

namespace bidify_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionService _auctionService;

        public AuctionController(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        // ------------------ CREATE ------------------
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<Guid>>> Create(
            [FromBody] AddAuctionRequest request)
        {
            var result = await _auctionService.CreateAuctionAsync(request);

            return Ok(ApiResponse<Guid>.SuccessResponse(
                result,
                "Auction created successfully"));
        }

        // ------------------ UPDATE ------------------
        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> Update(
            [FromRoute] Guid id,
            [FromBody] UpdateAuctionRequest request)
        {
            var result = await _auctionService.UpdateAuctionAsync(id, request);

            return Ok(ApiResponse<bool>.SuccessResponse(
                result,
                "Auction updated successfully"));
        }

        // ------------------ CANCEL BY USER ------------------
        [HttpPut("{id:guid}/cancel")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> CancelByUser(
            [FromRoute] Guid id)
        {
            var result = await _auctionService.CancelAuctionByUserAsync(id);

            return Ok(ApiResponse<bool>.SuccessResponse(
                result,
                "Auction cancelled successfully"));
        }

        // ------------------ APPROVE (ADMIN) ------------------
        [HttpPut("{id:guid}/approve")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<bool>>> Approve(
            [FromRoute] Guid id)
        {
            var result = await _auctionService.ApproveAuctionAsync(id);

            return Ok(ApiResponse<bool>.SuccessResponse(
                result,
                "Auction approved successfully"));
        }

        // ------------------ REJECT (ADMIN) ------------------
        [HttpPut("{id:guid}/reject")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<bool>>> Reject(
            [FromRoute] Guid id,
            [FromBody] RejectAuctionRequest request)
        {
            var result = await _auctionService.RejectAuctionAsync(request, id);

            return Ok(ApiResponse<bool>.SuccessResponse(
                result,
                "Auction rejected successfully"));
        }

        // ------------------ PLACE BID ------------------
        [HttpPost("{id:guid}/bid")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> PlaceBid(
            [FromRoute] Guid id,
            [FromBody] PlaceBidRequest request)
        {
            request.AuctionId = id;

            var result = await _auctionService.PlaceBidAsync(request);

            return Ok(ApiResponse<bool>.SuccessResponse(
                result,
                "Bid placed successfully"));
        }

        // ------------------ GET ACTIVE AUCTIONS ------------------
        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PagedResult<AuctionShortResponse>>>> GetActive(
            [FromQuery] AuctionQueryRequest request)
        {
            var result = await _auctionService.GetActiveAuctionsAsync(request);

            return Ok(ApiResponse<PagedResult<AuctionShortResponse>>
                .SuccessResponse(result, "Fetched active auctions successfully"));
        }

        // ------------------ GET FOR UPDATE ------------------
        [HttpGet("{id:guid}/for-update")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<AuctionShortResponseForUpdate>>> GetForUpdate(
            [FromRoute] Guid id)
        {
            var result = await _auctionService.GetAuctionForUpdateAsync(id);

            return Ok(ApiResponse<AuctionShortResponseForUpdate>
                .SuccessResponse(result, "Fetched auction for update successfully"));
        }

        // ------------------ GET MY AUCTIONS ------------------
        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedResult<AuctionShortResponse>>>> GetMyAuctions(
            [FromQuery] AuctionQueryRequest request)
        {
            var result = await _auctionService.GetMyAuctionsAsync(request);

            return Ok(ApiResponse<PagedResult<AuctionShortResponse>>
                .SuccessResponse(result, "Fetched my auctions successfully"));
        }

        // ------------------ GET AUCTIONS FOR ADMIN ------------------
        [HttpGet("admin")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<PagedResult<AuctionShortResponse>>>> GetForAdmin(
            [FromQuery] AuctionQueryRequest request)
        {
            var result = await _auctionService.GetAuctionsForAdminAsync(request);

            return Ok(ApiResponse<PagedResult<AuctionShortResponse>>
                .SuccessResponse(result, "Fetched auctions successfully"));
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuctionDetailResponse>>> GetDetail(
            [FromRoute] Guid id)
        {
            var result = await _auctionService.GetAuctionDetailAsync(id);

            return Ok(ApiResponse<AuctionDetailResponse>.SuccessResponse(
                result,
                "Fetched auction detail successfully"
            ));
        }

        [HttpGet("{id:guid}/seller")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<AuctionDetailResponseForSeller>>> GetDetailForSeller(
            [FromRoute] Guid id)
        {
            var result = await _auctionService.GetAuctionDetailForSellerAsync(id);

            return Ok(ApiResponse<AuctionDetailResponseForSeller>.SuccessResponse(
                result,
                "Fetched auction detail for seller successfully"
            ));
        }

    }
}