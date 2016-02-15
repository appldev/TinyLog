using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyLog
{
    /// <summary>
    /// This is the main Log class. All logging is configured and executed from this class.
    /// </summary>
    public class Log
    {
        #region ctor

        private Log()
        {
            _formatters = new ConcurrentBag<LogFormatter>();
            _writers = new ConcurrentBag<LogWriter>();
            _subscribers = new ConcurrentBag<LogSubscriber>();
        }

        private Log(IEnumerable<LogFormatter> logFormatters, IEnumerable<LogWriter> logWriters, IEnumerable<LogSubscriber> subscribers)
        {
            _formatters = new ConcurrentBag<LogFormatter>(logFormatters ?? new List<LogFormatter>());
            _writers = new ConcurrentBag<LogWriter>(logWriters);
            _subscribers = new ConcurrentBag<LogSubscriber>(subscribers ?? new List<LogSubscriber>());
        }

        #endregion

        #region static methods

        private static Log _default;

        /// <summary>
        /// Returns the default Log without any formatters, writers or subscribers preset
        /// formatters, writers and subscribers can be added using the Register methods
        /// </summary>
        public static Log Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new Log();
                }
                return _default;
            }
        }

        /// <summary>
        /// Creates a new log with the specified formatters, writers and subscribers
        /// </summary>
        /// <param name="logWriters">a list of writers to use</param>
        /// <param name="logFormatters">a list of formatters to use</param>
        /// <param name="subscribers">a list of subscribers to use</param>
        /// <returns></returns>
        public static Log Create(IEnumerable<LogWriter> logWriters, IEnumerable<LogFormatter> logFormatters = null, IEnumerable<LogSubscriber> subscribers = null)
        {
            if (logWriters == null)
            {
                throw new ArgumentNullException("logWriters");
            }
            else if (logWriters.Count() == 0)
            {
                throw new ArgumentException("You must specify at least one LogWriter", "logWriters");
            }
            logWriters.ToList().ForEach(writer =>
            {
                Exception ex;
                if (!writer.TryInitialize(out ex))
                {
                    throw new InvalidOperationException("A log writer failed to initialize: {0}",ex);
                }
            });
            if (subscribers != null)
            {
                if (subscribers != null)
                {
                    subscribers.ToList().ForEach(subscriber =>
                    {
                        Exception ex;
                        if (!subscriber.TryInitialize(out ex))
                        {
                            throw new InvalidOperationException("A log subscriber failed to initialize", ex);
                        }
                    });
                }
            }

            return new Log(logFormatters, logWriters, subscribers);
        }

        #endregion

        #region instance methods

        private ConcurrentBag<LogFormatter> _formatters;
        private ConcurrentBag<LogWriter> _writers;
        private ConcurrentBag<LogSubscriber> _subscribers;
        /// <summary>
        /// Registers a log formatter for the log
        /// </summary>
        /// <param name="formatter">a log formatter that implements the LogFormatter class</param>
        public void RegisterLogFormatter(LogFormatter formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }
            _formatters.Add(formatter);
        }

        /// <summary>
        /// Registers a new Log Writer
        /// </summary>
        /// <param name="writer">The LogWriter to register</param>
        public void RegisterLogWriter(LogWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            Exception ex;
            if (!writer.TryInitialize(out ex))
            {
                // TODO: Use an emergency log to log these errors as well?
                throw new InvalidOperationException("The log writer failed to initialize",ex);
            }
            else
            {
                _writers.Add(writer);
            }
        }

        /// <summary>
        /// Registers a new Log Subscriber
        /// </summary>
        /// <param name="subscriber">The LogSubscriber to register</param>
        public void RegisterLogSubscriber(LogSubscriber subscriber)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException("subscriber");
            }
            Exception ex;
            if (!subscriber.TryInitialize(out ex))
            {
                throw new InvalidOperationException("The log subscriber failed to initialize", ex);
            }
            _subscribers.Add(subscriber);
        }

        /// <summary>
        /// Writes a new log entry to the log
        /// </summary>
        /// <param name="logEntry">The LogEntry object to write</param>
        /// <returns>true if the LogEntry was comitted succesfully by all LogWriters</returns>
        public Task<bool> WriteLogEntryAsync(LogEntry logEntry)
        {
            return Task.FromResult<bool>(WriteLogEntry(logEntry));
        }

        /// <summary>
        /// Writes a new log entry to the log
        /// </summary>
        /// <param name="logEntry">The LogEntry object to write</param>
        /// <returns>true if the LogEntry was comitted succesfully by all LogWriters</returns>
        public bool WriteLogEntry(LogEntry logEntry)
        {
            int errors = -1;
            IEnumerable<LogWriter> current = _writers.Where(x => x.Filter.IsMatch(logEntry));
            if (current.Count() > 0)
            {
                errors = 0;
                Parallel.ForEach<LogWriter>(current, async writer =>
                {
                    Tuple<bool, Exception> result = await writer.TryWriteLogEntryAsync(logEntry);
                    if (!result.Item1)
                    {
                        System.Threading.Interlocked.Add(ref errors, 1);
                    }
                });
            }
            
            Parallel.ForEach(_subscribers.Where(x => x.Filter.IsMatch(logEntry)), async subscriber =>
                {
                    await subscriber.ReceiveAsync(logEntry, errors == 0);
                });

            return errors == 0;
        }


        /// <summary>
        /// Writes a new log entry to the log
        /// </summary>
        /// <param name="logEntry">The LogEntry object to write</param>
        /// <returns>true if the LogEntry was comitted succesfully by all LogWriters</returns>
        public bool WriteLogEntry(LogEntry logEntry, object customData)
        {
            return WriteLogEntry<object>(logEntry, customData);
        }

        /// <summary>
        /// Writes a new log entry to the log
        /// </summary>
        /// <param name="logEntry"></param>
        /// <param name="customData"></param>
        /// <returns></returns>
        public Task<bool> WriteLogEntryAsync(LogEntry logEntry, object customData)
        {
            return Task.FromResult<bool>(WriteLogEntry<object>(logEntry, customData));
        }

        /// <summary>
        /// Writes a new log entry to the log
        /// </summary>
        /// <typeparam name="T">The type of custom data to format the log entry with</typeparam>
        /// <param name="logEntry">the LogEntry object to write</param>
        /// <param name="customData">The custom data for the log entry</param>
        /// <returns>true if the LogEntry was comitted succesfully by all LogWriters</returns>
        public Task<bool> WriteLogEntryAsync<T>(LogEntry logEntry, T customData)
        {
            return Task.FromResult<bool>(WriteLogEntry<T>(logEntry, customData));
        }

        /// <summary>
        /// Writes a new log entry to the log
        /// </summary>
        /// <typeparam name="T">The type of custom data to format the log entry with</typeparam>
        /// <param name="logEntry">the LogEntry object to write</param>
        /// <param name="customData">The custom data for the log entry</param>
        /// <returns>true if the LogEntry was comitted succesfully by all LogWriters</returns>
        public bool WriteLogEntry<T>(LogEntry logEntry, T customData)
        {
            LogFormatter formatter = _formatters.FirstOrDefault(x => x.IsValidFormatterFor(customData));
            if (formatter != null)
            {
                formatter.Format(logEntry, customData);
                logEntry.CustomDataFormatter = logEntry.CustomDataFormatter ?? formatter.GetType().FullName;
            }
            else
            {
                logEntry.CustomData = string.Format("No formatter available for:   {0}", customData);
            }
            return WriteLogEntry(logEntry);
        }




        #endregion
    }
}
