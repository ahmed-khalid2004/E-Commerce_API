using DomainLayer.Contracts;
using DomainLayer.Models;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class UnitOfWork(StoreDbContext dbContext) : IUnitOfWork
    {
        private readonly StoreDbContext _dbContext = dbContext;
        private readonly Dictionary<string, object> _repositories = [];

        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            var typeName = typeof(TEntity).Name;

            if (_repositories.TryGetValue(typeName, out object? value))
            {
                return (IGenericRepository<TEntity, TKey>)value;
            }
            else
            {
                var Repo = new GenericRepository<TEntity, TKey>(_dbContext);
                _repositories["typeName"] = Repo;
                return Repo;
            }
        }
        public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
    }
}
