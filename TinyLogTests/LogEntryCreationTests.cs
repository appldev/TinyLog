using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyLog;
using TinyLog.Formatters;
using static TinyLogTests.LogHelpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace TinyLogTests
{
    [TestClass]
    public class LogEntryCreationTests : MessageQueueLogBaseClass
    {
        [TestInitialize]
        public void initialize()
        {
            log.RegisterLogFormatter(new JsonSerializationFormatter(true));
        }

        [TestMethod]
        public void LogEntryCreationAsGeneric()
        {
            List<LogEntry> entries = CreateLogEntries();
            List<Company> list = LogHelpers.GetCompanies(random.Next(20, 40), random.Next(5, 15));
            int num = 0;
            Parallel.ForEach(entries, async logEntry =>
            {
                if (await log.WriteLogEntryAsync<List<Company>>(logEntry, list))
                {
                    Interlocked.Add(ref num, 1);
                }
            });
            Console.WriteLine("{0} of {1} logs written to {2} with List<Company>", num, entries.Count, LogPath);
            Assert.IsTrue(num == entries.Count);

        }

        [TestMethod]
        public void LogEntryCreationAsObject()
        {
            List<LogEntry> entries = CreateLogEntries();
            List<Company> list = LogHelpers.GetCompanies(random.Next(20, 40), random.Next(5, 15));
            int num = 0;
            Parallel.ForEach(entries, async logEntry =>
            {
                if (await log.WriteLogEntryAsync(logEntry, list))
                {
                    Interlocked.Add(ref num, 1);
                }
            });
            Console.WriteLine("{0} of {1} logs written to {2} with object", num, entries.Count, LogPath);
            Assert.IsTrue(num == entries.Count);
        }

        [TestMethod]
        public void LogEntryCreationWithoutCustomData()
        {
            List<LogEntry> entries = CreateLogEntries();
            int num = 0;
            Parallel.ForEach(entries, async logEntry =>
            {
                if (await log.WriteLogEntryAsync(logEntry))
                {
                    Interlocked.Add(ref num, 1);
                }
            });
            Console.WriteLine("{0} of {1} logs written to {2} without custom data", num, entries.Count, LogPath);
            Assert.IsTrue(num == entries.Count);
        }


       
    }
}
