using System;
using System.Threading.Tasks;

namespace TinyLog
{
    public abstract class LogSubscriber
    {
        public LogSubscriber(LogEntryFilter filter)
        {
            Filter = filter ?? LogEntryFilter.Create();
        }

        private LogEntryFilter _Filter = null;
        /// <summary>
        /// The filter to match LogEntries against, when evaluating if the subscriber should be used
        /// </summary>
        public LogEntryFilter Filter
        {
            get
            {
                return _Filter;
            }
            set
            {
                _Filter = value;
            }
        }


        /// <summary>
        /// This method is called when the subscriber is registered
        /// </summary>
        /// <param name="initializeException">contains error information if the subscriber could not be initialized</param>
        /// <returns>true if the subscriber is valid for use</returns>
        public abstract bool TryInitialize(out Exception initializeException);

        /// <summary>
        /// Called when a log entry should be received
        /// </summary>
        /// <param name="logEntry">The LogEntry</param>
        /// <param name="Created">true if the log was submitted succesfully by all log writers, otherwise false.</param>
        public abstract Task ReceiveAsync(LogEntry logEntry, bool Created);
        /// <summary>
        /// Called when a log entry should be received
        /// </summary>
        /// <param name="logEntry">The LogEntry</param>
        /// <param name="Created">true if the log was submitted succesfully by all log writers, otherwise false.</param>
        public abstract void Receive(LogEntry logEntry, bool Created);
    }
}
