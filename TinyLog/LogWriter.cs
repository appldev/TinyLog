using System;
using System.Collections.Generic;
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
        /// Writes log entries as a batch. If this method is not overridden in a derived class, the log entries will be written using the WriteLogEntry() method
        /// </summary>
        /// <param name="logEntries">The log entries to write</param>
        /// <param name="writeExceptions">An aggregate exception with one inner exception for each error encountered</param>
        /// <returns>returns true if all log entries in the batch was written succesfully, otherwise false</returns>
        public virtual bool TryWriteLogEntryBatch(LogEntry[] logEntries, out AggregateException writeExceptions)
        {
            List<Exception> exceptions = new List<Exception>();
            foreach (LogEntry logEntry in logEntries)
            {
                Exception ex = null;
                if (!TryWriteLogEntry(logEntry, out ex))
                {
                    exceptions.Add(ex);    
                }
            }
            if (exceptions.Count > 0)
            {
                writeExceptions = new AggregateException(string.Format("{0} of {1} Log entries not written. See inner exceptions for details.", exceptions.Count, logEntries.Length), exceptions);
                return false;
            }
            writeExceptions = null;
            return true;
        }

        /// <summary>
        /// Writes log entries as a batch.If this method is not overridden in a derived class, the log entries will be written using the WriteLogEntry() method
        /// </summary>
        /// <param name="logEntries">The log entries to write</param>
        /// <returns>returns true if all log entries in the batch was written succesfully, otherwise false</returns>
        public virtual Task<Tuple<bool, AggregateException>> TryWriteLogEntryBatchAsync(LogEntry[] logEntries)
        {
            List<Exception> exceptions = new List<Exception>();
            foreach (LogEntry logEntry in logEntries)
            {
                Exception ex = null;
                if (!TryWriteLogEntry(logEntry, out ex))
                {
                    exceptions.Add(ex);
                }
            }
            if (exceptions.Count > 0)
            {
                AggregateException writeExceptions = new AggregateException(string.Format("{0} of {1} Log entries not written. See inner exceptions for details.", exceptions.Count, logEntries.Length), exceptions);
                return Task.FromResult<Tuple<bool, AggregateException>>(new Tuple<bool, AggregateException>(false, writeExceptions));
            }
            return Task.FromResult<Tuple<bool, AggregateException>>(new Tuple<bool, AggregateException>(true, null));
        }


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
