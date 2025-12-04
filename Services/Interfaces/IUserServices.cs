using bidify_be.DTOs.Auth;
using bidify_be.DTOs.Users;

namespace bidify_be.Services.Interfaces
{
    public interface IUserServices
    {
        Task<UserRegisterResponse> RegisterAsync(UserRegisterRequest request);
        Task<CurrentUserResponse> GetCurrentUserAsync();
        Task<UserResponse> GetByIdAsync(Guid id);
        Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request);
        Task DeleteAsync(Guid id);
        Task<RevokeRefreshTokenResponse> RevokeRefreshToken(RefreshTokenRequest refreshTokenRemoveRequest);
        Task<CurrentUserResponse> RefreshTokenAsync(RefreshTokenRequest request);

        Task<UserResponse> LoginAsync(UserLoginRequest request);

        Task<bool> VerifyEmail(VerifyEmailRequest user);

        Task<string> ResendVerifyCode(ResendCodeRequest request);
    }
}
