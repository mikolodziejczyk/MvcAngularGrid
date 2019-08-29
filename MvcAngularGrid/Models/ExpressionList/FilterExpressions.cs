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

        public static Expression<Func<T, bool>> Contains(LambdaExpression columnExpression, string value)
        {
            var callExpression = Expression.Call(columnExpression.Body, containsMethod, Expression.Constant(value));
            return MakeFilterExpression(callExpression, columnExpression);
        }

        public static Expression<Func<T, bool>> NotContains(LambdaExpression columnExpression, string value)
        {
            var callExpression = Expression.Call(columnExpression.Body, containsMethod, Expression.Constant(value));
            var innerExpression = Expression.Not(callExpression);
            return MakeFilterExpression(innerExpression, columnExpression);
        }

        public static Expression<Func<T, bool>> StartsWith(LambdaExpression columnExpression, string value)
        {
            var callExpression = Expression.Call(columnExpression.Body, startsWithMethod, Expression.Constant(value));
            return MakeFilterExpression(callExpression, columnExpression);
        }

        public static Expression<Func<T, bool>> Equal(LambdaExpression columnExpression, object value)
        {
            var innerExpression = Expression.Equal(columnExpression.Body, Expression.Constant(value, columnExpression.ReturnType));
            return MakeFilterExpression(innerExpression, columnExpression);
        }


        public static Expression<Func<T, bool>> NotEqual(LambdaExpression columnExpression, object value)
        {
            var innerExpression = Expression.Equal(columnExpression.Body, Expression.Constant(value, columnExpression.ReturnType));
            var notInnerExpression = Expression.Not(innerExpression);
            return MakeFilterExpression(notInnerExpression, columnExpression);
        }

        public static Expression<Func<T, bool>> GreaterThan(LambdaExpression columnExpression, object value)
        {
            var innerExpression = Expression.GreaterThan(columnExpression.Body, Expression.Constant(value, columnExpression.ReturnType));
            return MakeFilterExpression(innerExpression, columnExpression);
        }

        public static Expression<Func<T, bool>> GreaterThanOrEqual(LambdaExpression columnExpression, object value)
        {
            var innerExpression = Expression.GreaterThanOrEqual(columnExpression.Body, Expression.Constant(value, columnExpression.ReturnType));
            return MakeFilterExpression(innerExpression, columnExpression);
        }

        public static Expression<Func<T, bool>> LessThan(LambdaExpression columnExpression, object value)
        {
            var innerExpression = Expression.LessThan(columnExpression.Body, Expression.Constant(value, columnExpression.ReturnType));
            return MakeFilterExpression(innerExpression, columnExpression);
        }

        public static Expression<Func<T, bool>> LessThanOrEqual(LambdaExpression columnExpression, object value)
        {
            var innerExpression = Expression.LessThanOrEqual(columnExpression.Body, Expression.Constant(value, columnExpression.ReturnType));
            return MakeFilterExpression(innerExpression, columnExpression);
        }

        public static Expression<Func<T, bool>> InRange(LambdaExpression columnExpression, object valueA, object valueB)
        {
            var a = Expression.GreaterThanOrEqual(columnExpression.Body, Expression.Constant(valueA, columnExpression.ReturnType));
            var b = Expression.LessThanOrEqual(columnExpression.Body, Expression.Constant(valueB, columnExpression.ReturnType));
            var innerExpression = Expression.And(a, b);
            return MakeFilterExpression(innerExpression, columnExpression);
        }

        public static Expression<Func<T, bool>> GetFilterExpression(LambdaExpression columnExpression, UniversalFilterEntry universalFilterEntry)
        {
            Expression<Func<T, bool>> r = null;
            Type columnType = columnExpression.Body.Type;

            if (AllowedOperators.IsOperatorAllowedForType(universalFilterEntry.FilterOperator, columnType) == false)
            {
                throw new InvalidOperationException(String.Format("The operator {0} is invalid for the type {1}", universalFilterEntry.FilterOperator, columnExpression.Type.ToString()));
            }

            if (columnType == typeof(string))
            {
                string value = (string)universalFilterEntry.FirstValue;

                if (universalFilterEntry.FilterOperator == FilterOperator.Contains) r = Contains(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.StartsWith) r = StartsWith(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.Equals) r = Equal(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.NotContains) r = NotContains(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.NotEqual) r = NotEqual(columnExpression, value);
            }

            if (columnType == typeof(DateTime) || columnType == typeof(DateTime?))
            {
                object value = (object)universalFilterEntry.FirstValue;

                if (universalFilterEntry.FilterOperator == FilterOperator.Equals) r = Equal(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.NotEqual) r = NotEqual(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.GreaterThan) r = GreaterThan(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.LessThan) r = LessThan(columnExpression, value);

                if (universalFilterEntry.FilterOperator == FilterOperator.InRange)
                {
                    object secondValue = (object)universalFilterEntry.SecondValue;

                    r = InRange(columnExpression, value, secondValue);
                }

            }

            if (columnType == typeof(int) || columnType == typeof(int?) || columnType == typeof(decimal) || columnType == typeof(decimal?) || columnType == typeof(double) || columnType == typeof(double?))
            {
                object value = (object)universalFilterEntry.FirstValue;

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

                if (universalFilterEntry.FilterOperator == FilterOperator.Equals) r = Equal(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.NotEqual) r = NotEqual(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.GreaterThan) r = GreaterThan(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.GreaterThanOrEqual) r = GreaterThanOrEqual(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.LessThan) r = LessThan(columnExpression, value);
                if (universalFilterEntry.FilterOperator == FilterOperator.LessThanOrEqual) r = LessThanOrEqual(columnExpression, value);

                if (universalFilterEntry.FilterOperator == FilterOperator.InRange)
                {
                    object secondValue = (object)universalFilterEntry.SecondValue;
                    secondValue = Convert.ChangeType(secondValue, nonNullableColumnType);
                    r = InRange(columnExpression, value, secondValue);
                }

            }

            if (r == null) throw new InvalidOperationException((String.Format("Unsupported operator {0}.", universalFilterEntry.FilterOperator)));

            return r;
        }


        /// <summary>
        /// Simple helper, creates an Expression[Func[T, bool]] from a given inner expression and the column expression. This is to avoid repetitive code.
        /// </summary>
        /// <param name="innerExpression"></param>
        /// <param name="columnExpression"></param>
        /// <returns></returns>
        private static Expression<Func<T, bool>> MakeFilterExpression(Expression innerExpression, LambdaExpression columnExpression)
        {
            return Expression.Lambda<Func<T, bool>>(innerExpression, columnExpression.Parameters.First());
        }
    }
}