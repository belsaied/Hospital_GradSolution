using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Services.Abstraction.Contracts;
using Shared.Common;
using System.Net.Mail;

namespace Services.Implementations.UserManagementModule
{
    public class EmailService(IOptions<EmailOptions> _options) : IEmailService
    {
        public async Task SendVerificationEmailAsync(string toEmail, string token)
        {
            var link = $"{_options.Value.FrontendUrl}/verify-email?token={token}";
            await SendAsync(toEmail, "Verify your HMS account",
                $"<p>Click the link below to verify your email:</p>" +
                $"<p><a href='{link}'>Verify Email</a></p>" +
                $"<p>This link expires in 24 hours.</p>");
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string token)
        {
            var link = $"{_options.Value.FrontendUrl}/reset-password?token={token}";
            await SendAsync(toEmail, "Reset your HMS password",
                $"<p>Click the link below to reset your password:</p>" +
                $"<p><a href='{link}'>Reset Password</a></p>" +
                $"<p>This link expires in 1 hour.</p>");
        }

        private async Task SendAsync(string to, string subject, string htmlBody)
        {
            var opts = _options.Value;
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(opts.FromAddress));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(opts.SmtpHost, opts.SmtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(opts.Username, opts.Password);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }

}
