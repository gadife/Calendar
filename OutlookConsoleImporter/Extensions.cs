using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OutlookConsoleImporter
{
    public static class Extensions
    {
        public static void AddPostParamter(this HttpWebRequest request, string paramter)
        {
            request.Method = "POST";

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(paramter);

            request.ContentLength = data.Length; // Check if it 0 in the begining

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
        }

        public static DateTime ToGMT0(this DateTime date)
        {
            return DateTime.SpecifyKind(date, DateTimeKind.Utc);
        }

        public static string ToGoodString(this DateTime date)
        {
            return date.ToString("yyyy/MM/dd HH:mm");
        }
    }
}
