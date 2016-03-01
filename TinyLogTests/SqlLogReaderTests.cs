using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyLog;
using TinyLog.Formatters;
using System.Threading.Tasks;
using TinyLog.Readers;
using System.Collections.Generic;
using System.Linq;

namespace TinyLogTests
{
    [TestClass]
    public class SqlLogReaderTests : SqlLogBaseClass
    {
        [TestInitialize]
        public void Initialize()
        {
            log.RegisterLogFormatter(new ExceptionFormatter());
            log.RegisterLogFormatter(new JsonSerializationFormatter());
            log.RegisterLogReader(new SqlLogReader(ConnectionString));
        }


        [TestMethod]
        public void ReadAllLogs()
        {
            IEnumerable<LogEntry> result = log.ReadLogEntries(LogEntryFilter.Create(), -1);
            Console.WriteLine("Retrieved {0} log entries", result.Count());
        }

        [TestMethod]
        public void Read25LogEntries()
        {
            IEnumerable<LogEntry> result = log.ReadLogEntries(LogEntryFilter.Create(), 25);
            int i = result.Count();
            Console.WriteLine("Retrieved {0} log entries", i);
            Assert.IsTrue(i == 25);
        }

        [TestMethod]
        public async Task ReadBySeverity()
        {
            IEnumerable<LogEntry> result = await log.ReadLogEntriesAsync(LogEntryFilter.Create(),-1);
            int total = result.Count();
            Console.WriteLine("Retrieved {0} log entries total", total);
            int i = 0;
            foreach (LogEntrySeverity sev in Enum.GetValues(typeof(LogEntrySeverity)))
            {
                int x = await CountBySeverity(sev, total);
                Console.WriteLine("Retrieved {0} log entries with severity '{1}'", x, sev);
                i += x;
            }
            Assert.IsTrue(total == i);
        }

        private async Task<int> CountBySeverity(LogEntrySeverity sev, int max)
        {
            IEnumerable<LogEntry> result = await log.ReadLogEntriesAsync(new LogEntryFilter() { SeveritiesFilter = new LogEntrySeverity[] { sev } }, max);
            return result.Count();
        }
    }
}
