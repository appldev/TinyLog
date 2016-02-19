using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TinyLog.Formatters;
using TinyLog.Writers;

namespace TinyLog.MvcTestWebsite
{
    public class TinyLogConfig
    {
        public static void Initialize()
        {
            string ConnectionString = "Data Source=(localdb)\\ProjectsV12;Initial Catalog=TinyLogDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            Log.Default.RegisterLogWriter(new LazyLogWriter(new SqlLogWriter(ConnectionString),15000));
            Log.Default.RegisterLogFormatter(new JsonSerializationFormatter());
            Log.Default.RegisterLogFormatter(new ActionFilterLogFormatter());
            Log.Default.RegisterLogFormatter(new ExceptionFormatter());
        }
    }
}