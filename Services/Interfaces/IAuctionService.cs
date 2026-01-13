using bidify_be.Domain.Contracts;
using bidify_be.DTOs.Auction;

namespace bidify_be.Services.Interfaces
{
    public interface IAuctionService
    {
        // ================= USER =================

        /// <summary>
        /// Tạo auction mới (User)
        /// </summary>
        Task<Guid> CreateAuctionAsync(AddAuctionRequest request);

        /// <summary>
        /// Cập nhật auction (User - chỉ khi Pending)
        /// </summary>
        Task<bool> UpdateAuctionAsync(Guid auctionId, UpdateAuctionRequest request);

        /// <summary>
        /// User tự hủy auction
        /// </summary>
        Task<bool> CancelAuctionByUserAsync(Guid auctionId);

        /// <summary>
        /// Lấy chi tiết auction
        /// </summary>
        Task<AuctionDetailResponse> GetAuctionDetailAsync(Guid auctionId);

        Task<AuctionDetailResponseForSeller> GetAuctionDetailForSellerAsync(
            Guid auctionId
        );

        /// <summary>
        /// Danh sách auction đang diễn ra (User - trang đấu giá)
        /// </summary>
        Task<PagedResult<AuctionShortResponse>>
            GetActiveAuctionsAsync(AuctionQueryRequest request);

        Task<PagedResult<EndedAuctionShortResponse>>
            GetEndedAuctionsAsync(AuctionQueryRequest request);

        /// <summary>
        /// Danh sách auction của user hiện tại (User - trang quản lý)
        /// </summary>
        Task<PagedResult<AuctionShortResponse>>
            GetMyAuctionsAsync(AuctionQueryRequest request);

        /// <summary>
        /// Cập nhật auction khi có lượt đấu mới
        /// </summary>
        Task<bool> PlaceBidAsync(PlaceBidRequest request);


        // ================= ADMIN =================

        /// <summary>
        /// Phê duyệt auction
        /// </summary>
        Task<bool> ApproveAuctionAsync(Guid auctionId);

        /// <summary>
        /// Từ chối auction (Admin)
        /// </summary>
        Task<bool> RejectAuctionAsync(RejectAuctionRequest request, Guid AuctionId);

        /// <summary>
        /// Danh sách auction (Admin)
        /// </summary>
        Task<PagedResult<AuctionShortResponse>> GetAuctionsForAdminAsync(AuctionQueryRequest request);

        Task<AuctionShortResponseForUpdate> GetAuctionForUpdateAsync(Guid auctionId);

        Task<AuctionDetailResponseForUser> GetAuctionDetailForUserAsync(Guid auctionId);
    }
}
