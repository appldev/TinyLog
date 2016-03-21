using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace TinyLog.Azure
{
    public class LogEntryTableEntity : TableEntity
    {
        public LogEntryTableEntity()
        {

        }

        public LogEntryTableEntity(LogEntry logEntry)
        {
            RowKey = logEntry.Id.ToString();
            PartitionKey = logEntry.SeverityString;
            Id = logEntry.Id;
            CorrelationId = logEntry.CorrelationId;
            CreatedOn = logEntry.CreatedOn;
            Title = logEntry.Title;
            Message = logEntry.Message;
            Source = logEntry.Source;
            Area = logEntry.Area;
            Client = logEntry.Client;
            ClientInfo = logEntry.ClientInfo;
            Severity = logEntry.SeverityString;
            CustomData = logEntry.CustomData;
            CustomDataFormatter = logEntry.CustomDataFormatter;
            CustomDataType = logEntry.CustomDataType;
            Signature = logEntry.Signature;
            SignatureMethod = logEntry.SignatureMethod;
        }

        /// <summary>
        /// If specified, identities two or more log entries within a transaction
        /// </summary>
        public Guid? CorrelationId { get; set; }

        /// <summary>
        /// The unique id of the log
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// Local time of creation
        /// </summary>
        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
        /// <summary>
        /// Returns the string representation of the CreatedOn Date
        /// </summary>
        public string CreatedOnString
        {
            get
            {
                return CreatedOn.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");
            }
        }
        /// <summary>
        /// Log entry title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Log entry message
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// The source of the log entry
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// The area reffered to by the log entry
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// The severity level of the log
        /// </summary>
        public string Severity { get; set; }

        /// <summary>
        /// The client creating the log entry
        /// </summary>
        public string Client { get; set; }
        /// <summary>
        /// detailed information about the client creating the log entry
        /// </summary>
        public string ClientInfo { get; set; }
        /// <summary>
        /// Custom log data
        /// </summary>
        public string CustomData { get; set; }
        /// <summary>
        /// The formatter used for providing custom data for the log entry, if any.
        /// </summary>
        public string CustomDataFormatter { get; set; }

        /// <summary>
        /// Contains the type of data contained in the CustomData property.
        /// </summary>
        public string CustomDataType { get; set; }
        /// <summary>
        /// Contains the signature for the log entry. The signature is used to validate that the log entry has not been tampered with
        /// </summary>
        public string Signature { get; set; }
        /// <summary>
        /// Contains the name of the method used to create the signature for the log entry
        /// </summary>
        public string SignatureMethod { get; set; }

    }
}
