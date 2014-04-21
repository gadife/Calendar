using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace CalendarManager.Helpers
{
    public static class LocationHelper
    {
        #region Private Members

        private static readonly string m_apiAddress = @"http://maps.google.com/maps/api/geocode/json?address={0}&sensor=false";

        #endregion Private Members

        #region Public Methods

        public static string GetFormmatedLocation(string location)
        {
            location = location.Replace(",", "");

            //string url = string.Format(m_apiAddress, string.Join("+", location.Split(' ')));
            string url = string.Format(m_apiAddress, location);

            WebRequest request = HttpWebRequest.Create(url);
            HttpWebResponse respoonse = request.GetResponse() as HttpWebResponse;

            StreamReader loResponseStream = new StreamReader(respoonse.GetResponseStream());
            string body = loResponseStream.ReadToEnd();

            return GetLocationFromResult(body);
        }

        #endregion Public Methods

        #region Private Methods

        private static string GetLocationFromResult(string body)
        {
            JObject obj = JObject.Parse(body);
            string result = (string)(obj["status"]);

            if (result == "OK")
            {
                //string city = (string)obj["results"][0]["address_components"].First(ac => ac["types"].Any(t => (string)(t) == "locality"))["long_name"];
                string city = GetProperyOfAddress(obj, "locality");
                string country = GetProperyOfAddress(obj, "country");

                return string.Format("{0}, {1}", city, country);
            }
            else
            {
                return "Address not found";
            }
        }

        private static string GetProperyOfAddress(JObject obj, string propName)
        {
            var typeElement = obj["results"][0]["address_components"].FirstOrDefault(ac => ac["types"].Any(t => (string)(t) == propName));

            if (typeElement == null)
            {
                return "";
            }

            return typeElement["long_name"].ToString();

        }

        #endregion Private Methods
    }
}