using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyLog;
using TinyLog.Formatters;
using static TinyLogTests.LogHelpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TinyLogTests
{
    public class LogObject
    {
        public int Number { get; set; } = 1;

        public string Name
        {
            get
            {
                throw new InvalidOperationException("Name::Get throws an exception on purpose");
            }

            set
            {
                _Name = value;
            }
        }
        private string _Name = "";
    }

    [TestClass]
    public class JsonSerilizationFormatterExceptionTests : FileLogBaseClass
    {
        [TestInitialize]
        public void initialize()
        {
            log.RegisterLogFormatter(new JsonSerializationFormatter() { ThrowSerializationExceptions = false });
        }

        [TestMethod]
        public async Task WriteLogEntryWithSerializationException()
        {
            LogEntry entry = LogEntry.Create("WriteLogEntryWithSerializationException", "Message", LogEntrySourceDefaults.Log, "WriteLogEntryWithSerializationException", LogHelpers.RandomSeverity);
            bool b = await log.WriteLogEntryAsync<LogObject>(entry, new LogObject());
            Assert.IsTrue(entry.CustomData.Contains("Exception"));
            Console.WriteLine("Custom data Exception:\r\n{0}", entry.CustomData);
        }


    }
}
