using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace MvcAngularGrid.Models.ExpressionList
{
    public class FilterExpressions<T>
    {
        static MethodInfo containsMethod = typeof(String).GetMethod("Contains");
        static MethodInfo startsWithMethod = typeof(String).GetMethod("StartsWith", new Type[] { typeof(string) });

        public static Expression<Func<T, bool>> Contains(Expression<Func<T, object>> columnExpression, string value)
        {
            var callExpression = Expression.Call(columnExpression.Body, containsMethod, Expression.Constant(value));
            return MakeFilterExpression(callExpression, columnExpression);
        }

        public static Expression<Func<T, bool>> NotContains(Expression<Func<T, object>> columnExpression, string value)
        {
            var callExpression = Expression.Call(columnExpression.Body, containsMethod, Expression.Constant(value));
            var innerExpression = Expression.Not(callExpression);
            return MakeFilterExpression(innerExpression, columnExpression);
        }

        public static Expression<Func<T, bool>> StartsWith(Expression<Func<T, object>> columnExpression, string value)
        {
            var callExpression = Expression.Call(columnExpression.Body, startsWithMethod, Expression.Constant(value));
            return MakeFilterExpression(callExpression, columnExpression);
        }

        public static Expression<Func<T, bool>> Equal(Expression<Func<T, object>> columnExpression, string value)
        {
            var innerExpression = Expression.Equal(columnExpression.Body, Expression.Constant(value));
            return MakeFilterExpression(innerExpression, columnExpression);
        }


        public static Expression<Func<T, bool>> NotEqual(Expression<Func<T, object>> columnExpression, string value)
        {
            var innerExpression = Expression.Equal(columnExpression.Body, Expression.Constant(value));
            var notInnerExpression = Expression.Not(innerExpression);
            return MakeFilterExpression(notInnerExpression, columnExpression);
        }

        public static Expression<Func<T, bool>> GetFilterExpression(Expression<Func<T, object>> columnExpression, string value, FilterOperator filterOperator)
        {
            Expression<Func<T, bool>> r = null;
            Type columnType = columnExpression.Body.Type;

            if (AllowedOperators.IsOperatorAllowedForType(filterOperator, columnType) == false)
            {
                throw new InvalidOperationException(String.Format("The operator {0} is invalid for the type {1}", filterOperator, columnExpression.Type.ToString()));
            }

            if (columnType == typeof(string))
            {
                if (filterOperator == FilterOperator.Contains) r = Contains(columnExpression, value);
                if (filterOperator == FilterOperator.StartsWith) r = StartsWith(columnExpression, value);
                if (filterOperator == FilterOperator.Equals) r = Equal(columnExpression, value);
                if (filterOperator == FilterOperator.NotContains) r = NotContains(columnExpression, value);
                if (filterOperator == FilterOperator.NotEquals) r = NotEqual(columnExpression, value);
            }

            if (r == null) throw new InvalidOperationException((String.Format("Unsupported operator {0}.", filterOperator)));

            return r;
        }


        /// <summary>
        /// Simple helper, creates an Expression[Func[T, bool]] from a given inner expression and the column expression. This is to avoid repetitive code.
        /// </summary>
        /// <param name="innerExpression"></param>
        /// <param name="columnExpression"></param>
        /// <returns></returns>
        private static Expression<Func<T, bool>> MakeFilterExpression(Expression innerExpression, Expression<Func<T, object>> columnExpression)
        {
            return Expression.Lambda<Func<T, bool>>(innerExpression, columnExpression.Parameters.First());
        }
    }
}