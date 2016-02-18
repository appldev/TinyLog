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
        /// <summary>
        /// An error that occured in a web client. Example: a javascript that fails, etc.
        /// </summary>
        public static readonly string WebClient = "WebClient";
        /// <summary>
        /// An error that occured on the web server within the ASP.NET process
        /// </summary>
        public static readonly string ASPNETWebServer = "ASP.NET Web Server";
        /// <summary>
        /// An error that occured on the web server within the IIS process
        /// </summary>
        public static readonly string IISWebServer = "IIS Web Server";

        public static readonly string MVCController = "MVC Controller";


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
