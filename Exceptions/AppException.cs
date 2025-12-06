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
    

    public class UserNotFoundException : AppException
    {
        public UserNotFoundException(string message, ErrorCode errorCode = ErrorCode.UserNotFound)
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
