using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace TinyLog.CustomData
{
    /// <summary>
    /// Contains information from the Session state
    /// </summary>
    [Serializable]
    public class SessionStateContextCustomData
    {
        /// <summary>
        /// Creates a new instance of the SessionStateContextCustomData class from a HttpSessionState
        /// </summary>
        /// <param name="session">The session state object</param>
        /// <returns>A new instance of the SessionStateContextCustomData class</returns>
        public static SessionStateContextCustomData FromHttpSessionState(HttpSessionState session)
        {
            if (session == null)
            {
                return null;
            }
            HttpSessionStateWrapper wrapper = new HttpSessionStateWrapper(session);
            return FromHttpSessionState(wrapper);
        }

        /// <summary>
        /// Creates a new instance of the SessionStateContextCustomData class from a HttpSessionState
        /// </summary>
        /// <param name="session">The session state object</param>
        /// <returns>A new instance of the SessionStateContextCustomData class</returns>
        public static SessionStateContextCustomData FromHttpSessionState(HttpSessionStateBase session)
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

}
