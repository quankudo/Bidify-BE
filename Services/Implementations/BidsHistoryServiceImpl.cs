using bidify_be.Domain.Entities;
using bidify_be.DTOs.BidsHistory;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Services.Interfaces;

namespace bidify_be.Services.Implementations
{
    public class BidsHistoryServiceImpl : IBidsHistoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUserService;

        public BidsHistoryServiceImpl(IUnitOfWork uow, ICurrentUserService currentUserService)
        {
            _uow = uow;
            _currentUserService = currentUserService;
        }

        public async Task<BidsHistory> CreateBidsHistoryAsync(CreateBidsHistoryRequest request)
        {
            var bidsHistory = new BidsHistory
            {
                UserId = request.UserId,
                AuctionId = request.AuctionId,
                Price = request.Price,
                CreatedAt = DateTime.UtcNow
            };

            return await _uow.BidsHistoryRepository.AddBidsHistoryAsync(bidsHistory);
        }

        public async Task<List<BidsHistoryResponse>> GetBidsHistoriesByAuctionIdAsync(Guid auctionId, int skip, int take)
        {
            var bidsHistory = await _uow.BidsHistoryRepository.GetBidsHistoriesByAuctionIdAsync(auctionId, skip, take);
            return bidsHistory.Select(bh => new BidsHistoryResponse
            {
                Id = bh.Id,
                User = new DTOs.Users.UserShortResponse
                {
                    Id = bh.User.Id,
                    UserName = bh.User.UserName,
                    Avatar = bh.User.Avatar,
                    RateStar = bh.User.RateStar
                },
                AuctionId = bh.AuctionId,
                Price = bh.Price,
                CreatedAt = bh.CreatedAt
            }).ToList();
        }

        public async Task<List<BidsHistoryResponse>> GetBidsHistoriesByUserIdAsync(int skip, int take)
        {
            var userId = _currentUserService.GetUserId();

            var bidsHistory = await _uow.BidsHistoryRepository.GetBidsHistoriesByUserIdAsync(userId, skip, take);

            return bidsHistory.Select(bh => new BidsHistoryResponse
            {
                Id = bh.Id,
                User = new DTOs.Users.UserShortResponse
                {
                    Id = bh.User.Id,
                    UserName = bh.User.UserName,
                    Avatar = bh.User.Avatar,
                    RateStar = bh.User.RateStar
                },
                AuctionId = bh.AuctionId,
                Price = bh.Price,
                CreatedAt = bh.CreatedAt
            }).ToList();
        }
    }
}
