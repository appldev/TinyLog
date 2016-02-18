using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyLog.Mvc
{
    public class TinyLogHandleErrorInfo
    {
        public LogEntry LogEntry { get; set; } = new LogEntry();

        public int StatusCode { get; set; } = 0;
    }
}
