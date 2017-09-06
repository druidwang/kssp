// -----------------------------------------------------------------------
// <copyright file="QueryMgrImpl.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace com.Sconit.Service.Impl
{
    using System.Collections;
    using System.Collections.Generic;
    using com.Sconit.Persistence;
    using NHibernate.Criterion;
    using NHibernate.Type;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class QueryMgrImpl : BaseMgr, IQueryMgr
    {
        public INHQueryDao queryDao { get; set; }
        public ISqlDao sqlDao { get; set; }

        public T FindById<T>(object id)
        {
            return queryDao.FindById<T>(id);
        }

        public IList<T> FindAll<T>()
        {
            return queryDao.FindAll<T>();
        }

        public IList<T> FindAll<T>(int firstRow, int maxRows)
        {
            return queryDao.FindAll<T>(firstRow, maxRows);
        }

        public IList FindAll(string hql)
        {
            return queryDao.FindAllWithCustomQuery(hql);
        }

        public IList FindAll(string hql, object value)
        {
            return queryDao.FindAllWithCustomQuery(hql, value);
        }

        public IList FindAll(string hql, object value, IType type)
        {
            return queryDao.FindAllWithCustomQuery(hql, value, type);
        }

        public IList FindAll(string hql, object[] values)
        {
            return queryDao.FindAllWithCustomQuery(hql, values);
        }

        public IList FindAll(string hql, IEnumerable<object> values)
        {
            return queryDao.FindAllWithCustomQuery(hql, values.ToArray());
        }

        public IList FindAll(string hql, object[] values, IType[] types)
        {
            return queryDao.FindAllWithCustomQuery(hql, values, types);
        }

        public IList FindAll(string hql, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithCustomQuery(hql, firstRow, maxRows);
        }

        public IList FindAll(string hql, object value, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithCustomQuery(hql, value, firstRow, maxRows);
        }

        public IList FindAll(string hql, object value, IType type, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithCustomQuery(hql, value, type, firstRow, maxRows);
        }

        public IList FindAll(string hql, object[] values, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithCustomQuery(hql, values, firstRow, maxRows);
        }

        public IList FindAll(string hql, object[] values, IType[] types, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithCustomQuery(hql, values, types, firstRow, maxRows);
        }

        public IList<T> FindAll<T>(string hql)
        {
            return queryDao.FindAllWithCustomQuery<T>(hql);
        }

        public IList<T> FindAll<T>(string hql, object value)
        {
            return queryDao.FindAllWithCustomQuery<T>(hql, value);
        }

        public IList<T> FindAll<T>(string hql, object value, IType type)
        {
            return queryDao.FindAllWithCustomQuery<T>(hql, value, type);
        }

        public IList<T> FindAll<T>(string hql, object[] values)
        {
            return queryDao.FindAllWithCustomQuery<T>(hql, values);
        }

        public IList<T> FindAll<T>(string hql, IEnumerable<object> values)
        {
            return queryDao.FindAllWithCustomQuery<T>(hql, values.ToArray());
        }

        public IList<T> FindAllIn<T>(string hql, IEnumerable<object> inParam, IEnumerable<object> param = null)
        {
            if (inParam == null || inParam.Count() == 0)
            {
                return null;
            }
            List<T> tList = new List<T>();

            //把in转成or
            //from Item where Code in ( ? 
            hql = hql.TrimEnd(); //from Item Id in ( ?
            hql = hql.Remove(hql.Length - 1).TrimEnd();//删除"?"//from Item where Code in (
            hql = hql.Remove(hql.Length - 1).TrimEnd();//删除"("//from Item where Code in
            hql = hql.Remove(hql.Length - 2).TrimEnd();//删除"in"//from Item where Code
            string inField = hql.Split(' ').Last();//Code
            hql = hql.Remove(hql.Length - inField.Length);//from Item where 

            hql = string.Format("{0} ({1}=? ", hql, inField);
            //hqlStr.Append("(");//from Item where (
            //hqlStr.Append(inField);//from Item where (Code
            //hqlStr.Append("=? ");//from Item where (Code=? 

            int inParamCount = 2000;
            if (param != null)
            {
                inParamCount -= param.Count();
            }

            int skipCount = 0;
            while (true)
            {
                var hqlStr = new StringBuilder(hql);
                List<object> paramValue = new List<object>();
                var batchinParam = inParam.Skip(skipCount).Take(inParamCount);
                if (batchinParam.Count() == 0)
                {
                    break;
                }
                skipCount += inParamCount;

                for (int i = 0; i < batchinParam.Count(); i++)
                {
                    if (i > 0)
                    {
                        hqlStr.Append("or ");//from Item where (Code=? or 
                        hqlStr.Append(inField);//from Item where (Code=? or Code
                        hqlStr.Append("=? ");//from Item where (Code=? or Code=
                    }
                }
                hqlStr.Append(")");

                if (param != null)
                {
                    paramValue.AddRange(param);
                }
                paramValue.AddRange(batchinParam);
                var list = queryDao.FindAllWithCustomQuery<T>(hql, paramValue.ToArray());
                if (list != null)
                {
                    tList.AddRange(list);
                }
            }
            return tList;
        }

        public IList<T> FindAll<T>(string hql, object[] values, IType[] types)
        {
            return queryDao.FindAllWithCustomQuery<T>(hql, values, types);
        }

        public IList<T> FindAll<T>(string hql, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithCustomQuery<T>(hql, firstRow, maxRows);
        }

        public IList<T> FindAll<T>(string hql, object value, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithCustomQuery<T>(hql, value, firstRow, maxRows);
        }

        public IList<T> FindAll<T>(string hql, object value, IType type, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithCustomQuery<T>(hql, value, type, firstRow, maxRows);
        }

        public IList<T> FindAll<T>(string hql, object[] values, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithCustomQuery<T>(hql, values, firstRow, maxRows);
        }

        public IList<T> FindAll<T>(string hql, object[] values, IType[] types, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithCustomQuery<T>(hql, values, types, firstRow, maxRows);
        }

        public IList<T> FindAll<T>(string hql, IDictionary<string, object> param)
        {
            return queryDao.FindAllWithCustomQuery<T>(hql, param);
        }

        public IList<T> FindAll<T>(string hql, IDictionary<string, object> param, IType[] types)
        {
            return queryDao.FindAllWithCustomQuery<T>(hql, param, types);
        }

        public IList<T> FindAll<T>(string hql, IDictionary<string, object> param, IType[] types, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithCustomQuery<T>(hql, param, types, firstRow, maxRows);
        }

        public IList FindAll(DetachedCriteria criteria)
        {
            return queryDao.FindAll(criteria);
        }

        public IList FindAll(DetachedCriteria criteria, int firstRow, int maxRows)
        {
            return queryDao.FindAll(criteria, firstRow, maxRows);
        }

        public IList<T> FindAll<T>(DetachedCriteria criteria)
        {
            return queryDao.FindAll<T>(criteria);
        }

        public IList<T> FindAll<T>(DetachedCriteria criteria, int firstRow, int maxRows)
        {
            return queryDao.FindAll<T>(criteria, firstRow, maxRows);
        }

        public IList FindAllWithNamedQuery(string namedQuery)
        {
            return queryDao.FindAllWithNamedQuery(namedQuery);
        }

        public IList FindAllWithNamedQuery(string namedQuery, object value)
        {
            return queryDao.FindAllWithNamedQuery(namedQuery, value);
        }

        public IList FindAllWithNamedQuery(string namedQuery, object value, IType type)
        {
            return queryDao.FindAllWithNamedQuery(namedQuery, value, type);
        }

        public IList FindAllWithNamedQuery(string namedQuery, object[] values)
        {
            return queryDao.FindAllWithNamedQuery(namedQuery, values);
        }

        public IList FindAllWithNamedQuery(string namedQuery, object[] values, IType[] types)
        {
            return queryDao.FindAllWithNamedQuery(namedQuery, values, types);
        }

        public IList FindAllWithNamedQuery(string namedQuery, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithNamedQuery(namedQuery, firstRow, maxRows);
        }

        public IList FindAllWithNamedQuery(string namedQuery, object value, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithNamedQuery(namedQuery, value, firstRow, maxRows);
        }

        public IList FindAllWithNamedQuery(string namedQuery, object value, IType type, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithNamedQuery(namedQuery, value, type, firstRow, maxRows);
        }

        public IList FindAllWithNamedQuery(string namedQuery, object[] values, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithNamedQuery(namedQuery, values, firstRow, maxRows);
        }

        public IList FindAllWithNamedQuery(string namedQuery, object[] values, IType[] types, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithNamedQuery(namedQuery, values, types, firstRow, maxRows);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery)
        {
            return queryDao.FindAllWithNamedQuery<T>(namedQuery);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery, object value)
        {
            return queryDao.FindAllWithNamedQuery<T>(namedQuery, value);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery, object value, IType type)
        {
            return queryDao.FindAllWithNamedQuery<T>(namedQuery, value, type);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery, object[] values)
        {
            return queryDao.FindAllWithNamedQuery<T>(namedQuery, values);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery, object[] values, IType[] types)
        {
            return queryDao.FindAllWithNamedQuery<T>(namedQuery, values, types);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithNamedQuery<T>(namedQuery, firstRow, maxRows);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery, object value, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithNamedQuery<T>(namedQuery, value, firstRow, maxRows);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery, object value, IType type, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithNamedQuery<T>(namedQuery, value, type, firstRow, maxRows);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery, object[] values, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithNamedQuery<T>(namedQuery, values, firstRow, maxRows);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery, object[] values, IType[] types, int firstRow, int maxRows)
        {
            return queryDao.FindAllWithNamedQuery<T>(namedQuery, values, types, firstRow, maxRows);
        }

        public IList FindAllWithNativeSql(string sql)
        {
            return queryDao.FindAllWithNativeSql(sql);
        }

        public IList FindAllWithNativeSql(string sql, object value)
        {
            return queryDao.FindAllWithNativeSql(sql, value);
        }

        public IList FindAllWithNativeSql(string sql, object value, IType type)
        {
            return queryDao.FindAllWithNativeSql(sql, value, type);
        }

        public IList FindAllWithNativeSql(string sql, object[] values)
        {
            return queryDao.FindAllWithNativeSql(sql, values);
        }

        public IList FindAllWithNativeSql(string sql, object[] values, IType[] types)
        {
            return queryDao.FindAllWithNativeSql(sql, values, types);
        }

        public IList<T> FindAllWithNativeSql<T>(string sql)
        {
            return queryDao.FindAllWithNativeSql<T>(sql);
        }

        public IList<T> FindAllWithNativeSql<T>(string sql, object value)
        {
            return queryDao.FindAllWithNativeSql<T>(sql, value);
        }

        public IList<T> FindAllWithNativeSql<T>(string sql, object value, IType type)
        {
            return queryDao.FindAllWithNativeSql<T>(sql, value, type);
        }

        public IList<T> FindAllWithNativeSql<T>(string sql, object[] values)
        {
            return queryDao.FindAllWithNativeSql<T>(sql, values);
        }

        public IList<T> FindAllWithNativeSql<T>(string sql, object[] values, IType[] types)
        {
            return queryDao.FindAllWithNativeSql<T>(sql, values, types);
        }

        public IList<T> FindEntityWithNativeSql<T>(string sql)
        {
            return queryDao.FindEntityWithNativeSql<T>(sql);
        }

        public IList<T> FindEntityWithNativeSql<T>(string sql, object value)
        {
            return queryDao.FindEntityWithNativeSql<T>(sql, value);
        }

        public IList<T> FindEntityWithNativeSql<T>(string sql, object value, IType type)
        {
            return queryDao.FindEntityWithNativeSql<T>(sql, value, type);
        }

        public IList<T> FindEntityWithNativeSql<T>(string sql, object[] values)
        {
            return queryDao.FindEntityWithNativeSql<T>(sql, values);
        }

        public IList<T> FindEntityWithNativeSql<T>(string sql, object[] values, IType[] types)
        {
            return queryDao.FindEntityWithNativeSql<T>(sql, values, types);
        }

        public IList<T> FindEntityWithNativeSqlIn<T>(string sql, IEnumerable<object> inValues, IEnumerable<object> values = null)
        {
            if (inValues == null || inValues.Count() == 0)
            {
                return null;
            }
            List<T> tList = new List<T>();

            int inParamCount = 2000;
            if (values != null)
            {
                inParamCount -= values.Count();
            }

            //把in转成or
            //from Item where Code in ( ? 
            sql = sql.TrimEnd(); //from Item Id in ( ?
            sql = sql.Remove(sql.Length - 1).TrimEnd();//删除"?"//from Item where Code in (
            sql = sql.Remove(sql.Length - 1).TrimEnd();//删除"("//from Item where Code in
            sql = sql.Remove(sql.Length - 2).TrimEnd();//删除"in"//from Item where Code
            string inField = sql.Split(' ').Last();//Code
            sql = sql.Remove(sql.Length - inField.Length);//from Item where 

            sql = string.Format("{0} ({1}=? ", sql, inField);
            //sqlStr.Append("(");//from Item where (
            //sqlStr.Append(inField);//from Item where (Code
            //sqlStr.Append("=? ");//from Item where (Code=? 

            int skipCount = 0;
            while (true)
            {
                var sqlStr = new StringBuilder(sql);
                List<object> paramValue = new List<object>();
                var batchinParam = inValues.Skip(skipCount).Take(inParamCount);
                if (batchinParam.Count() == 0)
                {
                    break;
                }
                skipCount += inParamCount;

                for (int i = 0; i < batchinParam.Count(); i++)
                {
                    if (i > 0)
                    {
                        sqlStr.Append("or ");//from Item where (Code=? or 
                        sqlStr.Append(inField);//from Item where (Code=? or Code
                        sqlStr.Append("=? ");//from Item where (Code=? or Code=
                    }
                }
                sqlStr.Append(")");

                if (values != null)
                {
                    paramValue.AddRange(values);
                }
                paramValue.AddRange(batchinParam);
                var list = queryDao.FindEntityWithNativeSql<T>(sql, paramValue.ToArray());
                if (list != null)
                {
                    tList.AddRange(list);
                }
            }
            return tList;
        }

        public IList<T> FindAllWithNativeSqlIn<T>(string sql, IEnumerable<object> inValues, IEnumerable<object> values = null)
        {
            if (inValues == null || inValues.Count() == 0)
            {
                return null;
            }
            List<T> tList = new List<T>();

            int inParamCount = 2000;
            if (values != null)
            {
                inParamCount -= values.Count();
            }

            //把in转成or
            //from Item where Code in ( ? 
            sql = sql.TrimEnd(); //from Item Id in ( ?
            sql = sql.Remove(sql.Length - 1).TrimEnd();//删除"?"//from Item where Code in (
            sql = sql.Remove(sql.Length - 1).TrimEnd();//删除"("//from Item where Code in
            sql = sql.Remove(sql.Length - 2).TrimEnd();//删除"in"//from Item where Code
            string inField = sql.Split(' ').Last();//Code
            sql = sql.Remove(sql.Length - inField.Length);//from Item where 

            sql = string.Format("{0} ({1}=? ", sql, inField);
            //sqlStr.Append("(");//from Item where (
            //sqlStr.Append(inField);//from Item where (Code
            //sqlStr.Append("=? ");//from Item where (Code=? 

            int skipCount = 0;
            while (true)
            {
                var sqlStr = new StringBuilder(sql);
                List<object> paramValue = new List<object>();
                var batchinParam = inValues.Skip(skipCount).Take(inParamCount);
                if (batchinParam.Count() == 0)
                {
                    break;
                }
                skipCount += inParamCount;

                for (int i = 0; i < batchinParam.Count(); i++)
                {
                    if (i > 0)
                    {
                        sqlStr.Append("or ");//from Item where (Code=? or 
                        sqlStr.Append(inField);//from Item where (Code=? or Code
                        sqlStr.Append("=? ");//from Item where (Code=? or Code=
                    }
                }
                sqlStr.Append(")");

                if (values != null)
                {
                    paramValue.AddRange(values);
                }
                paramValue.AddRange(batchinParam);
                var list = queryDao.FindAllWithNativeSql<T>(sql, paramValue.ToArray());
                if (list != null)
                {
                    tList.AddRange(list);
                }
            }
            return tList;
        }

        public DataSet GetDatasetBySql(string commandText, SqlParameter[] commandParameters)
        {
            return sqlDao.GetDatasetBySql(commandText, commandParameters);
        }

        public DataSet GetDatasetByStoredProcedure(string commandText, SqlParameter[] commandParameters)
        {
            return sqlDao.GetDatasetByStoredProcedure(commandText, commandParameters);
        }
    }
}
