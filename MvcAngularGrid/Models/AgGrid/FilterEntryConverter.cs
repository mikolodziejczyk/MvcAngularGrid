using MvcAngularGrid.Models.ExpressionList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MvcAngularGrid.Models.AgGrid
{
    /// <summary>
    /// Converts an ag-grid  FilterEntry into UniversalFilterEntry
    /// </summary>
    public class FilterEntryConverter
    {
        /// <summary>
        /// Converts an ag-grid  FilterEntry into UniversalFilterEntry
        /// </summary>
        /// <param name="filterEntry">The ag-grid entry to convert</param>
        /// <returns>An UniversalFilterEntry instance or null if the data values in the FilterEntry instance couldn't be parsed.</returns>
        /// <remarks>Only non-parsable data cause null to be returned, in other cases an exception is thrown</remarks>
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
                else if (filterEntry.filterType == "date")
                {
                    r.FirstValue = DateTime.ParseExact(filterEntry.dateFrom, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    if (String.IsNullOrEmpty(filterEntry.dateTo) == false)
                    {
                        r.SecondValue = DateTime.ParseExact(filterEntry.dateTo, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    }
                }
                else if (filterEntry.filterType == "number")
                {
                    r.FirstValue = double.Parse(filterEntry.filter, CultureInfo.CurrentCulture);
                    if (String.IsNullOrEmpty(filterEntry.filterTo) == false)
                    {
                        r.SecondValue = double.Parse(filterEntry.filterTo, CultureInfo.CurrentCulture);
                    }
                }
                else
                {
                    throw new InvalidOperationException(String.Format("The filter type {0} is not supported.", filterEntry.filterType));
                }
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception)
            {
                r = null;
            }

            return r;
        }

        static public UniversalFilterEntry[] ConvertSequence(FilterEntry[] filterEntries)
        {
            return filterEntries.Where(x => x != null).Select(x => Convert(x)).Where(x => x != null).ToArray();
        }
    }
}