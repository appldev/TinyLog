using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyLog;
using TinyLog.Writers;

namespace TinyLogTests
{
    [TestClass]
    public class AzureStorageBaseClass
    {
        public AzureStorageBaseClass()
        {
            ConnectionString = connection("TinyLogQueue.txt");
            LogPath = "tinylog";
        }

        protected virtual string connection(string fileName)
        {
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
            if (!System.IO.File.Exists(path))
            {
                throw new InvalidOperationException(string.Format("The file {0} must exist and contain the connection string for the Azure Service Bus Queue for this test to run"));
            }
            return System.IO.File.ReadAllText(path);
        }

        public AzureStorageBaseClass(string connectionFile, string queuePath)
        {
            ConnectionString = connection(connectionFile);
            LogPath = queuePath;
        }

        public AzureStorageBaseClass(string connectionString = null, string queuePath = null, LogEntryFilter filter = null)
        {
            ConnectionString = connectionString ?? connection("TinyLogQueue.txt");
            LogPath = queuePath ?? "tinylog";
            Filter = filter;

        }

        public string LogPath = "tinylog";
        public string ConnectionString;
        public LogEntryFilter Filter = null;
        protected bool CreateQueueIfNotExists = false;
        protected bool IsTable = false;
        public Random random = new Random();

        public Log log { get; set; }


        [TestInitialize]
        public void InitializeLog()
        {
            Console.WriteLine("initializing the log: {0}", LogPath);
            if (IsTable)
            {
                log = Log.Create(new List<LogWriter>() { new AzureStorageTableLogWriter(ConnectionString, LogPath, CreateQueueIfNotExists) });
            }
            else
            {
                log = Log.Create(new List<LogWriter>() { new AzureStorageQueueLogWriter(ConnectionString, LogPath, CreateQueueIfNotExists) });
            }
        }

        [TestCleanup]
        public void CleanUp()
        {
            log.Dispose();
        }

    }
}
