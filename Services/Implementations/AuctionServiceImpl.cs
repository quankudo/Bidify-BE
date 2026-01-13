using AutoMapper;
using bidify_be.Domain.Constants;
using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.Domain.Enums;
using bidify_be.DTOs.Auction;
using bidify_be.DTOs.BidsHistory;
using bidify_be.DTOs.Product;
using bidify_be.DTOs.Users;
using bidify_be.Exceptions;
using bidify_be.Helpers;
using bidify_be.Hubs;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
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
        private readonly INotificationService _notificationService;
        private readonly IBidsHistoryService _bidsHistoryService;
        private readonly IHubContext<AppHub> _hub;

        public AuctionServiceImpl(IUnitOfWork unitOfWork,
            ILogger<AuctionServiceImpl> logger,
            IValidator<AddAuctionRequest> addValidator,
            IValidator<UpdateAuctionRequest> updateValidator,
            ICurrentUserService currentUserService,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService,
            IBidsHistoryService bidsHistoryService,
            IHubContext<AppHub> hubContext
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _addValidator = addValidator;
            _updateValidator = updateValidator;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _notificationService = notificationService;
            _bidsHistoryService = bidsHistoryService;
            _hub = hubContext;
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

            using var tx = await _unitOfWork.BeginTransactionAsync();

            try
            {
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

                // 4. Update auction
                auction.Status = AuctionStatus.Approved;
                auction.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.AuctionRepository.Update(auction);

                // 5. Notification content
                var title = "Auction Approved";
                var message =
                    $"Your auction for product #{auction.ProductId} has been approved " +
                    $"and is now ready for bidding.";

                var userIds = new[] { auction.UserId };

                // 6. Send notification (CHUNG TRANSACTION)
                var notifications = await _notificationService.SendAsync(
                    NotificationType.AUCTION_APPROVED,
                    title,
                    message,
                    userIds,
                    auction.Id
                );

                // 7. Commit DB
                await _unitOfWork.SaveChangesAsync();
                await tx.CommitAsync();

                // 8. SignalR notify (SAU COMMIT)
                await _hub.Clients.User(auction.UserId)
                    .SendAsync("ReceiveNotification", new NotificationDto
                    {
                        Id = notifications?.FirstOrDefault()?.Id,
                        NotificationType = NotificationType.AUCTION_APPROVED,
                        Title = title,
                        Message = message,
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false,
                        IsDeleted = false,
                        Mode = NotificationMode.Personal
                    });

                _logger.LogInformation(
                    "Auction approved successfully: {AuctionId}", auctionId);

                return true;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error while approving auction: {AuctionId}", auctionId);
                throw;
            }
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

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                throw new UnauthorizedAccessException("User not found.");
            }

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

            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var adminIds = admins.Select(a => a.Id);
            var title = "New Auction Created";
            var productName = auction.Product?.Name ?? $"Product #{auction.ProductId}";

            var message = $"Auction for {productName} has been created by {user.UserName}.";
            await _notificationService.SendWithSeparateTransactionAsync(NotificationType.AUCTION_CREATED, title, message, adminIds, auction.Id);

            await _hub.Clients.Group("Admins").SendAsync("ReceiveNotification", new NotificationDto
            {
                NotificationType = NotificationType.AUCTION_CREATED,
                Title = title,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                IsRead = false,
                Mode = NotificationMode.Broadcast,
            });

            return auction.Id;
        }



        public async Task<PagedResult<AuctionShortResponse>> GetActiveAuctionsAsync(AuctionQueryRequest request)
        {
            _logger.LogInformation("Getting active auctions");

            var result = await _unitOfWork.AuctionRepository
                .GetActiveAuctionsAsync(request);

            return result;
        }

        public async Task<AuctionDetailResponseForUser> GetAuctionDetailForUserAsync(Guid auctionId)
        {
            var auction = await _unitOfWork.AuctionRepository.GetAuctionDetailForUserAsync(auctionId)
                ?? throw new AuctionNotFoundException("Auction not found");

            return new AuctionDetailResponseForUser
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

                Product = _mapper.Map<ProductResponse>(auction.Product),

                User = new UserShortResponse
                {
                    Id = auction.User.Id,
                    UserName = auction.User.UserName,
                    Avatar = auction.User.Avatar,
                    RateStar = auction.User.RateStar
                },

                Winner = auction.Winner != null
                    ? new UserShortResponse
                    {
                        Id = auction.Winner.Id,
                        UserName = auction.Winner.UserName,
                        Avatar = auction.Winner.Avatar,
                        RateStar = auction.Winner.RateStar
                    }
                    : null,

                AuctionTag = auction.AuctionTags
                    .Select(t => new AuctionTagResponse
                    {
                        TagId = t.TagId,
                        TagName = t.Tag.Title
                    })
                    .ToList()
            };
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

            var userId = _currentUserService.GetUserId();
            EnsureAuthenticatedUser(userId);

            string? previousWinnerId = null;
            Auction auction;
            ApplicationUser user;
            BidsHistory bidHistory;

            await using var tx = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 1. Lock auction
                auction = await _unitOfWork.AuctionRepository
                    .GetByIdWithLockAsync(request.AuctionId)
                    ?? throw new KeyNotFoundException("Auction not found.");

                // 2. Business rules
                if (auction.Status != AuctionStatus.Approved)
                    throw new InvalidOperationException("Auction is not open for bidding.");

                var now = DateTime.UtcNow;
                if (now < auction.StartAt || now > auction.EndAt)
                    throw new InvalidOperationException("Auction is not active.");

                if (auction.UserId == userId)
                    throw new InvalidOperationException("Owner cannot bid on own auction.");

                // 3. Lock user
                user = await _unitOfWork.UserRepository
                    .GetByIdWithLockAsync(userId)
                    ?? throw new KeyNotFoundException("User not found.");

                if (user.BidCount < BidConfig.CostPerBid)
                    throw new InsufficientBidException();

                // 4. Price validation
                var currentPrice = auction.BuyNowPrice ?? auction.StartPrice;
                var minNextPrice = currentPrice + auction.StepPrice;

                if (request.BidPrice < minNextPrice)
                    throw new InvalidOperationException($"Bid must be ≥ {minNextPrice}");

                if ((request.BidPrice - currentPrice) % auction.StepPrice != 0)
                    throw new InvalidOperationException(
                        $"Bid must increase by step {auction.StepPrice}"
                    );

                // 5. Save old winner BEFORE overwrite
                previousWinnerId = auction.WinnerId;

                // 6. Update auction
                auction.BidCount += 1;
                auction.BuyNowPrice = request.BidPrice;
                auction.WinnerId = userId;
                auction.UpdatedAt = DateTime.UtcNow;

                // 7. Deduct bid
                user.BidCount -= BidConfig.CostPerBid;

                // 8. Save bid history
                var bidHistoryRequest = new CreateBidsHistoryRequest
                {
                    AuctionId = auction.Id,
                    UserId = user.Id,
                    Price = request.BidPrice
                };

                bidHistory = await _bidsHistoryService.CreateBidsHistoryAsync(bidHistoryRequest);
                _unitOfWork.AuctionRepository.Update(auction);
                _unitOfWork.UserRepository.Update(user);

                await _unitOfWork.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error while placing bid");
                throw;
            }

            // 9. Notification – previous winner (outbid)
            if (!string.IsNullOrEmpty(previousWinnerId) && previousWinnerId != userId)
            {
                await _notificationService.SendWithSeparateTransactionAsync(
                    NotificationType.NEW_BID,
                    "Bạn đã bị vượt giá",
                    $"Giá mới hiện tại là {auction.BuyNowPrice:N0}₫",
                    new[] { previousWinnerId },
                    auction.Id
                );
            }

            // 10. Notification – seller
            await _notificationService.SendWithSeparateTransactionAsync(
                NotificationType.NEW_BID,
                "Có lượt đấu giá mới",
                $"{user.UserName} vừa đặt giá {auction.BuyNowPrice:N0}₫",
                new[] { auction.UserId },
                auction.Id
            );

            var notifyPayload = new NotificationDto
            {
                NotificationType = NotificationType.NEW_BID,
                CreatedAt = DateTime.UtcNow,
                RelatedAuctionId = auction.Id,
                Mode = NotificationMode.Personal
            };

            if (previousWinnerId != null && previousWinnerId != userId)
            {
                await _hub.Clients.User(previousWinnerId)
                    .SendAsync("ReceiveNotification", notifyPayload with
                    {
                        Title = "Bạn đã bị vượt giá",
                        Message = $"Giá mới: {auction.BuyNowPrice:N0}₫"
                    });
            }

            // seller
            await _hub.Clients.User(auction.UserId)
                .SendAsync("ReceiveNotification", notifyPayload with
                {
                    Title = "Có lượt đấu giá mới",
                    Message = $"{user.UserName} vừa đặt giá {auction.BuyNowPrice:N0}₫"
                });

            // 11. Realtime broadcast
            await _hub.Clients
                .Group($"auction-{auction.Id}")
                .SendAsync("NewBid", new BidDto
                {
                    Id = bidHistory.Id,
                    AuctionId = auction.Id,
                    Price = auction.BuyNowPrice!.Value,
                    User = new UserShortResponse
                    {
                        Avatar = user.Avatar,
                        Id = user.Id,
                        RateStar = user.RateStar,
                        UserName = user.UserName
                    },
                    createdAt = DateTime.UtcNow
                });

            _logger.LogInformation(
                "Bid placed successfully. AuctionId={AuctionId}, UserId={UserId}, Price={Price}",
                auction.Id,
                userId,
                auction.BuyNowPrice
            );

            return true;
        }


        public async Task<bool> RejectAuctionAsync(
            RejectAuctionRequest request,
            Guid auctionId)
        {
            _logger.LogInformation(
                "Admin rejecting auction. AuctionId: {AuctionId}, Reason: {Reason}",
                auctionId,
                request.Reason
            );

            // 1. Auth - Admin only
            EnsureAdmin();

            using var tx = await _unitOfWork.BeginTransactionAsync();

            try
            {
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
                        "Cannot reject auction {AuctionId} with status {Status}",
                        auctionId,
                        auction.Status
                    );
                    throw new InvalidOperationException("Only pending auctions can be rejected.");
                }

                // 4. Update auction
                auction.Status = AuctionStatus.Cancelled; 
                auction.Note = request.Reason;
                auction.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.AuctionRepository.Update(auction);

                // 5. Notification content
                var title = "Auction Rejected";
                var message =
                    $"Your auction for product #{auction.ProductId} has been rejected. " +
                    $"Reason: {request.Reason}";

                var userIds = new[] { auction.UserId };

                // 6. Send notification (CHUNG TRANSACTION)
                var notifications = await _notificationService.SendAsync(
                    NotificationType.AUCTION_REJECTED,
                    title,
                    message,
                    userIds,
                    auction.Id
                );

                // 7. Commit DB
                await _unitOfWork.SaveChangesAsync();
                await tx.CommitAsync();

                // 8. SignalR notify (SAU COMMIT)
                await _hub.Clients.User(auction.UserId)
                    .SendAsync("ReceiveNotification", new NotificationDto
                    {
                        Id = notifications?.FirstOrDefault()?.Id,
                        NotificationType = NotificationType.AUCTION_REJECTED,
                        Title = title,
                        Message = message,
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false,
                        IsDeleted = false,
                        Mode = NotificationMode.Personal
                    });

                _logger.LogInformation(
                    "Auction rejected successfully: {AuctionId}",
                    auctionId
                );

                return true;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error while rejecting auction: {AuctionId}", auctionId);
                throw;
            }
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

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                throw new UnauthorizedAccessException("User not found.");
            }

            // 2. Validate
            await ValidateAsync(request, _updateValidator);

            using var tx = await _unitOfWork.BeginTransactionAsync();

            // 3. Get auction with tags (FOR UPDATE)
            var auction = await _unitOfWork.AuctionRepository
                .GetByIdIncludeTagsAsync(auctionId);

            try
            {

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
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Error while updating auction. AuctionId={AuctionId}", auctionId);
                throw;
            }

            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var adminIds = admins.Select(a => a.Id);
            var productName = auction.Product?.Name ?? $"Product #{auction.ProductId}";

            var title = auction.Status == AuctionStatus.Pending
                ? "Auction Updated (Pending Approval)"
                : "Auction Updated";

            var message = $"Auction for {productName} has been updated by {user.UserName}.";
            await _notificationService.SendWithSeparateTransactionAsync(NotificationType.AUCTION_UPDATED, title, message, adminIds, auction.Id);

            await _hub.Clients.Group("Admins").SendAsync("ReceiveNotification", new NotificationDto
            {
                NotificationType = NotificationType.AUCTION_UPDATED,
                Title = title,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                IsRead = false,
                Mode = NotificationMode.Broadcast,
            });

            return true;
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

        public async Task<PagedResult<EndedAuctionShortResponse>> GetEndedAuctionsAsync(AuctionQueryRequest request)
        {
            _logger.LogInformation("Getting ended auctions");

            var result = await _unitOfWork.AuctionRepository
                .GetEndedAuctionsAsync(request);

            return result;
        }

    }
}
