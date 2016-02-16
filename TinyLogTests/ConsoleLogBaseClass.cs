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
    public class ConsoleLogBaseClass
    {
        public ConsoleLogBaseClass()
        {

        }
        public ConsoleLogBaseClass(LogEntryFilter filter = null)
        {
            Filter = filter;
        }
        public LogEntryFilter Filter = null;
        public Random random = new Random();


        public Log log { get; set; }


        [TestInitialize]
        public void InitializeLog()
        {
            Console.WriteLine("initializing the console log");
            log = Log.Create(new List<LogWriter>() { new ConsoleLogWriter(Filter) });
        }

    }
}
