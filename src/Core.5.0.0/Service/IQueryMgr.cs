// -----------------------------------------------------------------------
// <copyright file="IQueryMgr.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace com.Sconit.Service
{
    using System.Collections;
    using System.Collections.Generic;
    using NHibernate.Criterion;
    using NHibernate.Type;
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IQueryMgr
    {                
        T FindById<T>(object id);

        IList<T> FindAll<T>();

        IList<T> FindAll<T>(int firstRow, int maxRows);

        IList FindAll(DetachedCriteria criteria);

        IList FindAll(DetachedCriteria criteria, int firstRow, int maxRows);

        IList<T> FindAll<T>(DetachedCriteria criteria);

        IList<T> FindAll<T>(DetachedCriteria criteria, int firstRow, int maxRows);

        IList FindAll(string hql);

        IList FindAll(string hql, object value);

        IList FindAll(string hql, object value, IType type);

        IList FindAll(string hql, object[] values);

        IList FindAll(string hql, IEnumerable<object> values);

        IList FindAll(string hql, object[] values, IType[] types);

        IList FindAll(string hql, int firstRow, int maxRows);

        IList FindAll(string hql, object value, int firstRow, int maxRows);

        IList FindAll(string hql, object value, IType type, int firstRow, int maxRows);

        IList FindAll(string hql, object[] values, int firstRow, int maxRows);

        IList FindAll(string hql, object[] values, IType[] types, int firstRow, int maxRows);

        IList<T> FindAll<T>(string hql);

        IList<T> FindAll<T>(string hql, object value);

        IList<T> FindAll<T>(string hql, object value, IType types);

        IList<T> FindAll<T>(string hql, object[] values);

        IList<T> FindAll<T>(string hql, IEnumerable<object> values);

        IList<T> FindAll<T>(string hql, object[] values, IType[] types);

        IList<T> FindAll<T>(string hql, int firstRow, int maxRows);

        IList<T> FindAll<T>(string hql, object value, int firstRow, int maxRows);

        IList<T> FindAll<T>(string hql, object value, IType type, int firstRow, int maxRows);

        IList<T> FindAll<T>(string hql, object[] values, int firstRow, int maxRows);

        IList<T> FindAll<T>(string hql, object[] values, IType[] types, int firstRow, int maxRows);

        IList<T> FindAll<T>(string hql, IDictionary<string, object> param);

        IList<T> FindAll<T>(string hql, IDictionary<string, object> param, IType[] types);

        IList<T> FindAll<T>(string hql, IDictionary<string, object> param, IType[] types, int firstRow, int maxRows);

        IList FindAllWithNamedQuery(string namedQuery);

        IList FindAllWithNamedQuery(string namedQuery, object value);

        IList FindAllWithNamedQuery(string namedQuery, object value, IType types);

        IList FindAllWithNamedQuery(string namedQuery, object[] values);

        IList FindAllWithNamedQuery(string namedQuery, object[] values, IType[] types);

        IList FindAllWithNamedQuery(string namedQuery, int firstRow, int maxRows);

        IList FindAllWithNamedQuery(string namedQuery, object value, int firstRow, int maxRows);

        IList FindAllWithNamedQuery(string namedQuery, object value, IType type, int firstRow, int maxRows);

        IList FindAllWithNamedQuery(string namedQuery, object[] values, int firstRow, int maxRows);

        IList FindAllWithNamedQuery(string namedQuery, object[] values, IType[] type, int firstRow, int maxRows);

        IList<T> FindAllWithNamedQuery<T>(string namedQuery);

        IList<T> FindAllWithNamedQuery<T>(string namedQuery, object value);

        IList<T> FindAllWithNamedQuery<T>(string namedQuery, object value, IType type);

        IList<T> FindAllWithNamedQuery<T>(string namedQuery, object[] values);

        IList<T> FindAllWithNamedQuery<T>(string namedQuery, object[] values, IType[] types);

        IList<T> FindAllWithNamedQuery<T>(string namedQuery, int firstRow, int maxRows);

        IList<T> FindAllWithNamedQuery<T>(string namedQuery, object value, int firstRow, int maxRows);

        IList<T> FindAllWithNamedQuery<T>(string namedQuery, object value, IType type, int firstRow, int maxRows);

        IList<T> FindAllWithNamedQuery<T>(string namedQuery, object[] values, int firstRow, int maxRows);

        IList<T> FindAllWithNamedQuery<T>(string namedQuery, object[] values, IType[] types, int firstRow, int maxRows);

        IList FindAllWithNativeSql(string sql);

        IList FindAllWithNativeSql(string sql, object value);

        IList FindAllWithNativeSql(string sql, object value, IType type);

        IList FindAllWithNativeSql(string sql, object[] values);

        IList FindAllWithNativeSql(string sql, object[] values, IType[] types);

        IList<T> FindAllWithNativeSql<T>(string sql);

        IList<T> FindAllWithNativeSql<T>(string sql, object value);

        IList<T> FindAllWithNativeSql<T>(string sql, object value, IType type);

        IList<T> FindAllWithNativeSql<T>(string sql, object[] values);

        IList<T> FindAllWithNativeSql<T>(string sql, object[] values, IType[] types);

        IList<T> FindEntityWithNativeSql<T>(string sql);

        IList<T> FindEntityWithNativeSql<T>(string sql, object value);

        IList<T> FindEntityWithNativeSql<T>(string sql, object value, IType type);

        IList<T> FindEntityWithNativeSql<T>(string sql, object[] values);

        IList<T> FindEntityWithNativeSql<T>(string sql, object[] values, IType[] types);

        DataSet GetDatasetBySql(string commandText, SqlParameter[] commandParameters);

        DataSet GetDatasetByStoredProcedure(string commandText, SqlParameter[] commandParameters);

        IList<T> FindAllIn<T>(string hql, IEnumerable<object> inParam, IEnumerable<object> param = null);

        IList<T> FindEntityWithNativeSqlIn<T>(string sql, IEnumerable<object> inValues, IEnumerable<object> values = null);

        IList<T> FindAllWithNativeSqlIn<T>(string sql, IEnumerable<object> inValues, IEnumerable<object> values = null);
    }
}
