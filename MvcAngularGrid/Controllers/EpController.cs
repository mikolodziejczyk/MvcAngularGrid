using EnergyPoint.Repository;
using MvcAngularGrid.Models.AgGrid;
using MvcAngularGrid.Models.ExpressionList;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MvcAngularGrid.Controllers
{
    public class EpController : Controller
    {
        public ActionResult Index()
        {

            string data;

            using (EnergyPointEntities context = new EnergyPointEntities())
            {
                var r = context.Connection.Select(x => new { ppe = x.PPE, meterCode = x.MeterCode, name = x.Name, tariff = x.Tariff.Name, company = x.Company.Acronym }).ToArray();

                data = JsonConvert.SerializeObject(r);

            }


            return Content(data, "application/json");
        }

        IDictionary<string, Expression<Func<Connection, object>>> columnSource = new Dictionary<string, Expression<Func<Connection, object>>>()
        {
            { "name",  x => x.Name},
            { "ppe",  x => x.PPE},
            { "company",  x => x.Company.Acronym},
            { "meterCode",  x => x.MeterCode},
            { "tariff",  x => x.Tariff.Name},
        };

        public ActionResult Page(int startRow, int endRow, SortEntry[] sortModel, Dictionary<string, FilterEntry> filterModel)
        {
            using (EnergyPointEntities context = new EnergyPointEntities())
            {
                IQueryable<Connection> query = context.Connection;


                // if filters aren't posted, in filters we get values from MVC; with "action", "controller" as keys; they must be excluded, currently by their null value criterion
                foreach (var kvp in filterModel.Where(x => x.Value != null))
                {
                    string field = kvp.Key;
                    string value = kvp.Value.filter;

                    Expression<Func<Connection, object>> columnExpression = columnSource[field];

                    FilterOperator filterOperator = FilterOperatorParser.filterOperators[kvp.Value.@type];

                    Expression<Func<Connection, bool>> filterExpression = FilterExpressions<Connection>.GetFilterExpression(columnExpression, value, filterOperator);

                    query = query.Where(filterExpression);
                }

                int count = query.Count();

                if (sortModel != null)
                {
                    for (int i = 0; i < sortModel.Length; i++)
                    {
                        SortEntry sortEntry = sortModel[i];

                        string field = sortEntry.colId;
                        bool isAsc = sortEntry.sort == SortEntry.asc;

                        Expression<Func<Connection, object>> orderExpression = columnSource[field];

                        //switch (field)
                        //{
                        //    case "name": orderExpression = x => x.Name; break;
                        //    case "ppe": orderExpression = x => x.PPE; break;
                        //    case "meterCode": orderExpression = x => x.MeterCode; break;
                        //    case "company": orderExpression = x => x.Company.Acronym; break;
                        //    case "tariff": orderExpression = x => x.Tariff.Name; break;
                        //    default: throw new InvalidOperationException("The column is not enabled for sorting.");
                        //}

                        if (i == 0)
                        {
                            query = isAsc ? query.OrderBy(orderExpression) : query.OrderByDescending(orderExpression);
                        }
                        else
                        {
                            IOrderedQueryable<Connection> oq = (IOrderedQueryable<Connection>)query;
                            query = isAsc ? oq.ThenBy(orderExpression) : oq.ThenByDescending(orderExpression);

                        }
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.Name);
                }

                var r = query.Skip(startRow).Take(endRow - startRow).Select(x =>
                    new
                    {
                        id = x.Id,
                        ppe = x.PPE,
                        meterCode = x.MeterCode,
                        name = x.Name,
                        tariff = x.Tariff.Name,
                        company = x.Company.Acronym,
                        startDate = x.StartDate,
                        endDate = x.EndDate
                    }).ToArray();



                var response = new { rows = r, count = count };

                return Json(response, JsonRequestBehavior.AllowGet);
            }
        }


    }
}