using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsImporter
{
    public class EventDto
    {
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ImporterName { get; set; }
        public bool IsAllDay { get; set; }
        public string Location { get; set; }
        public List<Attending> Attending { get; set; }
        public string Organizer { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public bool IsReccurnce { get; set; }

        public string Id { get; set; }
    }
}
