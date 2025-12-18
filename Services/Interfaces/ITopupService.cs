using bidify_be.Domain.Enums;
using bidify_be.DTOs.Topup;

namespace bidify_be.Services.Interfaces
{
    public interface ITopupService
    {

        Task<CreateTopupResult> CreateTopupAsync(
        decimal amount,
        PaymentMethod paymentMethod,
        HttpContext httpContext
    );

        Task HandleTopupSuccessAsync(
            string clientOrderId,
            string transactionCode,
            decimal paidAmount,
            string rawResponse
        );
    }

}
