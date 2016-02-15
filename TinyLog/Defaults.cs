using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyLog
{
    public static class LogEntrySourceDefaults
    {
        /// <summary>
        /// Logs coming from TinyLog itself (Indented for internal use only)
        /// </summary>
        public static readonly string Log = "TinyLog";
        /// <summary>
        /// General Application logs
        /// </summary>
        public static readonly string Application = "Application";


    }

    public static class LogAreaDefaults
    { 
    
        /// <summary>
        /// Logs related to LogWriters (Indented for internal use only)
        /// </summary>
        public static readonly string LogWriter = "LogWriter";
        /// <summary>
        /// Logs related to the LogFormatters (Indented for internal use only)
        /// </summary>
        public static readonly string LogFormatter = "LogFormatter";
        /// <summary>
        /// Logs related to the LogSubscribers (Indented for internal use only)
        /// </summary>
        public static readonly string LogSubscriber = "LogSubscriber";
    }
}
