using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAngularGrid.Models.ExpressionList
{
    /// <summary>
    /// Normalized filtering operation.
    /// </summary>
    public class UniversalFilterEntry
    {
        /// <summary>
        /// The first operator value, used by all operators. This can be string, DateTime or double.
        /// </summary>
        public object FirstValue { get; set; }
        /// <summary>
        /// The second operator value, used by selected operators, like inRange. This can be string, DateTime or double.
        /// </summary>
        public object SecondValue { get; set; }

        public FilterOperator FilterOperator { get; set; }
    }
}