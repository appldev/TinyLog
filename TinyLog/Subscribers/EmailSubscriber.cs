using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TinyLog.Subscribers
{
    public class EmailSubscriber : LogSubscriber
    {
        public EmailSubscriber(string host, int port, bool ssl, string userName, string password, string domain, string fromAddress, string toAddress, string subjectPrefix = null, LogEntryFilter filter = null)
            : base(filter)
        {
            _Host = host;
            _Port = port;
            _Ssl = ssl;
            _UserName = userName;
            _Password = password;
            _Domain = domain;
            _FromAddress = fromAddress;
            _ToAddress = toAddress;
            _SubjectPrefix = subjectPrefix;
        }

        private string _Host;
        private int _Port;
        private bool _Ssl;
        private string _UserName;
        private string _Password;
        private string _Domain;
        private string _FromAddress;
        private string _ToAddress;
        private string _SubjectPrefix;
        

        public override bool TryInitialize(out Exception initializeException)
        {
            initializeException = null;
            try
            {
                using (SmtpClient client = GetMailClient())
                {
                    client.Send(_FromAddress, _ToAddress, _SubjectPrefix ?? "TinyLog Log Subscriber initialized", "This is a confirmation that the TinyLog Log Provider has been initialized properly");
                }
                return true;
            }
            catch (Exception ex)
            {
                initializeException = ex;
                return false;
            }
        }

        public override Task ReceiveAsync(LogEntry logEntry, bool Created)
        {
            using (SmtpClient client = GetMailClient())
            {
                client.Send(_FromAddress, _ToAddress, (!string.IsNullOrEmpty(_SubjectPrefix) ? _SubjectPrefix + ": " : "") + logEntry.Title + (!Created ? " (This entry was not successfully created)" : ""), GetMailBody(logEntry));
            }
            return Task.FromResult<object>(null);
        }

        public override void Receive(LogEntry logEntry, bool Created)
        {
            using (SmtpClient client = GetMailClient())
            {
                client.Send(GetMessage(logEntry, Created));
            }
        }




        #region Private helper methods

        private MailMessage GetMessage(LogEntry logEntry, bool Created)
        {
            MailMessage msg = new MailMessage(_FromAddress, _ToAddress);
            msg.IsBodyHtml = false;
            msg.Priority = logEntry.Severity == LogEntrySeverity.Error || logEntry.Severity == LogEntrySeverity.Critical || !Created ? MailPriority.High : MailPriority.Normal;

            msg.Subject = (!string.IsNullOrEmpty(_SubjectPrefix) ? _SubjectPrefix + ": " : "") + logEntry.Title + (!Created ? " (This entry was not successfully created)" : "");
            msg.Body = GetMailBody(logEntry);
            return msg;
        }

        private SmtpClient GetMailClient()
        {
            SmtpClient client = new SmtpClient(_Host, _Port);
            client.EnableSsl = _Ssl;
            if (string.IsNullOrEmpty(_UserName))
            {
                client.UseDefaultCredentials = true;
            }
            else
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_UserName, _Password, _Domain);
            }
            return client;
        }

        private string GetMailBody(LogEntry logEntry)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Id: {0}\r\n", logEntry.Id);
            sb.AppendFormat("Date: {0}\r\n", logEntry.CreatedOnString);
            sb.AppendFormat("Severity: {0}\r\n", logEntry.SeverityString);
            sb.AppendFormat("Source: {0}\r\n", logEntry.Source);
            sb.AppendFormat("Area: {0}\r\n", logEntry.Area);
            sb.AppendFormat("Client: {0}\r\n", logEntry.Client);
            sb.AppendFormat("Client info: {0}\r\n\r\n", logEntry.ClientInfo);
            sb.AppendFormat("Message:\r\n{0}\r\n\r\n", logEntry.Message);
            sb.AppendFormat("Data:\r\n{0}\r\n", logEntry.CustomData);
            return sb.ToString();
        }

        #endregion  
    }
}
