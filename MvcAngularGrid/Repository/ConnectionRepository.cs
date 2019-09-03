using EnergyPoint.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAngularGrid.Repository
{
    public sealed class ConnectionRepository : IDisposable
    {
        private EnergyPointEntities context;

        public IQueryable<Connection> GetConnections()
        {
            if (context == null)
            {
                context = new EnergyPointEntities();
            }

            return context.Connection;
        }

        public void Dispose()
        {
            if (context != null) context.Dispose();
        }

    }
}