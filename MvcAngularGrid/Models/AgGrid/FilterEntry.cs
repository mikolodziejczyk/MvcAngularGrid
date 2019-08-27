using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAngularGrid.Models.AgGrid
{
    public class FilterEntry
    {
        public string filterType { get; set; }
        public string type { get; set; }
        public string filter { get; set; }
        public string filterTo { get; set; }
    }
}