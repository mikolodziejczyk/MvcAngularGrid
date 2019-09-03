using Autofac;
using Autofac.Integration.Mvc;
using MvcAngularGrid.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MvcAngularGrid
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();

            // Register your MVC controllers. (MvcApplication is the name of
            // the class in Global.asax.)
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // assembly scanning for an IIS hosted application
            Assembly[] assemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();
            builder.RegisterAssemblyTypes(assemblies).AsSelf();
            // any othe registration code
            builder.RegisterType<ConnectionRepository>().AsSelf().InstancePerRequest();

            // here we can add code for ModelBinders etc.

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));


            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
