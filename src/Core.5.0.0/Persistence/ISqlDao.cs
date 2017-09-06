using System.Data;
using System.Data.SqlClient;

namespace com.Sconit.Persistence
{
    public interface ISqlDao
    {
        int ExecuteSql(string commandText, SqlParameter[] commandParameters);

        int ExecuteStoredProcedure(string commandText, SqlParameter[] commandParameters);

        DataSet GetDatasetBySql(string commandText, SqlParameter[] commandParameters);

        DataSet GetDatasetByStoredProcedure(string commandText, SqlParameter[] commandParameters);

        SqlDataReader GetDataReaderByStoredProcedure(string commandText, SqlParameter[] commandParameters);

        void BulkInsert(DataTable dataTable);
    }
}
