using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CalendarManager.Models;
using CalendarManager.Helpers;
using System.Web.Script.Serialization;

namespace CalendarManager.Controllers
{
    public class CalendarApiController : Controller
    {
        [HttpPost]
        public ActionResult Insert(long userId, string hashedPassword, string json)
        {
            //try
            //{
            Logger.Info("Got new message from user id {0}", userId);

            if (UserHelper.Authenticate(userId, hashedPassword))
            {
                List<Event> events = new JavaScriptSerializer().Deserialize<List<Event>>(json);
                Logger.Debug("Finish deserialize json for user id {0} . There are {1} events", userId, events.Count);

                EventHelper.UpdateOrCreate(events);
            }

            return new EmptyResult();
            //}
            //catch (Exception e)+
            //{
            //    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Server error : " + GetInnerMessage(e), e);
            //}
        }

        //[HttpPost]
        //public ActionResult InsertByUser(long userId, string hashedPassword, string json)
        //{
        //    if (UserHelper.Authenticate(userId, hashedPassword))
        //    {
        //        List<Event> events = new JavaScriptSerializer().Deserialize<List<Event>>(json);
        //        Logger.Debug("Finish deserialize json for user id {0} . There are {1} events", userId, events.Count);

        //        EventHelper.UpdateOrCreate(events);
        //    }

        //    return new EmptyResult();
        //}

        private string GetInnerMessage(Exception e)
        {
            while (e.InnerException != null)
            {
                e = e.InnerException;
            }

            return e.Message;
        }
    }
}
