using System;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

namespace TinyLog.Sql
{
    internal static class Helpers
    {
        public static SqlCommand GetReadOneCommand(string tableName)
        {
            SqlCommand cmd = new SqlCommand(string.Format("SELECT TOP 1 Id FROM {0} WITH(NOLOCK)", tableName));
            return cmd;
        }
        public static SqlCommand GetInsertCommand(LogEntry logEntry, string table, SqlConnection con)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("INSERT  INTO  {0}(Id, CorrelationId,CreatedOn, Title, Message, Source, Area, Client, ClientInfo, Severity, CustomDataFormatter, CustomDataType, CustomData, Signature, SignatureMethod)", table);
            sql.AppendLine("VALUES (@Id, @CorrelationId, @CreatedOn, @Title, @Message, @Source, @Area, @Client, @ClientInfo, @Severity, @CustomDataFormatter, @CustomDataType, @CustomData, @Signature, @SignatureMethod)");
            SqlCommand cmd = new SqlCommand(sql.ToString(), con);
            cmd.Parameters.Add(GetParameter(cmd, "Id", SqlDbType.UniqueIdentifier, -1, logEntry.Id));
            cmd.Parameters.Add(GetParameter(cmd, "CorrelationId", SqlDbType.UniqueIdentifier, -1, logEntry.CorrelationId.HasValue ? (object)logEntry.CorrelationId.Value : (object)DBNull.Value));
            cmd.Parameters.Add(GetParameter(cmd, "CreatedOn", SqlDbType.DateTimeOffset, -1, logEntry.CreatedOn));
            cmd.Parameters.Add(GetParameter(cmd, "Title", SqlDbType.NVarChar, 200, ToDbNull(logEntry.Title)));
            cmd.Parameters.Add(GetParameter(cmd, "Message", SqlDbType.NVarChar, -1, ToDbNull(logEntry.Message)));
            cmd.Parameters.Add(GetParameter(cmd, "Source", SqlDbType.NVarChar, 50, ToDbNull(logEntry.Source)));
            cmd.Parameters.Add(GetParameter(cmd, "Area", SqlDbType.NVarChar, 50, ToDbNull(logEntry.Area)));
            cmd.Parameters.Add(GetParameter(cmd, "Client", SqlDbType.NVarChar, 200, ToDbNull(logEntry.Client)));
            cmd.Parameters.Add(GetParameter(cmd, "ClientInfo", SqlDbType.NVarChar, -1, ToDbNull(logEntry.ClientInfo)));
            cmd.Parameters.Add(GetParameter(cmd, "Severity", SqlDbType.NVarChar, 20, logEntry.SeverityString));
            cmd.Parameters.Add(GetParameter(cmd, "CustomDataFormatter", SqlDbType.NVarChar, 100, ToDbNull(logEntry.CustomDataFormatter)));
            cmd.Parameters.Add(GetParameter(cmd, "CustomDataType", SqlDbType.NVarChar, 100, ToDbNull(logEntry.CustomDataType)));
            cmd.Parameters.Add(GetParameter(cmd, "CustomData", SqlDbType.NVarChar, -1, ToDbNull(logEntry.CustomData)));
            cmd.Parameters.Add(GetParameter(cmd, "Signature", SqlDbType.VarChar, 64, ToDbNull(logEntry.Signature)));
            cmd.Parameters.Add(GetParameter(cmd, "SignatureMethod", SqlDbType.VarChar, 10, ToDbNull(logEntry.SignatureMethod)));
            return cmd;
        }

        internal static string GetSelectCommand(Guid Id, string table)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("SELECT * FROM {0}\r\n", table);
            sql.AppendFormat("WHERE Id = '{0}'\r\n", Id);
            return sql.ToString();
        }
        public static string GetSelectCommand(LogEntryFilter filter, string table, int maxRecords = -1)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("DECLARE @from datetimeoffset");
            sql.AppendLine("DECLARE @to datetimeoffset");
            sql.AppendFormat("SET @from = {0}\r\n", filter.FromDate.HasValue ? "'" + filter.FromDateString + "'" : "NULL");
            sql.AppendFormat("SET @to = {0}\r\n", filter.ToDate.HasValue ? "'" + filter.ToDateString + "'" : "NULL");
            sql.AppendFormat("SELECT {0} * FROM {1}\r\n", maxRecords > 0 ? "TOP " + maxRecords.ToString() : "", table);
            sql.AppendLine("WHERE (@from IS NULL OR CreatedOn >= @from)");
            sql.AppendLine("AND (@to IS NULL OR CreatedOn <= @to)");
            if (filter.SourcesFilter != null)
            {
                sql.AppendFormat("AND  Source IN ({0})\r\n", string.Join(",", filter.SourcesFilter.Select(x => "'" + x + "'")));
            }
            if (filter.AreasFilter != null)
            {
                sql.AppendFormat("AND  Area IN ({0})\r\n", string.Join(",", filter.AreasFilter.Select(x => "'" + x + "'")));
            }
            if (filter.SeveritiesFilter != null)
            {
                sql.AppendFormat("AND  Severity IN ({0})\r\n", string.Join(",", filter.SeveritiesFilter.Select(x => "'" + Enum.GetName(typeof(LogEntrySeverity), x) + "'")));
            }
            sql.AppendLine("ORDER BY CreatedOn DESC");
            return sql.ToString();
        }


        private static object ToDbNull(object s)
        {
            if (s == null)
            {
                return DBNull.Value;
            }
            else
            {
                return s;
            }

        }

        private static SqlParameter GetParameter(SqlCommand cmd, string Name, SqlDbType type, int size, object value)
        {
            SqlParameter p = cmd.CreateParameter();
            p.ParameterName = Name;
            p.SqlDbType = type;
            p.Size = size;
            p.Direction = ParameterDirection.Input;
            p.Value = value;

            return p;
        }
    }
}
