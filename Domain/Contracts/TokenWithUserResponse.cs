using bidify_be.DTOs.Users;

namespace bidify_be.Domain.Contracts
{
    public class TokenWithUserResponse
    {
        public string? RefreshToken { get; set; }
        public string? AccessToken { get; set; }
        public UserResponse User { get; set; }
    }
}
