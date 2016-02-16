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
    public class FileLogBaseClass
    {
        public FileLogBaseClass()
        {

        }
        public FileLogBaseClass(LogEntryFilter filter = null)
        {
            Filter = filter;
        }
        public string LogPath = Path.GetTempPath();
        public string LogName = "TinyLogTest_" + Path.GetRandomFileName() + ".txt";
        public LogEntryFilter Filter = null;
        public Random random = new Random();
        public string FullPath
        {
            get
            {
                return Path.Combine(LogPath, LogName);
            }
        }

        public Log log { get; set; }


        [TestInitialize]
        public void InitializeLog()
        {
            Console.WriteLine("initializing the log: {0}", FullPath);
            log = Log.Create(new List<LogWriter>() { new FileLogWriter(LogPath, LogName, false, Filter) });
        }

       

    }
}
