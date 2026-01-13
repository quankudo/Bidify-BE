using bidify_be.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace bidify_be.Domain.Entities
{
    public class ApplicationUser : IdentityUser
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
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;
        [Column(TypeName = "decimal(3,2)")]
        public decimal RateStar { get; set; } = 5;
        public string? ReferredBy { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
        public ICollection<BidsHistory> BidsHistories { get; set; } = new List<BidsHistory>();
    }
}
