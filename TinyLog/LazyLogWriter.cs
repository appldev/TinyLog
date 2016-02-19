using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace TinyLog
{
    /// <summary>
    /// LazyLogWriter encapsulates a LogWriter and makes data being written as Lazy, in intervals.
    /// When writing to this log, log entries will be queued for processing instead of being written to the backend storage, which provides fast feedback to the application for high volume logs.
    /// NOTE: When using the LazyLogWriter, be sure to call Log.Dispose() on any logs with this LogWriter before application exit, to flush any queued LogEntry items
    /// </summary>
    public class LazyLogWriter : LogWriter, IDisposable
    {
        /// <summary>
        /// Initializes the LazyLogWriter
        /// </summary>
        /// <param name="logWriter">The LogWriter to encapsulate</param>
        /// <param name="lazyWriteInterval">The write interval in milliseconds</param>
        public LazyLogWriter(LogWriter logWriter, double lazyWriteInterval = 30000)
        {
            _writer = logWriter;
            _Interval = lazyWriteInterval;
            _Queue = new ConcurrentQueue<LogEntry>();
        }

        private ConcurrentQueue<LogEntry> _Queue;
        private Timer _Timer;
        private LogWriter _writer;
        private bool _Enabled = false;
        private static object _EnabledLocker = new object();
        private double _Interval = 30000;

        /// <summary>
        /// returns true if the LogWriter is enabled. Writing log entries will automatically enable the LazyLogWriter
        /// </summary>
        public bool Enabled
        {
            get
            {
                lock(_EnabledLocker)
                {
                    return _Enabled;
                }
            }
            protected set
            {
                lock(_EnabledLocker)
                {
                    if (value)
                    {
                        if (_Timer == null)
                        {
                            _Timer = new Timer(ProcessQueue, null, TimeSpan.FromMilliseconds(_Interval), TimeSpan.FromMilliseconds(_Interval));
                        }
                        else
                        {
                            _Timer.Change((int)_Interval, (int)_Interval);
                        }

                        _Enabled = value;
                    }
                    else
                    {
                        if (_Timer != null)
                        {
                            _Timer.Dispose();
                            _Timer = null;
                        }
                        _Enabled = value;
                    }
                }
            }
        }

        /// <summary>
        /// Get/Set if the LogEntry items should be written as soon as they enter the queue. When true the timed intervals will not be used
        /// </summary>
        public bool AutoFlush { get; set; } = false;

        /// <summary>
        /// Adds a LogEntry. This method is for internal use only.
        /// </summary>
        /// <param name="logEntry">The LogEntry to add</param>
        protected void Add(LogEntry logEntry)
        {
            _Queue.Enqueue(logEntry);
            if (!Enabled)
            {
                if (AutoFlush)
                {
                    ProcessQueueItems(null);
                }
                else
                {
                    Enabled = true;
                }
            }
        }

        /// <summary>
        /// Flushes the Queue and writes any queued items to the backend storage
        /// </summary>
        protected void Flush()
        {
            ProcessQueueItems(null);
        }

        /// <summary>
        /// Processes the queue items
        /// </summary>
        /// <param name="state">The state object from the timer (not used)</param>
        private void ProcessQueueItems(object state)
        {
            LogEntry logEntry;
            while (_Queue.TryDequeue(out logEntry))
            {
                Exception ex = null;
                try
                {
                    if (!_writer.TryWriteLogEntry(logEntry, out ex))
                    {
                        switch (Log.EmergencyLogSetting)
                        {
                            case EmergencyLogSettings.Ignore:
                                break;
                            case EmergencyLogSettings.AddToEmergencyLog:
                                Log.WriteEmergencyLog(logEntry, ex);
                                break;
                            case EmergencyLogSettings.ThrowExceptions:
                                throw new InvalidOperationException(string.Format("The LazyLogWriter failed to write the log entry {0} to storage", logEntry.Id), ex);
                            default:
                                throw new InvalidOperationException("The LazyLogWriter contains an invalid EmergencyLogSetting");
                        }
                    }
                }
                catch (Exception exWriter)
                {
                    switch (Log.EmergencyLogSetting)
                    {
                        case EmergencyLogSettings.Ignore:
                            break;
                        case EmergencyLogSettings.AddToEmergencyLog:
                            Log.WriteEmergencyLog(logEntry, exWriter);
                            break;
                        case EmergencyLogSettings.ThrowExceptions:
                            throw new InvalidOperationException(string.Format("The LazyLogWriter failed to write the log entry {0} to storage", logEntry.Id), ex);
                        default:
                            throw new InvalidOperationException("The LazyLogWriter contains an invalid EmergencyLogSetting");
                    }
                }
            }
        }
        /// <summary>
        /// Callback used by the interval timer to process items
        /// </summary>
        /// <param name="state">The state from the timer (not used)</param>
        private void ProcessQueue(object state)
        {
            if (_Queue.Count == 0)
            {
                Enabled = false;
                return;
            }
            else if (!Enabled)
            {
                Enabled = true;
            }
            ProcessQueueItems(state);
        }

        /// <summary>
        /// TryInitialize is called immediately after adding a logwriter to the provider. It should check the backend storage
        /// and return true if the logwriter is valid for use. Only writers that return true is added to the provider
        /// </summary>
        /// <param name="initializeException">If TryInitialize returns false, initializeException will contain the cause or error information</param>
        /// <returns>true if the Log Writer is ready for use, otherwise false</returns>
        public override bool TryInitialize(out Exception initializeException)
        {
            initializeException = null;
            return _writer.TryInitialize(out initializeException);
        }

        /// <summary>
        /// Writes a log entry to the backend storage
        /// </summary>
        /// <param name="logEntry">The logentry to write</param>
        /// <param name="writeException">If the writer returns false, the writeException contains the error information</param>
        /// <returns>returns true if the log was sucessfully committed to the backend storage</returns>
        public override bool TryWriteLogEntry(LogEntry logEntry, out Exception writeException)
        {
            writeException = null;
            try
            {
                Add(logEntry);
                return true;
            }
            catch (Exception ex)
            {
                writeException = ex;
                return false;
            }
            
        }

        /// <summary>
        /// Writes a log entry to the backend storage
        /// </summary>
        /// <param name="logEntry">The logentry to write</param>
        /// <param name="writeException">If the writer returns false, the writeException contains the error information</param>
        /// <returns>returns true if the log was sucessfully committed to the backend storage</returns>
        public override Task<Tuple<bool, Exception>> TryWriteLogEntryAsync(LogEntry logEntry)
        {
            try
            {
                Add(logEntry);
                return Task.FromResult<Tuple<bool, Exception>>(new Tuple<bool, Exception>(true, null));
            }
            catch (Exception ex)
            {
                return Task.FromResult<Tuple<bool, Exception>>(new Tuple<bool, Exception>(false, ex));
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Disposes the LazyLogWriter and flushes the queue
        /// </summary>
        /// <param name="disposing">true to dispose the timer and underlying LogWriter</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_Timer != null)
                    {
                        _Timer.Dispose();
                    }
                    if (_writer != null && _writer is IDisposable)
                    {
                        (_writer as IDisposable).Dispose();
                    }
                }
                Flush();
                _Timer = null;
                _Queue = null;
                _writer = null;
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        /// <summary>
        /// Disposes the LazyLogWriter and flushes the queued LogEntry items to the backend storage
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
