using Newtonsoft.Json;
using System.Collections;
using System.Runtime.Serialization;

namespace TinyLog.Formatters
{
    public class JsonSerializationFormatter : LogFormatter
    {
        public JsonSerializationFormatter(bool indentText = true) : base(typeof(object), true)
        {
            indent = indentText ? Formatting.Indented : Formatting.None;
        }

        private Formatting indent = Formatting.Indented;
        private JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateParseHandling = DateParseHandling.DateTimeOffset
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
