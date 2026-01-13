using bidify_be.Domain.Contracts;
using bidify_be.DTOs.Order;
using bidify_be.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bidify_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // ========================= Admin Orders =========================
        [HttpGet("admin/search")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<PagedResult<OrderResponseForAdmin>>>> GetOrdersForAdmin([FromQuery] OrderFilterRequest filter)
        {
            var result = await _orderService.GetOrderForAdminAsync(filter);
            return Ok(ApiResponse<PagedResult<OrderResponseForAdmin>>.SuccessResponse(
                result, "Orders retrieved successfully for admin"
            ));
        }

        // ========================= Seller Orders =========================
        [HttpGet("seller/search")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedResult<OrderResponseForSeller>>>> GetOrdersForSeller([FromQuery] OrderFilterRequest filter)
        {
            var result = await _orderService.GetOrderForSellerAsync(filter);
            return Ok(ApiResponse<PagedResult<OrderResponseForSeller>>.SuccessResponse(
                result, "Orders retrieved successfully for seller"
            ));
        }

        // ========================= Winner Orders =========================
        [HttpGet("winner/search")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedResult<OrderResponseForWinner>>>> GetOrdersForWinner([FromQuery] OrderFilterRequest filter)
        {
            var result = await _orderService.GetOrderForWinnerAsync(filter);
            return Ok(ApiResponse<PagedResult<OrderResponseForWinner>>.SuccessResponse(
                result, "Orders retrieved successfully for winner"
            ));
        }

        [HttpPut("paid")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> PaidOrder([FromBody] PaidOrderRequest req)
        {
            await _orderService.PaidOrderAsync(req);
            return Ok(ApiResponse<bool>.SuccessResponse(
                true, "Orders retrieved successfully for winner"
            ));
        }
    }
}
