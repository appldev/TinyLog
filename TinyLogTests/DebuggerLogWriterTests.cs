//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using TinyLog;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Threading;

//namespace TinyLogTests
//{
//    [TestClass]
//    public class DebuggerLogWriterTests : DebuggerLogBaseClass
//    {
//        [TestMethod]
//        public void LogEntryCreationWithoutCustomData_DebuggerLogWriterTests()
//        {
//            List<LogEntry> entries = LogHelpers.CreateLogEntries();
//            int num = 0;
//            Parallel.ForEach(entries, async logEntry =>
//            {
//                if (await log.WriteLogEntryAsync(logEntry))
//                {
//                    Interlocked.Add(ref num, 1);
//                }
//            });
//            Console.WriteLine("{0} of {1} logs written to debugger without custom data", num, entries.Count);
//            Assert.IsTrue(num == entries.Count);
//        }
//    }
//}
