using System;
using System.Collections.Generic;
using System.Collections;

namespace com.Sconit.Persistence
{
    /// <summary>
    /// Summary description for IDaoBase.
    /// </summary>

    public interface IDao
    {
        object Create(object instance);

        void Update(object instance);

        void Delete(object instance);

        void DeleteAll(Type type);

        void Save(object instance);

        bool BatchInsert<T>(IList<T> instanceList);
    }
}
