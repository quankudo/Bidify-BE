using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using bidify_be.DTOs.TransitionPackageBid;
using bidify_be.Exceptions;
using bidify_be.Helpers;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Repository.Interfaces;
using bidify_be.Services.Interfaces;
using bidify_be.Validators.TransitionPackageBid;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace bidify_be.Services.Implementations
{
    public class TransitionPackageBidServiceImpl : ITransitionPackageBidService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransitionPackageBidServiceImpl> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<TransitionPackageBidRequest> _validator;
        public TransitionPackageBidServiceImpl(
            IUnitOfWork unitOfWork, 
            ICurrentUserService currentUserService, 
            ILogger<TransitionPackageBidServiceImpl> logger, 
            UserManager<ApplicationUser> userManager,
            IValidator<TransitionPackageBidRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _logger = logger;
            _userManager = userManager;
            _validator = validator;
        }
        public async Task<TransitionPackageBid> CreateAsync(TransitionPackageBidRequest request)
        {
            // Validate DTO
            var validationResult = await _validator.ValidateAsync(request);
            ValidationHelper.ThrowIfInvalid(validationResult, _logger);

            // Begin transaction
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Lấy package bid
                var packageBid = await _unitOfWork.PackageBids.GetByIdAsync(request.PackageBidId);
                if (packageBid == null)
                {
                    _logger.LogWarning("PackageBid with Id {PackageBidId} not found.", request.PackageBidId);
                    throw new PackageBidNotFoundException($"PackageBid {request.PackageBidId} not found.");
                }

                // Lấy user
                var userId = _currentUserService.GetUserId();
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with Id {UserId} not found.", userId);
                    throw new UserNotFoundException($"User {userId} not found.");
                }

                // Check đủ tiền
                if (user.Balance < packageBid.Price)
                {
                    _logger.LogWarning(
                        "User {UserId} has insufficient balance. Balance: {Balance}, Price: {Price}",
                        userId, user.Balance, packageBid.Price
                    );
                    throw new InsufficientBalanceException("Insufficient balance.");
                }

                var balanceBefore = user.Balance;
                var balanceAfter = user.Balance - packageBid.Price;

                // Update user
                user.Balance = balanceAfter;
                user.BidCount += packageBid.BidQuantity;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    _logger.LogError(
                        "Failed to update user {UserId}: {Errors}",
                        userId,
                        string.Join(", ", updateResult.Errors.Select(e => e.Description))
                    );
                    throw new InvalidOperationException("Unable to update user balance or bid count.");
                }

                // Tạo transition bid
                var transitionPackageBid = new TransitionPackageBid
                {
                    UserId = userId,
                    PackageBidId = request.PackageBidId,
                    BidCount = packageBid.BidQuantity,
                    Price = packageBid.Price,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.TransitionPackageBidRepository.CreateAsync(transitionPackageBid);

                // Tạo wallet transaction (sau khi đã có transition)
                var wallet = new WalletTransaction
                {
                    UserId = userId,
                    Amount = -packageBid.Price,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = balanceAfter,
                    CreatedAt = DateTime.UtcNow,
                    ReferenceId = transitionPackageBid.Id,
                    Type = WalletTransactionType.BuyBidPackage,
                    Description = $"Mua gói Bids: {packageBid.Title}"
                };

                await _unitOfWork.WalletTransactionRepository.AddAsync(wallet);

                // Commit
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "TransitionPackageBid created successfully. UserId: {UserId}, PackageBidId: {PackageBidId}, Price: {Price}",
                    userId, request.PackageBidId, packageBid.Price
                );

                return transitionPackageBid;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error creating TransitionPackageBid for UserId {UserId}, PackageBidId {PackageBidId}.",
                    _currentUserService.GetUserId(),
                    request.PackageBidId
                );

                await transaction.RollbackAsync();
                throw;
            }
        }



        public async Task<List<TransitionPackageBidResponse>> GetAllByUserIdAsync(TransitionPackageBidQuery req)
        {
            var userId = _currentUserService.GetUserId();
            _logger.LogInformation("Fetching all TransitionPackageBids for UserId {UserId}", userId);

            var bids = await _unitOfWork.TransitionPackageBidRepository.GetByUserIdAsync(userId, req.Skip, req.Take);

            _logger.LogInformation("Fetched {Count} TransitionPackageBids for UserId {UserId}", bids.Count, userId);

            return bids;
        }
    }
}
