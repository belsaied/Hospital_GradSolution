using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.DbContexts;
using Persistence.Implementations;
using Services;
using Services.Abstraction.Contracts;
using Services.Implementations;
using Services.Implementations.PatientModule;

var builder = WebApplication.CreateBuilder(args);
#region Services Configuration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<HospitalDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(cfg => { }, typeof(ServiceAssemblyReference).Assembly);
#region Allow DI for Service Manager with Factory Delegate.
builder.Services.AddScoped<IServiceManager, ServiceManagerWithFactoryDelegate>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<Func<IPatientService>>(provider =>
() => provider.GetRequiredService<IPatientService>()
); 
#endregion
builder.Services.AddOpenApi(); 
#endregion
builder.Services.AddScoped<IDataSeeding, DataSeeding>();
var app = builder.Build();

using var scope = app.Services.CreateScope();
var ObjOfdataSeeding = scope.ServiceProvider.GetRequiredService<IDataSeeding>();
await ObjOfdataSeeding.SeedDataAsync();

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
