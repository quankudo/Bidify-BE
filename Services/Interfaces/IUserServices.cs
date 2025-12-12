using bidify_be.Domain.Contracts;
using bidify_be.DTOs.Auth;
using bidify_be.DTOs.Users;

namespace bidify_be.Services.Interfaces
{
    public interface IUserServices
    {
        Task<UserRegisterResponse> RegisterAsync(UserRegisterRequest request);
        Task<CurrentUserResponse> GetCurrentUserAsync();
        Task<UserResponse> GetByIdAsync(Guid id);
        Task<UserResponse> UpdateAsync(UpdateUserRequest request);
        Task DeleteAsync(Guid id);
        Task<RevokeRefreshTokenResponse> RevokeRefreshToken(RefreshTokenRequest refreshTokenRemoveRequest);
        Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);

        Task<TokenWithUserResponse> LoginAsync(UserLoginRequest request);

        Task VerifyEmail(VerifyEmailRequest user);

        Task ResendVerifyCode(ResendCodeRequest request);

        Task ChangePassword(ChangePasswordRequest request);

        Task ForgetPassword(ForgetPasswordRequest request);

        Task<PagedResult<UserResponse>> GetAllUsersAsync(UserQueryRequest req);
    }
}
