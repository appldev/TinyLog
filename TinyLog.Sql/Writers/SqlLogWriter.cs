using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;
using TinyLog.Sql;

namespace TinyLog.Writers
{
    public class SqlLogWriter : LogWriter
    {
        public SqlLogWriter(string connectionString, string tableName = "TinyLog", int writeTimeoutSeconds = 30)
        {
            _ConnectionString = connectionString;
            _TableName = tableName;
            _Timeout = writeTimeoutSeconds;
        }
        private string _ConnectionString = "";
        private string _TableName = "";
        private int _Timeout = 30;

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

        public override bool TryWriteLogEntry(LogEntry logEntry, out Exception writeException)
        {
            writeException = null;
            using (TransactionScope trans = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions()
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromSeconds(_Timeout)
            }))
            {
                try
                {
                    using (SqlConnection context = new SqlConnection(_ConnectionString))
                    {
                        context.Open();
                        SqlCommand cmd = Helpers.GetInsertCommand(logEntry, _TableName, context);
                        cmd.Connection = context;
                        int i = cmd.ExecuteNonQuery();
                        context.Close();
                        if (i != 1)
                        {
                            throw new InvalidOperationException(string.Format("Unable to create the log entry. SQL Server returned {0} rows affected, expected 1", i));
                        }
                    }
                }
                catch (TransactionException exTrans)
                {
                    trans.Dispose();
                    writeException = exTrans;
                    return false;
                }
                catch (SqlException exSql)
                {
                    trans.Dispose();
                    writeException = exSql;
                    return false;
                }
                catch (ApplicationException exApp)
                {
                    trans.Dispose();
                    writeException = exApp;
                    return false;
                }
                catch (Exception ex)
                {
                    trans.Dispose();
                    writeException = ex;
                    return false;
                }
                trans.Complete();
            }
            return true;
                
        }

        public override Task<Tuple<bool, Exception>> TryWriteLogEntryAsync(LogEntry logEntry)
        {
            Exception writeException;
            bool b = TryWriteLogEntry(logEntry, out writeException);
            return Task.FromResult<Tuple<bool, Exception>>(new Tuple<bool, Exception>(b, writeException));
        }
    }
}
