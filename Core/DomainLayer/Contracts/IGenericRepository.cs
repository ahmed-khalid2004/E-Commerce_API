using DomainLayer.Models;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        Task<TEntity?> GetByIdAsync(TKey id);
        Task<IEnumerable<TEntity>> GetAllAsync();

        #region Methods with Specifications
        Task<TEntity?> GetByIdAsync(ISpecification<TEntity, TKey> specifications);
        Task<IEnumerable<TEntity>> GetAllAsync(ISpecification<TEntity, TKey> specifications);
        #endregion
    }
}