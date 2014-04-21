using CalendarManager.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CalendarManager.Models
{
    public partial class User
    {
        public List<Event> GetEvents()
        {
            using (EventsBbEntities context = new EventsBbEntities())
            {
                return context.Events.Where(e => e.EventUsers.Select(u => u.Email)
                                                             .Contains(this.Email))
                                     .ToList();
            }
        }
    }
}