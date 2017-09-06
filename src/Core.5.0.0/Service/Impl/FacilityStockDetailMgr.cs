using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Facility.Persistence;
using com.Sconit.Facility.Entity;
using NHibernate.Expression;
using com.Sconit.Service.Ext.Criteria;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service.Impl
{
    [Transactional]
    public class FacilityStockDetailMgr : FacilityStockDetailBaseMgr, IFacilityStockDetailMgr
    {
        #region Customized Methods

        public ICriteriaMgrE criteriaMgrE { get; set; }

        [Transaction(TransactionMode.Unspecified)]
        public IList<FacilityStockDetail> LoadFacilityStockDetails(string stNo)
        {
            DetachedCriteria criteria = DetachedCriteria.For<FacilityStockDetail>();
            criteria.Add(Expression.Eq("StNo", stNo));

            IList<FacilityStockDetail> facilityStockDetails = criteriaMgrE.FindAll<FacilityStockDetail>(criteria);

            return facilityStockDetails;
        }
        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.Facility.Service.Ext.Impl
{
    [Transactional]
    public partial class FacilityStockDetailMgrE : com.Sconit.Facility.Service.Impl.FacilityStockDetailMgr, IFacilityStockDetailMgrE
    {
    }
}

#endregion Extend Class