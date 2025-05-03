using DomainLayer.Models;
using System.Linq.Expressions;

namespace DomainLayer.Contracts
{
    public interface ISpecification<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        Expression<Func<TEntity, bool>> Criteria { get; }
        List<Expression<Func<TEntity, object>>> IncludeExpressions { get; }
    }
}