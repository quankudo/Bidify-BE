using bidify_be.Services.Interfaces;
using System.Security.Claims;

namespace bidify_be.Services.Implementations
{
    public class CurrentUserServiceImpl : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserServiceImpl() { }

        public CurrentUserServiceImpl(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }
    }
}
