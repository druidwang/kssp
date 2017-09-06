// -----------------------------------------------------------------------
// <copyright file="ICurrencyMgr.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace com.Sconit.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using com.Sconit.Entity.MD;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ICurrencyMgr
    {
        void UpdateCurrency(Currency currency);
        void CreateCurrency(Currency currency);
    }
}
