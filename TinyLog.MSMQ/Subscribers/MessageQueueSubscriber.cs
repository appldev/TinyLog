using System;
using System.Messaging;
using System.Threading.Tasks;
using TinyLog.MSMQ;

namespace TinyLog.Subscribers
{
    public class MessageQueueSubscriber : LogSubscriber
    {

        public MessageQueueSubscriber(string queueName, LogEntryFilter filter = null)
            : base(filter)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException("queueName");
            }
            _QueueName = queueName;
        }

        private string _QueueName;
        private MessageQueue _Queue;
        private static object queueLock = new object();
        public override void Receive(LogEntry logEntry, bool Created)
        {
            lock (queueLock)
            {
                Helpers.PostMessage(logEntry, _Queue);
            }
        }

        public override Task ReceiveAsync(LogEntry logEntry, bool Created)
        {
            Receive(logEntry, Created);
            return Task.FromResult<object>(null);
        }

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
                Helpers.PostMessage(LogEntry.Verbose(LogEntrySourceDefaults.Log, LogAreaDefaults.LogSubscriber, string.Format("The MessageQueueSubscriber initialized on the queue '{0}", _QueueName)), _Queue);
                return true;
            }
            catch (Exception ex)
            {
                initializeException = new InvalidOperationException("The MessageQueueSubscriber failed to initialize. See inner exception for details.", ex);
                return false;
            }
        }
    }
}
