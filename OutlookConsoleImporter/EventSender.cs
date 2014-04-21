using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using EventsImporter;
using System.Configuration;

namespace OutlookConsoleImporter
{
    public class EventSender
    {
        #region Private Memebers

        private static readonly string url = ConfigurationManager.AppSettings["InserUrl"];

        #endregion Private Memebers

        public void Send(EventsImporter.EventDto e, long userId, string password)
        {
            //using (var wb = new WebClient())
            //{
            //    //TODO : JSON
            //    var data = new NameValueCollection();
            //    data["userId"]  = userId.ToString() ;
            //    data["isAllDay"] = e.IsAllDay.ToString();
            //    data["hashedPassword"] = password.ToString();
            //    data["startDate"] = e.StartTime.ToGoodString();
            //    data["endDate"] = e.EndTime.ToGoodString();
            //    data["location"] = e.Location;
            //    data["eventId"] = e.Id;
            //    data["importerName"] = "Outlook";
            //    data["eventName"] = e.Name;
            //    data["attendees"] = e.Attending;

            //    wb.UploadValues(url, "POST", data);
            //}
        }

        public void Send(List<EventDto> events, long userId, string password)
        {
            using (var wb = new WebClient())
            {
                //TODO : JSON
                var data = new NameValueCollection();
                data["userId"] = userId.ToString();
                data["hashedPassword"] = password.ToString();
                data["json"] = GetEvetnsJson(events);

                try
                {
                    Console.WriteLine("Sending to {0}",url);
                    wb.UploadValues(url, "POST", data);
                }
                catch (WebException e)
                {
                    var resp = new StreamReader(e.Response.GetResponseStream()).ReadToEnd();

                    Console.WriteLine(resp);
                }
            }

            //var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            //httpWebRequest.ContentType = "text/json";
            //httpWebRequest.Method = "POST";

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    streamWriter.Write(GetJson(events));
            //    streamWriter.Flush();
            //    streamWriter.Close();

            //    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            //    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            //    {
            //        var result = streamReader.ReadToEnd();
            //    }
            //}
        }

        private string GetEvetnsJson(IEnumerable<EventDto> events)
        {
            //JavaScriptSerializer oSerializer = new JavaScriptSerializer();
            //string sJSON = oSerializer.Serialize(events);

            var toSerialize = events.Select(e => new
            {
                EventId = e.Id,
                Location = e.Location,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                IsAllDay = e.IsAllDay,
                EventUsers = e.Attending,
                OrganizerMail = e.Organizer,
                Subject = e.Subject,
                IsReccurnce = e.IsReccurnce
            });

            string sJSON = new JavaScriptSerializer().Serialize(toSerialize);

            return sJSON;
        }

        //public async Task SendAsync(Event e, long userId, string password)
        //{
        //    await Task.Factory.StartNew(() => Send(e, userId, password));
        //}
    }
}
