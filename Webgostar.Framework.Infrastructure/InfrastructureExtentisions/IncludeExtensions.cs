using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Webgostar.Framework.Base.BaseModels;

namespace Webgostar.Framework.Infrastructure.InfrastructureExtentisions
{
    public static class IncludeExtensions
    {
        public static IIncludableQueryable<TEntity, TProperty> IncludeQueryable<TEntity, TProperty>(
            this IQueryable<TEntity> query,
            Expression<Func<TEntity, TProperty>> navigationExpression,
            bool withoutDeleted = false) where TEntity : BaseEntity where TProperty : BaseEntity
        {
            if (withoutDeleted)
            {
                var resultDeleted = query.Include(navigationExpression);

                return resultDeleted;
            }

            var result = query.Include(navigationExpression);

            return result;
        }

        public static IIncludableQueryable<TEntity, IEnumerable<TProperty>> IncludeQueryable<TEntity, TProperty>(
            this IQueryable<TEntity> query,
            Expression<Func<TEntity, IEnumerable<TProperty>>> navigationExpression,
            bool withoutDeleted = false) where TEntity : BaseEntity where TProperty : BaseEntity
        {
            if (withoutDeleted)
            {
                var resultDeleted = query.Include(navigationExpression);

                return resultDeleted;
            }

            Expression<Func<IEnumerable<TProperty>, IEnumerable<TProperty>>> filterTemplate = q => q.Where(e => !e.IsDelete);

            var filterBody = ReplacingExpressionVisitor
                .Replace(filterTemplate.Parameters[0], navigationExpression.Body, filterTemplate.Body);
            var filterLambda = Expression
                .Lambda<Func<TEntity, IEnumerable<TProperty>>>(filterBody, navigationExpression.Parameters);

            var result = query.Include(filterLambda);

            return result;
        }

        public static IIncludableQueryable<TEntity, TProperty> ThenIncludeQueryable<TEntity, TPreviousProperty, TProperty>(
            this IIncludableQueryable<TEntity, TPreviousProperty> query,
            Expression<Func<TPreviousProperty, TProperty>> navigationExpression,
            bool withoutDeleted = false) where TEntity : BaseEntity where TProperty : BaseEntity
        {
            if (withoutDeleted)
            {
                var resultDeleted = query.ThenInclude(navigationExpression);

                return resultDeleted;
            }

            var result = query.ThenInclude(navigationExpression);

            return result;
        }

        public static IIncludableQueryable<TEntity, TProperty> ThenIncludeQueryable<TEntity, TPreviousProperty, TProperty>(
            this IIncludableQueryable<TEntity, IEnumerable<TPreviousProperty>> query,
            Expression<Func<TPreviousProperty, TProperty>> navigationExpression,
            bool withoutDeleted = false) where TEntity : BaseEntity where TProperty : BaseEntity
        {
            if (withoutDeleted)
            {
                var resultDeleted = query.ThenInclude(navigationExpression);

                return resultDeleted;
            }

            var result = query.ThenInclude(navigationExpression);

            return result;
        }

        public static IIncludableQueryable<TEntity, IEnumerable<TProperty>> ThenIncludeQueryable<TEntity, TPreviousProperty, TProperty>(
            this IIncludableQueryable<TEntity, IEnumerable<TPreviousProperty>> query,
            Expression<Func<TPreviousProperty, IEnumerable<TProperty>>> navigationExpression,
            bool withoutDeleted = false) where TEntity : BaseEntity where TProperty : BaseEntity
        {
            if (withoutDeleted)
            {
                var resultDeleted = query.ThenInclude(navigationExpression);

                return resultDeleted;
            }

            Expression<Func<IEnumerable<TProperty>, IEnumerable<TProperty>>> filterTemplate = q => q.Where(e => !e.IsDelete);

            var filterBody = ReplacingExpressionVisitor
                .Replace(filterTemplate.Parameters[0], navigationExpression.Body, filterTemplate.Body);
            var filterLambda = Expression
                .Lambda<Func<TPreviousProperty, IEnumerable<TProperty>>>(filterBody, navigationExpression.Parameters);

            var result = query.ThenInclude(filterLambda);

            return result;
        }

        public static IIncludableQueryable<TEntity, IEnumerable<TProperty>> ThenIncludeQueryable<TEntity, TPreviousProperty, TProperty>(
            this IIncludableQueryable<TEntity, TPreviousProperty> query,
            Expression<Func<TPreviousProperty, IEnumerable<TProperty>>> navigationExpression,
            bool withoutDeleted = false) where TEntity : BaseEntity where TProperty : BaseEntity
        {
            if (withoutDeleted)
            {
                var resultDeleted = query.ThenInclude(navigationExpression);

                return resultDeleted;
            }

            Expression<Func<IEnumerable<TProperty>, IEnumerable<TProperty>>> filterTemplate = q => q.Where(e => !e.IsDelete);

            var filterBody = ReplacingExpressionVisitor
                .Replace(filterTemplate.Parameters[0], navigationExpression.Body, filterTemplate.Body);
            var filterLambda = Expression
                .Lambda<Func<TPreviousProperty, IEnumerable<TProperty>>>(filterBody, navigationExpression.Parameters);

            var result = query.ThenInclude(filterLambda);

            return result;
        }
    }
}
