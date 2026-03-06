namespace Domain.Contracts
{
    public interface IAuditRepository
    {
        Task LogAsync(string userId, string action, string? details = null, string? ip = null);

    }
}
