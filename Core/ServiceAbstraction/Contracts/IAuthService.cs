using Shared.Dtos.UserManagementDtos;

namespace Services.Abstraction.Contracts
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto dto, string? callerRole);
        Task<AuthResultDto> LoginAsync(LoginDto dto);
        Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(string userId);
        Task VerifyEmailAsync(string token);
        Task ForgotPasswordAsync(string email);
        Task ResetPasswordAsync(ResetPasswordDto dto);
    }
}
