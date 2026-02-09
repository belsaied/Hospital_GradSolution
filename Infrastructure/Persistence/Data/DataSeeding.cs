using Domain.Contracts;
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

            //  Patients
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
