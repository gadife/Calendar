using Google.GData.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsImporter
{
    public class IsarelCalendarImporter : EventImporter
    {
        #region Private Members

        private static readonly string importerName = "Israel Holydays";

        #endregion Private Members

        #region Ctor

        public IsarelCalendarImporter() : base(null)
        {

        }
        
        #endregion Ctor

        #region IEventImporter

        public override IEnumerable<EventDto> GetEvents(bool shouldTakeAllParticipents = false)
        {
            CalendarService s = new CalendarService("AAA");

            var events = GetAllGmailEvents(s);

            foreach (var e in events)
            {
                yield return new EventDto()
                {
                    Name = e.Title.Text,
                    StartTime = e.Times.First().StartTime,
                    EndTime = e.Times.First().EndTime,
                    Id = e.Uid.Value,
                    IsAllDay = e.Times.First().AllDay,
                    ImporterName = importerName,
                };
            }
        }

        #endregion IEventImporter

        #region Proprties

        public override string Name { get { return importerName; } }

        #endregion Proprties

        #region Private Methods

        private IEnumerable<EventEntry> GetAllGmailEvents(CalendarService service)
        {
            // Create the query object:
            EventQuery query = new EventQuery();
            query.Uri = new Uri(@"http://www.google.com/calendar/feeds/iw.jewish%23holiday%40group.v.calendar.google.com/public/full");
            //if (startData != null)
            //    query.StartTime = startData.Value;

            // Tell the service to query:
            EventFeed calFeed = service.Query(query);
            return calFeed.Entries.Cast<EventEntry>();
        }

        #endregion Private Methods
    }
}
