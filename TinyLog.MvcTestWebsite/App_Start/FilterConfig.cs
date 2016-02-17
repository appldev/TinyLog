using System.Web;
using System.Web.Mvc;
using TinyLog.Mvc;

namespace TinyLog.MvcTestWebsite
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // filters.Add(new HandleErrorAttribute());
            filters.Add(new TinyLogHandleErrorAttribute());
            filters.Add(new TinyLogVerboseActionFilterAttribute());
        }
    }
}
