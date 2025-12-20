using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using bidify_be.DTOs;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

public class WalletService : IWalletService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public WalletService(
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task CreditAsync(
        ApplicationUser user,
        decimal amount,
        WalletTransactionType type,
        Guid referenceId,
        string description)
    {
        if (amount <= 0)
            throw new Exception("Invalid amount");

        var before = user.Balance;
        user.Balance += amount;

        var walletTx = new WalletTransaction
        {
            UserId = user.Id,
            Amount = amount,
            Type = type,
            BalanceBefore = before,
            BalanceAfter = user.Balance,
            ReferenceId = referenceId,
            Description = description
        };

        await _unitOfWork.WalletTransactionRepository.AddAsync(walletTx);
        await _userManager.UpdateAsync(user);
    }

    public async Task<List<WalletTransaction>> GetAllByUserIdAsync(WalletTransactionQuery req)
    {
        var userId = _currentUserService.GetUserId();
        return await _unitOfWork.WalletTransactionRepository.GetAllByUserIdAsync(req, userId);
    }
}
