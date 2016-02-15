using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyLog.Subscribers
{
    /// <summary>
    /// A subscriber that writes incoming log entries to json files in the specified folder
    /// </summary>
    public class JsonFileSubcriber : LogSubscriber
    {
        /// <summary>
        /// Creates an instance of the JsonFileSubcriber
        /// </summary>
        /// <param name="folder">The folder to dump log entries as xml files</param>
        /// <param name="indentText">true to indent the serialized json text</param>
        /// <param name="filter">A filter to use for receiving log entries</param>
        public JsonFileSubcriber(string folder, bool indentText = true, LogEntryFilter filter = null)
            : base(filter)
        {
            if (string.IsNullOrEmpty(folder))
            {
                throw new ArgumentNullException("folder");
            }
            _Folder = folder;
            indent = indentText ? Formatting.Indented : Formatting.None;
        }

        private Formatting indent = Formatting.Indented;
        private JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateParseHandling = DateParseHandling.DateTimeOffset
        };
        private string _Folder = null;
        private static object JsonFileLock = new object();
        public override void Receive(LogEntry logEntry, bool Created)
        {
            string fullPath = Path.Combine(_Folder, logEntry.Id.ToString() + (Created ? "" : "-notcreated") + ".json");
            lock (JsonFileLock)
            {
                File.WriteAllText(fullPath, JsonConvert.SerializeObject(logEntry, indent, settings));
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
                if (!Directory.Exists(_Folder))
                {
                    Directory.CreateDirectory(_Folder);
                }
            }
            catch (Exception ex)
            {
                initializeException = new Exception(string.Format("Unable to create or access the path '{0}'", _Folder), ex);
                return false;
            }
            string TestFile = Path.Combine(_Folder, Path.GetTempFileName());
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
    }
}
