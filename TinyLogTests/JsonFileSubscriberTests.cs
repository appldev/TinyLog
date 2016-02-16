using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyLog;
using TinyLog.Formatters;
using TinyLog.Subscribers;
using static TinyLogTests.LogHelpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace TinyLogTests
{
    [TestClass]
    public class JsonFileSubscriberTests: FileLogBaseClass
    {
        private string path = Path.Combine(Path.GetTempPath(), "JsonFileSubcriber_" + Path.GetRandomFileName());
        [TestInitialize]
        public void initialize()
        {
            log.RegisterLogFormatter(new JsonSerializationFormatter(false));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            log.RegisterLogSubscriber(new JsonFileSubcriber(path));
        }

        [TestCleanup]
        public void CleanUp()
        {
            string[] files = Directory.GetFiles(path);
            files.ToList().ForEach(file =>
            {
                File.Delete(file);
            });
            Directory.Delete(path);
        }
        [TestMethod]
        public async Task Write100LogEntries_JsonFileSubscriberTests()
        {
            int num = 0;
            for (int i = 0; i < 100; i++)
            {
                LogEntry entry = LogEntry.Create(string.Format("Company Entry #{0}", i), "", LogEntrySourceDefaults.Log, "JsonSerializationFormatterTests", LogHelpers.RandomSeverity);
                List<Company> list = LogHelpers.GetCompanies(random.Next(1, 50), random.Next(1, 10));
                bool b = await log.WriteLogEntryAsync<List<Company>>(entry, list);
                num += b ? 1 : 0;
            }
            Assert.IsTrue(num == 100);
            IEnumerable<string> files = Directory.EnumerateFiles(path);
            Assert.IsTrue(files.Count() == num);
        }
    }
}
