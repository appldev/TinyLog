using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace TinyLog.CustomData
{
    /// <summary>
    /// Contains custom data about a Controller Action captured by an action filter attribute
    /// </summary>
    public class ActionFilterCustomData
    {
        public ActionFilterCustomData()
        {

        }

        public static ActionFilterCustomData FromExceptionContext(ExceptionContext context)
        {

            return new ActionFilterCustomData()
            {
                UserContext = UserContextCustomData.FromHttpContext(context.HttpContext),
                RequestContext = new RequestContextCustomData()
                {
                    Browser = (HttpBrowserCapabilitiesWrapper)context.HttpContext.Request.Browser,
                    Form = context.HttpContext.Request.Form,
                    QueryString = context.HttpContext.Request.QueryString,
                    Url = context.HttpContext.Request.Url,
                    UrlReferrer = context.HttpContext.Request.UrlReferrer,
                    UserAgent = context.HttpContext.Request.UserAgent,
                    UserHostAddress = context.HttpContext.Request.UserHostAddress,
                    UserHostName = context.HttpContext.Request.UserHostName,
                    UserLanguages = context.HttpContext.Request.UserLanguages
                },
                HttpContext = new HttpContextCustomData()
                {
                    Headers = context.HttpContext.Request.Headers,
                    Exception = context.Exception,
                    ExceptionHandled = context.ExceptionHandled,
                    IsChildAction = context.IsChildAction
                },
                ControllerContext = new ControllerContextCustomData()
                {
                    RouteValues = context.RequestContext.RouteData.Values,
                    Session = SessionStateContextCustomData.FromSessionStateWrapper(context.HttpContext.Session),
                    ViewBag = context.Controller.ViewBag,
                    ViewData = context.Controller.ViewData
                }
            };
        }
        public static ActionFilterCustomData FromResultExecuted(ResultExecutedContext context)
        {
            return new ActionFilterCustomData()
            {
                UserContext = UserContextCustomData.FromHttpContext(context.HttpContext),
                RequestContext = new RequestContextCustomData()
                {
                    Browser = (HttpBrowserCapabilitiesWrapper)context.HttpContext.Request.Browser,
                    Form = context.HttpContext.Request.Form,
                    QueryString = context.HttpContext.Request.QueryString,
                    Url = context.HttpContext.Request.Url,
                    UrlReferrer = context.HttpContext.Request.UrlReferrer,
                    UserAgent = context.HttpContext.Request.UserAgent,
                    UserHostAddress = context.HttpContext.Request.UserHostAddress,
                    UserHostName = context.HttpContext.Request.UserHostName,
                    UserLanguages = context.HttpContext.Request.UserLanguages
                },
                ResponseContext = ResponseContextCustomData.FromResponseBase(context.HttpContext.Response),
                HttpContext = new HttpContextCustomData()
                {
                    Headers = context.HttpContext.Request.Headers,
                    Exception = context.Exception,
                    ExceptionHandled = context.ExceptionHandled,
                    IsChildAction = context.IsChildAction,
                    Canceled = context.Canceled
                },
                ControllerContext = new ControllerContextCustomData()
                {
                    RouteValues = context.RequestContext.RouteData.Values,
                    Session = SessionStateContextCustomData.FromSessionStateWrapper(context.HttpContext.Session),
                    ViewBag = context.Controller.ViewBag,
                    ViewData = context.Controller.ViewData
                }
            };
        }

        public static ActionFilterCustomData FromResultExecuting(ResultExecutingContext context)
        {
            return new ActionFilterCustomData()
            {
                UserContext = UserContextCustomData.FromHttpContext(context.HttpContext),
                RequestContext = new RequestContextCustomData()
                {
                    Browser = (HttpBrowserCapabilitiesWrapper)context.HttpContext.Request.Browser,
                    Form = context.HttpContext.Request.Form,
                    QueryString = context.HttpContext.Request.QueryString,
                    Url = context.HttpContext.Request.Url,
                    UrlReferrer = context.HttpContext.Request.UrlReferrer,
                    UserAgent = context.HttpContext.Request.UserAgent,
                    UserHostAddress = context.HttpContext.Request.UserHostAddress,
                    UserHostName = context.HttpContext.Request.UserHostName,
                    UserLanguages = context.HttpContext.Request.UserLanguages
                },
                ResponseContext = ResponseContextCustomData.FromResponseBase(context.HttpContext.Response),
                HttpContext = new HttpContextCustomData()
                {
                    Headers = context.HttpContext.Request.Headers,
                    IsChildAction = context.IsChildAction,
                    Cancel = context.Cancel
                },
                ControllerContext = new ControllerContextCustomData()
                {
                    RouteValues = context.RequestContext.RouteData.Values,
                    Session = SessionStateContextCustomData.FromSessionStateWrapper(context.HttpContext.Session),
                    ViewBag = context.Controller.ViewBag,
                    ViewData = context.Controller.ViewData
                }
            };
        }
        public static ActionFilterCustomData FromActionExecuted(ActionExecutedContext context)
        {
            return new ActionFilterCustomData()
            {
                UserContext = UserContextCustomData.FromHttpContext(context.HttpContext),
                RequestContext = new RequestContextCustomData()
                {
                    Browser = (HttpBrowserCapabilitiesWrapper)context.HttpContext.Request.Browser,
                    Form = context.HttpContext.Request.Form,
                    QueryString = context.HttpContext.Request.QueryString,
                    Url = context.HttpContext.Request.Url,
                    UrlReferrer = context.HttpContext.Request.UrlReferrer,
                    UserAgent = context.HttpContext.Request.UserAgent,
                    UserHostAddress = context.HttpContext.Request.UserHostAddress,
                    UserHostName = context.HttpContext.Request.UserHostName,
                    UserLanguages = context.HttpContext.Request.UserLanguages
                },
                ResponseContext = ResponseContextCustomData.FromResponseBase(context.HttpContext.Response),
                HttpContext = new HttpContextCustomData()
                {
                    Headers = context.HttpContext.Request.Headers,
                    Exception = context.Exception,
                    ExceptionHandled = context.ExceptionHandled,
                    IsChildAction = context.IsChildAction
                },
                ControllerContext = new ControllerContextCustomData()
                {
                    RouteValues = context.RequestContext.RouteData.Values,
                    Session = SessionStateContextCustomData.FromSessionStateWrapper(context.HttpContext.Session),
                    ViewBag = context.Controller.ViewBag,
                    ViewData = context.Controller.ViewData
                }
            };
        }
        public static ActionFilterCustomData FromActionExecuting(ActionExecutingContext context)
        {
            return new ActionFilterCustomData()
            {
                UserContext = UserContextCustomData.FromHttpContext(context.HttpContext),
                RequestContext = new RequestContextCustomData()
                {
                    Browser = (HttpBrowserCapabilitiesWrapper)context.HttpContext.Request.Browser,
                    Form = context.HttpContext.Request.Form,
                    QueryString = context.HttpContext.Request.QueryString,
                    Url = context.HttpContext.Request.Url,
                    UrlReferrer = context.HttpContext.Request.UrlReferrer,
                    UserAgent = context.HttpContext.Request.UserAgent,
                    UserHostAddress = context.HttpContext.Request.UserHostAddress,
                    UserHostName = context.HttpContext.Request.UserHostName,
                    UserLanguages = context.HttpContext.Request.UserLanguages
                },
                ResponseContext = ResponseContextCustomData.FromResponseBase(context.HttpContext.Response),
                HttpContext = new HttpContextCustomData()
                {
                    Headers = context.HttpContext.Request.Headers
                },
                ControllerContext = new ControllerContextCustomData()
                {
                    RouteValues = context.RequestContext.RouteData.Values,
                    Session = SessionStateContextCustomData.FromSessionStateWrapper(context.HttpContext.Session),
                    ViewBag = context.Controller.ViewBag,
                    ViewData = context.Controller.ViewData
                }
            };
        }
        public RequestContextCustomData RequestContext { get; set; }
        public ResponseContextCustomData ResponseContext { get; set; }
        public ControllerContextCustomData ControllerContext { get; set; }
        public UserContextCustomData UserContext { get; set; }
        public HttpContextCustomData HttpContext { get; set; }

    }



    [Serializable]
    public class SessionStateContextCustomData
    {
        public static SessionStateContextCustomData FromSessionStateWrapper(HttpSessionStateBase session)
        {
            return new SessionStateContextCustomData()
            {
                Items = session.Keys.OfType<string>().ToDictionary(k => k, k => session[k]),
                CookieMode = session.CookieMode,
                SessionID = session.SessionID,
                IsNewSession = session.IsNewSession,
                LCID = session.LCID
            };
        }

        public Dictionary<string, object> Items { get; set; }
        public HttpCookieMode CookieMode { get; set; }
        public string SessionID { get; set; }

        public bool IsNewSession { get; set; }

        public int LCID { get; set; }
    }

    public class UserContextCustomData
    {
        public UserContextCustomData() { }
        public static UserContextCustomData FromHttpContext(HttpContextBase httpContext)
        {
            IPrincipal principal = httpContext.User;

            UserContextCustomData context = new UserContextCustomData()
            {
                IsAuthenticated = httpContext.Request.IsAuthenticated,
                AnonymousID = httpContext.Request.AnonymousID,
                AuthenticationType = principal.Identity.AuthenticationType,
                Name = principal.Identity.Name
            };
            if (principal is WindowsPrincipal)
            {
                context.WindowsUser = (WindowsPrincipal)principal;
            }
            else if (principal is GenericPrincipal)
            {
                context.GenericUser = (GenericPrincipal)principal;
            }
            else if (principal is RolePrincipal)
            {
                context.RoleUser = (RolePrincipal)principal;
            }
            else if (principal is ClaimsPrincipal)
            {
                context.ClaimsUser = (ClaimsPrincipal)principal;
            }
            return context;
        }

        public string AnonymousID { get; set; }

        public bool IsAuthenticated { get; set; } = false;

        public string AuthenticationType { get; set; }

        public string Name { get; set; }

        public RolePrincipal RoleUser { get; set; }

        public ClaimsPrincipal ClaimsUser { get; set; }

        public GenericPrincipal GenericUser { get; set; }

        public WindowsPrincipal WindowsUser { get; set; }
    }

    public class ControllerContextCustomData
    {
        public ControllerContextCustomData() { }
        public SessionStateContextCustomData Session { get; set; }

        public dynamic ViewBag { get; set; }

        public ViewDataDictionary ViewData { get; set; }

        public RouteValueDictionary RouteValues { get; set; }
    }

    public class RequestContextCustomData
    {
        public RequestContextCustomData() { }

        public HttpBrowserCapabilitiesWrapper Browser { get; set; }

        public NameValueCollection Form { get; set; }

        public NameValueCollection QueryString { get; set; }

        public Uri Url { get; set; }

        public Uri UrlReferrer { get; set; }

        public string UserAgent { get; set; }

        public string UserHostAddress { get; set; }

        public string UserHostName { get; set; }

        public string[] UserLanguages { get; set; }

    }

    public class ResponseContextCustomData
    {
        public static ResponseContextCustomData FromResponseBase(HttpResponseBase response)
        {
            return new ResponseContextCustomData()
            {
                ContentType = response.ContentType,
                IsClientConnected = response.IsClientConnected,
                IsRequestBeingRedirected = response.IsRequestBeingRedirected,
                RedirectLocation = response.RedirectLocation,
                Status = response.Status,
                StatusCode = response.StatusCode,
                StatusDescription = response.StatusDescription
            };
        }

        public string ContentType { get; set; }
        public string Status { get; set; }
        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public bool IsClientConnected { get; set; }

        public bool IsRequestBeingRedirected { get; set; }

        public string RedirectLocation { get; set; }

    }

    public class HttpContextCustomData
    {
        public HttpContextCustomData() { }
        public NameValueCollection Headers { get; set; }
        public Exception Exception { get; set; }

        public bool ExceptionHandled { get; set; } = false;

        public bool IsChildAction { get; set; } = false;

        public bool Cancel { get; set; } = false;

        public bool Canceled { get; set; } = false;

    }
}
