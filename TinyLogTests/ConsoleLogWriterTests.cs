using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyLog;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using static TinyLogTests.LogHelpers;
using TinyLog.Formatters;

namespace TinyLogTests
{
    [TestClass]
    public class ConsoleLogWriterTests : ConsoleLogBaseClass
    {
        [TestInitialize]
        public void Initialize()
        {
            log.RegisterLogFormatter(new JsonSerializationFormatter());
        }
        [TestMethod]
        [TestCategory("Public")]
        public void LogEntryCreationWithoutCustomData_ConsoleLogWriterTests()
        {
            List<LogEntry> entries = LogHelpers.CreateLogEntries();
            int num = 0;
            Parallel.ForEach(entries, async logEntry =>
            {
                if (await log.WriteLogEntryAsync(logEntry))
                {
                    Interlocked.Add(ref num, 1);
                }
            });
            Console.WriteLine("{0} of {1} logs written to console without custom data", num, entries.Count);
            Assert.IsTrue(num == entries.Count);
        }

        [TestMethod]
        [TestCategory("Public")]
        public void LogEntryCreationWithCustomData_ConsoleLogWriterTests()
        {
            List<LogEntry> entries = LogHelpers.CreateLogEntries();
            List<Company> list = LogHelpers.GetCompanies(10, 2);
            int num = 0;
            Parallel.ForEach(entries, async logEntry =>
            {
                if (await log.WriteLogEntryAsync<List<Company>>(logEntry, list))
                {
                    Interlocked.Add(ref num, 1);
                }
            });
            Console.WriteLine("{0} of {1} logs written to console without custom data", num, entries.Count);
            Assert.IsTrue(num == entries.Count);
        }
    }
}
