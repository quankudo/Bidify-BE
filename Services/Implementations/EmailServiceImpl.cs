using bidify_be.Domain.Contracts;
using bidify_be.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace bidify_be.Services.Implementations
{
    public class EmailServiceImpl : IEmailService
    {
        private readonly MailSettings _settings;
        private readonly RazorTemplateService _razor;

        public EmailServiceImpl(IOptions<MailSettings> settings, RazorTemplateService razor)
        {
            _settings = settings.Value;
            _razor = razor;
        }

        public async Task SendOtpEmail(string to, string otp)
        {
            var model = new OtpModel
            {
                Code = otp,
                ExpireMinutes = 3
            };

            string html = await _razor.RenderAsync("OtpCode.cshtml", model);

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_settings.UserName));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = "Mã xác thực OTP";

            message.Body = new TextPart("html") { Text = html };

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.UserName, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendReferralRewardEmail(string referrerEmail, string referrerName, string newUserName, int rewardBids = 10)
        {
            var model = new ReferralRewardEmailModel
            {
                ReferrerName = referrerName,
                NewUserName = newUserName,
                RewardBids = rewardBids
            };

            string html = await _razor.RenderAsync("ReferralReward.cshtml", model);

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_settings.UserName));
            message.To.Add(MailboxAddress.Parse(referrerEmail));
            message.Subject = $"Bạn vừa nhận {rewardBids} Bids từ giới thiệu!";

            message.Body = new TextPart("html") { Text = html };

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.UserName, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendNewPasswordEmail(string to, string userName, string newPassword)
        {
            var model = new NewPasswordEmailModel
            {
                UserName = userName,
                NewPassword = newPassword
            };

            string html = await _razor.RenderAsync("NewPassword.cshtml", model);

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_settings.UserName));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = "Mật khẩu mới của bạn";

            message.Body = new TextPart("html") { Text = html };

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.UserName, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

    }
}
