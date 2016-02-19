using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TinyLog.CustomData;

namespace TinyLog.Mvc
{
    /// <summary>
    /// Contains static helper methods for setting up TinyLog for an MVC Web site
    /// </summary>
    public static class Helpers
    {
        #region Route Helpers

        private static Route _TinyLogErrorRoute = null;

        /// <summary>
        /// get or sets the standard TinyLog MVC error route
        /// </summary>
        public static Route TinyLogErrorRoute
        {
            get
            {
                if (_TinyLogErrorRoute == null)
                {
                    _TinyLogErrorRoute = new Route("error/{action}/{id}", new System.Web.Mvc.MvcRouteHandler())
                    {
                        Defaults = new RouteValueDictionary()
                        {
                            {"controller", "TinyLogError"},
                        {"action", "Index"},
                        {"id", UrlParameter.Optional}
                        },
                        DataTokens = new RouteValueDictionary()
                        {
                            {"namespaces", new string[] { "TinyLog.Mvc.Controllers" } }
                        }
                    };
                }
                return _TinyLogErrorRoute;
            }
            set
            {
                _TinyLogErrorRoute = value;
            }
        }

        /// <summary>
        /// Registers the TinyLog error routes at the top of the routing table
        /// </summary>
        /// <param name="routes">The RouteCollection</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.Insert(1, TinyLogErrorRoute);
        }

        #endregion

        #region Filter Helpers
        /// <summary>
        /// Registers the TinyLog HandleError filter as a global error handler
        /// </summary>
        /// <param name="filters">The GlobalFiltersCollection</param>
        /// /// <param name="registerGlobalVerboseFilter">true if the Verbose Action filter should be registered as a global filter. The default is false.</param>
        /// <param name="errorView">The path to the view file to route all errors to. If not specified, the default error view from the TinyLogErrorController will be used</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters, bool registerGlobalVerboseFilter = false, string errorView = null)
        {
            filters.Add(new TinyLogHandleErrorAttribute() { View = errorView ?? Controllers.TinyLogErrorController.ErrorViews[0] });
            if (registerGlobalVerboseFilter)
            {
                filters.Add(new TinyLogVerboseActionFilterAttribute());
            }
            
        }

        #endregion

        #region Application_Error handler
        /// <summary>
        /// This method should be called from the protected void Application_Error() function in global.asax.
        /// It will take care of logging ASP.NET level exceptions.
        /// </summary>
        /// <param name="sender">the event sender (global.asax)</param>
        /// <param name="e">Event arguments (empty)</param>
        public static void Application_Error(object sender, EventArgs e)
        {
            Exception ex = HttpContext.Current.Server.GetLastError();
            if (ex != null)
            {
                if (ex is HttpException)
                {
                    HttpContext.Current.Response.StatusCode = (ex as HttpException).GetHttpCode();
                }
                LogEntry logEntry = LogEntry.Critical(string.Format("ASP.NET ERROR/{0}", HttpContext.Current.Server.MachineName), ex.Message, LogEntrySourceDefaults.ASPNETWebServer, Convert.ToString(sender));
                Log.Default.WriteLogEntry<ActionFilterCustomData>(logEntry, ActionFilterCustomData.FromHttpContext(HttpContext.Current));
                if (HttpContext.Current.Session != null)
                {
                    TinyLogHandleErrorInfo model = new TinyLogHandleErrorInfo()
                    {
                        LogEntry = logEntry,
                        StatusCode = HttpContext.Current.Response.StatusCode
                    };
                    HttpContext.Current.Session["LASTERROR"] = model;
                }
                
                
                HttpContext.Current.Server.TransferRequest("~/error/index/" + HttpContext.Current.Response.StatusCode.ToString(), true);
            }
        }
        #endregion

        
    }
}
