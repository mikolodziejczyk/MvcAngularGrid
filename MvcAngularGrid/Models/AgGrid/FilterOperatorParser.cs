using MvcAngularGrid.Models.ExpressionList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAngularGrid.Models.AgGrid
{
    /// <summary>
    /// Replaces string-based ag-grid operator names with the universal FilterOperator enum.
    /// </summary>
    public static class FilterOperatorParser
    {
        public static IDictionary<string, FilterOperator> filterOperators = new Dictionary<string, FilterOperator>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "contains", FilterOperator.Contains },
            { "notContains", FilterOperator.NotContains },
            { "startsWith", FilterOperator.StartsWith },
            { "equals", FilterOperator.Equals },
            { "notEqual", FilterOperator.NotEqual },

            { "lessThan", FilterOperator.LessThan },
            { "lessThanOrEqual", FilterOperator.LessThanOrEqual },
            { "greaterThan", FilterOperator.GreaterThan },
            { "greaterThanOrEqual", FilterOperator.GreaterThanOrEqual },
            { "inRange", FilterOperator.InRange },
        };
    }
}