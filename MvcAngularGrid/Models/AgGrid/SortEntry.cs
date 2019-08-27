using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAngularGrid.Models.AgGrid
{
    public class SortEntry
    {
        public const string asc = "asc";
        public const string desc = "desc";

        public string colId { get; set; }
        public string sort { get; set; }
    }
}