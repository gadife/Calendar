using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EventsImporter
{
    public class Attending
    {
        public Attending(string email, bool approved)
        {
            Email = email;
            IsApproved = approved;
        }

        public string Email { get; set; }
        public bool IsApproved { get; set; }       
    }
}
