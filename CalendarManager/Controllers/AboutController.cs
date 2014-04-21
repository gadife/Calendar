using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CalendarManager.Models;
using CalendarManager.Helpers;

namespace CalendarManager.Controllers
{
    public class AboutController : Controller
    {
        public ActionResult About()
        {
            // Maybe it will be a static property and in that case ther wont be the need to create each call a new instance
            // if in the future the staff members will be in DB it be better to create a static class that loades all the 
            // staff memebers to a static list 

            #region Gadi

            StaffMember gadi = new StaffMember()
            {
                FullName = "Gadi Feldman",
                Company = new Tuple<string, string>("Citrix", @"http://www.citrix.com"),
                Description = "CEO and Lead Systems Engineer",
                Tags = new List<string>() { "Citrix", "Father", "Virtualizton", "CEO" },
                PictureFileName = "gadi.jpg",
                Location = "Israel",
            };
            gadi.AddSocialNetwork(SocialNetwrok.Facebook, @"https://www.facebook.com/gadifeldman?fref=ts");
            gadi.AddSocialNetwork(SocialNetwrok.Twitter, @"https://twitter.com/gadi_fe");
            gadi.AddSocialNetwork(SocialNetwrok.LinkedIn, @"http://www.linkedin.com/profile/view?id=11334148");
            
            #endregion

            #region Eric

            StaffMember eric = new StaffMember()
            {
                FullName = "Eric Feldman",
                Description = "Software Developer",
                Tags = new List<string>() { "Tag1", "Tag2", "Tag3" },
                PictureFileName = "eric.jpg",
                Location = "Israel"
            };
            eric.AddSocialNetwork(SocialNetwrok.Facebook, @"https://www.facebook.com/eric.feldman.93");
            eric.AddSocialNetwork(SocialNetwrok.Twitter, @"https://twitter.com/ericfel");
            eric.AddSocialNetwork(SocialNetwrok.LinkedIn, @"http://www.linkedin.com/profile/view?id=311990834");
            
            #endregion

            ViewBag.Layout = GetLayout();

            return View(new List<StaffMember>() { gadi,eric });
        }

        public ActionResult Homepage()
        {
            ViewBag.Layout = GetLayout();
            return View();
        }

        private string GetLayout()
        {
            return Session[SessionNames.Email] == null ? "_Notlogin" : "_Layout"; 
        }
    }
}
