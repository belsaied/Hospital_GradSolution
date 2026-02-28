using Domain.Models.AppointmentModule;
using Domain.Models.DoctorModule;
using Domain.Models.MedicalRecordModule;
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

        }
        #region Patient Module
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientMedicalHistory> PatientMedicalHistories { get; set; }
        public DbSet<PatientAllergy> PatientAllergies { get; set; }
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }
        #endregion

        #region Doctor Module
        public DbSet<Department> Departments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorQualification> DoctorQualifications { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }

        #endregion

        #region AppointmentModule
        public DbSet<Appointment> Appointments { get; set; }
        #endregion

        #region Medical Record Medule
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<VitalSign> VitalSigns { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<LabOrder> LabOrders { get; set; }
        public DbSet<LabResult> LabResults { get; set; }    
        #endregion
    }
}
