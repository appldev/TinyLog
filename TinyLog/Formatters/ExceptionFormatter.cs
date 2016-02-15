using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyLog.Formatters
{
    /// <summary>
    /// Formats an Exception into a Log entry
    /// </summary>
    public class ExceptionFormatter : LogFormatter
    {
        public ExceptionFormatter() : base(typeof(Exception), true)
        {
        }

        protected override void FormatLogEntry(LogEntry entry, object customData)
        {
            Exception ex = (Exception)customData;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("[{0}]: {1}\r\n", ex.GetType().Name, ex.Message);
            Exception exInner = ex.InnerException;
            while (exInner != null)
            {
                sb.AppendFormat("\r\n[{0}]: {1}\r\n", exInner.GetType().Name, exInner.Message);
                exInner = exInner.InnerException;
            }
            sb.AppendFormat("\r\nComplete stack trace:\r\n{0}", ex.StackTrace);
            entry.CustomData = sb.ToString();
        }
    }
}
