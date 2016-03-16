using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyLog;
using TinyLog.Formatters;
using static TinyLogTests.LogHelpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TinyLogTests
{
    [TestClass]
    public class JsonSerilizationFormatterTests : FileLogBaseClass
    {
        [TestInitialize]
        public void initialize()
        {
            log.RegisterLogFormatter(new JsonSerializationFormatter());
        }

        [TestMethod]
        public async Task Write100LogEntries_JsonSerializationFormatter()
        {
            int num = 0;
            for (int i = 0; i < 100; i++)
            {
                LogEntry entry = LogEntry.Create(string.Format("Company Entry #{0}", i), "", LogEntrySourceDefaults.Log, "JsonSerializationFormatterTests", LogHelpers.RandomSeverity);
                List<Company> list = LogHelpers.GetCompanies(random.Next(1, 50), random.Next(1, 10));
                bool b = await log.WriteLogEntryAsync<List<Company>>(entry,list);
                num += b ? 1 : 0;
            }
            Assert.IsTrue(num == 100);
        }

        [TestMethod]
        public async Task WriteExceptionsAsJaon()
        {
            bool b = await LogHelpers.WriteAggregateException(log);
            Console.WriteLine("Exceptions written to {0}", FullPath);
            Assert.IsTrue(b);
        }

    }
}
