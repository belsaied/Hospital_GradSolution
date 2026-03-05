using Domain.Models.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.Abstraction.Contracts;
using Services.Exceptions;
using Shared.Common;
using Shared.Dtos.UserManagementDtos;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace Services.Implementations.UserManagementModule
{
    public class AuthService(
    UserManager<ApplicationUser> _userManager,
    RoleManager<IdentityRole> _roleManager,
    IOptions<JwtOptions> _jwtOptions,
    IAuditService _auditService,
    IEmailService _emailService) : IAuthService
    {
        private static readonly string[] ProtectedRoles = ["SuperAdmin", "HospitalAdmin"];

        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto, string? callerRole)
        {
            if (ProtectedRoles.Contains(dto.Role) && callerRole != "SuperAdmin")
                throw new UnauthorizedException("You cannot assign that role.");

            if (dto.Role == "Doctor")
            {
                if (!dto.DoctorId.HasValue)
                    throw new ValidationException(["DoctorId is required for the Doctor role."]);
                if (_userManager.Users.Any(u => u.DoctorId == dto.DoctorId))
                    throw new ValidationException(["This DoctorId is already linked to another account."]);
            }

            if (dto.Role == "Patient")
            {
                if (!dto.PatientId.HasValue)
                    throw new ValidationException(["PatientId is required for the Patient role."]);
                if (_userManager.Users.Any(u => u.PatientId == dto.PatientId))
                    throw new ValidationException(["This PatientId is already linked to another account."]);
            }

            var user = new ApplicationUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.Email,
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new ValidationException(result.Errors.Select(e => e.Description));

            if (!await _roleManager.RoleExistsAsync(dto.Role))
                await _roleManager.CreateAsync(new IdentityRole(dto.Role));

            await _userManager.AddToRoleAsync(user, dto.Role);

            var rawToken = Guid.NewGuid().ToString();
            user.EmailVerificationTokenHash = HashToken(rawToken);
            user.EmailVerificationExpiry = DateTime.UtcNow.AddHours(24);
            await _userManager.UpdateAsync(user);

            await _emailService.SendVerificationEmailAsync(user.Email!, rawToken);

            // Return user info only — no tokens until email is verified
            return BuildAuthResult(user, [], null, null);
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email)
                       ?? throw new UnauthorizedException("Invalid email or password.");

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
                throw new AccountLockedException(user.LockoutEnd.Value);

            var passwordOk = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordOk)
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= 5)
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                await _userManager.UpdateAsync(user);
                await _auditService.LogAsync(user.Id, "LOGIN_FAILED");
                throw new UnauthorizedException("Invalid email or password.");
            }

            if (!user.IsEmailVerified)
                throw new EmailNotVerifiedException();

            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            user.LastLoginAt = DateTime.UtcNow;

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = GenerateJwt(user, roles);
            var (rawRefresh, hashRefresh) = GenerateRefreshToken();
            user.RefreshTokenHash = hashRefresh;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtOptions.Value.RefreshTokenDays);

            await _userManager.UpdateAsync(user);
            await _auditService.LogAsync(user.Id, "LOGIN_SUCCESS");

            return BuildAuthResult(user, roles, accessToken, rawRefresh);
        }

        public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken)
        {
            var hash = HashToken(refreshToken);
            var user = _userManager.Users.FirstOrDefault(u => u.RefreshTokenHash == hash)
                       ?? throw new UnauthorizedException("Invalid refresh token.");

            if (user.RefreshTokenExpiry < DateTime.UtcNow)
                throw new UnauthorizedException("Refresh token has expired.");

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
                throw new AccountLockedException(user.LockoutEnd.Value);

            var (rawRefresh, hashRefresh) = GenerateRefreshToken();
            user.RefreshTokenHash = hashRefresh;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtOptions.Value.RefreshTokenDays);
            await _userManager.UpdateAsync(user);

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = GenerateJwt(user, roles);

            return BuildAuthResult(user, roles, accessToken, rawRefresh);
        }

        public async Task LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return;

            user.RefreshTokenHash = null;
            user.RefreshTokenExpiry = null;
            await _userManager.UpdateAsync(user);
            await _auditService.LogAsync(userId, "LOGOUT");
        }

        public async Task VerifyEmailAsync(string token)
        {
            var hash = HashToken(token);
            var user = _userManager.Users
                           .FirstOrDefault(u => u.EmailVerificationTokenHash == hash)
                       ?? throw new UnauthorizedException("Invalid or expired verification token.");

            if (user.EmailVerificationExpiry < DateTime.UtcNow)
                throw new UnauthorizedException("Verification token has expired.");

            user.IsEmailVerified = true;
            user.EmailVerificationTokenHash = null;
            user.EmailVerificationExpiry = null;
            await _userManager.UpdateAsync(user);
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) return; // Always return — no user enumeration

            var rawToken = Guid.NewGuid().ToString();
            user.PasswordResetTokenHash = HashToken(rawToken);
            user.PasswordResetExpiry = DateTime.UtcNow.AddHours(1);
            await _userManager.UpdateAsync(user);

            await _emailService.SendPasswordResetEmailAsync(email, rawToken);
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var hash = HashToken(dto.Token);
            var user = _userManager.Users
                           .FirstOrDefault(u => u.PasswordResetTokenHash == hash)
                       ?? throw new UnauthorizedException("Invalid or expired reset token.");

            if (user.PasswordResetExpiry < DateTime.UtcNow)
                throw new UnauthorizedException("Reset token has expired.");

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, dto.NewPassword);
            if (!result.Succeeded)
                throw new ValidationException(result.Errors.Select(e => e.Description));

            user.PasswordResetTokenHash = null;
            user.PasswordResetExpiry = null;
            await _userManager.UpdateAsync(user);
        }

        // ------------------------------------------------------------------ Helpers
        private string GenerateJwt(ApplicationUser user, IList<string> roles)
        {
            var opts = _jwtOptions.Value;
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email,          user.Email!),
                new(ClaimTypes.Name,           user.FullName),
            };
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            if (user.DoctorId.HasValue)
                claims.Add(new Claim("doctor_id", user.DoctorId.Value.ToString()));
            if (user.PatientId.HasValue)
                claims.Add(new Claim("patient_id", user.PatientId.Value.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opts.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: opts.Issuer,
                audience: opts.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(opts.AccessTokenMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static (string raw, string hash) GenerateRefreshToken()
        {
            var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            return (raw, HashToken(raw));
        }

        private static string HashToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(bytes);
        }

        private AuthResultDto BuildAuthResult(
            ApplicationUser user, IList<string> roles,
            string? accessToken, string? refreshToken)
        {
            return new AuthResultDto
            {
                AccessToken = accessToken ?? string.Empty,
                RefreshToken = refreshToken ?? string.Empty,
                ExpiresIn = _jwtOptions.Value.AccessTokenMinutes * 60,
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName,
                    Roles = [.. roles],
                }
            };
        }
    }
}
