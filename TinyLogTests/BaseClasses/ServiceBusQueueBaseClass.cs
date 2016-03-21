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
    public class ServiceBusQueueBaseClass
    {
        public ServiceBusQueueBaseClass()
        {
            ConnectionString = connection("TinyLogServiceBus.txt");
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

        public ServiceBusQueueBaseClass(string connectionFile, string queuePath)
        {
            ConnectionString = connection(connectionFile);
            LogPath = queuePath;
        }

        public ServiceBusQueueBaseClass(string connectionString = null, string queuePath = null, LogEntryFilter filter = null)
        {
            ConnectionString = connectionString ?? connection("TinyLogServiceBus.txt");
            LogPath = queuePath ?? "tinylog";
            Filter = filter;

        }

        public string LogPath = "tinylog";
        public string ConnectionString;
        public LogEntryFilter Filter = null;
        protected bool CreateQueueIfNotExists = false;
        protected bool IsTopic = false;
        public Random random = new Random();

        public Log log { get; set; }


        [TestInitialize]
        public void InitializeLog()
        {
            Console.WriteLine("initializing the log: {0}", LogPath);
            if (IsTopic)
            {
                log = Log.Create(new List<LogWriter>() { new AzureServiceBusTopicLogWriter(ConnectionString, LogPath, CreateQueueIfNotExists) });
            }
            else
            {
                log = Log.Create(new List<LogWriter>() { new AzureServiceBusQueueLogWriter(ConnectionString, LogPath, CreateQueueIfNotExists) });
            }
        }

        [TestCleanup]
        public void CleanUp()
        {
            log.Dispose();
        }

    }
}
