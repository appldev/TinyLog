using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyLog.Azure
{
    internal static class Helpers
    {
        public static BrokeredMessage GetBrokeredMessage(LogEntry logEntry, bool withProperties = false)
        {
            BrokeredMessage m = new BrokeredMessage(logEntry);
            if (withProperties)
            {
                m.Properties["Id"] = logEntry.Id;
                m.Properties["CorrelationId"] = logEntry.CorrelationId;
                m.Properties["Title"] = logEntry.Title;
                m.Properties["Message"] = logEntry.Message;
                m.Properties["Severity"] = logEntry.SeverityString;
                m.Properties["Source"] = logEntry.Source;
                m.Properties["Area"] = logEntry.Area;
                m.Properties["Client"] = logEntry.Client;
                m.Properties["ClientInfo"] = logEntry.ClientInfo;
                m.Properties["CustomDataFormatter"] = logEntry.CustomDataFormatter;
                m.Properties["CustomDataType"] = logEntry.CustomDataType;
                m.Properties["SignatureMethod"] = logEntry.SignatureMethod;
            }
            return m;
        }

    }
}
