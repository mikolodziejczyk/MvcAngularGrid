using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAngularGrid.Models.AgGrid
{
    /// <summary>
    /// Represents a single filter entry for ag-grid.
    /// </summary>
    public class FilterEntry
    {
        /// <summary>
        /// What is the data type: text, date or number
        /// </summary>
        public string filterType { get; set; }
        public string type { get; set; }
        public string filter { get; set; }
        public string filterTo { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
    }
}