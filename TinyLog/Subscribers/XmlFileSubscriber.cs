using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyLog.Subscribers
{
    /// <summary>
    /// A subscriber that writes incoming log entries to xml files in the specified folder
    /// </summary>
    public class XmlFileSubscriber : LogSubscriber
    {
        /// <summary>
        /// Creates an instance of the XmlFileSubscriber
        /// </summary>
        /// <param name="folder">The folder to dump log entries as xml files</param>
        /// <param name="filter">A filter to use for receiving log entries</param>
        public XmlFileSubscriber(string folder, LogEntryFilter filter = null)
            : base(filter)
        {
            if (string.IsNullOrEmpty(folder))
            {
                throw new ArgumentNullException("folder");
            }
            _Folder = folder;
        }

        private string _Folder = null;
        private static object XmlFileLock = new object();
        System.Xml.Serialization.XmlSerializer _serializer = new System.Xml.Serialization.XmlSerializer(typeof(LogEntry));
        public override void Receive(LogEntry logEntry, bool Created)
        {
            string fullPath = Path.Combine(_Folder, logEntry.Id.ToString() + (Created ? "" : "-notcreated") + ".xml");
            lock (XmlFileLock)
            {
                using (System.Xml.XmlWriter tw = System.Xml.XmlTextWriter.Create(fullPath, new System.Xml.XmlWriterSettings() { CloseOutput = true, Encoding = Encoding.UTF8 }))
                {
                    _serializer.Serialize(tw, logEntry);
                    tw.Flush();
                    tw.Close();
                }
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
