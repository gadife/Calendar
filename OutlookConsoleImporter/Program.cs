using EventsImporter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlookConsoleImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            EventSender sender = new EventSender();

            OutlookImporter importer = new OutlookImporter("not importent");

            PrintMessage("Start fetching events from Outlook");
            var events = importer.GetEvents().ToList();
            PrintMessage(string.Format("Finish fetching events from Outlook ({0} topical events)", events.Count));

            long userId = long.Parse(ConfigurationManager.AppSettings["userId"]);
            string password = ConfigurationManager.AppSettings["userPassword"];

            PrintMessage("Sending meetings to server");
            sender.Send(events, userId, password);
            PrintMessage("Finish sending meetings to server");

            PrintMessage("Done");
            //Console.ReadLine();
        }

        private static void PrintMessage(string message)
        {
            Console.WriteLine("---------");
            Console.WriteLine(message);
            Console.WriteLine("---------");
            Console.WriteLine("");
        }
    }
}
