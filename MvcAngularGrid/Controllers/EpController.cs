using EnergyPoint.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}