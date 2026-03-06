using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.UserManagementDtos
{
    public record RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; init; } = string.Empty;


    }
}
