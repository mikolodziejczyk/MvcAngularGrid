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
        static public UniversalFilterEntry Convert(FilterEntry filterEntry)
        {
            UniversalFilterEntry r = new UniversalFilterEntry();

            r.FilterOperator = FilterOperatorParser.filterOperators[filterEntry.type];

            try
            {

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

                if (filterEntry.filterType == "number")
                {
                    r.FirstValue = double.Parse(filterEntry.filter, CultureInfo.CurrentCulture);
                    if (String.IsNullOrEmpty(filterEntry.filterTo) == false)
                    {
                        r.SecondValue = double.Parse(filterEntry.filterTo, CultureInfo.CurrentCulture);
                    }
                }
            }
            catch (Exception e)
            {
                r = null;
            }

            return r;
        }

        static public UniversalFilterEntry[] ConvertSequence(FilterEntry[] filterEntries)
        {
            return filterEntries.Select(x => Convert(x)).Where(x=>x != null).ToArray();
        }
    }
}