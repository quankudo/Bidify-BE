using AutoMapper;
using bidify_be.Domain.Constants;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Auth;
using bidify_be.DTOs.Users;
using bidify_be.Infrastructure.Context;
using bidify_be.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace bidify_be.Services.Implementations
{
    public class UserServiceImpl : IUserServices
    {
        private readonly ITokenService _tokenService;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserServiceImpl> _logger;
        private readonly ApplicationDbContext _dbContext;

        public UserServiceImpl(
            ITokenService tokenService, 
            ICurrentUserService currentUserService, 
            UserManager<ApplicationUser> userManager, 
            IMapper mapper, 
            ILogger<UserServiceImpl> logger,
            ApplicationDbContext dbContext
            )
        {
            _tokenService = tokenService;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<UserRegisterResponse> RegisterAsync(UserRegisterRequest request)
        {
            _logger.LogInformation("Registering user");
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogError("Email already exists");
                throw new Exception("Email already exists");
            }

            var newUser = _mapper.Map<ApplicationUser>(request);

            newUser.Status = true;
            newUser.RateStar = 5;
            newUser.BidCount = 0;
            newUser.Balance = 0;
            newUser.CreateAt = DateTime.Now;
            newUser.UpdateAt = DateTime.Now;
            
            newUser.VerifyCode = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            newUser.ExpireVerifyCode = DateTime.UtcNow.AddMinutes(3);
            var result = await _userManager.CreateAsync(newUser, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to create user: {errors}", errors);
                throw new Exception($"Failed to create user: {errors}");
            }

            _logger.LogInformation("User created successfully");
            await _userManager.AddToRoleAsync(newUser, AppRoles.User);

            return _mapper.Map<UserRegisterResponse>(newUser);
        }

        public async Task<UserResponse> LoginAsync(UserLoginRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Login request is null");
                throw new ArgumentNullException(nameof(request));
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                _logger.LogError("Invalid email or password");
                throw new Exception("Invalid email or password");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? AppRoles.User;

            // Generate access token
            var token = await _tokenService.GenerateToken(user);

            // Generate refresh token
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Hash the refresh token and store it in the database or override the existing refresh token
            using var sha256 = SHA256.Create();
            var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
            user.RefreshToken = Convert.ToBase64String(refreshTokenHash);
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(2);

            user.CreateAt = DateTime.Now;
            // Update user information in database
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to update user: {errors}", errors);
                throw new Exception($"Failed to update user: {errors}");
            }

            var userResponse = _mapper.Map<ApplicationUser, UserResponse>(user);
            userResponse.AccessToken = token;
            userResponse.RefreshToken = refreshToken;
            userResponse.Role = role;

            return userResponse;
        }

        public async Task<UserResponse> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting user by id");
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogError("User not found");
                throw new Exception("User not found");
            }
            _logger.LogInformation("User found");
            return _mapper.Map<UserResponse>(user);
        }

        public async Task<CurrentUserResponse> GetCurrentUserAsync()
        {
            var user = await _userManager.FindByIdAsync(_currentUserService.GetUserId());
            if (user == null)
            {
                _logger.LogError("User not found");
                throw new Exception("User not found");
            }
            return _mapper.Map<CurrentUserResponse>(user);
        }

        public async Task<CurrentUserResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            _logger.LogInformation("RefreshToken");

            // Hash the incoming RefreshToken and compare it with the one stored in the database
            using var sha256 = SHA256.Create();
            var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(request.RefreshToken));
            var hashedRefreshToken = Convert.ToBase64String(refreshTokenHash);

            // Find user based on the refresh token
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == hashedRefreshToken);
            if (user == null)
            {
                _logger.LogError("Invalid refresh token");
                throw new Exception("Invalid refresh token");
            }

            // Validate the refresh token expiry time
            if (user.RefreshTokenExpiryTime < DateTime.Now)
            {
                _logger.LogWarning("Refresh token expired for user ID: {UserId}", user.Id);
                throw new Exception("Refresh token expired");
            }

            // Generate a new access token
            var newAccessToken = await _tokenService.GenerateToken(user);
            _logger.LogInformation("Access token generated successfully");
            var currentUserResponse = _mapper.Map<CurrentUserResponse>(user);
            currentUserResponse.AccessToken = newAccessToken;
            return currentUserResponse;
        }

        public async Task<RevokeRefreshTokenResponse> RevokeRefreshToken(RefreshTokenRequest refreshTokenRemoveRequest)
        {
            _logger.LogInformation("Revoking refresh token");

            try
            {
                // Hash the refresh token
                using var sha256 = SHA256.Create();
                var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshTokenRemoveRequest.RefreshToken));
                var hashedRefreshToken = Convert.ToBase64String(refreshTokenHash);

                // Find the user based on the refresh token
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == hashedRefreshToken);
                if (user == null)
                {
                    _logger.LogError("Invalid refresh token");
                    throw new Exception("Invalid refresh token");
                }

                // Validate the refresh token expiry time
                if (user.RefreshTokenExpiryTime < DateTime.Now)
                {
                    _logger.LogWarning("Refresh token expired for user ID: {UserId}", user.Id);
                    throw new Exception("Refresh token expired");
                }

                // Remove the refresh token
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;

                // Update user information in database
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to update user");
                    return new RevokeRefreshTokenResponse
                    {
                        Message = "Failed to revoke refresh token"
                    };
                }
                _logger.LogInformation("Refresh token revoked successfully");
                return new RevokeRefreshTokenResponse
                {
                    Message = "Refresh token revoked successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to revoke refresh token: {ex}", ex.Message);
                throw new Exception("Failed to revoke refresh token");
            }
        }

        public async Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogError("User not found");
                throw new Exception("User not found");
            }

            user.UpdateAt = DateTime.Now;
            user.UserName = request.UserName;
            user.Email = request.Email;
            user.Gender = request.Gender;

            await _userManager.UpdateAsync(user);
            return _mapper.Map<UserResponse>(user);
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogWarning("Delete user failed: user {UserId} not found", id);
                throw new KeyNotFoundException("User not found");
            }

            if (!user.Status)
            {
                _logger.LogInformation("User {UserId} already deactivated", id);
                return;
            }

            user.Status = false;
            user.UpdateAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to deactivate user {UserId}: {errors}", id, errors);
                throw new Exception($"Failed to deactivate user: {errors}");
            }
        }


        public async Task<bool> VerifyEmail(VerifyEmailRequest request)
        {
            _logger.LogInformation("Verifying email for user {UserId}", request.Id);

            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
            {
                _logger.LogError("User not found: {UserId}", request.Id);
                throw new Exception("User not found");
            }

            ValidateEmailVerification(user, request);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                user.EmailConfirmed = true;
                user.Status = true;
                user.VerifyCode = null;
                user.ExpireVerifyCode = null;
                user.ReferralCode = Guid.NewGuid().ToString("N").Substring(0, 15).ToUpper();

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        _logger.LogError("Failed to update user: {Error}", error.Description);

                    throw new Exception("Failed to verify email");
                }

                // Xử lý referral nếu có
                if (!string.IsNullOrEmpty(user.ReferredBy))
                {
                    var referralUser = await _userManager.Users
                        .FirstOrDefaultAsync(u => u.ReferralCode == user.ReferredBy);

                    if (referralUser != null)
                    {
                        referralUser.BidCount += 10;

                        var referralResult = await _userManager.UpdateAsync(referralUser);
                        if (!referralResult.Succeeded)
                        {
                            _logger.LogError("Failed to update referral user balance: {UserId}", referralUser.Id);
                            throw new Exception("Failed to update referral user balance");
                        }
                    }
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Transaction rolled back due to error: {Message}", ex.Message);
                throw;
            }
        }


        private void ValidateEmailVerification(ApplicationUser user, VerifyEmailRequest request)
        {
            if (user.Email != request.Email)
                throw new Exception("Email does not match");

            if (user.VerifyCode != request.Code)
                throw new Exception("Verify code does not match");

            if (user.ExpireVerifyCode == null || user.ExpireVerifyCode < DateTime.UtcNow)
                throw new Exception("Verify code expired");
            if (user.Status) throw new Exception("User is active");
        }


        public async Task<string> ResendVerifyCode(ResendCodeRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null) throw new Exception("User not found");
            if (user.Email != request.Email) throw new Exception("Email does not match");
            if (user.Status) throw new Exception("User is active");

            var now = DateTime.UtcNow;
            var codeLifetime = TimeSpan.FromMinutes(3); // mã sống 3 phút
            var resendDelay = TimeSpan.FromMinutes(1);  // khoảng thời gian chờ 1 phút

            if (user.ExpireVerifyCode != null)
            {
                var lastSentTime = user.ExpireVerifyCode.Value - codeLifetime; // thời điểm gửi lần trước
                if (now < lastSentTime + resendDelay)
                {
                    var waitSeconds = (lastSentTime + resendDelay - now).TotalSeconds;
                    return $"Please wait {Math.Ceiling(waitSeconds)} seconds before resending code";
                }
            }

            // Tạo code mới
            user.VerifyCode = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            user.ExpireVerifyCode = now.Add(codeLifetime);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new Exception("Failed to resend verification code");

            return user.VerifyCode;
        }

    }
}
