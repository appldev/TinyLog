using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyLog;
using TinyLog.Formatters;
using System.Threading.Tasks;

namespace TinyLogTests
{

    [TestClass]
    public class SqlLazyLogWriterTests : SqlLazyLogBaseClass
    {
        [TestInitialize]
        public void Initialize()
        {
            log.RegisterLogFormatter(new ExceptionFormatter());
            log.RegisterLogFormatter(new JsonSerializationFormatter());
        }

        [TestMethod]
        public async Task Write100AggregateExceptions_SqlLazyLogWriterTests()
        {
            DateTime dt = DateTime.Now;
            int num = 0;

            for (int i = 0; i < 100; i++)
            {
                bool b = await LogHelpers.WriteAggregateException(log);
                num += b ? 1 : 0;
            }
            Console.WriteLine("{0} exceptions written in {1}ms", num, (DateTime.Now - dt).TotalMilliseconds);
            Assert.IsTrue(num == 100);
            Console.WriteLine("{0} exceptions written to {1}", num, DbPath);
        }

        [TestMethod]
        [TestCategory("High Volume")]
        public async Task Write2000Exceptions_SqlLazyLogWriterTests()
        {
            DateTime dt = DateTime.Now;
            int num = 0;
            for (int i = 0; i < 2000; i++)
            {
                bool b = await LogHelpers.WriteException(log, new Exception("Exception #" + i.ToString()));
                num += b ? 1 : 0;
            }
            Console.WriteLine("{0} exceptions written in {1}ms", num, (DateTime.Now - dt).TotalMilliseconds);
            Console.WriteLine("{0} exceptions written to {1}", num, DbPath);
            Assert.IsTrue(num == 2000);
        }
    }
}


