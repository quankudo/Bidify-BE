using bidify_be.Domain.Contracts;
using bidify_be.DTOs.TransitionPackageBid;
using bidify_be.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bidify_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransitionPackageBidsController : ControllerBase
    {
        private readonly ITransitionPackageBidService _service;

        public TransitionPackageBidsController(ITransitionPackageBidService service)
        {
            _service = service;
        }


        [HttpPost]
        [Authorize] // user phải đăng nhập
        public async Task<ActionResult<ApiResponse<TransitionPackageBidResponse>>> Create([FromBody] TransitionPackageBidRequest request)
        {
            var entity = await _service.CreateAsync(request);

            var response = new TransitionPackageBidResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                PackageBidId = entity.PackageBidId,
                Price = entity.Price,
                BidCount = entity.BidCount,
                CreatedAt = entity.CreatedAt
            };

            return Ok(ApiResponse<TransitionPackageBidResponse>.SuccessResponse(
                response, "TransitionPackageBid created successfully"
            ));
        }


        [HttpGet("user")]
        [Authorize] 
        public async Task<ActionResult<ApiResponse<List<TransitionPackageBidResponse>>>> GetAllByUserId()
        {
            var bids = await _service.GetAllByUserIdAsync();

            return Ok(ApiResponse<List<TransitionPackageBidResponse>>.SuccessResponse(
                bids, $"Fetched successfully {bids.Count} TransitionPackageBids"
            ));
        }
    }
}
