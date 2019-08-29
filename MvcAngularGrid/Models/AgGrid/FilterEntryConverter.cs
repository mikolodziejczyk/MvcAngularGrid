using MvcAngularGrid.Models.ExpressionList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MvcAngularGrid.Models.AgGrid
{
    public class FilterEntryConverter
    {
        public UniversalFilterEntry Convert(FilterEntry filterEntry)
        {
            UniversalFilterEntry r = new UniversalFilterEntry();

            r.FilterOperator = FilterOperatorParser.filterOperators[filterEntry.type];

            if (filterEntry.filterType == "text")
            {
                r.FirstValue = filterEntry.filter;
                r.SecondValue = null;
            }

            if (filterEntry.filterType == "date")
            {
                r.FirstValue = DateTime.ParseExact(filterEntry.dateFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                if (String.IsNullOrEmpty(filterEntry.dateTo) == false)
                {
                    r.SecondValue = DateTime.ParseExact(filterEntry.dateTo, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
            }

            return r;
        }

        public UniversalFilterEntry[] ConvertSequence(FilterEntry[] filterEntries)
        {
            return filterEntries.Select(x => Convert(x)).ToArray();
        }
    }
}