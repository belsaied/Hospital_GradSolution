using Domain.Contracts;
using Hospital_Grad.API.MiddleWares;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.DbContexts;
using Persistence.Implementations;
using Services;
using Services.Abstraction.Contracts;
using Services.Implementations;
using Services.Implementations.PatientModule;

var builder = WebApplication.CreateBuilder(args);
#region Services Configuration
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.UseInlineDefinitionsForEnums();
});
builder.Services.AddDbContext<HospitalDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(cfg => { }, typeof(ServiceAssemblyReference).Assembly);
#region Allow DI for Service Manager with Factory Delegate.
builder.Services.AddScoped<IServiceManager, ServiceManagerWithFactoryDelegate>();

// Register all services
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAllergyService, AllergyService>();
builder.Services.AddScoped<IMedicalHistoryService, MedicalHistoryService>();
builder.Services.AddScoped<IEmergencyContactService, EmergencyContactService>();

// Register factory delegates
builder.Services.AddScoped<Func<IPatientService>>(provider =>
    () => provider.GetRequiredService<IPatientService>()
);
builder.Services.AddScoped<Func<IAllergyService>>(provider =>
    () => provider.GetRequiredService<IAllergyService>()
);
builder.Services.AddScoped<Func<IMedicalHistoryService>>(provider =>
    () => provider.GetRequiredService<IMedicalHistoryService>()
);
builder.Services.AddScoped<Func<IEmergencyContactService>>(provider =>
    () => provider.GetRequiredService<IEmergencyContactService>()
);
#endregion
builder.Services.AddOpenApi(); 
#endregion
builder.Services.AddScoped<IDataSeeding, DataSeeding>();
var app = builder.Build();

using var scope = app.Services.CreateScope();
var ObjOfdataSeeding = scope.ServiceProvider.GetRequiredService<IDataSeeding>();
await ObjOfdataSeeding.SeedDataAsync();

app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
