using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsImporter
{
    public class ImporterManager
    {
        #region Private Members

        private List<EventImporter> m_importers;

        #endregion Private Members

        #region Ctor

        public ImporterManager()
        {
            m_importers = new List<EventImporter>();
        }

        #endregion Ctor

        #region Add Importers

        public void AddGmailAccount(string email, string password)
        {
            m_importers.Add(new GmailImporter(email, password));
        }
        public void AddIsraelHolydays()
        {
            m_importers.Add(new IsarelCalendarImporter());
        }
        public void AddOutlookAccount()
        {
        }

        #endregion Add Importers

        #region Public Methods

        public IEnumerable<EventDto> GetAllMeetings()
        {
            List<EventDto> meetings = new List<EventDto>();

            foreach (var item in m_importers) // TODO : parllel
            {
                meetings.AddRange(item.GetEvents()) ;
            }

            return meetings;
        }

        public IEnumerable<EventDto> GetMeetingsOfImporter(params string[] importerNames)
        {
            List<EventDto> meetings = new List<EventDto>();

            foreach (var item in m_importers.Where(m=>importerNames.Contains(m.Name))) // TODO : parllel
            {
                meetings.AddRange(item.GetEvents());
            }

            return meetings;
        }

        public IEnumerable<string> GetAllActiveImporters()
        {
            return m_importers.Select(m => m.Name);
        }

        #endregion Public Methods
    }
}
