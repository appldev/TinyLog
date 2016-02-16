using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyLog;
using TinyLog.Formatters;
using System.Threading.Tasks;

namespace TinyLogTests
{
    [TestClass]
    public class SqlLogWriterTests : SqlLogBaseClass
    {
        [TestInitialize]
        public void Initialize()
        {
            log.RegisterLogFormatter(new ExceptionFormatter());
            log.RegisterLogFormatter(new JsonSerializationFormatter());
        }

        [TestMethod]
        public async Task Write100AggregateExceptions_SqlLogWriterTests()
        {
            int num = 0;
            for (int i = 0; i < 100; i++)
            {
                bool b = await LogHelpers.WriteAggregateException(log);
                num += b ? 1 : 0;
            }
            Assert.IsTrue(num == 100);
            Console.WriteLine("{0} exceptions written to {1}", num, DbPath);
        }

        [TestMethod]
        public async Task Write100Exceptions_SqlLogWriterTests()
        {
            int num = 0;
            for (int i = 0; i < 100; i++)
            {
                bool b = await LogHelpers.WriteException(log, new Exception("Exception #" + i.ToString()));
                num += b ? 1 : 0;
            }
            Console.WriteLine("{0} exceptions written to {1}", num, DbPath);
            Assert.IsTrue(num == 100);
        }
    }
}
