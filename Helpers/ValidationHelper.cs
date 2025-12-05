using bidify_be.Exceptions;
using FluentValidation.Results;

namespace bidify_be.Helpers
{
    public static class ValidationHelper
    {
        public static void ThrowIfInvalid(ValidationResult result, ILogger logger = null)
        {
            if (result.IsValid)
                return;

            // Chuyển lỗi thành Dictionary<string, string[]>
            var errors = result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            // Log nếu có logger
            if (logger != null)
            {
                var flatErrors = string.Join(", ", errors.SelectMany(e => e.Value));
                logger.LogError("Validation failed: {Errors}", flatErrors);
            }

            throw new ValidationException("Validation failed", errors);
        }
    }

}
