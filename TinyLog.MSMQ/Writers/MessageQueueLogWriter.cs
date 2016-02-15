using System;
using System.Messaging;
using System.Threading.Tasks;
using TinyLog.MSMQ;

namespace TinyLog.Writers
{
    /// <summary>
    /// A log writer that posts all logs on a MessageQueue
    /// </summary>
    public class MessageQueueLogWriter : LogWriter
    {
        public MessageQueueLogWriter(string queueName, LogEntryFilter filter = null)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException("queueName");
            }
            if (filter != null)
            {
                Filter = filter;
            }
            _QueueName = queueName;
        }

        private string _QueueName;
        private MessageQueue _Queue;

        public override bool TryInitialize(out Exception initializeException)
        {
            initializeException = null;
            try
            {
                if (!MessageQueue.Exists(_QueueName))
                {
                    MessageQueue.Create(_QueueName, true);
                }
                _Queue = new MessageQueue(_QueueName, QueueAccessMode.Send);
                Helpers.PostMessage(LogEntry.Verbose(LogEntrySourceDefaults.Log, LogAreaDefaults.LogWriter, string.Format("The MessageQueueWriter initialized on the queue '{0}", _QueueName)),_Queue);
                return true;
            }
            catch (Exception ex)
            {
                initializeException = new InvalidOperationException("The MSMQWriter failed to initialize. See inner exception for details.", ex);
                return false;
            }
        }

        public override bool TryWriteLogEntry(LogEntry logEntry, out Exception writeException)
        {
            writeException = null;
            try
            {
                Helpers.PostMessage(logEntry, _Queue);
            }
            catch (Exception ex)
            {
                writeException = ex;
                return false;
            }
            return true;
        }

        public override Task<Tuple<bool, Exception>> TryWriteLogEntryAsync(LogEntry logEntry)
        {
            Exception writeException;
            bool b = TryWriteLogEntry(logEntry, out writeException);
            return Task.FromResult<Tuple<bool, Exception>>(new Tuple<bool, Exception>(b, writeException));

        }

        
    }
}
