using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Facility.Persistence;
using com.Sconit.Facility.Entity;
using NHibernate.Expression;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Entity;
using com.Sconit.Facility.Service.Ext;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service.Impl
{
    [Transactional]
    public class FacilityStockMasterMgr : FacilityStockMasterBaseMgr, IFacilityStockMasterMgr
    {

        public ICriteriaMgrE criteriaMgrE { get; set; }
        public IFacilityStockDetailMgrE facilityStockDetailMgr { get; set; }

        #region Customized Methods
        [Transaction(TransactionMode.Requires)]
        public override void CreateFacilityStockMaster(FacilityStockMaster facilityStockMaster)
        {

            base.CreateFacilityStockMaster(facilityStockMaster);

            #region 根据条件把明细生成好
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityMaster));
            if (!string.IsNullOrEmpty(facilityStockMaster.FacilityCategory))
            {
                criteria.Add(Expression.In("Category", facilityStockMaster.FacilityCategory.Split(',')));
            }
            if (!string.IsNullOrEmpty(facilityStockMaster.ChargeSite))
            {
                criteria.Add(Expression.In("ChargeSite", facilityStockMaster.ChargeSite.Split(',')));
            }
            if (!string.IsNullOrEmpty(facilityStockMaster.ChargeOrg))
            {
                criteria.Add(Expression.In("ChargeOrganization", facilityStockMaster.ChargeOrg.Split(',')));
            }
            if (!string.IsNullOrEmpty(facilityStockMaster.ChargePerson))
            {
                criteria.Add(Expression.In("CurrChargePerson", facilityStockMaster.ChargePerson.Split(',')));
            }
            if (!string.IsNullOrEmpty(facilityStockMaster.AssetNo))
            {
                criteria.Add(Expression.Like("AssetNo", facilityStockMaster.AssetNo, MatchMode.Anywhere));
            }

            //转让，报废，盘亏状态的不显示在明细中
            criteria.Add(Expression.Not(Expression.In("Status", new string[] { FacilityConstants.CODE_MASTER_FACILITY_STATUS_SCRAP, FacilityConstants.CODE_MASTER_FACILITY_STATUS_LOSE, FacilityConstants.CODE_MASTER_FACILITY_STATUS_SELL })));
            IList<FacilityMaster> facilityMasterList = criteriaMgrE.FindAll<FacilityMaster>(criteria);
            if (facilityMasterList != null && facilityMasterList.Count > 0)
            {
                foreach (FacilityMaster f in facilityMasterList)
                {
                    FacilityStockDetail d = new FacilityStockDetail();
                    d.StNo = facilityStockMaster.StNo;
                    d.FacilityMaster = f;
                    d.InvQty = 1;
                    d.Qty = 0;
                    d.DiffQty = d.InvQty - d.Qty;
                    d.CreateDate = facilityStockMaster.CreateDate;
                    d.CreateUser = facilityStockMaster.CreateUser;
                    d.LastModifyDate = facilityStockMaster.CreateDate;
                    d.LastModifyUser = facilityStockMaster.CreateUser;

                    facilityStockDetailMgr.CreateFacilityStockDetail(d);
                }

            }

            #endregion

        }

        public void ConfirmStockTakeDetail(IList<int> stockTakeDetailList)
        {
            foreach (int id in stockTakeDetailList)
            {
                FacilityStockDetail d = this.FindById<FacilityStockDetail>(id);
                d.Qty = d.InvQty;
                d.DiffQty = 0;
                //d.LastModifyDate = DateTime.Now;
                //d.LastModifyUser = 
                this.Update(d);
            }
        }

        public IList<FacilityStockDetail> GetUnConfirmedStockDetailList(string stNo)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityStockDetail));
            criteria.Add(Expression.Eq("StNo",stNo));
            criteria.Add(Expression.Gt("DiffQty", Decimal.Zero));
            return criteriaMgrE.FindAll<FacilityStockDetail>(criteria);
        }


        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.Facility.Service.Ext.Impl
{
    [Transactional]
    public partial class FacilityStockMasterMgrE : com.Sconit.Facility.Service.Impl.FacilityStockMasterMgr, IFacilityStockMasterMgrE
    {
    }
}

#endregion Extend Class