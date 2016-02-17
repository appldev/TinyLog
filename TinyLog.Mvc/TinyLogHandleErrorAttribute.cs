using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TinyLog.Mvc
{
    public class TinyLogHandleErrorAttribute : HandleErrorAttribute
    {
        private LogEntry _DefaultLogEntry = new LogEntry();
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            string title = _DefaultLogEntry.Title ?? (filterContext.Exception != null ? filterContext.Exception.GetType().Name : "Unhandled error");
            string message = _DefaultLogEntry.Message ?? (filterContext.Exception != null ? filterContext.Exception.Message : null);
            object source = _DefaultLogEntry.Source ?? (filterContext.RouteData.Values["controller"] ?? TinyLog.LogEntrySourceDefaults.WebServer);
            object area = _DefaultLogEntry.Area ?? (filterContext.RouteData.Values["action"] ?? TinyLog.LogAreaDefaults.LogFormatter);
            LogEntry entry = LogEntry.Error(title, message, (string)source, (string)area);
            // filterContext.Controller.ViewBag.LogEntry = entry;
            // filterContext.Controller.TempData.Add("LogEntry", entry);
            ((System.Web.Mvc.ViewResult)filterContext.Result).ViewData.Add("LogEntry", entry);
            filterContext.ExceptionHandled = TinyLog.Log.Default.WriteLogEntry<Exception>(entry, filterContext.Exception);
            
        }
    }
}
