using CalendarManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CalendarManager.Helpers
{
    public static class UserHelper
    {
        public static bool Authenticate(long userId, string hasedPassword)
        {
            //using (EventsBbEntities context = new EventsBbEntities())
            //{
            //    return context.Users.Any(u => u.Id == userId && u.Password == hasedPassword);
            //}
            return true;
        }

        public static bool Authenticate(string email, string password)
        {
            //using (EventsBbEntities context = new EventsBbEntities())
            //{
            //    return context.Users.Any(u => u.Email == email && u.Password == password);
            //}
            return true;
        }

        public static List<Event> GetUserEvents(string email)
        {
            using (EventsBbEntities context = new EventsBbEntities())
            {
                User user = context.Users.FirstOrDefault(u => u.Email == email);

                if (user == null)
                {
                    return new List<Event>();
                }
                else
                {
                    return user.GetEvents();
                }
            }
        }

        public static Dictionary<ReturnUser,List<TempEvent>> GetTempEvents(string email)
        {
            using (EventsBbEntities context = new EventsBbEntities())
            {
                User user = context.Users.Include(u=>u.TempEvents).FirstOrDefault(u => u.Email == email);

                return user.TempEvents.GroupBy(e => e.RequestId).ToDictionary(s => new ReturnUser(s.First().ReturnMail,s.First().ReturnName),
                                                                              s => s.ToList());
            }
        }
    }
}