using Hangfire.Dashboard;

namespace bidify_be.Infrastructure.Hangfire
{
    public class HangfireAdminAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // 1. Phải login
            if (httpContext.User?.Identity?.IsAuthenticated != true)
                return false;

            // 2. Phải có role Admin
            return httpContext.User.IsInRole("admin");
        }
    }
}
