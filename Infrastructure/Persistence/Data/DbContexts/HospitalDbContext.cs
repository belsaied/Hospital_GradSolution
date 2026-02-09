using Domain.Models.Enums;
using Domain.Models.PatientModule;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Data.DbContexts
{
    public class HospitalDbContext:DbContext
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options):base(options)
        {
            
        }
        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(HospitalDbContext).Assembly);
            // Global query filter
            modelBuilder.Entity<Patient>()
                .HasQueryFilter(p => p.Status != PatientStatus.Deceased);
        }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientMedicalHistory> PatientMedicalHistories { get; set; }
        public DbSet<PatientAllergy> PatientAllergies { get; set; }
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }
    }
}
