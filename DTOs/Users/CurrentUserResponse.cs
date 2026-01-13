namespace bidify_be.DTOs.Users
{
    public class CurrentUserResponse
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string AccessToken { get; set; }
        public string Role { get; set; }
        public string Avatar { get; set; }
        public string ReferralCode { get; set; }
        public int BidCount { get; set; } = 0;
        public decimal Balance { get; set; }
        public decimal RateStar { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
