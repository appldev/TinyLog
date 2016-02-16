using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyLog.Formatters;
using System.Threading.Tasks;
using TinyLog;
using System.Collections.Generic;
using static TinyLogTests.LogHelpers;
using TinyLog.Writers;

namespace TinyLogTests
{
    [TestClass]
    public class AllWritersTests : AllWritersBaseClass
    {
        [TestInitialize]
        public void initialize()
        {
            log.RegisterLogFormatter(new ExceptionFormatter());
            log.RegisterLogFormatter(new JsonSerializationFormatter());
        }

        [TestMethod]
        public async Task Write100LogEntriesInFileMsMqAndSqlLogs_AllWritersTests()
        {
            int num = 0;
            for (int i = 0; i < 100; i++)
            {
                LogEntry entry = LogEntry.Create(string.Format("Company Entry #{0}", i), "", LogEntrySourceDefaults.Log, "JsonSerializationFormatterTests", LogHelpers.RandomSeverity);
                List<Company> list = LogHelpers.GetCompanies(random.Next(1, 50), random.Next(1, 10));
                bool b = await log.WriteLogEntryAsync<List<Company>>(entry, list);
                num += b ? 1 : 0;
            }

            int MSMQ = MessageQueueLogWriter.Count(MsMqLogPath) - 1;
            Assert.IsTrue(num == 100);
            Assert.IsTrue(num == MSMQ);
        }

        
    }
}
