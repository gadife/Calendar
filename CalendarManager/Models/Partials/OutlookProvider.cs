using EventsImporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CalendarManager.Helpers;

namespace CalendarManager.Models
{
    public partial class OutlookProvider
    {
        private OutlookImporter m_importer;

        public override void AddEvent(EventDto e)
        {
            Logger.Debug("Start sending ICS of {0} to {1}", e.Subject, m_importer.Mail);
            m_importer.SendEvent(e);
            Logger.Debug("Finish sending ICS of {0} to {1}", e.Subject, m_importer.Mail);
        }

        public override Provider Initialize()
        {
            Logger.Debug("Start initializing Outlook Provider");
            m_importer = new OutlookImporter(this.User.Email);
            return this;
        }
    }
}