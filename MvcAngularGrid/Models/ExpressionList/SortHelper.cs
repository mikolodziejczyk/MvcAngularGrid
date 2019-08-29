using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MvcAngularGrid.Models.ExpressionList
{
    public class SortHelper
    {
        static public IOrderedQueryable<TSource> ApplyOrderByFromLambda<TSource>(IQueryable<TSource> query, LambdaExpression orderExpression, bool isAsc, bool isFirst)
        {
            IOrderedQueryable<TSource> r = null;

            if (orderExpression.Body.Type == typeof(DateTime))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, DateTime>>)orderExpression, isAsc, isFirst);
            }
            else
            if (orderExpression.Body.Type == typeof(DateTime?))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, DateTime?>>)orderExpression, isAsc, isFirst);
            }
            else
            if (orderExpression.Body.Type == typeof(String))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, String>>)orderExpression, isAsc, isFirst);
            }
            else
            if (orderExpression.Body.Type == typeof(decimal))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, decimal>>)orderExpression, isAsc, isFirst);
            }
            else
            if (orderExpression.Body.Type == typeof(decimal?))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, decimal?>>)orderExpression, isAsc, isFirst);
            }
            else
            if (orderExpression.Body.Type == typeof(double))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, double>>)orderExpression, isAsc, isFirst);
            }
            else
            if (orderExpression.Body.Type == typeof(double?))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, double?>>)orderExpression, isAsc, isFirst);
            }
            else
            if (orderExpression.Body.Type == typeof(int))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, int>>)orderExpression, isAsc, isFirst);
            }
            else
            if (orderExpression.Body.Type == typeof(int?))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, int?>>)orderExpression, isAsc, isFirst);
            }
            else
            if (orderExpression.Body.Type == typeof(long))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, long>>)orderExpression, isAsc, isFirst);
            }
            else
            if (orderExpression.Body.Type == typeof(long?))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, long?>>)orderExpression, isAsc, isFirst);
            }
            else
            if (orderExpression.Body.Type == typeof(short))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, short>>)orderExpression, isAsc, isFirst);
            }
            else
            if (orderExpression.Body.Type == typeof(short?))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, short?>>)orderExpression, isAsc, isFirst);
            }
            else
            if (orderExpression.Body.Type == typeof(byte))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, byte>>)orderExpression, isAsc, isFirst);
            }
            else
            if (orderExpression.Body.Type == typeof(byte?))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, byte?>>)orderExpression, isAsc, isFirst);
            }
            else
            if (orderExpression.Body.Type == typeof(bool))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, bool>>)orderExpression, isAsc, isFirst);
            }
            else
            if (orderExpression.Body.Type == typeof(bool?))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, bool?>>)orderExpression, isAsc, isFirst);
            }

            if (r == null)
            {
                throw new InvalidOperationException(String.Format("The LambdaExpression body type {0} is not supported here.", orderExpression.Body.Type));
            }

            return r;
        }

        static IOrderedQueryable<TSource> ApplyOrderBy<TSource, TKey>(IQueryable<TSource> query, Expression<Func<TSource, TKey>> orderExpression, bool isAsc, bool isFirst)
        {
            IOrderedQueryable<TSource> r;

            if (isFirst)
            {
                r = isAsc ? query.OrderBy(orderExpression) : query.OrderByDescending(orderExpression);
            }
            else
            {
                IOrderedQueryable<TSource> oq = (IOrderedQueryable<TSource>)query;
                r = isAsc ? oq.ThenBy(orderExpression) : oq.ThenByDescending(orderExpression);
            }

            return r;
        }
    }
}