using AutoMapper;
using bidify_be.Domain.Constants;
using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using bidify_be.DTOs.Auction;
using bidify_be.DTOs.Product;
using bidify_be.DTOs.Users;
using bidify_be.Exceptions;
using bidify_be.Helpers;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Repository.Interfaces;
using bidify_be.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UnauthorizedAccessException = bidify_be.Exceptions.UnauthorizedAccessException;

namespace bidify_be.Services.Implementations
{
    public class AuctionServiceImpl : IAuctionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuctionServiceImpl> _logger;
        private readonly IValidator<AddAuctionRequest> _addValidator;
        private readonly IValidator<UpdateAuctionRequest> _updateValidator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuctionServiceImpl(IUnitOfWork unitOfWork,
            ILogger<AuctionServiceImpl> logger,
            IValidator<AddAuctionRequest> addValidator,
            IValidator<UpdateAuctionRequest> updateValidator,
            ICurrentUserService currentUserService,
            IMapper mapper,
            UserManager<ApplicationUser> userManager
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _addValidator = addValidator;
            _updateValidator = updateValidator;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        private async Task ValidateAsync<T>(T request, IValidator<T> validator)
        {
            var result = await validator.ValidateAsync(request);
            ValidationHelper.ThrowIfInvalid(result, _logger);
        }

        private void EnsureAuthenticatedUser(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User is not authenticated");
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
        }

        private void EnsureAdmin()
        {
            if (!_currentUserService.IsAdmin())
            {
                _logger.LogWarning("Unauthorized admin operation attempted");
                throw new UnauthorizedAccessException("You do not have permission to perform this operation.");
            }
        }

        public async Task<bool> ApproveAuctionAsync(Guid auctionId)
        {
            _logger.LogInformation("Admin approving auction: {AuctionId}", auctionId);

            // 1. Auth - Admin only
            EnsureAdmin();

            // 2. Get auction
            var auction = await _unitOfWork.AuctionRepository.GetByIdAsync(auctionId);
            if (auction == null)
            {
                _logger.LogWarning("Auction not found: {AuctionId}", auctionId);
                throw new KeyNotFoundException("Auction not found.");
            }

            // 3. Business rule
            if (auction.Status != AuctionStatus.Pending)
            {
                _logger.LogWarning(
                    "Cannot approve auction {AuctionId} with status {Status}",
                    auctionId, auction.Status
                );
                throw new InvalidOperationException("Only pending auctions can be approved.");
            }

            // 4. Update
            auction.Status = AuctionStatus.Approved;
            auction.UpdatedAt = DateTime.UtcNow;

            // 5. Save
            _unitOfWork.AuctionRepository.Update(auction);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Auction approved successfully: {AuctionId}", auctionId);

            return true;
        }


        public async Task<bool> CancelAuctionByUserAsync(Guid auctionId)
        {
            _logger.LogInformation("User attempting to cancel auction: {AuctionId}", auctionId);

            // 1. Auth
            var userId = _currentUserService.GetUserId();
            EnsureAuthenticatedUser(userId);

            // 2. Get auction by id + owner
            var auction = await _unitOfWork.AuctionRepository.GetByIdAsync(auctionId, userId);
            if (auction == null)
            {
                _logger.LogWarning(
                    "Auction not found or not owned by user. AuctionId: {AuctionId}, UserId: {UserId}",
                    auctionId, userId
                );
                throw new UnauthorizedAccessException("You do not have permission to cancel this auction.");
            }

            // 3. Business rules
            var canCancel =
            auction.Status == AuctionStatus.Pending || auction.Status == AuctionStatus.Cancelled ||
            (auction.Status == AuctionStatus.Approved && auction.BidCount == 0);

            if (!canCancel)
            {
                _logger.LogWarning(
                    "Cannot cancel auction {AuctionId} with status {Status} and bid count {BidCount}",
                    auctionId, auction.Status, auction.BidCount
                );

                throw new InvalidOperationException("This auction cannot be cancelled.");
            }

            // 4. Update
            auction.Status = AuctionStatus.UserCancelled;
            auction.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.AuctionRepository.Update(auction);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Auction cancelled successfully. AuctionId: {AuctionId}, UserId: {UserId}",
                auctionId, userId
            );

            return true;
        }


        private bool CanAutoApprove(AddAuctionRequest request)
        {
            var duration = request.EndAt - request.StartAt;

            return
                duration.TotalHours >= 1 &&
                duration.TotalDays <= 7 &&
                request.StartPrice <= 100_000_000 &&
                request.StepPrice <= 1_000_000;
        }


        public async Task<Guid> CreateAuctionAsync(AddAuctionRequest request)
        {
            _logger.LogInformation("Creating auction for product: {ProductId}", request.ProductId);

            // 1. Auth
            var userId = _currentUserService.GetUserId();
            EnsureAuthenticatedUser(userId);

            // 2. Validate
            await ValidateAsync(request, _addValidator);

            // 3. Mapping
            var auction = _mapper.Map<Auction>(request);
            auction.UserId = userId;
            auction.CreatedAt = DateTime.UtcNow;
            auction.UpdatedAt = DateTime.UtcNow;

            // ⭐ Auto approve nếu thỏa điều kiện
            auction.Status = CanAutoApprove(request)
                ? AuctionStatus.Approved  
                : AuctionStatus.Pending;

            using var tx = await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.AuctionRepository.AddAsync(auction);
                await _unitOfWork.SaveChangesAsync();

                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error while creating auction");
                throw;
            }

            _logger.LogInformation(
                "Auction created with Id: {Id}, Status: {Status}",
                auction.Id,
                auction.Status
            );

            return auction.Id;
        }



        public async Task<PagedResult<AuctionShortResponse>> GetActiveAuctionsAsync(AuctionQueryRequest request)
        {
            _logger.LogInformation("Getting active auctions");

            var result = await _unitOfWork.AuctionRepository
                .GetActiveAuctionsAsync(request);

            return result;
        }


        public async Task<AuctionDetailResponse> GetAuctionDetailAsync(Guid auctionId)
        {
            var auction = await _unitOfWork.AuctionRepository.GetAuctionDetailAsync(auctionId)
                ?? throw new AuctionNotFoundException("Auction not found");

            return new AuctionDetailResponse
            {
                Id = auction.Id,
                UserId = auction.UserId,
                ProductId = auction.ProductId,
                BidCount = auction.BidCount,
                StartAt = auction.StartAt,
                EndAt = auction.EndAt,
                BuyNowPrice = auction.BuyNowPrice ?? 0,
                StepPrice = auction.StepPrice,
                StartPrice = auction.StartPrice,
                Status = auction.Status,
                Note = auction.Note,
                WinnerId = auction.WinnerId ?? string.Empty,
                CreatedAt = auction.CreatedAt,
                UpdatedAt = auction.UpdatedAt,

                Product = new ProductShortResponse
                {
                    Id = auction.Product.Id,
                    Name = auction.Product.Name,
                    Thumbnail = auction.Product.Thumbnail
                },

                User = new UserShortResponse
                {
                    Id = auction.User.Id,
                    UserName = auction.User.UserName,
                    Avatar = auction.User.Avatar,
                    RateStar = auction.User.RateStar
                },

                AuctionTag = auction.AuctionTags
                    .Select(t => new AuctionTagResponse
                    {
                        TagId = t.TagId,
                        TagName = t.Tag.Title
                    })
                    .ToList()
            };
        }

        // 🔥 Seller (không trả user info)
        public async Task<AuctionDetailResponseForSeller> GetAuctionDetailForSellerAsync(
            Guid auctionId
        )
        {
            var auction = await _unitOfWork.AuctionRepository.GetAuctionDetailAsync(auctionId)
                ?? throw new AuctionNotFoundException("Auction not found");

            var sellerId = _currentUserService.GetUserId();
            EnsureAuthenticatedUser(sellerId);

            if (auction.UserId != sellerId)
                throw new Exception("You are not the owner of this auction");

            return new AuctionDetailResponseForSeller
            {
                Id = auction.Id,
                UserId = auction.UserId,
                ProductId = auction.ProductId,
                BidCount = auction.BidCount,
                StartAt = auction.StartAt,
                EndAt = auction.EndAt,
                BuyNowPrice = auction.BuyNowPrice ?? 0,
                StepPrice = auction.StepPrice,
                StartPrice = auction.StartPrice,
                Status = auction.Status,
                Note = auction.Note,
                WinnerId = auction.WinnerId ?? string.Empty,
                CreatedAt = auction.CreatedAt,
                UpdatedAt = auction.UpdatedAt,

                Product = new ProductShortResponse
                {
                    Id = auction.Product.Id,
                    Name = auction.Product.Name,
                    Thumbnail = auction.Product.Thumbnail
                },

                AuctionTag = auction.AuctionTags
                    .Select(t => new AuctionTagResponse
                    {
                        TagId = t.TagId,
                        TagName = t.Tag.Title
                    })
                    .ToList()
            };
        }

        public async Task<PagedResult<AuctionShortResponse>> GetAuctionsForAdminAsync(AuctionQueryRequest request)
        {
            _logger.LogInformation("Admin getting auctions");

            // 1. Auth
            var userId = _currentUserService.GetUserId();
            EnsureAuthenticatedUser(userId);
            EnsureAdmin();

            // 3. Query
            var result = await _unitOfWork.AuctionRepository
                .GetAuctionsForAdminAsync(request);

            return result;
        }


        public async Task<PagedResult<AuctionShortResponse>> GetMyAuctionsAsync(AuctionQueryRequest request)
        {
            _logger.LogInformation("Getting auctions of current user");

            // 1. Auth
            var userId = _currentUserService.GetUserId();
            EnsureAuthenticatedUser(userId);

            // 3. Query
            var result = await _unitOfWork.AuctionRepository
                .GetAuctionsByUserAsync(userId, request);

            return result;
        }


        public async Task<bool> PlaceBidAsync(PlaceBidRequest request)
        {
            _logger.LogInformation(
                "User placing bid. AuctionId={AuctionId}, BidPrice={BidPrice}",
                request.AuctionId,
                request.BidPrice
            );

            // 1. Auth
            var userId = _currentUserService.GetUserId();
            EnsureAuthenticatedUser(userId);

            using var tx = await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 2. Lock auction (FOR UPDATE)
                var auction = await _unitOfWork.AuctionRepository
                    .GetByIdWithLockAsync(request.AuctionId);

                if (auction == null)
                {
                    _logger.LogWarning("Auction not found: {AuctionId}", request.AuctionId);
                    throw new KeyNotFoundException("Auction not found.");
                }

                // 3. Business rules - auction state
                if (auction.Status != AuctionStatus.Approved)
                    throw new InvalidOperationException("Auction is not open for bidding.");

                var now = DateTime.UtcNow;
                if (now < auction.StartAt || now > auction.EndAt)
                    throw new InvalidOperationException("Auction is not active.");

                if (auction.UserId == userId)
                    throw new InvalidOperationException("Owner cannot bid on own auction.");

                // 4. Lock user (FOR UPDATE)
                var user = await _unitOfWork.UserRepository
                    .GetByIdWithLockAsync(userId);

                if (user == null)
                    throw new KeyNotFoundException("User not found.");

                // 5. Check bid balance
                if (user.BidCount < BidConfig.CostPerBid)
                {
                    _logger.LogWarning(
                        "Insufficient bids. UserId={UserId}, Balance={Balance}",
                        userId,
                        user.BidCount
                    );

                    throw new InsufficientBidException();
                }

                // 6. Price validation
                var currentPrice = auction.BuyNowPrice ?? auction.StartPrice;
                var minNextPrice = currentPrice + auction.StepPrice;

                if (request.BidPrice < minNextPrice)
                    throw new InvalidOperationException(
                        $"Bid must be at least {minNextPrice}"
                    );

                // 7. Update auction
                auction.BidCount += 1;
                auction.BuyNowPrice = request.BidPrice;
                auction.WinnerId = userId;
                auction.UpdatedAt = DateTime.UtcNow;

                // 8. Deduct bids
                user.BidCount -= BidConfig.CostPerBid;

                _unitOfWork.AuctionRepository.Update(auction);
                _unitOfWork.UserRepository.Update(user);

                await _unitOfWork.SaveChangesAsync();
                await tx.CommitAsync();

                _logger.LogInformation(
                    "Bid placed successfully. AuctionId={AuctionId}, UserId={UserId}, RemainingBids={RemainingBids}",
                    auction.Id,
                    userId,
                    user.BidCount
                );

                return true;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error while placing bid");
                throw;
            }
        }




        public async Task<bool> RejectAuctionAsync(RejectAuctionRequest request, Guid AuctionId)
        {
            _logger.LogInformation(
                "Admin rejecting auction. AuctionId: {AuctionId}, Reason: {Reason}",
                AuctionId,
                request.Reason
            );

            // 1. Auth (Admin)
            EnsureAdmin();

            // 3. Get auction
            var auction = await _unitOfWork.AuctionRepository.GetByIdAsync(AuctionId);
            if (auction == null)
            {
                _logger.LogWarning("Auction not found. AuctionId: {AuctionId}", AuctionId);
                throw new KeyNotFoundException("Auction not found.");
            }

            // 4. Business rules
            if (auction.Status != AuctionStatus.Pending)
            {
                _logger.LogWarning(
                    "Cannot reject auction {AuctionId} with status {Status}",
                    AuctionId,
                    auction.Status
                );
                throw new InvalidOperationException("Only pending auctions can be rejected.");
            }

            // 5. Update
            auction.Status = AuctionStatus.Cancelled; // hoặc Rejected
            auction.Note = request.Reason;
            auction.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.AuctionRepository.Update(auction);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Auction rejected successfully. AuctionId: {AuctionId}",
                AuctionId
            );

            return true;
        }

        private void UpdateAuctionTags(Auction auction, UpdateAuctionRequest request)
        {
            var reqTagIds = request.Tags?
                .Select(x => x.TagId)
                .ToHashSet() ?? new HashSet<Guid>();

            var dbTagIds = auction.AuctionTags
                .Select(x => x.TagId)
                .ToHashSet();

            // Xóa tag không còn trong request
            foreach (var tag in auction.AuctionTags.ToList())
            {
                if (!reqTagIds.Contains(tag.TagId))
                {
                    auction.AuctionTags.Remove(tag);
                }
            }

            // Thêm tag mới
            foreach (var tagId in reqTagIds)
            {
                if (!dbTagIds.Contains(tagId))
                {
                    auction.AuctionTags.Add(new AuctionTag
                    {
                        AuctionId = auction.Id,
                        TagId = tagId
                    });
                }
            }
        }



        public async Task<bool> UpdateAuctionAsync(Guid auctionId, UpdateAuctionRequest request)
        {
            _logger.LogInformation(
                "Updating auction. AuctionId={AuctionId}",
                auctionId
            );

            // 1. Auth
            var userId = _currentUserService.GetUserId();
            EnsureAuthenticatedUser(userId);

            // 2. Validate
            await ValidateAsync(request, _updateValidator);

            using var tx = await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 3. Get auction with tags (FOR UPDATE)
                var auction = await _unitOfWork.AuctionRepository
                    .GetByIdIncludeTagsAsync(auctionId);

                if (auction == null)
                    throw new KeyNotFoundException("Auction not found.");

                // 4. Ownership check
                if (auction.UserId != userId)
                    throw new UnauthorizedAccessException("You are not the owner of this auction.");

                // 5. Check constraint
                var canUpdate =
                auction.Status == AuctionStatus.Pending || auction.Status == AuctionStatus.Cancelled || 
                auction.Status == AuctionStatus.UserCancelled ||
                (auction.Status == AuctionStatus.Approved && auction.BidCount == 0);

                if (!canUpdate)
                {
                    _logger.LogWarning(
                        "Cannot cancel auction {AuctionId} with status {Status} and bid count {BidCount}",
                        auctionId, auction.Status, auction.BidCount
                    );
                    throw new InvalidOperationException("Auction cannot be updated.");
                }

                // 6. Update basic fields
                auction.ProductId = request.ProductId;
                auction.StartAt = request.StartAt;
                auction.EndAt = request.EndAt;
                auction.StepPrice = request.StepPrice;
                auction.StartPrice = request.StartPrice;
                auction.Status = AuctionStatus.Pending;
                auction.UpdatedAt = DateTime.UtcNow;

                // 7. Update tags
                UpdateAuctionTags(auction, request);

                // 8. Persist
                _unitOfWork.AuctionRepository.Update(auction);
                await _unitOfWork.SaveChangesAsync();

                await tx.CommitAsync();

                _logger.LogInformation(
                    "Auction updated successfully. AuctionId={AuctionId}",
                    auctionId
                );

                return true;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error while updating auction. AuctionId={AuctionId}", auctionId);
                throw;
            }
        }

        public async Task<AuctionShortResponseForUpdate> GetAuctionForUpdateAsync(Guid auctionId)
        {
            var auction = await _unitOfWork.AuctionRepository.GetAuctionForUpdateAsync(auctionId);

            if (auction == null)
                throw new AuctionNotFoundException("Auction not found");

            return new AuctionShortResponseForUpdate
            {
                Id = auction.Id,
                ProductId = auction.ProductId,
                StartAt = auction.StartAt,
                EndAt = auction.EndAt,
                StartPrice = auction.StartPrice,
                StepPrice = auction.StepPrice,
                AuctionTag = auction.AuctionTags
                    .Select(at => new AuctionTagResponse
                    {
                        TagId = at.TagId,
                        TagName = at.Tag.Title,
                    })
                    .ToList()
            };
        }

    }
}
