using System;

namespace TinyLog
{
    /// <summary>
    /// The severity definition of a LogEntry
    /// </summary>
    public enum LogEntrySeverity
    {
        /// <summary>
        /// Critical system crash
        /// </summary>
        Critical = 1,
        /// <summary>
        /// Non-critical System error
        /// </summary>
        Error = 2,
        /// <summary>
        /// A system warning. Potential error.
        /// </summary>
        Warning = 3,
        /// <summary>
        /// System information
        /// </summary>
        Information = 4,
        /// <summary>
        /// Verbose information. Typically a system trace with internal information used for diagnostics and debugging
        /// </summary>
        Verbose = 5
    }

    /// <summary>
    /// Defines an entry in the log
    /// </summary>
    [Serializable]
    public class LogEntry
    {
        #region ctor
        public LogEntry()
        {
           
        }

        #endregion

        #region static instantiation methods

        public static LogEntry Create(string title, string message, LogEntrySeverity severity = LogEntrySeverity.Information)
        {
            return new LogEntry() { Title = title, Message = message, Severity = severity };
        }

        public static LogEntry Create(string title, string message, string source, string area, LogEntrySeverity severity = LogEntrySeverity.Information)
        {
            return new LogEntry() { Title = title, Message = message, Source = source, Area = area, Severity = severity };
        }

        public static LogEntry Warning(string title, string message, string source, string area)
        {
            return new LogEntry() { Title = title, Message = message, Source = source, Area = area, Severity = LogEntrySeverity.Warning };
        }

        public static LogEntry Error(string title, string message, string source, string area)
        {
            return new LogEntry() { Title = title, Message = message, Source = source, Area = area, Severity = LogEntrySeverity.Error };
        }

        public static LogEntry Critical(string title, string message, string source, string area)
        {
            return new LogEntry() { Title = title, Message = message, Source = source, Area = area, Severity = LogEntrySeverity.Critical };
        }

        public static LogEntry Verbose(string source, string area, string title = null, string message = null)
        {
            return new LogEntry() { Title = title, Message = message, Source = source, Area = area, Severity = LogEntrySeverity.Verbose };
        }

        #endregion  


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
       [System.Xml.Serialization.XmlText]
        public string Message { get; set; }
        /// <summary>
        /// The source of the log entry
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// The area reffered to by the log entry
        /// </summary>
        public string Area {get; set;}
        /// <summary>
        /// The severity level of the log
        /// </summary>
        public LogEntrySeverity Severity { get; set; } = LogEntrySeverity.Information;

        /// <summary>
        /// Returns the string representation of the Severity
        /// </summary>
        public string SeverityString
        {
            get
            {
                return Enum.GetName(typeof(LogEntrySeverity), Severity);
            }
        }

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
    }
}
