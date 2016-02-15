using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace TinyLog.Writers
{
    public class TraceLogWriter : LogWriter
    {
        public TraceLogWriter(LogEntryFilter filter = null)
        {
            if (filter != null)
            {
                Filter = filter;
            }
        }
        public override bool TryInitialize(out Exception initializeException)
        {
            initializeException = null;
            System.Diagnostics.Trace.AutoFlush = true;
            System.Diagnostics.Trace.TraceInformation(string.Format("The Trace writer '{0}' was initialized", this.GetType().FullName));
            return true;
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
                        Trace.TraceError(FormateLogMessage(logEntry));
                        break;
                    case LogEntrySeverity.Warning:
                        Trace.TraceWarning(FormateLogMessage(logEntry));
                        break;
                    case LogEntrySeverity.Information:
                    case LogEntrySeverity.Verbose:
                        Trace.TraceInformation(FormateLogMessage(logEntry));
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
