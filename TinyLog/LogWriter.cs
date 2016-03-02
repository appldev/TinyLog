using System;
using System.Threading.Tasks;

namespace TinyLog
{
    /// <summary>
    /// The log writer writes log entries to a backend storage
    /// </summary>
    public abstract class LogWriter
    {
        /// <summary>
        /// TryInitialize is called immediately after adding a logwriter to the provider. It should check the backend storage
        /// and return true if the logwriter is valid for use. Only writers that return true is added to the provider
        /// </summary>
        /// <param name="initializeException">If TryInitialize returns false, initializeException will contain the cause or error information</param>
        /// <returns>true if the Log Writer is ready for use, otherwise false</returns>
        public abstract bool TryInitialize(out Exception initializeException);

        /// <summary>
        /// Writes a log entry to the backend storage
        /// </summary>
        /// <param name="logEntry">The logentry to write</param>
        /// <param name="writeException">If the writer returns false, the writeException contains the error information</param>
        /// <returns>returns true if the log was sucessfully committed to the backend storage</returns>
        public abstract bool TryWriteLogEntry(LogEntry logEntry, out Exception writeException);

        /// <summary>
        /// Writes a log entry to the backend storage
        /// </summary>
        /// <param name="logEntry">The logentry to write</param>
        /// <param name="writeException">If the writer returns false, the writeException contains the error information</param>
        /// <returns>returns true if the log was sucessfully committed to the backend storage</returns>
        public abstract Task<Tuple<bool, Exception>> TryWriteLogEntryAsync(LogEntry logEntry);

        private LogEntryFilter _filter = LogEntryFilter.Create();

        /// <summary>
        /// Contains the filter for the LogWriter. Only Log entries matching the filter will be written using the LogWriter.
        /// The default filter is set to write all log entries.
        /// </summary>
        public LogEntryFilter Filter
        {
            get
            {
                return _filter;
            }

            set
            {
                _filter = value;
            }
        }
    }
}
