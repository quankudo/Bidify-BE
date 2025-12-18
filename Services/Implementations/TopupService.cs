using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using bidify_be.DTOs.Topup;
using bidify_be.DTOs.VNPay;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

public class TopupService : ITopupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWalletService _walletService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IVnPayService _paymentGateway; // VNPay abstraction
    private readonly ICurrentUserService _currentUserService;

    public TopupService(
        IUnitOfWork unitOfWork,
        IWalletService walletService,
        UserManager<ApplicationUser> userManager,
        IVnPayService paymentGateway,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _walletService = walletService;
        _userManager = userManager;
        _paymentGateway = paymentGateway;
        _currentUserService = currentUserService;
    }


    public async Task<CreateTopupResult> CreateTopupAsync(decimal amount, PaymentMethod paymentMethod, HttpContext httpContext)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than 0");

        var userId = _currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException();

        var clientOrderId = $"TOPUP_{Guid.NewGuid():N}";

        var paymentRequest = new PaymentInformationModel
        {
            Amount = amount,
            OrderType = "topup",
            OrderDescription = $"Topup wallet - {clientOrderId}",
            ClientOrderId = clientOrderId
        };

        var paymentUrl = _paymentGateway.CreatePaymentUrl(
            paymentRequest,
            httpContext
        );

        var topup = new TopupTransaction
        {
            UserId = userId,
            Amount = amount,
            PaymentMethod = paymentMethod,
            ClientOrderId = clientOrderId,
            Status = TopupTransactionsStatus.Pending,
            RequestPayload = JsonSerializer.Serialize(paymentRequest),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.TopupTransactionRepository.AddAsync(topup);
        await _unitOfWork.SaveChangesAsync();

        return new CreateTopupResult
        {
            ClientOrderId = clientOrderId,
            PaymentUrl = paymentUrl
        };
    }



    // ================================
    // 2️⃣ HANDLE CALLBACK SUCCESS
    // ================================
    public async Task HandleTopupSuccessAsync(
        string clientOrderId,
        string transactionCode,
        decimal paidAmount,
        string rawResponse)
    {
        using var tx = await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Lock row
            var topup = await _unitOfWork
                .TopupTransactionRepository
                .GetByClientOrderIdAsync(clientOrderId);

            if (topup == null)
                throw new Exception("Topup transaction not found");

            // Idempotent
            if (topup.Status == TopupTransactionsStatus.Success)
                return;

            // Amount mismatch → FAILED
            if (Math.Abs(topup.Amount - paidAmount) > 0.01m)
            {
                topup.Status = TopupTransactionsStatus.Failed;
                topup.ResponsePayload = rawResponse;
                topup.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.TopupTransactionRepository.UpdateAsync(topup);
                await _unitOfWork.SaveChangesAsync();
                await tx.CommitAsync();
                return;
            }

            // Success
            topup.Status = TopupTransactionsStatus.Success;
            topup.TransactionCode = transactionCode;
            topup.ResponsePayload = rawResponse;
            topup.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.TopupTransactionRepository.UpdateAsync(topup);

            var user = await _userManager.FindByIdAsync(topup.UserId);
            if (user == null)
                throw new Exception("User not found");

            await _walletService.CreditAsync(
                user,
                topup.Amount,
                WalletTransactionType.Topup,
                topup.Id,
                $"Topup via {topup.PaymentMethod} - {clientOrderId}"
            );

            await _unitOfWork.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

}
