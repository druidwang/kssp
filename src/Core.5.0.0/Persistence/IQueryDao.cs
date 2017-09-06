// -----------------------------------------------------------------------
// <copyright file="IQuery.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace com.Sconit.Persistence
{
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IQueryDao
    {
        IList<T> FindAll<T>();

        IList<T> FindAll<T>(int firstRow, int maxRows);

        T FindById<T>(object id);
    }
}
