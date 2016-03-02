using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TinyLog.CustomData;
using TinyLog.CustomData.Mvc;

namespace TinyLog.Mvc.Controllers
{
    /// <summary>
    /// Shorthand methods for adding error views to the Error controller
    /// </summary>
    public static class ErrorControllerExtensions
    {
        public static TinyLogErrorController RegisterErrorView(this TinyLogErrorController controller, int HttpStatusCode, string viewPath)
        {
            if (string.IsNullOrEmpty(viewPath))
            {
                throw new ArgumentNullException("viewPath");
            }
            TinyLogErrorController.ErrorViews[HttpStatusCode] = viewPath;
            return controller;
        }

        public static TinyLogErrorController RegisterDefaultErrorView(this TinyLogErrorController controller, string viewPath)
        {
            return RegisterErrorView(controller, 0, viewPath);
        }
    }
    public class TinyLogErrorController : Controller
    {
        /// <summary>
        /// Contains the default views that will be used for each http error status code.
        /// The status code 0 is used to store the default error view.
        /// </summary>
        public static Dictionary<int, string> ErrorViews { get; set; } = new Dictionary<int, string>()
        {
            {0, "~/views/shared/error.cshtml" }
        };

        /// <summary>
        /// This is the default title for all IIS Server errors. {0}, if specified,  will be replaced by the machine name
        /// </summary>
        public static string DefaultIISErrorLogTitle { get; set; } = "IIS ERROR/{0}";


        public ActionResult IIS(int? Id)
        {
            Exception ex = Server.GetLastError() ?? new Exception(string.Format("An unhandled Http {0} error occured", Id ?? 0));
            LogEntry logEntry = LogEntry.Error(DefaultIISErrorLogTitle.Contains("{0}") ? string.Format("IIS ERROR/{0}", Server.MachineName) : DefaultIISErrorLogTitle, ex.Message, TinyLog.LogEntrySourceDefaults.IISWebServer, null);
            logEntry.CorrelationId = (Guid?)HttpContext.Items["ActionFilterAttributeCorrelationId"];
            ActionFilterCustomData customData = ActionFilterCustomData.FromHttpContext(System.Web.HttpContext.Current);
            customData.HttpContext.Exception = customData.HttpContext.Exception ?? ex;
            Log.Default.WriteLogEntry<ActionFilterCustomData>(logEntry, customData);
            TinyLogHandleErrorInfo model = new TinyLogHandleErrorInfo()
            {
                LogEntry = logEntry,
                StatusCode = Id ?? 0
            };
            return View(GetErrorViewPath(Id),model);
        }

        public ActionResult Index(int? Id)
        {
            TinyLogHandleErrorInfo model = Session["LASTERROR"] as TinyLogHandleErrorInfo;
            if (model == null)
            {
                model = new TinyLogHandleErrorInfo();
            }
            else
            {
                Session.Remove("LASTERROR");
            }
            return View(GetErrorViewPath(Id),model);
        }

        private string GetErrorViewPath(int? Id)
        {
            string viewPath = Id.HasValue && ErrorViews.ContainsKey(Id.Value) ? ErrorViews[Id.Value] : null;
            return viewPath ?? ErrorViews[0];
        }

    }
}