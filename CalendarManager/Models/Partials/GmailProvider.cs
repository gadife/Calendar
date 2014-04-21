using EventsImporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CalendarManager.Helpers;

namespace CalendarManager.Models
{
    public partial class GmailProvider
    {
        private GmailImporter m_importer;

        public override void AddEvent(EventDto e)
        {
            m_importer.SendEvent(e);
        }

        public override Provider Initialize()
        {
            m_importer = new GmailImporter(this.UserName, this.Password);
            return this;
        }

        public override List<EventDto> GetEvents()
        {
            return m_importer.GetEvents().ToList();
        }
    }
}