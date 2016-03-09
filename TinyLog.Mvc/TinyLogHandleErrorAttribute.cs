using System;
using System.Web.Mvc;
using TinyLog.CustomData;
using TinyLog.CustomData.Mvc;

namespace TinyLog.Mvc
{
    /// <summary>
    /// A TinyLog specific HandleErrorAttribute. 
    /// </summary>
    public class TinyLogHandleErrorAttribute : HandleErrorAttribute
    {
        private LogEntry _DefaultLogEntry = new LogEntry();

        /// <summary>
        /// A log entry that contains defaults for Title, Message, Source and Area. If these values are not specified, the values will be generated from the Error
        /// </summary>
        public LogEntry DefaultLogEntry
        {
            get
            {
                return _DefaultLogEntry;
            }

            set
            {
                _DefaultLogEntry = value;
            }
        }

        private static bool IsBot(System.Web.HttpContextBase context)
        {
            return context.Request.UserAgent.ToLower().Contains("bot");
        }

        public bool LogCrawlers { get; set; } = false;

        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            filterContext.HttpContext.Response.StatusCode = 500;
            
            string title = DefaultLogEntry.Title ?? (filterContext.Exception != null ? filterContext.Exception.GetType().Name : "Unhandled error");
            string message = DefaultLogEntry.Message ?? (filterContext.Exception != null ? filterContext.Exception.Message : null);
            string source = DefaultLogEntry.Source ?? LogEntrySourceDefaults.MVCController;
            object area = DefaultLogEntry.Area ?? ((filterContext.RouteData.Values["area"] ?? "") + "/" + (filterContext.RouteData.Values["controller"] ?? filterContext.Controller.GetType().Name) + "/" + (filterContext.RouteData.Values["action"] ?? ""));
            
            LogEntry entry = LogEntry.Error(title, message, source, (string)area);
            entry.CorrelationId = (Guid?)filterContext.HttpContext.Items["ActionFilterAttributeCorrelationId"];

            ViewResult vr = new ViewResult()
            {
                 ViewName = this.View,
                 ViewData = new ViewDataDictionary(new TinyLogHandleErrorInfo() { LogEntry = entry, StatusCode = 500 })
                 {
                     { "StatusCode",500 },
                     {"LogEntry",entry }
                 }
            };
            filterContext.Result = vr;

            bool handled = true;
            if (LogCrawlers || (!LogCrawlers && !filterContext.HttpContext.Request.Browser.Crawler && !IsBot(filterContext.HttpContext)))
            {
                handled = TinyLog.Log.Default.WriteLogEntry<ActionFilterCustomData>(entry, ActionFilterCustomData.FromExceptionContext(filterContext));
            }

            filterContext.ExceptionHandled = handled;
        }
    }
}
