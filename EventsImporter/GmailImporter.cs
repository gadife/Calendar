using Google.GData.Calendar;
using Google.GData.Client;
using Google.GData.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsImporter
{
    public class GmailImporter : EventImporter
    {
        #region Private Members

        private static readonly string importerName = "Gmail";
        private string m_username, m_password;
        private CalendarService m_service;
        private Uri m_calendarUri;

        #endregion Private Members

        #region Ctor

        public GmailImporter(string username, string password):base(username)
        {
            m_username = username;
            m_password = password;

            m_service = new CalendarService("AAA");
            m_service.setUserCredentials(m_username, m_password);

            m_calendarUri = new Uri("http://www.google.com/calendar/feeds/" + m_service.Credentials.Username + "/private/full");
        }
        
        #endregion Ctor

        #region IEventImporter

        public override IEnumerable<EventDto> GetEvents(bool shouldTakeAllParticipents = false)
        {
            var newEvents = GetAllGmailEvents().Where(e=>e.Times.First().EndTime >= DateTime.Now);

            foreach (var e in newEvents)
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

        public override void SendEvent(EventDto e,string subject = null)
        {
            AtomEntry entry = m_service.Insert(m_calendarUri, DtoToEntry(e));
        }

        #endregion IEventImporter

        #region Proprties

        public override string Name { get { return importerName; } }

        #endregion Proprties

        #region Private Methods

        private IEnumerable<EventEntry> GetAllGmailEvents()
        {
            // Create the query object:
            EventQuery query = new EventQuery();
            query.Uri = m_calendarUri;
            //if (startData != null)
            //    query.StartTime = startData.Value;

            // Tell the service to query:
            EventFeed calFeed = m_service.Query(query);
            return calFeed.Entries.Cast<EventEntry>();
        }

        private EventEntry DtoToEntry(EventDto dto)
        {
            EventEntry entry = new EventEntry();

            // Event Info
            entry.Title.Text = dto.Name;
            entry.Content.Content = dto.Description;
            entry.Content.Type = "html";
            //entry.Uid = new GCalUid(dto.Id);

            if (!string.IsNullOrWhiteSpace(dto.Location))
            {
                Where location = new Where();
                location.ValueString = dto.Location;
                entry.Locations.Add(location); 
            }
           
            // Event time
            When eventTime = new When();
            eventTime.StartTime = dto.StartTime;
            eventTime.EndTime = dto.EndTime;
            eventTime.AllDay = dto.IsAllDay;
            entry.Times.Add(eventTime);

            // Add Users
            foreach (var attending in dto.Attending)
            {
                entry.Participants.Add(ConvertToWho(attending));
            }

            return entry;
        }

        private Who ConvertToWho(Attending attending)
        {
            Who praticipent = new Who();
            praticipent.ValueString = attending.Email;
            praticipent.Rel = Who.RelType.EVENT_ATTENDEE;

            Who.AttendeeType type = new Who.AttendeeType();
            type.Value = Who.AttendeeType.EVENT_OPTIONAL;
            praticipent.Attendee_Type = type;

            Who.AttendeeStatus staus = new Who.AttendeeStatus();
            staus.Value = attending.Approved ? Who.AttendeeStatus.EVENT_ACCEPTED : Who.AttendeeStatus.EVENT_INVITED;
            praticipent.Attendee_Status = staus;

            return praticipent;
        }

        #endregion Private Methods
    }
}

