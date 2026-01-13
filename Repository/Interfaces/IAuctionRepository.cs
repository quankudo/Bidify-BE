using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Auction;

namespace bidify_be.Repository.Interfaces
{
    public interface IAuctionRepository
    {
        Task AddAsync(Auction auction);

        Task<Auction?> GetByIdAsync(Guid auctionId, string userId);
        Task<Auction?> GetByIdAsync(Guid auctionId);

        Task<Auction?> GetByIdForBackgroudJobAsync(Guid auctionId);

        Task<Auction?> GetByIdWithLockAsync(Guid auctionId);

        Task<PagedResult<AuctionShortResponse>> GetActiveAuctionsAsync(
            AuctionQueryRequest request);

        Task<PagedResult<EndedAuctionShortResponse>> GetEndedAuctionsAsync(
            AuctionQueryRequest request);

        Task<PagedResult<AuctionShortResponse>> GetAuctionsByUserAsync(
            string userId,
            AuctionQueryRequest request);

        Task<PagedResult<AuctionShortResponse>> GetAuctionsForAdminAsync(
            AuctionQueryRequest request);

        Task<List<Auction>> GetEndedButNotProcessedAsync(
        DateTime nowUtc,
        int batchSize = 100);

        Task<Auction?> GetByIdIncludeTagsAsync(Guid auctionId);

        Task<Auction?> GetAuctionForUpdateAsync(Guid auctionId);

        void Update(Auction auction);

        Task<Auction?> GetAuctionDetailAsync(Guid auctionId);

        Task<Auction?> GetAuctionDetailForUserAsync(Guid auctionId);
    }
}
