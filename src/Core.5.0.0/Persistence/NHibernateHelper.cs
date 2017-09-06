using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Persister.Entity;
using System.Reflection;

namespace com.Sconit.Persistence
{
    public class NHibernateHelper
    {
        public static void GetTableProperty(ISession session, object entity, out string tableName, out Dictionary<string, string> propertyAndColumnNames)
        {
            ISessionFactory sessionFactory = session.SessionFactory;
            Type type = entity.GetType();
            var metaData = sessionFactory.GetClassMetadata(type.ToString());
            var persister = (AbstractEntityPersister)metaData;
            tableName = persister.TableName;
            propertyAndColumnNames = new Dictionary<string, string>();
            string[] databaseIdentifiers = persister.KeyColumnNames;
            var fieldInfo = typeof(AbstractEntityPersister)
                .GetField("subclassPropertyColumnNames", BindingFlags.NonPublic | BindingFlags.Instance);
            var pairs = (Dictionary<string, string[]>)fieldInfo.GetValue(persister);
            //主键
            foreach (var pair in pairs.Skip(pairs.Count() - databaseIdentifiers.Count()))
            {
                if (pair.Value.Length > 0)
                {
                    propertyAndColumnNames.Add(pair.Key, pair.Value[0]);
                }
            }
            //其他
            foreach (var pair in pairs.Take(pairs.Count() - databaseIdentifiers.Count() - 1))
            {
                if (pair.Value.Length > 0)
                {
                    propertyAndColumnNames.Add(pair.Key, pair.Value[0]);
                }
            }
        }
    }
}
