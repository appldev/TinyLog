using Newtonsoft.Json;
using System.Dynamic;
using System.Web.Mvc;
using TinyLog.Mvc;

namespace TinyLog.Formatters
{
    public class ControllerContextFormatter : LogFormatter
    {
        public ControllerContextFormatter(bool indentText = true)
            : base(typeof(ControllerContext),false)
        {
            
        }

        private Formatting indent = Formatting.Indented;
        private JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind
        };
        protected override void FormatLogEntry(LogEntry logEntry, object customData)
        {
            ControllerContext ctx = (ControllerContext)customData;
            logEntry.CustomData = JsonConvert.SerializeObject(logEntry, indent, settings);
            dynamic context = new ExpandoObject(), controller = new ExpandoObject();

            dynamic routeData = new ExpandoObject();
            routeData.dataTokens = Helpers.FormateRouteValueDictionary(ctx.RouteData.DataTokens);
            routeData.values = Helpers.FormateRouteValueDictionary(ctx.RouteData.Values);

            context.routeData = routeData;
            context.session = Helpers.FormatSession(ctx.HttpContext.Session);
            context.isAuthenticated = ctx.HttpContext.Request.IsAuthenticated;
            context.user = ctx.HttpContext.User.Identity.Name;
            context.authenticationType = ctx.HttpContext.User.Identity.AuthenticationType;

            logEntry.Client = ctx.HttpContext.Request.UserHostAddress;
            logEntry.Area = ctx.HttpContext.Request.Url.AbsoluteUri;

            

            //dynamic routeDataValues = new ExpandoObject();
            //routeData.area = ctx.RouteData.DataTokens["area"];
            // routeData.controller = ctx.RouteData.DataTokens["controller"];

            //foreach (string key in ctx.RouteData.Values.Keys)
            //{
            //    (routeDataValues as IDictionary<string, object>).Add(key, ctx.RouteData.Values[key]);
            //}
            //routeData.values = routeDataValues;
            //dynamic session = new ExpandoObject();



        }



       
    }
}
