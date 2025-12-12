using AutoMapper;
using bidify_be.Domain.Constants;
using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Auth;
using bidify_be.DTOs.Users;
using bidify_be.Exceptions;
using bidify_be.Helpers;
using bidify_be.Infrastructure.Context;
using bidify_be.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using UnauthorizedAccessException = bidify_be.Exceptions.UnauthorizedAccessException;

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
        private readonly IEmailService _emailService;
        private readonly IValidator<UserRegisterRequest> _validatorRegister;
        private readonly IValidator<UserLoginRequest> _validatorLogin;
        private readonly IValidator<UpdateUserRequest> _validatorUpdateUser;

        public UserServiceImpl(
            ITokenService tokenService, 
            ICurrentUserService currentUserService, 
            UserManager<ApplicationUser> userManager, 
            IMapper mapper, 
            ILogger<UserServiceImpl> logger,
            ApplicationDbContext dbContext,
            IEmailService emailService,
            IValidator<UserRegisterRequest> validatorRegister,
            IValidator<UserLoginRequest> validatorLogin,
            IValidator<UpdateUserRequest> validatorUpdateUser
            )
        {
            _tokenService = tokenService;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            _dbContext = dbContext;
            _emailService = emailService;
            _validatorRegister = validatorRegister;
            _validatorLogin = validatorLogin;
            _validatorUpdateUser = validatorUpdateUser;
        }

        public async Task<UserRegisterResponse> RegisterAsync(UserRegisterRequest request)
        {
            _logger.LogInformation("Registering user: {Email}", request.Email);

            var validationResult = await _validatorRegister.ValidateAsync(request);
            ValidationHelper.ThrowIfInvalid(validationResult, _logger);

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                throw new InvalidOperationException("Email already exists");

            var newUser = _mapper.Map<ApplicationUser>(request);

            newUser.Status = false;
            newUser.RateStar = 5;
            newUser.BidCount = 0;
            newUser.Balance = 0;
            newUser.CreateAt = DateTime.UtcNow;
            newUser.UpdateAt = DateTime.UtcNow;

            newUser.VerifyCode = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            newUser.ExpireVerifyCode = DateTime.UtcNow.AddMinutes(3);

            var result = await _userManager.CreateAsync(newUser, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to create user: {Errors}", errors);
                throw new InvalidOperationException(errors);
            }

            _logger.LogInformation("User created successfully: {UserId}", newUser.Id);

            await _userManager.AddToRoleAsync(newUser, AppRoles.User);
            await _emailService.SendOtpEmail(request.Email, newUser.VerifyCode);

            return _mapper.Map<UserRegisterResponse>(newUser);
        }


        public async Task<TokenWithUserResponse> LoginAsync(UserLoginRequest request)
        {
            _logger.LogInformation("Login user: {Email}", request.Email);

            var validationResult = await _validatorLogin.ValidateAsync(request);
            ValidationHelper.ThrowIfInvalid(validationResult, _logger);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                _logger.LogError("Invalid email or password");
                throw new InvalidCredentialsException("Invalid email or password");
            }

            if(!user.Status)
            {
                _logger.LogInformation("This Account of user {UserName} was locked", user.UserName);
                throw new UnauthorizedAccessException("This account was locked");
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
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

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
            userResponse.Role = role;
            var response = new TokenWithUserResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                User = userResponse
            };

            return response;
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

        public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            _logger.LogInformation("RefreshToken");

            // Hash the incoming RefreshToken
            using var sha256 = SHA256.Create();
            var hashedInputToken = Convert.ToBase64String(
                sha256.ComputeHash(Encoding.UTF8.GetBytes(request.RefreshToken))
            );

            // Find user by hashed refresh token
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == hashedInputToken);

            if (user == null)
            {
                _logger.LogError("Invalid refresh token");
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            // Validate expiry
            if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                _logger.LogWarning("Expired refresh token for User {UserId}", user.Id);
                throw new SecurityTokenExpiredException("Refresh token expired");
            }

            // Generate new access token
            var newAccessToken = await _tokenService.GenerateToken(user);
            _logger.LogInformation("Access token created");

            return new RefreshTokenResponse
            {
                AccessToken = newAccessToken
            };
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

        public async Task<UserResponse> UpdateAsync(UpdateUserRequest request)
        {
            _logger.LogInformation("Updating user");

            var validationResult = await _validatorUpdateUser.ValidateAsync(request);
            ValidationHelper.ThrowIfInvalid(validationResult, _logger);

            var userId = _currentUserService.GetUserId();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError("User not found");
                throw new UserNotFoundException("User not found");
            }

            user.UpdateAt = DateTime.UtcNow;
            user.UserName = request.UserName;
            user.Gender = request.Gender;
            user.Avatar = request.Avatar;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError("Cập nhật user thất bại: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new Exception("Cập nhật user thất bại");
            }

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


        public async Task VerifyEmail(VerifyEmailRequest request)
        {
            _logger.LogInformation("Verifying email for user {UserId}", request.Id);

            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
            {
                _logger.LogError("User not found: {UserId}", request.Id);
                throw new UserNotFoundException("User not found");
            }

            // Validate OTP / verification code
            ValidateEmailVerification(user, request);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // Update user email confirmation and status
                user.EmailConfirmed = true;
                user.Status = true;
                user.VerifyCode = null;
                user.ExpireVerifyCode = null;

                // Generate unique referral code
                user.ReferralCode = await GenerateUniqueReferralCodeAsync();

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        _logger.LogError("Failed to update user: {Error}", error.Description);

                    throw new InvalidOperationException("Failed to verify email");
                }

                ReferralRewardEmailModel referralEmailModel = null;

                // Handle referral if exists
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
                            throw new InvalidOperationException("Failed to update referral user balance");
                        }

                        referralEmailModel = new ReferralRewardEmailModel
                        {
                            ReferrerEmail = referralUser.Email,
                            ReferrerName = referralUser.UserName,
                            NewUserName = user.UserName,
                            RewardBids = 10
                        };
                    }
                }

                await transaction.CommitAsync();

                if (referralEmailModel != null)
                {
                    try
                    {
                        await _emailService.SendReferralRewardEmail(
                            referralEmailModel.ReferrerEmail,
                            referralEmailModel.ReferrerName,
                            referralEmailModel.NewUserName,
                            referralEmailModel.RewardBids
                        );
                    }
                    catch (Exception emailEx)
                    {
                        _logger.LogError("Failed to send referral reward email: {Message}", emailEx.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Transaction rolled back due to error: {Message}", ex.Message);
                throw;
            }
        }

        // Helper: generate unique referral code
        private async Task<string> GenerateUniqueReferralCodeAsync(int length = 15)
        {
            string code;
            bool exists;
            do
            {
                code = Guid.NewGuid().ToString("N").Substring(0, length).ToUpper();
                exists = await _userManager.Users.AnyAsync(u => u.ReferralCode == code);
            } while (exists);

            return code;
        }


        public async Task ResendVerifyCode(ResendCodeRequest request)
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
                    throw new Exception($"Please wait {Math.Ceiling(waitSeconds)} seconds before resending code");
                }
            }

            // Tạo code mới
            user.VerifyCode = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            user.ExpireVerifyCode = now.Add(codeLifetime);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new Exception("Failed to resend verification code");

            await _emailService.SendOtpEmail(request.Email, user.VerifyCode);
        }

        public async Task ChangePassword(ChangePasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                _logger.LogWarning("User not found: {Email}", request.Email);
                throw new Exception("User not found");
            }

            // Check old password
            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                _logger.LogWarning("Incorrect password for user {Email}", request.Email);
                throw new Exception("Incorrect current password");
            }

            // Prevent reusing old password
            if (request.Password == request.NewPassword)
            {
                throw new Exception("New password must be different from current password");
            }

            var result = await _userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Password change failed: {Errors}", errors);
                throw new Exception(errors);
            }
        }

        public async Task ForgetPassword(ForgetPasswordRequest request)
        {
            _logger.LogInformation("Resetting password for user {Email}", request.Email);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogError("User not found: {Email}", request.Email);
                throw new UserNotFoundException("User not found");
            }

            // 1. Generate new password
            var newPassword = GenerateRandomPassword();

            // 2. Reset password properly using Identity
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    _logger.LogError("Failed to reset password: {Error}", error.Description);

                throw new InvalidOperationException("Failed to reset password");
            }

            // 3. Send email with new password
            await _emailService.SendNewPasswordEmail(request.Email, user.UserName, newPassword);

            _logger.LogInformation("Password reset email sent to {Email}", request.Email);
        }

        private string GenerateRandomPassword(int length = 12)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
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

        public async Task<PagedResult<UserResponse>> GetAllUsersAsync(UserQueryRequest req)
        {
            _logger.LogInformation("Fetching users...");

            var usersQuery = _dbContext.Users.AsNoTracking().AsQueryable();

            // SEARCH: UserName, Email, PhoneNumber
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                string keyword = req.Search.Trim().ToLower();

                usersQuery = usersQuery.Where(x =>
                    x.UserName.ToLower().Contains(keyword) ||
                    x.Email.ToLower().Contains(keyword) ||
                    (x.PhoneNumber != null && x.PhoneNumber.ToLower().Contains(keyword))
                );
            }

            // FILTER STATUS
            if (req.Status.HasValue)
            {
                usersQuery = usersQuery.Where(x => x.Status == req.Status);
            }

            // FILTER ROLE (Join AspNetUserRoles + AspNetRoles)
            if (!string.IsNullOrWhiteSpace(req.Role))
            {
                string roleName = req.Role.Trim();

                usersQuery =
                    from u in usersQuery
                    join ur in _dbContext.UserRoles on u.Id equals ur.UserId
                    join r in _dbContext.Roles on ur.RoleId equals r.Id
                    where r.Name == roleName
                    select u;
            }

            // Count tổng
            int totalItems = await usersQuery.CountAsync();

            // Select chỉ trường cần thiết (KHÔNG SELECT *)
            var users = await usersQuery
                .OrderByDescending(x => x.CreateAt)
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(x => new UserResponse
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Email = x.Email,
                    Dob = x.Dob,
                    PhoneNumber = x.PhoneNumber,
                    Gender = x.Gender,
                    Avatar = x.Avatar,
                    ReferralCode = x.ReferralCode,
                    BidCount = x.BidCount,
                    Balance = x.Balance,
                    RateStar = x.RateStar,
                    CreateAt = x.CreateAt,
                    Status = x.Status,
                    UpdateAt = x.UpdateAt
                    // Role sẽ được gán phía dưới
                })
                .ToListAsync();

            // Lấy danh sách UserId sau khi paging
            var userIds = users.Select(u => u.Id).ToList();

            // Lấy Role tương ứng mỗi user
            var userRoles = await (
                from ur in _dbContext.UserRoles
                join r in _dbContext.Roles on ur.RoleId equals r.Id
                where userIds.Contains(ur.UserId)
                select new { ur.UserId, r.Name }
            ).ToListAsync();

            // Gán role cho từng user
            foreach (var u in users)
            {
                u.Role = userRoles.FirstOrDefault(x => x.UserId == u.Id)?.Name;
            }

            return new PagedResult<UserResponse>(users, totalItems, req.Page, req.PageSize);
        }

    }
}
