namespace bidify_be.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpEmail(string to, string otp);

        Task SendReferralRewardEmail(string referrerEmail, string referrerName, string newUserName, int rewardBids = 10);

        Task SendNewPasswordEmail(string to, string userName, string newPassword);
    }
}
