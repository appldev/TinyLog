using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Threading.Tasks;

namespace TinyLog.Writers
{
    public class AzureStorageQueueLogWriter : LogWriter
    {
        public AzureStorageQueueLogWriter(string connectionString, string queueName, bool createQueueIfNotExists = false)
        {
            _ConnectionString = connectionString;
            _QueueName = queueName;
            _CreateQueueIfNotExists = createQueueIfNotExists;

        }

        public override bool TryInitialize(out Exception initializeException)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_ConnectionString);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                queue = queueClient.GetQueueReference(_QueueName);
                if (_CreateQueueIfNotExists)
                {
                    queue.CreateIfNotExists();
                }
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
        private CloudQueue queue = null;


        public override bool TryWriteLogEntry(LogEntry logEntry, out Exception writeException)
        {
            try
            {
                //BinaryFormatter bf = new BinaryFormatter();
                string body = Newtonsoft.Json.JsonConvert.SerializeObject(logEntry);
                int i = body.Length * sizeof(Char);
                if (i > 65535)
                {
                    writeException = new InvalidOperationException(string.Format("The message length is {0}. The message must not exceed 65535 bytes", body.Length));
                    return false;
                }
                CloudQueueMessage m = new CloudQueueMessage(body);
                queue.AddMessage(m);
                writeException = null;
                return true;
            }
            catch (Exception ex)
            {
                writeException = ex;
                return false;
            }
        }

        public override Task<Tuple<bool, Exception>> TryWriteLogEntryAsync(LogEntry logEntry)
        {
            Exception writeException;
            bool b = TryWriteLogEntry(logEntry, out writeException);
            return Task.FromResult<Tuple<bool, Exception>>(new Tuple<bool, Exception>(b, writeException));
        }

    }
}
