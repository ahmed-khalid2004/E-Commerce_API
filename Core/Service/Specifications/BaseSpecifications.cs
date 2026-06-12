using DomainLayer.Contracts;
using DomainLayer.Models;
using System.Linq.Expressions;

namespace Service.Specifications
{
    abstract class BaseSpecifications<TEntity, TKey> : ISpecifications<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        protected BaseSpecifications(Expression<Func<TEntity, bool>>? criteriaExpression)
        {
            Criteria = criteriaExpression;
        }

        // ── Filter ────────────────────────────────────────────────────────────
        public Expression<Func<TEntity, bool>>? Criteria { get; private set; }

        // ── Expression-based includes (single-level) ──────────────────────────
        public List<Expression<Func<TEntity, object>>> IncludeExpressions { get; } = [];

        protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
            => IncludeExpressions.Add(includeExpression);

        // ── String-based includes (dot-notation ThenInclude) ──────────────────
        public List<string> IncludeStrings { get; } = [];

        protected void AddInclude(string includeString)
            => IncludeStrings.Add(includeString);

        // ── Sorting — nullable; evaluator checks for null before applying ──────
        public Expression<Func<TEntity, object>>? OrderBy { get; private set; }
        public Expression<Func<TEntity, object>>? OrderByDescending { get; private set; }

        protected void AddOrderBy(Expression<Func<TEntity, object>> orderByExp)
            => OrderBy = orderByExp;

        protected void AddOrderByDescending(Expression<Func<TEntity, object>> orderByDescExp)
            => OrderByDescending = orderByDescExp;

        // ── Pagination ────────────────────────────────────────────────────────
        public int Skip { get; private set; }
        public int Take { get; private set; }
        public bool IsPaginated { get; set; }

        protected void ApplyPagination(int pageSize, int pageIndex)
        {
            IsPaginated = true;
            Take = pageSize;
            Skip = (pageIndex - 1) * pageSize;
        }
    }
}