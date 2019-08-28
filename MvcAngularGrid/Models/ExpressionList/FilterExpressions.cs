﻿using System;
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
            Expression<Func<T, bool>> filterExpression = Expression.Lambda<Func<T, bool>>(callExpression, columnExpression.Parameters.First());
            return filterExpression;
        }

        public static Expression<Func<T, bool>> StartsWith(Expression<Func<T, object>> columnExpression, string value)
        {
            var callExpression = Expression.Call(columnExpression.Body, startsWithMethod, Expression.Constant(value));
            Expression<Func<T, bool>> filterExpression = Expression.Lambda<Func<T, bool>>(callExpression, columnExpression.Parameters.First());
            return filterExpression;
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
            }

            if (r == null) throw new InvalidOperationException((String.Format("Unsupported operator {0}.", filterOperator)));

            return r;
        }
    }
}