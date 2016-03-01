using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyLog.Sql;

namespace TinyLog.Readers
{
    public class SqlLogReader : LogReader
    {
        #region ctor

        public SqlLogReader(string connectionString, string tableName = "TinyLog", int readTimeoutSeconds = 30)
        {
            _ConnectionString = connectionString;
            _TableName = tableName;
            _Timeout = readTimeoutSeconds;
        }
        private string _ConnectionString = "";
        private string _TableName = "";
        private int _Timeout = 30;

        #endregion

        public override bool TryInitialize(out Exception initializeException)
        {
            initializeException = null;
            try
            {
                using (SqlConnection context = new SqlConnection(_ConnectionString))
                {
                    context.Open();
                    SqlCommand cmd = Helpers.GetReadOneCommand(_TableName);
                    cmd.Connection = context;
                    int i = cmd.ExecuteNonQuery();
                    context.Close();
                    return i == -1 || i == 1;
                }
            }
            catch (Exception e)
            {
                initializeException = new InvalidOperationException(string.Format("Unable to initialize the SQL Log Writer for the table '{0}'. See the inner exception for details", _TableName), e);
                return false;
            }
        }

        protected override IEnumerable<LogEntry> ByFilter(LogEntryFilter filter, int maxLogEntries = 100)
        {
            DataTable dt = GetData(Helpers.GetSelectCommand(filter, _TableName, maxLogEntries), _ConnectionString);
            if (dt.Rows.Count > 0)
            {
                return FromDataTable(dt);
            }
            return new List<LogEntry>();
        }

        protected override Task<IEnumerable<LogEntry>> ByFilterAsync(LogEntryFilter filter, int maxLogEntries = 100)
        {
            return Task.FromResult(ByFilter(filter, maxLogEntries));
        }

        protected override LogEntry ById(Guid Id)
        {
            DataTable dt = GetData(Helpers.GetSelectCommand(Id, _TableName), _ConnectionString);
            if (dt.Rows.Count == 1)
            {
                return FromDataRow(dt.Rows[0]);
            }
            return null;
        }

        protected override Task<LogEntry> ByIdAsync(Guid Id)
        {
            return Task.FromResult(ById(Id));
        }


        #region private static helper methods

        private static List<LogEntry> FromDataTable(DataTable dt)
        {
            List<LogEntry> logEntries = new List<LogEntry>();
            foreach (DataRow row in dt.Rows)
            {
                logEntries.Add(FromDataRow(row));
            }
            return logEntries;
        }

        private static LogEntry FromDataRow(DataRow row)
        {
            return new LogEntry()
            {
                Id = Field<Guid>(row, "Id"),
                CorrelationId = Field<Guid?>(row, "CorrelationId"),
                CreatedOn = Field<DateTimeOffset>(row, "CreatedOn"),
                Title = Field<string>(row, "Title"),
                Message = Field<string>(row, "Message"),
                Source = Field<string>(row, "Source"),
                Area = Field<string>(row, "Area"),
                Client = Field<string>(row, "Client"),
                ClientInfo = Field<string>(row, "ClientInfo"),
                Severity = (LogEntrySeverity)Enum.Parse(typeof(LogEntrySeverity), Field<string>(row, "Severity")),
                CustomDataFormatter = Field<string>(row, "CustomDataFormatter"),
                CustomDataType = Field<string>(row, "CustomDataType"),
                CustomData = Field<string>(row, "CustomData"),
                Signature = Field<string>(row, "Signature"),
                SignatureMethod = Field<string>(row, "SignatureMethod")
            };
        }

        private static T Field<T>(DataRow row, string name)
        {
            if (row.IsNull(name))
            {
                return default(T);
            }
            else
            {
                return (T)row[name];
            }
        }

        private static DataTable GetData(string sql, string connectionString)
        {
            DataTable dt = new DataTable();
            using (SqlConnection context = new SqlConnection(connectionString))
            {
                context.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter(sql, context))
                {
                    adapter.Fill(dt);
                }
                context.Close();
            }
            return dt;
        }

        #endregion

    }
}
