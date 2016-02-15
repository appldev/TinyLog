using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TinyLog.Writers
{
    /// <summary>
    /// Writes log entries to the file system
    /// </summary>
    public class FileLogWriter : LogWriter
    {
        /// <summary>
        /// Initializes the Log Writer
        /// </summary>
        /// <param name="logPath">The path to create the log files</param>
        /// <param name="oneFilePerDay"></param>
        public FileLogWriter(string logPath, string fileName = "TinyLog.txt", bool oneFilePerDay = true, LogEntryFilter filter = null)
        {
            CheckForNull(logPath, "logPath");
            CheckForNull(fileName, "fileName");
            _path = logPath;
            _extension = Path.GetExtension(fileName);
            _fileName = Path.GetFileNameWithoutExtension(fileName);
            _perDay = oneFilePerDay;
            Filter = filter ?? LogEntryFilter.Create();
        }

        private string _fileName = "";
        private string _extension = "";
        private string _path = "";
        private bool _perDay = true;

        #region LogWriter implementation

        private static object FileLock = new object();
        public override bool TryInitialize(out Exception initializeException)
        {
            initializeException = null;
            try
            {
                if (!Directory.Exists(_path))
                {
                    Directory.CreateDirectory(_path);
                }
            }
            catch (Exception ex)
            {
                initializeException = new Exception(string.Format("Unable to create or access the path '{0}'", _path), ex);
                return false;
            }
            string TestFile = Path.Combine(_path, Path.GetTempFileName());
            try
            {
                File.WriteAllText(TestFile, "TinyLog");
                File.Delete(TestFile);
            }
            catch (Exception ex2)
            {
                initializeException = new Exception(string.Format("Unable to write and/or delete the test file '{0}'", TestFile), ex2);
                return false;
            }
            return true;
        }



        public override bool TryWriteLogEntry(LogEntry logEntry, out Exception writeException)
        {
            string file = GetLogFilePath();
            writeException = null;
            try
            {
                lock(FileLock)
                {
                    if (!File.Exists(file))
                    {
                        File.WriteAllText(file, CreateHeader());
                    }
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("{0}", logEntry.CreatedOnString);
                    sb.Append(Append(logEntry.Id.ToString(), "\""));
                    sb.Append(Append(Enum.GetName(typeof(LogEntrySeverity), logEntry.Severity), "\""));
                    sb.Append(Append(logEntry.Title, "\""));
                    sb.Append(Append(logEntry.Source, "\""));
                    sb.Append(Append(logEntry.Area, "\""));
                    sb.Append(Append(logEntry.Client, "\""));
                    sb.Append(Append(logEntry.ClientInfo, "\""));
                    sb.Append(Append(logEntry.Message, "\""));
                    sb.Append(Append(logEntry.CustomData, "\""));
                    sb.Append(Append(logEntry.CustomDataFormatter, "\""));
                    sb.AppendLine();
                    File.AppendAllText(file, sb.ToString(), Encoding.UTF8);
                    return true;
                }
            }
            catch (Exception exWrite)
            {
                writeException = exWrite;
                return false;
            }
        }

        public override Task<Tuple<bool, Exception>> TryWriteLogEntryAsync(LogEntry logEntry)
        {
            Exception writeException;
            bool b = TryWriteLogEntry(logEntry, out writeException);
            return Task.FromResult<Tuple<bool, Exception>>(new Tuple<bool, Exception>(b, writeException));
        }

        #endregion

        #region private helpers

        private string CreateHeader()
        {
            return "CreatedOn;Id;Severity;Title;Source;Area;Client;ClientInfo;Message;CustomData;CustomDataFormatter\r\n";
        }

        private string Append(string info, string quote)
        {
            return string.Format(";{1}{0}{1}", info, quote);
        }

        private void CheckForNull(string parameter, string parameterName)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        private string GetLogFilePath()
        {
            string postfix = _perDay ? DateTime.Today.ToString("yyyyMMdd") : "";
            return Path.Combine(_path, _fileName + postfix + _extension);
        }

        #endregion


    }
}
