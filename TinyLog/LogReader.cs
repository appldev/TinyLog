using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyLog
{
    public abstract class LogReader
    {
        /// <summary>
        /// TryInitialize is called immediately after adding a LogReader to the provider. It should check the backend storage
        /// and return true if the LogReader is valid for use. Only readers that return true is added to the provider
        /// </summary>
        /// <param name="initializeException">If TryInitialize returns false, initializeException will contain the cause or error information</param>
        /// <returns>true if the Log Reader is ready for use, otherwise false</returns>
        public abstract bool TryInitialize(out Exception initializeException);


        internal IEnumerable<LogEntry> ReadByFilter(LogEntryFilter filter, int maxLogEntries = 100)
        {
            return ByFilter(filter, maxLogEntries);
        }

        internal Task<IEnumerable<LogEntry>> ReadByFilterAsync(LogEntryFilter filter, int maxLogEntries = 100)
        {
            return ByFilterAsync(filter, maxLogEntries);
        }

        internal LogEntry ReadById(Guid Id)
        {
            return ById(Id);
        }
        
        
        internal Task<LogEntry> ReadByIdAsync(Guid Id)
        {
            return ByIdAsync(Id);
        }


        /// <summary>
        /// Retrieves log entries by a filter
        /// </summary>
        /// <param name="filter">the filter to use</param>
        /// <param name="maxLogEntries">The number of log entries to return. specify zero or negative number to retrieve as many as possible</param>
        /// <returns>A list of log entries</returns>
        protected abstract IEnumerable<LogEntry> ByFilter(LogEntryFilter filter, int maxLogEntries = 100);

        /// <summary>
        /// Retrieves log entries by a filter as an async operation
        /// </summary>
        /// <param name="filter">the filter to use</param>
        /// <param name="maxLogEntries">The number of log entries to return. specify zero or negative number to retrieve as many as possible</param>
        /// <returns>A list of log entries</returns>
        protected abstract Task<IEnumerable<LogEntry>> ByFilterAsync(LogEntryFilter filter, int maxLogEntries = 100);

        /// <summary>
        /// Reads one log entry
        /// </summary>
        /// <param name="Id">The unique id</param>
        /// <returns>The log entry or null</returns>
        protected abstract LogEntry ById(Guid Id);
        /// <summary>
        /// Reads one log entry
        /// </summary>
        /// <param name="Id">The unique id</param>
        /// <returns>The log entry or null</returns>
        protected abstract Task<LogEntry> ByIdAsync(Guid Id);


    }
}
