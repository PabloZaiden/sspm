using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SelfServicePasswordManagement
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static bool EnforceHTTPS = false;

        protected void Application_Start()
        {
            if (ConfigurationManager.AppSettings["EnforceHTTPS"] == "true")
            {
                EnforceHTTPS = true;
            }

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest()
        {
            if (EnforceHTTPS && !Context.Request.IsSecureConnection)
            {
                Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));
            }
        }
    }
}
