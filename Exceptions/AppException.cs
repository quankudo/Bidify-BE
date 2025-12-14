using bidify_be.Domain.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace bidify_be.Exceptions
{
    public abstract class AppException : Exception
    {
        public int StatusCode { get; }
        public ErrorCode ErrorCode { get; }
        public string ErrorCodeString { get; }

        protected AppException(string message, int statusCode, ErrorCode errorCode)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            ErrorCodeString = errorCode.GetDescription();
        }
    }

    public class AddressLimitExceededException : AppException
    {
        public AddressLimitExceededException(string message, ErrorCode errorCode = ErrorCode.AddressLimitExceeded)
            : base(message, 400, errorCode) { }
    }

    public class EmailMismatchException : AppException
    {
        public EmailMismatchException(string message, ErrorCode errorCode = ErrorCode.EmailMismatch)
            : base(message, 400, errorCode) { }
    }

    public class InvalidVerifyCodeException : AppException
    {
        public InvalidVerifyCodeException(string message, ErrorCode errorCode = ErrorCode.InvalidVerifyCode)
            : base(message, 400, errorCode) { }
    }

    public class VerifyCodeExpiredException : AppException
    {
        public VerifyCodeExpiredException(string message, ErrorCode errorCode = ErrorCode.VerifyCodeExpired)
            : base(message, 400, errorCode) { }
    }

    public class UserAlreadyVerifiedException : AppException
    {
        public UserAlreadyVerifiedException(string message, ErrorCode errorCode = ErrorCode.UserAlreadyVerified)
            : base(message, 400, errorCode) { }
    }

    public class ResendCodeTooSoonException : AppException
    {
        public ResendCodeTooSoonException(string message, ErrorCode errorCode = ErrorCode.ResendCodeTooSoon)
            : base(message, 429, errorCode) { }
    }

    public class InvalidCredentialsException : AppException
    {
        public InvalidCredentialsException(string message, ErrorCode errorCode = ErrorCode.InvalidCredentials)
            : base(message, 401, errorCode) { }
    }

    public class UserNotFoundException : AppException
    {
        public UserNotFoundException(string message, ErrorCode errorCode = ErrorCode.UserNotFound)
            : base(message, 404, errorCode) { }
    }

    public class ProductNotFoundException : AppException
    {
        public ProductNotFoundException(string message, ErrorCode errorCode = ErrorCode.ProductNotFound)
            : base(message, 404, errorCode) { }
    }

    public class AddressNotFoundException : AppException
    {
        public AddressNotFoundException(string message, ErrorCode errorCode = ErrorCode.AddressNotFound)
            : base(message, 404, errorCode) { }
    }

    public class TagNotFoundException : AppException
    {
        public TagNotFoundException(string message, ErrorCode errorCode = ErrorCode.TagNotFound)
            : base(message, 404, errorCode) { }
    }

    public class PackageBidNotFoundException : AppException
    {
        public PackageBidNotFoundException(string message, ErrorCode errorCode = ErrorCode.PackageBidNotFound)
            : base(message, 404, errorCode) { }
    }

    public class CategoryNotFoundException : AppException
    {
        public CategoryNotFoundException(string message, ErrorCode errorCode = ErrorCode.CategoryNotFound)
            : base(message, 404, errorCode) { }
    }

    public class GiftTypeNotFoundException : AppException
    {
        public GiftTypeNotFoundException(string message, ErrorCode errorCode = ErrorCode.GiftTypeNotFound)
            : base(message, 404, errorCode) { }
    }

    public class GiftNotFoundException : AppException
    {
        public GiftNotFoundException(string message, ErrorCode errorCode = ErrorCode.GiftNotFound)
            : base(message, 404, errorCode) { }
    }

    public class VoucherNotFoundException : AppException
    {
        public VoucherNotFoundException(string message, ErrorCode errorCode = ErrorCode.VoucherNotFound)
            : base(message, 404, errorCode) { }
    }

    public class ValidationException : AppException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(
            string message,
            IDictionary<string, string[]> errors,
            ErrorCode errorCode = ErrorCode.ValidationFailed)
            : base(message, 400, errorCode)
        {
            Errors = errors;
        }
    }

    //var errors = new Dictionary<string, string[]>
    //{
    //    { "Email", new[] { "Invalid email address" } },
    //    { "Password", new[] { "Password too short", "Must contain digits" } }
    //};

    //throw new ValidationException("Validation failed", errors);


    public class UnauthorizedAccessException : AppException
    {
        public UnauthorizedAccessException(string message, ErrorCode errorCode = ErrorCode.UnauthorizedAccess)
            : base(message, 400, errorCode) { }
    }

}
