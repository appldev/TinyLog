using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.Web.Mvc;
using TinyLog.CustomData;
using TinyLog.Mvc;

namespace TinyLog.Formatters
{
    public class ActionFilterLogFormatter : LogFormatter
    {
        public ActionFilterLogFormatter(bool indentText = true)
            : base(typeof(ActionFilterCustomData), false)
        {

        }

        private Formatting indent = Formatting.Indented;

        private readonly static JsonSerializerSettings _SerializationSettings = new JsonSerializerSettings()
        {
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
            TypeNameHandling = TypeNameHandling.Objects,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateParseHandling = DateParseHandling.DateTimeOffset,
            ContractResolver = new HttpRequestBaseContractResolver(),
            Converters = new List<JsonConverter>() { new NameValueCollectionConverter() }
        };


        protected override void FormatLogEntry(LogEntry logEntry, object customData)
        {
            ActionFilterCustomData data = (customData as ActionFilterCustomData);
            logEntry.CustomData = JsonConvert.SerializeObject(data, indent, _SerializationSettings);
            if (data.RequestContext != null)
            {
                logEntry.Client = string.Format("{0}/{1}", data.RequestContext.UserHostAddress, data.RequestContext.UserHostName);
                if (data.RequestContext.Browser != null)
                {
                    logEntry.ClientInfo = string.Format("Browser: {0} ({1}). Platform: {2}", data.RequestContext.Browser.Browser, data.RequestContext.Browser.IsMobileDevice ? "Mobile" : "Desktop", data.RequestContext.Browser.Platform);
                }
            }
        }




    }
}
