namespace Shared.Dtos.UserManagementDtos
{
    public record ForgetPasswordDto
    {
        public string Email { get; init; } = string.Empty;

    }
}
