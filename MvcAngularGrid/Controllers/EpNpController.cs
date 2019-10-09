using EnergyPoint.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcAngularGrid.Controllers
{
    public class EpNpController : Controller
    {
        private readonly Repository.ConnectionRepository connectionRepository;

        public EpNpController(MvcAngularGrid.Repository.ConnectionRepository connectionRepository)
        {
            this.connectionRepository = connectionRepository ?? throw new ArgumentNullException(nameof(connectionRepository));
        }

        // GET: EpNp
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Connections()
        {
            IQueryable<Connection> query = connectionRepository.GetConnections();

            var r = query.Select(x => new
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
                isActive = x.Name.Length % 2 == 0 // fake boolean column
            }).ToArray();

            return new JsonNetResult(r);
        }
    }
}