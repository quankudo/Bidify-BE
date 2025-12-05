namespace bidify_be.Domain.Contracts
{
    public class ReferralRewardEmailModel
    {
        public string ReferrerEmail { get; set; }
        public string ReferrerName { get; set; }
        public string NewUserName { get; set; }
        public int RewardBids { get; set; } = 10;
    }

}
