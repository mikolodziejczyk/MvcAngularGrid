using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcAngularGrid.Controllers
{
    public class ListSettingsController : Controller
    {
        private readonly Repository.ListSettingsRepository listSettingsRepository;

        public ListSettingsController(MvcAngularGrid.Repository.ListSettingsRepository listSettingsRepository)
        {
            this.listSettingsRepository = listSettingsRepository ?? throw new ArgumentNullException(nameof(listSettingsRepository));
        }

        // GET: ListSettings
        public ActionResult GetListSettings(string viewId)
        {
            int userId = 1; // get the id of the user making the request here

            string viewSettings = this.listSettingsRepository.GetListSettings(userId, viewId);

            return new JsonNetResult(viewSettings);
        }

        // GET: ListSettings
        public ActionResult SaveListSettings(string viewId, string settings)
        {
            int userId = 1; // get the id of the user making the request here

            this.listSettingsRepository.SaveOrUpdateListSettings(userId, viewId, settings);

            return new EmptyResult();
        }
    }
}