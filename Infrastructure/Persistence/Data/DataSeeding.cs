using Domain.Contracts;
using Domain.Models.PatientModule;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.DbContexts;
using System.Text.Json;

namespace Persistence.Data
{
    public class DataSeeding(HospitalDbContext _dbContext) : IDataSeeding
    {
        public async Task SeedDataAsync()
        {
            try
            {
                // Check for pending migrations and apply them
                var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    await _dbContext.Database.MigrateAsync();
                }

                // 1. Seed Patients (no dependencies)
                if (!_dbContext.Patients.Any())
                {
                    var patientsData = File.OpenRead("..\\Infrastructure\\Persistence\\Data\\DataSeed\\patients.json");
                    var patients = await JsonSerializer.DeserializeAsync<List<Patient>>(patientsData);
                    if (patients is not null && patients.Any())
                    {
                        await _dbContext.Patients.AddRangeAsync(patients);
                        await _dbContext.SaveChangesAsync(); // Save to generate IDs
                    }
                }

                // 2. Seed Patient Allergies (depends on Patient)
                if (!_dbContext.PatientAllergies.Any())
                {
                    var allergiesData = File.OpenRead("..\\Infrastructure\\Persistence\\Data\\DataSeed\\allergies.json");
                    var allergies = await JsonSerializer.DeserializeAsync<List<PatientAllergy>>(allergiesData);
                    if (allergies is not null && allergies.Any())
                    {
                        await _dbContext.PatientAllergies.AddRangeAsync(allergies);
                    }
                }

                // 3. Seed Patient Medical Histories (depends on Patient)
                if (!_dbContext.PatientMedicalHistories.Any())
                {
                    var medicalHistoriesData = File.OpenRead("..\\Infrastructure\\Persistence\\Data\\DataSeed\\medical-histories.json");
                    var medicalHistories = await JsonSerializer.DeserializeAsync<List<PatientMedicalHistory>>(medicalHistoriesData);
                    if (medicalHistories is not null && medicalHistories.Any())
                    {
                        await _dbContext.PatientMedicalHistories.AddRangeAsync(medicalHistories);
                    }
                }

                // 4. Seed Emergency Contacts (depends on Patient)
                if (!_dbContext.EmergencyContacts.Any())
                {
                    var emergencyContactsData = File.OpenRead("..\\Infrastructure\\Persistence\\Data\\DataSeed\\emergency-contacts.json");
                    var emergencyContacts = await JsonSerializer.DeserializeAsync<List<EmergencyContact>>(emergencyContactsData);
                    if (emergencyContacts is not null && emergencyContacts.Any())
                    {
                        await _dbContext.EmergencyContacts.AddRangeAsync(emergencyContacts);
                    }
                }

                // Final save for all dependent entities
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (you can inject ILogger here)
                Console.WriteLine($"Error during data seeding: {ex.Message}");
                throw;
            }
        }
    }
}