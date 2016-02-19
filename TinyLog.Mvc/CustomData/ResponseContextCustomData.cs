using System;
using System.Web;

namespace TinyLog.CustomData
{
    /// <summary>
    /// Information from the HttpResponse
    /// </summary>
    [Serializable]
    public class ResponseContextCustomData
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="response">The HttpResponse to use</param>
        /// <returns>A new instance of ResponseContextCustomData</returns>
        public static ResponseContextCustomData FromHttpResponse(HttpResponse response)
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
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="response">The HttpResponse to use</param>
        /// <returns>A new instance of ResponseContextCustomData</returns>
        public static ResponseContextCustomData FromHttpResponse(HttpResponseBase response)
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
}
