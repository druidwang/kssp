using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Facility.Persistence;
using com.Sconit.Facility.Entity;
using com.Sconit.Entity;
using com.Sconit.Utility;
using com.Sconit.Entity.Exception;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using com.Sconit.Entity.MasterData;
using System.Linq;
using com.Sconit.Service.Ext.Criteria;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service.Impl
{
    [Transactional]
    public class FacilityMaintainPlanMgr : FacilityMaintainPlanBaseMgr, IFacilityMaintainPlanMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }

        #region Customized Methods

        public override void CreateFacilityMaintainPlan(FacilityMaintainPlan facilityMaintainPlan)
        {
            string type = facilityMaintainPlan.MaintainPlan.Type;
            int interval = facilityMaintainPlan.MaintainPlan.TypePeriod.HasValue ? facilityMaintainPlan.MaintainPlan.TypePeriod.Value : 0;
            int leadTime = facilityMaintainPlan.MaintainPlan.LeadTime.HasValue ? facilityMaintainPlan.MaintainPlan.LeadTime.Value : 0;

            if (type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_FREQUENCY)
            {
                facilityMaintainPlan.NextMaintainQty = facilityMaintainPlan.StartQty + interval;
                facilityMaintainPlan.NextWarnQty = facilityMaintainPlan.NextMaintainQty - leadTime;
            }
            else
            {
                #region 按周期的计算下次保养时间和下次提醒时间

                DateTime startDate = facilityMaintainPlan.StartDate.Value;
                if (type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_MINUTE)
                {
                    facilityMaintainPlan.NextMaintainDate = startDate.AddMinutes(interval);
                }
                else if (type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_HOUR)
                {
                    facilityMaintainPlan.NextMaintainDate = startDate.AddHours(interval);
                }
                else if (type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_DAY)
                {
                    facilityMaintainPlan.NextMaintainDate = startDate.AddDays(interval);
                }
                else if (type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_WEEK)
                {
                    facilityMaintainPlan.NextMaintainDate = startDate.AddDays(7 * interval);
                }
                else if (type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_MONTH)
                {
                    facilityMaintainPlan.NextMaintainDate = startDate.AddMonths(interval);
                }
                else if (type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_YEAR)
                {
                    facilityMaintainPlan.NextMaintainDate = startDate.AddYears(interval);
                }
                else
                {
                    facilityMaintainPlan.NextMaintainDate = startDate;
                }
                facilityMaintainPlan.NextWarnDate = facilityMaintainPlan.NextMaintainDate.Value.AddDays(-leadTime);
                #endregion
            }
            base.CreateFacilityMaintainPlan(facilityMaintainPlan);
        }

        public override void UpdateFacilityMaintainPlan(FacilityMaintainPlan facilityMaintainPlan)
        {
            UpdateFacilityMaintainPlan(facilityMaintainPlan, true);
        }

        public void UpdateFacilityMaintainPlan(FacilityMaintainPlan facilityMaintainPlan, bool isCalc)
        {
            FacilityMaintainPlan oldFacilityMaintainPlan = this.LoadFacilityMaintainPlan(facilityMaintainPlan.Id);
            if (isCalc)
            {

                string type = oldFacilityMaintainPlan.MaintainPlan.Type;
                int interval = oldFacilityMaintainPlan.MaintainPlan.TypePeriod.HasValue ? oldFacilityMaintainPlan.MaintainPlan.TypePeriod.Value : 0;
                int leadTime = oldFacilityMaintainPlan.MaintainPlan.LeadTime.HasValue ? oldFacilityMaintainPlan.MaintainPlan.LeadTime.Value : 0;

                if (type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_FREQUENCY)
                {
                    oldFacilityMaintainPlan.StartQty = facilityMaintainPlan.StartQty;
                    oldFacilityMaintainPlan.NextMaintainQty = oldFacilityMaintainPlan.StartQty + interval;
                    oldFacilityMaintainPlan.NextWarnQty = oldFacilityMaintainPlan.NextMaintainQty - leadTime;
                }
                else
                {
                    #region 计算下次保养时间和下次提醒时间
                    oldFacilityMaintainPlan.StartDate = facilityMaintainPlan.StartDate;
                    DateTime startDate = oldFacilityMaintainPlan.StartDate.Value;
                    if (type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_MINUTE)
                    {
                        oldFacilityMaintainPlan.NextMaintainDate = startDate.AddMinutes(interval);
                    }
                    else if (type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_HOUR)
                    {
                        oldFacilityMaintainPlan.NextMaintainDate = startDate.AddHours(interval);
                    }
                    else if (type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_DAY)
                    {
                        oldFacilityMaintainPlan.NextMaintainDate = startDate.AddDays(interval);
                    }
                    else if (type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_WEEK)
                    {
                        oldFacilityMaintainPlan.NextMaintainDate = startDate.AddDays(7 * interval);
                    }
                    else if (type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_MONTH)
                    {
                        oldFacilityMaintainPlan.NextMaintainDate = startDate.AddMonths(interval);
                    }
                    else if (type == FacilityConstants.CODE_MASTER_FACILITY_MAINTAIN_TYPE_YEAR)
                    {
                        oldFacilityMaintainPlan.NextMaintainDate = startDate.AddYears(interval);
                    }
                    else
                    {
                        oldFacilityMaintainPlan.NextMaintainDate = startDate;
                    }
                    oldFacilityMaintainPlan.NextWarnDate = oldFacilityMaintainPlan.NextMaintainDate.Value.AddDays(-leadTime);
                    #endregion
                }
            }
            base.UpdateFacilityMaintainPlan(oldFacilityMaintainPlan);
        }


        public void CreateFacilityMaintainPlanList(IList<FacilityMaintainPlan> facilityMaintainPlanList)
        {
            if (facilityMaintainPlanList != null && facilityMaintainPlanList.Count > 0)
            {
                foreach (FacilityMaintainPlan f in facilityMaintainPlanList)
                {
                    this.CreateFacilityMaintainPlan(f);
                }
            }
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.Facility.Service.Ext.Impl
{
    [Transactional]
    public partial class FacilityMaintainPlanMgrE : com.Sconit.Facility.Service.Impl.FacilityMaintainPlanMgr, IFacilityMaintainPlanMgrE
    {
    }
}

#endregion Extend Class