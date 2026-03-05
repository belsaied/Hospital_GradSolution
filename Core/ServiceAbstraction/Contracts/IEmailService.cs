namespace Services.Abstraction.Contracts
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string toEmail, string token);
        Task SendPasswordResetEmailAsync(string toEmail, string token);
    }
}
