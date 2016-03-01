using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace TinyLog.Formatters
{
    public class XmlSerializationFormatter : LogFormatter
    {
        public XmlSerializationFormatter() : base(new Type[] { typeof(object) },filterOnChildTypes: true)
        {
            
        }

        public override bool IsValidFormatterFor(object customData)
        {
            if (customData is ISerializable || customData is IList || customData is ICollection)
            {
                return true;
            }
            return base.IsValidFormatterFor(customData);
        }

        protected override void FormatLogEntry(LogEntry logEntry, object customData)
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(customData.GetType());
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                ser.Serialize(sw, customData);
                sw.Flush();
                logEntry.CustomData = sb.ToString();
            }
        }

        protected override object ParseCustomData(LogEntry logEntry)
        {
            Type t = Type.GetType(logEntry.CustomDataType, false);
            if (t == null)
            {
                return null;
            }

            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(t);
            StringBuilder sb = new StringBuilder();
            using (StringReader sr = new StringReader(logEntry.CustomData))
            {
                return ser.Deserialize(sr);
            }
        }
    }
}
