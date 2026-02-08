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

        public Guid? GetUserId()
        {
            var userIdString = _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userIdString, out var userId))
            {
                return userId;
            }

            return null;
        }

        public bool IsAdmin()
        {
            // Lấy tất cả claims Role
            var roles = _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role);

            // Kiểm tra có role "Admin" hay không
            return roles != null && roles.Any(r => r.Value.ToLower() == "admin");
        }

    }
}
