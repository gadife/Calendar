using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CalendarManager
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            AddCalendarRoutes(routes);
            AddUserRoutes(routes);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "About", action = "Homepage", id = UrlParameter.Optional }
            );
        }

        private static void AddCalendarRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "Get user events as json",
                "Calendar/events/{email}",
                new { controller = "Calendar", action = "GetEvents", id = UrlParameter.Optional },
                new { email = @"^[A-Za-z0-9._%+-]+@[a-zA-Z0-9.-]+\.[A-Za-z]{1,4}$" }
            );

            routes.MapRoute(
                name: "Show event",
                url: "Calendar/event/{id}",
                defaults: new { controller = "Calendar", action = "GetEvent", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Show user events",
                "Calendar/{email}",
                new { controller = "Calendar", action = "Index", id = UrlParameter.Optional },
                new { email = @"^[A-Za-z0-9._%+-]+@[a-zA-Z0-9.-]+\.[A-Za-z]{1,4}$" }
            );

            routes.MapRoute(
                "Calendar index",
                "Calendar/",
                new { controller = "Calendar", action = "Index", id = UrlParameter.Optional }
            );
        }

        private static void AddUserRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: "User login",
                url: "login",
                defaults: new { controller = "User", action = "login" }
            );
        }
    }
}