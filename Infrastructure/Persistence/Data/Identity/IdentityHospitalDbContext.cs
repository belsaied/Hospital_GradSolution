using Domain.Models.IdentityModule;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Data.Identity
{
    public class IdentityHospitalDbContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityHospitalDbContext(DbContextOptions<IdentityHospitalDbContext> options)
            : base(options) { }

        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
        }
    }
}
