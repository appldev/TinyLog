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
    public class MessageQueueLogBaseClass
    {
        public MessageQueueLogBaseClass()
        {

        }
        public MessageQueueLogBaseClass(string queuePath = null, LogEntryFilter filter = null)
        {
            if (!string.IsNullOrEmpty(queuePath))
            {
                LogPath = queuePath;
            }
            Filter = filter;
        }
        public string LogPath = ".\\Private$\\" + Path.GetRandomFileName();
        public LogEntryFilter Filter = null;
        public Random random = new Random();
        
        public Log log { get; set; }


        [TestInitialize]
        public void InitializeLog()
        {
            Console.WriteLine("initializing the log: {0}", LogPath);
            log = Log.Create(new List<LogWriter>() { new MessageQueueLogWriter(LogPath, Filter) });
        }

        [TestCleanup]
        public void CleanUp()
        {
            MessageQueueLogWriter.Delete(LogPath);
        }

    }
}
