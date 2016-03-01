using System;
using System.Collections.Specialized;
using System.Web;

namespace TinyLog.CustomData.Mvc
{
    /// <summary>
    /// Contains information from the HttpResponse
    /// </summary>
    [Serializable]
    public class RequestContextCustomData
    {
        /// <summary>
        /// Creates a new instance of the RequestContextCustomData from a HttpRequest
        /// </summary>
        /// <param name="request">The HttpRequest</param>
        /// <returns>A new instance of the RequestContextCustomData class</returns>
        public static RequestContextCustomData FromHttpRequest(HttpRequest request)
        {
            HttpRequestWrapper wrapper = new HttpRequestWrapper(request);
            return FromHttpRequest(wrapper);
        }
        /// <summary>
        /// Creates a new instance of the RequestContextCustomData from a HttpRequest
        /// </summary>
        /// <param name="request">The HttpRequest</param>
        /// <returns>A new instance of the RequestContextCustomData class</returns>
        public static RequestContextCustomData FromHttpRequest(HttpRequestBase request)
        {
            return new RequestContextCustomData()
            {
                Browser = (HttpBrowserCapabilitiesWrapper)request.Browser,
                Form = request.Form,
                QueryString = request.QueryString,
                Url = request.Url,
                UrlReferrer = request.UrlReferrer,
                UserAgent = request.UserAgent,
                UserHostAddress = request.UserHostAddress,
                UserHostName = request.UserHostName,
                UserLanguages = request.UserLanguages,
                ContentType = request.ContentType,
                HttpMethod = request.HttpMethod
            };
        }
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

        public string ContentType { get; set; }

        public string HttpMethod { get; set; }

    }
}
