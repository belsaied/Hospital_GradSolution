using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Contracts
{
    public interface IGenericRepository<TEntity,TKey> where TEntity : BaseEntity<TKey>
    {
         Task<IEnumerable<TEntity>> GetAllAsync();
         Task<TEntity?> GetByIdAsync(TKey id);
         Task AddAsync(TEntity entity);
         void Update(TEntity entity);
         void Delete(TKey id);

    }
}
