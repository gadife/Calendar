using DDay.iCal;
using DDay.iCal.Serialization.iCalendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace EventsImporter
{
    public class EventImporter
    {
        #region Private Members

        private string m_mail;

        #endregion Private Members

        #region Ctor

        public EventImporter(string email)
        {
            m_mail = email;
        }

        #endregion Ctor

        #region Public Methods

        public virtual IEnumerable<EventDto> GetEvents(bool shouldTakeAllParticipents = false)
        {
            throw new NotImplementedException();
        }
        public virtual void SendEvent(EventDto e, string subject = null)
        {
            SendAppitment(CreateIcsData(e), subject ?? string.Format("You have inveted to {0}", e.Name));
        }

        #endregion Public Methods

        #region Properties

        public virtual string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Mail
        {
            get { return m_mail; }
        }

        #endregion Properties

        #region Private Methods

        /// <summary>
        /// Remove to another class ( for testing )
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private string CreateIcsData(EventDto dto)
        {
            iCalendar ics = new iCalendar();
            ics.Method = "REQUEST";

            Event ev = ics.Create<Event>();
            ev.Start = new iCalDateTime(dto.StartTime);
            ev.End = new iCalDateTime(dto.EndTime);
            ev.Name = dto.Name;
            ev.UID = Guid.NewGuid().ToString();
            ev.Description = "Created by the site";
            ev.Location = dto.Location;
            ev.Organizer = new Organizer(dto.Organizer);
            ev.Attendees = dto.Attending.Select<Attending, IAttendee>(a => new Attendee(a.Email)).ToList();

            iCalendarSerializer seralizer = new iCalendarSerializer();
            return seralizer.SerializeToString(ics);
        }

        private void SendAppitment(string data, string subject)
        {

            System.Net.Mail.Attachment meeting = System.Net.Mail.Attachment.CreateAttachmentFromString(data, new ContentType("text/calendar"));

            using (MailMessage mail = new MailMessage("ericfeldman93@gmail.com", m_mail))
            {
                mail.Subject = subject;
                mail.Attachments.Add(meeting);
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("ericfeldman93@gmail.com", "ericfeldman789798")
                };

                smtp.Send(mail);
            }
        }

        #endregion Private Methods
    }
}
