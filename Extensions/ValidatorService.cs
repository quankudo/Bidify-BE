using FluentValidation;

namespace bidify_be.Extensions
{
    public static class ValidatorService
    {
        public static IServiceCollection AddValidators(
            this IServiceCollection services)
        {
            // Scan toàn bộ validators trong assembly bidify-be
            services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            return services;
        }
    }
}
