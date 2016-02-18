using System.Web.Mvc;


namespace TinyLog.MvcTestWebsite
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            TinyLog.Mvc.Helpers.RegisterGlobalFilters(filters,true);
            
        }
    }
}
