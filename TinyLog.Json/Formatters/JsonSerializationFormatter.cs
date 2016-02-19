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

        //public override bool IsValidFormatterFor(object customData)
        //{
        //    if (customData is ISerializable || customData is IList || customData is ICollection)
        //    {
        //        return true;
        //    }
        //    return base.IsValidFormatterFor(customData);
        //}

        protected override void FormatLogEntry(LogEntry logEntry, object customData)
        {
            logEntry.CustomData = JsonConvert.SerializeObject(customData, indent, settings);
        }
    }
}
