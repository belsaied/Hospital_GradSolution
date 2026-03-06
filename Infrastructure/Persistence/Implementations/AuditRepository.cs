using Domain.Contracts;
using Domain.Models.IdentityModule;
using Persistence.Data.Identity;

namespace Persistence.Implementations
{
    public class AuditRepository(IdentityHospitalDbContext _identityContext) : IAuditRepository
    {
        public async Task LogAsync(string userId, string action, string? details = null, string? ip = null)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                Details = details,
                IpAddress = ip,
            };
            await _identityContext.AuditLogs.AddAsync(log);
            await _identityContext.SaveChangesAsync();
        }
    }
}
