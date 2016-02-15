using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyLog.Writers
{
    /// <summary>
    /// A log writer that outputs to the attached debugger. Mainly used in development environments
    /// </summary>
    public class DebuggerLogWriter : LogWriter
    {
        public DebuggerLogWriter(LogEntryFilter filter = null)
        {
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
                if (!Debugger.IsAttached || !Debugger.IsLogging())
                {
                    initializeException = new InvalidOperationException("There is no debugger attached or the attached debugger is not enabled for logging");
                    return false;
                }
                Debug.WriteLine(string.Format("The LogWriter '{0}' was initialized", this.GetType().FullName), Enum.GetName(typeof(LogEntrySeverity), LogEntrySeverity.Verbose));
                return true;
            }
            catch (Exception ex)
            {
                initializeException = ex;
                return false;
            }
            
        }

        public override bool TryWriteLogEntry(LogEntry logEntry, out Exception writeException)
        {
            writeException = null;
            try
            {
                Debug.WriteLine(FormateLogMessage(logEntry), logEntry.SeverityString);
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
