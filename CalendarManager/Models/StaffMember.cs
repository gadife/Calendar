using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CalendarManager.Models
{
    public enum SocialNetwrok{Facebook,Twitter,LinkedIn};

    public class StaffMember
    {
        public StaffMember()
        {
            SocialNetwroks = new Dictionary<SocialNetwrok, string>();
            Tags = new List<string>();
        }

        public string FullName { get; set; }
        public string Location { get; set; }
        public string PictureFileName { get; set; }

        public string Description { get; set; }
        /// <summary>
        /// Tuple between company name and comapy site url
        /// </summary>
        public Tuple<string,string> Company { get; set; }

        public List<string> Tags{ get; set; }
        public Dictionary<SocialNetwrok,string> SocialNetwroks { get; set; }

        public void AddSocialNetwork(SocialNetwrok network, string url)
        {
            SocialNetwroks.Add(network, url);
        }
    }
}