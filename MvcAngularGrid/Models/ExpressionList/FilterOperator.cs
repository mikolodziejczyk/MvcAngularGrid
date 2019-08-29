using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAngularGrid.Models.ExpressionList
{
    public enum FilterOperator
    {
        Contains,
        NotContains,
        Equals,
        NotEqual,
        StartsWith,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        InRange
    }
}