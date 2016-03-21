using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;
using TinyLog.Azure;

namespace TinyLog.Writers
{
    public class AzureStorageTableLogWriter : LogWriter
    {
        public AzureStorageTableLogWriter(string connectionString, string tableName, bool createTableIfNotExists = false)
        {
            _ConnectionString = connectionString;
            _TableName = tableName;
            _CreateTableIfNotExists = createTableIfNotExists;

        }

        public override bool TryInitialize(out Exception initializeException)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_ConnectionString);
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                tableClient.DefaultRequestOptions.PayloadFormat = TablePayloadFormat.Json;
                table = tableClient.GetTableReference(_TableName);
                if (_CreateTableIfNotExists)
                {
                    table.CreateIfNotExists();
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
        private readonly string _TableName;
        private readonly bool _CreateTableIfNotExists = false;
        private CloudTable table = null;


        public override bool TryWriteLogEntry(LogEntry logEntry, out Exception writeException)
        {
            try
            {
                TableOperation to = TableOperation.Insert(new LogEntryTableEntity(logEntry));
                table.Execute(to);
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
