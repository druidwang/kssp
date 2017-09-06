using System;
using System.Collections;
using NHibernate.Type;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace com.Sconit.Service
{
    public interface IGenericMgr : IQueryMgr
    {
        void Save(object instance);

        bool BatchInsert<T>(IList<T> instanceList);

        void BulkInsert<T>(IList<T> instanceList);

        void CreateWithTrim(object instance);

        void UpdateWithTrim(object instance);

        void Create(object instance);

        void Update(object instance);

        void MergeUpdate(object instance);

        void Delete(object instance);

        void DeleteById<T>(object id);

        void Delete(IList instances);

        void Delete<T>(IList<T> instances);

        void DeleteAll(Type type);

        void FlushSession();

        void CleanSession();

        void Delete(string hqlString);

        //void Delete(string hqlString, object value);

        void Delete(string hqlString, object value, IType type);

        //void Delete(string hqlString, object[] values);

        void Delete(string hqlString, object[] values, IType[] types);

        int Update(string queryString);

        int Update(string queryString, object value);

        int Update(string queryString, object value, IType type);

        int Update(string queryString, object[] values);

        int Update(string queryString, object[] values, IType[] types);

        int ExecuteSql(string commandText);

        int ExecuteSql(string commandText, SqlParameter[] commandParameters);

        int ExecuteStoredProcedure(string commandText);

        int ExecuteStoredProcedure(string commandText, SqlParameter[] commandParameters);

        int ExecuteUpdateWithCustomQuery(string queryString);

        int ExecuteUpdateWithCustomQuery(string queryString, object value);

        int ExecuteUpdateWithCustomQuery(string queryString, object[] values);

        int ExecuteUpdateWithNativeQuery(string queryString);

        int ExecuteUpdateWithNativeQuery(string queryString, object value);

        int ExecuteUpdateWithNativeQuery(string queryString, object value, IType type);

        int ExecuteUpdateWithNativeQuery(string queryString, object[] values);

        int ExecuteUpdateWithNativeQuery(string queryString, object[] values, IType[] types);

        IList<T> FindAllWithCustomQuery<T>(string queryString);

        IList<T> FindAllWithCustomQuery<T>(string queryString, object value);

        IList<T> FindAllWithCustomQuery<T>(string queryString, object[] values);

    }
}
