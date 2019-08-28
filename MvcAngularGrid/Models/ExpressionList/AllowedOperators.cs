using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAngularGrid.Models.ExpressionList
{
    public static class AllowedOperators
    {
        public static FilterOperator[] OperatorsAllowedForString = new FilterOperator[] { FilterOperator.Contains, FilterOperator.Equals, FilterOperator.NotContains, FilterOperator.NotEquals, FilterOperator.StartsWith };

        public static bool IsOperatorAllowedForString(FilterOperator filterOperator)
        {
            return OperatorsAllowedForString.Contains(filterOperator);
        }

        public static bool IsOperatorAllowedForType(FilterOperator filterOperator, Type type)
        {
            bool r = false;

            if (type == typeof(String))
            {
                r = IsOperatorAllowedForString(filterOperator);
            }

            return r;
        }
    }
}