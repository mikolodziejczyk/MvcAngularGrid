using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAngularGrid.Repository
{
    public class ListSettingsRepository
    {
        static readonly ILog log = LogManager.GetLogger(typeof(ListSettingsRepository));

        /// <summary>
        /// The maximum legth of view settings data.
        /// </summary>
        const int viewDataMaxLength = 100000;
        /// <summary>
        /// The number of saved views a single user can have; this is an order of magnitude more than maximum number expected views.
        /// </summary>
        const int maxViewsPerUser = 1000;

        public string GetListSettings(int userId, string viewId)
        {
            string r = null;

            using (AngularGridEntities context = new AngularGridEntities())
            {
                ListSettings ls = context.ListSettings.Where(x => x.UserId == userId && x.ViewId == viewId).FirstOrDefault();
                if (ls != null) r = ls.Data;
            }

            return r;
        }

        public void SaveOrUpdateListSettings(int userId, string viewId, string data)
        {
            if (data == null) data = "";

            if (data.Length > viewDataMaxLength)
            {
                log.ErrorFormat("Saving the view settings failed for user {0} and list {1}; the settings length is {2} and exceeds the {3} limit.", userId, viewId, data.Length, viewDataMaxLength);
                throw new InvalidOperationException("The view settings data exceeds the maximum length.");
            }

            using (AngularGridEntities context = new AngularGridEntities())
            {
                ListSettings ls = context.ListSettings.Where(x => x.UserId == userId && x.ViewId == viewId).FirstOrDefault();
                // we don't need a transaction here as there's an unique index in the database

                if (ls != null)
                {
                    // update existing entry
                    ls.Data = data;
                    log.DebugFormat("Updating the view settings for user {0} and list {1}.");
                }
                else
                {
                    // create a new entry

                    // count the existing entries for the user to prevent malicious pollution
                    int existingEntriesCount = context.ListSettings.Where(x => x.UserId == userId).Count();
                    if (existingEntriesCount > maxViewsPerUser)
                    {
                        log.ErrorFormat("Saving the view settings failed for user {0} and list {1}; the current settings count is {2} and exceeds the {3} limit.", userId, viewId, existingEntriesCount, maxViewsPerUser);
                        throw new InvalidOperationException("The limit of view settings entries for the user has been exceeded.");
                    }

                    ListSettings listSettings = new ListSettings();
                    listSettings.UserId = userId;
                    listSettings.ViewId = viewId;
                    listSettings.Data = data;

                    context.ListSettings.Add(listSettings);

                    log.DebugFormat("Creating a new view settings entry for user {0} and list {1}.");
                }

                context.SaveChanges();
                log.DebugFormat("Changes in settings for user {0} and list {1} have been successfully saved to the database.");
            }
        }
    }
}