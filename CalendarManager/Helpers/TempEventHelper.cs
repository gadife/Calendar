using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CalendarManager.Models;
using System.Data.Entity;
using EventsImporter;

namespace CalendarManager.Helpers
{
    public static class TempEventHelper
    {
        #region Public Methods

        public static void SaveTempEventsAndSendMail(List<TempEvent> events, string mail)
        {
            using (EventsBbEntities context = new EventsBbEntities())
            {
                User user = context.Users.First(u => u.Email == mail);

                foreach (var e in events)
                {
                    e.User = user;
                    context.TempEvent.Add(e);
                }

                context.SaveChanges();
            }

            TempEvent first = events.First();
            
            EmailHelper.SendMail(mail,
                                "New meeting request", 
                                 string.Format("{0} ({1}) has inveted you to a meeting. <br> You can choose between the sujected times in the Requests panel in the site",
                                              first.ReturnName, first.ReturnMail));
        }

        public static TempEvent GetEvent(long id)
        {
            using (EventsBbEntities context = new EventsBbEntities())
            {
                return context.TempEvent.Include(t => t.User.Provider).First(e => e.Id == id);
            }
        }

        public static void HandleApprove(TempEvent e)
        {
            SendICSes(e);
            RemoveTemps(e.RequestId);
            // Insert Event To DB
        }

        public static int GetRequestCount(string mail)
        {
            using (EventsBbEntities context = new EventsBbEntities())
            {
                return context.TempEvent.Where(t => t.User.Email == mail).GroupBy(r=>r.RequestId).Count();
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static void SendICSes(TempEvent e)
        {
            Logger.Debug("Start sending ICSes about meeting {0} to requester and user", e.Title);

            // send to the user
            Logger.Debug("Send ICS of '{0}' to the user ({1})", e.Title, e.User.Email);
            Logger.Debug("Provider : {0}", e.User.GetType());
            e.User.Provider.Initialize().AddEvent(e.ToEvent());

            // send to the requester
            Logger.Debug("Start Send ICS to the meeting requester ({0})",e.ReturnMail);
            EventImporter imp = new EventImporter(e.ReturnMail);
            imp.SendEvent(e.ToEvent(), string.Format("{0} has approved your meeting", e.User.Name));
            Logger.Debug("Finish sending ICS to the meeting requester ({0})", e.ReturnMail);

            Logger.Debug("Finish sending ICSes about meeting {0} to requester and user", e.Title);
        }

        private static void RemoveTemps(string requestId)
        {
            using (EventsBbEntities context = new EventsBbEntities())
            {
                var toDelete = context.TempEvent.Where(e => e.RequestId == requestId);
                foreach (var temp in toDelete)
                {
                    context.TempEvent.Remove(temp);
                }

                context.SaveChanges();
            }
        }

        #endregion Private Methods
    }
}