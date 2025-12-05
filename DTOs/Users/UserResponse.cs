namespace bidify_be.DTOs.Users
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int BidCount { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? AccessToken { get; set; }
        public string? Role { get; set; }
        public string? Avatar { get; set; }
        public string? ReferralCode { get; set; }
        public decimal Balance { get; set; }
        public decimal RateStar { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string? RefreshToken { get; set; }
    }
}
