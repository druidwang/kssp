namespace com.Sconit.Service.Impl
{
    #region retrive
    using System;
    using System.Collections;
    using Castle.Services.Transaction;
    using com.Sconit.Entity;
    using com.Sconit.Entity.ACC;
    using com.Sconit.Persistence;
    using NHibernate.Type;
    using System.Collections.Generic;
    using NHibernate.Criterion;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    [Transactional]
    public class GenericMgrImpl : BaseMgr, IGenericMgr, IQueryMgr
    {
        public GenericMgrImpl(INHDao dao)
        {
            this.dao = dao;
        }
        /// <summary>
        /// NHibernate数据获取对象
        /// </summary>
        private INHDao dao { get; set; }
        public ISqlDao sqlDao { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        [Transaction(TransactionMode.Requires)]
        public void Save(object instance)
        {
            IAuditable auditable = instance as IAuditable;
            if (auditable != null)
            {
                DateTime dateTimeNow = DateTime.Now;
                User user = SecurityContextHolder.Get();
                if (auditable.CreateUserName == null)
                {
                    auditable.CreateUserId = user.Id;
                    auditable.CreateUserName = user.FullName;
                    auditable.CreateDate = dateTimeNow;
                    auditable.LastModifyUserId = user.Id;
                    auditable.LastModifyUserName = user.FullName;
                    auditable.LastModifyDate = dateTimeNow;
                }
            }
            dao.Save(instance);
        }

        [Transaction(TransactionMode.Requires)]
        public bool BatchInsert<T>(IList<T> instanceList)
        {
            if (instanceList != null && instanceList.Count > 0)
            {
                DateTime dateTimeNow = DateTime.Now;
                User user = SecurityContextHolder.Get();

                foreach (var instance in instanceList)
                {
                    IAuditable auditable = instance as IAuditable;
                    if (auditable != null)
                    {
                        if (auditable.CreateUserName == null)
                        {
                            auditable.CreateUserId = user.Id;
                            auditable.CreateUserName = user.FullName;
                            auditable.CreateDate = dateTimeNow;
                            auditable.LastModifyUserId = user.Id;
                            auditable.LastModifyUserName = user.FullName;
                            auditable.LastModifyDate = dateTimeNow;
                        }
                    }
                }
                return dao.BatchInsert(instanceList);
            }
            else
            {
                return false;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void BulkInsert<T>(IList<T> instanceList)
        {
            if (instanceList != null && instanceList.Count > 0)
            {
                DateTime dateTimeNow = DateTime.Now;
                User user = SecurityContextHolder.Get();

                foreach (var instance in instanceList)
                {
                    IAuditable auditable = instance as IAuditable;
                    if (auditable != null)
                    {
                        if (auditable.CreateUserName == null)
                        {
                            auditable.CreateUserId = user.Id;
                            auditable.CreateUserName = user.FullName;
                            auditable.CreateDate = dateTimeNow;
                            auditable.LastModifyUserId = user.Id;
                            auditable.LastModifyUserName = user.FullName;
                            auditable.LastModifyDate = dateTimeNow;
                        }
                    }
                }

                DataTable dataTable = IListToDataTable<T>(instanceList);
                sqlDao.BulkInsert(dataTable);
            }
        }

        private DataTable IListToDataTable<T>(IList<T> instanceList)
        {
            if (instanceList == null || instanceList.Count == 0)
            {
                return null;
            }
            string tableName = string.Empty;
            Dictionary<string, string> propertyAndColumnNames = new Dictionary<string, string>();
            dao.GetTableProperty(instanceList.First(), out tableName, out propertyAndColumnNames);

            DataTable dt = new DataTable(tableName);

            System.Reflection.PropertyInfo[] myPropertyInfo = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (T t in instanceList)
            {
                if (t == null)
                {
                    continue;
                }

                DataRow row = dt.NewRow();

                for (int i = 0, j = myPropertyInfo.Length; i < j; i++)
                {
                    System.Reflection.PropertyInfo pi = myPropertyInfo[i];
                    if (pi.CanWrite)
                    {
                        if (propertyAndColumnNames.ContainsKey(pi.Name))
                        {
                            string columnName = propertyAndColumnNames[pi.Name];
                            if (dt.Columns[columnName] == null)
                            {
                                DataColumn column = new DataColumn(columnName);
                                if (!pi.PropertyType.ToString().Contains("System.Nullable"))
                                {
                                    column = new DataColumn(columnName, pi.PropertyType);
                                }
                                dt.Columns.Add(column);
                            }
                            row[columnName] = pi.GetValue(t, null);
                        }
                    }
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateWithTrim(object instance)
        {
            TrimInstance(instance);
            this.Create(instance);
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateWithTrim(object instance)
        {
            TrimInstance(instance);
            this.Update(instance);
        }

        private static void TrimInstance(object instance)
        {
            PropertyInfo[] propertyInfo = instance.GetType().GetProperties();
            foreach (PropertyInfo pi in propertyInfo)
            {
                object oldValue = pi.GetValue(instance, null);
                if (pi.CanWrite && pi.PropertyType.ToString() == "System.String" && oldValue != null)
                {
                    string newValue = oldValue.ToString().Trim();
                    if (newValue == string.Empty)
                    {
                        newValue = null;
                    }
                    pi.SetValue(instance, newValue, null);
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void Create(IList instanceList)
        {
            DateTime dateTimeNow = DateTime.Now;
            User user = SecurityContextHolder.Get();
            foreach (var instance in instanceList)
            {
                IAuditable auditable = instance as IAuditable;
                if (auditable != null)
                {
                    if (user != null)
                    {
                        auditable.CreateUserId = user.Id;
                        auditable.CreateUserName = user.FullName;
                        auditable.LastModifyUserId = user.Id;
                        auditable.LastModifyUserName = user.FullName;
                    }
                    auditable.CreateDate = dateTimeNow;
                    auditable.LastModifyDate = dateTimeNow;
                }
            }

            string tableName = string.Empty;
            Dictionary<string, string> propertyAndColumnNames = new Dictionary<string, string>();
            dao.GetTableProperty(instanceList[0], out tableName, out propertyAndColumnNames);
            string sql = string.Empty;
            foreach (var instance in instanceList)
            {

            }

            sqlDao.ExecuteSql(sql, null);
        }

        [Transaction(TransactionMode.Requires)]
        public void Create(object instance)
        {
            IAuditable auditable = instance as IAuditable;

            if (auditable != null)
            {
                DateTime dateTimeNow = DateTime.Now;
                User user = SecurityContextHolder.Get();
                if (user != null)
                {
                    auditable.CreateUserId = user.Id;
                    auditable.CreateUserName = user.FullName;
                    auditable.LastModifyUserId = user.Id;
                    auditable.LastModifyUserName = user.FullName;
                }
                auditable.CreateDate = dateTimeNow;
                auditable.LastModifyDate = dateTimeNow;
            }

            dao.Create(instance);
        }

        [Transaction(TransactionMode.Requires)]
        public void Update(object instance)
        {
            IAuditable auditable = instance as IAuditable;
            if (auditable != null)
            {
                DateTime dateTimeNow = DateTime.Now;
                User user = SecurityContextHolder.Get();
                auditable.LastModifyUserId = user.Id;
                auditable.LastModifyUserName = user.FullName;
                auditable.LastModifyDate = dateTimeNow;
            }
            dao.Update(instance);
        }

        [Transaction(TransactionMode.Requires)]
        public void MergeUpdate(object instance)
        {
            IAuditable auditable = instance as IAuditable;
            if (auditable != null)
            {
                DateTime dateTimeNow = DateTime.Now;
                User user = SecurityContextHolder.Get();
                auditable.LastModifyUserId = user.Id;
                auditable.LastModifyUserName = user.FullName;
                auditable.LastModifyDate = dateTimeNow;
            }
            dao.MergeUpdate(instance);
        }

        [Transaction(TransactionMode.Requires)]
        public int Update(string queryString)
        {
            return dao.ExecuteUpdateWithCustomQuery(queryString, (object[])null, (IType[])null);
        }

        [Transaction(TransactionMode.Requires)]
        public int Update(string queryString, object value)
        {
            return dao.ExecuteUpdateWithCustomQuery(queryString, new object[] { value }, (IType[])null);
        }

        [Transaction(TransactionMode.Requires)]
        public int Update(string queryString, object value, IType type)
        {
            return dao.ExecuteUpdateWithCustomQuery(queryString, new object[] { value }, new IType[] { type });
        }

        [Transaction(TransactionMode.Requires)]
        public int Update(string queryString, object[] values)
        {
            return dao.ExecuteUpdateWithCustomQuery(queryString, values, (IType[])null);
        }

        [Transaction(TransactionMode.Requires)]
        public int Update(string queryString, object[] values, IType[] types)
        {
            return dao.ExecuteUpdateWithCustomQuery(queryString, values, types);
        }

        [Transaction(TransactionMode.Requires)]
        public void Delete(object instance)
        {
            dao.Delete(instance);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteById<T>(object id)
        {
            object instance = this.FindById<T>(id);
            dao.Delete(instance);
        }

        [Transaction(TransactionMode.Requires)]
        public void Delete(IList instances)
        {
            if (instances != null && instances.Count > 0)
            {
                foreach (object inst in instances)
                {
                    dao.Delete(inst);
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void Delete<T>(IList<T> instances)
        {
            if (instances != null && instances.Count > 0)
            {
                foreach (object inst in instances)
                {
                    dao.Delete(inst);
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void DeleteAll(Type type)
        {
            dao.DeleteAll(type);
        }

        public void FlushSession()
        {
            this.dao.FlushSession();
        }

        public void CleanSession()
        {
            dao.CleanSession();
        }

        [Transaction(TransactionMode.Requires)]
        public void Delete(string hqlString)
        {
            dao.Delete(hqlString);
        }

        //public void Delete(string hqlString, object value)
        //{
        //    dao.Delete(hqlString, value);
        //}

        [Transaction(TransactionMode.Requires)]
        public void Delete(string hqlString, object value, IType type)
        {
            dao.Delete(hqlString, value, type);
        }

        //public void Delete(string hqlString, object[] values)
        //{
        //    dao.Delete(hqlString, values);
        //}

        [Transaction(TransactionMode.Requires)]
        public void Delete(string hqlString, object[] values, IType[] types)
        {
            dao.Delete(hqlString, values, types);
        }

        public T FindById<T>(object id)
        {
            return dao.FindById<T>(id);
        }

        public IList<T> FindAll<T>()
        {
            return dao.FindAll<T>();
        }

        public IList<T> FindAll<T>(int firstRow, int maxRows)
        {
            return dao.FindAll<T>(firstRow, maxRows);
        }

        public IList FindAll(string hql)
        {
            return dao.FindAllWithCustomQuery(hql);
        }

        public IList FindAll(string hql, object value)
        {
            return dao.FindAllWithCustomQuery(hql, value);
        }

        public IList FindAll(string hql, object value, IType type)
        {
            return dao.FindAllWithCustomQuery(hql, value, type);
        }

        public IList FindAll(string hql, object[] values)
        {
            return dao.FindAllWithCustomQuery(hql, values);
        }

        public IList FindAll(string hql, IEnumerable<object> values)
        {
            return dao.FindAllWithCustomQuery(hql, values.ToArray());
        }

        public IList<T> FindAll<T>(string hql, IEnumerable<object> values)
        {
            return dao.FindAllWithCustomQuery<T>(hql, values.ToArray());
        }

        public IList FindAll(string hql, object[] values, IType[] types)
        {
            return dao.FindAllWithCustomQuery(hql, values, types);
        }

        public IList FindAll(string hql, int firstRow, int maxRows)
        {
            return dao.FindAllWithCustomQuery(hql, firstRow, maxRows);
        }

        public IList FindAll(string hql, object value, int firstRow, int maxRows)
        {
            return dao.FindAllWithCustomQuery(hql, value, firstRow, maxRows);
        }

        public IList FindAll(string hql, object value, IType type, int firstRow, int maxRows)
        {
            return dao.FindAllWithCustomQuery(hql, value, type, firstRow, maxRows);
        }

        public IList FindAll(string hql, object[] values, int firstRow, int maxRows)
        {
            return dao.FindAllWithCustomQuery(hql, values, firstRow, maxRows);
        }

        public IList FindAll(string hql, object[] values, IType[] types, int firstRow, int maxRows)
        {
            return dao.FindAllWithCustomQuery(hql, values, types, firstRow, maxRows);
        }

        public IList<T> FindAll<T>(string hql)
        {
            return dao.FindAllWithCustomQuery<T>(hql);
        }

        public IList<T> FindAll<T>(string hql, object value)
        {
            return dao.FindAllWithCustomQuery<T>(hql, value);
        }

        public IList<T> FindAll<T>(string hql, object value, IType type)
        {
            return dao.FindAllWithCustomQuery<T>(hql, value, type);
        }

        public IList<T> FindAll<T>(string hql, object[] values)
        {
            return dao.FindAllWithCustomQuery<T>(hql, values);
        }

        public IList<T> FindAll<T>(string hql, object[] values, IType[] types)
        {
            return dao.FindAllWithCustomQuery<T>(hql, values, types);
        }

        public IList<T> FindAll<T>(string hql, int firstRow, int maxRows)
        {
            return dao.FindAllWithCustomQuery<T>(hql, firstRow, maxRows);
        }

        public IList<T> FindAll<T>(string hql, object value, int firstRow, int maxRows)
        {
            return dao.FindAllWithCustomQuery<T>(hql, value, firstRow, maxRows);
        }

        public IList<T> FindAll<T>(string hql, object value, IType type, int firstRow, int maxRows)
        {
            return dao.FindAllWithCustomQuery<T>(hql, value, type, firstRow, maxRows);
        }

        public IList<T> FindAll<T>(string hql, object[] values, int firstRow, int maxRows)
        {
            return dao.FindAllWithCustomQuery<T>(hql, values, firstRow, maxRows);
        }

        public IList<T> FindAll<T>(string hql, object[] values, IType[] types, int firstRow, int maxRows)
        {
            return dao.FindAllWithCustomQuery<T>(hql, values, types, firstRow, maxRows);
        }

        public IList<T> FindAll<T>(string hql, IDictionary<string, object> param)
        {
            return dao.FindAllWithCustomQuery<T>(hql, param);
        }

        public IList<T> FindAll<T>(string hql, IDictionary<string, object> param, IType[] types)
        {
            return dao.FindAllWithCustomQuery<T>(hql, param, types);
        }

        public IList<T> FindAll<T>(string hql, IDictionary<string, object> param, IType[] types, int firstRow, int maxRows)
        {
            return dao.FindAllWithCustomQuery<T>(hql, param, types, firstRow, maxRows);
        }

        public IList FindAll(DetachedCriteria criteria)
        {
            return dao.FindAll(criteria);
        }

        public IList FindAll(DetachedCriteria criteria, int firstRow, int maxRows)
        {
            return dao.FindAll(criteria, firstRow, maxRows);
        }

        public IList<T> FindAll<T>(DetachedCriteria criteria)
        {
            return dao.FindAll<T>(criteria);
        }

        public IList<T> FindAll<T>(DetachedCriteria criteria, int firstRow, int maxRows)
        {
            return dao.FindAll<T>(criteria, firstRow, maxRows);
        }

        public IList FindAllWithNamedQuery(string namedQuery)
        {
            return dao.FindAllWithNamedQuery(namedQuery);
        }

        public IList FindAllWithNamedQuery(string namedQuery, object value)
        {
            return dao.FindAllWithNamedQuery(namedQuery, value);
        }

        public IList FindAllWithNamedQuery(string namedQuery, object value, IType type)
        {
            return dao.FindAllWithNamedQuery(namedQuery, value, type);
        }

        public IList FindAllWithNamedQuery(string namedQuery, object[] values)
        {
            return dao.FindAllWithNamedQuery(namedQuery, values);
        }

        public IList FindAllWithNamedQuery(string namedQuery, object[] values, IType[] types)
        {
            return dao.FindAllWithNamedQuery(namedQuery, values, types);
        }

        public IList FindAllWithNamedQuery(string namedQuery, int firstRow, int maxRows)
        {
            return dao.FindAllWithNamedQuery(namedQuery, firstRow, maxRows);
        }

        public IList FindAllWithNamedQuery(string namedQuery, object value, int firstRow, int maxRows)
        {
            return dao.FindAllWithNamedQuery(namedQuery, value, firstRow, maxRows);
        }

        public IList FindAllWithNamedQuery(string namedQuery, object value, IType type, int firstRow, int maxRows)
        {
            return dao.FindAllWithNamedQuery(namedQuery, value, type, firstRow, maxRows);
        }

        public IList FindAllWithNamedQuery(string namedQuery, object[] values, int firstRow, int maxRows)
        {
            return dao.FindAllWithNamedQuery(namedQuery, values, firstRow, maxRows);
        }

        public IList FindAllWithNamedQuery(string namedQuery, object[] values, IType[] types, int firstRow, int maxRows)
        {
            return dao.FindAllWithNamedQuery(namedQuery, values, types, firstRow, maxRows);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery)
        {
            return dao.FindAllWithNamedQuery<T>(namedQuery);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery, object value)
        {
            return dao.FindAllWithNamedQuery<T>(namedQuery, value);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery, object value, IType type)
        {
            return dao.FindAllWithNamedQuery<T>(namedQuery, value, type);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery, object[] values)
        {
            return dao.FindAllWithNamedQuery<T>(namedQuery, values);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery, object[] values, IType[] types)
        {
            return dao.FindAllWithNamedQuery<T>(namedQuery, values, types);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery, int firstRow, int maxRows)
        {
            return dao.FindAllWithNamedQuery<T>(namedQuery, firstRow, maxRows);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery, object value, int firstRow, int maxRows)
        {
            return dao.FindAllWithNamedQuery<T>(namedQuery, value, firstRow, maxRows);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery, object value, IType type, int firstRow, int maxRows)
        {
            return dao.FindAllWithNamedQuery<T>(namedQuery, value, type, firstRow, maxRows);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery, object[] values, int firstRow, int maxRows)
        {
            return dao.FindAllWithNamedQuery<T>(namedQuery, values, firstRow, maxRows);
        }

        public IList<T> FindAllWithNamedQuery<T>(string namedQuery, object[] values, IType[] types, int firstRow, int maxRows)
        {
            return dao.FindAllWithNamedQuery<T>(namedQuery, values, types, firstRow, maxRows);
        }

        public IList FindAllWithNativeSql(string sql)
        {
            return dao.FindAllWithNativeSql(sql);
        }

        public IList FindAllWithNativeSql(string sql, object value)
        {
            return dao.FindAllWithNativeSql(sql, value);
        }

        public IList FindAllWithNativeSql(string sql, object value, IType type)
        {
            return dao.FindAllWithNativeSql(sql, value, type);
        }

        public IList FindAllWithNativeSql(string sql, object[] values)
        {
            return dao.FindAllWithNativeSql(sql, values);
        }

        public IList FindAllWithNativeSql(string sql, object[] values, IType[] types)
        {
            return dao.FindAllWithNativeSql(sql, values, types);
        }

        public IList<T> FindAllWithNativeSql<T>(string sql)
        {
            return dao.FindAllWithNativeSql<T>(sql);
        }

        public IList<T> FindAllWithNativeSql<T>(string sql, object value)
        {
            return dao.FindAllWithNativeSql<T>(sql, value);
        }

        public IList<T> FindAllWithNativeSql<T>(string sql, object value, IType type)
        {
            return dao.FindAllWithNativeSql<T>(sql, value, type);
        }

        public IList<T> FindAllWithNativeSql<T>(string sql, object[] values)
        {
            return dao.FindAllWithNativeSql<T>(sql, values);
        }

        public IList<T> FindAllWithNativeSql<T>(string sql, object[] values, IType[] types)
        {
            return dao.FindAllWithNativeSql<T>(sql, values, types);
        }

        public IList<T> FindEntityWithNativeSql<T>(string sql)
        {
            return dao.FindEntityWithNativeSql<T>(sql);
        }

        public IList<T> FindEntityWithNativeSql<T>(string sql, object value)
        {
            return dao.FindEntityWithNativeSql<T>(sql, value);
        }

        public IList<T> FindEntityWithNativeSql<T>(string sql, object value, IType type)
        {
            return dao.FindEntityWithNativeSql<T>(sql, value, type);
        }

        public IList<T> FindEntityWithNativeSql<T>(string sql, object[] values)
        {
            return dao.FindEntityWithNativeSql<T>(sql, values);
        }

        public IList<T> FindEntityWithNativeSql<T>(string sql, object[] values, IType[] types)
        {
            return dao.FindEntityWithNativeSql<T>(sql, values, types);
        }

        public DataSet GetDatasetBySql(string commandText, SqlParameter[] commandParameters)
        {
            return sqlDao.GetDatasetBySql(commandText, commandParameters);
        }

        public DataSet GetDatasetByStoredProcedure(string commandText, SqlParameter[] commandParameters)
        {
            return sqlDao.GetDatasetByStoredProcedure(commandText, commandParameters);
        }
        public Int32 ExecuteStoredProcedure(string commandText)
        {
            return sqlDao.ExecuteStoredProcedure(commandText, null);
        }

        public Int32 ExecuteStoredProcedure(string commandText, SqlParameter[] commandParameters)
        {
            return sqlDao.ExecuteStoredProcedure(commandText, commandParameters);
        }

        public Int32 ExecuteSql(string commandText)
        {
            return sqlDao.ExecuteSql(commandText, null);
        }

        public Int32 ExecuteSql(string commandText, SqlParameter[] commandParameters)
        {
            return sqlDao.ExecuteSql(commandText, commandParameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hql">select i from Item where Code in ( ?</param>
        /// <param name="inParam"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IList<T> FindAllIn<T>(string hql, IEnumerable<object> inParam, IEnumerable<object> param = null)
        {
            inParam = inParam.Where(i => i != null).ToList();
            if (inParam == null || inParam.Count() == 0)
            {
                return null;
            }
            List<T> tList = new List<T>();
            int inParamCount = 1000;
            if (param != null)
            {
                inParamCount -= param.Count();
            }
            int skipCount = 0;
            while (true)
            {
                var hqlStr = new StringBuilder(hql);
                var paramList = new List<object>();

                var batchinParam = inParam.Skip(skipCount).Take(inParamCount).ToList();//得到in参数
                if (batchinParam.Count() == 0)
                {
                    break;
                }
                skipCount += inParamCount;
                for (int i = 1; i < batchinParam.Count(); i++)
                {
                    hqlStr.Append(",?");
                }
                hqlStr.Append(")");
                if (param != null)
                {
                    paramList.AddRange(param);
                }
                paramList.AddRange(batchinParam);
                var list = dao.FindAllWithCustomQuery<T>(hqlStr.ToString(), paramList.ToArray());
                if (list != null)
                {
                    tList.AddRange(list);
                }
            }
            return tList;
        }

        public IList<T> FindEntityWithNativeSqlIn<T>(string sql, IEnumerable<object> inValues, IEnumerable<object> values = null)
        {
            if (inValues == null || inValues.Count() == 0)
            {
                return null;
            }
            List<T> tList = new List<T>();

            int inParamCount = 1000;
            if (values != null)
            {
                inParamCount -= values.Count();
            }

            //把in转成or//from Item where Code in ( ? 
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
                var batchinParam = inValues.Skip(skipCount).Take(inParamCount).ToList();
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
                sqlStr.Append(" )");

                if (values != null)
                {
                    paramValue.AddRange(values);
                }
                paramValue.AddRange(batchinParam);
                var list = dao.FindEntityWithNativeSql<T>(sqlStr.ToString(), paramValue.ToArray());
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

            int inParamCount = 1000;
            if (values != null)
            {
                inParamCount -= values.Count();
            }

            int skipCount = 0;

            //把in转成or
            //from Item where Code in ( ? 
            sql = sql.TrimEnd(); //from Item Id in ( ?
            sql = sql.Remove(sql.Length - 1).TrimEnd();//删除"?"//from Item where Code in (
            sql = sql.Remove(sql.Length - 1).TrimEnd();//删除"("//from Item where Code in
            sql = sql.Remove(sql.Length - 2).TrimEnd();//删除"in"//from Item where Code
            string inField = sql.Split(' ').Last();//Code
            sql = sql.Remove(sql.Length - inField.Length);//from Item where 

            sql = string.Format("{0} ({1}=? ", sql, inField);
            //var sqlStr = new StringBuilder(sql);
            //sqlStr.Append("(");//from Item where (
            //sqlStr.Append(inField);//from Item where (Code
            //sqlStr.Append("=? ");//from Item where (Code=? 

            while (true)
            {
                List<object> paramValue = new List<object>();
                var sqlStr = new StringBuilder(sql);
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
                sqlStr.Append(" )");

                if (values != null)
                {
                    paramValue.AddRange(values);
                }
                paramValue.AddRange(batchinParam);
                var list = dao.FindAllWithNativeSql<T>(sqlStr.ToString(), paramValue.ToArray());
                if (list != null)
                {
                    tList.AddRange(list);
                }
            }
            return tList;
        }


        public int ExecuteUpdateWithCustomQuery(string queryString)
        {
            return dao.ExecuteUpdateWithCustomQuery(queryString);
        }

        public int ExecuteUpdateWithCustomQuery(string queryString, object value)
        {
            return dao.ExecuteUpdateWithCustomQuery(queryString, value);
        }

        public int ExecuteUpdateWithCustomQuery(string queryString, object[] values)
        {
            return dao.ExecuteUpdateWithCustomQuery(queryString, values);
        }

        public int ExecuteUpdateWithNativeQuery(string queryString)
        {
            return dao.ExecuteUpdateWithNativeQuery(queryString);
        }

        public int ExecuteUpdateWithNativeQuery(string queryString, object value)
        {
            return dao.ExecuteUpdateWithNativeQuery(queryString, value);
        }

        public int ExecuteUpdateWithNativeQuery(string queryString, object value, IType type)
        {
            return dao.ExecuteUpdateWithNativeQuery(queryString, value, type);
        }

        public int ExecuteUpdateWithNativeQuery(string queryString, object[] values)
        {
            return dao.ExecuteUpdateWithNativeQuery(queryString, values);
        }

        public int ExecuteUpdateWithNativeQuery(string queryString, object[] values, IType[] types)
        {
            return dao.ExecuteUpdateWithNativeQuery(queryString, values, types);
        }

        public IList<T> FindAllWithCustomQuery<T>(string queryString)
        {
            return dao.FindAllWithCustomQuery<T>(queryString);
        }

        public IList<T> FindAllWithCustomQuery<T>(string queryString, object value)
        {
            return dao.FindAllWithCustomQuery<T>(queryString, value);
        }

        public IList<T> FindAllWithCustomQuery<T>(string queryString, object[] values)
        {
            return dao.FindAllWithCustomQuery<T>(queryString, values);
        }
    }
}
