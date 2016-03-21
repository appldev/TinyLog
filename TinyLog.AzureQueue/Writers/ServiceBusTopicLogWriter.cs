using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Threading.Tasks;
using TinyLog.Azure;

namespace TinyLog.Writers
{
    public class AzureServiceBusTopicLogWriter : LogWriter, IDisposable
    {
        public AzureServiceBusTopicLogWriter(string connectionString, string topicName, bool createTopicIfNotExists = false)
        {
            _ConnectionString = connectionString;
            _TopicName = topicName;
            _CreateTopicIfNotExists = createTopicIfNotExists;
        }

        public override bool TryInitialize(out Exception initializeException)
        {
            try
            {
                if (_CreateTopicIfNotExists)
                {
                    NamespaceManager ns = NamespaceManager.CreateFromConnectionString(_ConnectionString);
                    
                    if (!ns.TopicExists(_TopicName))
                    {
                        ns.CreateTopic(_TopicName);
                    }
                }
                client = TopicClient.CreateFromConnectionString(_ConnectionString, _TopicName);
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
        private readonly string _TopicName;
        private readonly bool _CreateTopicIfNotExists = false;
        private TopicClient client = null;


        public override bool TryWriteLogEntry(LogEntry logEntry, out Exception writeException)
        {
            try
            {
                BrokeredMessage m = Helpers.GetBrokeredMessage(logEntry,true);
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
