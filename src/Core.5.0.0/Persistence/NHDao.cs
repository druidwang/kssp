using System;
using System.Collections;
using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration.Util;
using NHibernate;
using NHibernate.Collection;
using NHibernate.Exceptions;
using NHibernate.Proxy;
using NHibernate.Type;

namespace com.Sconit.Persistence
{
    public class NHDao : NHQueryDao, INHDao
    {
        public virtual object Create(object instance)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.Save(instance);
                    //session.Flush();
                    return instance;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Create for " + instance.GetType().Name, ex);
                }
            }
        }

        public virtual bool BatchInsert<T>(IList<T> instanceList)
        {
            if (instanceList != null && instanceList.Count > 0)
            {
                using (IStatelessSession session = GetStatelessSession())
                {
                    try
                    {
                        foreach (var instance in instanceList)
                        {
                            session.Insert(instance);
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        throw new DataException("Could not perform Create for " + instanceList[0].GetType().Name, ex);
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public virtual void Delete(object instance)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.Delete(instance);
                    //session.Flush();
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Delete for " + instance.GetType().Name, ex);
                }
            }
        }

        public virtual void Update(object instance)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.Update(instance);
                    //SaveOrUpdateCopy可以解决在hibernate中同一个session里面有了两个相同标识的错误
                    //a different object with the same identifier value was already associated with the session
                    //不知道有没有什么未知影响
                    //session.SaveOrUpdateCopy(instance);
                    //session.Flush();
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Update for " + instance.GetType().Name, ex);
                }
            }
        }

        public virtual void MergeUpdate(object instance)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.Merge(instance);
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Update for " + instance.GetType().Name, ex);
                }
            }
        }

        public virtual void DeleteAll(Type type)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.Delete(String.Format("from {0}", type.Name));
                    //session.Flush();
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform DeleteAll for " + type.Name, ex);
                }
            }
        }

        public virtual void Save(object instance)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.SaveOrUpdate(instance);
                    //session.Flush();
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Save for " + instance.GetType().Name, ex);
                }
            }
        }

        public virtual void Delete(string hqlString)
        {
            Delete(hqlString, (object[])null, (IType[])null);
        }

        public virtual void Delete(string hqlString, object value, IType type)
        {
            Delete(hqlString, new object[] { value }, new IType[] { type });
        }


        //public virtual void Delete(string hqlString, object value)
        //{
        //    Delete(hqlString, new object[] { value }, (IType[])null);
        //}

        //public virtual void Delete(string hqlString, object[] values)
        //{
        //    Delete(hqlString, values, (IType[])null);
        //}

        public virtual void Delete(string hqlString, object[] values, IType[] types)
        {
            if (hqlString == null || hqlString.Length == 0) throw new ArgumentNullException("hqlString");
            if (values != null && types != null && types.Length != values.Length) throw new ArgumentException("Length of values array must match length of types array");

            using (ISession session = GetSession())
            {
                try
                {
                    if (values == null)
                    {
                        session.Delete(hqlString);
                    }
                    //else if (types == null)
                    //{
                    //    session.Delete(hqlString, values);
                    //}
                    else
                    {
                        session.Delete(hqlString, values, types);
                    }
                    //session.Flush();
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Delete for " + hqlString, ex);
                }
            }
        }

        public virtual int ExecuteUpdateWithCustomQuery(string queryString)
        {
            return ExecuteUpdateWithCustomQuery(queryString, (object[])null, (IType[])null);
        }

        public virtual int ExecuteUpdateWithCustomQuery(string queryString, object value)
        {
            return ExecuteUpdateWithCustomQuery(queryString, new object[] { value }, (IType[])null);
        }

        public virtual int ExecuteUpdateWithCustomQuery(string queryString, object value, IType type)
        {
            return ExecuteUpdateWithCustomQuery(queryString, new object[] { value }, new IType[] { type });
        }

        public virtual int ExecuteUpdateWithCustomQuery(string queryString, object[] values)
        {
            return ExecuteUpdateWithCustomQuery(queryString, values, (IType[])null);
        }

        public virtual int ExecuteUpdateWithCustomQuery(string queryString, object[] values, IType[] types)
        {
            if (queryString == null || queryString.Length == 0) throw new ArgumentNullException("queryString");
            if (values != null && types != null && types.Length != values.Length) throw new ArgumentException("Length of values array must match length of types array");

            using (ISession session = GetSession())
            {
                try
                {
                    IQuery query = session.CreateQuery(queryString);
                    if (values != null)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (types != null && types[i] != null)
                            {
                                query.SetParameter(i, values[i], types[i]);
                            }
                            else
                            {
                                query.SetParameter(i, values[i]);
                            }
                        }
                    }

                    int resultCount = query.ExecuteUpdate();
                    return resultCount;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Find for custom query : " + queryString, ex);
                }
            }
        }

        public virtual int ExecuteUpdateWithNativeQuery(string queryString)
        {
            return ExecuteUpdateWithNativeQuery(queryString, (object[])null, (IType[])null);
        }

        public virtual int ExecuteUpdateWithNativeQuery(string queryString, object value)
        {
            return ExecuteUpdateWithNativeQuery(queryString, new object[] { value }, (IType[])null);
        }

        public virtual int ExecuteUpdateWithNativeQuery(string queryString, object value, IType type)
        {
            return ExecuteUpdateWithNativeQuery(queryString, new object[] { value }, new IType[] { type });
        }

        public virtual int ExecuteUpdateWithNativeQuery(string queryString, object[] values)
        {
            return ExecuteUpdateWithNativeQuery(queryString, values, (IType[])null);
        }

        public virtual int ExecuteUpdateWithNativeQuery(string queryString, object[] values, IType[] types)
        {
            if (queryString == null || queryString.Length == 0) throw new ArgumentNullException("queryString");
            if (values != null && types != null && types.Length != values.Length) throw new ArgumentException("Length of values array must match length of types array");

            using (ISession session = GetSession())
            {
                try
                {
                    IQuery query = session.CreateSQLQuery(queryString);
                    if (values != null)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (types != null && types[i] != null)
                            {
                                query.SetParameter(i, values[i], types[i]);
                            }
                            else
                            {
                                query.SetParameter(i, values[i]);
                            }
                        }
                    }

                    int resultCount = query.ExecuteUpdate();
                    return resultCount;
                }
                catch (Exception ex)
                {
                    throw new DataException("Could not perform Find for custom query : " + queryString, ex);
                }
            }
        }

        public void InitializeLazyProperties(object instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            using (ISession session = GetSession())
            {
                foreach (object val in ReflectionUtility.GetPropertiesDictionary(instance).Values)
                {
                    if (val is INHibernateProxy || val is IPersistentCollection)
                    {
                        if (!NHibernateUtil.IsInitialized(val))
                        {
                            session.Lock(instance, LockMode.None);
                            NHibernateUtil.Initialize(val);
                        }
                    }
                }
            }
        }

        public void InitializeLazyProperty(object instance, string propertyName)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName == null || propertyName.Length == 0) throw new ArgumentNullException("collectionPropertyName");

            IDictionary<string, object> properties = ReflectionUtility.GetPropertiesDictionary(instance);
            if (!properties.ContainsKey(propertyName))
                throw new ArgumentOutOfRangeException("collectionPropertyName", "Property "
                    + propertyName + " doest not exist for type "
                    + instance.GetType().ToString() + ".");

            using (ISession session = GetSession())
            {
                object val = properties[propertyName];

                if (val is INHibernateProxy || val is IPersistentCollection)
                {
                    if (!NHibernateUtil.IsInitialized(val))
                    {
                        session.Lock(instance, LockMode.None);
                        NHibernateUtil.Initialize(val);
                    }
                }
            }
        }

        public void FlushSession()
        {
            using (ISession session = GetSession())
            {
                session.Flush();
            }
        }

        public void CleanSession()
        {
            using (ISession session = GetSession())
            {
                session.Clear();
            }
        }

        public void GetTableProperty(object entity, out string tableName, out Dictionary<string, string> propertyAndColumnNames)
        {
            using (ISession session = GetSession())
            {
                NHibernateHelper.GetTableProperty(session, entity, out tableName, out propertyAndColumnNames);
            }
        }
    }
}
