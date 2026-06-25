
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MODULO_CLIENTE
{
    public class MvcApplication : System.Web.HttpApplication
    {
       
        protected void Application_Start()
        {

           
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            UnityConfig.RegisterComponents();

        }


        protected void Application_BeginRequest()
        {
            string path = Request.Url.AbsolutePath;

            if (path.EndsWith(".js") || path.EndsWith(".css"))
            {
                Response.Cache.SetCacheability(HttpCacheability.Public);
                Response.Cache.SetMaxAge(TimeSpan.FromDays(365));
                Response.Cache.SetExpires(DateTime.UtcNow.AddDays(365));
            }
        }

    }
}
