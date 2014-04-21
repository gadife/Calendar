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
    public class CalendarController : Controller
    {
        #region Private Members

        private static Dictionary<string, string> importerColors;

        #endregion Private Members

        #region Static Ctor

        static CalendarController()
        {
            importerColors = new Dictionary<string, string>()
            {
                {"Gmail","green"},
                {"Outlook","blue"},
                {"Israel Holydays","red"},
            };

            //manager = new ImporterManager();
            //manager.AddGmailAccount("ericfeldman93@gmail.com", "ericfeldman789798");
            //manager.AddIsraelHolydays();
        }

        #endregion Static Ctor

        #region Index

        /// <summary>
        /// Shows the user events
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public ActionResult Index(string email)
        {
            // Check for Session , if there is a session display logged on user calendar
            //return View();

            CheckboxValues values = new CheckboxValues();
            if (Session[SessionNames.Email] != null && (email == null || email == Session[SessionNames.Email].ToString()))
            {
                email = Session[SessionNames.Email].ToString();
                values.IsSelfView = true;
            }
            values.UserEmail = email;
            //UserHelper.GetUserEvents(email);
            //manager.GetAllActiveImporters().ToList().ForEach(m => values.Add(m));

            ViewBag.Layout = GetLayout();
            return View(values);
        }

        /// <summary>
        /// Return the Json of the events
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public ActionResult GetEvents(string email)
        {
            //List<string> active = new List<string>();
            //foreach (var item in manager.GetAllActiveImporters())
            //{
            //    if (Request.Params[item] != null && Request.Params[item] != "false")
            //    {
            //        active.Add(item);
            //    }
            //}
            //var allMeetings = active.Count == 0 ? manager.GetAllMeetings() :
            //                                      manager.GetMeetingsOfImporter(active.ToArray());
            Logger.Debug("Start fetching events of {0}", email);

            List<Event> allMeetings = null;
            if (Session[SessionNames.Email] != null && (email == null || email == Session[SessionNames.Email].ToString()))
            {
                allMeetings = UserHelper.GetUserEvents(email);
            }
            else
            {
                allMeetings = SlotHelper.GetBusyTimes(email);
            }

            //var allMeetings = UserHelper.GetUserEvents(email);

            var json = Json(allMeetings.Select(x => new
                                    {
                                        start = ToUnixTimespan(x.StartTime),
                                        //end = !x.IsAllDay ? ToUnixTimespan(x.EndTime) : ToUnixTimespan(x.EndTime.AddDays(-1)),
                                        end = ToUnixTimespan(x.EndTime),
                                        title = GetEventTitle(email,x),
                                        color = importerColors["Outlook"],
                                        location = "location",
                                        allDay = false//x.IsAllDay
                                        //url = GetEventUrl(x)
                                    }).ToArray(),JsonRequestBehavior.AllowGet);

            return json;
        }

        /// <summary>
        /// Show a spacific event by his id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetEvent(long id)
        {
           Event model = EventHelper.GetEventById(id);

            return View("Event", model);
        }

        #endregion Index

        #region Add

        /// <summary>
        /// Returns the Add page 
        /// </summary>
        public ActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// Returns the summery of the busy times of all the users
        /// </summary>
        [HttpPost]
        public ActionResult GetBusyTimes(string users, double duration, double gmtOffset)
        {
            var busy = SlotHelper.GetBusyTimes(users.Split(',').ToList(),
                                               duration,
                                               DateTime.Today.AddHours(gmtOffset),
                                               DateTime.Today.AddDays(7).AddHours(gmtOffset));

            var json = Json(busy.Select(x => new
            {
                start = ToUnixTimespan(x.StartTime),
                end = ToUnixTimespan(x.EndTime),
                title = "Busy",
                color = "Red",
                allDay = false,
            }).ToArray(), JsonRequestBehavior.AllowGet);

            return json;
        }

        /// <summary>
        /// Adds the event to the DB and send the event to all of the users
        /// </summary>
        [HttpPost]
        public ActionResult AddData(string users, string start, string end, double gmtOffset, bool isAllDay, string title, string location, string desc)
        {
            //gmtOffset *= -1;
            //DateTime startDate = start.FromJSFriendlyFormat(true).AddHours(gmtOffset);
            //DateTime endDate = end.FromJSFriendlyFormat(true).AddHours(gmtOffset);

            //EventHelper.SendNewEvent(users.Split(',').ToList(),
            //                         startDate,
            //                         endDate,
            //                         title,
            //                         location,
            //                         desc,
            //                         isAllDay);

            return new EmptyResult();
        }

        /// <summary>
        /// Returns the view of the the additional paramters for the event ( location , title , description etc )
        /// </summary>
        [HttpPost]
        public ActionResult GetAddView(string start, string end, bool allDay)
        {
            return PartialView("_newEvent", new Tuple<string, string, bool>(start, end, allDay));
        }

        #endregion Add

        #region Add2

        [HttpPost]
        public ActionResult SendUserEventOptions(string json, double gmtOffset, string email)
        {
            gmtOffset *= -1;
            string requestId = Guid.NewGuid().ToString();

            List<TempEvent> events = new JavaScriptSerializer().Deserialize<List<TempEvent>>(json);
            foreach (var e in events)
            {
                e.StartDate = e.StartDateString.FromJSFriendlyFormat().AddHours(gmtOffset);
                e.EndDate = e.EndDateString.FromJSFriendlyFormat().AddHours(gmtOffset);
                e.RequestId = requestId;
            }

            TempEventHelper.SaveTempEventsAndSendMail(events, email);

            return new EmptyResult();
        }

        #endregion

        #region Private Methods

        private long ToUnixTimespan(DateTime date)
        {
            // the date is allready UTC , i have saved it that way in the DB
            TimeSpan tspan = date.Subtract(
            new DateTime(1970, 1, 1, 0, 0, 0));

            return (long)Math.Truncate(tspan.TotalSeconds);
        }

        private string GetEventTitle(string email , Event e)
        {
            if (Session[SessionNames.Email] != null && (email == null || email == Session[SessionNames.Email].ToString()))
            {
                return e.Subject;
            }
            else
            {
                //return e.Location;
                return "";
            }
        }

        private string GetEventUrl(Event e)
        {
            if(Session[SessionNames.Email] == null || e.EventUsers.All(u => u.Email != Session[SessionNames.Email].ToString()))
            {
                return null;
            }

            return  "/calendar/event/" + e.Id;
        }

        #endregion Private Methods

        [HttpPost]
        public ActionResult ChangeOptions(CheckboxValues model)
        {
            return new EmptyResult();
        }

        private string GetLayout()
        {
            return Session[SessionNames.Email] == null ? "_Notlogin" : "_Layout";
        }
    }
}
