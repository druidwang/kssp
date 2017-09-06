using NHibernate.Type;
using System.Collections.Generic;

namespace com.Sconit.Persistence
{
    /// <summary>
    /// Summary description for INHDaoBase.
    /// </summary>

    public interface INHDao : IDao, INHQueryDao
    {

        void MergeUpdate(object instance);

        void Delete(string hqlString);

        //void Delete(string hqlString, object value);

        void Delete(string hqlString, object value, IType type);

        //void Delete(string hqlString, object[] values);

        void Delete(string hqlString, object[] values, IType[] type);

        int ExecuteUpdateWithCustomQuery(string queryString);

        int ExecuteUpdateWithCustomQuery(string queryString, object value);

        int ExecuteUpdateWithCustomQuery(string queryString, object value, IType type);

        int ExecuteUpdateWithCustomQuery(string queryString, object[] values);

        int ExecuteUpdateWithCustomQuery(string queryString, object[] values, IType[] types);

        int ExecuteUpdateWithNativeQuery(string queryString);

        int ExecuteUpdateWithNativeQuery(string queryString, object value);

        int ExecuteUpdateWithNativeQuery(string queryString, object value, IType type);

        int ExecuteUpdateWithNativeQuery(string queryString, object[] values);

        int ExecuteUpdateWithNativeQuery(string queryString, object[] values, IType[] types);

        void InitializeLazyProperties(object instance);

        void InitializeLazyProperty(object instance, string propertyName);

        void FlushSession();

        void CleanSession();

        void GetTableProperty(object entity, out string tableName, out Dictionary<string, string> propertyAndColumnNames);
    }
}
