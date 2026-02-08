using bidify_be.Domain.Abstractions.Entities;
using bidify_be.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace bidify_be.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>, IDateTracking
    {
        public Gender? Gender { get; set; }
        public string? RefreshToken { get; set; }
        public int BidCount { get; set; } = 0;
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string? VerifyCode { get; set; }
        public DateTime? ExpireVerifyCode { get; set; }
        public DateTime? Dob { get; set; }
        public string? Avatar { get; set; }
        public string? PublicId { get; set; }
        public string? ReferralCode { get; set; }
        public bool Status { get; set; } = true;
        public decimal Balance { get; set; }
        public decimal RateStar { get; set; }
        public string? ReferredBy { get; set; }

        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
        public ICollection<BidsHistory> BidsHistories { get; set; } = new List<BidsHistory>();
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
