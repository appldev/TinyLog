using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace TinyLog.CustomData.Mvc
{
    /// <summary>
    /// Contains information from the User context
    /// </summary>
    [Serializable]
    public class UserContextCustomData
    {
        public UserContextCustomData() { }

        /// <summary>
        /// Creates a new instance of the UserContextCustomData from a HttpContext
        /// </summary>
        /// <param name="httpContext">The HttpContext</param>
        /// <returns>A new instance of the UserContextCustomData</returns>
        public static UserContextCustomData FromHttpContext(HttpContext httpContext, ActionFilterCustomData.Details detail)
        {
            HttpContextWrapper wrapper = new HttpContextWrapper(httpContext);
            return FromHttpContext(wrapper, detail);
        }
        /// <summary>
        /// Creates a new instance of the UserContextCustomData from a HttpContext
        /// </summary>
        /// <param name="httpContext">The HttpContext</param>
        /// <returns>A new instance of the UserContextCustomData</returns>
        public static UserContextCustomData FromHttpContext(HttpContextBase httpContext, ActionFilterCustomData.Details detail)
        {
            IPrincipal principal = httpContext.User;
            if (principal == null)
            {
                return null;
            }
            UserContextCustomData context = new UserContextCustomData()
            {
                IsAuthenticated = httpContext.Request.IsAuthenticated,
                AnonymousID = httpContext.Request.AnonymousID,
                AuthenticationType = principal.Identity.AuthenticationType,
                Name = principal.Identity.Name
            };
            //if (principal is WindowsPrincipal)
            //{
            //    context.WindowsUser = (WindowsPrincipal)principal;
            //}
            //else if (principal is GenericPrincipal)
            //{
            //    context.GenericUser = (GenericPrincipal)principal;
            //}
            //else if (principal is RolePrincipal)
            //{
            //    context.RoleUser = (RolePrincipal)principal;
            //}
            //else if (principal is ClaimsPrincipal)
            //{
            //    context.ClaimsUser = (ClaimsPrincipal)principal;
            //}
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

}
