using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Facility.Persistence;
using NHibernate.Expression;
using com.Sconit.Facility.Entity;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service.Impl
{
    [Transactional]
    public class FacilityItemMgr : FacilityItemBaseMgr, IFacilityItemMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }

        #region Customized Methods


        public IList<FacilityItem> GetFacilityItemList(string fcId, string itemCode,string allocateType)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityItem));
            criteria.Add(Expression.Eq("FacilityMaster.FCID", fcId));
            criteria.Add(Expression.Eq("Item.Code", itemCode));
            criteria.Add(Expression.Eq("AllocateType", allocateType));
            return criteriaMgrE.FindAll<FacilityItem>(criteria);
        }
        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.Facility.Service.Ext.Impl
{
    [Transactional]
    public partial class FacilityItemMgrE : com.Sconit.Facility.Service.Impl.FacilityItemMgr, IFacilityItemMgrE
    {
    }
}

#endregion Extend Class