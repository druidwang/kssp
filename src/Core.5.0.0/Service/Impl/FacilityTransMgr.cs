using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Facility.Persistence;
using com.Sconit.Facility.Entity;
using com.Sconit.Entity;
using com.Sconit.Service.Ext.Criteria;
using NHibernate.Expression;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service.Impl
{
    [Transactional]
    public class FacilityTransMgr : FacilityTransBaseMgr, IFacilityTransMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }

        #region Customized Methods
        [Transaction(TransactionMode.Requires)]
        public override void UpdateFacilityTrans(FacilityTrans facilityTrans)
        {
            if (facilityTrans.TransType == FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_MAINTAIN_START || facilityTrans.TransType == FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_MAINTAIN_FINISH)
            {
                if (!string.IsNullOrEmpty(facilityTrans.BatchNo))
                {
                    DetachedCriteria criteria = DetachedCriteria.For<FacilityTrans>();
                    criteria.Add(Expression.Eq("BatchNo", facilityTrans.BatchNo));
                    criteria.Add(Expression.Not(Expression.Eq("Id", facilityTrans.Id)));

                    IList<FacilityTrans> facilityTransList = criteriaMgrE.FindAll<FacilityTrans>(criteria);
                    if (facilityTransList != null && facilityTransList.Count > 0)
                    {
                        foreach (FacilityTrans trans in facilityTransList)
                        {

                            trans.StartDate = facilityTrans.StartDate;
                            trans.EndDate = facilityTrans.EndDate;
                            trans.Remark = facilityTrans.Remark;
                            base.UpdateFacilityTrans(trans);

                        }
                    }
                }
            }
            base.UpdateFacilityTrans(facilityTrans);


        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.Facility.Service.Ext.Impl
{
    [Transactional]
    public partial class FacilityTransMgrE : com.Sconit.Facility.Service.Impl.FacilityTransMgr, IFacilityTransMgrE
    {
    }
}

#endregion Extend Class