using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace EventsImporter
{
    public class OutlookImporter : EventImporter
    {
        #region Private Members

        private static readonly string IMPORTER_NAME = "Outlook";
        private Recipient m_userMail;

        #endregion Private Members

        #region Ctor

        public OutlookImporter(string email):base(email)
        {
              
        }

        #endregion Ctor

        #region Public Methods

        public override IEnumerable<EventDto> GetEvents(bool shouldTakeAllParticipents = false)
        {
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now.AddMonths(1);

            string filter = string.Format("[Start] >= '{0}' AND [End] <= '{1}'",
                            start.ToString("g"),
                            end.ToString("g"));
            Application oApp = null;
            NameSpace mapiNamespace = null;
            MAPIFolder CalendarFolder = null;
            Items outlookCalendarItems = null;
            bool outlookExists = true;

            try
            {
                oApp = new Application();
            }
            catch (COMException e)
            {
                outlookExists = false;
                Console.WriteLine("Could not find Outlook");
                Console.WriteLine(e.Message);
            }

            if (outlookExists)
            {
                m_userMail = oApp.Session.CurrentUser;
                Console.WriteLine("This is the new version 21.4!!!");
                Console.WriteLine(GetMailFormRecipients(m_userMail));

                mapiNamespace = oApp.GetNamespace("MAPI");
                CalendarFolder = mapiNamespace.GetDefaultFolder(OlDefaultFolders.olFolderCalendar);
                outlookCalendarItems = CalendarFolder.Items;
                outlookCalendarItems.IncludeRecurrences = true;
                outlookCalendarItems.Sort("[Start]", Type.Missing);
                outlookCalendarItems = outlookCalendarItems.Restrict(filter);

                foreach (AppointmentItem item in outlookCalendarItems)
                {
                    EventDto e = null;
                    //if (item.IsRecurring)
                    //{
                    //    RecurrencePattern rp = item.GetRecurrencePattern();
                    //    AppointmentItem recur = null;

                    //    for (DateTime cur = start; cur <= end; cur = cur.AddDays(1))
                    //    {
                    //        try
                    //        {
                    //            recur = rp.GetOccurrence(cur);
                    //            if (ShouldFilter(item))
                    //            {
                    //                continue;
                    //            }

                    //            e = FillData(recur);
                    //        }
                    //        catch
                    //        { continue; }
                    //    }
                    //}
                    //else
                    //{
                    if (ShouldFilter(item))
                    {
                        continue;
                    }

                    e = FillData(item, shouldTakeAllParticipents);
                    //}

                    if (e != null) // try to remove
                    {
                        yield return e;
                    }
                }
            }
        }

        public IEnumerable<EventDto> GetEventWithoutOtherParticipents()
        {
            return GetEvents(true);
        }

        #endregion Public Methods

        #region Private Methods

        private EventDto FillData(AppointmentItem app, bool shouldTakeAllParticipents)
        {
            EventDto e = new EventDto() { ImporterName = IMPORTER_NAME };

            e.Location = app.Location;
            e.Name = app.Subject;
            e.StartTime = app.Start.ToUniversalTime();
            e.EndTime = app.End.ToUniversalTime();
            e.IsAllDay = app.AllDayEvent;
            e.Id = app.GlobalAppointmentID;
            e.Attending = GetAllRecipients(app.Recipients, shouldTakeAllParticipents);
            e.Organizer = GetMeetingOrganizerEmail(app);
            e.Subject = app.Subject;
            e.IsReccurnce = app.RecurrenceState == OlRecurrenceState.olApptOccurrence;

            return e;
        }

        private bool ShouldFilter(AppointmentItem app)
        {
            return app.BusyStatus == OlBusyStatus.olFree;
        }

        private List<Attending> GetAllRecipients(Recipients recipients,bool shouldTakeAllParticipents)
        {
            if (shouldTakeAllParticipents)
            {
                HashSet<Attending> attendings = new HashSet<Attending>();
                for (int i = 1; i <= recipients.Count; i++)
                {
                    attendings.Add(RecipientsToAttending(recipients[i]));
                }
                attendings.Add(RecipientsToAttending(m_userMail));

                return attendings.ToList();
            }
            else
            {
                // i'm adding the current user by myslef , so the current user will be that last one
                return new List<Attending>() { RecipientsToAttending(m_userMail) }; 
            }
        }

        private string GetMeetingOrganizerEmail(AppointmentItem app)
        {
            //string PR_SENT_REPRESENTING_ENTRYID =
            //    @"http://schemas.microsoft.com/mapi/proptag/0x00410102";
            //string organizerEntryID =
            //    app.PropertyAccessor.BinaryToString(
            //        app.PropertyAccessor.GetProperty(
            //        PR_SENT_REPRESENTING_ENTRYID));

            //AddressEntry organizer = app.Session.GetAddressEntryFromID(organizerEntryID);
            //if (organizer != null)
            //{
            //    return organizer.Address;
            //}
            //else
            //{
            //    return null;
            //}
            return "Soon";
        }

        private Attending RecipientsToAttending(Recipient recipient)
        {
            return new Attending(GetMailFormRecipients(recipient),
                                                 recipient.MeetingResponseStatus == OlResponseStatus.olResponseAccepted);
        }

        private static string GetMailFormRecipients(Recipient rec)
        {
            string userMail = string.Empty;
            try
            {
                var exchangeUser = rec.AddressEntry.GetExchangeUser();
                userMail = exchangeUser == null ? rec.Address : exchangeUser.PrimarySmtpAddress;
            }
            catch (COMException)
            {
                userMail = rec.AddressEntry.Address;
            }

            return userMail;
        }

        #endregion Private Methods

        #region Proprties

        public override string Name { get { return IMPORTER_NAME; } }

        #endregion Proprties
    }
}
