using bidify_be.Domain.Contracts;
using bidify_be.DTOs.Auth;
using bidify_be.DTOs.Users;
using bidify_be.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace bidify_be.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserServices _userService;

        public AuthController(IUserServices userService)
        {
            _userService = userService;
        }

        [HttpPost("auth/register")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<UserRegisterResponse>>> Register([FromBody] UserRegisterRequest request)
        {
            var response = await _userService.RegisterAsync(request);
            return Ok(ApiResponse<UserRegisterResponse>.SuccessResponse(
                response, "User registered successfully"
            ));
        }


        [HttpPost("auth/login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<UserResponse>>> Login([FromBody] UserLoginRequest request)
        {
            var response = await _userService.LoginAsync(request);
            return Ok(ApiResponse<UserResponse>.SuccessResponse(
                response, "Login successful"
            ));
        }

        [HttpPost("auth/verify-email")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            await _userService.VerifyEmail(request);

            return Ok(ApiResponse<bool>.SuccessResponse(
                true,
                "Email verified successfully"
            ));
        }

        [HttpPost("auth/change-password")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            await _userService.ChangePassword(request);

            return Ok(ApiResponse<bool>.SuccessResponse(
                true,
                "Change password successfully"
            ));

        }

        [HttpPost("auth/forget-password")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> ForgetPassword([FromBody] ForgetPasswordRequest request)
        {
            await _userService.ForgetPassword(request);

            return Ok(ApiResponse<bool>.SuccessResponse(
                true,
                "Reset password successfully. Please check your email."
            ));
        }


        [HttpPost("auth/resend-code")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<bool>>> ResendCode([FromBody] ResendCodeRequest request)
        {
            await _userService.ResendVerifyCode(request);

            return Ok(ApiResponse<bool>.SuccessResponse(
                true,
                "Verification code resent successfully"
            ));
        }


        [HttpGet("user/{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserResponse>>> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound(ApiResponse<UserResponse>.FailResponse(
                    "User not found"
                ));
            }

            return Ok(ApiResponse<UserResponse>.SuccessResponse(
                user,
                "User retrieved successfully"
            ));
        }

        [HttpPost("auth/refresh-token")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<RefreshTokenResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _userService.RefreshTokenAsync(request);

            if (result == null)
            {
                return BadRequest(ApiResponse<RefreshTokenResponse>.FailResponse(
                    "Invalid or expired refresh token"
                ));
            }

            return Ok(ApiResponse<RefreshTokenResponse>.SuccessResponse(
                result,
                "Token refreshed successfully"
            ));
        }


        [HttpPost("auth/revoke-refresh-token")]
        [Authorize]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request)
        {
            var response = await _userService.RevokeRefreshToken(request);
            if (response != null && response.Message == "Refresh token revoked successfully")
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpGet("current-user")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var response = await _userService.GetCurrentUserAsync();
            return Ok(response);
        }


        [HttpDelete("user/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userService.DeleteAsync(id);
            return Ok();
        }
    }
}
