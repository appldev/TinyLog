using System;
using System.Collections.Specialized;

namespace TinyLog.CustomData
{
    /// <summary>
    /// Information from the HttpContext
    /// </summary>
    [Serializable]
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
