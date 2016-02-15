using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyLog.Writers
{
    public class EventLogwriter : LogWriter
    {
        private string _log = "Application";
        private string _source;
        public EventLogwriter(string eventSource, string eventLog = "Application", LogEntryFilter filter = null)
        {
            if (string.IsNullOrEmpty(eventSource))
            {
                throw new ArgumentNullException("eventSource");
            }
            _source = eventSource;
            if (string.IsNullOrEmpty(eventLog))
            {
                throw new ArgumentNullException("eventLog");
            }
            _log = eventLog;
            if (filter != null)
            {
                Filter = filter;
            }
        }
        public override bool TryInitialize(out Exception initializeException)
        {
            initializeException = null;
            try
            {
                if (!EventLog.SourceExists(_source))
                {
                    EventLog.CreateEventSource(_source, _log);
                }
                EventLog.WriteEntry(_source, string.Format("The EventLog writer '{0}' was initialized", this.GetType().FullName), EventLogEntryType.Information);
                return true;
            }
            catch (Exception ex)
            {
                initializeException = new InvalidOperationException("The EventLog writer could not be initialized for the source '{0}' in the log '{1}'. This is probably due to missing credentials. See the inner exception for details.", ex);
                return false;
            }
        }

        public override bool TryWriteLogEntry(LogEntry logEntry, out Exception writeException)
        {
            writeException = null;
            try
            {
                switch (logEntry.Severity)
                {
                    case LogEntrySeverity.Error:
                    case LogEntrySeverity.Critical:
                        EventLog.WriteEntry(_source, FormateLogMessage(logEntry), EventLogEntryType.Error);
                        break;
                    case LogEntrySeverity.Warning:
                        EventLog.WriteEntry(_source, FormateLogMessage(logEntry), EventLogEntryType.Warning);
                        break;
                    case LogEntrySeverity.Information:
                    case LogEntrySeverity.Verbose:
                        EventLog.WriteEntry(_source, FormateLogMessage(logEntry), EventLogEntryType.Information);
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("The severity type '{0}' is not supported", logEntry.SeverityString));
                }
                return true;
            }
            catch (Exception ex)
            {
                writeException = ex;
                return false;
            }
        }

        public override Task<Tuple<bool, Exception>> TryWriteLogEntryAsync(LogEntry logEntry)
        {
            Exception writeException;
            bool b = TryWriteLogEntry(logEntry, out writeException);
            return Task.FromResult<Tuple<bool, Exception>>(new Tuple<bool, Exception>(b, writeException));
        }

        private string FormateLogMessage(LogEntry logEntry)
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
    }
}
