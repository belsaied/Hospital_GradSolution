using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.DbContexts;

namespace Persistence.Data
{
    public class DataSeeding(HospitalDbContext _dbContext) : IDataSeeding
    {
        public void SeedData()
        {
            if(_dbContext.Database.GetPendingMigrations().Any())
            {
                _dbContext.Database.Migrate();
            }

        }
    }
}
