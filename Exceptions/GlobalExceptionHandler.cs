using bidify_be.Domain.Contracts;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace bidify_be.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (httpContext.Response.HasStarted)
            {
                _logger.LogWarning("Response has already started. Cannot write error response.");
                return false;
            }

            var traceId = httpContext.TraceIdentifier;
            var response = new ErrorResponse { TraceId = traceId };

            switch (exception)
            {
                case ValidationException vex:
                    response.StatusCode = vex.StatusCode;
                    response.Title = "Validation Error";
                    response.Message = vex.Message;
                    response.ErrorCode = vex.ErrorCodeString;
                    response.Errors = vex.Errors; 
                    break;

                case AppException appEx:
                    _logger.LogWarning(exception,
                        "Handled AppException: {Message} | ErrorCode: {ErrorCode} | TraceId: {TraceId}",
                        exception.Message, appEx.ErrorCodeString, traceId);

                    response.StatusCode = appEx.StatusCode;
                    response.Title = appEx.GetType().Name;
                    response.Message = appEx.Message;
                    response.ErrorCode = appEx.ErrorCodeString;
                    break;

                case BadHttpRequestException badReqEx:
                    _logger.LogWarning(badReqEx, "BadHttpRequest: {Message} | TraceId: {TraceId}", badReqEx.Message, traceId);

                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Title = badReqEx.GetType().Name;
                    response.Message = "Bad request.";
                    break;

                default:
                    _logger.LogError(exception,
                        "Unhandled exception: {Message} | TraceId: {TraceId}",
                        exception.Message, traceId);

                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Title = "Internal Server Error";
                    response.Message = "An unexpected error occurred."; 
                    break;
            }

            httpContext.Response.StatusCode = (int)response.StatusCode;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true;
        }
    }
}


//{
//  "title": "ValidationException",
//  "message": "Email không hợp lệ",
//  "errorCode": "EMAIL_INVALID",
//  "statusCode": 400,
//  "traceId": "0HL5L9RKKJQ4K:00000001"
//}

//FE
//switch (error.errorCode)
//{
//    case "EMAIL_INVALID":
//        showMessage("Vui lòng nhập đúng email");
//        break;
//    case "PASSWORD_TOO_SHORT":
//        showMessage("Mật khẩu quá ngắn");
//        break;
//}
