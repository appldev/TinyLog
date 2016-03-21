using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyLog.Formatters;
using System.Threading.Tasks;
using TinyLog;
using System.Collections.Generic;
using static TinyLogTests.LogHelpers;

namespace TinyLogTests
{
    [TestClass]
    public class StorageTableWriterTests : AzureStorageBaseClass
    {
        public StorageTableWriterTests()
        {
            CreateQueueIfNotExists = true;
            LogPath = "TinyLogTable";
            IsTable = true;
        }



        [TestInitialize]
        [TestCategory("Local")]
        public void initialize()
        {
            log.RegisterLogFormatter(new ExceptionFormatter());
            log.RegisterLogFormatter(new JsonSerializationFormatter(true));
        }


        [TestMethod]
        [TestCategory("Local")]
        public async Task StorageTableWriterTests_Write100Exceptions()
        {
            int num = 0;
            for (int i = 0; i < 100; i++)
            {
                bool b = await LogHelpers.WriteException(log, new Exception("Exception #" + i.ToString()));
                num += b ? 1 : 0;
            }
            Console.WriteLine("{0} exceptions written to {1}", num, LogPath);
            Assert.IsTrue(num == 100);
        }

        [TestMethod]
        [TestCategory("Local")]
        public async Task StorageTableWriterTests_Write100LogEntries()
        {
            int num = 0;
            for (int i = 0; i < 100; i++)
            {
                LogEntry entry = LogEntry.Create(string.Format("Company Entry #{0}", i), "", LogEntrySourceDefaults.Log, "ServiceBusQueueWriterTests", LogHelpers.RandomSeverity);
                List<Company> list = LogHelpers.GetCompanies(random.Next(1, 15), random.Next(1, 5));
                bool b = await log.WriteLogEntryAsync<List<Company>>(entry, list);
                num += b ? 1 : 0;
            }
            Assert.IsTrue(num == 100);
            Console.WriteLine("{0} company lists written to {1}", num, LogPath);
        }
    }
}
