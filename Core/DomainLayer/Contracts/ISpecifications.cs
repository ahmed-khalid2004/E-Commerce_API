using DomainLayer.Models;
using System.Linq.Expressions;

namespace DomainLayer.Contracts
{
    public interface ISpecifications<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        Expression<Func<TEntity, bool>>? Criteria { get; }

        List<Expression<Func<TEntity, object>>> IncludeExpressions { get; }
        List<string> IncludeStrings { get; }

        // Nullable — not every spec applies sorting
        Expression<Func<TEntity, object>>? OrderBy { get; }
        Expression<Func<TEntity, object>>? OrderByDescending { get; }

        int Skip { get; }
        int Take { get; }
        bool IsPaginated { get; set; }
    }
}