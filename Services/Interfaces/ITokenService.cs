using bidify_be.Domain.Entities;
using bidify_be.DTOs.Auth;

namespace bidify_be.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);
        string GenerateRefreshToken();
    }
}
