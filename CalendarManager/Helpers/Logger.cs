using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace CalendarManager.Helpers
{
    public static class Logger
    {
        private static readonly ILog m_log = LogManager.GetLogger(typeof(Logger));

        // if this is where ruby i have just use missing_method and call the method of the log instance

        public static void Error(Exception exception, string message, params object[] param)
        {
            m_log.Error(string.Format(message,param) , exception);
        }

        public static void Info(string message)
        {
            m_log.Info(message);
        }

        public static void Info(string message, params object[] param)
        {
            m_log.Info(string.Format(message, param));
        }

        public static void Debug(string message)
        {
            m_log.Debug(message);
        }

        public static void Debug(string message, params object[] param)
        {
            m_log.DebugFormat(message,param);
        } 
    }
}