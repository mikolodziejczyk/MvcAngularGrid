using EnergyPoint.Repository;
using GridServerSideSortingAndFiltering.AgGrid;
using GridServerSideSortingAndFiltering.ExpressionList;
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
        public EpController(MvcAngularGrid.Repository.ConnectionRepository connectionRepository)
        {
            this.connectionRepository = connectionRepository ?? throw new ArgumentNullException(nameof(connectionRepository));
        }

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

        /// <summary>
        /// Expressions for all sortable and filterable columns in the grid
        /// In expressions, enums must be cast to int.
        /// In expressions, nullables can be used as nullables or converted to a non-nullable values.
        /// </summary>
        static IDictionary<string, LambdaExpression> columnSource = new Dictionary<string, LambdaExpression>()
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
        private readonly Repository.ConnectionRepository connectionRepository;

        public ActionResult Page(int startRow, int endRow, SortEntry[] sortModel, Dictionary<string, FilterEntry> filterModel, string globalFilter)
        {

            IQueryable<Connection> query = connectionRepository.GetConnections();

            // if filters aren't posted, in filters we get values from MVC; with "action", "controller" as keys; they must be excluded, currently by their null value criterion
            foreach (var kvp in filterModel.Where(x => x.Value != null))
            {
                // get expression for this column
                LambdaExpression columnExpression = columnSource[kvp.Key];
                // convert grid-specific filter to an universal entry
                UniversalFilterEntry universalFilterEntry = FilterEntryConverter.Convert(kvp.Value);
                // check whether the entry was parsed successfully
                if (universalFilterEntry == null) continue;
                // get the fltering expression from universalFilterEntry
                Expression<Func<Connection, bool>> filterExpression = FilterExpressions<Connection>.GetFilterExpression(columnExpression, universalFilterEntry);
                // and apply it to the query
                query = query.Where(filterExpression);
            }

            // global filtering 
            if (String.IsNullOrWhiteSpace(globalFilter) == false)
            {
                query = query.Where(x => x.Name.Contains(globalFilter) || x.PPE.Contains(globalFilter) || x.MeterCode.Contains(globalFilter) || x.Company.Acronym.Contains(globalFilter) || x.Tariff.Name.Contains(globalFilter));
            }

            int count = query.Count();

            if (sortModel != null)
            {
                for (int i = 0; i < sortModel.Length; i++)
                {
                    SortEntry sortEntry = sortModel[i];

                    string column = sortEntry.colId;
                    bool isAsc = sortEntry.sort == SortEntry.asc;
                    bool isFirst = i == 0;

                    LambdaExpression columnExpression = columnSource[column];

                    query = SortHelper.ApplyOrderByFromLambda(query, columnExpression, isAsc, isFirst);
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
                    orderedCapacityNullable = x.OrderedCapacity,
                }).ToArray();


            var response = new { rows = r, count = count };

            return new JsonNetResult(response);
        }


        public ActionResult PageNoRepository(int startRow, int endRow, SortEntry[] sortModel, Dictionary<string, FilterEntry> filterModel, string globalFilter)
        {
            using (EnergyPointEntities context = new EnergyPointEntities())
            {
                IQueryable<Connection> query = context.Connection;


                // if filters aren't posted, in filters we get values from MVC; with "action", "controller" as keys; they must be excluded, currently by their null value criterion
                foreach (var kvp in filterModel.Where(x => x.Value != null))
                {
                    // get expression for this column
                    LambdaExpression columnExpression = columnSource[kvp.Key];
                    // convert grid-specific filter to an universal entry
                    UniversalFilterEntry universalFilterEntry = FilterEntryConverter.Convert(kvp.Value);
                    // check whether the entry was parsed successfully
                    if (universalFilterEntry == null) continue;
                    // get the fltering expression from universalFilterEntry
                    Expression<Func<Connection, bool>> filterExpression = FilterExpressions<Connection>.GetFilterExpression(columnExpression, universalFilterEntry);
                    // and apply it to the query
                    query = query.Where(filterExpression);
                }

                // global filtering 
                if (String.IsNullOrWhiteSpace(globalFilter) == false)
                {
                    query = query.Where(x => x.Name.Contains(globalFilter) || x.PPE.Contains(globalFilter) || x.MeterCode.Contains(globalFilter) || x.Company.Acronym.Contains(globalFilter) || x.Tariff.Name.Contains(globalFilter));
                }

                int count = query.Count();

                if (sortModel != null)
                {
                    for (int i = 0; i < sortModel.Length; i++)
                    {
                        SortEntry sortEntry = sortModel[i];

                        string column = sortEntry.colId;
                        bool isAsc = sortEntry.sort == SortEntry.asc;
                        bool isFirst = i == 0;

                        LambdaExpression columnExpression = columnSource[column];

                        query = SortHelper.ApplyOrderByFromLambda(query, columnExpression, isAsc, isFirst);
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
                        orderedCapacityNullable = x.OrderedCapacity,
                    }).ToArray();



                var response = new { rows = r, count = count };

                return new JsonNetResult(response);
            }
        }




    }
}