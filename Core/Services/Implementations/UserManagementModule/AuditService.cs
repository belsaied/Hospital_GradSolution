using Domain.Models.IdentityModule;
using Services.Abstraction.Contracts;

namespace Services.Implementations.UserManagementModule
{
    public class AuditService(IdentityHospitalDbContext _identityContext) : IAuditService
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
