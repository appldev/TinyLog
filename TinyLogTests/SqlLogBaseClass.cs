using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TinyLog;
using TinySql.Writers;

namespace TinyLogTests
{
    [TestClass]
    public class SqlLogBaseClass
    {
        public SqlLogBaseClass()
        {
            string path = Assembly.GetExecutingAssembly().Location.Replace(Path.GetFileName(Assembly.GetExecutingAssembly().Location), "");
            DirectoryInfo dir = new DirectoryInfo(path).Parent.Parent.Parent;

            DbPath = Path.Combine(dir.FullName, "TinyLog.Sql\\Database\\TinyLogDb.mdf").ToUpper();
            // Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=C:\GITHUB\TINYLOG\TINYSQL.SQL\DATABASE\TINYLOGDB.MDF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False
            ConnectionString = string.Format(ConnectionString, Path.GetFileNameWithoutExtension(DbPath));
        }
        public SqlLogBaseClass(LogEntryFilter filter = null) : this()
        {
            Filter = filter;
        }

        public string DbPath = "";
        // public string ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True";
        public string ConnectionString = "Data Source=(localdb)\\ProjectsV12;Initial Catalog={0};Integrated Security=True;Connect Timeout=30;Encrypt=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";


        public LogEntryFilter Filter = null;
        public Random random = new Random();

        public Log log { get; set; }


        [TestInitialize]
        public void InitializeLog()
        {
            Console.WriteLine("initializing the log: {0}", DbPath);
            log = Log.Create(new List<LogWriter>() { new SqlLogWriter(ConnectionString) });
        }




    }
}
