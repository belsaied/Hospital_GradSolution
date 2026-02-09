using Domain.Contracts;
using Domain.Models;
using Persistence.Data.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Implementations
{

    public class UnitOfWork : IUnitOfWork
    {
        private readonly HospitalDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories =[];
        public UnitOfWork(HospitalDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            var Entitytype = typeof(TEntity);
            if(_repositories.TryGetValue(Entitytype,out object? repo))
                return (IGenericRepository<TEntity,TKey>) repo;

            var NewRepo = new GenericRepository<TEntity, TKey>(_dbContext);
            _repositories.Add(Entitytype, NewRepo);
            return NewRepo;
        }

        public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
        
    }
}
