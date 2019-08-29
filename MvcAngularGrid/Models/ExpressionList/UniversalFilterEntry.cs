using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAngularGrid.Models.ExpressionList
{
    public class UniversalFilterEntry
    {
        /// <summary>
        /// The first operator value, used by all operators.
        /// </summary>
        public object FirstValue { get; set; }
        /// <summary>
        /// The second operator value, used by selected operators.
        /// </summary>
        public object SecondValue { get; set; }

        public FilterOperator FilterOperator { get; set; }
    }
}