using bidify_be.Domain.Enums;

namespace bidify_be.DTOs.Users
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int BidCount { get; set; }
        public string? PhoneNumber { get; set; }
        public Gender? Gender { get; set; }
        public string? Role { get; set; }
        public string? Avatar { get; set; }
        public string? ReferralCode { get; set; }
        public decimal Balance { get; set; }
        public decimal RateStar { get; set; }
        public bool Status { get; set; }
        public DateTime? Dob { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
