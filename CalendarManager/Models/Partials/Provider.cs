using EventsImporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CalendarManager.Models
{
    public partial class Provider
    {
        public virtual void AddEvent(EventDto e)
        {
            throw new NotImplementedException();
        }

        public virtual Provider Initialize()
        {
            throw new NotImplementedException();
        }

        public virtual List<EventDto> GetEvents()
        {
            throw new NotImplementedException();
        }
    }
}