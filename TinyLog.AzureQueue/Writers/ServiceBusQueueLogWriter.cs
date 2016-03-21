using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Threading.Tasks;
using TinyLog.Azure;

namespace TinyLog.Writers
{
    public class AzureServiceBusQueueLogWriter : LogWriter, IDisposable
    {
        public AzureServiceBusQueueLogWriter(string connectionString, string queueName, bool createQueueIfNotExists = false)
        {
            _ConnectionString = connectionString;
            _QueueName = queueName;
            _CreateQueueIfNotExists = createQueueIfNotExists;
            
        }

        public override bool TryInitialize(out Exception initializeException)
        {
            try
            {
                if (_CreateQueueIfNotExists)
                {
                    NamespaceManager ns = NamespaceManager.CreateFromConnectionString(_ConnectionString);
                    if (!ns.QueueExists(_QueueName))
                    {
                        ns.CreateQueue(_QueueName);
                    }
                }
                client = QueueClient.CreateFromConnectionString(_ConnectionString, _QueueName);
                initializeException = null;
                return true;
            }
            catch (Exception ex)
            {
                initializeException = ex;
                return false;
            }

        }

        private readonly string _ConnectionString;
        private readonly string _QueueName;
        private readonly bool _CreateQueueIfNotExists = false;
        private QueueClient client = null;


        public override bool TryWriteLogEntry(LogEntry logEntry, out Exception writeException)
        {
            try
            {
                BrokeredMessage m = Helpers.GetBrokeredMessage(logEntry);
                client.Send(m);
                writeException = null;
                return true;
            }
            catch (Exception ex)
            {
                writeException = ex;
                return false;
                throw;
            }
        }

        public override Task<Tuple<bool, Exception>> TryWriteLogEntryAsync(LogEntry logEntry)
        {
            Exception writeException;
            bool b = TryWriteLogEntry(logEntry, out writeException);
            return Task.FromResult<Tuple<bool, Exception>>(new Tuple<bool, Exception>(b, writeException));
        }

        public void Close()
        {
            if (client != null && !client.IsClosed)
            {
                client.Close();
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Close();
                }
                disposedValue = true;
            }
        }

        

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
