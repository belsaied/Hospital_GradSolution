using Domain.Contracts;
using Domain.Models.AppointmentModule;
using Domain.Models.DoctorModule;
using Domain.Models.PatientModule;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.DbContexts;
using System.Text.Json;
using System.Text.Json.Serialization;


public class DataSeeding(HospitalDbContext _dbContext) : IDataSeeding
{
    public async Task SeedDataAsync()
    {
        try
        {
            var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
                await _dbContext.Database.MigrateAsync();

            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Infrastructure", "Persistence", "Data", "DataSeed");

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            #region Patient Module
            if (!await _dbContext.Patients.AnyAsync())
            {
                var patientsPath = Path.Combine(basePath, "patients.json");
                var patients = await LoadJsonAsync<Patient>(patientsPath, jsonOptions);

                if (patients?.Any() == true)
                {
                    await _dbContext.Patients.AddRangeAsync(patients);
                    await _dbContext.SaveChangesAsync();
                }
            }

            //  Allergies
            if (!await _dbContext.PatientAllergies.AnyAsync())
            {
                var allergiesPath = Path.Combine(basePath, "allergies.json");
                var allergies = await LoadJsonAsync<PatientAllergy>(allergiesPath, jsonOptions);

                if (allergies?.Any() == true)
                    await _dbContext.PatientAllergies.AddRangeAsync(allergies);
            }

            //  Medical Histories
            if (!await _dbContext.PatientMedicalHistories.AnyAsync())
            {
                var historiesPath = Path.Combine(basePath, "medical-histories.json");
                var histories = await LoadJsonAsync<PatientMedicalHistory>(historiesPath, jsonOptions);

                if (histories?.Any() == true)
                    await _dbContext.PatientMedicalHistories.AddRangeAsync(histories);
            }

            //  Emergency Contacts
            if (!await _dbContext.EmergencyContacts.AnyAsync())
            {
                var contactsPath = Path.Combine(basePath, "emergency-contacts.json");
                var contacts = await LoadJsonAsync<EmergencyContact>(contactsPath, jsonOptions);

                if (contacts?.Any() == true)
                    await _dbContext.EmergencyContacts.AddRangeAsync(contacts);
            } 
            #endregion
            await _dbContext.SaveChangesAsync();

            #region Doctor Module
            if (!await _dbContext.Departments.AnyAsync())
            {
                var departmentsPath = Path.Combine(basePath, "departments.json");
                var departments = await LoadJsonAsync<Department>(departmentsPath, jsonOptions);
                if (departments?.Any() == true)
                {
                    await _dbContext.Departments.AddRangeAsync(departments);
                    await _dbContext.SaveChangesAsync();
                }
            }

            if (!await _dbContext.Doctors.AnyAsync())
            {
                var doctorPath = Path.Combine(basePath, "doctors.json");
                var doctor = await LoadJsonAsync<Doctor>(doctorPath, jsonOptions);
                if (doctor?.Any() == true)
                {
                    await _dbContext.Doctors.AddRangeAsync(doctor);
                    await _dbContext.SaveChangesAsync();
                }
            }
            if (!await _dbContext.DoctorQualifications.AnyAsync())
            {
                var qualsPath = Path.Combine(basePath, "doctor-qualifications.json");
                var qualifications = await LoadJsonAsync<DoctorQualification>(qualsPath, jsonOptions);
                if (qualifications?.Any() == true)
                    await _dbContext.DoctorQualifications.AddRangeAsync(qualifications);
            }

            if (!await _dbContext.DoctorSchedules.AnyAsync())
            {
                var schedulesPath = Path.Combine(basePath, "doctor-schedules.json");
                var schedules = await LoadJsonAsync<DoctorSchedule>(schedulesPath, jsonOptions);
                if (schedules?.Any() == true)
                    await _dbContext.DoctorSchedules.AddRangeAsync(schedules);
            }

            #endregion
            await _dbContext.SaveChangesAsync();

            #region Appointment Module
            if (!await _dbContext.Appointments.AnyAsync())
            {
                var aptPath = Path.Combine(basePath, "appointments.json");
                var appointments = await LoadJsonAsync<Appointment>(aptPath, jsonOptions);
                if (appointments?.Any() == true)
                    await _dbContext.Appointments.AddRangeAsync(appointments);
            }
            #endregion
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Seed Error: {ex.Message}");
            throw;
        }
    }

    private async Task<List<T>?> LoadJsonAsync<T>(string path, JsonSerializerOptions options)
    {
        if (!File.Exists(path))
            return null;

        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<List<T>>(stream, options);
    }
}
