using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TinyLog.Formatters
{
    public class JsonSerializationFormatter : LogFormatter
    {
        public JsonSerializationFormatter(bool indentText = true) : base(new List<Type>() { typeof(object) }, true)
        {
            indent = indentText ? Formatting.Indented : Formatting.None;
        }

        private Formatting indent = Formatting.Indented;
        private JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateParseHandling = DateParseHandling.DateTimeOffset,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
            TypeNameHandling = TypeNameHandling.Objects
        };


        protected override void FormatLogEntry(LogEntry logEntry, object customData)
        {
            logEntry.CustomData = JsonConvert.SerializeObject(customData, indent, settings);
        }

        protected override object ParseCustomData(LogEntry logEntry)
        {
            Type t = Type.GetType(logEntry.CustomDataType, false);
            if (t == null)
            {
                return null;
            }
            return JsonConvert.DeserializeObject(logEntry.CustomData, t, settings);
            
        }
    }
}
