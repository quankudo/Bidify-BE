using bidify_be.Domain.Enums;
using bidify_be.DTOs.Product;
using bidify_be.DTOs.Users;

namespace bidify_be.DTOs.Auction
{
    // ================= BASE =================
    public abstract class AuctionBaseResponse
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid ProductId { get; set; }
        public int BidCount { get; set; } = 0;

        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }

        public decimal BuyNowPrice { get; set; }
        public decimal StepPrice { get; set; }
        public decimal StartPrice { get; set; }

        public AuctionStatus Status { get; set; }
        public string Note { get; set; } = string.Empty;
        public string WinnerId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<AuctionTagResponse> AuctionTag { get; set; } = new();
    }

    // ================= DETAIL =================
    public class AuctionDetailResponse : AuctionBaseResponse
    {
        public ProductShortResponse Product { get; set; }
        public UserShortResponse User { get; set; }
    }

    public class AuctionDetailResponseForUser : AuctionBaseResponse
    {
        public ProductResponse Product { get; set; }
        public UserShortResponse User { get; set; }
        public UserShortResponse Winner { get; set; }
    }

    public class AuctionDetailResponseForSeller : AuctionBaseResponse
    {
        public ProductShortResponse Product { get; set; }
    }

    // ================= SHORT =================
    public class AuctionShortResponse : AuctionBaseResponse
    {
        public ProductShortResponse Product { get; set; }
    }

    public class EndedAuctionShortResponse : AuctionShortResponse
    {
        public UserShortResponse Winner { get; set; }
    }

    // ================= UPDATE =================
    public class AuctionShortResponseForUpdate
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }

        public decimal StepPrice { get; set; }
        public decimal StartPrice { get; set; }

        public List<AuctionTagResponse> AuctionTag { get; set; } = new();
    }

    // ================= TAG =================
    public class AuctionTagResponse
    {
        public Guid TagId { get; set; }
        public string TagName { get; set; } = string.Empty;
    }
}
