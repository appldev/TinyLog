using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace TinyLog.CustomData
{




    /// <summary>
    /// Contains information from the Controller context
    /// </summary>
    [Serializable]
    public class ControllerContextCustomData
    {
        public ControllerContextCustomData() { }
        public SessionStateContextCustomData Session { get; set; }

        public dynamic ViewBag { get; set; }

        public ViewDataDictionary ViewData { get; set; }

        public RouteValueDictionary RouteValues { get; set; }
    }

    

    

    
}
