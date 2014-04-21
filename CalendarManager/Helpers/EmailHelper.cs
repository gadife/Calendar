using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace CalendarManager.Helpers
{
    public class EmailHelper
    {
        #region Private Members

        private static string m_email;
        private static string m_password;
        private static string m_smtp;
        private static int? m_smtpPort;

        #endregion Private Members

        #region Ctor

        static EmailHelper()
        {
            m_email = ConfigurationManager.AppSettings["sendEmail"];
            m_password = ConfigurationManager.AppSettings["sendPassword"];
            m_smtp = ConfigurationManager.AppSettings["smtp"];

            string portString = ConfigurationManager.AppSettings["smtpPort"];
            if (!string.IsNullOrWhiteSpace(portString))
            {
                m_smtpPort = int.Parse(portString);
            }
        }
        
        #endregion Ctor

        #region Public Methods

        public static void SendMail(string to, string subject, string content)
        {
            using (MailMessage mail = new MailMessage(m_email, to))
            {
                mail.Subject = subject;
                mail.Body = content;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient
                {
                    Host = m_smtp,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(m_email, m_password)
                };
                if (m_smtpPort != null)
                {
                    smtp.Port = m_smtpPort.Value;
                }

                smtp.Send(mail);
            }
        }

        #endregion Public Methods
    }
}