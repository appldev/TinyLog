using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyLog.Subscribers;
using TinyLog;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinyLog.Writers;

namespace TinyLogTests
{
    [TestClass]
    public class MessageQueueSubscriberTests : FileLogBaseClass
    {
        private string queueName = ".\\Private$\\" + System.IO.Path.GetRandomFileName();

        [TestInitialize]
        [TestCategory("Local")]
        public void Initialize()
        {
            log.RegisterLogSubscriber(new MessageQueueSubscriber(queueName,
                LogEntryFilter.Create(severities: new LogEntrySeverity[] { LogEntrySeverity.Error, LogEntrySeverity.Critical })));
        }

        [TestCleanup]
        [TestCategory("Local")]
        public void CleanUp()
        {
            MessageQueueLogWriter.Delete(queueName);
        }

        [TestMethod]
        [TestCategory("Local")]
        public void WriteEntriesWithErrorCriticalSubscriber()
        {
            List<LogEntry> entries = new List<LogEntry>();
            for (int i = 0; i < 100; i++)
            {
                entries.Add(LogEntry.Create("Title #" + i.ToString(), "", LogHelpers.RandomSeverity));
            }
            entries.Add(LogEntry.Error("Error", "Forced error from test", null, null));
            entries.Add(LogEntry.Critical("Critical", "Forced error from test", null, null));

            int expect = entries.Count(x => x.Severity == LogEntrySeverity.Critical || x.Severity == LogEntrySeverity.Error);
            Console.WriteLine("{0} out of 100 logs are error or critical", expect);
            int num = 0;
            entries.ForEach(async entry =>
            {
                if (await log.WriteLogEntryAsync(entry))
                {
                    num++;
                }
            });
            int count = MessageQueueLogWriter.Count(queueName) - 1;
            Console.WriteLine("{0} out of {1} logs written and {2} out of expected {3} logs was sent to the subscriber",num,entries.Count, count,expect);
            Assert.IsTrue(expect == count);
        }
    }
}
