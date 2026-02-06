using bidify_be.Domain.Enums;

namespace bidify_be.DTOs.Users
{
    // ================= BASE =================
    public abstract class UserBaseResponse
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public decimal RateStar { get; set; }
    }

    // ================= SHORT =================
    public class UserShortResponse : UserBaseResponse
    {
        // dùng cho hiển thị nhanh (auction, product, comment...)
    }

    // ================= DETAIL =================
    public class UserResponse : UserBaseResponse
    {
        public string Email { get; set; } = string.Empty;
        public int BidCount { get; set; }

        public string? PhoneNumber { get; set; }
        public Gender? Gender { get; set; }
        public string? Role { get; set; }

        public string? ReferralCode { get; set; }

        public decimal Balance { get; set; }
        public bool Status { get; set; }
        public bool IsVerifyEmail { get; set; }

        public DateTime? Dob { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
