using EnergyPoint.Repository;
using MvcAngularGrid.Models.AgGrid;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

                    Expression<Func<Connection, bool>> filterExpression;

                    switch (field)
                    {
                        case "name": filterExpression = x => x.Name.Contains(value); break;
                        case "ppe": filterExpression = x => x.PPE.Contains(value); break;
                        case "meterCode": filterExpression = x => x.MeterCode.Contains(value); break;
                        case "company": filterExpression = x => x.Company.Acronym.Contains(value); break;
                        case "tariff": filterExpression = x => x.Tariff.Name.Contains(value); break;
                        default: throw new InvalidOperationException("The column is not enabled for filtering.");
                    }

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

                        Expression<Func<Connection, object>> orderExpression;

                        switch (field)
                        {
                            case "name": orderExpression = x => x.Name; break;
                            case "ppe": orderExpression = x => x.PPE; break;
                            case "meterCode": orderExpression = x => x.MeterCode; break;
                            case "company": orderExpression = x => x.Company.Acronym; break;
                            case "tariff": orderExpression = x => x.Tariff.Name; break;
                            default: throw new InvalidOperationException("The column is not enabled for sorting.");
                        }

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

        //public ActionResult Page(int first, int rows, string globalFilter, SortMeta[] multiSortMeta, IDictionary<string, FilterEntry> filters)
        //{

        //    string data;

        //    using (EnergyPointEntities context = new EnergyPointEntities())
        //    {
        //        IQueryable<Connection> query = context.Connection;

        //        if (globalFilter != null)
        //        {
        //            query = query.Where(x => x.Name.Contains(globalFilter) || x.PPE.Contains(globalFilter) ||
        //                                     x.MeterCode.Contains(globalFilter) || x.LocationDescription.Contains(globalFilter) ||
        //                                     x.Company.Name1.Contains(globalFilter) || x.Company.Acronym.Contains(globalFilter) ||
        //                                     x.Tariff.Name.Contains(globalFilter)
        //                                     );
        //        }


        //        // if filters aren't posted, in filters we get values from MVC; with "action", "controller" as keys; they must be excluded, currently by their null value criterion
        //        foreach (var kvp in filters.Where(x => x.Value != null).Where(x => x.Key != "global"))
        //        {
        //            string field = kvp.Key;
        //            string value = kvp.Value.value;

        //            Expression<Func<Connection, bool>> filterExpression;

        //            switch (field)
        //            {
        //                case "name": filterExpression = x => x.Name.Contains(value); break;
        //                case "ppe": filterExpression = x => x.PPE.Contains(value); break;
        //                case "meterCode": filterExpression = x => x.MeterCode.Contains(value); break;
        //                case "company": filterExpression = x => x.Company.Acronym.Contains(value); break;
        //                case "tariff": filterExpression = x => x.Tariff.Name.Contains(value); break;
        //                default: throw new InvalidOperationException("The column is not enabled for filtering.");
        //            }

        //            query = query.Where(filterExpression);

        //            // test

        //            //Expression<Func<Connection, string>> filterBase;
        //            //filterBase = x => x.PPE;

        //            //Expression<Func<string, bool>> filterMethod = x => x.Contains(value);





        //            //Expression<Func<Connection, string>> stringPropertyExpression;

        //            //switch (field)
        //            //{
        //            //    case "name": stringPropertyExpression = x => x.Name; break;
        //            //    case "ppe": stringPropertyExpression = x => x.PPE; break;
        //            //    case "meterCode": stringPropertyExpression = x => x.MeterCode; break;
        //            //    case "company": stringPropertyExpression = x => x.Company.Acronym; break;
        //            //    case "tariff": stringPropertyExpression = x => x.Tariff.Name; break;
        //            //    default: throw new InvalidOperationException("The column is not enabled for sorting.");
        //            //}

        //            //filterExpression = Expression.Call(stringPropertyExpression, "Contains", new Type[] { typeof(string) }, new Expression[] { Expression.Constant(value) });

        //            //if (kvp.Key == "ppe")
        //            //{
        //            //    query = query.Where(x => x.PPE.Contains(kvp.Value.value));
        //            //}

        //            //if (kvp.Key == "meterCode")
        //            //{
        //            //    query = query.Where(x => x.MeterCode.Contains(kvp.Value.value));
        //            //}

        //            //if (kvp.Key == "company")
        //            //{
        //            //    query = query.Where(x => x.Company.Acronym.Contains(kvp.Value.value));
        //            //}

        //            //if (kvp.Key == "tariff")
        //            //{
        //            //    query = query.Where(x => x.Tariff.Name.Contains(kvp.Value.value));
        //            //}

        //            //if (kvp.Key == "name")
        //            //{
        //            //    query = query.Where(x => x.Name.Contains(kvp.Value.value));
        //            //}


        //        }

        //        int count = query.Count();


        //        if (multiSortMeta != null)
        //        {
        //            for (int i = 0; i < multiSortMeta.Length; i++)
        //            {

        //                string field = multiSortMeta[i].field;
        //                bool isAsc = multiSortMeta[i].order == 1;
        //                Expression<Func<Connection, object>> orderExpression;

        //                switch (field)
        //                {
        //                    case "name": orderExpression = x => x.Name; break;
        //                    case "ppe": orderExpression = x => x.PPE; break;
        //                    case "meterCode": orderExpression = x => x.MeterCode; break;
        //                    case "company": orderExpression = x => x.Company.Acronym; break;
        //                    case "tariff": orderExpression = x => x.Tariff.Name; break;
        //                    default: throw new InvalidOperationException("The column is not enabled for sorting.");
        //                }

        //                if (i == 0)
        //                {
        //                    query = isAsc ? query.OrderBy(orderExpression) : query.OrderByDescending(orderExpression);
        //                }
        //                else
        //                {
        //                    IOrderedQueryable<Connection> oq = (IOrderedQueryable<Connection>)query;
        //                    query = isAsc ? oq.ThenBy(orderExpression) : oq.ThenByDescending(orderExpression);

        //                }


        //                //if (i == 0)
        //                //{
        //                //    switch (field)
        //                //    {
        //                //        case "name": query = isAsc ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name); break;
        //                //        case "ppe": query = isAsc ? query.OrderBy(x => x.PPE) : query.OrderByDescending(x => x.PPE); break;
        //                //        case "meterCode": query = isAsc ? query = query.OrderBy(x => x.MeterCode) : query.OrderByDescending(x => x.MeterCode); break;
        //                //        case "company": query = isAsc ? query.OrderBy(x => x.Company.Acronym) : query.OrderByDescending(x => x.Company.Acronym); break;
        //                //        case "tariff": query = isAsc ? query.OrderBy(x => x.Tariff.Name) : query.OrderByDescending(x => x.Tariff.Name); break;
        //                //            //case "Tariff.NameDESC": query = query.OrderByDescending(x => x.Tariff.Name); break;
        //                //            //case "StartDateASC": query = query.OrderBy(x => x.StartDate); break;
        //                //            //case "StartDateDESC": query = query.OrderByDescending(x => x.StartDate); break;
        //                //            //case "EndDateASC": query = query.OrderBy(x => x.EndDate); break;
        //                //            //case "EndDateDESC": query = query.OrderByDescending(x => x.EndDate); break;
        //                //    }
        //                //}
        //                //else
        //                //{
        //                //    IOrderedQueryable<Connection> oq = (IOrderedQueryable<Connection>)query;

        //                //    switch (field)
        //                //    {
        //                //        case "name": query = isAsc ? oq.ThenBy(x => x.Name) : oq.ThenByDescending(x => x.Name); break;
        //                //        case "ppe": query = isAsc ? oq.ThenBy(x => x.PPE) : oq.ThenByDescending(x => x.PPE); break;
        //                //        case "meterCode": query = isAsc ? oq.ThenBy(x => x.MeterCode) : oq.ThenByDescending(x => x.MeterCode); break;
        //                //        case "company": query = isAsc ? oq.ThenBy(x => x.Company.Acronym) : oq.ThenByDescending(x => x.Company.Acronym); break;
        //                //        case "tariff": query = isAsc ? oq.ThenBy(x => x.Tariff.Name) : oq.ThenByDescending(x => x.Tariff.Name); break;
        //                //            //case "meterCodeDESC": query = query.OrderByDescending(x => x.MeterCode); break;
        //                //            //case "Company.AcronymASC": query = query.OrderBy(x => x.Company.Acronym); break;
        //                //            //case "Company.AcronymDESC": query = query.OrderByDescending(x => x.Company.Acronym); break;
        //                //            //case "Tariff.NameASC": query = query.OrderBy(x => x.Tariff.Name); break;
        //                //            //case "Tariff.NameDESC": query = query.OrderByDescending(x => x.Tariff.Name); break;
        //                //            //case "StartDateASC": query = query.OrderBy(x => x.StartDate); break;
        //                //            //case "StartDateDESC": query = query.OrderByDescending(x => x.StartDate); break;
        //                //            //case "EndDateASC": query = query.OrderBy(x => x.EndDate); break;
        //                //            //case "EndDateDESC": query = query.OrderByDescending(x => x.EndDate); break;
        //                //    }
        //                //}
        //            }
        //        }
        //        else
        //        {
        //            Expression<Func<Connection, object>> orderExpression = x => x.PPE;
        //            query = query.OrderBy(orderExpression);
        //            // query = query.OrderBy(x => x.PPE);
        //        }

        //        var r = query.Skip(first).Take(rows).Select(x =>
        //            new
        //            {
        //                id = x.Id,
        //                ppe = x.PPE,
        //                meterCode = x.MeterCode,
        //                name = x.Name,
        //                tariff = x.Tariff.Name,
        //                company = x.Company.Acronym,
        //                startDate = x.StartDate,
        //                endDate = x.EndDate
        //            });

        //        var response = new { rows = r, count = count };

        //        data = JsonConvert.SerializeObject(response);

        //    }


        //    return Content(data, "application/json");
        //}

    }
}