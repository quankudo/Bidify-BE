using bidify_be.Domain.Contracts;
using bidify_be.DTOs.Tags;
using bidify_be.DTOs.Topup;
using bidify_be.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bidify_be.Controllers
{
    [Route("api/")]
    [ApiController]
    public class TopupController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly ITopupService _topupService;

        public TopupController(IVnPayService vnPayService, ITopupService topupService)
        {
            _vnPayService = vnPayService;
            _topupService = topupService;
        }

        [HttpGet("Payment/PaymentCallbackVnpay")]
        public async Task<IActionResult> VNPayReturn()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (!response.Success || response.VnPayResponseCode != "00")
                return Redirect("http://localhost:5173/thanh-toan-that-bai");

            await _topupService.HandleTopupSuccessAsync(
                response.OrderId,
                response.TransactionId,
                decimal.Parse(Request.Query["vnp_Amount"]) / 100,
                Request.QueryString.Value
            );

            return Redirect("http://localhost:5173/thanh-toan-thanh-cong");
        }

        [Authorize]
        [HttpPost("Topup")]
        public async Task<ActionResult<ApiResponse<CreateTopupRequest>>> CreateTopup([FromBody] CreateTopupRequest req)
        {
            var result = await _topupService.CreateTopupAsync(
                req.Amount,
                req.PaymentMethod,
                HttpContext
            );

            return Ok(ApiResponse<CreateTopupResult>.SuccessResponse(result, "Topup successfully"));
        }

    }
}
