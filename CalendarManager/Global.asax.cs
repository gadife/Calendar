using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
//[assembly: log4net.Config.XmlConfigurator]

namespace CalendarManager
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);   
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            string l4net = Server.MapPath("~/log4net.config");
            //log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(l4net));
            log4net.Config.XmlConfigurator.Configure();

            //EventsImporter.GmailImporter g = new EventsImporter.GmailImporter("ericfeldman93@gmail.com", "ericfeldman789798");
            //List<Models.Event> bla = new List<Models.Event>();
            //foreach (var item in g.GetEvents())
            //{
            //    var e = new Models.Event();
            //    e.StartTime = item.StartTime;
            //    e.EndTime = item.EndTime;
            //    e.IsAllDay = item.IsAllDay;
            //    e.EventId = item.Id;
            //    e.EventUsers.Add(new Models.EventUser(){Email="ericfeldman93@gmail.com"});
            //    bla.Add(e);
            //}
            //Helpers.EventHelper.UpdateOrCreate(bla);
        }
    }
}