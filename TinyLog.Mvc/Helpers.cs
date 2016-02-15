using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace TinyLog.Mvc
{
    public static class Helpers
    {
        public static ExpandoObject FormatSession(HttpSessionStateBase Session)
        {
            dynamic session = new ExpandoObject();
            foreach (string key in Session.Keys)
            {
                (session as IDictionary<string, object>).Add(key, Session[key]);
            }
            return session;
        }

        public static ExpandoObject FormateRouteValueDictionary(RouteValueDictionary dict)
        {
            dynamic values = new ExpandoObject();
            foreach (string key in dict.Keys)
            {
                (values as IDictionary<string, object>).Add(key, dict[key]);
            }
            return values;
        }
    }
}
