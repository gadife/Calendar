using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CalendarManager.Models
{
    public class ReturnUser
    {
        public ReturnUser(string mail,string name)
        {
            Email = mail;
            Name = name;
        }

        public string Email { get; set; }
        public string Name { get; set; }
    }
}