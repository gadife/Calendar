using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CalendarManager.Models;
using System.Data.Entity;
using System.Data.Entity.Validation;
using EventsImporter;

namespace CalendarManager.Helpers
{
    public static class EventHelper
    {
        #region Public Methods

        //public static bool UpdateOrCreate(IEnumerable<Event> events, long userId)
        //{
        //    using (EventsBbEntities context = new EventsBbEntities())
        //    {
        //        //User user = context.Users.FirstOrDefault(u=>u.userus
        //        var events = from user in context.Users
        //                     join e in context.Events
        //                     on e.
                             
        //    }
        //}

        public static bool UpdateOrCreate(IEnumerable<Event> events)
        {
            bool result = true;

            //TODO : Transactions
            //using (EventsBbEntities context = new EventsBbEntities())
            EventsBbEntities context = null;
            try
            {
                context = new EventsBbEntities();
                RemoveDeletedEvents(context, events);

                foreach (var e in events)
                {
                    Logger.Debug("Start working on event named '{0}' ( {1} )", e.Subject,e.EventId);

                    Event dbEvent = null;
                    if (e.IsReccurnce.Value)
                    {
                        Logger.Debug("{0} is a reccurence event", e.Subject);
                        dbEvent = context.Events.FirstOrDefault(eve => eve.EventId == e.EventId &&
                                                                       eve.StartTime == e.StartTime &&
                                                                       eve.EndTime == e.EndTime);
                    }
                    else
                    {
                        dbEvent = context.Events.FirstOrDefault(eve => eve.EventId == e.EventId);
                    }
                    UpdateEventData(ref dbEvent, e, context);

                    try
                    {
                        context.SaveChanges();
                        result &= true;
                        Logger.Debug("Insert/Updated event named '{0}' succsessfully", e.Subject);
                    }
                    catch (DbEntityValidationException ex)
                    {
                        result &= false;
                        Logger.Error(ex, "An exception as accured while saving event name '{0}'. validation errors : {1}", 
                                         e.Subject, 
                                         string.Join(" , ",ex.EntityValidationErrors.First().ValidationErrors.Select(v=>v.PropertyName+" - "+v.ErrorMessage)));
                    }
                    catch (Exception ex)
                    {
                        result &= false;
                        Logger.Error(ex, "An exception as accured while saving event name '{0}'", e.Subject);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "An error as accured");
            }
            finally
            {
                if(context != null)
                {
                    context.Dispose();    
                }
            }

            string logMessage = result ? "Seccsessfully insert/updated all the event" : "Faild insert/update all events . Check previus logs for more info";
            Logger.Info(logMessage);

            return result;
        }

        public static Event GetEventById(long eventId)
        {
            using (EventsBbEntities context = new EventsBbEntities())
            {
                return context.Events.Include(e => e.EventUsers)
                                     .FirstOrDefault(e => e.Id == eventId);
            }
        }

        public static void AddEvent(string usersString, DateTime start, DateTime end)
        {
            //List<User> users = usersString.Split(',').Select(u => new User()
            //                                               {
            //                                                   Email = u,
            //                                               }); 
        }

        public static bool SendNewEvent(List<string> users, DateTime start, DateTime end, string title,string location,string desc,bool isAllDay)
        {
            string eventId = Guid.NewGuid().ToString();
            Event e = new Event()
            {
                StartTime = start,
                EndTime = end,
                LastUpdateTime = DateTime.Now,
                IsAllDay = isAllDay,
                EventId = eventId,
                Location = location,
            };

            foreach (var user in users)
            {
                e.EventUsers.Add(new EventUser()
                {
                    Email = user,
                    Approved = false,
                });
            }

            using (EventsBbEntities context = new EventsBbEntities())
            {
                foreach (var mail in users)
                {
                    User user = context.Users.Include(u => u.Provider).FirstOrDefault(u => u.Email == mail);

                    // Maybe send the mails in another thread or save the users and send after the connection to the DB is closed
                    if (user == null)
                    {
                        EventImporter imp = new EventImporter(mail);
                        imp.SendEvent(e.ToDto(title,desc));
                    }
                    else
                    {
                        user.Provider.Initialize().AddEvent(e.ToDto(title,desc));
                    }
                    
                    //e.EventId = eventId; // each provider will have a different id , insert the event for each EventId will fix the problem
                    // Maybe i can set the event id !
                }

                return true;
                //try
                //{
                //    context.SaveChanges(); // Maybe not add but in the next sync the event will apper 
                //    return true;
                //}
                //catch
                //{
                //    return false;
                //}
            }


        }

        #endregion Public Methods

        #region Private Methods

        private static void UpdateEventData(ref Event dbEvent, Event userEvent ,EventsBbEntities context)
        {
            if (dbEvent == null)
            {
                dbEvent = new Event();
                dbEvent.EventId = userEvent.EventId;
                context.Events.Add(dbEvent);
            }

            dbEvent.LastUpdateTime = DateTime.Now.ToUniversalTime();
            dbEvent.Location = userEvent.Location;
            dbEvent.StartTime = userEvent.StartTime.ToUniversalTime();
            dbEvent.EndTime = userEvent.EndTime.ToUniversalTime();
            dbEvent.IsAllDay = userEvent.IsAllDay;
            dbEvent.OrganizerMail = userEvent.OrganizerMail;
            dbEvent.Subject = userEvent.Subject;

            UpdateEventUsers(dbEvent, userEvent,context);
        }

        private static void UpdateEventUsers(Event dbEvent, Event userEvent,EventsBbEntities context)
        {
            var toRemove = dbEvent.EventUsers.Select(u => u.Email)
                                             .Except(userEvent.EventUsers.Select(u => u.Email))
                                             .ToList();

            //var toAdd = userEvent.EventUsers.Select(u => u.Email)
            //                                 .Except(dbEvent.EventUsers.Select(u => u.Email));

            var toAdd = userEvent.EventUsers.Where(u => !dbEvent.EventUsers.Select(du => du.Email)
                                                                        .Contains(u.Email));

            foreach (var item in dbEvent.EventUsers)
            {
                context.EventUsers.Attach(item);
            }

            foreach (var userMail in toRemove)
            {
                EventUser u = dbEvent.EventUsers.First(e => e.Email == userMail);
                
                dbEvent.EventUsers.Remove(u);
                context.EventUsers.Remove(u);
            }

            foreach (var user in toAdd)
            {
                dbEvent.EventUsers.Add(user);
            }
        }

        private static void RemoveDeletedEvents(EventsBbEntities context, IEnumerable<Event> userEvents)
        {
            var ids = userEvents.Select(ue => ue.EventId);
            var toRemove = context.Events.Include(e=>e.EventUsers)
                                         .Where(e => e.EndTime > DateTime.Today &&
                                                    (!ids.Contains(e.EventId)));

            foreach (var e in toRemove)
            {
                List<EventUser> users = e.EventUsers.ToList();
                foreach (var u in users)
                {
                    context.EventUsers.Remove(u);
                }
                context.Events.Remove(e);
            }
        }

        private static string SendDefaultEvent(string mail,string title ,Event e)
        {
            throw new NotImplementedException();
        }

        #endregion Private Methods
    }
}