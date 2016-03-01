using System;
using System.Data.SqlClient;
using System.Net;
using System.Text;

namespace TinyLog.Formatters
{
    /// <summary>
    /// Formats an Exception into a Log entry
    /// </summary>
    public class ExceptionFormatter : LogFormatter
    {
        public ExceptionFormatter() : base(new Type[] { typeof(Exception) }, true)
        {
        }

        protected override void FormatLogEntry(LogEntry entry, object customData)
        {
            Exception ex = (Exception)customData;
            StringBuilder sb = new StringBuilder();
            FormatException(sb, ex);
            sb.AppendFormat("\r\nComplete stack trace:\r\n{0}", ex.StackTrace);
            entry.CustomData = sb.ToString();
        }

        /// <summary>
        /// Formats an SQL Exception
        /// </summary>
        /// <param name="sb">The StringBuilder to use</param>
        /// <param name="ex">The SqlException</param>
        protected void FormatSqlException(StringBuilder sb, SqlException ex)
        {
            sb.AppendFormat("[{0}]: {1}\r\n", ex.GetType().Name, ex.Message);
            sb.AppendFormat("error code: {0}\r\n", ex.ErrorCode);
            sb.AppendFormat("error number: {0}\r\n", ex.Number);
            sb.AppendFormat("Source: {0}\r\n", ex.Source);
            sb.AppendFormat("Server: {0}\r\n", ex.Server);
            sb.AppendFormat("Procedure: {0}\r\n", ex.Procedure);
            sb.AppendFormat("Line number: {0}\r\n", ex.LineNumber);
            if (ex.Errors.Count > 0)
            {
                sb.AppendLine("SQL Errors:");
                foreach (SqlError err in ex.Errors)
                {
                    sb.AppendFormat("Message: {0}\r\n", err.Message);
                    sb.AppendFormat("error number: {0}\r\n", err.Number);
                    sb.AppendFormat("Source: {0}\r\n", err.Source);
                    sb.AppendFormat("Server: {0}\r\n", err.Server);
                    sb.AppendFormat("Procedure: {0}\r\n", err.Procedure);
                    sb.AppendFormat("Line number: {0}\r\n", err.LineNumber);
                    sb.AppendLine("---------------------------------------");
                }
            }
        }

        /// <summary>
        /// Formats a web exception
        /// </summary>
        /// <param name="sb">The StringBuilder to use</param>
        /// <param name="ex">The WebException</param>
        protected void FormatWebException(StringBuilder sb, WebException ex)
        {
            sb.AppendFormat("[{0}]: {1}\r\n", ex.GetType().Name, ex.Message);
            sb.AppendFormat("Status: {0}\r\n", ex.Status);
            sb.AppendFormat("Source: {0}\r\n", ex.Source);
            if (ex.Response != null)
            {
                sb.AppendFormat("Response uri: {0}\r\n", ex.Response.ResponseUri.ToString());
                sb.AppendFormat("Response content type: {0}\r\n", ex.Response.ContentType);
                sb.AppendFormat("Response content length: {0}\r\n", ex.Response.ContentLength);
            }
            if (ex.Response.Headers.Count > 0)
            {
                sb.AppendLine("Response headers:");
                foreach (string key in ex.Response.Headers.AllKeys)
                {
                    sb.AppendFormat("[0]: {1}\r\n", key, ex.Response.Headers[key]);
                }
            }

        }

        /// <summary>
        /// Formats an exception and all inner exceptions. AggregateException types are also iterated
        /// </summary>
        /// <param name="sb">The StringBuilder to use</param>
        /// <param name="ex">The main exception</param>
        protected void FormatException(StringBuilder sb, Exception ex)
        {
            if (ex is ArgumentException)
            {
                sb.AppendFormat("[{0}]: (Parameter: {1}) {2}\r\n", ex.GetType().Name, (ex as ArgumentException).ParamName, ex.Message);
            }
            else if (ex is ArgumentNullException)
            {
                sb.AppendFormat("[{0}]: (Parameter: {1}) {2}\r\n", ex.GetType().Name, (ex as ArgumentNullException).ParamName, ex.Message);
            }
            else if (ex is SqlException)
            {
                FormatSqlException(sb, (ex as SqlException));
            }
            else if (ex is System.Net.WebException)
            {
                FormatWebException(sb, (ex as WebException));
            }
            else
            {
                sb.AppendFormat("[{0}]: {1}\r\n", ex.GetType().Name, ex.Message);
            }
            
            if (ex is AggregateException)
            {
                foreach (Exception e in (ex as AggregateException).InnerExceptions)
                {
                    FormatException(sb, e);
                }
            }
            Exception exInner = ex.InnerException;
            while (exInner != null)
            {
                sb.Append("\r\n");
                FormatException(sb, exInner);
                exInner = exInner.InnerException;
            }
        }

        protected override object ParseCustomData(LogEntry logEntry)
        {
            if (logEntry.CustomDataType != null && logEntry.CustomDataType.Equals(this.GetType().FullName, StringComparison.OrdinalIgnoreCase))
            {
                return logEntry.CustomData;
            }
            return null;
        }
    }
}
