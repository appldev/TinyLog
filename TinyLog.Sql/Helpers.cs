using System;
using System.Text;
using System.Data.SqlClient;
using System.Data;

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
            sql.AppendFormat("INSERT  INTO  {0}(Id, CorrelationId,CreatedOn, Title, Message, Source, Area, Client, ClientInfo, Severity, CustomDataFormatter, CustomData, Signature, SignatureMethod)", table);
            sql.AppendLine("VALUES (@Id, @CorrelationId, @CreatedOn, @Title, @Message, @Source, @Area, @Client, @ClientInfo, @Severity, @CustomDataFormatter, @CustomData, Signature, SignatureMethod)");
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
            cmd.Parameters.Add(GetParameter(cmd, "CustomData", SqlDbType.NVarChar, -1, ToDbNull(logEntry.CustomData)));
            cmd.Parameters.Add(GetParameter(cmd, "Signature", SqlDbType.VarChar, 64, ToDbNull(logEntry.Signature)));
            cmd.Parameters.Add(GetParameter(cmd, "SignatureMethod", SqlDbType.VarChar, 10, ToDbNull(logEntry.SignatureMethod)));
            return cmd;
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
