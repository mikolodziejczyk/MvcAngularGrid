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
        // all entity column types for which numeric filter should be used
        static HashSet<Type> numericTypes = new HashSet<Type>() { typeof(int), typeof(int?), typeof(long), typeof (long?), typeof(short), typeof(short?), typeof(byte), typeof(byte?),
            typeof(decimal), typeof(decimal?), typeof(double), typeof(double?) };

        #region expressions

        internal static Expression Contains(LambdaExpression columnExpression, string value)
        {
            return Expression.Call(columnExpression.Body, containsMethod, Expression.Constant(value));
        }

        internal static Expression NotContains(LambdaExpression columnExpression, string value)
        {
            var callExpression = Expression.Call(columnExpression.Body, containsMethod, Expression.Constant(value));
            return Expression.Not(callExpression);
        }

        internal static Expression StartsWith(LambdaExpression columnExpression, string value)
        {
            return Expression.Call(columnExpression.Body, startsWithMethod, Expression.Constant(value));
        }

        internal static Expression Equal(LambdaExpression columnExpression, object value)
        {
            return Expression.Equal(columnExpression.Body, Expression.Constant(value, columnExpression.ReturnType));
        }


        internal static Expression NotEqual(LambdaExpression columnExpression, object value)
        {
            var innerExpression = Expression.Equal(columnExpression.Body, Expression.Constant(value, columnExpression.ReturnType));
            return Expression.Not(innerExpression);
        }

        internal static Expression GreaterThan(LambdaExpression columnExpression, object value)
        {
            return Expression.GreaterThan(columnExpression.Body, Expression.Constant(value, columnExpression.ReturnType));
        }

        internal static Expression GreaterThanOrEqual(LambdaExpression columnExpression, object value)
        {
            return Expression.GreaterThanOrEqual(columnExpression.Body, Expression.Constant(value, columnExpression.ReturnType));
        }

        internal static Expression LessThan(LambdaExpression columnExpression, object value)
        {
            return Expression.LessThan(columnExpression.Body, Expression.Constant(value, columnExpression.ReturnType));
        }

        internal static Expression LessThanOrEqual(LambdaExpression columnExpression, object value)
        {
            return Expression.LessThanOrEqual(columnExpression.Body, Expression.Constant(value, columnExpression.ReturnType));
        }

        internal static Expression InRange(LambdaExpression columnExpression, object valueA, object valueB)
        {
            var a = Expression.GreaterThanOrEqual(columnExpression.Body, Expression.Constant(valueA, columnExpression.ReturnType));
            var b = Expression.LessThanOrEqual(columnExpression.Body, Expression.Constant(valueB, columnExpression.ReturnType));
            return Expression.And(a, b);
        }

        #endregion

        /// <summary>
        /// Returns filtering expression to be used in Where for the specified columnExpression and universalFilterEntry.
        /// </summary>
        /// <param name="columnExpression"></param>
        /// <param name="universalFilterEntry"></param>
        /// <returns>A lambda expression representing the universalFilterEntry applied to columnExpression.Body</returns>
        public static Expression<Func<T, bool>> GetFilterExpression(LambdaExpression columnExpression, UniversalFilterEntry universalFilterEntry)
        {
            Expression innerExpression = null;
            Type columnType = columnExpression.Body.Type;

            if (AllowedOperators.IsOperatorAllowedForType(universalFilterEntry.FilterOperator, columnType) == false)
            {
                throw new InvalidOperationException(String.Format("The operator {0} is invalid for the type {1}", universalFilterEntry.FilterOperator, columnExpression.Type.ToString()));
            }

            if (columnType == typeof(string))
            {
                string value = (string)universalFilterEntry.FirstValue;

                if (universalFilterEntry.FilterOperator == FilterOperator.Contains) innerExpression = Contains(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.StartsWith) innerExpression = StartsWith(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.Equals) innerExpression = Equal(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.NotContains) innerExpression = NotContains(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.NotEqual) innerExpression = NotEqual(columnExpression, value);
            }
            else if (columnType == typeof(DateTime) || columnType == typeof(DateTime?))
            {
                object value = (object)universalFilterEntry.FirstValue;

                if (universalFilterEntry.FilterOperator == FilterOperator.Equals) innerExpression = Equal(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.NotEqual) innerExpression = NotEqual(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.GreaterThan) innerExpression = GreaterThan(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.LessThan) innerExpression = LessThan(columnExpression, value);

                if (universalFilterEntry.FilterOperator == FilterOperator.InRange)
                {
                    object secondValue = (object)universalFilterEntry.SecondValue;

                    innerExpression = InRange(columnExpression, value, secondValue);
                }

            }
            else if (numericTypes.Contains(columnType))
            {
                object value = universalFilterEntry.FirstValue;

                // Different number types can be converted directly to another
                // However, we cannot convert them directly to a another nullable, e.g. we cannot convert double => Nullable<decimal> with Convert.ChangeType()
                // Therefore, if the column is nullable, we need to find the base type (e.g. Nullable<decimal> => decimal), then convert to this type with Convert.ChangeType()
                // Converting to Nullable<decimal> is not required at this point, it will be handled in the method producing expression.

                Type nonNullableColumnType = columnType;

                if (columnType.IsValueType && columnType.IsGenericType && columnType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    nonNullableColumnType = columnType.GenericTypeArguments[0];
                }

                value = Convert.ChangeType(value, nonNullableColumnType);

                if (universalFilterEntry.FilterOperator == FilterOperator.Equals) innerExpression = Equal(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.NotEqual) innerExpression = NotEqual(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.GreaterThan) innerExpression = GreaterThan(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.GreaterThanOrEqual) innerExpression = GreaterThanOrEqual(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.LessThan) innerExpression = LessThan(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.LessThanOrEqual) innerExpression = LessThanOrEqual(columnExpression, value);

                if (universalFilterEntry.FilterOperator == FilterOperator.InRange)
                {
                    object secondValue = (object)universalFilterEntry.SecondValue;
                    secondValue = Convert.ChangeType(secondValue, nonNullableColumnType);
                    innerExpression = InRange(columnExpression, value, secondValue);
                }

            }

            if (innerExpression == null) throw new InvalidOperationException((String.Format("Unsupported operator {0}.", universalFilterEntry.FilterOperator)));

            Expression<Func<T, bool>> r = Expression.Lambda<Func<T, bool>>(innerExpression, columnExpression.Parameters.First());

            return r;
        }
    }
}