using Domain.Models.IdentityModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.UserManagementDtos;
using System.Security.Claims;

namespace Presentation.Controllers
{

    public class AuthController(
        IAuthService _authService,
        UserManager<ApplicationUser> _userManager) : ApiController
    {
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var callerRole = User.FindFirstValue(ClaimTypes.Role);
            return StatusCode(201, await _authService.RegisterAsync(dto, callerRole));
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
            => Ok(await _authService.LoginAsync(dto));
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenDto dto)
            => Ok(await _authService.RefreshTokenAsync(dto.RefreshToken));

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _authService.LogoutAsync(userId);
            return NoContent();
        }
        [AllowAnonymous]
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            await _authService.VerifyEmailAsync(token);
            return Ok("Email verified successfully.");
        }
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordDto dto)
        {
            await _authService.ForgotPasswordAsync(dto.Email);
            return Ok(); // Always 200 — prevents user enumeration
        }
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            await _authService.ResetPasswordAsync(dto);
            return Ok();
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpDelete("lockout/{userId}")]
        public async Task<IActionResult> UnlockAccount(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();
            user.LockoutEnd = null;
            user.FailedLoginAttempts = 0;
            await _userManager.UpdateAsync(user);
            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userInfo = new
            {
                Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Email = User.FindFirstValue(ClaimTypes.Email),
                FullName = User.FindFirstValue(ClaimTypes.Name),
                Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList(),
                DoctorId = User.FindFirstValue("doctor_id"),
                PatientId = User.FindFirstValue("patient_id")
            };
            return Ok(userInfo);
        }
    }
}
