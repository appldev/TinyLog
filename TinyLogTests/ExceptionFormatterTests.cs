using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyLog.Formatters;
using System.Threading.Tasks;

namespace TinyLogTests
{
    [TestClass]
    public class ExceptionFormatterTests : FileLogBaseClass
    {
        [TestInitialize]
        public void initialize()
        {
            log.RegisterLogFormatter(new ExceptionFormatter());
        }

        [TestMethod]
        public async Task Write100Exceptions_ExceptionFormatter()
        {
            int num = 0;
            for (int i = 0; i < 100; i++)
            {
                bool b = await LogHelpers.WriteException(log, new Exception("Exception #" + i.ToString()));
                num += b ? 1 : 0;
            }
            Console.WriteLine("{0} exceptions written to {1}", num, FullPath);
            Assert.IsTrue(num == 100);
        }

        [TestMethod]
        public async Task Write100AggregateExceptions_ExceptionFormatter()
        {
            int num = 0;
            for (int i = 0; i < 100; i++)
            {
                bool b = await LogHelpers.WriteAggregateException(log);
                num += b ? 1 : 0;
            }
            Assert.IsTrue(num == 100);
            Console.WriteLine("{0} exceptions written to {1}", num, FullPath);
        }
    }
}
