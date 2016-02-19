using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyLog;

namespace TinyLogTests
{
    public static class LogHelpers
    {
        public class Company
        {
            public string Name { get; set; }
            public DateTime CreatedOn { get; set; } = DateTime.Now;
            public List<Person> People { get; set; } = new List<Person>();
        }

        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }



        public static List<Company> GetCompanies(int number, int peoplePerCompany)
        {
            List<Company> list = new List<Company>();
            Random r = new Random();
            for (int i = 0; i < number; i++)
            {
                Company c = new Company()
                {
                    Name = string.Format("Company #{0}", i)
                };
                for (int x = 0; x < peoplePerCompany; x++)
                {
                    c.People.Add(new Person()
                    {
                        Name = string.Format("Person #{0} in Company #{1}", x, i),
                        Age = r.Next(18, 65)
                    });
                }
                list.Add(c);
            }
            return list;
        }

        private static Random r = new Random();
        public static LogEntrySeverity RandomSeverity
        {
            get
            {
                return (LogEntrySeverity)Convert.ToInt32((r.NextDouble() * 4) + 1);
            }
        }
        public static async Task<bool> WriteException(Log log, Exception ex, LogEntrySeverity? severity = null)
        {
            LogEntry entry = LogEntry.Create(ex.Message, ex.Message, severity ?? RandomSeverity);
            return await log.WriteLogEntryAsync<Exception>(entry, ex);
        }

        public static List<LogEntry> CreateLogEntries()
        {
            List<LogEntry> entries = new List<LogEntry>();
            entries.Add(LogEntry.Create("Create Title", "Create Message"));
            entries.Add(LogEntry.Create("Create Title", "Create Message", LogEntrySeverity.Verbose));
            entries.Add(LogEntry.Create("Create Title", "Create Message", "Create Source", "Create Area"));
            entries.Add(LogEntry.Create("Create Title", "Create Message", "Create Source", "Create Area", LogEntrySeverity.Warning));
            entries.Add(LogEntry.Verbose("Verbose source", "Verbose area"));
            entries.Add(LogEntry.Verbose("Verbose source", "Verbose area", "Verbose Title"));
            entries.Add(LogEntry.Verbose("Verbose source", "Verbose area", "Verbose Title", "Verbose Message"));
            entries.Add(LogEntry.Warning("Warning Title", "Warning Message", "Warning Source", "Warning Area"));
            entries.Add(LogEntry.Error("Error Title", "Error Message", "Error Source", "Error Area"));
            entries.Add(LogEntry.Critical("Critical Title", "Critical Message", "Critical Source", "Critical Area"));
            return entries;
        }


        public static Task<bool> WriteAggregateException(Log log)
        {

            ConcurrentQueue<Exception> exceptions = new ConcurrentQueue<Exception>();
            Parallel.For(0, 100, i =>
            {
                try
                {
                    if (i % 2 == 0)
                    {
                        exceptions.Enqueue(new Exception("Exception from number " + i.ToString()));
                    }
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });
            try
            {
                return WriteException(log, new AggregateException(string.Format("{0} aggregate exceptions", exceptions.Count), exceptions));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

    }
}
