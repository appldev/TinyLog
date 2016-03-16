using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TinyLog.CustomData;
using TinyLog.CustomData.Mvc;
using TinyLog.Mvc;

namespace TinyLog.Formatters
{
    public class ActionFilterLogFormatter : LogFormatter
    {
        public ActionFilterLogFormatter(bool indentText = true)
            : base(new List<Type>() { typeof(ActionFilterCustomData) }, false)
        {

        }

        private Formatting indent = Formatting.Indented;

        public bool ThrowSerializationExceptions { get; set; } = true;

        /// <summary>
        /// Contains the default json serialization settings
        /// </summary>
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
            try
            {
                logEntry.CustomData = JsonConvert.SerializeObject(data, indent, _SerializationSettings);
            }
            catch (Exception ex)
            {
                if (ThrowSerializationExceptions)
                {
                    throw ex;
                }
                else
                {
                    var a = new
                    {
                        CustomDataType = customData.GetType().Name,
                        Exception = JsonConvert.SerializeObject(ex, indent, _SerializationSettings)
                    };
                    logEntry.CustomData = JsonConvert.SerializeObject(a, indent, _SerializationSettings);
                }
                
            }
            
            if (data.RequestContext != null)
            {
                logEntry.Client = string.Format("{0}/{1}", data.RequestContext.UserHostAddress, data.RequestContext.UserHostName);
                if (data.RequestContext.Browser != null)
                {
                    logEntry.ClientInfo = string.Format("Browser: {0} ({1}). Platform: {2}", data.RequestContext.Browser.Browser, data.RequestContext.Browser.IsMobileDevice ? "Mobile" : "Desktop", data.RequestContext.Browser.Platform);
                }
            }
        }

        protected override object ParseCustomData(LogEntry logEntry)
        {
            Type t = Type.GetType(logEntry.CustomDataType, false);
            if (t == null)
            {
                return null;
            }
            return JsonConvert.DeserializeObject(logEntry.CustomData, t, _SerializationSettings);

        }
    }
}
