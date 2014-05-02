using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace OutlookConsoleImporter
{
    public class WebClientEx : WebClient
    {
        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest w = base.GetWebRequest(address);
            w.Timeout = Timeout;
            return w;
        }
    }
}
