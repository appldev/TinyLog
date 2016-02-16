using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TinyLog;
using TinyLog.Writers;
using TinyLog.Writers;

namespace TinyLogTests
{
    [TestClass]
    public class AllWritersBaseClass
    {
        public AllWritersBaseClass()
        {
            string path = Assembly.GetExecutingAssembly().Location.Replace(Path.GetFileName(Assembly.GetExecutingAssembly().Location), "");
            DirectoryInfo dir = new DirectoryInfo(path).Parent.Parent.Parent;

            DbPath = Path.Combine(dir.FullName, "TinyLog.Sql\\Database\\TinyLogDb.mdf").ToUpper();
            // Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=C:\GITHUB\TINYLOG\TINYSQL.SQL\DATABASE\TINYLOGDB.MDF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False
            ConnectionString = string.Format(ConnectionString, Path.GetFileNameWithoutExtension(DbPath));

        }
        public AllWritersBaseClass(LogEntryFilter filter = null) : this()
        {
            Filter = filter;
        }
        public string FileLogPath = Path.GetTempPath();
        public string MsMqLogPath = ".\\Private$\\" + Path.GetRandomFileName();
        public string LogName = "TinyLogTest_" + Path.GetRandomFileName() + ".txt";
        public string ConnectionString = "Data Source=(localdb)\\ProjectsV12;Initial Catalog={0};Integrated Security=True;Connect Timeout=30;Encrypt=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public string DbPath = "";
        public LogEntryFilter Filter = null;
        public Random random = new Random();
        public string FullPath
        {
            get
            {
                return Path.Combine(FileLogPath, LogName);
            }
        }

        public Log log { get; set; }


        [TestInitialize]
        public void InitializeLog()
        {
            Console.WriteLine("initializing the log: {0}", FullPath);
            log = Log.Create(new List<LogWriter>() {
                new FileLogWriter(FileLogPath, LogName, false, Filter),
                new MessageQueueLogWriter(MsMqLogPath, Filter),
                new SqlLogWriter(ConnectionString)
        });
        }

        [TestCleanup]
        public void CleanUp()
        {
            MessageQueueLogWriter.Delete(MsMqLogPath);
        }

    }
}
