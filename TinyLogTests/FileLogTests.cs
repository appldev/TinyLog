using System;
using TinyLog;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TinyLogTests
{
    [TestClass]
    public class FileLogTests
    {
        private string LogFile = "TinyLog" + DateTime.Today.Year.ToString() + DateTime.Today.Month.ToString("00") + DateTime.Today.Day.ToString("00") + ".csv";
        [TestInitialize]
        public void Initialize()
        {
            //if (System.IO.File.Exists("C:\\temp\\TinyLog\\" + LogFile))
            //{
            //    System.IO.File.Delete("C:\\temp\\TinyLog\\" + LogFile);
            //}
            Log.Default.RegisterLogFormatter(new TinyLog.Formatters.XmlSerializationFormatter());
            Log.Default.RegisterLogFormatter(new TinyLog.Formatters.ExceptionFormatter());
            Log.Default.RegisterLogWriter(new TinyLog.Writers.FileLogWriter("C:\\temp\\TinyLog", "TinyLog.csv"));
        }
        [TestMethod]
        public void TestExceptionLogging()
        {
            TinyLog.Log.Default.RegisterLogSubscriber(new TinyLog.Subscribers.XmlFileSubscriber("C:\\temp\\TinyLog\\XmlFiles"));

            InvalidOperationException ex = new InvalidOperationException("This is an invalid operation");
            Exception ex2 = new Exception("This is a generic exception with an inner exception",
                new ArgumentException("The is the inner exception"));

            int numLogs = 100;

            Parallel.For(0, numLogs, i =>
             {
                 Assert.IsTrue(TinyLog.Log.Default.WriteLogEntry<Exception>(LogEntry.Create("Log Title", "Log message"), ex));
                 Assert.IsTrue(TinyLog.Log.Default.WriteLogEntry<Exception>(LogEntry.Create("Log Title", "Log message"), ex2));
             });

        }

        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }


        [TestMethod]
        public void TestObjectLogging()
        {
            string emailFile = "C:\\temp\\TinyLog\\emailsettings.txt";
            if (System.IO.File.Exists(emailFile))
            {
                string line = System.IO.File.ReadAllText(emailFile);
                if (!string.IsNullOrEmpty(line))
                {
                    // host,port,ssl,username,password,domain,fromaddress,toaddress,subjectprefix
                    string[] email = line.Split(',');
                    if (email.Length == 9)
                    {
                        TinyLog.Log.Default.RegisterLogSubscriber(new TinyLog.Subscribers.EmailSubscriber(
    email[0], Convert.ToInt32(email[1]), email[2].Equals("true"), email[3], email[4], email[5], email[6], email[7], email[8]));
                    }
                }
            }


            List<Person> People = new List<Person>()
            {
                new Person() {Name = "John", Age = 20 },
                new Person() {Name = "Mary", Age = 31 }

            };

            Assert.IsTrue(TinyLog.Log.Default.WriteLogEntry<List<Person>>(LogEntry.Create("List<Person> being logged", "Contains 2 People"), People));
        }

        [TestMethod]
        public void TestObjectLoggingWithLogSubscriber()
        {
            string emailFile = "C:\\temp\\TinyLog\\emailsettings.txt";
            if (System.IO.File.Exists(emailFile))
            {
                string line = System.IO.File.ReadAllText(emailFile);
                if (!string.IsNullOrEmpty(line))
                {
                    // host,port,ssl,username,password,domain,fromaddress,toaddress,subjectprefix
                    string[] email = line.Split(',');
                    if (email.Length == 9)
                    {
                        TinyLog.Log.Default.RegisterLogSubscriber(new TinyLog.Subscribers.EmailSubscriber(email[0], Convert.ToInt32(email[1]), email[2].Equals("true"), 
                            email[3], email[4], email[5], email[6], email[7], email[8], LogEntryFilter.Create(severities: new LogEntrySeverity[] { LogEntrySeverity.Error, LogEntrySeverity.Critical })));
                    }
                }
            }
            

            TinyLog.Log.Default.RegisterLogSubscriber(new TinyLog.Subscribers.XmlFileSubscriber("C:\\temp\\TinyLog\\XmlFiles"));

            List<Person> People = new List<Person>()
            {
                new Person() {Name = "John", Age = 20 },
                new Person() {Name = "Mary", Age = 31 }

            };

            Assert.IsTrue(TinyLog.Log.Default.WriteLogEntry<List<Person>>(LogEntry.Create("List<Person> being logged as normal", "Contains 2 People"), People));
            Assert.IsTrue(TinyLog.Log.Default.WriteLogEntry<List<Person>>(LogEntry.Create("List<Person> being logged as CRITICAL", "Contains 2 People", LogEntrySeverity.Critical), People));
        }
    }
}
