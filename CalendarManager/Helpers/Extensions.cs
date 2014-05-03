using CalendarManager.Models;
using EventsImporter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace CalendarManager.Helpers
{
    public static class Extensions
    {
        public static string ToJSFriendlyFormat(this DateTime date)
        {
            return date.ToString("dd:MM:yyyy:HH:mm:ss");
        }

        public static DateTime FromJSFriendlyFormat(this string dateString, bool assumeUTC = false)
        {
            DateTime date = DateTime.ParseExact(dateString, "dd:MM:yyyy:HH:mm:ss", CultureInfo.InvariantCulture, assumeUTC ? DateTimeStyles.AssumeUniversal : DateTimeStyles.None);
            if (assumeUTC)
            {
                DateTime.SpecifyKind(date, DateTimeKind.Utc);
            }
            return date;
        }

        public static DateTime ToDate(this string dateString)
        {
            return DateTime.ParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Help method that manages the active navbar menuitem 
        /// </summary>
        /// <returns></returns>
        public static MvcHtmlString MenuLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object htmlParams = null)
        {
            var currentAction = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
            var currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");

            var builder = new TagBuilder("li")
            {
                InnerHtml = htmlHelper.ActionLink(linkText, actionName, controllerName, null, htmlParams).ToHtmlString()
            };

            if (controllerName == currentController && actionName == currentAction)
                builder.AddCssClass("active");

            return new MvcHtmlString(builder.ToString());
        }

        public static EventDto ToDto(this Event e, string title, string desc)
        {
            return new EventDto()
            {
                EndTime = e.EndTime,
                StartTime = e.StartTime,
                Name = title,
                IsAllDay = e.IsAllDay,
                Attending = e.EventUsers.Select(s => new Attending(s.Email, false)).ToList(),
                Location = e.Location,
                Description = desc
            };
        }

        public static EventDto ToEvent(this TempEvent e)
        {
            return new EventDto()
            {
                StartTime = e.StartDate,
                EndTime = e.EndDate,
                Name = e.Title,
                Location = e.Location,
                Description = e.Desc,
                IsAllDay = e.IsAllDay,
                Organizer = e.ReturnMail,
                Attending = new List<Attending>()
                           {
                               new Attending(e.User.Email,true),
                               new Attending(e.ReturnMail,false)
                           }
            };
        }
    }
}