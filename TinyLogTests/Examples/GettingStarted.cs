using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyLog;
using TinyLog.Writers;
using TinyLog.Formatters;

namespace TinyLogTests.Documentation
{
    class GettingStarted
    {
        public void BasicSetup()
        {
            // First we setup a Log writer, that will store the log entries we submit
            TinyLog.Log.Default.RegisterLogWriter(new FileLogWriter(System.IO.Path.GetTempPath()));

            // You are now ready to submit logs, that will be stored in a file in the temp folder
            LogEntry entry = LogEntry.Create("Hello world", "My First entry ever");
            TinyLog.Log.Default.WriteLogEntry(entry);

            // You probably want to include all kinds of custom information to your logs, that will store
            // relevant context sensitive information. All the custom data you include, can be formatted
            // by special log formatters, that will re-format or serialize the information into a human
            // readable form. To do this, you can add one or more Log Formatters, that will be responsible
            // for formatting specific types of custom data:
            TinyLog.Log.Default.RegisterLogFormatter(new ExceptionFormatter());

            InvalidOperationException exception = new InvalidOperationException("This is an example of thrown exception in your app");
            entry = LogEntry.Error("This is an error", "See the custom data for more information",source: "My app", area: "Getting Started");
            TinyLog.Log.Default.WriteLogEntry<Exception>(entry, exception);

            // You can add multiple log formatters, log writers and log subscribers
            
            // Let's add a Message Queue log writer also:
            TinyLog.Log.Default.RegisterLogWriter(new MessageQueueLogWriter(".\\Private$\\MyTinyLogQueue"));

            // We also add a json log formatter, that will serialize all types of custom objects
            TinyLog.Log.Default.RegisterLogFormatter(new JsonSerializationFormatter());

            // now lets create an object and log it
            var myObj = new
            {
                Name = "Michael Randrup",
                Email = "michael_randrup@hotmail.com"
            };
            entry = LogEntry.Verbose(source: "My App", area: "Getting Started", title: "Owner registration", message: "This is the log owner");

            // The custom data log will be json serialized and logged both in the Mesage Queue and in the log file in the temp folder
            TinyLog.Log.Default.WriteLogEntry<object>(entry, myObj);
























        }


    }
}
