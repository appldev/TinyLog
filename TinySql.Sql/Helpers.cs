using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using TinyLog;
using System.Data;

namespace TinySql.Sql
{
    internal static class Helpers
    {
        public static SqlCommand GetReadOneCommand(string tableName)
        {
            SqlCommand cmd = new SqlCommand(string.Format("SELECT TOP 1 * FROM {0} WITH(NOLOCK)", tableName));
            return cmd;
        }
        public static SqlCommand GetInsertCommand(LogEntry logEntry, string table, SqlConnection con)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("INSERT  INTO  {0}(Id, CorrelationId,CreatedOn, Title, Message, Source, Area, Client, ClientInfo, Severity, CustomDataFormatter, CustomData)", table);
            sql.AppendLine("VALUES (@Id, @CorrelationId, @CreatedOn, @Title, @Message, @Source, @Area, @Client, @ClientInfo, @Severity, @CustomDataFormatter, @CustomData)");
            SqlCommand cmd = new SqlCommand(sql.ToString(), con);
            cmd.Parameters.Add(GetParameter(cmd, "Id", SqlDbType.UniqueIdentifier, -1, logEntry.Id));
            cmd.Parameters.Add(GetParameter(cmd, "CorrelationId", SqlDbType.UniqueIdentifier, -1, logEntry.CorrelationId.HasValue ? (object)logEntry.CorrelationId.Value : (object)DBNull.Value));
            cmd.Parameters.Add(GetParameter(cmd, "CreatedOn", SqlDbType.DateTimeOffset, -1, logEntry.CreatedOn));
            cmd.Parameters.Add(GetParameter(cmd, "Title", SqlDbType.NVarChar, 200, DbStr(logEntry.Title)));
            cmd.Parameters.Add(GetParameter(cmd, "Message", SqlDbType.NVarChar, -1, DbStr(logEntry.Message)));
            cmd.Parameters.Add(GetParameter(cmd, "Source", SqlDbType.NVarChar, 50, DbStr(logEntry.Source)));
            cmd.Parameters.Add(GetParameter(cmd, "Area", SqlDbType.NVarChar, 50, DbStr(logEntry.Area)));
            cmd.Parameters.Add(GetParameter(cmd, "Client", SqlDbType.NVarChar, 200, DbStr(logEntry.Client)));
            cmd.Parameters.Add(GetParameter(cmd, "ClientInfo", SqlDbType.NVarChar, -1, DbStr(logEntry.ClientInfo)));
            cmd.Parameters.Add(GetParameter(cmd, "Severity", SqlDbType.NVarChar, 20, logEntry.SeverityString));
            cmd.Parameters.Add(GetParameter(cmd, "CustomDataFormatter", SqlDbType.NVarChar, 100, DbStr(logEntry.CustomDataFormatter)));
            cmd.Parameters.Add(GetParameter(cmd, "CustomData", SqlDbType.NVarChar, -1, DbStr(logEntry.CustomData)));
            return cmd;
        }

        private static object DbStr(string s)
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
