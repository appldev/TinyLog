using System.Web;
using System.Web.Mvc;

namespace TinyLog.CustomData.Mvc
{
    /// <summary>
    /// Contains custom data about a Controller Action captured by an action filter attribute
    /// </summary>
    public class ActionFilterCustomData
    {
        public enum Details
        {
            Full,
            Minimal
        }
        public ActionFilterCustomData()
        {

        }


        public static ActionFilterCustomData FromHttpContext(HttpContext context, Details detail = Details.Minimal)
        {
            return new ActionFilterCustomData()
            {
                UserContext = UserContextCustomData.FromHttpContext(context, detail),
                RequestContext = RequestContextCustomData.FromHttpRequest(context.Request, detail),
                ResponseContext = ResponseContextCustomData.FromHttpResponse(context.Response, detail),
                HttpContext = new HttpContextCustomData()
                {
                    Headers = detail == ActionFilterCustomData.Details.Minimal ? null : context.Request.Headers,
                    Exception = context.Server.GetLastError(),
                    ExceptionHandled = false,
                    IsChildAction = false
                },
                ControllerContext = new ControllerContextCustomData()
                {
                    Session = SessionStateContextCustomData.FromHttpSessionState(context.Session, detail)
                }
            };
        }


        /// <summary>
        /// Creates an ActionFilterCustomData instance
        /// </summary>
        /// <param name="context">The context object to create the instance from</param>
        /// <returns>A new instance of the ActionFilterCustomData</returns>
        public static ActionFilterCustomData FromExceptionContext(ExceptionContext context, Details detail = Details.Minimal)
        {

            return new ActionFilterCustomData()
            {
                UserContext = UserContextCustomData.FromHttpContext(context.HttpContext,detail),
                RequestContext = RequestContextCustomData.FromHttpRequest(context.HttpContext.Request, detail),
                ResponseContext = ResponseContextCustomData.FromHttpResponse(context.HttpContext.Response, detail),
                HttpContext = new HttpContextCustomData()
                {
                    Headers = detail == ActionFilterCustomData.Details.Minimal ? null : context.HttpContext.Request.Headers,
                    Exception = context.Exception,
                    ExceptionHandled = context.ExceptionHandled,
                    IsChildAction = context.IsChildAction
                },
                ControllerContext = new ControllerContextCustomData()
                {
                    RouteValues = context.RequestContext.RouteData.Values,
                    Session = SessionStateContextCustomData.FromHttpSessionState(context.HttpContext.Session, detail),
                    ViewBag = detail == ActionFilterCustomData.Details.Minimal ? null : context.Controller.ViewBag,
                    ViewData = detail == ActionFilterCustomData.Details.Minimal ? null : context.Controller.ViewData
                }
            };
        }
        /// <summary>
        /// Creates an ActionFilterCustomData instance
        /// </summary>
        /// <param name="context">The context object to create the instance from</param>
        /// <returns>A new instance of the ActionFilterCustomData</returns>
        public static ActionFilterCustomData FromResultExecuted(ResultExecutedContext context, Details detail = Details.Minimal)
        {
            return new ActionFilterCustomData()
            {
                UserContext = UserContextCustomData.FromHttpContext(context.HttpContext,detail),
                RequestContext = RequestContextCustomData.FromHttpRequest(context.HttpContext.Request, detail),
                ResponseContext = ResponseContextCustomData.FromHttpResponse(context.HttpContext.Response, detail),
                HttpContext = new HttpContextCustomData()
                {
                    Headers = detail == ActionFilterCustomData.Details.Minimal ? null : context.HttpContext.Request.Headers,
                    Exception = context.Exception,
                    ExceptionHandled = context.ExceptionHandled,
                    IsChildAction = context.IsChildAction,
                    Canceled = context.Canceled
                },
                ControllerContext = new ControllerContextCustomData()
                {
                    RouteValues = detail == ActionFilterCustomData.Details.Minimal ? null : context.RequestContext.RouteData.Values,
                    Session = SessionStateContextCustomData.FromHttpSessionState(context.HttpContext.Session, detail),
                    ViewBag = detail == ActionFilterCustomData.Details.Minimal ? null : context.Controller.ViewBag,
                    ViewData = detail == ActionFilterCustomData.Details.Minimal ? null : context.Controller.ViewData
                }
            };
        }

        /// <summary>
        /// Creates an ActionFilterCustomData instance
        /// </summary>
        /// <param name="context">The context object to create the instance from</param>
        /// <returns>A new instance of the ActionFilterCustomData</returns>
        public static ActionFilterCustomData FromResultExecuting(ResultExecutingContext context, Details detail = Details.Minimal)
        {
            return new ActionFilterCustomData()
            {
                UserContext = UserContextCustomData.FromHttpContext(context.HttpContext, detail),
                RequestContext = RequestContextCustomData.FromHttpRequest(context.HttpContext.Request, detail),
                ResponseContext = ResponseContextCustomData.FromHttpResponse(context.HttpContext.Response, detail),
                HttpContext = new HttpContextCustomData()
                {
                    Headers = detail == ActionFilterCustomData.Details.Minimal ? null : context.HttpContext.Request.Headers,
                    IsChildAction = context.IsChildAction,
                    Cancel = context.Cancel
                },
                ControllerContext = new ControllerContextCustomData()
                {
                    RouteValues = detail == ActionFilterCustomData.Details.Minimal ? null : context.RequestContext.RouteData.Values,
                    Session = SessionStateContextCustomData.FromHttpSessionState(context.HttpContext.Session, detail),
                    ViewBag = detail == ActionFilterCustomData.Details.Minimal ? null : context.Controller.ViewBag,
                    ViewData = detail == ActionFilterCustomData.Details.Minimal ? null : context.Controller.ViewData
                }
            };
        }
        /// <summary>
        /// Creates an ActionFilterCustomData instance
        /// </summary>
        /// <param name="context">The context object to create the instance from</param>
        /// <returns>A new instance of the ActionFilterCustomData</returns>
        public static ActionFilterCustomData FromActionExecuted(ActionExecutedContext context, Details detail = Details.Minimal)
        {
            return new ActionFilterCustomData()
            {
                UserContext = UserContextCustomData.FromHttpContext(context.HttpContext,detail),
                RequestContext = RequestContextCustomData.FromHttpRequest(context.HttpContext.Request, detail),
                ResponseContext = ResponseContextCustomData.FromHttpResponse(context.HttpContext.Response, detail),
                HttpContext = new HttpContextCustomData()
                {
                    Headers = detail == ActionFilterCustomData.Details.Minimal ? null : context.HttpContext.Request.Headers,
                    Exception = context.Exception,
                    ExceptionHandled = context.ExceptionHandled,
                    IsChildAction = context.IsChildAction
                },
                ControllerContext = new ControllerContextCustomData()
                {
                    RouteValues = detail == ActionFilterCustomData.Details.Minimal ? null : context.RequestContext.RouteData.Values,
                    Session = SessionStateContextCustomData.FromHttpSessionState(context.HttpContext.Session,detail),
                    ViewBag = detail == ActionFilterCustomData.Details.Minimal ? null : context.Controller.ViewBag,
                    ViewData = detail == ActionFilterCustomData.Details.Minimal ? null : context.Controller.ViewData
                }
            };
        }
        /// <summary>
        /// Creates an ActionFilterCustomData instance
        /// </summary>
        /// <param name="context">The context object to create the instance from</param>
        /// <returns>A new instance of the ActionFilterCustomData</returns>
        public static ActionFilterCustomData FromActionExecuting(ActionExecutingContext context, Details detail = Details.Minimal)
        {
            return new ActionFilterCustomData()
            {
                UserContext = UserContextCustomData.FromHttpContext(context.HttpContext,detail),
                RequestContext = RequestContextCustomData.FromHttpRequest(context.HttpContext.Request,detail),
                ResponseContext = ResponseContextCustomData.FromHttpResponse(context.HttpContext.Response,detail),
                HttpContext = new HttpContextCustomData()
                {
                    Headers = detail == ActionFilterCustomData.Details.Minimal ? null : context.HttpContext.Request.Headers
                },
                ControllerContext = new ControllerContextCustomData()
                {
                    RouteValues = detail == ActionFilterCustomData.Details.Minimal ? null : context.RequestContext.RouteData.Values,
                    Session = SessionStateContextCustomData.FromHttpSessionState(context.HttpContext.Session, detail),
                    ViewBag = detail == ActionFilterCustomData.Details.Minimal ? null : context.Controller.ViewBag,
                    ViewData = detail == ActionFilterCustomData.Details.Minimal ? null : context.Controller.ViewData
                }
            };
        }
        /// <summary>
        /// Contains information from the Request
        /// </summary>
        public RequestContextCustomData RequestContext { get; set; }
        /// <summary>
        /// Contains information from the Response
        /// </summary>
        public ResponseContextCustomData ResponseContext { get; set; }
        /// <summary>
        /// Contains information from the MVC Controller
        /// </summary>
        public ControllerContextCustomData ControllerContext { get; set; }
        /// <summary>
        /// Contains information from the User
        /// </summary>
        public UserContextCustomData UserContext { get; set; }
        /// <summary>
        /// Contains general information from the HttpContext
        /// </summary>
        public HttpContextCustomData HttpContext { get; set; }

    }
}
