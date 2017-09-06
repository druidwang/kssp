﻿// -----------------------------------------------------------------------
// <copyright file="INHQuery.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace com.Sconit.Persistence
{
    using System.Collections;
    using System.Collections.Generic;
    using NHibernate.Criterion;
    using NHibernate.Type;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface INHQueryDao : IQueryDao
    {
        IList FindAllWithCustomQuery(string queryString);

        IList FindAllWithCustomQuery(string queryString, object value);

        IList FindAllWithCustomQuery(string queryString, object value, IType type);

        IList FindAllWithCustomQuery(string queryString, object[] values);

        IList FindAllWithCustomQuery(string queryString, object[] values, IType[] types);

        IList FindAllWithCustomQuery(string queryString, int firstRow, int maxRows);

        IList FindAllWithCustomQuery(string queryString, object value, int firstRow, int maxRows);

        IList FindAllWithCustomQuery(string queryString, object value, IType type, int firstRow, int maxRows);

        IList FindAllWithCustomQuery(string queryString, object[] values, int firstRow, int maxRows);

        IList FindAllWithCustomQuery(string queryString, object[] values, IType[] types, int firstRow, int maxRows);

        IList FindAllWithNamedQuery(string namedQuery);

        IList FindAllWithNamedQuery(string namedQuery, object value);

        IList FindAllWithNamedQuery(string namedQuery, object value, IType type);

        IList FindAllWithNamedQuery(string namedQuery, object[] values);

        IList FindAllWithNamedQuery(string namedQuery, object[] values, IType[] types);

        IList FindAllWithNamedQuery(string namedQuery, int firstRow, int maxRows);

        IList FindAllWithNamedQuery(string namedQuery, object value, int firstRow, int maxRows);

        IList FindAllWithNamedQuery(string namedQuery, object value, IType type, int firstRow, int maxRows);

        IList FindAllWithNamedQuery(string namedQuery, object[] values, int firstRow, int maxRows);

        IList FindAllWithNamedQuery(string namedQuery, object[] values, IType[] types, int firstRow, int maxRows);

        IList<T> FindAllWithCustomQuery<T>(string queryString);

        IList<T> FindAllWithCustomQuery<T>(string queryString, object value);

        IList<T> FindAllWithCustomQuery<T>(string queryString, object value, IType type);

        IList<T> FindAllWithCustomQuery<T>(string queryString, object[] values);

        IList<T> FindAllWithCustomQuery<T>(string queryString, object[] values, IType[] types);

        IList<T> FindAllWithCustomQuery<T>(string queryString, int firstRow, int maxRows);

        IList<T> FindAllWithCustomQuery<T>(string queryString, object value, int firstRow, int maxRows);

        IList<T> FindAllWithCustomQuery<T>(string queryString, object value, IType type, int firstRow, int maxRows);

        IList<T> FindAllWithCustomQuery<T>(string queryString, object[] values, int firstRow, int maxRows);

        IList<T> FindAllWithCustomQuery<T>(string queryString, object[] values, IType[] types, int firstRow, int maxRows);

        IList<T> FindAllWithCustomQuery<T>(string namedQuery, IDictionary<string, object> param);

        IList<T> FindAllWithCustomQuery<T>(string namedQuery, IDictionary<string, object> param, IType[] types);

        IList<T> FindAllWithCustomQuery<T>(string namedQuery, IDictionary<string, object> param, IType[] types, int firstRow, int maxRows);

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

        IList<T> FindAll<T>(ICriterion[] criterias);

        IList<T> FindAll<T>(ICriterion[] criterias, int firstRow, int maxRows);

        IList<T> FindAll<T>(ICriterion[] criterias, Order[] sortItems);

        IList<T> FindAll<T>(ICriterion[] criterias, Order[] sortItems, int firstRow, int maxRows);

        IList FindAll(DetachedCriteria criteria);

        IList FindAll(DetachedCriteria criteria, int firstRow, int maxRows);

        IList<T> FindAll<T>(DetachedCriteria criteria);

        IList<T> FindAll<T>(DetachedCriteria criteria, int firstRow, int maxRows);

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
    }
}
