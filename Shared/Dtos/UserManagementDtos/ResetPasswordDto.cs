using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.UserManagementDtos
{
    public record ResetPasswordDto
    {
        [Required]
        public string Token { get; init; } = string.Empty;
        [Required][MinLength(8)]
        public string NewPassword { get; init; } = string.Empty;

    }
}
