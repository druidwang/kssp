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
    public class MaintainPlanMgr : MaintainPlanBaseMgr, IMaintainPlanMgr
    {
        #region Customized Methods
        public ICriteriaMgrE criteriaMgrE { get; set; }

        public IList<MaintainPlan> GetMaintainPlanList(string facilityCategory)
        {
            DetachedCriteria criteria = DetachedCriteria.For<MaintainPlan>();
            criteria.Add(Expression.Or(Expression.Eq("FacilityCategory", facilityCategory), Expression.Eq("FacilityCategory", string.Empty)));
            return criteriaMgrE.FindAll<MaintainPlan>(criteria);
        }

        [Transaction(TransactionMode.Requires)]
        public override void CreateMaintainPlan(MaintainPlan entity)
        {
            #region 把当前的FacilityMaintainPlan都删掉
            DetachedCriteria criteria = DetachedCriteria.For<FacilityMaintainPlan>();
            criteria.CreateAlias("MaintainPlan", "p");
            criteria.Add(Expression.Eq("p.Code", entity.Code));

            IList<FacilityMaintainPlan> facilityMaintainPlanList = criteriaMgrE.FindAll<FacilityMaintainPlan>(criteria);
            if (facilityMaintainPlanList != null && facilityMaintainPlanList.Count > 0)
            {
                foreach (FacilityMaintainPlan f in facilityMaintainPlanList)
                {
                    Delete(f);
                }
            }
            #endregion
            base.CreateMaintainPlan(entity);
        }
        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.Facility.Service.Ext.Impl
{
    [Transactional]
    public partial class MaintainPlanMgrE : com.Sconit.Facility.Service.Impl.MaintainPlanMgr, IMaintainPlanMgrE
    {
    }
}

#endregion Extend Class