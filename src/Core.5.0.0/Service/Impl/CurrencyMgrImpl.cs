// -----------------------------------------------------------------------
// <copyright file="CurrencyMgrImpl.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace com.Sconit.Service.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Castle.Services.Transaction;
    using com.Sconit.Entity.MD;
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Transactional]
    public class CurrencyMgrImpl : BaseMgr, ICurrencyMgr
    {
        public IGenericMgr genericMgr { get; set; }

        [Transaction(TransactionMode.Requires)]
        public void UpdateCurrency(Currency currency)
        {
            if (currency.IsBase)
            {
                UnBaseCurrency();
            }
            this.genericMgr.Update(currency);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateCurrency(Currency currency)
        {
            if (currency.IsBase)
            {
                UnBaseCurrency();
            }
            this.genericMgr.Create(currency);
        }

        private void UnBaseCurrency()
        {
            IList<Currency> currencyList = genericMgr.FindAll<Currency>("from Currency as c where c.IsBase = ?", true);
            for (int i = 0; i < currencyList.Count; i++)
            {
                currencyList[i].IsBase = false;
                this.genericMgr.Update(currencyList[i]);
            }
        }
    }
}
