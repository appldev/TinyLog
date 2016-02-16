using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyLog
{

    /// <summary>
    /// Settings for how TinyLog should handle exceptions thrown in the core functions and libraries
    /// </summary>
    public enum EmergencyLogSettings
    {
        /// <summary>
        /// Ignore any exceptions
        /// </summary>
        Ignore = 1,
        /// <summary>
        /// Add the exceptions to the emergency log, if available. Otherwise throw the exceptions
        /// </summary>
        AddToEmergencyLog = 2,
        /// <summary>
        /// Throw the exceptions and let the consuming application handle the exceptions
        /// </summary>
        ThrowExceptions = 3
    }


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

        #region static Emergency log

        private static Log _EmergencyLog;
        /// <summary>
        /// Contains the global Emergency log setting that all logs will use. This setting can be overridden in each WriteLog() call
        /// </summary>
        public static EmergencyLogSettings EmergencyLogSetting { get; set; } = EmergencyLogSettings.ThrowExceptions;

        /// <summary>
        /// Setup the emergency log with a FileLogWriter in the default temp folder and with a random file name. It also adds the default Exception formatter
        /// </summary>
        public static void SetupEmergencyLog()
        {
            SetupEmergencyLog(System.IO.Path.GetTempPath(), "TinyLog_" + System.IO.Path.GetRandomFileName() + ".log");
        }

        /// <summary>
        /// Setup the emergency log with a FileLogWriter in the path specified and an Exception formatter
        /// </summary>
        /// <param name="logPath">The location of the log</param>
        /// <param name="logFileName">The name of the log</param>
        public static void SetupEmergencyLog(string logPath, string logFileName)
        {
            _EmergencyLog = Create(new List<LogWriter>() { new Writers.FileLogWriter(logPath, logFileName) }, new List<LogFormatter>() { new Formatters.ExceptionFormatter() });
        }

        /// <summary>
        /// Setup the emergency log with user defined settings
        /// </summary>
        /// <param name="emergencyLog"></param>
        public static void SetupEmergencyLog(Log emergencyLog)
        {
            _EmergencyLog = emergencyLog;
        }

        public static Log EmergencyLog
        {
            get
            {
                return _EmergencyLog;
            }
        }

        /// <summary>
        ///  Writes an entry in the Emergency log
        /// </summary>
        /// <param name="logEntry">The log entry to write</param>
        /// <param name="exception">The exception to attach to the log entry</param>
        private static async void WriteEmergencyLog(LogEntry logEntry, Exception exception)
        {
            if (_EmergencyLog == null)
            {
                throw new InvalidOperationException("Cannot write exceptions to the Emergency log before it has been setup. Use the SetupEmergencyLog() methods to create a log first");
            }
            if (logEntry == null)
            {
                throw new ArgumentNullException("logEntry");
            }
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            try
            {
                bool b = await EmergencyLog.WriteLogEntryAsync<Exception>(logEntry, exception, EmergencyLogSettings.ThrowExceptions);
                if (!b)
                {
                    throw new InvalidOperationException(string.Format("An Emergency log was not written: {0} with the title {1}", logEntry.Id, logEntry.Title), exception);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("An Emergency log was not written: {0} with the title {1}", logEntry.Id, logEntry.Title), ex);
            }
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
                    throw new InvalidOperationException("A log writer failed to initialize: {0}", ex);
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
                throw new InvalidOperationException("The log writer failed to initialize", ex);
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
        /// <param name="emergencyLogSetting">The emergency log setting to use in the write operation. If null, the global emergency log setting will be used</param>
        /// <returns>true if the LogEntry was comitted succesfully by all LogWriters</returns>
        public Task<bool> WriteLogEntryAsync(LogEntry logEntry, EmergencyLogSettings? emergencyLogSetting = null)
        {
            return Task.FromResult<bool>(WriteLogEntry(logEntry,emergencyLogSetting ?? EmergencyLogSetting));
        }


        /// <summary>
        /// Writes a new log entry to the log
        /// </summary>
        /// <param name="logEntry">The LogEntry object to write</param>
        /// <returns>true if the LogEntry was comitted succesfully by all LogWriters</returns>
        public bool WriteLogEntry(LogEntry logEntry)
        {
            return WriteLogEntry(logEntry, EmergencyLogSetting);
        }

        /// <summary>
        /// Writes a new log entry to the log
        /// </summary>
        /// <param name="logEntry">The LogEntry object to write</param>
        /// <param name="emergencyLogSetting">The emergency log setting to use in the write operation</param>
        /// <returns>true if the LogEntry was comitted succesfully by all LogWriters</returns>
        public bool WriteLogEntry(LogEntry logEntry, EmergencyLogSettings emergencyLogSetting)
        {
            int errors = -1;
            IEnumerable<LogWriter> current = _writers.Where(x => x.Filter.IsMatch(logEntry));
            ConcurrentBag<Exception> parallelExceptions = new ConcurrentBag<Exception>();
            if (current.Count() > 0)
            {
                errors = 0;

                Parallel.ForEach<LogWriter>(current, async writer =>
                {
                    try
                    {
                        Tuple<bool, Exception> result = await writer.TryWriteLogEntryAsync(logEntry);
                        if (!result.Item1)
                        {
                            parallelExceptions.Add(result.Item2);
                            System.Threading.Interlocked.Add(ref errors, 1);
                        }
                    }
                    catch (Exception writeException)
                    {
                        System.Threading.Interlocked.Add(ref errors, 1);
                        parallelExceptions.Add(writeException);
                    }
                });

                if (parallelExceptions.Count > 0)
                {
                    switch (emergencyLogSetting)
                    {
                        case EmergencyLogSettings.Ignore:
                            break;
                        case EmergencyLogSettings.AddToEmergencyLog:
                            WriteEmergencyLog(LogEntry.Copy(logEntry), new AggregateException(parallelExceptions));
                            break;
                        case EmergencyLogSettings.ThrowExceptions:
                            throw new AggregateException("Unhandled errors occured in one or more log writers", parallelExceptions);
                        default:
                            break;
                    }
                }
            }

            parallelExceptions = new ConcurrentBag<Exception>();
            Parallel.ForEach(_subscribers.Where(x => x.Filter.IsMatch(logEntry)), async subscriber =>
                {
                    try
                    {
                        await subscriber.ReceiveAsync(logEntry, errors == 0);
                    }
                    catch (Exception subscriberException)
                    {
                        parallelExceptions.Add(subscriberException);
                    }

                });

            if (parallelExceptions.Count > 0)
            {
                switch (emergencyLogSetting)
                {
                    case EmergencyLogSettings.Ignore:
                        break;
                    case EmergencyLogSettings.AddToEmergencyLog:
                        WriteEmergencyLog(LogEntry.Copy(logEntry), new AggregateException(parallelExceptions));
                        break;
                    case EmergencyLogSettings.ThrowExceptions:
                        throw new AggregateException("Unhandled errors occured in one or more log writers", parallelExceptions);
                    default:
                        break;
                }
            }

            return errors == 0;
        }


        /// <summary>
        /// Writes a new log entry to the log
        /// </summary>
        /// <param name="logEntry">The LogEntry object to write</param>
        /// <param name="emergencyLogSetting">The emergency log setting to use in the write operation. If null, the global emergency log setting will be used</param>
        /// <returns>true if the LogEntry was comitted succesfully by all LogWriters</returns>
        public bool WriteLogEntry(LogEntry logEntry, object customData, EmergencyLogSettings? emergencyLogSetting = null)
        {
            return WriteLogEntry<object>(logEntry, customData, emergencyLogSetting ?? EmergencyLogSetting);
        }

        /// <summary>
        /// Writes a new log entry to the log
        /// </summary>
        /// <param name="logEntry"></param>
        /// <param name="customData"></param>
        /// <param name="emergencyLogSetting">The emergency log setting to use in the write operation. If null, the global emergency log setting will be used</param>
        /// <returns></returns>
        public Task<bool> WriteLogEntryAsync(LogEntry logEntry, object customData, EmergencyLogSettings? emergencyLogSetting = null)
        {
            return Task.FromResult<bool>(WriteLogEntry<object>(logEntry, customData, emergencyLogSetting ?? EmergencyLogSetting));
        }

        /// <summary>
        /// Writes a new log entry to the log
        /// </summary>
        /// <typeparam name="T">The type of custom data to format the log entry with</typeparam>
        /// <param name="logEntry">the LogEntry object to write</param>
        /// <param name="customData">The custom data for the log entry</param>
        /// <param name="emergencyLogSetting">The emergency log setting to use in the write operation. If null, the global emergency log setting will be used</param>
        /// <returns>true if the LogEntry was comitted succesfully by all LogWriters</returns>
        public Task<bool> WriteLogEntryAsync<T>(LogEntry logEntry, T customData, EmergencyLogSettings? emergencyLogSetting = null)
        {
            return Task.FromResult<bool>(WriteLogEntry<T>(logEntry, customData, emergencyLogSetting ?? EmergencyLogSetting));
        }

        /// <summary>
        /// Writes a new log entry to the log
        /// </summary>
        /// <typeparam name="T">The type of custom data to format the log entry with</typeparam>
        /// <param name="logEntry">the LogEntry object to write</param>
        /// <param name="customData">The custom data for the log entry</param>
        /// <param name="emergencyLogSetting">The emergency log setting to use in the write operation</param>
        /// <returns>true if the LogEntry was comitted succesfully by all LogWriters</returns>
        public bool WriteLogEntry<T>(LogEntry logEntry, T customData, EmergencyLogSettings? emergencyLogSetting = null)
        {
            EmergencyLogSettings setting = emergencyLogSetting ?? EmergencyLogSetting;
            try
            {
                LogFormatter formatter = _formatters.FirstOrDefault(x => x.IsValidFormatterFor(customData));
                if (formatter != null)
                {
                    formatter.Format(logEntry, customData);
                    logEntry.CustomDataFormatter = logEntry.CustomDataFormatter ?? formatter.GetType().FullName;
                }
                else
                {
                    logEntry.CustomData = string.Format("No formatter available for: {0}", customData);
                }
            }
            catch (Exception ex)
            {
                switch (setting)
                {
                    case EmergencyLogSettings.Ignore:
                        break;
                    case EmergencyLogSettings.AddToEmergencyLog:
                        WriteEmergencyLog(LogEntry.Copy(logEntry), ex);
                        break;
                    case EmergencyLogSettings.ThrowExceptions:
                        throw new InvalidOperationException("The log entry formatter failed to format the log entry", ex);
                    default:
                        break;
                }
            }
            
            return WriteLogEntry(logEntry, setting);
        }

        




        #endregion
    }
}
