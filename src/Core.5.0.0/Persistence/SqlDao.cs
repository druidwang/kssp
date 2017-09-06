using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System;

namespace com.Sconit.Persistence
{
    //this is the delegate for sql helper
    public class SqlDao : ISqlDao
    {
        public string ConnectionString { get; set; }

        public int ExecuteSql(string commandText, SqlParameter[] commandParameters)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, commandParameters);
        }

        public int ExecuteStoredProcedure(string commandText, SqlParameter[] commandParameters)
        {
            return ExecuteNonQuery(CommandType.StoredProcedure, commandText, commandParameters);
        }

        public DataSet GetDatasetBySql(string commandText, SqlParameter[] commandParameters)
        {
            return GetDataset(CommandType.Text, commandText, commandParameters);
        }

        public DataSet GetDatasetByStoredProcedure(string commandText, SqlParameter[] commandParameters)
        {
            return GetDataset(CommandType.StoredProcedure, commandText, commandParameters);
        }

        public SqlDataReader GetDataReaderByStoredProcedure(string commandText, SqlParameter[] commandParameters)
        {
            return GetDataReader(CommandType.StoredProcedure, commandText, commandParameters);
        }

        public void BulkInsert(DataTable dataTable)
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            SqlBulkCopy bulkCopy = null;

            try
            {
                connection = new SqlConnection(ConnectionString);
                bulkCopy = new SqlBulkCopy(connection);
                bulkCopy.DestinationTableName = dataTable.TableName;
                bulkCopy.BatchSize = dataTable.Rows.Count;
                if (dataTable != null && dataTable.Columns.Count > 0)
                {
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }
                }
                connection.Open();
                if (dataTable != null && dataTable.Rows.Count != 0)
                {
                    bulkCopy.WriteToServer(dataTable);
                }
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
                if (bulkCopy != null)
                {
                    bulkCopy.Close();
                }
            }
        }

        private int ExecuteNonQuery(CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            int executeRecord = 0;
            try
            {
                connection = new SqlConnection(ConnectionString);
                connection.Open();

                //start a transaction
                transaction = connection.BeginTransaction();
                executeRecord += SqlHelper.ExecuteNonQuery(transaction, commandType, commandText, commandParameters);
                transaction.Commit();
                return executeRecord;
            }
            catch (SqlException ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }

        private DataSet GetDataset(CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            DataSet executeDataSet = new DataSet();
            try
            {
                connection = new SqlConnection(ConnectionString);
                connection.Open();

                //start a transaction
                //transaction = connection.BeginTransaction();
                executeDataSet = SqlHelper.ExecuteDataset(connection, commandType, commandText, commandParameters);
                //transaction.Commit();
                return executeDataSet;
            }
            catch (SqlException ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }

        private SqlDataReader GetDataReader(CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            SqlConnection connection = null;
            //SqlTransaction transaction = null;
            try
            {
                connection = new SqlConnection(ConnectionString);
                connection.Open();

                //start a transaction
                //transaction = connection.BeginTransaction();
                return SqlHelper.ExecuteReader(connection, commandType, commandText, commandParameters);
                //transaction.Commit();
                //return executeDataSet;
            }
            catch (SqlException ex)
            {
                //if (transaction != null)
                //{
                //    transaction.Rollback();
                //}

                throw ex;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }
    }
}
