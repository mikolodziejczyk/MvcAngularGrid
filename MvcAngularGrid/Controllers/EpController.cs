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

        IDictionary<string, LambdaExpression> columnSource = new Dictionary<string, LambdaExpression>()
        {
            { "name", (Expression<Func<Connection, string>>)(x => x.Name)},
            { "ppe",  (Expression<Func<Connection, string>>)(x => x.PPE)},
            { "company",  (Expression<Func<Connection, string>>)(x => x.Company.Acronym)},
            { "meterCode",  (Expression<Func<Connection, string>>)(x => x.MeterCode)},
            { "tariff",  (Expression<Func<Connection, string>>)(x => x.Tariff.Name)},
            { "startDate", (Expression<Func<Connection, DateTime>>)(x => x.StartDate)},
            { "endDate",  (Expression<Func<Connection, DateTime>>)(x => x.EndDate ?? new DateTime(1990,01,01))},
            { "orderedCapacity",  (Expression<Func<Connection, decimal>>)(x => x.OrderedCapacity ?? -1)},
            { "endDateNullable",  (Expression<Func<Connection, DateTime?>>)(x => x.EndDate)},
            { "orderedCapacityNullable",  (Expression<Func<Connection, decimal?>>)(x => x.OrderedCapacity)}
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

                    LambdaExpression columnExpression = columnSource[field];

                    // FilterOperator filterOperator = FilterOperatorParser.filterOperators[kvp.Value.@type];
                    FilterEntryConverter filterEntryConverter = new FilterEntryConverter();
                    UniversalFilterEntry universalFilterEntry = filterEntryConverter.Convert(kvp.Value);

                    Expression<Func<Connection, bool>> filterExpression = FilterExpressions<Connection>.GetFilterExpression(columnExpression, universalFilterEntry);

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
                        bool isFirst = i == 0;

                        LambdaExpression lambdaExpression = columnSource[field];

                        query = ApplyOrderByFromLambda(query, lambdaExpression, isAsc, isFirst);

                        // LambdaExpression orderExpression = columnSource[field];

                        //switch (field)
                        //{
                        //    case "name": orderExpression = x => x.Name; break;
                        //    case "ppe": orderExpression = x => x.PPE; break;
                        //    case "meterCode": orderExpression = x => x.MeterCode; break;
                        //    case "company": orderExpression = x => x.Company.Acronym; break;
                        //    case "tariff": orderExpression = x => x.Tariff.Name; break;
                        //    default: throw new InvalidOperationException("The column is not enabled for sorting.");
                        //}

                        //if (i == 0)
                        //{
                        //    query = isAsc ? query.OrderBy(orderExpression) : query.OrderByDescending(orderExpression);
                        //}
                        //else
                        //{
                        //    IOrderedQueryable<Connection> oq = (IOrderedQueryable<Connection>)query;
                        //    query = isAsc ? oq.ThenBy(orderExpression) : oq.ThenByDescending(orderExpression);

                        //}
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
                        endDate = x.EndDate,
                        orderedCapacity = x.OrderedCapacity,
                        endDateNullable = x.EndDate,
                        orderedCapacityNullable = x.OrderedCapacity
                    }).ToArray();



                var response = new { rows = r, count = count };

                return new JsonNetResult(response);
            }
        }

        IOrderedQueryable<TSource> ApplyOrderByFromLambda<TSource>(IQueryable<TSource> query, LambdaExpression orderExpression, bool isAsc, bool isFirst)
        {
            IOrderedQueryable<TSource> r = null;

            if (orderExpression.Body.Type == typeof(DateTime))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, DateTime>>)orderExpression, isAsc, isFirst);
            }

            if (orderExpression.Body.Type == typeof(DateTime?))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, DateTime?>>)orderExpression, isAsc, isFirst);
            }

            if (orderExpression.Body.Type == typeof(String))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, String>>)orderExpression, isAsc, isFirst);
            }

            if (orderExpression.Body.Type == typeof(Decimal))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, Decimal>>)orderExpression, isAsc, isFirst);
            }

            if (orderExpression.Body.Type == typeof(Decimal?))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, Decimal?>>)orderExpression, isAsc, isFirst);
            }

            if (orderExpression.Body.Type == typeof(int))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, int>>)orderExpression, isAsc, isFirst);
            }

            if (orderExpression.Body.Type == typeof(int?))
            {
                r = ApplyOrderBy(query, (Expression<Func<TSource, int?>>)orderExpression, isAsc, isFirst);
            }


            if (r == null)
            {
                throw new InvalidOperationException(String.Format("The LambdaExpression body type {0} is not supported here.", orderExpression.Body.Type));
            }

            return r;
        }

        IOrderedQueryable<TSource> ApplyOrderBy<TSource, TKey>(IQueryable<TSource> query, Expression<Func<TSource, TKey>> orderExpression, bool isAsc, bool isFirst)
        {
            IOrderedQueryable<TSource> r;

            if (isFirst)
            {
                r = isAsc ? query.OrderBy(orderExpression) : query.OrderByDescending(orderExpression);
            }
            else
            {
                IOrderedQueryable<TSource> oq = (IOrderedQueryable<TSource>)query;
                r = isAsc ? oq.ThenBy(orderExpression) : oq.ThenByDescending(orderExpression);
            }

            return r;
        }


    }
}