using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace VigoBAS_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Aktiver brukerkonto",
                url: "{controller}/{action}",
                defaults: new { controller = "ActivateUser", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Glemt passord",
                url: "{controller}/{action}",
                defaults: new { controller = "ForgottenPassword", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Glemt brukernavn",
                url: "{controller}/{action}",
                defaults: new { controller = "ForgottenUsername", action = "Index", id = UrlParameter.Optional }
            );
            
            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

        }
    }
}
