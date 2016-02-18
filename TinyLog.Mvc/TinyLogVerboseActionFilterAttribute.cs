using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TinyLog.CustomData;

namespace TinyLog.Mvc
{
    public class TinyLogVerboseActionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Verbose logs on controller action events. Can be applied globally, to specific controller or specific actions
        /// </summary>
        /// <param name="verboseLogEvents">The events to log. The default is all events</param>
        /// <param name="verboseLog">a custom log to use. If not specified the TinyLog.Log.Default log will be used</param>
        public TinyLogVerboseActionFilterAttribute(VerboseLogEvent verboseLogEvents = VerboseLogEvent.All, Log verboseLog = null)
        {
            VerboseLogEvents = verboseLogEvents;
            log = verboseLog ?? TinyLog.Log.Default;
        }

        [Flags]
        public enum VerboseLogEvent
        {
            OnActionExecuting,
            OnActionExecuted,
            OnResultExecuting,
            OnResultExecuted,
            All = OnActionExecuting | OnActionExecuted | OnResultExecuting | OnResultExecuted
        }
        public VerboseLogEvent VerboseLogEvents {get; set;} = VerboseLogEvent.All;


        private Log log = null;
        private LogEntry FromContext(ControllerContext context, string title)
        {
            object source = context.RouteData.Values["controller"] ?? context.Controller.GetType().Name;
            object area = context.RouteData.Values["action"] ?? "";
            LogEntry entry = LogEntry.Verbose((string)source, (string)area, title);
            entry.CorrelationId = (Guid?)context.HttpContext.Items["ActionFilterAttributeCorrelationId"];
            return entry;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            filterContext.HttpContext.Items["ActionFilterAttributeCorrelationId"] = Guid.NewGuid();
            if ((VerboseLogEvents & VerboseLogEvent.OnActionExecuting) == VerboseLogEvent.OnActionExecuting)
            {
                log.WriteLogEntry<ActionFilterCustomData>(FromContext(filterContext.Controller.ControllerContext, "OnActionExecuting"), ActionFilterCustomData.FromActionExecuting(filterContext));
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if ((VerboseLogEvents & VerboseLogEvent.OnActionExecuted) == VerboseLogEvent.OnActionExecuted)
            {
                log.WriteLogEntry<ActionFilterCustomData>(FromContext(filterContext.Controller.ControllerContext, "OnActionExecuted"), ActionFilterCustomData.FromActionExecuted(filterContext));
            }
        }
        
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if ((VerboseLogEvents & VerboseLogEvent.OnResultExecuting) == VerboseLogEvent.OnResultExecuting)
            {
                log.WriteLogEntry<ActionFilterCustomData>(FromContext(filterContext.Controller.ControllerContext, "OnResultExecuting"), ActionFilterCustomData.FromResultExecuting(filterContext));
            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if ((VerboseLogEvents & VerboseLogEvent.OnResultExecuted) == VerboseLogEvent.OnResultExecuted)
            {
                log.WriteLogEntry<ActionFilterCustomData>(FromContext(filterContext.Controller.ControllerContext, "OnResultExecuted"), ActionFilterCustomData.FromResultExecuted(filterContext));
            }
        }


    }
}
