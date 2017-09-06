using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity.BIL;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.INP;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.PRD;
using com.Sconit.Entity.SYS;
using com.Sconit.Entity.VIEW;
using com.Sconit.Persistence;
using System.Data.SqlClient;
using System.Data;
using com.Sconit.Entity.SCM;
using NHibernate.Type;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class LocationDetailMgrImpl : BaseMgr, ILocationDetailMgr
    {
        #region 变量
        public IGenericMgr genericMgr { get; set; }
        public IBillMgr billMgr { get; set; }
        public ISystemMgr systemMgr { get; set; }
        public IFinanceCalendarMgr financeCalendarMgr { get; set; }
        public IHuMgr huMgr { get; set; }
        public IItemMgr itemMgr { get; set; }
        public ISqlDao sqlDao { get; set; }
        public INumberControlMgr numberControlMgr { get; set; }


        private static string SelectHuStatement = "from LocationLotDetail where HuId = ?";
        //        private static string SelectMinusInventoryStatement = @"from LocationLotDetail where Location = ? 
        //                                                                                                and Item = ?
        //                                                                                                and HuId is null
        //                                                                                                and Qty < 0
        //                                                                                                and QualityType = ? 
        //                                                                                                and OccupyType = ? 
        //                                                                                                and IsConsignment = ? 
        //                                                                                                and IsFreeze = False 
        //                                                                                                ";

        //        private static string SelectManufacturePartyConsignmentInventoryStatement = @"from LocationLotDetail where Location = ? 
        //                                                                                                and Item = ?
        //                                                                                                and HuId is null
        //                                                                                                and Qty < 0
        //                                                                                                and QualityType = ? 
        //                                                                                                and OccupyType = ? 
        //                                                                                                and IsConsignment = ? 
        //                                                                                                and IsFreeze = False 
        //                                                                                                and ManufactureParty = ?
        //                                                                                                ";

        //占用的一定是正数库存
        //        private static string SelectOccupyInventoryStatement = @"from LocationLotDetail where Location = ? 
        //                                                                                                and Item = ?   
        //                                                                                                and HuId is null
        //                                                                                                and Qty > 0
        //                                                                                                and QualityType = ? 
        //                                                                                                and OccupyType = ? 
        //                                                                                                and OccupyReferenceNo = ? 
        //                                                                                                and IsFreeze = False 
        //                                                                                                ";

        //        private static string SelectVoidOccupyInventoryStatement = @"from LocationLotDetail where Location = ? 
        //                                                                                                and Item = ?  
        //                                                                                                and PlanBill = ?      
        //                                                                                                and IsConsignment = True  
        //                                                                                                and HuId is null
        //                                                                                                and Qty > 0
        //                                                                                                and QualityType = ? 
        //                                                                                                and OccupyType = ? 
        //                                                                                                and OccupyReferenceNo = ? 
        //                                                                                                and IsFreeze = False 
        //                                                                                                ";

        //        private static string SelectPlusInventoryStatement = @"from LocationLotDetail where Location = ? 
        //                                                                                                and Item = ?
        //                                                                                                and HuId is null
        //                                                                                                and Qty > 0
        //                                                                                                and QualityType = ? 
        //                                                                                                and OccupyType = ? 
        //                                                                                                and IsConsignment = ? 
        //                                                                                                and IsFreeze = False 
        //                                                                                                ";

        //        //冲销出库的一定是正数库存
        //        private static string SelectVoidInventoryStatement = @"from LocationLotDetail where Location = ? 
        //                                                                                                and Item = ? 
        //                                                                                                and PlanBill = ?      
        //                                                                                                and IsConsignment = True
        //                                                                                                and Qty > 0
        //                                                                                                and HuId is null
        //                                                                                                and QualityType = ? 
        //                                                                                                and OccupyType = ? 
        //                                                                                                and IsFreeze = False 
        //                                                                                                ";
        #endregion

        #region public methods

        #region 查询条码库存
        public LocationLotDetail GetHuLocationLotDetail(string huId)
        {
            IList<LocationLotDetail> locationLotDetailList = this.genericMgr.FindAll<LocationLotDetail>(SelectHuStatement, new object[] { huId });
            if (locationLotDetailList != null && locationLotDetailList.Count > 0)
            {
                if (locationLotDetailList.Count > 1)
                {
                    throw new TechnicalException("HuId " + huId + " in more than one location.");
                }

                return locationLotDetailList[0];
            }
            return null;
        }

        public IList<LocationLotDetail> GetHuLocationLotDetails(IList<string> huIdList)
        {
            if (huIdList == null || huIdList.Count == 0)
            {
                return null;
            }

            string hql = string.Empty;
            IList<object> paras = new List<object>();

            foreach (string huId in huIdList)
            {
                if (hql == string.Empty)
                {
                    hql = "from LocationLotDetail where HuId in (?";
                }
                else
                {
                    hql += ",?";
                }
                paras.Add(huId);
            }
            hql += ")";
            IList<Hu> invHuLists = this.genericMgr.FindAllIn<Hu>("from Hu where HuId in (?", huIdList);
            IList<LocationLotDetail> huLocationLotDetails = this.genericMgr.FindAll<LocationLotDetail>(hql, paras.ToArray());
            foreach (var huLocationLotDetail in huLocationLotDetails)
            {
                huLocationLotDetail.ItemVersion = invHuLists.Where(p => p.HuId == huLocationLotDetail.HuId).FirstOrDefault().ItemVersion;
            }
            return huLocationLotDetails;
        }
        #endregion

        #region 查询历史库存 有问题待解决
        public IList<HistoryInventory> GetHistoryLocationDetails(string locatoin, IList<string> itemList, DateTime historyDate, string SortDesc, int PageSize, int Page)
        {
            SqlParameter[] parm = new SqlParameter[6];

            parm[0] = new SqlParameter("@Location", SqlDbType.VarChar, 50);
            parm[0].Value = locatoin;

            string itemParm = null;
            if (itemList != null && itemList.Count > 0)
            {
                foreach (string itemCode in itemList)
                {
                    if (itemParm == null)
                    {
                        itemParm = itemCode;
                    }
                    else
                    {
                        itemParm += "," + itemCode;
                    }
                }

            }
            parm[1] = new SqlParameter("@Item", SqlDbType.VarChar, 4000);
            parm[1].Value = itemParm;

            parm[2] = new SqlParameter("@HistoryDate", SqlDbType.DateTime);
            parm[2].Value = historyDate;

            DataSet set = this.sqlDao.GetDatasetByStoredProcedure("USP_Busi_GetHistoryInv", parm);

            //if (set.Tables.Count > 2)
            //{
            IList<HistoryInventory> historyInventoryList = new List<HistoryInventory>();
            foreach (DataRow dr in set.Tables[0].Rows)
            {
                HistoryInventory historyInventory = new HistoryInventory();
                historyInventory.Location = (string)dr["Location"];
                historyInventory.Item = (string)dr["Item"];
                historyInventory.ManufactureParty = dr["ManufactureParty"] != DBNull.Value ? (string)dr["ManufactureParty"] : "";
                historyInventory.Qty = Convert.ToDecimal(dr["Qty"]);
                historyInventory.CsQty = Convert.ToDecimal(dr["CsQty"]);
                historyInventory.RejectQty = Convert.ToDecimal(dr["RejectQty"]);
                historyInventory.QualifyQty = Convert.ToDecimal(dr["QualifyQty"]);
                historyInventory.RejectQty = Convert.ToDecimal(dr["RejectQty"]);
                historyInventory.ATPQty = Convert.ToDecimal(dr["ATPQty"]);
                historyInventory.FreezeQty = Convert.ToDecimal(dr["FreezeQty"]);
                historyInventoryList.Add(historyInventory);
            }

            return historyInventoryList;
            //}
            //return null;
        }
        #endregion

        #region 查询条码历史库存 有问题待解决
        public IList<HuHistoryInventory> GetHuHistoryLocationDetails(string locatoin, IList<string> itemList, DateTime historyDate)
        {
            SqlParameter[] parm = new SqlParameter[3];

            parm[0] = new SqlParameter("@Location", SqlDbType.VarChar, 50);
            parm[0].Value = locatoin;

            string itemParm = null;
            if (itemList != null && itemList.Count > 0)
            {
                foreach (string itemCode in itemList)
                {
                    if (itemParm == null)
                    {
                        itemParm = itemCode;
                    }
                    else
                    {
                        itemParm += "," + itemCode;
                    }
                }

            }
            parm[1] = new SqlParameter("@Item", SqlDbType.VarChar, 4000);
            parm[1].Value = itemParm;

            parm[2] = new SqlParameter("@HistoryDate", SqlDbType.DateTime);
            parm[2].Value = historyDate;

            SqlDataReader reader = this.sqlDao.GetDataReaderByStoredProcedure("USP_Busi_GetHuHistoryInv", parm);

            if (reader != null)
            {
                IList<HuHistoryInventory> huHistoryInventoryList = new List<HuHistoryInventory>();
                while (reader.Read())
                {
                    HuHistoryInventory huHistoryInventory = new HuHistoryInventory();
                    huHistoryInventory.Location = reader["Location"].ToString();
                    huHistoryInventory.Item = reader["Item"].ToString();
                    huHistoryInventory.HuId = reader.GetValue(4).ToString();
                    huHistoryInventory.LotNo = reader.GetValue(5).ToString();
                    huHistoryInventory.Qty = (decimal)reader["Qty"];
                    huHistoryInventory.QualityType = (com.Sconit.CodeMaster.QualityType)(int.Parse(reader["QualityType"].ToString()));

                    huHistoryInventoryList.Add(huHistoryInventory);
                }

                return huHistoryInventoryList;
            }
            return null;
        }
        #endregion

        #region 发货出库
        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> InventoryOut(IpDetail ipDetail)
        {
            return InventoryOut(ipDetail, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> InventoryOut(IpDetail ipDetail, DateTime effectiveDate)
        {
            List<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();
            com.Sconit.CodeMaster.TransactionType? transType = GetTransactionType(ipDetail);
            if (transType.HasValue)
            {
                foreach (IpDetailInput ipDetailInput in ipDetail.IpDetailInputs)
                {
                    InventoryIO inventoryIO = new InventoryIO();

                    inventoryIO.Location = ipDetail.LocationFrom;
                    inventoryIO.Item = ipDetail.Item;
                    inventoryIO.HuId = ipDetailInput.HuId;
                    inventoryIO.Qty = -ipDetailInput.ShipQty * ipDetail.UnitQty;  //转换为库存单位，发货为负数
                    inventoryIO.LotNo = ipDetailInput.LotNo;
                    inventoryIO.QualityType = ipDetail.QualityType;     //不合格品的ATP状态一定是false，合格品的状态一定是true，质检不采用ASN发货这里不可能出现
                    inventoryIO.IsATP = ipDetail.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                    inventoryIO.IsFreeze = false;                       //可能指定移库冻结的零件？
                    inventoryIO.IsCreatePlanBill = ipDetailInput.IsCreatePlanBill;
                    inventoryIO.IsConsignment = ipDetailInput.IsConsignment;
                    inventoryIO.PlanBill = ipDetailInput.PlanBill;
                    inventoryIO.ActingBill = ipDetailInput.ActingBill;
                    inventoryIO.TransactionType = transType.Value;
                    //if (ipDetail.CurrentOccupyType == com.Sconit.CodeMaster.OccupyType.Pick)
                    //{
                    //    //发货不能发捡货单占用的零件
                    //    throw new TechnicalException("Can't ship material occupied by picklist.");
                    //}
                    //else if (ipDetail.CurrentOccupyType == com.Sconit.CodeMaster.OccupyType.Inspect
                    //    && (transType.Value != com.Sconit.CodeMaster.TransactionType.ISS_TR
                    //    || transType.Value != com.Sconit.CodeMaster.TransactionType.ISS_TR_VOID))
                    //{
                    //    //检验、不合格品占用的零件只能做移库及移库冲销
                    //    throw new TechnicalException("Can't ship material occupied by inspect order.");
                    //}
                    inventoryIO.OccupyType = ipDetailInput.OccupyType;
                    inventoryIO.OccupyReferenceNo = ipDetailInput.OccupyReferenceNo;
                    inventoryIO.IsVoid = ipDetail.IsVoid;
                    inventoryIO.EffectiveDate = effectiveDate;
                    inventoryIO.ManufactureParty = ipDetailInput.ManufactureParty;

                    IList<InventoryTransaction> currentInventoryTransactionList = RecordInventory(inventoryIO);
                    #region 记录WMS发货单行号
                    foreach (InventoryTransaction currentInventoryTransaction in currentInventoryTransactionList)
                    {
                        currentInventoryTransaction.WMSIpSeq = ipDetailInput.WMSIpSeq;
                    }
                    #endregion
                    RecordLocationTransaction(ipDetail, ipDetailInput, effectiveDate, transType.Value, currentInventoryTransactionList);
                    inventoryTransactionList.AddRange(currentInventoryTransactionList);
                }
            }
            //else if (ipDetail.OrderType == com.Sconit.CodeMaster.OrderType.Procurement
            //    && ipDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Normal)
            //{
            //    #region 采购发货为供应商生成在途寄售物料
            //    foreach (IpDetailInput ipDetailInput in ipDetail.IpDetailInputs)
            //    {
            //        inventoryTransactionList = new List<InventoryTransaction>();

            //        InventoryTransaction inventoryTransaction = new InventoryTransaction();
            //        inventoryTransaction.Item = ipDetail.Item;
            //        inventoryTransaction.HuId = ipDetailInput.HuId;
            //        inventoryTransaction.LotNo = ipDetailInput.LotNo;
            //        inventoryTransaction.IsConsignment = true;
            //        inventoryTransaction.IsCreatePlanBill = true;
            //        PlanBill planBill = this.billMgr.CreatePlanBill(ipDetail, ipDetailInput, effectiveDate);
            //        inventoryTransaction.PlanBill = planBill.Id;
            //        inventoryTransaction.Qty = ipDetailInput.ShipQty;
            //        inventoryTransaction.QualityType = ipDetail.QualityType;
            //        inventoryTransaction.IsATP = ipDetail.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
            //        inventoryTransaction.IsFreeze = false;
            //        inventoryTransaction.OccupyType = com.Sconit.CodeMaster.OccupyType.None;
            //        inventoryTransactionList.Add(inventoryTransaction);
            //    }
            //    #endregion
            //}
            else if (((ipDetail.OrderType == com.Sconit.CodeMaster.OrderType.Procurement    //采购发货
                        || ipDetail.OrderType == com.Sconit.CodeMaster.OrderType.ScheduleLine        //计划协议
                        || ipDetail.OrderType == com.Sconit.CodeMaster.OrderType.CustomerGoods)      //客供品发货
                        && ipDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Normal)
                || (ipDetail.OrderType == com.Sconit.CodeMaster.OrderType.Distribution
                && ipDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Return)//销售退货发货
                        || ipDetail.OrderType == com.Sconit.CodeMaster.OrderType.SubContract   //委外
                        || ipDetail.OrderType == com.Sconit.CodeMaster.OrderType.Production     //生产
                )
            {
                #region 生成在途非寄售物料
                inventoryTransactionList = new List<InventoryTransaction>();
                foreach (IpDetailInput ipDetailInput in ipDetail.IpDetailInputs)
                {
                    InventoryTransaction inventoryTransaction = new InventoryTransaction();

                    inventoryTransaction.Item = ipDetail.Item;
                    inventoryTransaction.HuId = ipDetailInput.HuId;
                    inventoryTransaction.LotNo = ipDetailInput.LotNo;
                    inventoryTransaction.IsConsignment = false;
                    inventoryTransaction.IsCreatePlanBill = false;
                    inventoryTransaction.Qty = -ipDetailInput.ShipQty * ipDetail.UnitQty;  //转换为库存单位，出库位负数
                    inventoryTransaction.QualityType = ipDetail.QualityType;
                    inventoryTransaction.IsATP = ipDetail.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                    inventoryTransaction.IsFreeze = false;
                    inventoryTransaction.OccupyType = com.Sconit.CodeMaster.OccupyType.None;

                    inventoryTransactionList.Add(inventoryTransaction);
                }
                #endregion
            }
            else
            {
                throw new TechnicalException("未知情况，需要跟踪。");
            }

            return inventoryTransactionList;
        }

        //[Transaction(TransactionMode.Requires)]
        //public IList<InventoryTransaction> CancelInventoryOut(IpDetail ipDetail)
        //{
        //    return CancelInventoryOut(ipDetail, DateTime.Now);
        //}

        //[Transaction(TransactionMode.Requires)]
        //public IList<InventoryTransaction> CancelInventoryOut(IpDetail ipDetail, DateTime effectiveDate)
        //{
        //    List<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();
        //    com.Sconit.CodeMaster.TransactionType? transType = GetTransactionType(ipDetail);
        //    if (transType.HasValue)
        //    {
        //        foreach (IpDetailInput ipDetailInput in ipDetail.IpDetailInputs)
        //        {
        //            InventoryIO inventoryIO = new InventoryIO();

        //            inventoryIO.Location = ipDetail.LocationFrom;
        //            inventoryIO.Item = ipDetail.Item;
        //            inventoryIO.HuId = ipDetailInput.HuId;
        //            inventoryIO.Qty = ipDetailInput.ShipQty * ipDetail.UnitQty;  //转换为库存单位，冲销数为正数
        //            inventoryIO.LotNo = ipDetailInput.LotNo;
        //            inventoryIO.QualityType = ipDetail.QualityType;     //不合格品的ATP状态一定是false，合格品的状态一定是true，质检不采用ASN发货这里不可能出现
        //            inventoryIO.IsATP = ipDetail.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
        //            inventoryIO.IsFreeze = false;                       //可能指定移库冻结的零件？
        //            inventoryIO.IsCreatePlanBill = ipDetailInput.IsCreatePlanBill;
        //            inventoryIO.IsConsignment = ipDetailInput.IsConsignment;
        //            inventoryIO.PlanBill = ipDetailInput.PlanBill;
        //            inventoryIO.ActingBill = ipDetailInput.ActingBill;
        //            inventoryIO.TransactionType = transType.Value;
        //            inventoryIO.OccupyType = ipDetail.CurrentOccupyType;
        //            inventoryIO.OccupyReferenceNo = ipDetail.CurrentOccupyReferenceNo;
        //            inventoryIO.IsVoid = ipDetail.IsVoid;
        //            inventoryIO.EffectiveDate = effectiveDate;

        //            inventoryTransactionList.AddRange(RecordInventory(inventoryIO));
        //            RecordLocationTransaction(ipDetail, ipDetailInput, effectiveDate, transType.Value, inventoryTransactionList);
        //        }
        //    }
        //    else if (((ipDetail.OrderType == com.Sconit.CodeMaster.OrderType.Procurement    //采购发货
        //       || ipDetail.OrderType == com.Sconit.CodeMaster.OrderType.CustomerGoods)     //客供品发货
        //       && ipDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Normal)
        //       || (ipDetail.OrderType == com.Sconit.CodeMaster.OrderType.Distribution      //销售退货发货
        //       && ipDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Return))
        //    {
        //        //在途物料冲销不影响库存
        //    }
        //    else
        //    {
        //        throw new TechnicalException("未知情况，需要跟踪。");
        //    }

        //    return inventoryTransactionList;
        //}

        private com.Sconit.CodeMaster.TransactionType? GetTransactionType(IpDetail ipDetail)
        {
            if (ipDetail.OrderType == com.Sconit.CodeMaster.OrderType.Procurement
                || ipDetail.OrderType == com.Sconit.CodeMaster.OrderType.CustomerGoods)
            {
                if (ipDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Normal)
                {
                    return null;
                }
                else if (ipDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Return)
                {
                    if (!ipDetail.IsVoid)
                    {
                        return com.Sconit.CodeMaster.TransactionType.ISS_PO;
                    }
                    else
                    {
                        return com.Sconit.CodeMaster.TransactionType.ISS_PO_VOID;
                    }
                }
            }
            else if (ipDetail.OrderType == com.Sconit.CodeMaster.OrderType.ScheduleLine)
            {
                if (ipDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Normal)
                {
                    return null;
                }
                else if (ipDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Return)
                {
                    if (!ipDetail.IsVoid)
                    {
                        return com.Sconit.CodeMaster.TransactionType.ISS_SL;
                    }
                    else
                    {
                        return com.Sconit.CodeMaster.TransactionType.ISS_SL_VOID;
                    }
                }
            }
            else if (ipDetail.OrderType == com.Sconit.CodeMaster.OrderType.Distribution)
            {
                if (ipDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Normal)
                {
                    if (!ipDetail.IsVoid)
                    {
                        return com.Sconit.CodeMaster.TransactionType.ISS_SO;
                    }
                    else
                    {
                        return com.Sconit.CodeMaster.TransactionType.ISS_SO_VOID;
                    }
                }
                else if (ipDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Return)
                {
                    return null;
                }
            }
            else if (ipDetail.OrderType == com.Sconit.CodeMaster.OrderType.Transfer)
            {
                if (ipDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Normal)
                {
                    if (!ipDetail.IsVoid)
                    {
                        return com.Sconit.CodeMaster.TransactionType.ISS_TR;
                    }
                    else
                    {
                        return com.Sconit.CodeMaster.TransactionType.ISS_TR_VOID;
                    }
                }
                else if (ipDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Return)
                {
                    if (!ipDetail.IsVoid)
                    {
                        return com.Sconit.CodeMaster.TransactionType.ISS_TR_RTN;
                    }
                    else
                    {
                        return com.Sconit.CodeMaster.TransactionType.ISS_TR_RTN_VOID;
                    }
                }
            }
            else if (ipDetail.OrderType == com.Sconit.CodeMaster.OrderType.SubContractTransfer)
            {
                if (ipDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Normal)
                {
                    if (!ipDetail.IsVoid)
                    {
                        return com.Sconit.CodeMaster.TransactionType.ISS_STR;
                    }
                    else
                    {
                        return com.Sconit.CodeMaster.TransactionType.ISS_STR_VOID;
                    }
                }
                else if (ipDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Return)
                {
                    if (!ipDetail.IsVoid)
                    {
                        return com.Sconit.CodeMaster.TransactionType.ISS_STR_RTN;
                    }
                    else
                    {
                        return com.Sconit.CodeMaster.TransactionType.ISS_STR_RTN_VOID;
                    }
                }
            }
            else if (ipDetail.OrderType == com.Sconit.CodeMaster.OrderType.Production)
            {
                if (ipDetail.OrderSubType == CodeMaster.OrderSubType.Return)
                {
                    if (!ipDetail.IsVoid)
                    {
                        return com.Sconit.CodeMaster.TransactionType.ISS_WO_RTN;
                    }
                    else
                    {
                        return com.Sconit.CodeMaster.TransactionType.ISS_WO_RTN_VOID;
                    }
                }
                else
                {
                    return null;
                }
            }
            else if (ipDetail.OrderType == com.Sconit.CodeMaster.OrderType.SubContract)
            {
                if (ipDetail.OrderSubType == CodeMaster.OrderSubType.Return)
                {
                    if (!ipDetail.IsVoid)
                    {
                        return com.Sconit.CodeMaster.TransactionType.ISS_SWO_RTN;
                    }
                    else
                    {
                        return com.Sconit.CodeMaster.TransactionType.ISS_SWO_RTN_VOID;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new TechnicalException("Wrong order type of ipDetail:[" + ipDetail.OrderType + "]");
            }

            return null;
        }

        private void RecordLocationTransaction(IpDetail ipDetail, IpDetailInput ipDetailInput, DateTime effectiveDate,
           com.Sconit.CodeMaster.TransactionType transType, IList<InventoryTransaction> inventoryTransactionList)
        {
            DateTime dateTimeNow = DateTime.Now;

            //根据PlanBill和ActingBill分组，为了不同供应商的库存事务分开
            var groupedInventoryTransactionList = from trans in inventoryTransactionList
                                                  group trans by new
                                                  {
                                                      IsConsignment = trans.IsConsignment,
                                                      PlanBill = trans.PlanBill,
                                                      ActingBill = trans.ActingBill
                                                  }
                                                      into result
                                                      select new
                                                      {
                                                          IsConsignment = result.Key.IsConsignment,
                                                          PlanBill = result.Key.PlanBill,
                                                          ActingBill = result.Key.ActingBill,
                                                          Qty = result.Sum(trans => trans.Qty),
                                                          PlanBillQty = result.Sum(trans => trans.PlanBillQty),
                                                          ActingBillQty = result.Sum(trans => trans.ActingBillQty),
                                                          InventoryTransactionList = result.ToList()
                                                      };

            foreach (var groupedInventoryTransaction in groupedInventoryTransactionList)
            {
                LocationTransaction locationTransaction = new LocationTransaction();

                locationTransaction.OrderNo = ipDetail.OrderNo;
                locationTransaction.OrderType = ipDetail.OrderType;
                locationTransaction.OrderSubType = ipDetail.OrderSubType;
                locationTransaction.OrderDetailSequence = ipDetail.OrderDetailSequence;
                locationTransaction.OrderDetailId = ipDetail.OrderDetailId.Value;
                //locationTransaction.OrderBomDetId = 
                locationTransaction.IpNo = ipDetail.IpNo;
                locationTransaction.IpDetailId = ipDetail.Id;
                locationTransaction.IpDetailSequence = ipDetail.Sequence;
                locationTransaction.ReceiptNo = string.Empty;
                //locationTransaction.RecDetSeq = 
                locationTransaction.SequenceNo = ipDetailInput.SequenceNo;
                //locationTransaction.TraceCode =
                locationTransaction.Item = ipDetail.Item;
                locationTransaction.Uom = ipDetail.Uom;
                locationTransaction.BaseUom = ipDetail.BaseUom;
                locationTransaction.Qty = groupedInventoryTransaction.Qty / ipDetail.UnitQty;
                locationTransaction.UnitQty = ipDetail.UnitQty;
                locationTransaction.IsConsignment = groupedInventoryTransaction.IsConsignment;
                if (groupedInventoryTransaction.IsConsignment && groupedInventoryTransaction.PlanBill.HasValue)
                {
                    locationTransaction.PlanBill = groupedInventoryTransaction.PlanBill.Value;
                }
                locationTransaction.PlanBillQty = groupedInventoryTransaction.PlanBillQty / ipDetail.UnitQty;
                if (groupedInventoryTransaction.ActingBill.HasValue)
                {
                    locationTransaction.ActingBill = groupedInventoryTransaction.ActingBill.Value;
                }
                locationTransaction.ActingBillQty = groupedInventoryTransaction.ActingBillQty / ipDetail.UnitQty;
                locationTransaction.QualityType = ipDetail.QualityType;
                locationTransaction.HuId = ipDetailInput.HuId;
                locationTransaction.LotNo = ipDetailInput.LotNo;
                locationTransaction.TransactionType = transType;
                locationTransaction.IOType = CodeMaster.TransactionIOType.Out;
                locationTransaction.PartyFrom = ipDetail.CurrentPartyFrom;
                locationTransaction.PartyTo = ipDetail.CurrentPartyTo;
                locationTransaction.LocationFrom = ipDetail.LocationFrom;
                locationTransaction.LocationTo = ipDetail.LocationTo;
                locationTransaction.LocationIOReason = string.Empty;
                locationTransaction.EffectiveDate = effectiveDate;
                locationTransaction.CreateUserId = SecurityContextHolder.Get().Id;
                locationTransaction.CreateDate = dateTimeNow;

                this.genericMgr.Create(locationTransaction);

                RecordLocationTransactionDetail(locationTransaction, groupedInventoryTransaction.InventoryTransactionList);
            }
        }
        #endregion

        #region 收货入库
        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> InventoryIn(ReceiptDetail receiptDetail)
        {
            return InventoryIn(receiptDetail, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> InventoryIn(ReceiptDetail receiptDetail, DateTime effectiveDate)
        {
            List<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();
            com.Sconit.CodeMaster.TransactionType? transType = GetTransactionType(receiptDetail);
            if (transType.HasValue)
            {
                #region 收货冲销，如果一个条码对应多条receiptDetailInput，需要先拆箱
                if (receiptDetail.IsVoid)
                {
                    var groupedHuIdList = from det in receiptDetail.ReceiptDetailInputs
                                          where !string.IsNullOrWhiteSpace(det.HuId)
                                          group det by det.HuId into gj
                                          select new
                                          {
                                              HuId = gj.Key,
                                              ReceiptDetailInputList = gj.ToList()
                                          };

                    if (groupedHuIdList != null && groupedHuIdList.Count() > 0)
                    {
                        //已知Bug，如果收货时不结算，两条不同的PlanBill生成条码会强制结算。但是在冲销时不会反结算。
                        #region 循环拆箱
                        foreach (var groupedHuId in groupedHuIdList.Where(g => g.ReceiptDetailInputList.Count() > 1))
                        {
                            InventoryUnPack inventoryUnPack = new InventoryUnPack();

                            inventoryUnPack.HuId = groupedHuId.HuId;
                            inventoryUnPack.CurrentHu = this.huMgr.GetHuStatus(groupedHuId.HuId);
                            IList<InventoryUnPack> inventoryUnPackList = new List<InventoryUnPack>();
                            inventoryUnPackList.Add(inventoryUnPack);
                            this.InventoryUnPack(inventoryUnPackList, effectiveDate);

                            foreach (ReceiptDetailInput receiptDetailInput in groupedHuId.ReceiptDetailInputList)
                            {
                                //把ReceiptDetailInput的HuId和LotNo至空，按数量出库。
                                receiptDetailInput.HuId = null;
                                receiptDetailInput.LotNo = null;
                                receiptDetailInput.IsConsignment = false;
                                receiptDetailInput.PlanBill = null;
                            }
                        }
                        #endregion

                        this.genericMgr.FlushSession();
                    }
                }
                #endregion

                foreach (ReceiptDetailInput receiptDetailInput in receiptDetail.ReceiptDetailInputs)
                {
                    if (receiptDetailInput.ReceivedIpLocationDetailList != null && receiptDetailInput.ReceivedIpLocationDetailList.Count > 0)
                    {
                        List<InventoryTransaction> currentReceiptDetailInputInventoryTransactionList = new List<InventoryTransaction>();
                        #region 基于Ip收货，零件的寄售信息都在IpLocationDetail上面
                        foreach (IpLocationDetail receivedIpLocationDetail in receiptDetailInput.ReceivedIpLocationDetailList)
                        {
                            ReceiptDetailInput thisReceiptDetailInput = Mapper.Map<ReceiptDetailInput, ReceiptDetailInput>(receiptDetailInput);
                            thisReceiptDetailInput.ReceiveQty = (decimal)receivedIpLocationDetail.ReceivedQty / receiptDetail.UnitQty;
                            if (receiptDetail.CurrentIsReceiveScanHu && !string.IsNullOrWhiteSpace(receivedIpLocationDetail.HuId))
                            {
                                thisReceiptDetailInput.HuId = receivedIpLocationDetail.HuId;
                                thisReceiptDetailInput.LotNo = receivedIpLocationDetail.LotNo;
                            }

                            InventoryIO inventoryIO = new InventoryIO();

                            inventoryIO.Location = receiptDetail.LocationTo;
                            inventoryIO.Item = receiptDetail.Item;
                            if (receiptDetail.CurrentIsReceiveScanHu && receiptDetailInput.ReceivedIpLocationDetailList.Count == 1)
                            {
                                //如果收货扫描条码，并且只有一条发货库存明细，可以直接收为条码
                                //要用收货Input上指定的条码
                                inventoryIO.HuId = thisReceiptDetailInput.HuId;
                                inventoryIO.LotNo = thisReceiptDetailInput.LotNo;
                            }
                            inventoryIO.Qty = receivedIpLocationDetail.ReceivedQty;             //库存单位
                            inventoryIO.QualityType = receivedIpLocationDetail.QualityType;     //不合格品的ATP状态一定是false，合格品的状态一定是true，质检不采用ASN发货这里不可能出现
                            inventoryIO.IsATP = receivedIpLocationDetail.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                            inventoryIO.IsFreeze = false;                       //可能指定移库冻结的零件？
                            inventoryIO.IsCreatePlanBill = receivedIpLocationDetail.IsCreatePlanBill;
                            inventoryIO.IsConsignment = receivedIpLocationDetail.IsConsignment;
                            inventoryIO.PlanBill = receivedIpLocationDetail.PlanBill;
                            inventoryIO.ActingBill = receivedIpLocationDetail.ActingBill;
                            inventoryIO.TransactionType = transType.Value;
                            //if (receivedIpLocationDetail.OccupyType == CodeMaster.OccupyType.Inspect)   //只有检验的收货要保留占用，其它收货不用保留
                            //{
                            inventoryIO.OccupyType = receivedIpLocationDetail.OccupyType;
                            inventoryIO.OccupyReferenceNo = receivedIpLocationDetail.OccupyReferenceNo;
                            //}
                            //inventoryIO.IsVoid = receiptDetail.IsVoid;
                            inventoryIO.EffectiveDate = effectiveDate;

                            #region 寄售处理
                            if (!receiptDetail.IsVoid &&
                                (((receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.Procurement
                                || receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.ScheduleLine)
                                && receiptDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Normal)
                               || (receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.Distribution
                                    && transType != CodeMaster.TransactionType.RCT_TR)  //差异调整至发货方，不用产生结算
                               || receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.SubContract))
                            {
                                if (receivedIpLocationDetail.IsConsignment && receivedIpLocationDetail.PlanBill.HasValue)
                                {
                                    throw new TechnicalException("Can't create new planbill when receiptDetailInput aready has unsettled planbill.");
                                }
                                else
                                {
                                    #region 记录待结算
                                    PlanBill planBill = this.billMgr.CreatePlanBill(receiptDetail, thisReceiptDetailInput, effectiveDate);

                                    inventoryIO.IsConsignment = true;
                                    inventoryIO.IsCreatePlanBill = true;
                                    inventoryIO.PlanBill = planBill.Id;
                                    inventoryIO.CurrentPlanBill = planBill;

                                    thisReceiptDetailInput.IsConsignment = true;
                                    thisReceiptDetailInput.IsCreatePlanBill = true;
                                    thisReceiptDetailInput.PlanBill = planBill.Id;
                                    #endregion
                                }
                            }
                            #endregion

                            IList<InventoryTransaction> currentInventoryTransactionList = RecordInventory(inventoryIO);
                            #region 记录HuId，LotNo和WMS发货单行号
                            foreach (InventoryTransaction currentInventoryTransaction in currentInventoryTransactionList)
                            {
                                if (receiptDetail.CurrentIsReceiveScanHu && receiptDetailInput.ReceivedIpLocationDetailList.Count > 1
                                    && !string.IsNullOrWhiteSpace(receiptDetailInput.HuId))
                                {
                                    Hu hu = this.genericMgr.FindById<Hu>(receiptDetailInput.HuId);
                                    currentInventoryTransaction.HuId = hu.HuId;
                                    currentInventoryTransaction.LotNo = hu.LotNo;
                                }
                                currentInventoryTransaction.WMSRecSeq = receiptDetailInput.WMSRecSeq;
                            }
                            #endregion

                            RecordLocationTransaction(receiptDetail, thisReceiptDetailInput, effectiveDate, transType.Value, currentInventoryTransactionList);
                            inventoryTransactionList.AddRange(currentInventoryTransactionList);
                            currentReceiptDetailInputInventoryTransactionList.AddRange(currentInventoryTransactionList);
                        }

                        #region 收货扫描条码，需要装箱创建条码
                        if (receiptDetail.CurrentIsReceiveScanHu && receiptDetailInput.ReceivedIpLocationDetailList.Count > 1
                            && !string.IsNullOrWhiteSpace(receiptDetailInput.HuId))
                        {
                            #region 装箱
                            IList<InventoryPack> inventoryPackList = new List<InventoryPack>();
                            InventoryPack inventoryPack = new InventoryPack();
                            inventoryPack.Location = receiptDetail.LocationTo;
                            inventoryPack.HuId = receiptDetailInput.HuId;
                            inventoryPack.LocationLotDetailIdList = currentReceiptDetailInputInventoryTransactionList.Select(inv => inv.LocationLotDetailId).ToList();

                            inventoryPackList.Add(inventoryPack);
                            this.InventoryPack(inventoryPackList, effectiveDate);
                            #endregion
                        }
                        #endregion
                        #endregion
                    }
                    else
                    {
                        #region 基于ReceiptDetailInput收货
                        InventoryIO inventoryIO = new InventoryIO();

                        inventoryIO.Location = receiptDetail.LocationTo;
                        inventoryIO.Item = receiptDetail.Item;
                        inventoryIO.HuId = receiptDetailInput.HuId;
                        inventoryIO.LotNo = receiptDetailInput.LotNo;
                        inventoryIO.Qty = receiptDetailInput.ReceiveQty * receiptDetail.UnitQty;  //转换为库存单位
                        inventoryIO.QualityType = receiptDetailInput.QualityType;     //不合格品的ATP状态一定是false，合格品的状态一定是true，质检不采用ASN发货这里不可能出现
                        inventoryIO.IsATP = receiptDetailInput.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                        inventoryIO.IsFreeze = false;                       //可能指定移库冻结的零件？
                        inventoryIO.IsCreatePlanBill = receiptDetailInput.IsCreatePlanBill;
                        inventoryIO.IsConsignment = receiptDetailInput.IsConsignment;
                        inventoryIO.PlanBill = receiptDetailInput.PlanBill;
                        inventoryIO.ActingBill = receiptDetailInput.ActingBill;
                        inventoryIO.TransactionType = transType.Value;
                        inventoryIO.OccupyType = receiptDetailInput.OccupyType;
                        inventoryIO.OccupyReferenceNo = receiptDetailInput.OccupyReferenceNo;
                        inventoryIO.IsVoid = receiptDetail.IsVoid;
                        inventoryIO.EffectiveDate = effectiveDate;
                        //inventoryIO.ManufactureParty = ;

                        #region 寄售处理
                        if (!receiptDetail.IsVoid &&
                            (((receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.Procurement
                            || receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.ScheduleLine)
                            && receiptDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Normal)
                           || (receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.Distribution
                                && transType != CodeMaster.TransactionType.RCT_TR)  //差异调整至发货方，不用产生结算
                           || receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.SubContract))
                        {
                            if (receiptDetailInput.IsConsignment && receiptDetailInput.PlanBill.HasValue)
                            {
                                throw new TechnicalException("Can't create new planbill when receiptDetailInput aready has unsettled planbill.");
                            }

                            #region 记录待结算
                            PlanBill planBill = this.billMgr.CreatePlanBill(receiptDetail, receiptDetailInput, effectiveDate);

                            inventoryIO.IsConsignment = true;
                            inventoryIO.IsCreatePlanBill = true;
                            inventoryIO.PlanBill = planBill.Id;
                            inventoryIO.CurrentPlanBill = planBill;

                            receiptDetailInput.IsConsignment = true;
                            receiptDetailInput.IsCreatePlanBill = true;
                            receiptDetailInput.PlanBill = planBill.Id;
                            #endregion

                            #region 判断是否立即结算
                            //if (receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.SubContract)   //委外加工立即结算
                            //{
                            //    isBillSettle = true;
                            //}
                            //else if (receiptDetail.ReceivedQty < 0)  // 判断寄售负库存是否立即结算
                            //{
                            //    isBillSettle = true;
                            //}
                            //else if (string.IsNullOrWhiteSpace(receiptDetail.LocationTo) == null
                            //    && receiptDetail.BillTerm == com.Sconit.CodeMaster.OrderBillTerm.ReceivingSettlement) //判断目的库位为空的立即结算，如销售收货结算
                            //{
                            //    isBillSettle = true;
                            //}

                            //if (isBillSettle)
                            //{
                            //    //结算
                            //    planBill.CurrentActingQty = planBill.PlanQty;
                            //    BillTransaction billTransaction = this.billMgr.SettleBill(planBill, effectiveDate);
                            //    inventoryIO.IsConsignment = false;
                            //    receiptDetailInput.IsConsignment = false;
                            //}
                            #endregion
                        }
                        #endregion

                        IList<InventoryTransaction> currentInventoryTransactionList = RecordInventory(inventoryIO);
                        #region 记录WMS发货单行号
                        foreach (InventoryTransaction currentInventoryTransaction in currentInventoryTransactionList)
                        {
                            currentInventoryTransaction.WMSRecSeq = receiptDetailInput.WMSRecSeq;
                        }
                        #endregion
                        RecordLocationTransaction(receiptDetail, receiptDetailInput, effectiveDate, transType.Value, currentInventoryTransactionList);
                        inventoryTransactionList.AddRange(currentInventoryTransactionList);
                        #endregion
                    }
                }

                return inventoryTransactionList;
            }
            else if ((receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.Procurement
                    || receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.ScheduleLine
                    || receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.SubContract)
                  && receiptDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Return)
            {
                #region 采购退货收货
                if (!receiptDetail.IsVoid)
                {
                    foreach (ReceiptDetailInput receiptDetailInput in receiptDetail.ReceiptDetailInputs)
                    {
                        foreach (IpLocationDetail receivedIpLocationDetail in receiptDetailInput.ReceivedIpLocationDetailList)
                        {
                            InventoryTransaction inventoryTransaction = new InventoryTransaction();
                            inventoryTransaction.Item = receiptDetail.Item;
                            inventoryTransaction.HuId = receiptDetailInput.HuId;
                            inventoryTransaction.LotNo = receiptDetailInput.LotNo;
                            inventoryTransaction.IsConsignment = receivedIpLocationDetail.IsConsignment;
                            //inventoryTransaction.IsCreatePlanBill = receiptDetailInput.IsCreatePlanBill;
                            inventoryTransaction.PlanBill = receivedIpLocationDetail.PlanBill;
                            if (receivedIpLocationDetail.IsConsignment && receivedIpLocationDetail.PlanBill.HasValue)
                            {
                                inventoryTransaction.PlanBillQty = receivedIpLocationDetail.ReceivedQty;  //转换为库存单位
                            }
                            //inventoryTransaction.ActingBill = receiptDetailInput.ActingBill;
                            //inventoryTransaction.ActingBillQty = ;
                            inventoryTransaction.Qty = receivedIpLocationDetail.ReceivedQty;  //转换为库存单位，入库位正数
                            inventoryTransaction.QualityType = receiptDetailInput.QualityType;
                            inventoryTransaction.IsATP = receiptDetailInput.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                            inventoryTransaction.IsFreeze = false;
                            inventoryTransaction.OccupyType = receiptDetailInput.OccupyType;
                            inventoryTransaction.OccupyReferenceNo = receiptDetailInput.OccupyReferenceNo;

                            #region 退货收货
                            if (receivedIpLocationDetail.IsConsignment && receivedIpLocationDetail.PlanBill.HasValue)
                            {
                                //如果寄售库存，冲销PlanBill。
                                PlanBill planBill = this.genericMgr.FindById<PlanBill>(receivedIpLocationDetail.PlanBill.Value);
                                planBill.CurrentVoidQty = receivedIpLocationDetail.ReceivedQty / receiptDetail.UnitQty;  //订单单位
                                this.billMgr.VoidPlanBill(planBill);

                                //inventoryTransaction.IsConsignment = false;
                                //inventoryTransaction.PlanBill = null;
                            }
                            else
                            {
                                //非寄售库存产生负数PlanBill，立即结算。
                                ReceiptDetailInput thisReceiptDetailInput = Mapper.Map<ReceiptDetailInput, ReceiptDetailInput>(receiptDetailInput);
                                thisReceiptDetailInput.ReceiveQty = receivedIpLocationDetail.ReceivedQty / receiptDetail.UnitQty;   //订单单位

                                PlanBill planBill = this.billMgr.CreatePlanBill(receiptDetail, thisReceiptDetailInput, effectiveDate);
                                planBill.CurrentActingQty = planBill.PlanQty;
                                planBill.CurrentLocation = inventoryTransaction.Location;
                                SettleBillTransaction billTransaction = this.billMgr.SettleBill(planBill, effectiveDate);

                                inventoryTransaction.IsConsignment = false;
                                inventoryTransaction.IsCreatePlanBill = true;
                                inventoryTransaction.PlanBill = planBill.Id;
                                inventoryTransaction.PlanBillQty = 0;
                                inventoryTransaction.ActingBill = billTransaction.ActingBill;
                                inventoryTransaction.ActingBillQty = receiptDetailInput.ReceiveQty * receiptDetail.UnitQty;
                            }
                            #endregion

                            inventoryTransactionList.Add(inventoryTransaction);
                        }
                    }
                }
                #endregion

                #region 采购退货收货冲销
                else
                {
                    #region 采购退货收货冲销
                    foreach (ReceiptLocationDetail receiptLocationDetail in receiptDetail.ReceiptLocationDetails)
                    {
                        InventoryTransaction inventoryTransaction = new InventoryTransaction();
                        inventoryTransaction.Item = receiptDetail.Item;
                        inventoryTransaction.HuId = receiptLocationDetail.HuId;
                        inventoryTransaction.LotNo = receiptLocationDetail.LotNo;
                        inventoryTransaction.IsConsignment = receiptLocationDetail.IsConsignment;
                        //inventoryTransaction.IsCreatePlanBill = receiptDetailInput.IsCreatePlanBill;
                        inventoryTransaction.PlanBill = receiptLocationDetail.PlanBill;
                        if (receiptLocationDetail.IsConsignment && receiptLocationDetail.PlanBill.HasValue)
                        {
                            inventoryTransaction.PlanBillQty = receiptLocationDetail.Qty;  //转换为库存单位
                        }
                        //inventoryTransaction.ActingBill = receiptDetailInput.ActingBill;
                        //inventoryTransaction.ActingBillQty = ;
                        inventoryTransaction.Qty = receiptLocationDetail.Qty;  //转换为库存单位，入库位正数
                        inventoryTransaction.QualityType = receiptLocationDetail.QualityType;
                        inventoryTransaction.IsATP = receiptLocationDetail.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                        inventoryTransaction.IsFreeze = false;
                        inventoryTransaction.OccupyType = receiptLocationDetail.OccupyType;
                        inventoryTransaction.OccupyReferenceNo = receiptLocationDetail.OccupyReferenceNo;

                        if (receiptLocationDetail.IsConsignment && receiptLocationDetail.PlanBill.HasValue)
                        {
                            PlanBill planBill = this.genericMgr.FindById<PlanBill>(receiptLocationDetail.PlanBill);
                            planBill.CurrentVoidQty = -receiptLocationDetail.Qty / receiptDetail.UnitQty;  //退货收货冲销，要反冲销PlanQty。转为订单单位
                            this.billMgr.VoidPlanBill(planBill);
                        }

                        if (receiptLocationDetail.ActingBill.HasValue)
                        {
                            ActingBill actingBill = this.genericMgr.FindById<ActingBill>(receiptLocationDetail.ActingBill);
                            PlanBill planBill = this.genericMgr.FindById<PlanBill>(receiptLocationDetail.PlanBill);
                            actingBill.CurrentVoidQty = -receiptLocationDetail.Qty / receiptDetail.UnitQty;   //转为订单单位
                            this.billMgr.VoidSettleBill(actingBill, planBill, receiptLocationDetail.IsCreatePlanBill);
                        }

                        inventoryTransactionList.Add(inventoryTransaction);
                    }
                    #endregion
                }
                #endregion

                return inventoryTransactionList;
            }
            else if ((receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.Procurement
                        || receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.ScheduleLine
                        || receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.SubContract)
                    && receiptDetail.IpDetailType == CodeMaster.IpDetailType.Gap
                    && receiptDetail.IpGapAdjustOption == CodeMaster.IpGapAdjustOption.GI)
            {
                #region 差异收货调整至供应商
                foreach (ReceiptDetailInput receiptDetailInput in receiptDetail.ReceiptDetailInputs)
                {
                    InventoryTransaction inventoryTransaction = new InventoryTransaction();
                    inventoryTransaction.Item = receiptDetail.Item;
                    inventoryTransaction.HuId = receiptDetailInput.HuId;
                    inventoryTransaction.LotNo = receiptDetailInput.LotNo;
                    inventoryTransaction.IsConsignment = false;
                    inventoryTransaction.IsCreatePlanBill = false;
                    //inventoryTransaction.PlanBill = receiptDetailInput.PlanBill;
                    //inventoryTransaction.PlanBillQty = receiptDetailInput.ReceiveQty * receiptDetail.UnitQty;  //转换为库存单位
                    //inventoryTransaction.ActingBill = receiptDetailInput.ActingBill;
                    //inventoryTransaction.ActingBillQty = ;
                    inventoryTransaction.Qty = receiptDetailInput.ReceiveQty * receiptDetail.UnitQty;  //转换为库存单位，入库位正数
                    inventoryTransaction.QualityType = receiptDetailInput.QualityType;
                    inventoryTransaction.IsATP = receiptDetailInput.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                    inventoryTransaction.IsFreeze = false;
                    //inventoryTransaction.OccupyType = receiptDetailInput.OccupyType;
                    //inventoryTransaction.OccupyReferenceNo = receiptDetailInput.OccupyReferenceNo;

                    inventoryTransactionList.Add(inventoryTransaction);
                }
                #endregion
            }
            else if (receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.Production
                && receiptDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Return)
            {
                #region 原材料回用
                foreach (ReceiptDetailInput receiptDetailInput in receiptDetail.ReceiptDetailInputs)
                {
                    InventoryTransaction inventoryTransaction = new InventoryTransaction();
                    inventoryTransaction.Item = receiptDetail.Item;
                    inventoryTransaction.HuId = receiptDetailInput.HuId;
                    inventoryTransaction.LotNo = receiptDetailInput.LotNo;
                    inventoryTransaction.IsConsignment = false;
                    inventoryTransaction.IsCreatePlanBill = false;
                    //inventoryTransaction.PlanBill = receiptDetailInput.PlanBill;
                    //inventoryTransaction.PlanBillQty = receiptDetailInput.ReceiveQty * receiptDetail.UnitQty;  //转换为库存单位
                    //inventoryTransaction.ActingBill = receiptDetailInput.ActingBill;
                    //inventoryTransaction.ActingBillQty = ;
                    inventoryTransaction.Qty = receiptDetailInput.ReceiveQty * receiptDetail.UnitQty;  //转换为库存单位，入库位正数
                    inventoryTransaction.QualityType = receiptDetailInput.QualityType;
                    inventoryTransaction.IsATP = receiptDetailInput.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                    inventoryTransaction.IsFreeze = false;
                    //inventoryTransaction.OccupyType = receiptDetailInput.OccupyType;
                    //inventoryTransaction.OccupyReferenceNo = receiptDetailInput.OccupyReferenceNo;

                    inventoryTransactionList.Add(inventoryTransaction);
                }
                #endregion
            }
            else if (receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.Distribution
                  && receiptDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Normal)
            {
                #region 销售收货
                foreach (ReceiptDetailInput receiptDetailInput in receiptDetail.ReceiptDetailInputs)
                {
                    InventoryTransaction inventoryTransaction = new InventoryTransaction();
                    inventoryTransaction.Item = receiptDetail.Item;
                    inventoryTransaction.HuId = receiptDetailInput.HuId;
                    inventoryTransaction.LotNo = receiptDetailInput.LotNo;
                    //inventoryTransaction.IsConsignment = receiptDetailInput.IsConsignment;
                    //inventoryTransaction.IsCreatePlanBill = receiptDetailInput.IsCreatePlanBill;
                    //inventoryTransaction.PlanBill = receiptDetailInput.PlanBill;
                    //inventoryTransaction.ActingBill = receiptDetailInput.ActingBill;
                    inventoryTransaction.Qty = receiptDetailInput.ReceiveQty * receiptDetail.UnitQty;  //转换为库存单位，入库位正数
                    inventoryTransaction.QualityType = receiptDetailInput.QualityType;
                    inventoryTransaction.IsATP = receiptDetailInput.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                    inventoryTransaction.IsFreeze = false;
                    inventoryTransaction.OccupyType = receiptDetailInput.OccupyType;
                    inventoryTransaction.OccupyReferenceNo = receiptDetailInput.OccupyReferenceNo;

                    if (!receiptDetail.IsVoid)
                    {
                        #region 收货
                        PlanBill planBill = this.billMgr.CreatePlanBill(receiptDetail, receiptDetailInput, effectiveDate);
                        inventoryTransaction.IsConsignment = true;
                        inventoryTransaction.IsCreatePlanBill = true;
                        inventoryTransaction.PlanBill = planBill.Id;

                        if (receiptDetail.BillTerm == com.Sconit.CodeMaster.OrderBillTerm.ReceivingSettlement)
                        {
                            planBill.CurrentActingQty = planBill.PlanQty;
                            planBill.CurrentLocation = planBill.LocationFrom;
                            SettleBillTransaction billTransaction = this.billMgr.SettleBill(planBill, effectiveDate);

                            inventoryTransaction.IsConsignment = false;
                            inventoryTransaction.PlanBillQty = 0;
                            inventoryTransaction.ActingBill = billTransaction.ActingBill;
                            inventoryTransaction.ActingBillQty = receiptDetailInput.ReceiveQty * receiptDetail.UnitQty;
                        }
                        #endregion
                    }
                    else
                    {
                        #region 收货冲销
                        if (receiptDetailInput.ActingBill.HasValue)
                        {
                            ActingBill actingBill = this.genericMgr.FindById<ActingBill>(receiptDetailInput.ActingBill);
                            PlanBill planBill = this.genericMgr.FindById<PlanBill>(receiptDetailInput.PlanBill);
                            actingBill.CurrentVoidQty = -receiptDetailInput.ReceiveQty;
                            this.billMgr.VoidSettleBill(actingBill, planBill, receiptDetailInput.IsCreatePlanBill);
                        }
                        else if (receiptDetailInput.IsCreatePlanBill)
                        {
                            PlanBill planBill = this.genericMgr.FindById<PlanBill>(receiptDetailInput.PlanBill);
                            planBill.CurrentVoidQty = -receiptDetailInput.ReceiveQty;
                            this.billMgr.VoidPlanBill(planBill);
                        }
                        #endregion
                    }

                    inventoryTransactionList.Add(inventoryTransaction);
                }
                #endregion
            }
            else
            {
                throw new TechnicalException("未知情况，需要跟踪。");
            }

            return inventoryTransactionList;
        }

        private com.Sconit.CodeMaster.TransactionType? GetTransactionType(ReceiptDetail receiptDetail)
        {
            if (receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.Procurement
                || receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.CustomerGoods)
            {
                if (receiptDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Normal)
                {
                    if (receiptDetail.IpDetailType == CodeMaster.IpDetailType.Gap &&
                        receiptDetail.IpGapAdjustOption == CodeMaster.IpGapAdjustOption.GI)
                    {
                        //差异收货调整至发货方（供应商）
                        return null;
                    }
                    else if (!receiptDetail.IsVoid)
                    {
                        return com.Sconit.CodeMaster.TransactionType.RCT_PO;
                    }
                    else
                    {
                        return com.Sconit.CodeMaster.TransactionType.RCT_PO_VOID;
                    }
                }
                else if (receiptDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Return)
                {
                    //if (receiptDetail.IsVoid)
                    //{
                    //    //采购退货冲销
                    //    return com.Sconit.CodeMaster.TransactionType.ISS_PO_VOID;
                    //}
                    //else
                    //{
                    //    return null;
                    //}
                    return null;
                }
            }
            else if (receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.ScheduleLine)
            {
                if (receiptDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Normal)
                {
                    if (receiptDetail.IpDetailType == CodeMaster.IpDetailType.Gap
                        && receiptDetail.IpGapAdjustOption == CodeMaster.IpGapAdjustOption.GI)
                    {
                        //差异收货调整至发货方（供应商）
                        return null;
                    }
                    else if (!receiptDetail.IsVoid)
                    {
                        return com.Sconit.CodeMaster.TransactionType.RCT_SL;
                    }
                    else
                    {
                        return com.Sconit.CodeMaster.TransactionType.RCT_SL_VOID;
                    }
                }
                else if (receiptDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Return)
                {
                    if (receiptDetail.IsVoid)
                    {
                        //采购退货冲销
                        return com.Sconit.CodeMaster.TransactionType.ISS_SL_VOID;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else if (receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.Distribution)
            {
                if (receiptDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Normal)
                {
                    if (receiptDetail.IpDetailType == CodeMaster.IpDetailType.Gap
                        && receiptDetail.IpGapAdjustOption == CodeMaster.IpGapAdjustOption.GI)
                    {
                        //差异收货调整至发货方，作为移库接收
                        return com.Sconit.CodeMaster.TransactionType.RCT_TR;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (receiptDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Return)
                {
                    if (!receiptDetail.IsVoid)
                    {
                        return com.Sconit.CodeMaster.TransactionType.RCT_SO;
                    }
                    else
                    {
                        return com.Sconit.CodeMaster.TransactionType.RCT_SO_VOID;
                    }
                }
            }
            else if (receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.Transfer)
            {
                if (receiptDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Normal)
                {
                    if (!receiptDetail.IsVoid)
                    {
                        return com.Sconit.CodeMaster.TransactionType.RCT_TR;
                    }
                    else
                    {
                        return com.Sconit.CodeMaster.TransactionType.RCT_TR_VOID;
                    }
                }
                else if (receiptDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Return)
                {
                    if (!receiptDetail.IsVoid)
                    {
                        return com.Sconit.CodeMaster.TransactionType.RCT_TR_RTN;
                    }
                    else
                    {
                        return com.Sconit.CodeMaster.TransactionType.RCT_TR_RTN_VOID;
                    }
                }
            }
            else if (receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.SubContractTransfer)
            {
                if (receiptDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Normal)
                {
                    if (!receiptDetail.IsVoid)
                    {
                        return com.Sconit.CodeMaster.TransactionType.RCT_STR;
                    }
                    else
                    {
                        return com.Sconit.CodeMaster.TransactionType.RCT_STR_VOID;
                    }
                }
                else if (receiptDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Return)
                {
                    if (!receiptDetail.IsVoid)
                    {
                        return com.Sconit.CodeMaster.TransactionType.RCT_STR_RTN;
                    }
                    else
                    {
                        return com.Sconit.CodeMaster.TransactionType.RCT_STR_RTN_VOID;
                    }
                }
            }
            else if (receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.Production)
            {

                if (receiptDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Return)
                {
                    return null;
                }
                else
                {
                    if (receiptDetail.IpDetailType == CodeMaster.IpDetailType.Gap
                     && receiptDetail.IpGapAdjustOption == CodeMaster.IpGapAdjustOption.GI)
                    {
                        return null;
                    }
                    else
                    {
                        if (!receiptDetail.IsVoid)
                        {
                            return com.Sconit.CodeMaster.TransactionType.RCT_WO;
                        }
                        else
                        {
                            return com.Sconit.CodeMaster.TransactionType.RCT_WO_VOID;
                        }
                    }
                }
            }
            else if (receiptDetail.OrderType == com.Sconit.CodeMaster.OrderType.SubContract)
            {
                if (receiptDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Normal)
                {
                    //调整发货方库存,委外不作调整
                    if (receiptDetail.IpDetailType == CodeMaster.IpDetailType.Gap
                        && receiptDetail.IpGapAdjustOption == CodeMaster.IpGapAdjustOption.GI)
                    {
                        return null;
                    }
                    else
                    {
                        if (!receiptDetail.IsVoid)
                        {
                            return com.Sconit.CodeMaster.TransactionType.RCT_SWO;
                        }
                        else
                        {
                            return com.Sconit.CodeMaster.TransactionType.RCT_SWO_VOID;
                        }
                    }
                }
                else if (receiptDetail.OrderSubType == com.Sconit.CodeMaster.OrderSubType.Return)
                {
                    return null;
                }
            }
            else
            {
                throw new TechnicalException("Wrong order type of receiptDetail:[" + receiptDetail.OrderType + "]");
            }

            return null;
        }

        private void RecordLocationTransaction(ReceiptDetail receiptDetail, ReceiptDetailInput receiptDetailInput, DateTime effectiveDate,
           com.Sconit.CodeMaster.TransactionType transType, IList<InventoryTransaction> inventoryTransactionList)
        {
            DateTime dateTimeNow = DateTime.Now;
            //根据PlanBill和ActingBill分组，为了不同供应商的库存事务分开
            var groupedInventoryTransactionList = from trans in inventoryTransactionList
                                                  group trans by new
                                                  {
                                                      IsConsignment = trans.IsConsignment,
                                                      PlanBill = trans.PlanBill,
                                                      ActingBill = trans.ActingBill
                                                  }
                                                      into result
                                                      select new
                                                      {
                                                          IsConsignment = result.Key.IsConsignment,
                                                          PlanBill = result.Key.PlanBill,
                                                          ActingBill = result.Key.ActingBill,
                                                          Qty = result.Sum(trans => trans.Qty),
                                                          PlanBillQty = result.Sum(trans => trans.PlanBillQty),
                                                          ActingBillQty = result.Sum(trans => trans.ActingBillQty),
                                                          InventoryTransactionList = result.ToList()
                                                      };

            foreach (var groupedInventoryTransaction in groupedInventoryTransactionList)
            {
                LocationTransaction locationTransaction = new LocationTransaction();

                locationTransaction.OrderNo = receiptDetail.OrderNo;
                locationTransaction.OrderType = receiptDetail.OrderType;
                locationTransaction.OrderSubType = receiptDetail.OrderSubType;
                locationTransaction.OrderDetailSequence = receiptDetail.OrderDetailSequence;
                locationTransaction.OrderDetailId = receiptDetail.OrderDetailId.Value;
                //locationTransaction.OrderBomDetId = 
                locationTransaction.IpNo = receiptDetail.IpNo;
                locationTransaction.IpDetailId = receiptDetail.IpDetailId.HasValue ? receiptDetail.IpDetailId.Value : 0;
                locationTransaction.IpDetailSequence = receiptDetail.IpDetailSequence;
                locationTransaction.ReceiptNo = receiptDetail.ReceiptNo;
                locationTransaction.ReceiptDetailId = receiptDetail.Id;
                locationTransaction.ReceiptDetailSequence = receiptDetail.Sequence;
                //locationTransaction.SequenceNo = 
                //locationTransaction.TraceCode =
                locationTransaction.SequenceNo = receiptDetailInput.SequenceNo;
                locationTransaction.Item = receiptDetail.Item;
                locationTransaction.Uom = receiptDetail.Uom;
                locationTransaction.BaseUom = receiptDetail.BaseUom;
                locationTransaction.Qty = groupedInventoryTransaction.Qty / receiptDetail.UnitQty;
                locationTransaction.UnitQty = receiptDetail.UnitQty;
                locationTransaction.IsConsignment = groupedInventoryTransaction.IsConsignment;
                if (groupedInventoryTransaction.IsConsignment && groupedInventoryTransaction.PlanBill.HasValue)
                {
                    locationTransaction.PlanBill = groupedInventoryTransaction.PlanBill.Value;
                }
                locationTransaction.PlanBillQty = groupedInventoryTransaction.PlanBillQty / receiptDetail.UnitQty;
                if (groupedInventoryTransaction.ActingBill.HasValue)
                {
                    locationTransaction.ActingBill = groupedInventoryTransaction.ActingBill.Value;
                }
                locationTransaction.ActingBillQty = groupedInventoryTransaction.ActingBillQty / receiptDetail.UnitQty;
                locationTransaction.QualityType = receiptDetail.QualityType;
                locationTransaction.HuId = receiptDetailInput.HuId;
                locationTransaction.LotNo = receiptDetailInput.LotNo;
                locationTransaction.TransactionType = transType;
                locationTransaction.IOType = CodeMaster.TransactionIOType.In;
                locationTransaction.PartyFrom = receiptDetail.CurrentPartyFrom;
                locationTransaction.PartyTo = receiptDetail.CurrentPartyTo;
                locationTransaction.LocationFrom = receiptDetail.LocationFrom;
                locationTransaction.LocationTo = receiptDetail.LocationTo;
                locationTransaction.LocationIOReason = string.Empty;
                locationTransaction.EffectiveDate = effectiveDate;
                locationTransaction.CreateUserId = SecurityContextHolder.Get().Id;
                locationTransaction.CreateDate = dateTimeNow;

                this.genericMgr.Create(locationTransaction);

                RecordLocationTransactionDetail(locationTransaction, groupedInventoryTransaction.InventoryTransactionList);
            }
        }

        #endregion

        #region 生产线投料
        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> FeedProductRawMaterial(FeedInput feedInput)
        {
            return FeedProductRawMaterial(feedInput, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> FeedProductRawMaterial(FeedInput feedInput, DateTime effectiveDate)
        {
            #region 出库位
            List<InventoryTransaction> issInventoryTransactionList = new List<InventoryTransaction>();
            InventoryIO inventoryIO = new InventoryIO();

            inventoryIO.Location = feedInput.LocationFrom;
            inventoryIO.Item = feedInput.Item;
            inventoryIO.HuId = feedInput.HuId;
            inventoryIO.Qty = -feedInput.Qty * feedInput.UnitQty;    //出库为负数，库存单位
            inventoryIO.LotNo = feedInput.LotNo;
            inventoryIO.QualityType = feedInput.QualityType;
            inventoryIO.IsATP = feedInput.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
            inventoryIO.IsFreeze = false;                       //可能指定移库冻结的零件？
            //inventoryIO.IsCreatePlanBill = ipDetailInput.IsCreatePlanBill;
            //inventoryIO.IsConsignment = ipDetailInput.IsConsignment;
            //inventoryIO.PlanBill = ipDetailInput.PlanBill;
            //inventoryIO.ActingBill = ipDetailInput.ActingBill;
            inventoryIO.TransactionType = com.Sconit.CodeMaster.TransactionType.ISS_MIN; //生产投料出库
            inventoryIO.OccupyType = com.Sconit.CodeMaster.OccupyType.None; //应该是非占用的零件才能投料
            //inventoryIO.OccupyReferenceNo = ipDetail.CurrentOccupyReferenceNo;
            inventoryIO.IsVoid = false;
            inventoryIO.EffectiveDate = effectiveDate;
            //inventoryIO.ManufactureParty = ;

            issInventoryTransactionList.AddRange(RecordInventory(inventoryIO));
            //记录出库事务
            RecordLocationTransaction(feedInput, effectiveDate, issInventoryTransactionList, true);
            #endregion

            #region 入生产线物料 by 李秋云 不记入库
            //var groupedInventoryTransactionList = from trans in issInventoryTransactionList
            //                                      group trans by new
            //                                      {
            //                                          IsConsignment = trans.IsConsignment,
            //                                          PlanBill = trans.PlanBill
            //                                      } into result
            //                                      select new
            //                                      {
            //                                          IsConsignment = result.Key.IsConsignment,
            //                                          PlanBill = result.Key.PlanBill,
            //                                          Qty = result.Sum(trans => -trans.Qty)
            //                                      };

            //IList<ProductLineLocationDetail> productLineLocationDetailList = new List<ProductLineLocationDetail>();
            //foreach (var trans in groupedInventoryTransactionList)
            //{
            //    ProductLineLocationDetail productLineLocationDetail = new ProductLineLocationDetail();

            //    productLineLocationDetail.ProductLine = feedInput.ProductLine;
            //    productLineLocationDetail.ProductLineFacility = feedInput.ProductLineFacility;
            //    productLineLocationDetail.OrderNo = feedInput.OrderNo;
            //    productLineLocationDetail.OrderDetailId = feedInput.OrderDetailId;
            //    productLineLocationDetail.TraceCode = feedInput.TraceCode;
            //    productLineLocationDetail.Operation = feedInput.Operation;
            //    productLineLocationDetail.OpReference = feedInput.OpReference;
            //    productLineLocationDetail.Item = feedInput.Item;
            //    productLineLocationDetail.ItemDescription = feedInput.CurrentItem.Description;
            //    productLineLocationDetail.ReferenceItemCode = feedInput.CurrentItem.ReferenceCode;
            //    productLineLocationDetail.HuId = feedInput.HuId;
            //    productLineLocationDetail.LotNo = feedInput.LotNo;
            //    productLineLocationDetail.IsConsignment = trans.IsConsignment;
            //    productLineLocationDetail.PlanBill = trans.PlanBill;
            //    productLineLocationDetail.Qty = trans.Qty; //库存单位
            //    productLineLocationDetail.BackFlushQty = 0;
            //    productLineLocationDetail.VoidQty = 0;
            //    productLineLocationDetail.LocationFrom = feedInput.LocationFrom;
            //    productLineLocationDetail.ReserveNo = feedInput.ReserveNo;       //预留号
            //    productLineLocationDetail.ReserveLine = feedInput.ReserveLine;   //行号
            //    productLineLocationDetail.AUFNR = feedInput.AUFNR;               //生产单号
            //    productLineLocationDetail.ICHARG = feedInput.ICHARG;             //生产批次
            //    productLineLocationDetail.BWART = feedInput.BWART;               //移动类型
            //    productLineLocationDetail.NotReport = feedInput.NotReport;       //不导出给SAP

            //    this.genericMgr.Create(productLineLocationDetail);

            //    productLineLocationDetailList.Add(productLineLocationDetail);
            //    //InventoryTransaction rctInventoryTransaction = Mapper.Map<InventoryTransaction, InventoryTransaction>(trans);
            //    //rctInventoryTransaction.ActingBillQty *= -1;
            //    //rctInventoryTransaction.Qty *= -1;

            //    //rctInventoryTransactionList.Add(rctInventoryTransaction);
            //}

            //List<InventoryTransaction> rctInventoryTransactionList = (from det in productLineLocationDetailList
            //                                                          select new InventoryTransaction
            //                                                          {
            //                                                              LocationLotDetailId = det.Id,
            //                                                              Location = det.ProductLine,
            //                                                              Bin = det.ProductLineFacility,
            //                                                              Item = det.Item,
            //                                                              HuId = det.HuId,
            //                                                              LotNo = det.LotNo,
            //                                                              Qty = det.Qty,
            //                                                              IsConsignment = det.IsConsignment,
            //                                                              PlanBill = det.PlanBill,
            //                                                              PlanBillQty = det.IsConsignment ? det.Qty : 0,
            //                                                              QualityType = det.QualityType,
            //                                                              IsATP = det.QualityType == com.Sconit.CodeMaster.QualityType.Qualified
            //                                                          }).ToList();
            ////记录入库事务 
            //RecordLocationTransaction(feedInput, effectiveDate, rctInventoryTransactionList, false);
            //issInventoryTransactionList.AddRange(rctInventoryTransactionList);
            #endregion

            return issInventoryTransactionList;
        }

        private void RecordLocationTransaction(FeedInput feedInput, DateTime effectiveDate, IList<InventoryTransaction> inventoryTransactionList, bool isIssue)  //isIssue区分是投料出库还是投料入生产线
        {

            DateTime dateTimeNow = DateTime.Now;

            //根据PlanBill和ActingBill分组，为了不同供应商的库存事务分开
            var groupedInventoryTransactionList = from trans in inventoryTransactionList
                                                  group trans by new
                                                  {
                                                      IsConsignment = trans.IsConsignment,
                                                      PlanBill = trans.PlanBill,
                                                      ActingBill = trans.ActingBill
                                                  }
                                                      into result
                                                      select new
                                                      {
                                                          IsConsignment = result.Key.IsConsignment,
                                                          PlanBill = result.Key.PlanBill,
                                                          ActingBill = result.Key.ActingBill,
                                                          //Qty = result.Sum(trans => isIssue ? -trans.Qty : trans.Qty),
                                                          Qty = result.Sum(trans => trans.Qty),
                                                          //PlanBillQty = result.Sum(trans => isIssue ? -trans.PlanBillQty : trans.PlanBillQty),
                                                          PlanBillQty = result.Sum(trans => trans.PlanBillQty),
                                                          //ActingBillQty = result.Sum(trans => isIssue ? -trans.ActingBillQty : trans.ActingBillQty),
                                                          ActingBillQty = result.Sum(trans => trans.ActingBillQty),
                                                          InventoryTransactionList = result.ToList()
                                                      };

            foreach (var groupedInventoryTransaction in groupedInventoryTransactionList)
            {
                LocationTransaction locationTransaction = new LocationTransaction();
                if (feedInput.OrderNo != null)
                {
                    locationTransaction.OrderNo = feedInput.OrderNo;
                    locationTransaction.OrderType = feedInput.OrderType;
                    locationTransaction.OrderSubType = feedInput.OrderSubType;
                    //locationTransaction.OrderDetailSequence =
                    //locationTransaction.OrderDetailId =
                    //locationTransaction.OrderBomDetId = 
                }
                //locationTransaction.IpNo = 
                //locationTransaction.IpDetailId = 
                //locationTransaction.IpDetailSequence = 
                //locationTransaction.ReceiptNo = 
                //locationTransaction.ReceiptDetailId = 
                //locationTransaction.ReceiptDetailSequence = 
                //locationTransaction.SequenceNo = 
                locationTransaction.TraceCode = feedInput.TraceCode;
                locationTransaction.Item = feedInput.Item;
                locationTransaction.Uom = feedInput.Uom;
                locationTransaction.BaseUom = feedInput.BaseUom;
                locationTransaction.Qty = groupedInventoryTransaction.Qty / feedInput.UnitQty;//isIssue ? -feedInput.Qty : feedInput.Qty;
                locationTransaction.UnitQty = feedInput.UnitQty;
                locationTransaction.IsConsignment = groupedInventoryTransaction.IsConsignment;//isIssue ? -feedInput.Qty : feedInput.Qty;
                if (groupedInventoryTransaction.IsConsignment && groupedInventoryTransaction.PlanBill.HasValue)
                {
                    locationTransaction.PlanBill = groupedInventoryTransaction.PlanBill.Value;//inventoryTransactionList.Sum(i => (isIssue ? -i.PlanBillQty : i.PlanBillQty) / feedInput.UnitQty);
                }
                locationTransaction.PlanBillQty = groupedInventoryTransaction.PlanBillQty / feedInput.UnitQty;//inventoryTransactionList.Sum(i => (isIssue ? -i.PlanBillQty : i.PlanBillQty) / feedInput.UnitQty);
                if (groupedInventoryTransaction.ActingBill.HasValue)
                {
                    locationTransaction.ActingBill = groupedInventoryTransaction.ActingBill.Value;//inventoryTransactionList.Sum(i => (isIssue ? -i.PlanBillQty : i.PlanBillQty) / feedInput.UnitQty);
                }
                locationTransaction.ActingBillQty = groupedInventoryTransaction.ActingBillQty / feedInput.UnitQty; //inventoryTransactionList.Sum(i => (isIssue ? -i.ActingBillQty : i.ActingBillQty) / feedInput.UnitQty);
                locationTransaction.QualityType = feedInput.QualityType;
                locationTransaction.HuId = feedInput.HuId;
                locationTransaction.LotNo = feedInput.LotNo;
                locationTransaction.TransactionType = isIssue ? com.Sconit.CodeMaster.TransactionType.ISS_WO : com.Sconit.CodeMaster.TransactionType.RCT_MIN;
                locationTransaction.IOType = isIssue ? CodeMaster.TransactionIOType.Out : CodeMaster.TransactionIOType.In;
                locationTransaction.PartyFrom = feedInput.CurrentLocationFrom.Region;
                locationTransaction.PartyTo = feedInput.CurrentProductLine.PartyFrom;
                locationTransaction.LocationFrom = feedInput.LocationFrom;
                //locationTransaction.LocationTo = feedInput.ProductLine;  //记录投料入的生产线
                locationTransaction.LocationIOReason = string.Empty;
                locationTransaction.EffectiveDate = effectiveDate;
                locationTransaction.CreateUserId = SecurityContextHolder.Get().Id;
                locationTransaction.CreateDate = dateTimeNow;

                this.genericMgr.Create(locationTransaction);

                RecordLocationTransactionDetail(locationTransaction, groupedInventoryTransaction.InventoryTransactionList);
            }
        }
        #endregion

        #region 生产线退料
        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> ReturnProductRawMaterial(ReturnInput returnInput)
        {
            return ReturnProductRawMaterial(returnInput, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> ReturnProductRawMaterial(ReturnInput returnInput, DateTime effectiveDate)
        {
            #region 出生产线
            #region 查询生产线上物料
            IList<ProductLineLocationDetail> productLineLocationDetailList = null;
            if (returnInput.ProductLineLocationDetailId.HasValue)
            {
                string hql = "from ProductLineLocationDetail where Id = ? and IsClose = ? ";

                IList<object> para = new List<object>();
                para.Add(returnInput.ProductLineLocationDetailId.Value);
                para.Add(false);

                productLineLocationDetailList = this.genericMgr.FindAll<ProductLineLocationDetail>(hql, para.ToArray());

                if (productLineLocationDetailList != null && productLineLocationDetailList.Count > 0)
                {
                    OrderMaster orderMaster = this.genericMgr.FindById<OrderMaster>(productLineLocationDetailList[0].OrderNo);
                    FlowMaster flowMaster = this.genericMgr.FindById<FlowMaster>(productLineLocationDetailList[0].ProductLine);
                    Location location = this.genericMgr.FindById<Location>(productLineLocationDetailList[0].LocationFrom);
                    Item item = this.genericMgr.FindById<Item>(productLineLocationDetailList[0].Item);
                    returnInput.OrderNo = orderMaster.OrderNo;
                    returnInput.OrderType = orderMaster.Type;
                    returnInput.OrderSubType = orderMaster.SubType;
                    returnInput.TraceCode = productLineLocationDetailList[0].TraceCode;
                    returnInput.Item = productLineLocationDetailList[0].Item;
                    returnInput.Uom = item.Uom;
                    returnInput.BaseUom = item.Uom;
                    returnInput.UnitQty = 1;
                    returnInput.QualityType = productLineLocationDetailList[0].QualityType;
                    returnInput.HuId = productLineLocationDetailList[0].HuId;
                    returnInput.LotNo = productLineLocationDetailList[0].LotNo;
                    returnInput.CurrentProductLine = flowMaster;
                    returnInput.ProductLine = flowMaster.Code;
                    returnInput.CurrentLocationTo = location;
                }
            }
            else
            {
                string hql = "from ProductLineLocationDetail where ProductLine = ? and Item = ? and QualityType = ? and IsClose = ? ";
                IList<object> para = new List<object>();
                para.Add(returnInput.ProductLine);
                para.Add(returnInput.Item);
                para.Add(returnInput.QualityType);
                para.Add(false);

                if (!string.IsNullOrWhiteSpace(returnInput.ProductLineFacility))
                {
                    hql += " and ProductLineFacility = ?";
                    para.Add(returnInput.ProductLineFacility);
                }

                if (!string.IsNullOrWhiteSpace(returnInput.OrderNo))
                {
                    hql += " and OrderNo = ?";
                    para.Add(returnInput.OrderNo);

                    if (!string.IsNullOrWhiteSpace(returnInput.TraceCode))
                    {
                        hql += " and TraceCode = ?";
                        para.Add(returnInput.TraceCode);
                    }

                    if (returnInput.Operation.HasValue)
                    {
                        hql += " and Operation = ?";
                        para.Add(returnInput.Operation.Value);

                        if (!string.IsNullOrWhiteSpace(returnInput.OpReference))
                        {
                            hql += " and OpReference = ?";
                            para.Add(returnInput.OpReference);
                        }
                        else
                        {
                            //hql += " and OpReference is null";
                        }
                    }
                    else
                    {
                        //hql += " and Operation is null";
                    }
                }

                if (!string.IsNullOrWhiteSpace(returnInput.HuId))
                {
                    hql += " and HuId = ?";
                    para.Add(returnInput.HuId);
                }

                //hql += " order by CreateDate desc";  //
                productLineLocationDetailList = this.genericMgr.FindAll<ProductLineLocationDetail>(hql, para.ToArray());
            }
            #endregion

            #region 回冲生产线物料
            List<InventoryTransaction> issInventoryTransactionList = new List<InventoryTransaction>();
            decimal remainQty = returnInput.Qty * returnInput.UnitQty;  //转为库存单位
            if (productLineLocationDetailList != null && productLineLocationDetailList.Count > 0)
            {
                #region 循环扣减物料
                foreach (ProductLineLocationDetail productLineLocationDetail in productLineLocationDetailList)
                {
                    if (remainQty == 0)
                    {
                        break;
                    }

                    if (productLineLocationDetail.RemainBackFlushQty == 0)
                    {
                        continue;
                    }

                    InventoryTransaction inventoryTransaction = new InventoryTransaction();

                    if (productLineLocationDetail.RemainBackFlushQty >= remainQty)
                    {
                        productLineLocationDetail.VoidQty += remainQty;
                        inventoryTransaction.Qty = -remainQty;   //出库位负数
                        remainQty = 0;
                        if (productLineLocationDetail.RemainBackFlushQty == 0)
                        {
                            productLineLocationDetail.IsClose = true;
                        }
                    }
                    else
                    {
                        remainQty -= productLineLocationDetail.RemainBackFlushQty;
                        inventoryTransaction.Qty = -productLineLocationDetail.RemainBackFlushQty;   //出库位负数
                        productLineLocationDetail.VoidQty += productLineLocationDetail.RemainBackFlushQty;
                        productLineLocationDetail.IsClose = true;
                    }
                    inventoryTransaction.LocationLotDetailId = productLineLocationDetail.Id;
                    inventoryTransaction.Location = productLineLocationDetail.ProductLine;
                    inventoryTransaction.OrgLocation = productLineLocationDetail.LocationFrom;
                    inventoryTransaction.Bin = productLineLocationDetail.ProductLineFacility;
                    inventoryTransaction.Item = productLineLocationDetail.Item;
                    inventoryTransaction.HuId = productLineLocationDetail.HuId;
                    inventoryTransaction.LotNo = productLineLocationDetail.LotNo;
                    inventoryTransaction.IsCreatePlanBill = false;
                    inventoryTransaction.IsConsignment = productLineLocationDetail.IsConsignment;
                    inventoryTransaction.PlanBill = productLineLocationDetail.PlanBill;
                    if (inventoryTransaction.IsConsignment)
                    {
                        inventoryTransaction.PlanBillQty = inventoryTransaction.Qty;
                    }
                    inventoryTransaction.ActingBill = null;
                    inventoryTransaction.ActingBillQty = 0;
                    inventoryTransaction.QualityType = productLineLocationDetail.QualityType;
                    inventoryTransaction.IsATP = productLineLocationDetail.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                    inventoryTransaction.IsFreeze = false;
                    inventoryTransaction.OccupyType = com.Sconit.CodeMaster.OccupyType.None;

                    issInventoryTransactionList.Add(inventoryTransaction);
                    this.genericMgr.Update(productLineLocationDetail);
                }
                #endregion
            }

            if (remainQty > 0)
            {
                if (!string.IsNullOrWhiteSpace(returnInput.HuId))
                {
                    throw new BusinessException("退料条码{0}在生产线{1}上不存在或余额不足。", returnInput.HuId, returnInput.ProductLine);
                }
                else
                {
                    throw new BusinessException("退料零件{0}在生产线{1}上余额不足。", returnInput.Item, returnInput.ProductLine);

                }
            }
            #endregion

            #region 记录退生产线事务
            if (issInventoryTransactionList.Count == 0)
            {
                throw new TechnicalException("Return InventoryTransaction is empty.");
            }

            RecordLocationTransaction(returnInput, effectiveDate, issInventoryTransactionList, true);
            #endregion
            #endregion

            #region 入库位
            var groupedInventoryTransactionList = from trans in issInventoryTransactionList
                                                  group trans by new
                                                  {
                                                      LocationTo = !string.IsNullOrWhiteSpace(returnInput.LocationTo) ? returnInput.LocationTo : trans.OrgLocation,
                                                      IsConsignment = trans.IsConsignment,
                                                      PlanBill = trans.PlanBill
                                                  } into result
                                                  select new
                                                  {
                                                      LocationTo = result.Key.LocationTo,
                                                      IsConsignment = result.Key.IsConsignment,
                                                      PlanBill = result.Key.PlanBill,
                                                      //Qty = result.Sum(trans => -trans.Qty),
                                                      Qty = result.Sum(trans => trans.Qty),
                                                  };

            List<InventoryTransaction> rctInventoryTransactionList = new List<InventoryTransaction>();
            foreach (var trans in groupedInventoryTransactionList)
            {
                InventoryIO inventoryIO = new InventoryIO();

                inventoryIO.Location = trans.LocationTo;
                inventoryIO.Item = returnInput.Item;
                inventoryIO.HuId = returnInput.HuId;
                inventoryIO.LotNo = returnInput.LotNo;
                inventoryIO.Qty = -trans.Qty;    //入库为正数
                inventoryIO.QualityType = returnInput.QualityType;
                inventoryIO.IsATP = returnInput.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                inventoryIO.IsFreeze = false;
                inventoryIO.IsCreatePlanBill = false;
                inventoryIO.IsConsignment = trans.IsConsignment;
                inventoryIO.PlanBill = trans.PlanBill;
                inventoryIO.ActingBill = null;
                inventoryIO.TransactionType = com.Sconit.CodeMaster.TransactionType.RCT_MIN_RTN; //生产投料退库入库
                inventoryIO.OccupyType = com.Sconit.CodeMaster.OccupyType.None; //应该是非占用的零件才能投料
                //inventoryIO.OccupyReferenceNo = ipDetail.CurrentOccupyReferenceNo;
                inventoryIO.IsVoid = false;
                inventoryIO.EffectiveDate = effectiveDate;
                //inventoryIO.ManufactureParty = ;

                rctInventoryTransactionList.AddRange(RecordInventory(inventoryIO));
            }
            //记录出库事务
            RecordLocationTransaction(returnInput, effectiveDate, rctInventoryTransactionList, false);
            #endregion

            issInventoryTransactionList.AddRange(rctInventoryTransactionList);

            return issInventoryTransactionList;
        }

        private void RecordLocationTransaction(ReturnInput returnInput, DateTime effectiveDate, IList<InventoryTransaction> inventoryTransactionList, bool isIssue)  //isIssue区分是退料出生产线还是退料入库
        {
            var groupedInventoryTransactionList = from trans in inventoryTransactionList
                                                  group trans by new
                                                  {
                                                      //为了能够退料回原投料库位，要对LocationTo分组，如果指定了退料的出库库位则按照出库库位分组。
                                                      LocationTo = !string.IsNullOrWhiteSpace(returnInput.LocationTo) ? returnInput.LocationTo : (isIssue ? trans.OrgLocation : trans.Location),
                                                      IsConsignment = trans.IsConsignment,
                                                      PlanBill = trans.PlanBill,
                                                      ActingBill = trans.ActingBill
                                                  } into result
                                                  select new
                                                  {
                                                      LocationTo = result.Key.LocationTo,
                                                      IsConsignment = result.Key.IsConsignment,
                                                      PlanBill = result.Key.PlanBill,
                                                      ActingBill = result.Key.ActingBill,
                                                      //Qty = result.Sum(trans => isIssue ? -trans.Qty : trans.Qty),
                                                      Qty = result.Sum(trans => trans.Qty),
                                                      //PlanBillQty = result.Sum(trans => isIssue ? -trans.PlanBillQty : trans.PlanBillQty),
                                                      PlanBillQty = result.Sum(trans => trans.PlanBillQty),
                                                      //ActingBillQty = result.Sum(trans => isIssue ? -trans.ActingBillQty : trans.ActingBillQty),
                                                      ActingBillQty = result.Sum(trans => trans.ActingBillQty),
                                                      InventoryTransactionList = result.ToList()
                                                  };


            DateTime dateTimeNow = DateTime.Now;

            foreach (var trans in groupedInventoryTransactionList)
            {
                LocationTransaction locationTransaction = new LocationTransaction();
                if (returnInput.OrderNo != null)
                {
                    locationTransaction.OrderNo = returnInput.OrderNo;
                    locationTransaction.OrderType = returnInput.OrderType;
                    locationTransaction.OrderSubType = returnInput.OrderSubType;
                    //locationTransaction.OrderDetailSequence =
                    //locationTransaction.OrderDetailId =
                    //locationTransaction.OrderBomDetId = 
                }
                //locationTransaction.IpNo = 
                //locationTransaction.IpDetailId = 
                //locationTransaction.IpDetailSequence = 
                //locationTransaction.ReceiptNo = 
                //locationTransaction.ReceiptDetailId = 
                //locationTransaction.ReceiptDetailSequence = 
                //locationTransaction.SequenceNo = 
                locationTransaction.TraceCode = returnInput.TraceCode;
                locationTransaction.Item = returnInput.Item;
                locationTransaction.Uom = returnInput.Uom;
                locationTransaction.BaseUom = returnInput.BaseUom;
                locationTransaction.Qty = trans.Qty / returnInput.UnitQty;
                locationTransaction.UnitQty = returnInput.UnitQty;
                locationTransaction.IsConsignment = trans.IsConsignment;
                if (trans.IsConsignment && trans.PlanBill.HasValue)
                {
                    locationTransaction.PlanBill = trans.PlanBill.Value;
                }
                locationTransaction.PlanBillQty = trans.PlanBillQty / returnInput.UnitQty;
                if (trans.ActingBill.HasValue)
                {
                    locationTransaction.ActingBill = trans.ActingBill.Value;
                }
                locationTransaction.ActingBillQty = trans.ActingBillQty / returnInput.UnitQty;
                locationTransaction.QualityType = returnInput.QualityType;
                locationTransaction.HuId = returnInput.HuId;
                locationTransaction.LotNo = returnInput.LotNo;
                locationTransaction.TransactionType = isIssue ? com.Sconit.CodeMaster.TransactionType.ISS_MIN_RTN : com.Sconit.CodeMaster.TransactionType.RCT_MIN_RTN;
                locationTransaction.IOType = isIssue ? com.Sconit.CodeMaster.TransactionIOType.Out : com.Sconit.CodeMaster.TransactionIOType.In;
                locationTransaction.PartyFrom = returnInput.CurrentProductLine.PartyFrom;
                //如果指定了退料库位，直接取缓存的退料区域。如果没有指定，则查找原来投料的库位区域
                locationTransaction.PartyTo = !string.IsNullOrWhiteSpace(returnInput.LocationTo) ? returnInput.CurrentLocationTo.Region : this.genericMgr.FindById<Location>(trans.LocationTo).Region;
                locationTransaction.LocationFrom = returnInput.ProductLine;  //记录投料入的生产线
                locationTransaction.LocationTo = trans.LocationTo;
                locationTransaction.LocationIOReason = string.Empty;
                locationTransaction.EffectiveDate = effectiveDate;
                locationTransaction.CreateUserId = SecurityContextHolder.Get().Id;
                locationTransaction.CreateDate = dateTimeNow;

                this.genericMgr.Create(locationTransaction);

                RecordLocationTransactionDetail(locationTransaction, trans.InventoryTransactionList);
            }
        }
        #endregion

        #region 加权平均回冲生产线物料
        public IList<InventoryTransaction> BackflushProductWeightAverageRawMaterial(IList<WeightAverageBackflushInput> backflushInputList)
        {
            return BackflushProductWeightAverageRawMaterial(backflushInputList, DateTime.Now);
        }

        public IList<InventoryTransaction> BackflushProductWeightAverageRawMaterial(IList<WeightAverageBackflushInput> backflushInputList, DateTime effectiveDate)
        {
            var productLine = (from input in backflushInputList
                               group input by new { ProductLine = input.ProductLine, ProductLineFacility = input.ProductLineFacility } into result
                               select result.Key).Single();

            #region 查询生产线投料明细和待回冲明细
            string selectProductLineLocationDetailStatement = "from ProductLineLocationDetail where ProductLine = ? and IsClose = False";
            string selectPlanBackflushStatement = "from PlanBackflush where ProductLine = ? and IsClose = False";

            IList<object> para = new List<object>();
            para.Add(productLine.ProductLine);
            if (!string.IsNullOrWhiteSpace(productLine.ProductLineFacility))
            {
                selectProductLineLocationDetailStatement += " and ProductLineFacility = ?";
                selectPlanBackflushStatement += " and ProductLineFacility = ?";
                para.Add(productLine.ProductLineFacility);
            }

            string itemStatement = string.Empty;
            foreach (string item in backflushInputList.Select(f => f.Item).Distinct())
            {
                if (itemStatement == string.Empty)
                {
                    itemStatement = " and Item in(?";
                }
                else
                {
                    itemStatement += ", ?";
                }
                para.Add(item);
            }
            itemStatement += ")";
            selectProductLineLocationDetailStatement += itemStatement;
            selectPlanBackflushStatement += itemStatement;

            IList<ProductLineLocationDetail> productLineLocationDetailList =
                this.genericMgr.FindAll<ProductLineLocationDetail>(selectProductLineLocationDetailStatement, para.ToArray());

            IList<PlanBackflush> planBackflushList =
               this.genericMgr.FindAll<PlanBackflush>(selectPlanBackflushStatement, para.ToArray());
            #endregion

            List<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();
            List<WeightAverageBackflushResult> weightAverageBackflushResultList = new List<WeightAverageBackflushResult>();

            #region 循环回冲零件
            //小数保留位数
            int decimalLength = int.Parse(systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DecimalLength));

            foreach (WeightAverageBackflushInput backflushInput in backflushInputList)
            {
                //对应的待回冲生产单Bom
                IList<PlanBackflush> thisPlanBackflushList = planBackflushList.Where(p => p.Item == backflushInput.Item).ToList();

                if (thisPlanBackflushList == null || thisPlanBackflushList.Count == 0)
                {
                    throw new BusinessException("没有找到和物料{0}对应的待回冲生产单Bom。", backflushInput.Item);
                }

                IList<ProductLineLocationDetail> targetProductLineLocationDetail = (from det in productLineLocationDetailList
                                                                                    where det.Item == backflushInput.Item
                                                                                    select det).ToList();

                decimal remainQty = backflushInput.Qty * backflushInput.UnitQty;  //转为库存单位
                if (targetProductLineLocationDetail != null && targetProductLineLocationDetail.Count > 0)
                {
                    #region 循环扣减物料
                    foreach (ProductLineLocationDetail productLineLocationDetail in targetProductLineLocationDetail)
                    {
                        if (remainQty == 0)
                        {
                            break;
                        }

                        if (productLineLocationDetail.RemainBackFlushQty == 0)
                        {
                            continue;
                        }

                        decimal currentBFQty = 0;
                        if (productLineLocationDetail.RemainBackFlushQty >= remainQty)
                        {
                            currentBFQty = remainQty;
                            productLineLocationDetail.BackFlushQty += currentBFQty;
                            remainQty = 0;
                            if (productLineLocationDetail.RemainBackFlushQty == 0)
                            {
                                productLineLocationDetail.IsClose = true;
                            }
                        }
                        else
                        {
                            currentBFQty = productLineLocationDetail.RemainBackFlushQty;
                            remainQty -= currentBFQty;
                            productLineLocationDetail.BackFlushQty += productLineLocationDetail.RemainBackFlushQty;
                            productLineLocationDetail.IsClose = true;
                        }

                        //剩余分摊数
                        decimal remianBFQty = currentBFQty;
                        decimal remianActingBFQty = 0;

                        #region 判断是否结算
                        SettleBillTransaction billTransaction = null;
                        if (productLineLocationDetail.IsConsignment && productLineLocationDetail.PlanBill.HasValue)
                        {
                            PlanBill pb = this.genericMgr.FindById<PlanBill>(productLineLocationDetail.PlanBill.Value);
                            pb.CurrentActingQty = currentBFQty / pb.UnitQty; //转为结算单位
                            pb.CurrentLocation = productLineLocationDetail.LocationFrom;
                            billTransaction = this.billMgr.SettleBill(pb, effectiveDate);
                            remianActingBFQty = currentBFQty;   //基本单位
                        }
                        #endregion

                        this.genericMgr.Update(productLineLocationDetail);

                        #region 计算每个PlanBackflush的加权平均数
                        //计算分摊因子
                        decimal averageFact = remianBFQty / planBackflushList.Sum(p => p.Qty);
                        decimal actingAverageFact = remianActingBFQty / planBackflushList.Sum(p => p.Qty);

                        for (int i = 0; i < thisPlanBackflushList.Count; i++)
                        {
                            #region 回冲结果
                            PlanBackflush planBackflush = thisPlanBackflushList[i];
                            WeightAverageBackflushResult weightAverageBackflushResult = new WeightAverageBackflushResult();
                            weightAverageBackflushResult.PlanBackflush = planBackflush;
                            weightAverageBackflushResult.ProductLineLocationDetail = productLineLocationDetail;
                            weightAverageBackflushResult.CurrentProductLine = backflushInput.CurrentProductLine;
                            weightAverageBackflushResult.LocationFrom = productLineLocationDetail.LocationFrom;
                            weightAverageBackflushResult.Operation = productLineLocationDetail.Operation;
                            weightAverageBackflushResult.OpReference = productLineLocationDetail.OpReference;
                            weightAverageBackflushResult.ProductLine = productLineLocationDetail.ProductLine;
                            weightAverageBackflushResult.ProductLineFacility = productLineLocationDetail.ProductLineFacility;
                            weightAverageBackflushResult.QualityType = productLineLocationDetail.QualityType;

                            if (i != thisPlanBackflushList.Count - 1)
                            {
                                weightAverageBackflushResult.BaseQty = Math.Round(averageFact * planBackflush.Qty, decimalLength, MidpointRounding.AwayFromZero);  //基本单位
                                weightAverageBackflushResult.BaseActingQty = Math.Round(actingAverageFact * planBackflush.Qty, decimalLength, MidpointRounding.AwayFromZero);   //基本单位

                                remianBFQty -= weightAverageBackflushResult.BaseQty;
                                remianActingBFQty -= weightAverageBackflushResult.BaseActingQty;
                            }
                            else
                            {
                                weightAverageBackflushResult.BaseQty = remianBFQty;
                                weightAverageBackflushResult.BaseActingQty = remianActingBFQty;

                                remianBFQty = 0;
                                remianActingBFQty = 0;
                            }
                            weightAverageBackflushResult.Qty = weightAverageBackflushResult.BaseQty / planBackflush.UnitQty;    //转为回冲的单位
                            weightAverageBackflushResult.ActingQty = weightAverageBackflushResult.BaseActingQty / planBackflush.UnitQty;   //转为回冲的单位

                            #region 库存事务
                            InventoryTransaction inventoryTransaction = new InventoryTransaction();

                            inventoryTransaction.Qty = -weightAverageBackflushResult.BaseQty;   //出库为负数
                            inventoryTransaction.LocationLotDetailId = productLineLocationDetail.Id;
                            inventoryTransaction.Location = productLineLocationDetail.LocationFrom;
                            inventoryTransaction.Item = productLineLocationDetail.Item;
                            inventoryTransaction.HuId = productLineLocationDetail.HuId;
                            inventoryTransaction.LotNo = productLineLocationDetail.LotNo;
                            inventoryTransaction.IsCreatePlanBill = false;
                            inventoryTransaction.IsConsignment = productLineLocationDetail.IsConsignment;
                            inventoryTransaction.PlanBill = productLineLocationDetail.PlanBill;
                            //if (inventoryTransaction.IsConsignment)
                            //{
                            //    
                            //理论上都是非寄售库存，PlanBillQty必定为零。
                            inventoryTransaction.PlanBillQty = 0;
                            //}
                            inventoryTransaction.QualityType = productLineLocationDetail.QualityType;
                            inventoryTransaction.IsATP = productLineLocationDetail.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                            inventoryTransaction.IsFreeze = false;
                            inventoryTransaction.OccupyType = com.Sconit.CodeMaster.OccupyType.None;
                            inventoryTransaction.PlanBackflushId = planBackflush.Id;

                            if (billTransaction != null)
                            {
                                inventoryTransaction.IsConsignment = false;
                                inventoryTransaction.ActingBill = billTransaction.ActingBill;
                                inventoryTransaction.ActingBillQty = -weightAverageBackflushResult.BaseActingQty;  //基本单位，采购结算为负数
                                inventoryTransaction.BillTransactionId = billTransaction.Id;
                            }

                            weightAverageBackflushResult.InventoryTransaction = inventoryTransaction;

                            inventoryTransactionList.Add(inventoryTransaction);
                            #endregion

                            weightAverageBackflushResultList.Add(weightAverageBackflushResult);
                            #endregion
                        }
                        #endregion
                    }
                    #endregion

                    if (remainQty > 0)
                    {
                        throw new BusinessException("零件{0}在生产线{1}上余额不足。", backflushInput.Item, backflushInput.ProductLine);
                    }
                }
            }
            #endregion

            #region 记录工单投料明细
            DateTime dateTimeNow = DateTime.Now;
            User currentUser = SecurityContextHolder.Get();

            foreach (OrderBackflushDetail orderBackflushDetail in (from r in weightAverageBackflushResultList
                                                                   group r by new
                                                                   {
                                                                       PlanBackflush = r.PlanBackflush,
                                                                       LocationFrom = r.LocationFrom,
                                                                       Operation = r.Operation,
                                                                       OpReference = r.OpReference,
                                                                       ProductLine = r.ProductLine,
                                                                       ProductLineFacility = r.ProductLineFacility,
                                                                       QualityType = r.QualityType,
                                                                       InventoryTransaction = r.InventoryTransaction
                                                                   } into result
                                                                   select new OrderBackflushDetail
                                                                   {
                                                                       OrderNo = result.Key.PlanBackflush.OrderNo,
                                                                       OrderDetailId = result.Key.PlanBackflush.OrderDetailId,
                                                                       OrderDetailSequence = result.Key.PlanBackflush.OrderDetailSequence,
                                                                       OrderBomDetailId = result.Key.PlanBackflush.OrderBomDetailId,
                                                                       OrderBomDetailSequence = result.Key.PlanBackflush.OrderBomDetailSequence,
                                                                       ReceiptNo = result.Key.PlanBackflush.ReceiptNo,
                                                                       ReceiptDetailId = result.Key.PlanBackflush.ReceiptDetailId,
                                                                       ReceiptDetailSequence = result.Key.PlanBackflush.ReceiptDetailSequence,
                                                                       Bom = result.Key.PlanBackflush.Bom,
                                                                       FGItem = result.Key.PlanBackflush.FGItem,
                                                                       Item = result.Key.PlanBackflush.Item,
                                                                       ItemDescription = result.Key.PlanBackflush.ItemDescription,
                                                                       ReferenceItemCode = result.Key.PlanBackflush.ReferenceItemCode,
                                                                       Uom = result.Key.PlanBackflush.Uom,
                                                                       BaseUom = result.Key.PlanBackflush.BaseUom,
                                                                       UnitQty = result.Key.PlanBackflush.UnitQty,
                                                                       ManufactureParty = result.Key.PlanBackflush.ManufactureParty,
                                                                       //TraceCode = result.Key.PlanBackflush.TraceCode,
                                                                       //HuId = result.Key.HuId,
                                                                       //LotNo = result.Key.LotNo,
                                                                       Operation = result.Key.Operation,
                                                                       OpReference = result.Key.OpReference,
                                                                       BackflushedQty = result.Key.QualityType == CodeMaster.QualityType.Qualified ? result.Sum(p => p.Qty) : 0,
                                                                       BackflushedRejectQty = result.Key.QualityType == CodeMaster.QualityType.Reject ? result.Sum(p => p.Qty) : 0,
                                                                       //BackflushedScrapQty = input.BackflushedQty,
                                                                       LocationFrom = result.Key.LocationFrom,
                                                                       ProductLine = result.Key.ProductLine,
                                                                       ProductLineFacility = result.Key.ProductLineFacility,
                                                                       ReserveNo = result.Key.PlanBackflush.ReserveNo,
                                                                       ReserveLine = result.Key.PlanBackflush.ReserveLine,
                                                                       AUFNR = result.Key.PlanBackflush.AUFNR,
                                                                       PlanBill = result.Key.InventoryTransaction != null ? result.Key.InventoryTransaction.PlanBill : null,
                                                                       ICHARG = result.Key.PlanBackflush.ICHARG,
                                                                       BWART = result.Key.PlanBackflush.BWART,
                                                                       NotReport = false,   //理论都需要汇报
                                                                       EffectiveDate = effectiveDate,
                                                                       CreateUserId = currentUser.Id,
                                                                       CreateUserName = currentUser.FullName,
                                                                       CreateDate = dateTimeNow,
                                                                       IsVoid = false,
                                                                   }))
            {
                this.genericMgr.Create(orderBackflushDetail);
            }

            #endregion

            #region 记录库存事务
            RecordLocationTransaction(weightAverageBackflushResultList, effectiveDate);
            #endregion

            return inventoryTransactionList;
        }

        private void RecordLocationTransaction(IList<WeightAverageBackflushResult> weightAverageBackflushResultList, DateTime effectiveDate)
        {
            DateTime dateTimeNow = DateTime.Now;
            foreach (var trans in from w in weightAverageBackflushResultList
                                  group w by new
                                  {
                                      OrderNo = w.PlanBackflush.OrderNo,
                                      OrderType = w.PlanBackflush.OrderType,
                                      OrderSubType = w.PlanBackflush.OrderSubType,
                                      OrderDetailSequence = w.PlanBackflush.OrderDetailSequence,
                                      OrderDetailId = w.PlanBackflush.OrderDetailId,
                                      OrderBomDetailId = w.PlanBackflush.OrderBomDetailId,
                                      OrderBomDetailSequence = w.PlanBackflush.OrderBomDetailSequence,
                                      ReceiptNo = w.PlanBackflush.ReceiptNo,
                                      ReceiptDetailId = w.PlanBackflush.ReceiptDetailId,
                                      ReceiptDetailSequence = w.PlanBackflush.ReceiptDetailSequence,
                                      FGItem = w.PlanBackflush.FGItem,
                                      Item = w.PlanBackflush.Item,
                                      Uom = w.PlanBackflush.Uom,
                                      BaseUom = w.PlanBackflush.BaseUom,
                                      UnitQty = w.PlanBackflush.UnitQty,
                                      ActingBill = w.InventoryTransaction.ActingBill,
                                      QualityType = w.ProductLineLocationDetail.QualityType,
                                      PartyFrom = w.CurrentProductLine.PartyFrom,
                                      PartyTo = w.CurrentProductLine.PartyTo,
                                      LocationFrom = w.ProductLineLocationDetail.ProductLine,
                                  } into result
                                  select new
                                  {
                                      OrderNo = result.Key.OrderNo,
                                      OrderType = result.Key.OrderType,
                                      OrderSubType = result.Key.OrderSubType,
                                      OrderDetailSequence = result.Key.OrderDetailSequence,
                                      OrderDetailId = result.Key.OrderDetailId,
                                      OrderBomDetailId = result.Key.OrderBomDetailId,
                                      OrderBomDetailSequence = result.Key.OrderBomDetailSequence,
                                      ReceiptNo = result.Key.ReceiptNo,
                                      ReceiptDetailId = result.Key.ReceiptDetailId,
                                      ReceiptDetailSequence = result.Key.ReceiptDetailSequence,
                                      FGItem = result.Key.FGItem,
                                      Item = result.Key.Item,
                                      Uom = result.Key.Uom,
                                      BaseUom = result.Key.BaseUom,
                                      UnitQty = result.Key.UnitQty,
                                      Qty = result.Sum(q => q.Qty),   //订单单位
                                      PlanBillQty = 0,                //订单单位
                                      ActingBill = result.Key.ActingBill,
                                      ActingBillQty = result.Sum(q => q.ActingQty),    //订单单位
                                      QualityType = result.Key.QualityType,
                                      PartyFrom = result.Key.PartyFrom,
                                      PartyTo = result.Key.PartyTo,
                                      LocationFrom = result.Key.LocationFrom,
                                      List = result.ToList()
                                  })
            {

                LocationTransaction locationTransaction = new LocationTransaction();

                locationTransaction.OrderNo = trans.OrderNo;
                locationTransaction.OrderType = trans.OrderType;
                locationTransaction.OrderSubType = trans.OrderSubType;
                locationTransaction.OrderDetailSequence = trans.OrderDetailSequence;
                locationTransaction.OrderDetailId = trans.OrderDetailId;
                locationTransaction.OrderBomDetailId = trans.OrderBomDetailId;
                locationTransaction.OrderBomDetailSequence = trans.OrderBomDetailSequence;
                locationTransaction.ReceiptNo = trans.ReceiptNo;
                locationTransaction.ReceiptDetailId = trans.ReceiptDetailId;
                locationTransaction.ReceiptDetailSequence = trans.ReceiptDetailSequence;
                //locationTransaction.SequenceNo = 
                locationTransaction.Item = trans.Item;
                locationTransaction.Uom = trans.Uom;
                locationTransaction.BaseUom = trans.BaseUom;
                locationTransaction.UnitQty = trans.UnitQty;
                locationTransaction.Qty = trans.Qty;
                locationTransaction.IsConsignment = false;
                //locationTransaction.PlanBill = 
                locationTransaction.PlanBillQty = trans.PlanBillQty;
                if (trans.ActingBill.HasValue)
                {
                    locationTransaction.ActingBill = trans.ActingBill.Value;
                }
                locationTransaction.ActingBillQty = trans.ActingBillQty;
                locationTransaction.QualityType = trans.QualityType;
                locationTransaction.PartyFrom = trans.PartyFrom;
                locationTransaction.PartyTo = trans.PartyTo;
                locationTransaction.LocationFrom = trans.LocationFrom;
                locationTransaction.TransactionType = CodeMaster.TransactionType.ISS_WO_BF;
                locationTransaction.IOType = CodeMaster.TransactionIOType.Out;
                locationTransaction.LocationIOReason = string.Empty;
                locationTransaction.EffectiveDate = effectiveDate;
                locationTransaction.CreateUserId = SecurityContextHolder.Get().Id;
                locationTransaction.CreateDate = dateTimeNow;

                this.genericMgr.Create(locationTransaction);

                #region 记录库存事务明细，待验证是否正确
                IList<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();
                foreach (WeightAverageBackflushResult result in trans.List)
                {
                    inventoryTransactionList.Add(result.InventoryTransaction);
                }
                RecordLocationTransactionDetail(locationTransaction, inventoryTransactionList);
                #endregion
            }
        }

        //private void RecordLocationTransaction(WeightAverageBackflushInput backflushInput, DateTime effectiveDate, IList<InventoryTransaction> inventoryTransactionList, IList<PlanBackflush> planBackflushList)
        //{
        //    DateTime dateTimeNow = DateTime.Now;

        //    //小数保留位数
        //    int decimalLength = int.Parse(systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.DecimalLength));

        //    //剩余回冲数量
        //    decimal remainQty = backflushInput.Qty; //正数
        //    //剩余回冲开票数
        //    decimal remainActingQty = inventoryTransactionList.Sum(trans => -trans.ActingBillQty);  //转为正数

        //    #region 为了解决从不同库位投料不同质量状态的零件对库位和质量状态进行分组
        //    var groupedInventoryTransaction = from trans in inventoryTransactionList
        //                                      group trans by new { QualityType = trans.QualityType, Location = trans.Location } into result
        //                                      select new
        //                                      {
        //                                          Location = result.Key.Location,
        //                                          CurrentLocation = this.genericMgr.FindById<Location>(result.Key.Location),
        //                                          QualityType = result.Key.QualityType,
        //                                          Qty = result.Sum(trans => -trans.Qty), //基本单位                                                  
        //                                          ActingQty = result.Sum(trans => -trans.ActingBillQty) //基本单位
        //                                      };

        //    //计算分摊因子
        //    decimal averageFact = backflushInput.Qty / groupedInventoryTransaction.Sum(p => p.Qty);
        //    decimal actingAverageFact = backflushInput.Qty / groupedInventoryTransaction.Sum(p => p.ActingQty);
        //    #endregion

        //    //汇总回冲数，转为基本单位
        //    decimal totalPlanBackflushQty = planBackflushList.Sum(p => p.Qty * p.UnitQty);

        //    int loopCount = 0;
        //    foreach (var trans in groupedInventoryTransaction)
        //    {
        //        decimal thisGroupedRemainQty = 0;
        //        decimal thisGroupedActingRemainQty = 0;

        //        #region 计算本分组需要回冲的数量
        //        if (loopCount != groupedInventoryTransaction.Count())
        //        {
        //            thisGroupedRemainQty = Math.Round(averageFact * trans.Qty, decimalLength, MidpointRounding.AwayFromZero);
        //            remainQty -= thisGroupedRemainQty;

        //            thisGroupedActingRemainQty = Math.Round(actingAverageFact * trans.ActingQty, decimalLength, MidpointRounding.AwayFromZero);
        //            remainActingQty -= thisGroupedActingRemainQty;
        //        }
        //        else
        //        {
        //            thisGroupedRemainQty = remainQty;
        //            thisGroupedActingRemainQty = remainActingQty;
        //        }
        //        #endregion

        //        //计算本组的分摊因子
        //        decimal currAverageFact = thisGroupedRemainQty / planBackflushList.Sum(p => p.Qty * p.UnitQty);  //乘以基本单位，为了解决BOM单位不同的问题
        //        decimal currActingAverageFact = thisGroupedActingRemainQty / planBackflushList.Sum(p => p.Qty * p.UnitQty);

        //        //本组中的InventoryTransaction
        //        IList<InventoryTransaction> currentInventoryTransactionList = inventoryTransactionList.Where(p => p.Location == trans.Location && p.QualityType == trans.QualityType).ToList();
        //        foreach (InventoryTransaction currentInventoryTransaction in currentInventoryTransactionList)
        //        {
        //            currentInventoryTransaction.RemainQty = currentInventoryTransaction.Qty;
        //            currentInventoryTransaction.RemainActingBillQty = currentInventoryTransaction.ActingBillQty;
        //        }

        //        foreach (PlanBackflush planBackflush in planBackflushList)
        //        {
        //            LocationTransaction locationTransaction = new LocationTransaction();

        //            locationTransaction.OrderNo = planBackflush.OrderNo;
        //            locationTransaction.OrderType = planBackflush.OrderType;
        //            locationTransaction.OrderSubType = planBackflush.OrderSubType;
        //            locationTransaction.OrderDetailSequence = planBackflush.OrderDetailSequence;
        //            locationTransaction.OrderDetailId = planBackflush.OrderDetailId;
        //            locationTransaction.OrderBomDetailId = planBackflush.OrderBomDetailId;
        //            locationTransaction.OrderBomDetailSequence = planBackflush.OrderBomDetailSequence;
        //            locationTransaction.ReceiptNo = planBackflush.ReceiptNo;
        //            locationTransaction.ReceiptDetailId = planBackflush.ReceiptDetailId;
        //            //locationTransaction.SequenceNo = 
        //            //locationTransaction.TraceCode =
        //            locationTransaction.ReceiptDetailSequence = planBackflush.ReceiptDetailSequence;
        //            locationTransaction.Item = backflushInput.Item;
        //            locationTransaction.Uom = backflushInput.Uom;
        //            locationTransaction.BaseUom = backflushInput.BaseUom;
        //            locationTransaction.UnitQty = backflushInput.UnitQty;
        //            locationTransaction.QualityType = trans.QualityType;
        //            #region 计算回冲数
        //            if (planBackflushList.IndexOf(planBackflush) != planBackflushList.Count - 1)
        //            {
        //                locationTransaction.Qty = Math.Round(currAverageFact * planBackflush.Qty, decimalLength, MidpointRounding.AwayFromZero) / planBackflush.UnitQty;  //转为回冲的单位
        //                locationTransaction.ActingBillQty = Math.Round(currActingAverageFact * planBackflush.Qty, decimalLength, MidpointRounding.AwayFromZero) / planBackflush.UnitQty;  //转为回冲的单位
        //                thisGroupedRemainQty -= locationTransaction.Qty;
        //                thisGroupedActingRemainQty -= locationTransaction.ActingBillQty;
        //            }
        //            else
        //            {
        //                locationTransaction.Qty = thisGroupedRemainQty;
        //                locationTransaction.ActingBillQty = thisGroupedActingRemainQty;
        //            }
        //            locationTransaction.Qty *= -1;   //转为负数
        //            locationTransaction.ActingBillQty *= -1;   //转为负数
        //            #endregion
        //            locationTransaction.TransactionType = com.Sconit.CodeMaster.TransactionType.ISS_WO_BF;
        //            locationTransaction.PartyFrom = trans.CurrentLocation.Region;
        //            locationTransaction.PartyTo = backflushInput.CurrentProductLine.PartyFrom;
        //            locationTransaction.LocationFrom = trans.Location;  //记录投料入的生产线
        //            locationTransaction.LocationTo = backflushInput.ProductLine;
        //            locationTransaction.LocationIOReason = string.Empty;
        //            locationTransaction.EffectiveDate = effectiveDate;
        //            locationTransaction.CreateUserId = SecurityContextHolder.Get().Id;
        //            locationTransaction.CreateDate = dateTimeNow;

        //            this.genericMgr.Create(locationTransaction);

        //            #region 记录库存明细事务
        //            IList<InventoryTransaction> recordInventoryTransactionList = new List<InventoryTransaction>();
        //            //同一笔InventoryTransaction要按照PlanBackflush的比率拆分
        //            foreach (InventoryTransaction currentInventoryTransaction in currentInventoryTransactionList)
        //            {
        //                InventoryTransaction inventoryTransaction = Mapper.Map<InventoryTransaction, InventoryTransaction>(currentInventoryTransaction);
        //                if (planBackflushList.IndexOf(planBackflush) != planBackflushList.Count - 1)
        //                {
        //                    inventoryTransaction.Qty = Math.Round(currentInventoryTransaction.Qty * planBackflush.Qty / totalPlanBackflushQty / planBackflush.UnitQty);
        //                    inventoryTransaction.ActingBillQty = Math.Round(currentInventoryTransaction.ActingBillQty * planBackflush.Qty / totalPlanBackflushQty / planBackflush.UnitQty);

        //                    currentInventoryTransaction.RemainQty -= inventoryTransaction.Qty;
        //                    currentInventoryTransaction.RemainActingBillQty -= inventoryTransaction.ActingBillQty;
        //                }
        //                else
        //                {
        //                    inventoryTransaction.Qty = currentInventoryTransaction.RemainQty;
        //                    inventoryTransaction.ActingBillQty = currentInventoryTransaction.RemainActingBillQty;
        //                }

        //                recordInventoryTransactionList.Add(inventoryTransaction);
        //            }

        //            RecordLocationTransactionDetail(locationTransaction, recordInventoryTransactionList);
        //            #endregion
        //        }
        //    }
        //}
        #endregion

        #region 回冲投料至生产单的物料
        public IList<InventoryTransaction> BackflushProductMaterial(IList<BackflushInput> backflushInputList)
        {
            return BackflushProductMaterial(backflushInputList, DateTime.Now);
        }

        public IList<InventoryTransaction> BackflushProductMaterial(IList<BackflushInput> backflushInputList, DateTime effectiveDate)
        {
            List<InventoryTransaction> totalInventoryTransactionList = new List<InventoryTransaction>();
            foreach (BackflushInput backflushInput in backflushInputList)
            {
                if (backflushInput.OrderBomDetail != null)
                {
                    #region 指定OrderBomDetail
                    if (backflushInput.OrderBomDetail.BackFlushMethod == CodeMaster.BackFlushMethod.BackFlushOrder)
                    {
                        #region 只回冲在制品
                        IList<InventoryTransaction> thisInventoryTransactionList = BackflushProductLineLocationDetail(backflushInput, effectiveDate);
                        totalInventoryTransactionList.AddRange(thisInventoryTransactionList);
                        RecordLocationTransaction(backflushInput, effectiveDate, thisInventoryTransactionList, false);
                        backflushInput.InventoryTransactionList = thisInventoryTransactionList;
                        #endregion
                    }
                    else if (backflushInput.OrderBomDetail.BackFlushMethod == CodeMaster.BackFlushMethod.GoodsReceive)
                    {
                        #region 收货回冲
                        #region 先冲在制品
                        IList<InventoryTransaction> thisInventoryTransactionList = BackflushProductLineLocationDetail(backflushInput, effectiveDate);
                        #endregion

                        #region 再冲线旁物料
                        decimal backflushedQty = thisInventoryTransactionList.Sum(trans => trans.Qty);  //库存单位
                        decimal remainBackflushQty = backflushInput.Qty * backflushInput.UnitQty - backflushedQty;
                        if (remainBackflushQty != 0) //考虑到投料回冲可能出现负数，这里用!=
                        {
                            InventoryIO inventoryIO = new InventoryIO();

                            inventoryIO.Location = backflushInput.OrderBomDetail.Location;
                            inventoryIO.Item = backflushInput.Item;
                            inventoryIO.HuId = backflushInput.HuId;
                            inventoryIO.LotNo = backflushInput.LotNo;
                            inventoryIO.QualityType = CodeMaster.QualityType.Qualified; //回冲的物料一定是合格品，不能回冲不合格品物料
                            inventoryIO.IsATP = true;
                            inventoryIO.IsFreeze = false;                       //可能指定冻结的零件？
                            //inventoryIO.IsCreatePlanBill = 
                            //inventoryIO.IsConsignment = 
                            //inventoryIO.PlanBill = 
                            //inventoryIO.ActingBill = 
                            if (backflushInput.OrderSubType == CodeMaster.OrderSubType.Return)
                            {
                                inventoryIO.Qty = remainBackflushQty;  //为正数,增加原材料库存
                                if (backflushInput.OrderType == CodeMaster.OrderType.Production)
                                {
                                    inventoryIO.TransactionType = CodeMaster.TransactionType.ISS_WO_RTN;
                                }
                                else if (backflushInput.OrderType == CodeMaster.OrderType.SubContract)
                                {
                                    inventoryIO.TransactionType = CodeMaster.TransactionType.ISS_SWO_RTN;
                                }
                            }
                            else
                            {
                                inventoryIO.Qty = -remainBackflushQty;  //发货为负数
                                if (backflushInput.OrderType == CodeMaster.OrderType.Production)
                                {
                                    inventoryIO.TransactionType = CodeMaster.TransactionType.ISS_WO;
                                }
                                else if (backflushInput.OrderType == CodeMaster.OrderType.SubContract)
                                {
                                    inventoryIO.TransactionType = CodeMaster.TransactionType.ISS_SWO;
                                }
                            }

                            //inventoryIO.OccupyType = 
                            //inventoryIO.OccupyReferenceNo = 
                            inventoryIO.IsVoid = false;
                            inventoryIO.EffectiveDate = effectiveDate;
                            //inventoryIO.ManufactureParty = ;

                            ((List<InventoryTransaction>)thisInventoryTransactionList).AddRange(RecordInventory(inventoryIO));
                        }
                        #endregion

                        RecordLocationTransaction(backflushInput, effectiveDate, thisInventoryTransactionList, false);
                        totalInventoryTransactionList.AddRange(thisInventoryTransactionList);
                        backflushInput.InventoryTransactionList = thisInventoryTransactionList;
                        #endregion
                    }
                    else if (backflushInput.OrderBomDetail.BackFlushMethod == CodeMaster.BackFlushMethod.WeightAverage)
                    {
                        #region 加权平均回冲
                        PlanBackflush planBackflush = new PlanBackflush();

                        planBackflush.ProductLine = backflushInput.ProductLine;
                        planBackflush.ProductLineFacility = backflushInput.ProductLineFacility;
                        planBackflush.OrderNo = backflushInput.OrderNo;
                        planBackflush.OrderType = backflushInput.OrderType;
                        planBackflush.OrderSubType = backflushInput.OrderSubType;
                        planBackflush.OrderDetailSequence = backflushInput.OrderDetailSequence;
                        planBackflush.OrderDetailId = backflushInput.OrderDetailId;
                        planBackflush.OrderBomDetailId = backflushInput.OrderBomDetail.Id;
                        planBackflush.OrderBomDetailSequence = backflushInput.OrderBomDetail.Sequence;
                        planBackflush.ReceiptNo = backflushInput.ReceiptNo;
                        planBackflush.ReceiptDetailId = backflushInput.ReceiptDetailId;
                        planBackflush.ReceiptDetailSequence = backflushInput.ReceiptDetailSequence;
                        planBackflush.Bom = backflushInput.OrderBomDetail.Bom;
                        planBackflush.FGItem = backflushInput.FGItem;
                        planBackflush.Item = backflushInput.Item;
                        planBackflush.ItemDescription = backflushInput.OrderBomDetail.ItemDescription;
                        planBackflush.ReferenceItemCode = backflushInput.OrderBomDetail.ReferenceItemCode;
                        planBackflush.Uom = backflushInput.Uom;
                        planBackflush.BaseUom = backflushInput.BaseUom;
                        planBackflush.UnitQty = backflushInput.UnitQty;
                        planBackflush.ManufactureParty = backflushInput.OrderBomDetail.ManufactureParty;
                        planBackflush.Qty = backflushInput.Qty;
                        planBackflush.IsClose = false;
                        planBackflush.ReserveNo = backflushInput.OrderBomDetail.ReserveNo;
                        planBackflush.ReserveLine = backflushInput.OrderBomDetail.ReserveLine;
                        planBackflush.AUFNR = backflushInput.OrderBomDetail.AUFNR;
                        planBackflush.ICHARG = backflushInput.OrderBomDetail.ICHARG;
                        planBackflush.BWART = backflushInput.OrderBomDetail.BWART;

                        this.genericMgr.Create(planBackflush);
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    #region 未指定OrderBomDetail，回冲所有ProductLineLocationDetail
                    IList<InventoryTransaction> thisInventoryTransactionList = BackflushProductLineLocationDetail(backflushInput, effectiveDate);
                    totalInventoryTransactionList.AddRange(thisInventoryTransactionList);
                    RecordLocationTransaction(backflushInput, effectiveDate, thisInventoryTransactionList, false);
                    backflushInput.InventoryTransactionList = thisInventoryTransactionList;
                    #endregion
                }
            }

            return totalInventoryTransactionList;
        }

        private IList<InventoryTransaction> BackflushProductLineLocationDetail(BackflushInput backflushInput, DateTime effectiveDate)
        {
            IList<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();

            if (backflushInput.ProductLineLocationDetailList != null && backflushInput.ProductLineLocationDetailList.Count > 0)
            {
                decimal remainQty = backflushInput.Qty * backflushInput.UnitQty;
                foreach (ProductLineLocationDetail productLineLocationDetail in backflushInput.ProductLineLocationDetailList)
                {
                    if (productLineLocationDetail.RemainBackFlushQty == 0)
                    {
                        continue;
                    }

                    if (remainQty <= 0)
                    {
                        break;
                    }
                    SettleBillTransaction billTransaction = null;
                    if (productLineLocationDetail.IsConsignment && productLineLocationDetail.PlanBill.HasValue)
                    {
                        var planBill = this.genericMgr.FindById<PlanBill>(productLineLocationDetail.PlanBill.Value);
                        planBill.CurrentLocation = productLineLocationDetail.LocationFrom;
                        billTransaction = this.billMgr.SettleBill(planBill, effectiveDate);
                    }
                    InventoryTransaction inventoryTransaction = new InventoryTransaction();
                    inventoryTransaction.LocationLotDetailId = productLineLocationDetail.Id;
                    inventoryTransaction.Location = productLineLocationDetail.ProductLine;
                    inventoryTransaction.Bin = productLineLocationDetail.ProductLineFacility;
                    inventoryTransaction.Item = productLineLocationDetail.Item;
                    inventoryTransaction.HuId = productLineLocationDetail.HuId;
                    inventoryTransaction.LotNo = productLineLocationDetail.LotNo;
                    if (remainQty >= productLineLocationDetail.RemainBackFlushQty)
                    {
                        remainQty -= productLineLocationDetail.RemainBackFlushQty;
                        inventoryTransaction.Qty = -productLineLocationDetail.RemainBackFlushQty;
                        productLineLocationDetail.BackFlushQty += productLineLocationDetail.RemainBackFlushQty;
                        productLineLocationDetail.IsClose = true;
                    }
                    else
                    {
                        inventoryTransaction.Qty = -remainQty;
                        productLineLocationDetail.BackFlushQty += remainQty;
                        remainQty = 0;
                        if (productLineLocationDetail.RemainBackFlushQty == 0)
                        {
                            productLineLocationDetail.IsClose = true;
                        }
                    }
                    this.genericMgr.Update(productLineLocationDetail);
                    inventoryTransaction.IsCreatePlanBill = false;
                    inventoryTransaction.IsConsignment = false;
                    inventoryTransaction.PlanBill = productLineLocationDetail.PlanBill;
                    inventoryTransaction.PlanBillQty = 0;
                    if (billTransaction != null)
                    {
                        inventoryTransaction.ActingBill = billTransaction.ActingBill;
                        inventoryTransaction.ActingBillQty = billTransaction.BillQty;
                        inventoryTransaction.BillTransactionId = billTransaction.Id;
                    }
                    inventoryTransaction.QualityType = productLineLocationDetail.QualityType;
                    inventoryTransaction.IsFreeze = false;
                    inventoryTransaction.IsATP = productLineLocationDetail.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                    inventoryTransaction.OccupyType = CodeMaster.OccupyType.None;
                    inventoryTransaction.OccupyReferenceNo = null;
                    inventoryTransaction.Operation = productLineLocationDetail.Operation;
                    inventoryTransaction.OpReference = productLineLocationDetail.OpReference;
                    inventoryTransaction.OrgLocation = productLineLocationDetail.LocationFrom;
                    inventoryTransaction.ReserveNo = productLineLocationDetail.ReserveNo;
                    inventoryTransaction.ReserveLine = productLineLocationDetail.ReserveLine;
                    inventoryTransaction.AUFNR = productLineLocationDetail.AUFNR;
                    inventoryTransaction.BWART = productLineLocationDetail.BWART;
                    inventoryTransaction.ICHARG = productLineLocationDetail.ICHARG;
                    inventoryTransaction.NotReport = productLineLocationDetail.NotReport;

                    inventoryTransactionList.Add(inventoryTransaction);
                }
            }
            return inventoryTransactionList;
        }

        private void RecordLocationTransaction(BackflushInput backflushInput, DateTime effectiveDate, IList<InventoryTransaction> inventoryTransactionList, bool isVoid)
        {
            if (inventoryTransactionList != null && inventoryTransactionList.Count > 0)
            {
                var groupedInventoryTransactionList = from trans in inventoryTransactionList
                                                      group trans by new
                                                      {
                                                          QualityType = trans.QualityType,
                                                          HuId = trans.HuId,
                                                          LotNo = trans.LotNo,
                                                          Location = trans.Location,
                                                          ActingBill = trans.ActingBill,
                                                      } into result
                                                      select new
                                                      {
                                                          QualityType = result.Key.QualityType,
                                                          HuId = result.Key.HuId,
                                                          LotNo = result.Key.LotNo,
                                                          Location = result.Key.Location,
                                                          Qty = result.Sum(g => g.Qty),
                                                          ActingBill = result.Key.ActingBill,
                                                          ActingBillQty = result.Sum(g => g.ActingBillQty),
                                                          TransList = result.ToList()
                                                      };

                DateTime dateTimeNow = DateTime.Now;
                foreach (var groupedInventoryTransaction in groupedInventoryTransactionList)
                {
                    LocationTransaction locationTransaction = new LocationTransaction();

                    locationTransaction.OrderNo = backflushInput.OrderNo;
                    locationTransaction.OrderType = backflushInput.OrderType;
                    locationTransaction.OrderSubType = backflushInput.OrderSubType;
                    locationTransaction.OrderDetailSequence = backflushInput.OrderDetailSequence;
                    locationTransaction.OrderDetailId = backflushInput.OrderDetailId;
                    if (backflushInput.OrderBomDetail != null)
                    {
                        locationTransaction.OrderBomDetailId = backflushInput.OrderBomDetail.Id;
                        locationTransaction.OrderBomDetailSequence = backflushInput.OrderBomDetail.Sequence;

                    }
                    //locationTransaction.IpNo = 
                    //locationTransaction.IpDetailId = 
                    //locationTransaction.IpDetailSequence = 
                    locationTransaction.ReceiptNo = backflushInput.ReceiptNo;
                    locationTransaction.ReceiptDetailId = backflushInput.ReceiptDetailId;
                    locationTransaction.ReceiptDetailSequence = backflushInput.ReceiptDetailSequence;
                    //locationTransaction.SequenceNo = 
                    locationTransaction.TraceCode = backflushInput.TraceCode;
                    locationTransaction.Item = backflushInput.Item;
                    locationTransaction.Uom = backflushInput.Uom;
                    locationTransaction.BaseUom = backflushInput.BaseUom;
                    locationTransaction.Qty = groupedInventoryTransaction.Qty / backflushInput.UnitQty;
                    locationTransaction.UnitQty = backflushInput.UnitQty;
                    locationTransaction.IsConsignment = false;
                    //locationTransaction.PlanBill = 
                    locationTransaction.PlanBillQty = 0;
                    if (groupedInventoryTransaction.ActingBill.HasValue)
                    {
                        locationTransaction.ActingBill = groupedInventoryTransaction.ActingBill.Value;
                    }
                    locationTransaction.ActingBillQty = groupedInventoryTransaction.ActingBillQty / backflushInput.UnitQty;
                    locationTransaction.QualityType = groupedInventoryTransaction.QualityType;
                    locationTransaction.HuId = groupedInventoryTransaction.HuId;
                    locationTransaction.LotNo = groupedInventoryTransaction.LotNo;
                    if (!isVoid)
                    {
                        if (backflushInput.OrderType == CodeMaster.OrderType.Production)
                        {
                            if (backflushInput.OrderSubType == CodeMaster.OrderSubType.Return)
                            {
                                locationTransaction.TransactionType = CodeMaster.TransactionType.ISS_WO_RTN;
                            }
                            else
                            {
                                locationTransaction.TransactionType =
                                     backflushInput.CurrentProductLine.Code != groupedInventoryTransaction.Location ? //如果回冲库位和生产线代码不一致，一定是回冲线旁的物料
                                     com.Sconit.CodeMaster.TransactionType.ISS_WO : com.Sconit.CodeMaster.TransactionType.ISS_WO_BF;
                            }
                        }
                        else if (backflushInput.OrderType == CodeMaster.OrderType.SubContract)
                        {
                            if (backflushInput.OrderSubType == CodeMaster.OrderSubType.Return)
                            {
                                locationTransaction.TransactionType = CodeMaster.TransactionType.ISS_SWO_RTN;
                            }
                            else
                            {
                                locationTransaction.TransactionType =
                                    backflushInput.CurrentProductLine.Code != groupedInventoryTransaction.Location ? //如果回冲库位和生产线代码不一致，一定是回冲线旁的物料
                                    com.Sconit.CodeMaster.TransactionType.ISS_SWO : com.Sconit.CodeMaster.TransactionType.ISS_SWO_BF;
                            }
                        }
                    }
                    else
                    {
                        if (backflushInput.OrderSubType == CodeMaster.OrderSubType.Return)
                        {
                            if (backflushInput.OrderType == CodeMaster.OrderType.Production)
                            {
                                locationTransaction.TransactionType = CodeMaster.TransactionType.ISS_WO_RTN_VOID;
                            }
                            else if (backflushInput.OrderType == CodeMaster.OrderType.SubContract)
                            {
                                locationTransaction.TransactionType = CodeMaster.TransactionType.ISS_SWO_RTN_VOID;
                            }
                        }
                        else
                        {
                            if (backflushInput.OrderType == CodeMaster.OrderType.Production)
                            {
                                locationTransaction.TransactionType = CodeMaster.TransactionType.ISS_WO_VOID;
                            }
                            else if (backflushInput.OrderType == CodeMaster.OrderType.SubContract)
                            {
                                locationTransaction.TransactionType = CodeMaster.TransactionType.ISS_SWO_VOID;
                            }
                        }
                    }
                    locationTransaction.IOType = CodeMaster.TransactionIOType.Out;

                    locationTransaction.PartyFrom =
                        backflushInput.CurrentProductLine.Code != groupedInventoryTransaction.Location ? //如果回冲库位和生产线代码不一致，一定是回冲线旁的物料
                        this.genericMgr.FindById<Location>(groupedInventoryTransaction.Location).Region : backflushInput.CurrentProductLine.PartyFrom;
                    locationTransaction.PartyTo = backflushInput.CurrentProductLine.PartyTo;
                    locationTransaction.LocationFrom = groupedInventoryTransaction.Location;  //记录投料入的生产线
                    //locationTransaction.LocationTo = 
                    locationTransaction.LocationIOReason = string.Empty;
                    locationTransaction.EffectiveDate = effectiveDate;
                    locationTransaction.CreateUserId = SecurityContextHolder.Get().Id;
                    locationTransaction.CreateDate = dateTimeNow;
                    locationTransaction.IpNo = backflushInput.IpNo;

                    this.genericMgr.Create(locationTransaction);

                    RecordLocationTransactionDetail(locationTransaction, groupedInventoryTransaction.TransList);
                }
            }
        }
        #endregion

        #region 冲销生产回冲
        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> CancelBackflushProductMaterial(IList<BackflushInput> backflushInputList)
        {
            return CancelBackflushProductMaterial(backflushInputList, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> CancelBackflushProductMaterial(IList<BackflushInput> backflushInputList, DateTime effectiveDate)
        {
            List<InventoryTransaction> totalInventoryTransactionList = new List<InventoryTransaction>();
            foreach (BackflushInput backflushInput in backflushInputList)
            {
                #region 收货回冲
                InventoryIO inventoryIO = new InventoryIO();

                //inventoryIO.Location = backflushInput.OrderBomDetail.Location;
                inventoryIO.Location = backflushInput.Location;
                inventoryIO.Item = backflushInput.Item;
                //inventoryIO.HuId = 
                //inventoryIO.LotNo = 
                inventoryIO.QualityType = CodeMaster.QualityType.Qualified; //回冲的物料一定是合格品，不能回冲不合格品物料
                inventoryIO.IsATP = true;
                inventoryIO.IsFreeze = false;                       //可能指定冻结的零件？
                //inventoryIO.IsCreatePlanBill = 
                //inventoryIO.IsConsignment = 
                //inventoryIO.PlanBill = 
                //inventoryIO.ActingBill = 
                if (backflushInput.OrderSubType == CodeMaster.OrderSubType.Return)
                {
                    //减少原材料库存
                    inventoryIO.Qty = -backflushInput.Qty * backflushInput.UnitQty;
                    if (backflushInput.OrderType == CodeMaster.OrderType.Production)
                    {
                        inventoryIO.TransactionType = CodeMaster.TransactionType.ISS_WO_RTN_VOID;
                    }
                    else if (backflushInput.OrderType == CodeMaster.OrderType.SubContract)
                    {
                        inventoryIO.TransactionType = CodeMaster.TransactionType.ISS_SWO_RTN_VOID;
                    }
                }
                else
                {
                    //增加原材料库存
                    inventoryIO.Qty = backflushInput.Qty * backflushInput.UnitQty;
                    if (backflushInput.OrderType == CodeMaster.OrderType.Production)
                    {
                        inventoryIO.TransactionType = CodeMaster.TransactionType.ISS_WO_VOID;
                    }
                    else if (backflushInput.OrderType == CodeMaster.OrderType.SubContract)
                    {
                        inventoryIO.TransactionType = CodeMaster.TransactionType.ISS_SWO_VOID;
                    }
                }
                //inventoryIO.OccupyType = 
                //inventoryIO.OccupyReferenceNo = 
                inventoryIO.IsVoid = true;
                inventoryIO.EffectiveDate = effectiveDate;
                //inventoryIO.ManufactureParty = ;

                IList<InventoryTransaction> thisInventoryTransactionList = RecordInventory(inventoryIO);
                RecordLocationTransaction(backflushInput, effectiveDate, thisInventoryTransactionList, true);
                totalInventoryTransactionList.AddRange(thisInventoryTransactionList);
                backflushInput.InventoryTransactionList = thisInventoryTransactionList;
                #endregion
            }

            return totalInventoryTransactionList;
        }
        #endregion


        #region 报验
        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> InventoryInspect(InspectMaster inspectMaster)
        {
            return InventoryInspect(inspectMaster, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> InventoryInspect(InspectMaster inspectMaster, DateTime effectiveDate)
        {
            if (inspectMaster.InspectDetails == null || inspectMaster.InspectDetails.Count == 0)
            {
                return null;
            }

            List<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();

            foreach (InspectDetail inspectDetail in inspectMaster.InspectDetails.OrderByDescending(det => det.IsConsignment))
            {
                #region 检验出库
                InventoryIO inventoryOut = new InventoryIO();

                inventoryOut.Location = inspectDetail.LocationFrom;
                inventoryOut.Item = inspectDetail.Item;
                inventoryOut.HuId = inspectDetail.HuId;
                inventoryOut.Qty = -inspectDetail.InspectQty * inspectDetail.UnitQty;  //转换为库存单位，为负数
                inventoryOut.LotNo = inspectDetail.LotNo;
                //inventoryOut.ManufactureParty = null;
                inventoryOut.QualityType = com.Sconit.CodeMaster.QualityType.Qualified; //报验一定是合格品
                inventoryOut.IsATP = true;
                inventoryOut.IsFreeze = false;
                inventoryOut.IsCreatePlanBill = false;
                inventoryOut.IsConsignment = inspectDetail.IsConsignment;
                inventoryOut.PlanBill = inspectDetail.PlanBill;
                inventoryOut.ActingBill = null;
                inventoryOut.TransactionType = inspectMaster.IsATP ? CodeMaster.TransactionType.ISS_INP : CodeMaster.TransactionType.ISS_ISL;
                inventoryOut.OccupyType = CodeMaster.OccupyType.None;
                inventoryOut.OccupyReferenceNo = null;
                inventoryOut.IsVoid = false;
                inventoryOut.EffectiveDate = effectiveDate;
                inventoryOut.ManufactureParty = inspectDetail.ManufactureParty;

                IList<InventoryTransaction> currentInventoryOutTransactionList = RecordInventory(inventoryOut);

                RecordLocationTransaction(inspectDetail, inspectMaster, effectiveDate, currentInventoryOutTransactionList, true);
                inventoryTransactionList.AddRange(currentInventoryOutTransactionList);
                #endregion


                #region 检验入库
                foreach (InventoryTransaction currentInventoryOutTransaction in currentInventoryOutTransactionList)
                {
                    InventoryIO inventoryIn = new InventoryIO();

                    inventoryIn.Location = inspectDetail.LocationFrom;
                    inventoryIn.Item = inspectDetail.Item;
                    inventoryIn.HuId = inspectDetail.HuId;
                    inventoryIn.Qty = -currentInventoryOutTransaction.Qty;
                    inventoryIn.LotNo = inspectDetail.LotNo;
                    //inventoryIn.ManufactureParty = null;
                    inventoryIn.QualityType = com.Sconit.CodeMaster.QualityType.Inspect; //报验入库为待验状态
                    inventoryIn.IsATP = inspectMaster.IsATP; //使用报验单上的可用标记，如果是二次检验的为false
                    inventoryIn.IsFreeze = false;
                    inventoryIn.IsCreatePlanBill = false;
                    inventoryIn.IsConsignment = currentInventoryOutTransaction.IsConsignment;
                    inventoryIn.PlanBill = currentInventoryOutTransaction.PlanBill;
                    inventoryIn.ActingBill = null;
                    inventoryIn.TransactionType = inspectMaster.IsATP ? CodeMaster.TransactionType.RCT_INP : CodeMaster.TransactionType.RCT_ISL;
                    inventoryIn.OccupyType = CodeMaster.OccupyType.Inspect;
                    inventoryIn.OccupyReferenceNo = inspectMaster.InspectNo;
                    inventoryIn.IsVoid = false;
                    inventoryIn.EffectiveDate = effectiveDate;
                    //inventoryIO.ManufactureParty = ;

                    IList<InventoryTransaction> currentInventoryInTransactionList = RecordInventory(inventoryIn);

                    RecordLocationTransaction(inspectDetail, inspectMaster, effectiveDate, currentInventoryInTransactionList, false);
                    inventoryTransactionList.AddRange(currentInventoryInTransactionList);
                }
                #endregion
            }

            return inventoryTransactionList;
        }

        private void RecordLocationTransaction(InspectDetail inspectDetail, InspectMaster inspectMaster, DateTime effectiveDate, IList<InventoryTransaction> inventoryTransactionList, bool isIssue)
        {
            DateTime dateTimeNow = DateTime.Now;

            //根据PlanBill和ActingBill分组，为了不同供应商的库存事务分开
            var groupedInventoryTransactionList = from trans in inventoryTransactionList
                                                  group trans by new
                                                  {
                                                      IsConsignment = trans.IsConsignment,
                                                      PlanBill = trans.PlanBill,
                                                      ActingBill = trans.ActingBill
                                                  }
                                                      into result
                                                      select new
                                                      {
                                                          IsConsignment = result.Key.IsConsignment,
                                                          PlanBill = result.Key.PlanBill,
                                                          ActingBill = result.Key.ActingBill,
                                                          //Qty = result.Sum(trans => isIssue ? -trans.Qty : trans.Qty),
                                                          Qty = result.Sum(trans => trans.Qty),
                                                          //PlanBillQty = result.Sum(trans => isIssue ? -trans.PlanBillQty : trans.PlanBillQty),
                                                          PlanBillQty = result.Sum(trans => trans.PlanBillQty),
                                                          //ActingBillQty = result.Sum(trans => isIssue ? -trans.ActingBillQty : trans.ActingBillQty),
                                                          ActingBillQty = result.Sum(trans => trans.ActingBillQty),
                                                          InventoryTransactionList = result.ToList()
                                                      };

            foreach (var groupedInventoryTransaction in groupedInventoryTransactionList)
            {
                LocationTransaction locationTransaction = new LocationTransaction();

                locationTransaction.OrderNo = inspectDetail.InspectNo;
                //locationTransaction.OrderType = feedInput.OrderType;
                //locationTransaction.OrderSubType = feedInput.OrderSubType;
                locationTransaction.OrderDetailSequence = inspectDetail.Sequence;
                locationTransaction.OrderDetailId = inspectDetail.Id;
                //locationTransaction.OrderBomDetId = 
                //locationTransaction.IpNo = 
                //locationTransaction.IpDetailId = 
                //locationTransaction.IpDetailSequence = 
                //locationTransaction.ReceiptNo = 
                //locationTransaction.ReceiptDetailId = 
                //locationTransaction.ReceiptDetailSequence = 
                //locationTransaction.SequenceNo = 
                //locationTransaction.TraceCode = feedInput.TraceCode;
                locationTransaction.Item = inspectDetail.Item;
                locationTransaction.Uom = inspectDetail.Uom;
                locationTransaction.BaseUom = inspectDetail.BaseUom;

                locationTransaction.Qty = groupedInventoryTransaction.Qty / inspectDetail.UnitQty;
                locationTransaction.UnitQty = inspectDetail.UnitQty;
                locationTransaction.IsConsignment = groupedInventoryTransaction.IsConsignment;
                if (groupedInventoryTransaction.IsConsignment && groupedInventoryTransaction.PlanBill.HasValue)
                {
                    locationTransaction.PlanBill = groupedInventoryTransaction.PlanBill.Value;
                }
                locationTransaction.PlanBillQty = groupedInventoryTransaction.PlanBillQty / inspectDetail.UnitQty;
                if (groupedInventoryTransaction.ActingBill.HasValue)
                {
                    locationTransaction.ActingBill = groupedInventoryTransaction.ActingBill.Value;
                }
                locationTransaction.ActingBillQty = groupedInventoryTransaction.ActingBillQty / inspectDetail.UnitQty;
                locationTransaction.QualityType = isIssue ? CodeMaster.QualityType.Qualified : CodeMaster.QualityType.Inspect;
                locationTransaction.HuId = inspectDetail.HuId;
                locationTransaction.LotNo = inspectDetail.LotNo;
                locationTransaction.TransactionType = isIssue ?
                   (inspectMaster.IsATP ? CodeMaster.TransactionType.ISS_INP : CodeMaster.TransactionType.ISS_ISL) :
                   (inspectMaster.IsATP ? CodeMaster.TransactionType.RCT_INP : CodeMaster.TransactionType.RCT_ISL);
                locationTransaction.IOType = isIssue ? CodeMaster.TransactionIOType.Out : CodeMaster.TransactionIOType.In;
                locationTransaction.PartyFrom = inspectMaster.Region;
                locationTransaction.PartyTo = inspectMaster.Region;
                locationTransaction.LocationFrom = inspectDetail.LocationFrom;
                locationTransaction.LocationTo = inspectDetail.LocationFrom;
                locationTransaction.LocationIOReason = string.Empty;
                locationTransaction.EffectiveDate = effectiveDate;
                locationTransaction.CreateUserId = SecurityContextHolder.Get().Id;
                locationTransaction.CreateDate = dateTimeNow;

                this.genericMgr.Create(locationTransaction);

                RecordLocationTransactionDetail(locationTransaction, inventoryTransactionList);
            }
        }
        #endregion

        #region 检验判定
        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> InspectJudge(InspectMaster inspectMaster, IList<InspectResult> inspectResultList)
        {
            return InspectJudge(inspectMaster, inspectResultList, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> InspectJudge(InspectMaster inspectMaster, IList<InspectResult> inspectResultList, DateTime effectiveDate)
        {
            if (inspectResultList == null || inspectResultList.Count == 0)
            {
                return null;
            }

            List<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();

            foreach (InspectResult inspectResult in inspectResultList)
            {
                #region 判定出库
                InventoryIO inventoryOut = new InventoryIO();

                inventoryOut.Location = inspectResult.CurrentLocation;
                inventoryOut.Item = inspectResult.Item;
                inventoryOut.HuId = inspectResult.HuId;
                inventoryOut.Qty = -inspectResult.JudgeQty * inspectResult.UnitQty;  //转换为库存单位，为负数
                inventoryOut.LotNo = inspectResult.LotNo;
                //inventoryOut.ManufactureParty = null;
                inventoryOut.QualityType = com.Sconit.CodeMaster.QualityType.Inspect; //判定一定是待验
                inventoryOut.IsATP = inspectMaster.IsATP;
                inventoryOut.IsFreeze = false;
                inventoryOut.IsCreatePlanBill = false;
                inventoryOut.IsConsignment = false;
                inventoryOut.PlanBill = null;
                inventoryOut.ActingBill = null;
                inventoryOut.TransactionType = inspectResult.JudgeResult == CodeMaster.JudgeResult.Qualified ? CodeMaster.TransactionType.ISS_INP_QDII : CodeMaster.TransactionType.ISS_INP_REJ;
                inventoryOut.OccupyType = CodeMaster.OccupyType.Inspect;
                inventoryOut.OccupyReferenceNo = inspectMaster.InspectNo;
                inventoryOut.IsVoid = false;
                inventoryOut.EffectiveDate = effectiveDate;
                inventoryOut.ManufactureParty = inspectResult.ManufactureParty;

                IList<InventoryTransaction> currentInventoryOutTransactionList = RecordInventory(inventoryOut);

                RecordLocationTransaction(inspectMaster, inspectResult, effectiveDate, currentInventoryOutTransactionList, true);
                inventoryTransactionList.AddRange(currentInventoryOutTransactionList);
                #endregion

                #region 判定入库
                foreach (InventoryTransaction currentInventoryOutTransaction in currentInventoryOutTransactionList)
                {
                    InventoryIO inventoryIn = new InventoryIO();

                    inventoryIn.Location = inspectResult.CurrentLocation;
                    inventoryIn.Item = inspectResult.Item;
                    inventoryIn.HuId = inspectResult.HuId;
                    //inventoryIn.Qty = inspectResult.JudgeQty * inspectResult.UnitQty;  //转换为库存单位
                    inventoryIn.Qty = -currentInventoryOutTransaction.Qty;  //转换为库存单位
                    inventoryIn.LotNo = inspectResult.LotNo;
                    //inventoryIn.ManufactureParty = null;
                    inventoryIn.QualityType = inspectResult.JudgeResult == CodeMaster.JudgeResult.Qualified
                        ? CodeMaster.QualityType.Qualified : CodeMaster.QualityType.Reject;  //判定合格为合格品，不合格为不合格品
                    inventoryIn.IsATP = inspectResult.JudgeResult == CodeMaster.JudgeResult.Qualified;  //合格为可用，不合格为不可用
                    inventoryIn.IsFreeze = false;
                    inventoryIn.IsCreatePlanBill = false;
                    inventoryIn.IsConsignment = currentInventoryOutTransaction.IsConsignment;
                    inventoryIn.PlanBill = currentInventoryOutTransaction.PlanBill;
                    inventoryIn.ActingBill = null;
                    inventoryIn.TransactionType = inspectResult.JudgeResult == CodeMaster.JudgeResult.Qualified
                        ? CodeMaster.TransactionType.RCT_INP_QDII : CodeMaster.TransactionType.RCT_INP_REJ;
                    inventoryIn.OccupyType = CodeMaster.OccupyType.None;   //判定完就不占用了
                    inventoryIn.OccupyReferenceNo = null;
                    inventoryIn.IsVoid = false;
                    inventoryIn.EffectiveDate = effectiveDate;
                    //inventoryIO.ManufactureParty = ;
                    inventoryIn.Bin = currentInventoryOutTransaction.Bin;//自动上架

                    IList<InventoryTransaction> currentInventoryInTransactionList = RecordInventory(inventoryIn);

                    RecordLocationTransaction(inspectMaster, inspectResult, effectiveDate, currentInventoryInTransactionList, false);
                    inventoryTransactionList.AddRange(currentInventoryInTransactionList);
                }
                #endregion
            }

            return inventoryTransactionList;
        }

        private void RecordLocationTransaction(InspectMaster inspectMaster, InspectResult inspectResult, DateTime effectiveDate, IList<InventoryTransaction> inventoryTransactionList, bool isIssue)
        {
            DateTime dateTimeNow = DateTime.Now;

            //根据PlanBill和ActingBill分组，为了不同供应商的库存事务分开
            var groupedInventoryTransactionList = from trans in inventoryTransactionList
                                                  group trans by new
                                                  {
                                                      IsConsignment = trans.IsConsignment,
                                                      PlanBill = trans.PlanBill,
                                                      ActingBill = trans.ActingBill
                                                  }
                                                      into result
                                                      select new
                                                      {
                                                          IsConsignment = result.Key.IsConsignment,
                                                          PlanBill = result.Key.PlanBill,
                                                          ActingBill = result.Key.ActingBill,
                                                          //Qty = result.Sum(trans => isIssue ? -trans.Qty : trans.Qty),
                                                          Qty = result.Sum(trans => trans.Qty),
                                                          //PlanBillQty = result.Sum(trans => isIssue ? -trans.PlanBillQty : trans.PlanBillQty),
                                                          PlanBillQty = result.Sum(trans => trans.PlanBillQty),
                                                          //ActingBillQty = result.Sum(trans => isIssue ? -trans.ActingBillQty : trans.ActingBillQty),
                                                          ActingBillQty = result.Sum(trans => trans.ActingBillQty),
                                                          InventoryTransactionList = result.ToList()
                                                      };

            foreach (var groupedInventoryTransaction in groupedInventoryTransactionList)
            {
                LocationTransaction locationTransaction = new LocationTransaction();

                locationTransaction.OrderNo = inspectResult.InspectNo;
                //locationTransaction.OrderType = ;
                //locationTransaction.OrderSubType = ;
                //locationTransaction.OrderDetailSequence = ;
                locationTransaction.OrderDetailId = inspectResult.Id;
                //locationTransaction.OrderBomDetId = 
                //locationTransaction.IpNo = 
                //locationTransaction.IpDetailId = 
                //locationTransaction.IpDetailSequence = 
                //locationTransaction.ReceiptNo = 
                //locationTransaction.ReceiptDetailId = 
                //locationTransaction.ReceiptDetailSequence = 
                //locationTransaction.SequenceNo = 
                //locationTransaction.TraceCode = ;
                locationTransaction.Item = inspectResult.Item;
                locationTransaction.Uom = inspectResult.Uom;
                locationTransaction.BaseUom = inspectResult.BaseUom;
                locationTransaction.Qty = groupedInventoryTransaction.Qty / inspectResult.UnitQty;
                locationTransaction.UnitQty = inspectResult.UnitQty;
                locationTransaction.IsConsignment = groupedInventoryTransaction.IsConsignment;
                if (groupedInventoryTransaction.IsConsignment && groupedInventoryTransaction.PlanBill.HasValue)
                {
                    locationTransaction.PlanBill = groupedInventoryTransaction.PlanBill.Value;
                }
                locationTransaction.PlanBillQty = groupedInventoryTransaction.PlanBillQty / inspectResult.UnitQty;
                if (groupedInventoryTransaction.ActingBill.HasValue)
                {
                    locationTransaction.ActingBill = groupedInventoryTransaction.ActingBill.Value;
                }
                locationTransaction.ActingBillQty = groupedInventoryTransaction.ActingBillQty / inspectResult.UnitQty;
                locationTransaction.QualityType = isIssue ?
                    CodeMaster.QualityType.Inspect : (inspectResult.JudgeResult == CodeMaster.JudgeResult.Qualified ?
                                                        CodeMaster.QualityType.Qualified : CodeMaster.QualityType.Reject);
                locationTransaction.HuId = inspectResult.HuId;
                locationTransaction.LotNo = inspectResult.LotNo;
                locationTransaction.TransactionType = isIssue ?
                   (inspectResult.JudgeResult == CodeMaster.JudgeResult.Qualified ?
                        CodeMaster.TransactionType.ISS_INP_QDII : CodeMaster.TransactionType.ISS_INP_REJ)
                 : (inspectResult.JudgeResult == CodeMaster.JudgeResult.Qualified ?
                        CodeMaster.TransactionType.RCT_INP_QDII : CodeMaster.TransactionType.RCT_INP_REJ);
                locationTransaction.IOType = isIssue ? CodeMaster.TransactionIOType.Out : CodeMaster.TransactionIOType.In;
                locationTransaction.PartyFrom = inspectMaster.Region;
                locationTransaction.PartyTo = inspectMaster.Region;
                locationTransaction.LocationFrom = inspectResult.CurrentLocation;
                locationTransaction.LocationTo = inspectResult.CurrentLocation;
                locationTransaction.LocationIOReason = string.Empty;
                locationTransaction.EffectiveDate = effectiveDate;
                locationTransaction.CreateUserId = SecurityContextHolder.Get().Id;
                locationTransaction.CreateDate = dateTimeNow;

                this.genericMgr.Create(locationTransaction);
                RecordLocationTransactionDetail(locationTransaction, inventoryTransactionList);
            }
        }
        #endregion

        #region 让步使用
        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> ConcessionToUse(ConcessionMaster consessionMaster)
        {
            return ConcessionToUse(consessionMaster, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> ConcessionToUse(ConcessionMaster consessionMaster, DateTime effectiveDate)
        {
            if (consessionMaster.ConcessionDetails != null && consessionMaster.ConcessionDetails.Count > 0)
            {
                List<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();
                foreach (ConcessionDetail concessionDetail in consessionMaster.ConcessionDetails)
                {
                    #region 让步出库
                    InventoryIO inventoryOut = new InventoryIO();

                    inventoryOut.Location = concessionDetail.LocationFrom;
                    inventoryOut.Item = concessionDetail.Item;
                    inventoryOut.HuId = concessionDetail.HuId;
                    inventoryOut.Qty = -concessionDetail.Qty * concessionDetail.UnitQty;  //转换为库存单位，为负数
                    inventoryOut.LotNo = concessionDetail.LotNo;
                    inventoryOut.QualityType = com.Sconit.CodeMaster.QualityType.Reject; //让步使用一定是不合格品
                    inventoryOut.IsATP = false;
                    inventoryOut.IsFreeze = false;
                    inventoryOut.IsCreatePlanBill = false;
                    inventoryOut.IsConsignment = false;
                    inventoryOut.PlanBill = null;
                    inventoryOut.ActingBill = null;
                    inventoryOut.TransactionType = CodeMaster.TransactionType.ISS_INP_CCS;
                    inventoryOut.OccupyType = CodeMaster.OccupyType.None;
                    inventoryOut.OccupyReferenceNo = null;
                    inventoryOut.IsVoid = false;
                    inventoryOut.EffectiveDate = effectiveDate;
                    //inventoryIO.ManufactureParty = ;

                    IList<InventoryTransaction> currentInventoryOutTransactionList = RecordInventory(inventoryOut);

                    RecordLocationTransaction(consessionMaster, concessionDetail, effectiveDate, currentInventoryOutTransactionList, true);
                    inventoryTransactionList.AddRange(currentInventoryOutTransactionList);
                    #endregion

                    #region 让步入库
                    foreach (InventoryTransaction currentInventoryOutTransaction in currentInventoryOutTransactionList)
                    {
                        InventoryIO inventoryIn = new InventoryIO();

                        inventoryIn.Location = concessionDetail.LocationTo;
                        inventoryIn.Item = concessionDetail.Item;
                        inventoryIn.HuId = concessionDetail.HuId;
                        inventoryIn.Qty = -currentInventoryOutTransaction.Qty;
                        inventoryIn.LotNo = concessionDetail.LotNo;
                        inventoryIn.QualityType = CodeMaster.QualityType.Qualified;
                        inventoryIn.IsATP = true;
                        inventoryIn.IsFreeze = false;
                        inventoryIn.IsCreatePlanBill = false;
                        inventoryIn.IsConsignment = currentInventoryOutTransaction.IsConsignment;
                        inventoryIn.PlanBill = currentInventoryOutTransaction.PlanBill;
                        inventoryIn.ActingBill = null;
                        inventoryIn.TransactionType = CodeMaster.TransactionType.RCT_INP_CCS;
                        inventoryIn.OccupyType = CodeMaster.OccupyType.None;
                        inventoryIn.OccupyReferenceNo = null;
                        inventoryIn.IsVoid = false;
                        inventoryIn.EffectiveDate = effectiveDate;
                        //inventoryIO.ManufactureParty = ;

                        IList<InventoryTransaction> currentInventoryInTransactionList = RecordInventory(inventoryIn);

                        RecordLocationTransaction(consessionMaster, concessionDetail, effectiveDate, currentInventoryInTransactionList, false);
                        inventoryTransactionList.AddRange(currentInventoryInTransactionList);
                    }

                    #endregion
                }

                return inventoryTransactionList;
            }

            return null;
        }

        private void RecordLocationTransaction(ConcessionMaster consessionMaster, ConcessionDetail concessionDetail, DateTime effectiveDate, IList<InventoryTransaction> inventoryTransactionList, bool isIssue)
        {
            DateTime dateTimeNow = DateTime.Now;


            //根据PlanBill和ActingBill分组，为了不同供应商的库存事务分开
            var groupedInventoryTransactionList = from trans in inventoryTransactionList
                                                  group trans by new
                                                  {
                                                      IsConsignment = trans.IsConsignment,
                                                      PlanBill = trans.PlanBill,
                                                      ActingBill = trans.ActingBill
                                                  }
                                                      into result
                                                      select new
                                                      {
                                                          IsConsignment = result.Key.IsConsignment,
                                                          PlanBill = result.Key.PlanBill,
                                                          ActingBill = result.Key.ActingBill,
                                                          //Qty = result.Sum(trans => isIssue ? -trans.Qty : trans.Qty),
                                                          Qty = result.Sum(trans => trans.Qty),
                                                          //PlanBillQty = result.Sum(trans => isIssue ? -trans.PlanBillQty : trans.PlanBillQty),
                                                          PlanBillQty = result.Sum(trans => trans.PlanBillQty),
                                                          //ActingBillQty = result.Sum(trans => isIssue ? -trans.ActingBillQty : trans.ActingBillQty),
                                                          ActingBillQty = result.Sum(trans => trans.ActingBillQty),
                                                          InventoryTransactionList = result.ToList()
                                                      };

            foreach (var groupedInventoryTransaction in groupedInventoryTransactionList)
            {
                LocationTransaction locationTransaction = new LocationTransaction();

                locationTransaction.OrderNo = concessionDetail.ConcessionNo;
                //locationTransaction.OrderType = ;
                //locationTransaction.OrderSubType = ;
                locationTransaction.OrderDetailSequence = concessionDetail.Sequence;
                locationTransaction.OrderDetailId = concessionDetail.Id;
                //locationTransaction.OrderBomDetId = 
                //locationTransaction.IpNo = 
                //locationTransaction.IpDetailId = 
                //locationTransaction.IpDetailSequence = 
                //locationTransaction.ReceiptNo = 
                //locationTransaction.ReceiptDetailId = 
                //locationTransaction.ReceiptDetailSequence = 
                //locationTransaction.SequenceNo = 
                //locationTransaction.TraceCode = ;
                locationTransaction.Item = concessionDetail.Item;
                locationTransaction.Uom = concessionDetail.Uom;
                locationTransaction.BaseUom = concessionDetail.BaseUom;
                locationTransaction.Qty = groupedInventoryTransaction.Qty / concessionDetail.UnitQty;
                locationTransaction.UnitQty = concessionDetail.UnitQty;
                locationTransaction.IsConsignment = groupedInventoryTransaction.IsConsignment;
                if (groupedInventoryTransaction.IsConsignment && groupedInventoryTransaction.PlanBill.HasValue)
                {
                    locationTransaction.PlanBill = groupedInventoryTransaction.PlanBill.Value;
                }
                locationTransaction.PlanBillQty = groupedInventoryTransaction.PlanBillQty / concessionDetail.UnitQty;
                if (groupedInventoryTransaction.ActingBill.HasValue)
                {
                    locationTransaction.ActingBill = groupedInventoryTransaction.ActingBill.Value;
                }
                locationTransaction.ActingBillQty = groupedInventoryTransaction.ActingBillQty / concessionDetail.UnitQty;
                locationTransaction.QualityType = isIssue ?
                    CodeMaster.QualityType.Reject : CodeMaster.QualityType.Qualified;
                locationTransaction.HuId = concessionDetail.HuId;
                locationTransaction.LotNo = concessionDetail.LotNo;
                locationTransaction.TransactionType = isIssue ? CodeMaster.TransactionType.ISS_INP_CCS : CodeMaster.TransactionType.RCT_INP_CCS;
                locationTransaction.IOType = isIssue ? CodeMaster.TransactionIOType.Out : CodeMaster.TransactionIOType.In;
                locationTransaction.PartyFrom = consessionMaster.Region;
                locationTransaction.PartyTo = consessionMaster.Region;
                locationTransaction.LocationFrom = concessionDetail.LocationFrom;
                locationTransaction.LocationTo = concessionDetail.LocationTo;
                locationTransaction.LocationIOReason = string.Empty;
                locationTransaction.EffectiveDate = effectiveDate;
                locationTransaction.CreateUserId = SecurityContextHolder.Get().Id;
                locationTransaction.CreateDate = dateTimeNow;

                this.genericMgr.Create(locationTransaction);
                RecordLocationTransactionDetail(locationTransaction, inventoryTransactionList);
            }
        }
        #endregion

        #region 装箱
        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> InventoryPack(IList<InventoryPack> inventoryPackList)
        {
            return InventoryPack(inventoryPackList, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> InventoryPack(IList<InventoryPack> inventoryPackList, DateTime effectiveDate)
        {
            IList<HuStatus> huStatusList = this.huMgr.GetHuStatus(inventoryPackList.Select(i => i.HuId).ToList());

            #region 检验
            BusinessException businessException = new BusinessException();
            foreach (InventoryPack inventoryPack in inventoryPackList)
            {
                HuStatus huStatus = huStatusList.Where(h => h.HuId == inventoryPack.HuId).SingleOrDefault();

                if (huStatus == null)
                {
                    businessException.AddMessage("条码{0}不存在。", huStatus.HuId);
                }

                if (huStatus.Status == CodeMaster.HuStatus.Location)
                {
                    businessException.AddMessage("条码{0}已经在库位{1}中，不能装箱。", huStatus.HuId, huStatus.Location);
                }

                if (huStatus.Status == CodeMaster.HuStatus.Ip)
                {
                    businessException.AddMessage("条码{0}为库位{1}至库位{2}的在途库存，不能装箱。", huStatus.HuId, huStatus.LocationFrom, huStatus.LocationTo);
                }

                if (inventoryPack.OccupyType != CodeMaster.OccupyType.None)
                {
                    businessException.AddMessage("零件{0}的库存已经被占用，不能装箱。", huStatus.Item);
                }

                inventoryPack.CurrentHu = huStatus;
            }

            if (businessException.HasMessage)
            {
                throw businessException;
            }
            #endregion

            #region 查找库位
            IList<Location> locationList = BatchLoadLocations(inventoryPackList.Select(i => i.Location).ToList());
            #endregion

            #region 循环装箱
            IList<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();
            foreach (InventoryPack inventoryPack in inventoryPackList)
            {
                inventoryPack.CurrentLocation = locationList.Where(l => l.Code == inventoryPack.Location).Single();

                #region 出库
                InventoryIO inventoryOut = new InventoryIO();
                IList<InventoryTransaction> issInventoryTransactionList = new List<InventoryTransaction>();

                if (inventoryPack.LocationLotDetailIdList != null && inventoryPack.LocationLotDetailIdList.Count > 0)
                {
                    #region 指定库存明细拆箱
                    foreach (int locationLotDetailId in inventoryPack.LocationLotDetailIdList)
                    {
                        inventoryOut.LocationLotDetailId = locationLotDetailId;
                        //inventoryOut.Qty = inventoryPack;  //转换为库存单位
                        inventoryOut.EffectiveDate = effectiveDate;

                        ((List<InventoryTransaction>)issInventoryTransactionList).AddRange(RecordInventory(inventoryOut));
                    }
                    #endregion
                }
                else
                {
                    inventoryOut.Location = inventoryPack.Location;
                    inventoryOut.Item = inventoryPack.CurrentHu.Item;
                    inventoryOut.HuId = null;
                    inventoryOut.Qty = -inventoryPack.CurrentHu.Qty * inventoryPack.CurrentHu.UnitQty;  //转换为库存单位
                    inventoryOut.LotNo = null;
                    inventoryOut.QualityType = inventoryPack.CurrentHu.QualityType;
                    inventoryOut.IsATP = inventoryPack.CurrentHu.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                    inventoryOut.IsFreeze = false;
                    inventoryOut.IsCreatePlanBill = false;
                    inventoryOut.IsConsignment = false;
                    inventoryOut.PlanBill = null;
                    inventoryOut.ActingBill = null;
                    inventoryOut.TransactionType = CodeMaster.TransactionType.ISS_REP;
                    inventoryOut.OccupyType = inventoryPack.OccupyType;
                    inventoryOut.OccupyReferenceNo = inventoryPack.OccupyReferenceNo;
                    inventoryOut.IsVoid = false;
                    inventoryOut.EffectiveDate = effectiveDate;
                    //inventoryIO.ManufactureParty = ;

                    issInventoryTransactionList = RecordInventory(inventoryOut);
                }

                #region 如果翻箱的出库库存明细有寄售非寄售混杂或者有多条寄售信息，立即结算
                TryManualSettleBill(issInventoryTransactionList, effectiveDate);
                #endregion

                ((List<InventoryTransaction>)inventoryTransactionList).AddRange(issInventoryTransactionList);
                RecordLocationTransaction(inventoryPack, effectiveDate, issInventoryTransactionList, true);
                #endregion

                #region 入库
                InventoryIO inventoryIn = new InventoryIO();

                inventoryIn.Location = inventoryPack.Location;
                inventoryIn.Item = inventoryPack.CurrentHu.Item;
                inventoryIn.HuId = inventoryPack.CurrentHu.HuId;
                inventoryIn.LotNo = inventoryPack.CurrentHu.LotNo;
                inventoryIn.Qty = inventoryPack.CurrentHu.Qty * inventoryPack.CurrentHu.UnitQty;  //转换为库存单位
                inventoryIn.QualityType = inventoryPack.CurrentHu.QualityType;
                inventoryIn.IsATP = inventoryPack.CurrentHu.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                inventoryIn.IsFreeze = false;
                inventoryIn.IsCreatePlanBill = false;
                inventoryIn.IsConsignment = issInventoryTransactionList.Select(i => i.IsConsignment).First();
                inventoryIn.PlanBill = inventoryIn.IsConsignment ? issInventoryTransactionList[0].PlanBill : null;
                inventoryIn.ActingBill = null;
                inventoryIn.TransactionType = CodeMaster.TransactionType.RCT_REP;
                inventoryIn.OccupyType = inventoryPack.OccupyType;
                inventoryIn.OccupyReferenceNo = inventoryPack.OccupyReferenceNo;
                inventoryIn.IsVoid = false;
                inventoryIn.EffectiveDate = effectiveDate;
                //inventoryIO.ManufactureParty = ;

                IList<InventoryTransaction> rctInventoryTransactionList = RecordInventory(inventoryIn);
                RecordLocationTransaction(inventoryPack, effectiveDate, rctInventoryTransactionList, false);
                ((List<InventoryTransaction>)inventoryTransactionList).AddRange(issInventoryTransactionList);
                #endregion
            }

            return inventoryTransactionList;
            #endregion
        }

        private void RecordLocationTransaction(InventoryPack inventoryPack, DateTime effectiveDate, IList<InventoryTransaction> inventoryTransactionList, bool isIssue)
        {
            DateTime dateTimeNow = DateTime.Now;

            //根据PlanBill和ActingBill分组，为了不同供应商的库存事务分开
            var groupedInventoryTransactionList = from trans in inventoryTransactionList
                                                  group trans by new
                                                  {
                                                      IsConsignment = trans.IsConsignment,
                                                      PlanBill = trans.PlanBill,
                                                      ActingBill = trans.ActingBill
                                                  }
                                                      into result
                                                      select new
                                                      {
                                                          IsConsignment = result.Key.IsConsignment,
                                                          PlanBill = result.Key.PlanBill,
                                                          ActingBill = result.Key.ActingBill,
                                                          //Qty = result.Sum(trans => isIssue ? -trans.Qty : trans.Qty),
                                                          Qty = result.Sum(trans => trans.Qty),
                                                          //PlanBillQty = result.Sum(trans => isIssue ? -trans.PlanBillQty : trans.PlanBillQty),
                                                          PlanBillQty = result.Sum(trans => trans.PlanBillQty),
                                                          //ActingBillQty = result.Sum(trans => isIssue ? -trans.ActingBillQty : trans.ActingBillQty),
                                                          ActingBillQty = result.Sum(trans => trans.ActingBillQty),
                                                          InventoryTransactionList = result.ToList()
                                                      };

            foreach (var groupedInventoryTransaction in groupedInventoryTransactionList)
            {
                LocationTransaction locationTransaction = new LocationTransaction();

                //locationTransaction.OrderNo = ;
                //locationTransaction.OrderType = ;
                //locationTransaction.OrderSubType = ;
                //locationTransaction.OrderDetailSequence = ;
                //locationTransaction.OrderDetailId = ;
                //locationTransaction.OrderBomDetId = 
                //locationTransaction.IpNo = 
                //locationTransaction.IpDetailId = 
                //locationTransaction.IpDetailSequence = 
                //locationTransaction.ReceiptNo = 
                //locationTransaction.ReceiptDetailId = 
                //locationTransaction.ReceiptDetailSequence = 
                //locationTransaction.SequenceNo = 
                //locationTransaction.TraceCode = ;
                locationTransaction.Item = inventoryPack.CurrentHu.Item;
                locationTransaction.Uom = inventoryPack.CurrentHu.Uom;
                locationTransaction.BaseUom = inventoryPack.CurrentHu.BaseUom;
                locationTransaction.Qty = groupedInventoryTransaction.Qty / inventoryPack.CurrentHu.UnitQty;
                locationTransaction.UnitQty = inventoryPack.CurrentHu.UnitQty;
                locationTransaction.IsConsignment = groupedInventoryTransaction.IsConsignment;
                if (groupedInventoryTransaction.IsConsignment && groupedInventoryTransaction.PlanBill.HasValue)
                {
                    locationTransaction.PlanBill = groupedInventoryTransaction.PlanBill.Value;
                }
                locationTransaction.PlanBillQty = groupedInventoryTransaction.PlanBillQty / inventoryPack.CurrentHu.UnitQty;
                if (groupedInventoryTransaction.ActingBill.HasValue)
                {
                    locationTransaction.ActingBill = groupedInventoryTransaction.ActingBill.Value;
                }
                locationTransaction.ActingBillQty = groupedInventoryTransaction.ActingBillQty / inventoryPack.CurrentHu.UnitQty;
                locationTransaction.QualityType = inventoryPack.CurrentHu.QualityType;
                locationTransaction.HuId = isIssue ? null : inventoryPack.CurrentHu.HuId;
                locationTransaction.LotNo = isIssue ? null : inventoryPack.CurrentHu.LotNo;
                locationTransaction.TransactionType = isIssue ? CodeMaster.TransactionType.ISS_REP : CodeMaster.TransactionType.RCT_REP;
                locationTransaction.IOType = isIssue ? CodeMaster.TransactionIOType.Out : CodeMaster.TransactionIOType.In;
                locationTransaction.PartyFrom = inventoryPack.CurrentLocation.Region;
                locationTransaction.PartyTo = inventoryPack.CurrentLocation.Region;
                locationTransaction.LocationFrom = inventoryPack.Location;
                locationTransaction.LocationTo = inventoryPack.Location;
                locationTransaction.LocationIOReason = string.Empty;
                locationTransaction.EffectiveDate = effectiveDate;
                locationTransaction.CreateUserId = SecurityContextHolder.Get().Id;
                locationTransaction.CreateDate = dateTimeNow;

                this.genericMgr.Create(locationTransaction);
                RecordLocationTransactionDetail(locationTransaction, groupedInventoryTransaction.InventoryTransactionList);
            }
        }
        #endregion

        #region 拆箱
        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> InventoryUnPack(IList<InventoryUnPack> inventoryUnPackList)
        {
            return InventoryUnPack(inventoryUnPackList, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> InventoryUnPack(IList<InventoryUnPack> inventoryUnPackList, DateTime effectiveDate)
        {
            IList<HuStatus> huStatusList = this.huMgr.GetHuStatus(inventoryUnPackList.Select(i => i.HuId).ToList());

            #region 检验
            BusinessException businessException = new BusinessException();
            foreach (InventoryUnPack inventoryUnPack in inventoryUnPackList)
            {
                HuStatus huStatus = huStatusList.Where(h => h.HuId == inventoryUnPack.HuId).SingleOrDefault();

                if (huStatus == null)
                {
                    businessException.AddMessage("条码{0}不存在。", huStatus.HuId);
                }

                if (huStatus.Status == CodeMaster.HuStatus.Ip)
                {
                    businessException.AddMessage("条码{0}为库位{1}至库位{2}的在途库存，不能拆箱。", huStatus.HuId, huStatus.LocationFrom, huStatus.LocationTo);
                }

                if (huStatus.Status == CodeMaster.HuStatus.NA)
                {
                    businessException.AddMessage("条码{0}已经不在任何库位中，不能拆箱。", huStatus.HuId);
                }

                if (huStatus.IsConsignment == true)
                {
                    int userId = SecurityContextHolder.Get().Id;
                    bool hasPermission = genericMgr.FindAll<UserPermissionView>(" from UserPermissionView as u where u.UserId=? and u.PermissionCode='Client_UnPackAllowCs' ", new object[] { userId }).Count == 0;
                    if (hasPermission)
                    {
                        businessException.AddMessage("寄售的条码{0}不能拆箱。", huStatus.HuId);
                    }
                }
                inventoryUnPack.CurrentHu = huStatus;
            }

            if (businessException.HasMessage)
            {
                throw businessException;
            }
            #endregion


            #region 做一个单号
            string repackNo = "DEVN" + numberControlMgr.GetNextSequenceo("DEVN").PadLeft(9, '0');
            #endregion

            #region 循环拆箱
            IList<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();
            foreach (InventoryUnPack inventoryUnPack in inventoryUnPackList)
            {
                #region 出库
                InventoryIO inventoryOut = new InventoryIO();

                inventoryOut.Location = inventoryUnPack.CurrentHu.Location;
                inventoryOut.Item = inventoryUnPack.CurrentHu.Item;
                inventoryOut.HuId = inventoryUnPack.CurrentHu.HuId;
                inventoryOut.Qty = -inventoryUnPack.CurrentHu.Qty * inventoryUnPack.CurrentHu.UnitQty;  //转换为库存单位
                inventoryOut.LotNo = inventoryUnPack.CurrentHu.LotNo;
                inventoryOut.QualityType = inventoryUnPack.CurrentHu.QualityType;
                inventoryOut.IsATP = inventoryUnPack.CurrentHu.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                inventoryOut.IsFreeze = inventoryUnPack.CurrentHu.IsFreeze;
                inventoryOut.IsCreatePlanBill = false;
                inventoryOut.IsConsignment = inventoryUnPack.CurrentHu.IsConsignment;
                inventoryOut.PlanBill = inventoryUnPack.CurrentHu.PlanBill;
                inventoryOut.ActingBill = null;
                inventoryOut.TransactionType = CodeMaster.TransactionType.ISS_REP;
                inventoryOut.OccupyType = inventoryUnPack.CurrentHu.OccupyType;
                inventoryOut.OccupyReferenceNo = inventoryUnPack.CurrentHu.OccupyReferenceNo;
                inventoryOut.IsVoid = false;
                inventoryOut.EffectiveDate = effectiveDate;
                //inventoryIO.ManufactureParty = ;

                IList<InventoryTransaction> issInventoryTransactionList = RecordInventory(inventoryOut);
                ((List<InventoryTransaction>)inventoryTransactionList).AddRange(issInventoryTransactionList);
                RecordLocationTransaction(inventoryUnPack, effectiveDate, issInventoryTransactionList, true, repackNo);
                #endregion

                #region 入库
                InventoryIO inventoryIn = new InventoryIO();

                inventoryIn.Location = inventoryUnPack.CurrentHu.Location;
                inventoryIn.Item = inventoryUnPack.CurrentHu.Item;
                inventoryIn.HuId = null;
                inventoryIn.LotNo = null;
                inventoryIn.Qty = inventoryUnPack.CurrentHu.Qty * inventoryUnPack.CurrentHu.UnitQty;  //转换为库存单位
                inventoryIn.QualityType = inventoryUnPack.CurrentHu.QualityType;
                inventoryIn.IsATP = inventoryUnPack.CurrentHu.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                inventoryIn.IsFreeze = inventoryUnPack.CurrentHu.IsFreeze;
                inventoryIn.IsCreatePlanBill = false;
                inventoryIn.IsConsignment = inventoryUnPack.CurrentHu.IsConsignment;
                inventoryIn.PlanBill = inventoryUnPack.CurrentHu.PlanBill;
                inventoryIn.ActingBill = null;
                inventoryIn.TransactionType = CodeMaster.TransactionType.RCT_REP;
                inventoryIn.OccupyType = inventoryUnPack.CurrentHu.OccupyType;
                inventoryIn.OccupyReferenceNo = inventoryUnPack.CurrentHu.OccupyReferenceNo;
                inventoryIn.IsVoid = false;
                inventoryIn.EffectiveDate = effectiveDate;
                //inventoryIO.ManufactureParty = ;

                IList<InventoryTransaction> rctInventoryTransactionList = RecordInventory(inventoryIn);
                RecordLocationTransaction(inventoryUnPack, effectiveDate, rctInventoryTransactionList, false, repackNo);
                ((List<InventoryTransaction>)inventoryTransactionList).AddRange(issInventoryTransactionList);
                #endregion
            }

            return inventoryTransactionList;
            #endregion
        }

        private void RecordLocationTransaction(InventoryUnPack inventoryUnPack, DateTime effectiveDate, IList<InventoryTransaction> inventoryTransactionList, bool isIssue,string repackNo)
        {
            DateTime dateTimeNow = DateTime.Now;

            //根据PlanBill和ActingBill分组，为了不同供应商的库存事务分开
            var groupedInventoryTransactionList = from trans in inventoryTransactionList
                                                  group trans by new
                                                  {
                                                      IsConsignment = trans.IsConsignment,
                                                      PlanBill = trans.PlanBill,
                                                      ActingBill = trans.ActingBill
                                                  }
                                                      into result
                                                      select new
                                                      {
                                                          IsConsignment = result.Key.IsConsignment,
                                                          PlanBill = result.Key.PlanBill,
                                                          ActingBill = result.Key.ActingBill,
                                                          //Qty = result.Sum(trans => isIssue ? -trans.Qty : trans.Qty),
                                                          Qty = result.Sum(trans => trans.Qty),
                                                          //PlanBillQty = result.Sum(trans => isIssue ? -trans.PlanBillQty : trans.PlanBillQty),
                                                          PlanBillQty = result.Sum(trans => trans.PlanBillQty),
                                                          //ActingBillQty = result.Sum(trans => isIssue ? -trans.ActingBillQty : trans.ActingBillQty),
                                                          ActingBillQty = result.Sum(trans => trans.ActingBillQty),
                                                          InventoryTransactionList = result.ToList()
                                                      };

            foreach (var groupedInventoryTransaction in groupedInventoryTransactionList)
            {
                LocationTransaction locationTransaction = new LocationTransaction();

                //locationTransaction.OrderNo = ;
                //locationTransaction.OrderType = ;
                //locationTransaction.OrderSubType = ;
                //locationTransaction.OrderDetailSequence = ;
                //locationTransaction.OrderDetailId = ;
                //locationTransaction.OrderBomDetId = 
                //locationTransaction.IpNo = 
                //locationTransaction.IpDetailId = 
                //locationTransaction.IpDetailSequence = 
                //locationTransaction.ReceiptNo = 
                //locationTransaction.ReceiptDetailId = 
                //locationTransaction.ReceiptDetailSequence = 
                //locationTransaction.SequenceNo = 
                //locationTransaction.TraceCode = ;
                locationTransaction.Item = inventoryUnPack.CurrentHu.Item;
                locationTransaction.Uom = inventoryUnPack.CurrentHu.Uom;
                locationTransaction.BaseUom = inventoryUnPack.CurrentHu.BaseUom;
                locationTransaction.Qty = groupedInventoryTransaction.Qty / inventoryUnPack.CurrentHu.UnitQty;
                locationTransaction.UnitQty = inventoryUnPack.CurrentHu.UnitQty;
                locationTransaction.IsConsignment = groupedInventoryTransaction.IsConsignment;
                if (groupedInventoryTransaction.IsConsignment && groupedInventoryTransaction.PlanBill.HasValue)
                {
                    locationTransaction.PlanBill = groupedInventoryTransaction.PlanBill.Value;
                }
                locationTransaction.PlanBillQty = groupedInventoryTransaction.PlanBillQty / inventoryUnPack.CurrentHu.UnitQty;
                if (groupedInventoryTransaction.ActingBill.HasValue)
                {
                    locationTransaction.ActingBill = groupedInventoryTransaction.ActingBill.Value;
                }
                locationTransaction.ActingBillQty = groupedInventoryTransaction.ActingBillQty / inventoryUnPack.CurrentHu.UnitQty;
                locationTransaction.QualityType = inventoryUnPack.CurrentHu.QualityType;
                locationTransaction.HuId = isIssue ? inventoryUnPack.CurrentHu.HuId : null;
                locationTransaction.LotNo = isIssue ? inventoryUnPack.CurrentHu.LotNo : null;
                locationTransaction.TransactionType = isIssue ? CodeMaster.TransactionType.ISS_REP : CodeMaster.TransactionType.RCT_REP;
                locationTransaction.IOType = isIssue ? CodeMaster.TransactionIOType.Out : CodeMaster.TransactionIOType.In;
                locationTransaction.PartyFrom = inventoryUnPack.CurrentHu.Region;
                locationTransaction.PartyTo = inventoryUnPack.CurrentHu.Region;
                locationTransaction.LocationFrom = inventoryUnPack.CurrentHu.Location;
                locationTransaction.LocationTo = inventoryUnPack.CurrentHu.Location;
                locationTransaction.LocationIOReason = string.Empty;
                locationTransaction.EffectiveDate = effectiveDate;
                locationTransaction.CreateUserId = SecurityContextHolder.Get().Id;
                locationTransaction.CreateDate = dateTimeNow;

                locationTransaction.OrderNo = repackNo;

                this.genericMgr.Create(locationTransaction);
                RecordLocationTransactionDetail(locationTransaction, groupedInventoryTransaction.InventoryTransactionList);
            }
        }
        #endregion

        #region 翻箱
        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> InventoryRePack(IList<InventoryRePack> inventoryRePackList)
        {
            return InventoryRePack(inventoryRePackList, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> InventoryRePack(IList<InventoryRePack> inventoryRePackList, DateTime effectiveDate)
        {
            IList<HuStatus> huStatusList = this.huMgr.GetHuStatus(inventoryRePackList.Select(i => i.HuId).ToList());
            #region 检验
            BusinessException businessException = new BusinessException();

            foreach (InventoryRePack inventoryRePack in inventoryRePackList)
            {
                HuStatus huStatus = huStatusList.Where(h => h.HuId == inventoryRePack.HuId).SingleOrDefault();

                if (huStatus == null)
                {
                    businessException.AddMessage("条码{0}不存在。", huStatus.HuId);
                }

                if (inventoryRePack.Type == CodeMaster.RePackType.Out)
                {
                    if (huStatus.Status == CodeMaster.HuStatus.Ip)
                    {
                        businessException.AddMessage("条码{0}为库位{1}至库位{2}的在途库存，不能翻箱。", huStatus.HuId, huStatus.LocationFrom, huStatus.LocationTo);
                    }

                    if (huStatus.Status == CodeMaster.HuStatus.NA)
                    {
                        businessException.AddMessage("条码{0}已经不在任何库位中，不能翻箱。", huStatus.HuId);
                    }

                    if (huStatus.OccupyType != CodeMaster.OccupyType.None)
                    {
                        businessException.AddMessage("条码{0}已经被占用，不能翻箱。", huStatus.HuId);
                    }
                }
                else
                {
                    if (huStatus.Status == CodeMaster.HuStatus.Location)
                    {
                        businessException.AddMessage("条码{0}已经在库位{1}中，不能翻箱。", huStatus.HuId, huStatus.Location);
                    }

                    if (huStatus.Status == CodeMaster.HuStatus.Ip)
                    {
                        businessException.AddMessage("条码{0}为库位{1}至库位{2}的在途库存，不能翻箱。", huStatus.HuId, huStatus.LocationFrom, huStatus.LocationTo);
                    }
                }
                inventoryRePack.Location = huStatus.Location;
                inventoryRePack.CurrentHu = huStatus;
            }

            var inventoryRePackOut = inventoryRePackList.Where(i => i.Type == CodeMaster.RePackType.Out);
            var inventoryRePackIn = inventoryRePackList.Where(i => i.Type == CodeMaster.RePackType.In);

            if (inventoryRePackOut.Select(i => i.Location).Distinct().Count() > 1)
            {
                businessException.AddMessage("翻箱前条码的库位不一致。");
            }

            if (inventoryRePackOut.Sum(i => i.CurrentHu.Qty * i.CurrentHu.UnitQty)
                 != inventoryRePackList.Where(i => i.Type == CodeMaster.RePackType.In).Sum(i => i.CurrentHu.Qty * i.CurrentHu.UnitQty))
            {
                businessException.AddMessage("翻箱前的条码数量和翻箱后的条码数量不一致。");
            }

            if (inventoryRePackList.Select(i => i.CurrentHu.Item).Distinct().Count() > 1)
            {
                businessException.AddMessage("不允许多个物料同时翻箱。");
            }

            #region 合并翻箱前寄售的信息
            //检查寄售信息是否一致
            if (inventoryRePackList.Where(i => i.Type == CodeMaster.RePackType.Out).Select(i => i.CurrentHu.IsConsignment).Distinct().Count() > 1)
            {
                businessException.AddMessage("翻箱前的条码存在寄售和非寄售。");
            }
            //检查所有的订单是否一致
            if (inventoryRePackOut.Where(i => i.CurrentHu.IsConsignment == true).Count() > 0)
            {
                this.billMgr.MergePlanBill(inventoryRePackOut.Select(i => i.CurrentHu).ToList());
            }
            #endregion
            //if (inventoryRePackList.Where(i => i.Type == CodeMaster.RePackType.Out && i.CurrentHu.PlanBill.HasValue && i.CurrentHu.IsConsignment).Distinct().Count() > 1)
            //{
            //    businessException.AddMessage("翻箱前的条码寄售账单不一致。");
            //}

            if (businessException.HasMessage)
            {
                throw businessException;
            }
            #endregion

            #region 查找库位
            Location location = this.genericMgr.FindById<Location>(inventoryRePackList.Where(i => i.Type == CodeMaster.RePackType.Out).Select(i => i.Location).First());
            //IList<Location> locationList = BatchLoadLocations(inventoryRePackList.Where(i => i.Type == CodeMaster.RePackType.Out).Select(i => i.Location).ToList());
            #endregion

            #region 循环拆箱
            IList<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();

            #region 做一个单号
            string repackNo = "REPK" + numberControlMgr.GetNextSequenceo("REPK").PadLeft(9, '0');
            #endregion


            #region 出库
            IList<InventoryTransaction> totalIssInventoryTransactionList = new List<InventoryTransaction>();
            foreach (InventoryRePack inventoryRePack in inventoryRePackOut)
            {
                inventoryRePack.CurrentLocation = location;

                InventoryIO inventoryOut = new InventoryIO();

                inventoryOut.Location = inventoryRePack.CurrentHu.Location;
                inventoryOut.Item = inventoryRePack.CurrentHu.Item;
                inventoryOut.HuId = inventoryRePack.CurrentHu.HuId;
                inventoryOut.Qty = -inventoryRePack.CurrentHu.Qty * inventoryRePack.CurrentHu.UnitQty;  //转换为库存单位
                inventoryOut.LotNo = inventoryRePack.CurrentHu.LotNo;
                inventoryOut.QualityType = inventoryRePack.CurrentHu.QualityType;
                inventoryOut.IsATP = inventoryRePack.CurrentHu.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                inventoryOut.IsFreeze = inventoryRePack.CurrentHu.IsFreeze;
                inventoryOut.IsCreatePlanBill = false;
                inventoryOut.IsConsignment = inventoryRePack.CurrentHu.IsConsignment;
                inventoryOut.PlanBill = inventoryRePack.CurrentHu.PlanBill;
                inventoryOut.ActingBill = null;
                inventoryOut.TransactionType = CodeMaster.TransactionType.ISS_REP;
                inventoryOut.OccupyType = inventoryRePack.CurrentHu.OccupyType;
                inventoryOut.OccupyReferenceNo = inventoryRePack.CurrentHu.OccupyReferenceNo;
                inventoryOut.IsVoid = false;
                inventoryOut.EffectiveDate = effectiveDate;

                //inventoryIO.ManufactureParty = ;

                IList<InventoryTransaction> issInventoryTransactionList = RecordInventory(inventoryOut);

                #region 如果翻箱的出库库存明细有寄售非寄售混杂或者有多条寄售信息，立即结算
                //TryManualSettleBill(totalIssInventoryTransactionList, effectiveDate);
                #endregion

                ((List<InventoryTransaction>)totalIssInventoryTransactionList).AddRange(issInventoryTransactionList);
                ((List<InventoryTransaction>)inventoryTransactionList).AddRange(issInventoryTransactionList);
                RecordLocationTransaction(inventoryRePack, effectiveDate, issInventoryTransactionList, true, repackNo);

                #region 把托盘关系去掉
                if (!string.IsNullOrEmpty(inventoryRePack.CurrentHu.PalletCode))
                {

                    var palletHu = this.genericMgr.FindAll<Entity.INV.PalletHu>("from PalletHu h where h.HuId = ? and h.PalletCode = ?", new object[] { inventoryRePack.CurrentHu.HuId, inventoryRePack.CurrentHu.PalletCode }).FirstOrDefault();
                    if (palletHu != null)
                    {
                        genericMgr.Delete(palletHu);
                    }


                    Hu repackHu = genericMgr.FindById<Hu>(inventoryRePack.CurrentHu.HuId);
                    repackHu.PalletCode = string.Empty;
                    genericMgr.Update(repackHu);
                }
                #endregion

            }
            #endregion

            #region 入库
            //如果是寄售的话因为前面合并了寄售信息，用出库的第一条寄售Id即可
            foreach (InventoryRePack inventoryRePack in inventoryRePackIn)
            {
                inventoryRePack.CurrentLocation = location;

                InventoryIO inventoryIn = new InventoryIO();

                inventoryIn.Location = location.Code;
                inventoryIn.Item = inventoryRePack.CurrentHu.Item;
                inventoryIn.HuId = inventoryRePack.CurrentHu.HuId;
                inventoryIn.LotNo = inventoryRePack.CurrentHu.LotNo;
                inventoryIn.Qty = inventoryRePack.CurrentHu.Qty * inventoryRePack.CurrentHu.UnitQty;  //转换为库存单位
                inventoryIn.QualityType = inventoryRePack.CurrentHu.QualityType;
                inventoryIn.IsATP = inventoryRePack.CurrentHu.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                inventoryIn.IsFreeze = inventoryRePack.CurrentHu.IsFreeze;
                inventoryIn.IsCreatePlanBill = false;
                inventoryIn.IsConsignment = totalIssInventoryTransactionList.Select(i => i.IsConsignment).First();
                //在翻之前一定会合并寄售信息，这样才可以取第一条寄售的信息做为入库的PlanBillId
                inventoryIn.PlanBill = inventoryIn.IsConsignment ? inventoryRePackOut.FirstOrDefault().CurrentHu.PlanBill : null;
                inventoryIn.ActingBill = null;
                inventoryIn.TransactionType = CodeMaster.TransactionType.RCT_REP;
                inventoryIn.OccupyType = inventoryRePack.CurrentHu.OccupyType;
                inventoryIn.OccupyReferenceNo = inventoryRePack.CurrentHu.OccupyReferenceNo;
                inventoryIn.IsVoid = false;
                inventoryIn.EffectiveDate = effectiveDate;
                //inventoryIO.ManufactureParty = ;

                IList<InventoryTransaction> rctInventoryTransactionList = RecordInventory(inventoryIn);
                ((List<InventoryTransaction>)inventoryTransactionList).AddRange(rctInventoryTransactionList);
                RecordLocationTransaction(inventoryRePack, effectiveDate, rctInventoryTransactionList, false, repackNo);
            }
            #endregion

            return inventoryTransactionList;
            #endregion
        }

        private void RecordLocationTransaction(InventoryRePack inventoryRePack, DateTime effectiveDate, IList<InventoryTransaction> inventoryTransactionList, bool isIssue, string orderNo = "")
        {
            DateTime dateTimeNow = DateTime.Now;

            //根据PlanBill和ActingBill分组，为了不同供应商的库存事务分开
            var groupedInventoryTransactionList = from trans in inventoryTransactionList
                                                  group trans by new
                                                  {
                                                      IsConsignment = trans.IsConsignment,
                                                      PlanBill = trans.PlanBill,
                                                      ActingBill = trans.ActingBill
                                                  }
                                                      into result
                                                      select new
                                                      {
                                                          IsConsignment = result.Key.IsConsignment,
                                                          PlanBill = result.Key.PlanBill,
                                                          ActingBill = result.Key.ActingBill,
                                                          //Qty = result.Sum(trans => isIssue ? -trans.Qty : trans.Qty),
                                                          Qty = result.Sum(trans => trans.Qty),
                                                          //PlanBillQty = result.Sum(trans => isIssue ? -trans.PlanBillQty : trans.PlanBillQty),
                                                          PlanBillQty = result.Sum(trans => trans.PlanBillQty),
                                                          //ActingBillQty = result.Sum(trans => isIssue ? -trans.ActingBillQty : trans.ActingBillQty),
                                                          ActingBillQty = result.Sum(trans => trans.ActingBillQty),
                                                          InventoryTransactionList = result.ToList()
                                                      };

            foreach (var groupedInventoryTransaction in groupedInventoryTransactionList)
            {
                LocationTransaction locationTransaction = new LocationTransaction();

                //locationTransaction.OrderNo = ;
                //locationTransaction.OrderType = ;
                //locationTransaction.OrderSubType = ;
                //locationTransaction.OrderDetailSequence = ;
                //locationTransaction.OrderDetailId = ;
                //locationTransaction.OrderBomDetId = 
                //locationTransaction.IpNo = 
                //locationTransaction.IpDetailId = 
                //locationTransaction.IpDetailSequence = 
                //locationTransaction.ReceiptNo = 
                //locationTransaction.ReceiptDetailId = 
                //locationTransaction.ReceiptDetailSequence = 
                //locationTransaction.SequenceNo = 
                //locationTransaction.TraceCode = ;


                locationTransaction.OrderNo = orderNo;
                locationTransaction.Item = inventoryRePack.CurrentHu.Item;
                locationTransaction.Uom = inventoryRePack.CurrentHu.Uom;
                locationTransaction.BaseUom = inventoryRePack.CurrentHu.BaseUom;
                locationTransaction.Qty = groupedInventoryTransaction.Qty / inventoryRePack.CurrentHu.UnitQty;
                locationTransaction.UnitQty = inventoryRePack.CurrentHu.UnitQty;
                locationTransaction.IsConsignment = groupedInventoryTransaction.IsConsignment;
                if (groupedInventoryTransaction.IsConsignment && groupedInventoryTransaction.PlanBill.HasValue)
                {
                    locationTransaction.PlanBill = groupedInventoryTransaction.PlanBill.Value;
                }
                locationTransaction.PlanBillQty = groupedInventoryTransaction.PlanBillQty / inventoryRePack.CurrentHu.UnitQty;
                if (groupedInventoryTransaction.ActingBill.HasValue)
                {
                    locationTransaction.ActingBill = groupedInventoryTransaction.ActingBill.Value;
                }
                locationTransaction.ActingBillQty = groupedInventoryTransaction.ActingBillQty / inventoryRePack.CurrentHu.UnitQty;
                locationTransaction.QualityType = inventoryRePack.CurrentHu.QualityType;
                locationTransaction.HuId = inventoryRePack.CurrentHu.HuId;
                locationTransaction.LotNo = inventoryRePack.CurrentHu.LotNo;
                locationTransaction.TransactionType = isIssue ? CodeMaster.TransactionType.ISS_REP : CodeMaster.TransactionType.RCT_REP;
                locationTransaction.IOType = isIssue ? CodeMaster.TransactionIOType.Out : CodeMaster.TransactionIOType.In;
                locationTransaction.PartyFrom = inventoryRePack.CurrentLocation.Region;
                locationTransaction.PartyTo = inventoryRePack.CurrentLocation.Region;
                locationTransaction.LocationFrom = inventoryRePack.CurrentLocation.Code;
                locationTransaction.LocationTo = inventoryRePack.CurrentLocation.Code;
                locationTransaction.LocationIOReason = string.Empty;
                locationTransaction.EffectiveDate = effectiveDate;
                locationTransaction.CreateUserId = SecurityContextHolder.Get().Id;
                locationTransaction.CreateDate = dateTimeNow;

                this.genericMgr.Create(locationTransaction);
                RecordLocationTransactionDetail(locationTransaction, groupedInventoryTransaction.InventoryTransactionList);
            }
        }
        #endregion

        #region 下架
        [Transaction(TransactionMode.Requires)]
        public void InventoryPick(IList<InventoryPick> inventoryPickList)
        {
            IList<LocationLotDetail> locationLotDetailList = this.GetHuLocationLotDetails(inventoryPickList.Select(i => i.HuId).ToList());
            foreach (LocationLotDetail locationLotDetail in locationLotDetailList)
            {
                if (!string.IsNullOrEmpty(locationLotDetail.Bin))
                {
                    locationLotDetail.IsPick = true;
                    RecordPickTransaction(locationLotDetail);
                    locationLotDetail.Bin = null;
                    this.genericMgr.Update(locationLotDetail);
                }

                //#region 条码和托盘取消关联
                //Hu hu = genericMgr.FindById<Hu>(locationLotDetail.HuId);
                //hu.PalletCode = string.Empty;
                //this.genericMgr.Update(hu);
                //#endregion
            }
        }
        private void RecordPickTransaction(LocationLotDetail locationLotDetail)
        {
            User user = SecurityContextHolder.Get();
            PickTransaction pickTransaction = new PickTransaction();
            pickTransaction.CreateDate = DateTime.Now;
            pickTransaction.CreateUserName = user.FullName;
            pickTransaction.CreateUserId = user.Id;
            pickTransaction.HuId = locationLotDetail.HuId;
            pickTransaction.LotNo = locationLotDetail.LotNo;
            pickTransaction.Bin = locationLotDetail.Bin;
            pickTransaction.Item = locationLotDetail.Item;
            //pickTransaction.ItemDescription = locationLotDetail.ItemDescription;
            pickTransaction.Location = locationLotDetail.Location;
            pickTransaction.IsPick = locationLotDetail.IsPick;
            this.genericMgr.Create(pickTransaction);
        }
        #endregion

        #region 上架
        [Transaction(TransactionMode.Requires)]
        public void InventoryPut(IList<InventoryPut> inventoryPutList)
        {
            IList<HuStatus> huStatusList = this.huMgr.GetHuStatus(inventoryPutList.Select(i => i.HuId).ToList());

            #region 循环获取Bin
            string hql = string.Empty;
            IList<object> paras = new List<object>();
            foreach (string binCode in inventoryPutList.Select(i => i.Bin).Distinct())
            {
                if (hql == string.Empty)
                {
                    hql = "from LocationBin where Code in (?";
                }
                else
                {
                    hql += ", ?";
                }
                paras.Add(binCode);
            }
            hql += ")";
            IList<LocationBin> binList = this.genericMgr.FindAll<LocationBin>(hql, paras.ToArray());
            #endregion

            #region 检验
            BusinessException businessException = new BusinessException();

            foreach (InventoryPut inventoryPut in inventoryPutList)
            {
                HuStatus huStatus = huStatusList.Where(h => h.HuId == inventoryPut.HuId).SingleOrDefault();

                if (huStatus == null)
                {
                    businessException.AddMessage("条码{0}不存在。", huStatus.HuId);
                }

                if (huStatus.Status == CodeMaster.HuStatus.Ip)
                {
                    businessException.AddMessage("条码{0}为库位{1}至库位{2}的在途库存，不能上架。", huStatus.HuId, huStatus.LocationFrom, huStatus.LocationTo);
                }

                if (huStatus.Status == CodeMaster.HuStatus.NA)
                {
                    businessException.AddMessage("条码{0}已经不在任何库位中，不能上架。", huStatus.HuId);
                }

                inventoryPut.CurrentBin = binList.Where(b => b.Code == inventoryPut.Bin).Single();
                if (huStatus.Status == CodeMaster.HuStatus.Location && huStatus.Location != inventoryPut.CurrentBin.Location)
                {
                    businessException.AddMessage("库格{0}不在条码{1}所属的库位{2}中，不能上架。", inventoryPut.Bin, huStatus.HuId, huStatus.Location);
                }
            }

            if (businessException.HasMessage)
            {
                throw businessException;
            }
            #endregion

            #region 上架
            IList<LocationLotDetail> locationLotDetailList = GetHuLocationLotDetails(inventoryPutList.Select(i => i.HuId).ToList());
            foreach (InventoryPut inventoryPut in inventoryPutList)
            {
                LocationLotDetail locationLotDetail = locationLotDetailList.Where(l => l.HuId == inventoryPut.HuId).SingleOrDefault();

                locationLotDetail.Bin = inventoryPut.Bin;
                locationLotDetail.IsPick = false;
                RecordPickTransaction(locationLotDetail);
                this.genericMgr.Update(locationLotDetail);
            }
            #endregion
        }

        [Transaction(TransactionMode.Requires)]
        public void InventoryPut(IList<StockTakeDetail> stockTakeDetailList)
        {
            IList<string> huIdList = stockTakeDetailList.Where(s => !string.IsNullOrWhiteSpace(s.Bin)).Select(s => s.HuId).ToList();
            IList<HuStatus> huStatusList = this.huMgr.GetHuStatus(huIdList);

            //需要上架列表
            IList<StockTakeDetail> putStockTakeDetailList = new List<StockTakeDetail>();
            foreach (StockTakeDetail stockTakeDetail in stockTakeDetailList.Where(s => !string.IsNullOrWhiteSpace(s.Bin)))
            {
                HuStatus huStatus = huStatusList.Where(h => h.HuId == stockTakeDetail.HuId).SingleOrDefault();

                if (huStatus == null)
                {
                    throw new BusinessException("条码{0}不存在。", huStatus.HuId);
                }

                if (stockTakeDetail.Location == huStatus.Location
                    && stockTakeDetail.Bin != huStatus.Bin)
                {
                    //条码在盘点的库位中，并且不在条码的库格中
                    putStockTakeDetailList.Add(stockTakeDetail);
                }
            }

            #region 上架
            if (putStockTakeDetailList.Count > 0)
            {
                IList<LocationLotDetail> huLocationLotDetailList = this.GetHuLocationLotDetails(putStockTakeDetailList.Select(s => s.HuId).ToList());
                foreach (LocationLotDetail huLocationLotDetail in huLocationLotDetailList)
                {
                    if (huLocationLotDetail.Qty > 0)
                    {
                        StockTakeDetail stockTakeDetail = putStockTakeDetailList.Where(s => s.HuId == huLocationLotDetail.HuId).Single();
                        huLocationLotDetail.Bin = stockTakeDetail.Bin;
                        //this.genericMgr.Update(stockTakeDetail);
                    }
                }
            }
            #endregion
        }
        #endregion

        #region 盘点差异调整
        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> StockTakeAdjust(StockTakeMaster stockTakeMaster, IList<StockTakeResult> stockTakeResultList)
        {
            if (stockTakeMaster.EffectiveDate.HasValue)
            {
                return StockTakeAdjust(stockTakeMaster, stockTakeResultList, stockTakeMaster.EffectiveDate.Value);
            }
            else
            {
                return StockTakeAdjust(stockTakeMaster, stockTakeResultList, DateTime.Now);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> StockTakeAdjust(StockTakeMaster stockTakeMaster, IList<StockTakeResult> stockTakeResultList, DateTime effectiveDate)
        {
            List<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();

            if (stockTakeResultList != null && stockTakeResultList.Count > 0)
            {
                foreach (StockTakeResult stockTakeResult in stockTakeResultList)
                {
                    if (stockTakeResult.DifferenceQty == 0)
                    {
                        continue;
                    }
                    if (stockTakeResult.QualityType == CodeMaster.QualityType.Inspect
                        || stockTakeResult.QualityType == CodeMaster.QualityType.Reject)
                    {
                        //待验的库存不能调整。
                        throw new TechnicalException("Can't adjust inspect/reject inventory.");
                    }
                    InventoryIO inventoryIO = new InventoryIO();
                    inventoryIO.Location = stockTakeResult.Location;
                    inventoryIO.Bin = stockTakeResult.Bin;
                    inventoryIO.Item = stockTakeResult.Item;
                    inventoryIO.HuId = stockTakeResult.HuId;
                    inventoryIO.LotNo = stockTakeResult.LotNo;
                    inventoryIO.Qty = stockTakeResult.DifferenceQty;  //盘亏为负数，出库。盘盈为正数，入库。
                    inventoryIO.QualityType = stockTakeResult.QualityType;
                    inventoryIO.IsATP = stockTakeResult.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                    inventoryIO.IsFreeze = false;
                    inventoryIO.IsCreatePlanBill = false;
                    inventoryIO.IsConsignment = false;
                    inventoryIO.PlanBill = null;
                    inventoryIO.ActingBill = null;
                    inventoryIO.TransactionType = CodeMaster.TransactionType.CYC_CNT;
                    inventoryIO.OccupyType = CodeMaster.OccupyType.None;
                    inventoryIO.OccupyReferenceNo = null;
                    inventoryIO.IsVoid = false;
                    inventoryIO.EffectiveDate = effectiveDate;
                    //inventoryIO.ManufactureParty = ;

                    IList<InventoryTransaction> currentInventoryTransactionList = RecordInventory(inventoryIO);
                    RecordLocationTransaction(stockTakeMaster, stockTakeResult, effectiveDate, currentInventoryTransactionList);
                    inventoryTransactionList.AddRange(currentInventoryTransactionList);
                }
            }

            return inventoryTransactionList;
        }

        private void RecordLocationTransaction(StockTakeMaster stockTakeMaster, StockTakeResult stockTakeResult, DateTime effectiveDate, IList<InventoryTransaction> inventoryTransactionList)
        {
            DateTime dateTimeNow = DateTime.Now;
            LocationTransaction locationTransaction = new LocationTransaction();

            locationTransaction.OrderNo = stockTakeResult.StNo;
            //locationTransaction.OrderType = ;
            //locationTransaction.OrderSubType = ;
            //locationTransaction.OrderDetailSequence =;
            //locationTransaction.OrderDetailId =;
            //locationTransaction.OrderBomDetId = 
            //locationTransaction.IpNo = 
            //locationTransaction.IpDetailId = 
            //locationTransaction.IpDetailSequence = 
            //locationTransaction.ReceiptNo = 
            //locationTransaction.ReceiptDetailId = 
            //locationTransaction.ReceiptDetailSequence = 
            //locationTransaction.SequenceNo = 
            //locationTransaction.TraceCode = ;
            locationTransaction.Item = stockTakeResult.Item;
            locationTransaction.Uom = stockTakeResult.Uom;
            locationTransaction.BaseUom = stockTakeResult.Uom;
            locationTransaction.Qty = stockTakeResult.DifferenceQty;
            locationTransaction.UnitQty = 1;
            locationTransaction.ActingBillQty = inventoryTransactionList.Sum(i => i.ActingBillQty);
            locationTransaction.QualityType = stockTakeResult.QualityType;
            locationTransaction.HuId = stockTakeResult.HuId;
            locationTransaction.LotNo = stockTakeResult.LotNo;
            locationTransaction.TransactionType = CodeMaster.TransactionType.CYC_CNT;
            locationTransaction.IOType = stockTakeResult.DifferenceQty < 0 ? CodeMaster.TransactionIOType.Out : CodeMaster.TransactionIOType.In;
            locationTransaction.PartyFrom = stockTakeMaster.Region;
            locationTransaction.PartyTo = stockTakeMaster.Region;
            locationTransaction.LocationFrom = stockTakeResult.Location;
            locationTransaction.LocationTo = stockTakeResult.Location;
            locationTransaction.LocationIOReason = stockTakeMaster.CostCenter;
            locationTransaction.EffectiveDate = effectiveDate;
            locationTransaction.CreateUserId = SecurityContextHolder.Get().Id;
            locationTransaction.CreateDate = dateTimeNow;

            this.genericMgr.Create(locationTransaction);
            RecordLocationTransactionDetail(locationTransaction, inventoryTransactionList);
        }
        #endregion

        #region 库存占用
        [Transaction(TransactionMode.Requires)]
        public IList<LocationLotDetail> InventoryOccupy(IList<InventoryOccupy> inventoryOccupyList)
        {
            IList<LocationLotDetail> locationLotDetailList = this.GetHuLocationLotDetails(inventoryOccupyList.Select(i => i.HuId).ToList());

            locationLotDetailList = locationLotDetailList.Where(l => l.Qty > 0).ToList();

            #region 检查条码是否在库位中存在
            BusinessException businessException = new BusinessException();
            foreach (InventoryOccupy inveontryOccupy in inventoryOccupyList)
            {
                LocationLotDetail locationLotDetail = locationLotDetailList.Where(l => l.HuId == inveontryOccupy.HuId).SingleOrDefault();
                if (locationLotDetail == null)
                {
                    businessException = new BusinessException(string.Format("条码{0}不在任何库位中。", inveontryOccupy.HuId));
                }
                else if (!string.IsNullOrWhiteSpace(inveontryOccupy.Location) && locationLotDetail.Location != inveontryOccupy.Location)
                {
                    businessException = new BusinessException(string.Format("条码{0}不在指定的库位{1}中。", inveontryOccupy.HuId, inveontryOccupy.Location));
                }

                #region 检查条码是否指定的质量状态
                if (locationLotDetail != null && locationLotDetail.QualityType != inveontryOccupy.QualityType)
                {
                    businessException = new BusinessException(string.Format("条码{0}的质量状态不是要求的{1}。", inveontryOccupy.HuId,
                        this.systemMgr.GetCodeDetailDescription(CodeMaster.CodeMaster.QualityType, ((int)inveontryOccupy.QualityType).ToString())));
                }
                #endregion

                #region 检查条码是否冻结
                if (locationLotDetail != null && locationLotDetail.IsFreeze)
                {
                    businessException = new BusinessException(string.Format("条码{0}已经被冻结。", inveontryOccupy.HuId));
                }
                #endregion
            }

            if (businessException.HasMessage)
            {
                throw businessException;
            }
            #endregion

            #region 检查条码是否被占用
            IList<LocationLotDetail> occupyLocationLotDetailList = locationLotDetailList.Where(l => l.OccupyType != CodeMaster.OccupyType.None).ToList();
            if (occupyLocationLotDetailList != null && occupyLocationLotDetailList.Count > 0)
            {
                foreach (LocationLotDetail locationLotDetail in occupyLocationLotDetailList)
                {
                    CheckLocationLotDetailOccupied(locationLotDetail, businessException);
                }
            }
            #endregion

            #region 条码占用
            foreach (InventoryOccupy inveontryOccupy in inventoryOccupyList)
            {
                LocationLotDetail locationLotDetail = locationLotDetailList.Where(l => l.HuId == inveontryOccupy.HuId).Single();

                locationLotDetail.Bin = null;                                           //下架
                locationLotDetail.OccupyType = inveontryOccupy.OccupyType;               //占用
                locationLotDetail.OccupyReferenceNo = inveontryOccupy.OccupyReferenceNo;//占用单号
                this.genericMgr.Update(locationLotDetail);
            }
            #endregion

            return locationLotDetailList;
        }
        #endregion

        #region 库存占用取消
        [Transaction(TransactionMode.Requires)]
        public IList<LocationLotDetail> CancelInventoryOccupy(CodeMaster.OccupyType occupyType, string occupyReferenceNo)
        {
            return CancelInventoryOccupy(occupyType, occupyReferenceNo, null);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<LocationLotDetail> CancelInventoryOccupy(CodeMaster.OccupyType occupyType, string occupyReferenceNo, IList<string> huIdList)
        {
            string hql = "from LocationLotDetail where OccupyType = ? and OccupyReferenceNo = ?";
            IList<LocationLotDetail> locationLotDetailList = this.genericMgr.FindAll<LocationLotDetail>(hql, new object[] { occupyType, occupyReferenceNo });
            locationLotDetailList = locationLotDetailList.Where(l => l.Qty > 0).ToList();

            if (huIdList != null && huIdList.Count > 0)
            {
                BusinessException businessException = new BusinessException();
                foreach (string huId in huIdList)
                {
                    LocationLotDetail locationLotDetail = locationLotDetailList.Where(l => l.HuId == huId).SingleOrDefault();
                    if (locationLotDetail == null)
                    {
                        businessException.AddMessage("条码{0}没有被订单{1}占用。", huId, occupyReferenceNo);
                    }
                }

                if (businessException.HasMessage)
                {
                    throw businessException;
                }
            }

            IList<LocationLotDetail> returnLocationLotDetailList = new List<LocationLotDetail>();
            foreach (LocationLotDetail locationLotDetail in locationLotDetailList)
            {
                if (huIdList == null || huIdList.Contains(locationLotDetail.HuId))
                {
                    locationLotDetail.OccupyType = CodeMaster.OccupyType.None;
                    locationLotDetail.OccupyReferenceNo = null;
                    this.genericMgr.Update(locationLotDetail);

                    returnLocationLotDetailList.Add(locationLotDetail);
                }
            }

            return returnLocationLotDetailList;
        }
        #endregion

        #region 计划外出/入库
        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> InventoryOtherInOut(MiscOrderMaster miscOrderMaster, MiscOrderDetail miscOrderDetail)
        {
            return InventoryOtherInOut(miscOrderMaster, miscOrderDetail, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> InventoryOtherInOut(MiscOrderMaster miscOrderMaster, MiscOrderDetail miscOrderDetail, DateTime effectiveDate)
        {
            List<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();

            #region 出库
            if (miscOrderMaster.IsScanHu)
            {
                #region 条码
                foreach (MiscOrderLocationDetail miscOrderLocationDetail in miscOrderDetail.MiscOrderLocationDetails)
                {
                    if (miscOrderMaster.Type == CodeMaster.MiscOrderType.GR && miscOrderMaster.IsCs
                        && !string.IsNullOrWhiteSpace(miscOrderDetail.ManufactureParty))
                    {
                        PlanBill planBill = this.billMgr.CreatePlanBill(miscOrderMaster, miscOrderDetail, miscOrderLocationDetail, effectiveDate);

                        miscOrderLocationDetail.IsCreatePlanBill = true;
                        miscOrderLocationDetail.IsConsignment = true;
                        miscOrderLocationDetail.PlanBill = planBill.Id;
                    }

                    InventoryIO inventoryIO = new InventoryIO();

                    inventoryIO.Location = !string.IsNullOrWhiteSpace(miscOrderDetail.Location) ? miscOrderDetail.Location : miscOrderMaster.Location;
                    inventoryIO.Item = miscOrderLocationDetail.Item;
                    inventoryIO.HuId = miscOrderLocationDetail.HuId;
                    inventoryIO.Qty = miscOrderMaster.Type == CodeMaster.MiscOrderType.GI ? -miscOrderLocationDetail.Qty : miscOrderLocationDetail.Qty; //出库为负数
                    inventoryIO.LotNo = miscOrderLocationDetail.LotNo;
                    inventoryIO.QualityType = miscOrderLocationDetail.QualityType;
                    inventoryIO.IsATP = miscOrderLocationDetail.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                    inventoryIO.IsFreeze = miscOrderLocationDetail.IsFreeze;
                    inventoryIO.IsCreatePlanBill = miscOrderLocationDetail.IsCreatePlanBill;
                    inventoryIO.IsConsignment = miscOrderLocationDetail.IsConsignment;
                    inventoryIO.PlanBill = miscOrderLocationDetail.PlanBill;
                    inventoryIO.ActingBill = miscOrderLocationDetail.ActingBill;
                    inventoryIO.TransactionType = miscOrderMaster.Type == CodeMaster.MiscOrderType.GI ? CodeMaster.TransactionType.ISS_UNP : CodeMaster.TransactionType.RCT_UNP;
                    inventoryIO.OccupyType = miscOrderLocationDetail.OccupyType;
                    inventoryIO.OccupyReferenceNo = miscOrderLocationDetail.OccupyReferenceNo;
                    inventoryIO.IsVoid = false;
                    inventoryIO.EffectiveDate = effectiveDate;
                    //inventoryIO.ManufactureParty = ;

                    IList<InventoryTransaction> currentInventoryTransactionList = RecordInventory(inventoryIO);
                    RecordLocationTransaction(miscOrderMaster, miscOrderDetail, effectiveDate, currentInventoryTransactionList, false);
                    inventoryTransactionList.AddRange(currentInventoryTransactionList);
                }
                #endregion
            }
            else
            {
                #region 数量
                PlanBill planBill = null;
                if (miscOrderMaster.Type == CodeMaster.MiscOrderType.GR
                       && !string.IsNullOrWhiteSpace(miscOrderDetail.ManufactureParty))
                {
                    planBill = this.billMgr.CreatePlanBill(miscOrderMaster, miscOrderDetail, null, effectiveDate);
                }

                InventoryIO inventoryIO = new InventoryIO();

                inventoryIO.Location = !string.IsNullOrWhiteSpace(miscOrderDetail.Location) ? miscOrderDetail.Location : miscOrderMaster.Location;
                inventoryIO.Item = miscOrderDetail.Item;
                //inventoryIO.HuId = miscOrderDetail.HuId;
                inventoryIO.Qty = (miscOrderMaster.Type == CodeMaster.MiscOrderType.GI ? -miscOrderDetail.Qty : miscOrderDetail.Qty) * miscOrderDetail.UnitQty; //出库为负数，同时转为库存单位
                //inventoryIO.LotNo = miscOrderLocationDetail.LotNo;
                inventoryIO.QualityType = miscOrderMaster.QualityType;
                inventoryIO.IsATP = miscOrderMaster.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                inventoryIO.IsFreeze = false;
                inventoryIO.IsCreatePlanBill = planBill != null;
                inventoryIO.IsConsignment = planBill != null;
                inventoryIO.PlanBill = planBill != null ? (int?)planBill.Id : null;
                //inventoryIO.ActingBill = miscOrderLocationDetail.ActingBill;
                inventoryIO.TransactionType = miscOrderMaster.Type == CodeMaster.MiscOrderType.GI ? CodeMaster.TransactionType.ISS_UNP : CodeMaster.TransactionType.RCT_UNP;
                //inventoryIO.OccupyType = miscOrderLocationDetail.OccupyType;
                //inventoryIO.OccupyReferenceNo = miscOrderLocationDetail.OccupyReferenceNo;
                inventoryIO.IsVoid = false;
                inventoryIO.EffectiveDate = effectiveDate;
                inventoryIO.ManufactureParty = miscOrderDetail.ManufactureParty;

                IList<InventoryTransaction> currentInventoryTransactionList = RecordInventory(inventoryIO);
                RecordLocationTransaction(miscOrderMaster, miscOrderDetail, effectiveDate, currentInventoryTransactionList, false);
                inventoryTransactionList.AddRange(currentInventoryTransactionList);
                #endregion
            }
            #endregion
            return inventoryTransactionList;
        }

        private void RecordLocationTransaction(MiscOrderMaster miscOrderMaster, MiscOrderDetail miscOrderDetail, DateTime effectiveDate, IList<InventoryTransaction> inventoryTransactionList, bool isVoid)
        {
            DateTime dateTimeNow = DateTime.Now;

            if (miscOrderMaster.IsScanHu)
            {
                //按条码出入库只可能有一条
                InventoryTransaction inventoryTransaction = inventoryTransactionList[0];

                LocationTransaction locationTransaction = new LocationTransaction();
                locationTransaction.OrderNo = miscOrderMaster.MiscOrderNo;
                //locationTransaction.OrderType = ipDetail.OrderType;
                //locationTransaction.OrderSubType = ipDetail.OrderSubType;
                locationTransaction.OrderDetailSequence = miscOrderDetail.Sequence;
                locationTransaction.OrderDetailId = miscOrderDetail.Id;
                //locationTransaction.OrderBomDetId = 
                //locationTransaction.IpNo = ipDetail.IpNo;
                //locationTransaction.IpDetailId = ipDetail.Id;
                //locationTransaction.IpDetailSequence = ipDetail.Sequence;
                //locationTransaction.ReceiptNo = string.Empty;
                //locationTransaction.RecDetSeq = 
                //locationTransaction.SequenceNo = ipDetailInput.SequenceNo;
                //locationTransaction.TraceCode =
                locationTransaction.Item = miscOrderDetail.Item;
                locationTransaction.Uom = miscOrderDetail.Uom;
                locationTransaction.BaseUom = miscOrderDetail.BaseUom;
                locationTransaction.Qty = inventoryTransaction.Qty / miscOrderDetail.UnitQty;  //转为订单单位
                locationTransaction.UnitQty = miscOrderDetail.UnitQty;
                locationTransaction.IsConsignment = inventoryTransaction.IsConsignment;
                if (inventoryTransaction.PlanBill.HasValue)
                {
                    locationTransaction.PlanBill = inventoryTransaction.PlanBill.Value;
                }
                locationTransaction.PlanBillQty = inventoryTransaction.PlanBillQty / miscOrderDetail.UnitQty;  //转为订单单位
                if (inventoryTransaction.ActingBill.HasValue)
                {
                    locationTransaction.ActingBill = inventoryTransaction.ActingBill.Value;
                }
                locationTransaction.ActingBillQty = inventoryTransaction.ActingBillQty / miscOrderDetail.UnitQty;  //转为订单单位
                locationTransaction.QualityType = inventoryTransaction.QualityType;
                locationTransaction.HuId = inventoryTransaction.HuId;
                locationTransaction.LotNo = inventoryTransaction.LotNo;
                if (!isVoid)
                {
                    locationTransaction.TransactionType = miscOrderMaster.Type == CodeMaster.MiscOrderType.GI ? CodeMaster.TransactionType.ISS_UNP : CodeMaster.TransactionType.RCT_UNP;
                }
                else
                {
                    locationTransaction.TransactionType = miscOrderMaster.Type == CodeMaster.MiscOrderType.GI ? CodeMaster.TransactionType.ISS_UNP_VOID : CodeMaster.TransactionType.RCT_UNP_VOID;
                }
                locationTransaction.IOType = miscOrderMaster.Type == CodeMaster.MiscOrderType.GI ? CodeMaster.TransactionIOType.Out : CodeMaster.TransactionIOType.In;
                locationTransaction.PartyFrom = miscOrderMaster.Type == CodeMaster.MiscOrderType.GI ? miscOrderMaster.Region : null;
                locationTransaction.PartyTo = miscOrderMaster.Type == CodeMaster.MiscOrderType.GI ? null : miscOrderMaster.Region;
                locationTransaction.LocationFrom = miscOrderMaster.Type == CodeMaster.MiscOrderType.GI ? (!string.IsNullOrWhiteSpace(miscOrderDetail.Location) ? miscOrderDetail.Location : miscOrderMaster.Location) : null;
                locationTransaction.LocationTo = miscOrderMaster.Type == CodeMaster.MiscOrderType.GI ? null : (!string.IsNullOrWhiteSpace(miscOrderDetail.Location) ? miscOrderDetail.Location : miscOrderMaster.Location);
                locationTransaction.LocationIOReason = miscOrderMaster.MoveType;
                locationTransaction.EffectiveDate = effectiveDate;
                locationTransaction.CreateUserId = SecurityContextHolder.Get().Id;
                locationTransaction.CreateDate = dateTimeNow;

                this.genericMgr.Create(locationTransaction);

                RecordLocationTransactionDetail(locationTransaction, inventoryTransactionList);
            }
            else
            {
                //根据PlanBill和ActingBill分组，为了不同供应商的库存事务分开
                var groupedInventoryTransactionList = from trans in inventoryTransactionList
                                                      group trans by new
                                                      {
                                                          IsConsignment = trans.IsConsignment,
                                                          PlanBill = trans.PlanBill,
                                                          ActingBill = trans.ActingBill
                                                      }
                                                          into result
                                                          select new
                                                          {
                                                              IsConsignment = result.Key.IsConsignment,
                                                              PlanBill = result.Key.PlanBill,
                                                              ActingBill = result.Key.ActingBill,
                                                              Qty = result.Sum(trans => trans.Qty),
                                                              PlanBillQty = result.Sum(trans => trans.PlanBillQty),
                                                              ActingBillQty = result.Sum(trans => trans.ActingBillQty),
                                                              InventoryTransactionList = result.ToList()
                                                          };

                foreach (var groupedInventoryTransaction in groupedInventoryTransactionList)
                {
                    LocationTransaction locationTransaction = new LocationTransaction();

                    locationTransaction.OrderNo = miscOrderMaster.MiscOrderNo;
                    //locationTransaction.OrderType = ipDetail.OrderType;
                    //locationTransaction.OrderSubType = ipDetail.OrderSubType;
                    locationTransaction.OrderDetailSequence = miscOrderDetail.Sequence;
                    locationTransaction.OrderDetailId = miscOrderDetail.Id;
                    //locationTransaction.OrderBomDetId = 
                    //locationTransaction.IpNo = ipDetail.IpNo;
                    //locationTransaction.IpDetailId = ipDetail.Id;
                    //locationTransaction.IpDetailSequence = ipDetail.Sequence;
                    //locationTransaction.ReceiptNo = string.Empty;
                    //locationTransaction.RecDetSeq = 
                    //locationTransaction.SequenceNo = ipDetailInput.SequenceNo;
                    //locationTransaction.TraceCode =
                    locationTransaction.Item = miscOrderDetail.Item;
                    locationTransaction.Uom = miscOrderDetail.Uom;
                    locationTransaction.BaseUom = miscOrderDetail.BaseUom;
                    locationTransaction.Qty = groupedInventoryTransaction.Qty / miscOrderDetail.UnitQty; //转为订单单位
                    locationTransaction.UnitQty = miscOrderDetail.UnitQty;
                    locationTransaction.IsConsignment = groupedInventoryTransaction.IsConsignment;
                    if (groupedInventoryTransaction.IsConsignment && groupedInventoryTransaction.PlanBill.HasValue)
                    {
                        locationTransaction.PlanBill = groupedInventoryTransaction.PlanBill.Value;
                    }
                    locationTransaction.PlanBillQty = groupedInventoryTransaction.PlanBillQty / miscOrderDetail.UnitQty;  //转为订单单位
                    if (groupedInventoryTransaction.ActingBill.HasValue)
                    {
                        locationTransaction.ActingBill = groupedInventoryTransaction.ActingBill.Value;
                    }
                    locationTransaction.ActingBillQty = groupedInventoryTransaction.ActingBillQty / miscOrderDetail.UnitQty;  //转为订单单位
                    locationTransaction.QualityType = miscOrderMaster.QualityType;
                    //locationTransaction.HuId = ipDetailInput.HuId;
                    //locationTransaction.LotNo = ipDetailInput.LotNo;
                    if (!isVoid)
                    {
                        locationTransaction.TransactionType = miscOrderMaster.Type == CodeMaster.MiscOrderType.GI ? CodeMaster.TransactionType.ISS_UNP : CodeMaster.TransactionType.RCT_UNP;
                    }
                    else
                    {
                        locationTransaction.TransactionType = miscOrderMaster.Type == CodeMaster.MiscOrderType.GI ? CodeMaster.TransactionType.ISS_UNP_VOID : CodeMaster.TransactionType.RCT_UNP_VOID;
                    }
                    locationTransaction.IOType = miscOrderMaster.Type == CodeMaster.MiscOrderType.GI ? CodeMaster.TransactionIOType.Out : CodeMaster.TransactionIOType.In;
                    locationTransaction.PartyFrom = miscOrderMaster.Type == CodeMaster.MiscOrderType.GI ? miscOrderMaster.Region : null;
                    locationTransaction.PartyTo = miscOrderMaster.Type == CodeMaster.MiscOrderType.GI ? null : miscOrderMaster.Region;
                    locationTransaction.LocationFrom = miscOrderMaster.Type == CodeMaster.MiscOrderType.GI ? (!string.IsNullOrWhiteSpace(miscOrderDetail.Location) ? miscOrderDetail.Location : miscOrderMaster.Location) : null;
                    locationTransaction.LocationTo = miscOrderMaster.Type == CodeMaster.MiscOrderType.GI ? null : !string.IsNullOrWhiteSpace(miscOrderDetail.Location) ? miscOrderDetail.Location : miscOrderMaster.Location;
                    locationTransaction.LocationIOReason = miscOrderMaster.MoveType;
                    locationTransaction.EffectiveDate = effectiveDate;
                    locationTransaction.CreateUserId = SecurityContextHolder.Get().Id;
                    locationTransaction.CreateDate = dateTimeNow;

                    this.genericMgr.Create(locationTransaction);

                    RecordLocationTransactionDetail(locationTransaction, groupedInventoryTransaction.InventoryTransactionList);
                }
            }
        }
        #endregion

        #region 计划外出/入库冲销
        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> CancelInventoryOtherInOut(MiscOrderMaster miscOrderMaster)
        {
            return CancelInventoryOtherInOut(miscOrderMaster, DateTime.Now);
        }

        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> CancelInventoryOtherInOut(MiscOrderMaster miscOrderMaster, DateTime effectiveDate)
        {
            List<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();

            foreach (MiscOrderDetail miscOrderDetail in miscOrderMaster.MiscOrderDetails)
            {
                List<InventoryTransaction> detailInventoryTransactionList = new List<InventoryTransaction>();

                foreach (MiscOrderLocationDetail miscOrderLocationDetail in miscOrderDetail.MiscOrderLocationDetails)
                {
                    InventoryIO inventoryIO = new InventoryIO();

                    inventoryIO.Location = !string.IsNullOrWhiteSpace(miscOrderDetail.Location) ? miscOrderDetail.Location : miscOrderMaster.Location;
                    inventoryIO.Item = miscOrderLocationDetail.Item;
                    inventoryIO.Qty = -miscOrderLocationDetail.Qty;
                    inventoryIO.HuId = miscOrderLocationDetail.HuId;
                    inventoryIO.LotNo = miscOrderLocationDetail.LotNo;
                    inventoryIO.QualityType = miscOrderLocationDetail.QualityType;
                    inventoryIO.IsATP = miscOrderLocationDetail.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                    inventoryIO.IsFreeze = miscOrderLocationDetail.IsFreeze;
                    inventoryIO.IsCreatePlanBill = miscOrderLocationDetail.IsCreatePlanBill;
                    //inventoryIO.IsConsignment = miscOrderLocationDetail.IsConsignment;
                    inventoryIO.IsConsignment = miscOrderLocationDetail.ActingBill.HasValue ? true : false; //解决寄售物资计划外出库冲销记录库存丢失寄售信息
                    inventoryIO.PlanBill = miscOrderLocationDetail.PlanBill;
                    inventoryIO.ActingBill = miscOrderLocationDetail.ActingBill;
                    inventoryIO.TransactionType = miscOrderMaster.Type == CodeMaster.MiscOrderType.GI ? CodeMaster.TransactionType.ISS_UNP_VOID : CodeMaster.TransactionType.RCT_UNP_VOID;
                    inventoryIO.OccupyType = miscOrderLocationDetail.OccupyType;
                    inventoryIO.OccupyReferenceNo = miscOrderLocationDetail.OccupyReferenceNo;
                    inventoryIO.IsVoid = true;
                    inventoryIO.EffectiveDate = effectiveDate;
                    //inventoryIO.ManufactureParty = ;

                    IList<InventoryTransaction> locationDetailInventoryTransactionList = RecordInventory(inventoryIO);
                    if (miscOrderMaster.IsScanHu)
                    {
                        RecordLocationTransaction(miscOrderMaster, miscOrderDetail, effectiveDate, locationDetailInventoryTransactionList, true);
                    }
                    detailInventoryTransactionList.AddRange(locationDetailInventoryTransactionList);
                }

                if (!miscOrderMaster.IsScanHu)
                {
                    RecordLocationTransaction(miscOrderMaster, miscOrderDetail, effectiveDate, detailInventoryTransactionList, true);
                }
                inventoryTransactionList.AddRange(detailInventoryTransactionList);
            }

            return inventoryTransactionList;
        }
        #endregion



        #region 库存物料替换
        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> InventoryExchange(IList<ItemExchange> itemExchangeList)
        {
            if (itemExchangeList != null && itemExchangeList.Count > 0)
            {
                List<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();
                foreach (ItemExchange itemExchange in itemExchangeList)
                {
                    this.genericMgr.Create(itemExchange);

                    #region 出库
                    InventoryIO inventoryOut = new InventoryIO();

                    inventoryOut.Location = itemExchange.LocationFrom;
                    inventoryOut.Item = itemExchange.ItemFrom;
                    //inventoryOut.HuId = itemExchange.HuId;
                    inventoryOut.Qty = -itemExchange.Qty * itemExchange.UnitQty;  //转换为库存单位，为负数
                    //inventoryOut.LotNo = concessionDetail.LotNo;
                    //inventoryOut.ManufactureParty = null;
                    inventoryOut.QualityType = itemExchange.QualityType;
                    inventoryOut.IsATP = itemExchange.QualityType == CodeMaster.QualityType.Qualified;
                    inventoryOut.IsFreeze = false;
                    inventoryOut.IsCreatePlanBill = false;
                    inventoryOut.IsConsignment = false;
                    inventoryOut.PlanBill = null;
                    inventoryOut.ActingBill = null;
                    //inventoryOut.TransactionType = CodeMaster.TransactionType.ISS_IIC;
                    inventoryOut.OccupyType = CodeMaster.OccupyType.None;
                    inventoryOut.OccupyReferenceNo = null;
                    inventoryOut.IsVoid = false;
                    inventoryOut.EffectiveDate = itemExchange.EffectiveDate;
                    //inventoryOut.ManufactureParty = itemExchange.ManufactureParty;
                    //OutQty,OutUom,OutBaseUom,OutUnitQty,InHu,OutHu,OrderType 
                    inventoryOut.HuId = itemExchange.OldHu;
                    inventoryOut.LotNo = itemExchange.LotNo;

                    IList<InventoryTransaction> currentInventoryOutTransactionList = RecordInventory(inventoryOut);

                    RecordLocationTransaction(itemExchange, currentInventoryOutTransactionList, true, false);
                    inventoryTransactionList.AddRange(currentInventoryOutTransactionList);
                    #endregion

                    #region 入库
                    InventoryIO inventoryIn = new InventoryIO();

                    //OutQty,OutUom,OutBaseUom,OutUnitQty,OrderType 
                    inventoryIn.Location = itemExchange.LocationTo;
                    inventoryIn.Item = itemExchange.ItemTo;
                    //inventoryIn.HuId = itemExchange.HuId;
                    inventoryIn.Qty = itemExchange.NewQty * itemExchange.NewUnitQty;  //转换为库存单位
                    //inventoryIn.LotNo = itemExchange.LotNo;
                    //inventoryIn.ManufactureParty = null;
                    inventoryIn.QualityType = itemExchange.QualityType;
                    inventoryIn.IsATP = itemExchange.QualityType == CodeMaster.QualityType.Qualified;
                    inventoryIn.IsFreeze = false;
                    inventoryIn.IsCreatePlanBill = false;
                    inventoryIn.IsConsignment = false;
                    inventoryIn.PlanBill = null;
                    inventoryIn.ActingBill = null;
                    //inventoryIn.TransactionType = CodeMaster.TransactionType.RCT_IIC;
                    inventoryIn.OccupyType = CodeMaster.OccupyType.None;
                    inventoryIn.OccupyReferenceNo = null;
                    inventoryIn.IsVoid = false;
                    inventoryIn.EffectiveDate = itemExchange.EffectiveDate;
                    //inventoryIn.ManufactureParty = ;
                    inventoryIn.HuId = itemExchange.NewHu;
                    inventoryIn.LotNo = itemExchange.LotNo;

                    IList<InventoryTransaction> currentInventoryInTransactionList = RecordInventory(inventoryIn);

                    RecordLocationTransaction(itemExchange, currentInventoryInTransactionList, false, false);
                    inventoryTransactionList.AddRange(currentInventoryInTransactionList);
                    #endregion
                }

                return inventoryTransactionList;
            }

            return null;
        }

        private void RecordLocationTransaction(ItemExchange itemExchange, IList<InventoryTransaction> inventoryTransactionList, bool isIssue, bool isVoid)
        {
            //OutQty,OutUom,OutBaseUom,OutUnitQty,InHu,OutHu,OrderType 
            DateTime dateTimeNow = DateTime.Now;

            LocationTransaction locationTransaction = new LocationTransaction();

            //locationTransaction.OrderNo = ;
            //locationTransaction.OrderType = itemExchange.OrderType;
            //locationTransaction.OrderSubType = ;
            //locationTransaction.OrderDetailSequence = ;
            locationTransaction.OrderDetailId = itemExchange.Id;
            //locationTransaction.OrderBomDetId = 
            //locationTransaction.IpNo = 
            //locationTransaction.IpDetailId = 
            //locationTransaction.IpDetailSequence = 
            //locationTransaction.ReceiptNo = 
            //locationTransaction.ReceiptDetailId = 
            //locationTransaction.ReceiptDetailSequence = 
            //locationTransaction.SequenceNo = 
            //locationTransaction.TraceCode = ;
            locationTransaction.Item = isIssue ? itemExchange.ItemFrom : itemExchange.ItemTo;
            locationTransaction.Uom = isIssue ? itemExchange.Uom : itemExchange.NewUom;
            locationTransaction.BaseUom = isIssue ? itemExchange.BaseUom : itemExchange.NewBaseUom;
            locationTransaction.Qty = isIssue ? (-itemExchange.Qty / itemExchange.UnitQty) : (itemExchange.NewQty / itemExchange.NewUnitQty); //转为订单单位
            locationTransaction.UnitQty = isIssue ? itemExchange.UnitQty : itemExchange.NewUnitQty;
            locationTransaction.IsConsignment = false;
            //locationTransaction.PlanBill = null;
            //locationTransaction.ActingBill = null;
            //locationTransaction.ActingBillQty = 0;  //转为订单单位
            locationTransaction.QualityType = itemExchange.QualityType;
            locationTransaction.HuId = isIssue ? itemExchange.OldHu : itemExchange.NewHu;
            locationTransaction.LotNo = itemExchange.LotNo;

            #region 库存事务类型
            locationTransaction.LocationIOReason = itemExchange.ItemExchangeType.ToString();
            if (isIssue)
            {
                if (isVoid)
                {
                    locationTransaction.TransactionType = CodeMaster.TransactionType.ISS_IIC_VOID;
                }
                else
                {
                    locationTransaction.TransactionType = CodeMaster.TransactionType.ISS_IIC;
                }
            }
            else
            {
                if (isVoid)
                {
                    locationTransaction.TransactionType = CodeMaster.TransactionType.RCT_IIC_VOID;
                }
                else
                {
                    locationTransaction.TransactionType = CodeMaster.TransactionType.RCT_IIC;
                }
            }
            #endregion

            locationTransaction.IOType = isIssue ? CodeMaster.TransactionIOType.Out : CodeMaster.TransactionIOType.In;
            locationTransaction.PartyFrom = itemExchange.RegionFrom;
            locationTransaction.PartyTo = itemExchange.RegionTo;
            locationTransaction.LocationFrom = itemExchange.LocationFrom;
            locationTransaction.LocationTo = itemExchange.LocationTo;
            if (itemExchange.ItemExchangeType == CodeMaster.ItemExchangeType.Aging)
            {
                locationTransaction.LocationIOReason = "老化";
            }
            else if (itemExchange.ItemExchangeType == CodeMaster.ItemExchangeType.Filter)
            {
                locationTransaction.LocationIOReason = "过滤";
            }
            locationTransaction.EffectiveDate = itemExchange.EffectiveDate;
            locationTransaction.CreateUserId = SecurityContextHolder.Get().Id;
            locationTransaction.CreateDate = dateTimeNow;

            this.genericMgr.Create(locationTransaction);
            RecordLocationTransactionDetail(locationTransaction, inventoryTransactionList);
        }
        #endregion

        #region 库存物料替换冲销
        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> CancelInventoryExchange(IList<ItemExchange> itemExchangeList)
        {
            if (itemExchangeList != null && itemExchangeList.Count > 0)
            {

                foreach (ItemExchange itemExchange in itemExchangeList)
                {
                    if (!itemExchange.IsVoid)
                    {
                        itemExchange.IsVoid = true;
                        this.genericMgr.Update(itemExchange);
                    }
                    else
                    {
                        throw new BusinessException("库存物料替换已经冲销。");
                    }
                }

                List<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();
                foreach (ItemExchange itemExchange in itemExchangeList)
                {
                    #region 入库冲销
                    InventoryIO inventoryIn = new InventoryIO();

                    inventoryIn.Location = itemExchange.LocationTo;
                    inventoryIn.Item = itemExchange.ItemTo;
                    inventoryIn.HuId = itemExchange.NewHu;
                    inventoryIn.Qty = -itemExchange.NewQty * itemExchange.NewUnitQty; ;
                    inventoryIn.LotNo = itemExchange.LotNo;
                    //inventoryIn.ManufactureParty = null;
                    inventoryIn.QualityType = itemExchange.QualityType;
                    inventoryIn.IsATP = itemExchange.QualityType == CodeMaster.QualityType.Qualified;
                    inventoryIn.IsFreeze = false;
                    inventoryIn.IsCreatePlanBill = false;
                    inventoryIn.IsConsignment = false;
                    inventoryIn.PlanBill = null;
                    inventoryIn.ActingBill = null;
                    //inventoryIn.TransactionType = CodeMaster.TransactionType.RCT_IIC_VOID;
                    inventoryIn.OccupyType = CodeMaster.OccupyType.None;
                    inventoryIn.OccupyReferenceNo = null;
                    inventoryIn.IsVoid = true;
                    inventoryIn.EffectiveDate = itemExchange.EffectiveDate;
                    //inventoryOut.ManufactureParty = ;

                    IList<InventoryTransaction> currentInventoryInTransactionList = RecordInventory(inventoryIn);

                    RecordLocationTransaction(itemExchange, currentInventoryInTransactionList, false, true);
                    inventoryTransactionList.AddRange(currentInventoryInTransactionList);
                    #endregion

                    #region 出库
                    InventoryIO inventoryOut = new InventoryIO();

                    inventoryOut.Location = itemExchange.LocationFrom;
                    inventoryOut.Item = itemExchange.ItemFrom;
                    inventoryOut.HuId = itemExchange.OldHu;
                    inventoryOut.Qty = itemExchange.Qty * itemExchange.UnitQty;
                    inventoryOut.LotNo = itemExchange.LotNo;
                    //inventoryOut.ManufactureParty = null;
                    inventoryOut.QualityType = itemExchange.QualityType;
                    inventoryOut.IsATP = itemExchange.QualityType == CodeMaster.QualityType.Qualified;
                    inventoryOut.IsFreeze = false;
                    inventoryOut.IsCreatePlanBill = false;
                    inventoryOut.IsConsignment = false;
                    inventoryOut.PlanBill = null;
                    inventoryOut.ActingBill = null;
                    //inventoryOut.TransactionType = CodeMaster.TransactionType.ISS_IIC_VOID;
                    inventoryOut.OccupyType = CodeMaster.OccupyType.None;
                    inventoryOut.OccupyReferenceNo = null;
                    inventoryOut.IsVoid = true;
                    inventoryOut.EffectiveDate = itemExchange.EffectiveDate;
                    //inventoryOut.ManufactureParty = itemExchange.ManufactureParty;

                    IList<InventoryTransaction> currentInventoryOutTransactionList = RecordInventory(inventoryOut);

                    RecordLocationTransaction(itemExchange, currentInventoryOutTransactionList, true, true);
                    inventoryTransactionList.AddRange(currentInventoryOutTransactionList);
                    #endregion
                }

                return inventoryTransactionList;
            }

            return null;
        }
        #endregion

        #region 库存物料冻结
        [Transaction(TransactionMode.Requires)]
        public void InventoryFreeze(IList<string> huIdList)
        {
            if (huIdList == null || huIdList.Count == 0)
            {
                throw new BusinessException("冻结条码为空。");
            }

            IList<LocationLotDetail> locationLotDetailList = this.GetHuLocationLotDetails(huIdList);
            BusinessException businessException = new BusinessException();
            foreach (string huId in huIdList)
            {
                LocationLotDetail locationLotDetail = locationLotDetailList.Where(l => l.HuId == huId).SingleOrDefault();
                if (locationLotDetail == null)
                {
                    businessException.AddMessage("条码{0}不在库位中。", huId);
                    continue;
                }

                if (locationLotDetail.IsFreeze)
                {
                    businessException.AddMessage("条码{0}已经冻结。", huId);
                    continue;
                }

                if (locationLotDetail.QualityType == CodeMaster.QualityType.Inspect)
                {
                    businessException.AddMessage("条码{0}为待验品,不能冻结。", huId);
                }
                else if (locationLotDetail.QualityType == CodeMaster.QualityType.Reject)
                {
                    businessException.AddMessage("条码{0}为不合格品,不能冻结。", huId);
                }
            }

            if (businessException.HasMessage)
            {
                throw businessException;
            }

            foreach (LocationLotDetail locationLotDetail in locationLotDetailList)
            {
                locationLotDetail.IsFreeze = true;
                locationLotDetail.IsATP = false;
                this.genericMgr.Update(locationLotDetail);
                RecordFreezeTransaction(locationLotDetail);
            }
        }
        [Transaction(TransactionMode.Requires)]
        public void InventoryFreeze(IList<string> huIdList, string reason)
        {
            if (huIdList == null || huIdList.Count == 0)
            {
                throw new BusinessException("冻结条码为空。");
            }

            IList<LocationLotDetail> locationLotDetailList = this.GetHuLocationLotDetails(huIdList);
            BusinessException businessException = new BusinessException();
            foreach (string huId in huIdList)
            {
                LocationLotDetail locationLotDetail = locationLotDetailList.Where(l => l.HuId == huId).SingleOrDefault();
                if (locationLotDetail == null)
                {
                    businessException.AddMessage("条码{0}不在库位中。", huId);
                    continue;
                }

                if (locationLotDetail.IsFreeze)
                {
                    businessException.AddMessage("条码{0}已经冻结。", huId);
                    continue;
                }

                if (locationLotDetail.QualityType == CodeMaster.QualityType.Inspect)
                {
                    businessException.AddMessage("条码{0}为待验品,不能冻结。", huId);
                }
                else if (locationLotDetail.QualityType == CodeMaster.QualityType.Reject)
                {
                    businessException.AddMessage("条码{0}为不合格品,不能冻结。", huId);
                }
            }

            if (businessException.HasMessage)
            {
                throw businessException;
            }

            foreach (LocationLotDetail locationLotDetail in locationLotDetailList)
            {
                locationLotDetail.IsFreeze = true;
                locationLotDetail.IsATP = false;
                locationLotDetail.Reason = reason;
                locationLotDetail.ItemVersion = locationLotDetail.ItemVersion;
                this.genericMgr.Update(locationLotDetail);
                RecordFreezeTransaction(locationLotDetail);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void InventoryFreeze(string item, string location, string lotNo, string manufactureParty)
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                throw new TechnicalException("Item is null");
            }

            string hql = "from LocationLotDetail where QualityType=? and IsFreeze = ? and Item = ?";
            IList<object> parm = new List<object>();
            parm.Add(CodeMaster.QualityType.Qualified);
            parm.Add(false);
            parm.Add(item);

            if (!string.IsNullOrWhiteSpace(location))
            {
                hql += " and Location = ?";
                parm.Add(location);
            }

            if (!string.IsNullOrWhiteSpace(lotNo))
            {
                hql += " and LotNo = ?";
                parm.Add(lotNo);
            }

            if (!string.IsNullOrWhiteSpace(manufactureParty))
            {
                hql += " and ManufactureParty = ?";
                parm.Add(manufactureParty);
            }

            IList<LocationLotDetail> locationLotDetailList = this.genericMgr.FindAll<LocationLotDetail>(hql, parm.ToArray());

            if (locationLotDetailList != null && locationLotDetailList.Count > 0)
            {
                foreach (LocationLotDetail locationLotDetail in locationLotDetailList)
                {
                    locationLotDetail.IsFreeze = true;
                    locationLotDetail.IsATP = false;
                    this.genericMgr.Update(locationLotDetail);
                    RecordFreezeTransaction(locationLotDetail);
                }
            }
        }
        #endregion

        #region 库存物料解冻
        [Transaction(TransactionMode.Requires)]
        public void InventoryUnFreeze(IList<string> huIdList)
        {
            if (huIdList == null || huIdList.Count == 0)
            {
                throw new BusinessException("解冻条码为空。");
            }

            IList<LocationLotDetail> locationLotDetailList = this.GetHuLocationLotDetails(huIdList);
            BusinessException businessException = new BusinessException();
            foreach (string huId in huIdList)
            {
                LocationLotDetail locationLotDetail = locationLotDetailList.Where(l => l.HuId == huId).SingleOrDefault();
                if (locationLotDetail == null)
                {
                    businessException.AddMessage("条码{0}不在库位中。", huId);
                }
                else if (!locationLotDetail.IsFreeze)
                {
                    businessException.AddMessage("条码{0}没有冻结。", huId);
                }
            }

            if (businessException.HasMessage)
            {
                throw businessException;
            }

            foreach (LocationLotDetail locationLotDetail in locationLotDetailList)
            {
                locationLotDetail.IsFreeze = false;
                locationLotDetail.IsATP = true;
                this.genericMgr.Update(locationLotDetail);
                RecordFreezeTransaction(locationLotDetail);
            }
        }
        [Transaction(TransactionMode.Requires)]
        public void InventoryUnFreeze(IList<string> huIdList, string reason)
        {
            if (huIdList == null || huIdList.Count == 0)
            {
                throw new BusinessException("解冻条码为空。");
            }

            IList<LocationLotDetail> locationLotDetailList = this.GetHuLocationLotDetails(huIdList);
            BusinessException businessException = new BusinessException();
            foreach (string huId in huIdList)
            {
                LocationLotDetail locationLotDetail = locationLotDetailList.Where(l => l.HuId == huId).SingleOrDefault();
                if (locationLotDetail == null)
                {
                    businessException.AddMessage("条码{0}不在库位中。", huId);
                }
                else if (!locationLotDetail.IsFreeze)
                {
                    businessException.AddMessage("条码{0}没有冻结。", huId);
                }
            }

            if (businessException.HasMessage)
            {
                throw businessException;
            }

            foreach (LocationLotDetail locationLotDetail in locationLotDetailList)
            {
                locationLotDetail.IsFreeze = false;
                locationLotDetail.IsATP = true;
                locationLotDetail.Reason = reason;
                this.genericMgr.Update(locationLotDetail);
                RecordFreezeTransaction(locationLotDetail);
            }
        }
        [Transaction(TransactionMode.Requires)]
        public void InventoryUnFreeze(string item, string location, string lotNo, string manufactureParty)
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                throw new TechnicalException("Item is null");
            }

            string hql = "from LocationLotDetail where QualityType=? and IsFreeze = ? and Item = ?";
            IList<object> parm = new List<object>();
            parm.Add(CodeMaster.QualityType.Qualified);
            parm.Add(true);
            parm.Add(item);

            if (!string.IsNullOrWhiteSpace(location))
            {
                hql += " and Location = ?";
                parm.Add(location);
            }

            if (!string.IsNullOrWhiteSpace(lotNo))
            {
                hql += " and LotNo = ?";
                parm.Add(lotNo);
            }

            if (!string.IsNullOrWhiteSpace(manufactureParty))
            {
                hql += " and ManufactureParty = ?";
                parm.Add(manufactureParty);
            }

            IList<LocationLotDetail> locationLotDetailList = this.genericMgr.FindAll<LocationLotDetail>(hql, parm.ToArray());

            if (locationLotDetailList != null && locationLotDetailList.Count > 0)
            {
                foreach (LocationLotDetail locationLotDetail in locationLotDetailList)
                {
                    locationLotDetail.IsFreeze = false;
                    locationLotDetail.IsATP = true;
                    this.genericMgr.Update(locationLotDetail);
                    RecordFreezeTransaction(locationLotDetail);
                }
            }
        }

        private void RecordFreezeTransaction(LocationLotDetail locationLotDetail)
        {
            User user = SecurityContextHolder.Get();
            FreezeTransaction freezeTransaction = new FreezeTransaction();
            freezeTransaction.CreateDate = DateTime.Now;
            freezeTransaction.CreateUserName = user.FullName;
            freezeTransaction.CreateUserId = user.Id;
            freezeTransaction.Freeze = locationLotDetail.IsFreeze;
            freezeTransaction.HuId = locationLotDetail.HuId;
            freezeTransaction.Location = locationLotDetail.Location;
            freezeTransaction.Item = locationLotDetail.Item;
            freezeTransaction.LotNo = locationLotDetail.LotNo;
            freezeTransaction.Bin = locationLotDetail.Bin;
            freezeTransaction.Reason = locationLotDetail.Reason;
            freezeTransaction.ItemVersion = locationLotDetail.ItemVersion;
            this.genericMgr.Create(freezeTransaction);
        }
        #endregion

        #region 客户化代码
        [Transaction(TransactionMode.Requires)]
        public IList<InventoryTransaction> InventoryRePack(IList<InventoryRePack> inventoryRePackList, Boolean isCheckOccupy, DateTime effectiveDate)
        {
            IList<HuStatus> huStatusList = this.huMgr.GetHuStatus(inventoryRePackList.Select(i => i.HuId).ToList());

            #region 检验
            BusinessException businessException = new BusinessException();

            foreach (InventoryRePack inventoryRePack in inventoryRePackList)
            {
                HuStatus huStatus = huStatusList.Where(h => h.HuId == inventoryRePack.HuId).SingleOrDefault();

                if (huStatus == null)
                {
                    businessException.AddMessage("条码{0}不存在。", huStatus.HuId);
                }

                if (inventoryRePack.Type == CodeMaster.RePackType.Out)
                {
                    if (huStatus.Status == CodeMaster.HuStatus.Ip)
                    {
                        businessException.AddMessage("条码{0}为库位{1}至库位{2}的在途库存，不能翻箱。", huStatus.HuId, huStatus.LocationFrom, huStatus.LocationTo);
                    }

                    if (huStatus.Status == CodeMaster.HuStatus.NA)
                    {
                        businessException.AddMessage("条码{0}已经不在任何库位中，不能翻箱。", huStatus.HuId);
                    }

                    if (isCheckOccupy == true)
                    {
                        if (huStatus.OccupyType != CodeMaster.OccupyType.None)
                        {
                            businessException.AddMessage("条码{0}已经被占用，不能翻箱。", huStatus.HuId);
                        }
                    }
                }
                else
                {
                    if (huStatus.Status == CodeMaster.HuStatus.Location)
                    {
                        businessException.AddMessage("条码{0}已经在库位{1}中，不能翻箱。", huStatus.HuId, huStatus.Location);
                    }

                    if (huStatus.Status == CodeMaster.HuStatus.Ip)
                    {
                        businessException.AddMessage("条码{0}为库位{1}至库位{2}的在途库存，不能翻箱。", huStatus.HuId, huStatus.LocationFrom, huStatus.LocationTo);
                    }
                }
                inventoryRePack.Location = huStatus.Location;
                inventoryRePack.CurrentHu = huStatus;
            }

            if (inventoryRePackList.Where(i => i.Type == CodeMaster.RePackType.Out).Select(i => i.Location).Distinct().Count() > 1)
            {
                businessException.AddMessage("翻箱前条码的库位不一致。");
            }

            if (inventoryRePackList.Select(i => i.CurrentHu.Item).Distinct().Count() > 1)
            {
                businessException.AddMessage("翻箱前后的物料不一致。");
            }

            if (inventoryRePackList.Where(i => i.Type == CodeMaster.RePackType.Out).Sum(i => i.CurrentHu.Qty * i.CurrentHu.UnitQty)
                 != inventoryRePackList.Where(i => i.Type == CodeMaster.RePackType.In).Sum(i => i.CurrentHu.Qty * i.CurrentHu.UnitQty))
            {
                businessException.AddMessage("翻箱前的条码数量和翻箱后的条码数量不一致。");
            }

            if (businessException.HasMessage)
            {
                throw businessException;
            }
            #endregion

            #region 查找库位
            Location location = this.genericMgr.FindById<Location>(inventoryRePackList.Where(i => i.Type == CodeMaster.RePackType.Out).Select(i => i.Location).First());
            //IList<Location> locationList = BatchLoadLocations(inventoryRePackList.Where(i => i.Type == CodeMaster.RePackType.Out).Select(i => i.Location).ToList());
            #endregion

            #region 循环拆箱
            IList<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();

            #region 出库
            IList<InventoryTransaction> totalIssInventoryTransactionList = new List<InventoryTransaction>();
            foreach (InventoryRePack inventoryRePack in inventoryRePackList.Where(i => i.Type == CodeMaster.RePackType.Out))
            {
                inventoryRePack.CurrentLocation = location;

                InventoryIO inventoryOut = new InventoryIO();

                inventoryOut.Location = inventoryRePack.CurrentHu.Location;
                inventoryOut.Item = inventoryRePack.CurrentHu.Item;
                inventoryOut.HuId = inventoryRePack.CurrentHu.HuId;
                inventoryOut.Qty = -inventoryRePack.CurrentHu.Qty * inventoryRePack.CurrentHu.UnitQty;  //转换为库存单位
                inventoryOut.LotNo = inventoryRePack.CurrentHu.LotNo;
                inventoryOut.QualityType = inventoryRePack.CurrentHu.QualityType;
                inventoryOut.IsATP = inventoryRePack.CurrentHu.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                inventoryOut.IsFreeze = inventoryRePack.CurrentHu.IsFreeze;
                inventoryOut.IsCreatePlanBill = false;
                inventoryOut.IsConsignment = inventoryRePack.CurrentHu.IsConsignment;
                inventoryOut.PlanBill = inventoryRePack.CurrentHu.PlanBill;
                inventoryOut.ActingBill = null;
                inventoryOut.TransactionType = CodeMaster.TransactionType.ISS_REP;
                inventoryOut.OccupyType = inventoryRePack.CurrentHu.OccupyType;
                inventoryOut.OccupyReferenceNo = inventoryRePack.CurrentHu.OccupyReferenceNo;
                inventoryOut.IsVoid = false;
                inventoryOut.EffectiveDate = effectiveDate;
                //inventoryIO.ManufactureParty = ;

                IList<InventoryTransaction> issInventoryTransactionList = RecordInventory(inventoryOut);

                #region 如果翻箱的出库库存明细有寄售非寄售混杂或者有多条寄售信息，立即结算
                //寄售翻箱不能立即结算
                //TryManualSettleBill(totalIssInventoryTransactionList, effectiveDate);
                #endregion

                ((List<InventoryTransaction>)totalIssInventoryTransactionList).AddRange(issInventoryTransactionList);
                ((List<InventoryTransaction>)inventoryTransactionList).AddRange(issInventoryTransactionList);
                RecordLocationTransaction(inventoryRePack, effectiveDate, issInventoryTransactionList, true);
            }
            #endregion

            #region 入库
            foreach (InventoryRePack inventoryRePack in inventoryRePackList.Where(i => i.Type == CodeMaster.RePackType.In))
            {
                inventoryRePack.CurrentLocation = location;

                InventoryIO inventoryIn = new InventoryIO();

                inventoryIn.Location = location.Code;
                inventoryIn.Item = inventoryRePack.CurrentHu.Item;
                inventoryIn.HuId = inventoryRePack.CurrentHu.HuId;
                inventoryIn.LotNo = inventoryRePack.CurrentHu.LotNo;
                inventoryIn.Qty = inventoryRePack.CurrentHu.Qty * inventoryRePack.CurrentHu.UnitQty;  //转换为库存单位
                inventoryIn.QualityType = inventoryRePack.CurrentHu.QualityType;
                inventoryIn.IsATP = inventoryRePack.CurrentHu.QualityType == com.Sconit.CodeMaster.QualityType.Qualified;
                inventoryIn.IsFreeze = inventoryRePack.CurrentHu.IsFreeze;
                inventoryIn.IsCreatePlanBill = false;
                inventoryIn.IsConsignment = totalIssInventoryTransactionList.Select(i => i.IsConsignment).First();
                inventoryIn.PlanBill = inventoryIn.IsConsignment ? totalIssInventoryTransactionList[0].PlanBill : null;
                inventoryIn.ActingBill = null;
                inventoryIn.TransactionType = CodeMaster.TransactionType.RCT_REP;
                inventoryIn.OccupyType = inventoryRePack.CurrentHu.OccupyType;
                inventoryIn.OccupyReferenceNo = inventoryRePack.CurrentHu.OccupyReferenceNo;
                inventoryIn.IsVoid = false;
                inventoryIn.EffectiveDate = effectiveDate;
                //inventoryIO.ManufactureParty = ;

                IList<InventoryTransaction> rctInventoryTransactionList = RecordInventory(inventoryIn);
                ((List<InventoryTransaction>)inventoryTransactionList).AddRange(rctInventoryTransactionList);
                RecordLocationTransaction(inventoryRePack, effectiveDate, rctInventoryTransactionList, false);
            }
            #endregion

            return inventoryTransactionList;
            #endregion
        }


        /// <summary>
        /// 客戶化拆箱功能
        /// </summary>
        /// <param name="huList"></param>
        /// <returns></returns>
        [Transaction(TransactionMode.Requires)]
        public List<Hu> DevanningHu(Hu hu)
        {

            List<Hu> devanningHuList = new List<Hu>();
            List<string> oldHuList = new List<string>();
            List<string> newHuList = new List<string>();

           oldHuList.Add(hu.HuId);
           decimal remainQty = hu.Qty;
           string[] qtyStrArray = hu.DevanningQtyStr.Split(',');

           foreach (string qtyStr in qtyStrArray)
           {
               Hu newHu = huMgr.CloneHu(hu, Convert.ToDecimal(qtyStr));
               genericMgr.Create(newHu);
               newHuList.Add(newHu.HuId);
               remainQty -= newHu.Qty;

               if (remainQty < 0)
               {
                   throw new BusinessException("拆箱数总和大于原条码数");
               }

               devanningHuList.Add(newHu);
           }

           #region 剩余打一张条码
           if (remainQty > 0)
           {
               Hu remainHu = huMgr.CloneHu(hu, remainQty);
               genericMgr.Create(remainHu);
               newHuList.Add(remainHu.HuId);
               devanningHuList.Add(remainHu);
           }
           #endregion


           genericMgr.FlushSession();

            var inventoryPackList = new List<Entity.INV.InventoryRePack>();
            foreach (var huId in oldHuList)
            {
                var inventoryUnPack = new Entity.INV.InventoryRePack();
                inventoryUnPack.HuId = huId;
                inventoryUnPack.Type = CodeMaster.RePackType.Out;
                inventoryPackList.Add(inventoryUnPack);
            }
            foreach (var huId in newHuList)
            {
                var inventoryUnPack = new Entity.INV.InventoryRePack();
                inventoryUnPack.HuId = huId;
                inventoryUnPack.Type = CodeMaster.RePackType.In;
                inventoryPackList.Add(inventoryUnPack);
            }

            InventoryRePack(inventoryPackList);


            return devanningHuList;
        }
        #endregion

        #region 收发存
        public IList<LocationDetailIOB> GetLocationDetailIOB(string location, string item, DateTime startDate, DateTime endDate)
        {
            var iobList = new ConcurrentBag<LocationDetailIOB>();

            string hqlInv = @"select l.Item,l.Location,
                              sum(l.Qty) as Qty,sum(l.QualifyQty) as QualifyQty,sum(l.InspectQty) as InspectQty,sum(l.RejectQty) as RejectQty,
                              i.Uom,i.Desc1,i.RefCode,Loc.Name,sum(CsQty) As CsQty
                              from VIEW_LocationDet as l 
                              join MD_Item as i on l.Item = i.Code 
                              join MD_Location as loc on l.Location = loc.Code
                              where 1=1 ";
            string hqlStart = @"select l.Item,
	                            case when l.IOType=1 then l.LocFrom else l.LocTo end as Location,
	                            l.TransType,sum(l.Qty*l.UnitQty) as Qty,QualityType, sum(Case when iscs=1 then 1 else 0 end*l.Qty*l.UnitQty) as CsQty
	                            from VIEW_LocTrans l where l.EffDate >= ? and l.EffDate < ? ";
            string hqlEnd = @" select l.Item,
                            case when l.IOType=1 then l.LocFrom else l.LocTo end as Location,
                            l.TransType,sum(l.Qty*l.UnitQty) as Qty,QualityType,sum(Case when iscs=1 then 1 else 0 end*l.Qty*l.UnitQty) as CsQty
                            from VIEW_LocTrans l where l.EffDate >= ? ";

            var paramInv = new List<object>();
            var paramStart = new List<object>();
            paramStart.Add(startDate);
            paramStart.Add(endDate);
            var paramEnd = new List<object>();
            paramEnd.Add(endDate);

            if (!string.IsNullOrWhiteSpace(item))
            {
                hqlInv += " and l.Item =? ";
                paramInv.Add(item);
                hqlStart += " and l.Item =? ";
                paramStart.Add(item);
                hqlEnd += " and l.Item =? ";
                paramEnd.Add(item);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                hqlInv += " and l.Location =? ";
                paramInv.Add(location);
                hqlStart += "and ((l.IOType=1 and l.LocFrom =?) or (l.IOType=0 and l.LocTo =?)) ";
                paramStart.Add(location);
                paramStart.Add(location);
                hqlEnd += " and ((l.IOType=1 and l.LocFrom =?) or (l.IOType=0 and l.LocTo =?))  ";
                paramEnd.Add(location);
                paramEnd.Add(location);
            }

            hqlInv += " group by l.Item,l.Location,i.Uom,i.Desc1,i.RefCode,Loc.Name order by l.Location,l.Item ";
            hqlStart += " group by l.Item,l.TransType,l.LocFrom,l.LocTo,l.IOType,QualityType ";
            hqlEnd += " group by l.Item,l.TransType,l.LocFrom,l.LocTo,l.IOType,QualityType ";

            var invList = this.genericMgr.FindAllWithNativeSql<object[]>(hqlInv, paramInv.ToArray())
                .Select(p => new
                {
                    Item = (string)p[0],
                    Location = (string)p[1],
                    ItemDescription = (string)p[7] + (string.IsNullOrWhiteSpace((string)p[8]) ? string.Empty : "[" + (string)p[8] + "]"),
                    LocationName = (string)p[9],
                    Uom = (string)p[6],
                    //当前库存
                    Qty = (decimal)p[2],
                    QualifyQty = (decimal)p[3],
                    InspectQty = (decimal)p[4],
                    RejectQty = (decimal)p[5],
                    CsQty = (decimal)p[10],
                }).ToList();

            var invStartListObj = this.genericMgr.FindAllWithNativeSql<object[]>(hqlStart, paramStart.ToArray())
                .Select(p => new
                {
                    Item = (string)p[0],
                    Location = (string)p[1],
                    TransactionType = (object)p[2],
                    Qty = (decimal)p[3],
                    QualityType = (object)p[4],
                    CSQty = (decimal)p[5]
                });
            if (invStartListObj != null)
            {
                foreach (var obj in invStartListObj)
                {
                    if (true)
                    {
                    }
                }
            }
            var invEndListObj = this.genericMgr.FindAllWithNativeSql<object[]>(hqlEnd, paramEnd.ToArray())
                .Select(p => new
                {
                    Item = (string)p[0],
                    Location = (string)p[1],
                    TransactionType = (object)p[2],
                    Qty = (decimal)p[3],
                    QualityType = (object)p[4],
                    CSQty = (decimal)p[5]
                });
            var invStartList = new List<LocTransaction>();
            foreach (var invStart in invStartListObj)
            {
                LocTransaction invData = new LocTransaction();
                invData.Item = invStart.Item;
                invData.Location = invStart.Location;
                invData.Qty = invStart.Qty;
                invData.CSQty = invStart.CSQty;
                invData.QualityType = (CodeMaster.QualityType)(int.Parse(invStart.QualityType.ToString()));
                invData.TransactionType = (CodeMaster.TransactionType)(int.Parse(invStart.TransactionType.ToString()));
                invStartList.Add(invData);
            }
            var invEndList = new List<LocTransaction>();
            foreach (var invEnd in invEndListObj)
            {
                LocTransaction invData = new LocTransaction();
                invData.Item = invEnd.Item;
                invData.Location = invEnd.Location;
                invData.Qty = invEnd.Qty;
                invData.CSQty = invEnd.CSQty;
                invData.QualityType = (CodeMaster.QualityType)(int.Parse(invEnd.QualityType.ToString()));
                invData.TransactionType = (CodeMaster.TransactionType)(int.Parse(invEnd.TransactionType.ToString()));
                invEndList.Add(invData);
            }
            var locationItem = invStartList.GroupBy(p => new { p.Item, p.Location })
                .Union(invEndList.GroupBy(p => new { p.Item, p.Location }))
                .GroupBy(p => new { p.Key.Item, p.Key.Location }).Select(p => p.First());

            var additionInv = from l in locationItem
                              join k in invList
                              on new { l.Key.Item, l.Key.Location } equals new { k.Item, k.Location }
                              into result
                              from p in result.DefaultIfEmpty()
                              where p == null
                              select new
                              {
                                  Item = l.Key.Item,
                                  Location = l.Key.Location,
                                  ItemDescription = itemMgr.GetCacheItem(l.Key.Item).FullDescription,
                                  LocationName = genericMgr.FindById<Location>(l.Key.Location).Name,
                                  Uom = itemMgr.GetCacheItem(l.Key.Item).Uom,
                                  //当前库存
                                  Qty = 0M,
                                  QualifyQty = 0M,
                                  InspectQty = 0M,
                                  RejectQty = 0M,
                                  CsQty = 0M
                              };
            invList.AddRange(additionInv);

            var totalIob = new LocationDetailIOB();
            iobList.Add(totalIob);
            Parallel.ForEach(invList, inv =>
            //foreach (var inv in invList)
            {
                var iob = new LocationDetailIOB();
                iob.Item = inv.Item;
                iob.ItemDescription = inv.ItemDescription;
                iob.Location = inv.Location;
                iob.LocationName = inv.LocationName;
                iob.Uom = inv.Uom;
                //当前库存
                iob.Start = inv.Qty;
                iob.StartNml = inv.QualifyQty;
                iob.StartInp = inv.InspectQty;
                iob.StartRej = inv.RejectQty;
                iob.StartCs = inv.CsQty;

                #region 倒推出期末库存
                foreach (var invEnd in invEndList)
                {
                    if (invEnd.Item == inv.Item && invEnd.Location == inv.Location)
                    {
                        //var transType = invEnd.TransactionType;
                        //decimal qty = invEnd.Qty;
                        //var qualityType = invEnd.QualityType;
                        //this.SetLocationDetailIOB(totalIob, iob, transType, qualityType, qty);
                        switch (invEnd.QualityType)
                        {
                            case CodeMaster.QualityType.Inspect:
                                iob.StartInp -= invEnd.Qty;
                                break;
                            case CodeMaster.QualityType.Qualified:
                                iob.StartNml -= invEnd.Qty;
                                break;
                            case CodeMaster.QualityType.Reject:
                                iob.StartRej -= invEnd.Qty;
                                break;
                        }
                        iob.Start -= invEnd.Qty;
                        iob.StartCs -= invEnd.CSQty;
                    }
                }

                //期末库存
                iob.End = iob.Start;
                iob.EndCs = iob.StartCs;
                iob.EndNml = iob.StartNml;
                iob.EndInp = iob.StartInp;
                iob.EndRej = iob.StartRej;
                #endregion

                //继续倒推得出期初库存和收发量
                foreach (var invStart in invStartList)
                {
                    if (invStart.Item == inv.Item && invStart.Location == inv.Location)
                    {
                        var transType = invStart.TransactionType;
                        decimal qty = invStart.Qty;
                        decimal csqty = invStart.CSQty;
                        var qualityType = invStart.QualityType;
                        this.SetLocationDetailIOB(totalIob, iob, transType, qualityType, qty, csqty);
                    }
                }
                iobList.Add(iob);
            }
            );
            return iobList.OrderBy(p => p.Item).ToList();
        }

        private void SetLocationDetailIOB(LocationDetailIOB totalIob, LocationDetailIOB iob, CodeMaster.TransactionType transType, CodeMaster.QualityType qualityType, decimal qty, decimal csqty)
        {
            #region  库存事务
            /*
                other	LOC_INI	100	//库存初始化	
                issso	ISS_SO	101	//销售出库	
                issso	ISS_SO_VOID	102	//销售出库冲销	
                rctso	RCT_SO	103	//销售退货入库	
                rctso	RCT_SO_VOID	104	//销售退货入库冲销	
                rctpo	RCT_PO	201	//采购入库	
                rctpo	RCT_PO_VOID	202	//采购入库冲销	
                isspo	ISS_PO	203	//采购退货	
                isspo	ISS_PO_VOID	204	//采购退货冲销	
                rctpo	RCT_SL	205	//计划协议入库	
                rctpo	RCT_SL_VOID	206	//计划协议入库冲销	
                isspo	ISS_SL	207	//计划协议退货	
                isspo	ISS_SL_VOID	208	//计划协议退货冲销	
                isstr	ISS_TR	301	//移库出库	
                isstr	ISS_TR_VOID	302	//移库出库冲销	
                rcttr	RCT_TR	303	//移库入库	
                rcttr	RCT_TR_VOID	304	//移库入库冲销	
                isstr	ISS_TR_RTN	311	//移库退货出库	
                isstr	ISS_TR_RTN_VOID	312	//移库退货出库冲销	
                rcttr	RCT_TR_RTN	313	//移库退货入库	
                rcttr	RCT_TR_RTN_VOID	314	//移库退货入库冲销	
                issstr	ISS_STR	305	//委外移库出库	
                issstr	ISS_STR_VOID	306	//委外移库出库冲销	
                rctstr	RCT_STR	307	//委外移库入库	
                rctstr	RCT_STR_VOID	308	//委外移库入库冲销	
                issstr	ISS_STR_RTN	315	//委外移库退货出库	
                issstr	ISS_STR_RTN_VOID	316	//委外移库退货出库冲销	
                rctstr	RCT_STR_RTN	317	//委外移库退货入库	
                rctstr	RCT_STR_RTN_VOID	318	//委外移库退货入库冲销	
                isswo	ISS_WO	401	//生产出库/原材料	
                isswo	ISS_WO_VOID	402	//生产出库/原材料冲销	
                isswo	ISS_WO_BF	403	//生产投料回冲出库/出生产线	
                isswo	ISS_WO_BF_VOID	404	//生产投料回冲出库/出生产线冲销	
                rctwo	RCT_WO	405	//生产入库/成品	
                rctwo	RCT_WO_VOID	406	//生产入库/成品冲销	
                -	ISS_MIN	407	//生产投料出库	
                -	ISS_MIN_RTN	408	//生产投料退库出库	
                -	RCT_MIN	409	//生产投料入库/入生产线	
                -	RCT_MIN_RTN	410	//生产投料出库/出生产线	
                issswo	ISS_SWO	411	//委外生产出库/原材料	
                issswo	ISS_SWO_VOID	412	//委外生产出库/原材料冲销	
                issswo	ISS_SWO_BF	413	//委外生产投料回冲出库/出生产线	
                issswo	ISS_SWO_BF_VOID	414	//委外生产投料回冲出库/出生产线冲销	
                rctswo	RCT_SWO	415	//委外生产入库/成品	
                rctswo	RCT_SWO_VOID	416	//委外生产入库/成品冲销	
                -	ISS_INP	501	//报验出库	
                inp	RCT_INP	502	//报验入库	
                -	ISS_ISL	503	//隔离出库	
                inp	RCT_ISL	504	//隔离入库	
                -	ISS_INP_QDII	505	//检验合格出库	
                qdii	RCT_INP_QDII	506	//检验合格入库	
                -	ISS_INP_REJ	507	//检验不合格出库	
                rej	RCT_INP_REJ	508	//检验不合格入库	
                -	ISS_INP_CCS	509	//让步使用出库	
                css	RCT_INP_CCS	510	//让步使用入库	
                cyccnt	CYC_CNT	601	//盘点差异出入库	
                cyccnt	CYC_CNT_VOID	602	//盘点差异出入库	
                issunp	ISS_UNP	603	//计划外出库	
                issunp	ISS_UNP_VOID	604	//计划外出库冲销	
                rctunp	RCT_UNP	605	//计划外入库	
                rctunp	RCT_UNP_VOID	606	//计划外入库冲销	
                -	ISS_REP	607	//翻箱出库	
                -	RCT_REP	608	//翻箱入库	
                -	ISS_PUT	609	//上架出库	
                -	RCT_PUT	610	//上架入库	
                -	ISS_PIK	611	//下架出库	
                -	RCT_PIK	612	//下架入库	
                issiic	ISS_IIC	613	//库存物料替换出库	
                issiic	ISS_IIC_VOID	614	//库存物料替换出库冲销	
                rctiic	RCT_IIC	615	//库存物料替换入库	
                rctiic	RCT_IIC_VOID	616	//库存物料替换入库冲销	
                -	ISS_AGE	901	//老化出库	
                -	ISS_AGE_VOID	902	//老化出库冲销	
                -	RCT_AGE	903	//老化入库	
                -	RCT_AGE_VOID	904	//老化入库冲销	
                issiic	ISS_FLT	905	//过滤出库	
                issiic	ISS_FLT_VOID	906	//过滤出库冲销	
                rctiic	RCT_FLT	907	//过滤入库	
                rctiic	RCT_FLT_VOID	908	//过滤入库冲销	
                */
            #endregion

            switch (transType)
            {
                case CodeMaster.TransactionType.CYC_CNT:
                case CodeMaster.TransactionType.CYC_CNT_VOID:
                    iob.CycCnt += qty;
                    totalIob.CycCnt++;
                    break;
                case CodeMaster.TransactionType.ISS_IIC:
                case CodeMaster.TransactionType.ISS_IIC_VOID:
                    iob.IssIic += qty;
                    totalIob.IssIic++;
                    break;
                case CodeMaster.TransactionType.ISS_PO:
                case CodeMaster.TransactionType.ISS_PO_VOID:
                case CodeMaster.TransactionType.ISS_SL:
                case CodeMaster.TransactionType.ISS_SL_VOID:
                    iob.IssPo += qty;
                    totalIob.IssPo++;
                    break;
                case CodeMaster.TransactionType.ISS_SO:
                case CodeMaster.TransactionType.ISS_SO_VOID:
                    iob.IssSo += qty;
                    totalIob.IssSo++;
                    break;
                case CodeMaster.TransactionType.ISS_STR:
                case CodeMaster.TransactionType.ISS_STR_VOID:
                case CodeMaster.TransactionType.ISS_STR_RTN:
                case CodeMaster.TransactionType.ISS_STR_RTN_VOID:
                    iob.IssStr += qty;
                    totalIob.IssStr++;
                    break;
                case CodeMaster.TransactionType.ISS_SWO:
                case CodeMaster.TransactionType.ISS_SWO_VOID:
                case CodeMaster.TransactionType.ISS_SWO_BF:
                case CodeMaster.TransactionType.ISS_SWO_BF_VOID:
                case CodeMaster.TransactionType.ISS_SWO_RTN:
                case CodeMaster.TransactionType.ISS_SWO_RTN_VOID:
                    iob.IssSwo += qty;
                    totalIob.IssSwo++;
                    break;
                case CodeMaster.TransactionType.ISS_TR:
                case CodeMaster.TransactionType.ISS_TR_VOID:
                case CodeMaster.TransactionType.ISS_TR_RTN:
                case CodeMaster.TransactionType.ISS_TR_RTN_VOID:
                    iob.IssTr += qty;
                    totalIob.IssTr++;
                    break;
                case CodeMaster.TransactionType.ISS_UNP:
                case CodeMaster.TransactionType.ISS_UNP_VOID:
                    iob.IssUnp += qty;
                    totalIob.IssUnp++;
                    break;
                case CodeMaster.TransactionType.ISS_WO:
                case CodeMaster.TransactionType.ISS_WO_VOID:
                case CodeMaster.TransactionType.ISS_WO_BF:
                case CodeMaster.TransactionType.ISS_WO_BF_VOID:
                case CodeMaster.TransactionType.ISS_WO_RTN:
                case CodeMaster.TransactionType.ISS_WO_RTN_VOID:
                    iob.IssWo += qty;
                    totalIob.IssWo++;
                    break;
                case CodeMaster.TransactionType.RCT_IIC:
                case CodeMaster.TransactionType.RCT_IIC_VOID:
                    iob.RctIic += qty;
                    totalIob.RctIic++;
                    break;
                case CodeMaster.TransactionType.RCT_PO:
                case CodeMaster.TransactionType.RCT_PO_VOID:
                case CodeMaster.TransactionType.RCT_SL:
                case CodeMaster.TransactionType.RCT_SL_VOID:
                    iob.RctPo += qty;
                    totalIob.RctPo++;
                    break;
                case CodeMaster.TransactionType.RCT_SO:
                case CodeMaster.TransactionType.RCT_SO_VOID:
                    iob.RctSo += qty;
                    totalIob.RctSo++;
                    break;
                case CodeMaster.TransactionType.RCT_STR:
                case CodeMaster.TransactionType.RCT_STR_VOID:
                case CodeMaster.TransactionType.RCT_STR_RTN:
                case CodeMaster.TransactionType.RCT_STR_RTN_VOID:
                    iob.RctStr += qty;
                    totalIob.RctStr++;
                    break;
                case CodeMaster.TransactionType.RCT_SWO:
                case CodeMaster.TransactionType.RCT_SWO_VOID:
                case CodeMaster.TransactionType.RCT_SWO_RTN:
                case CodeMaster.TransactionType.RCT_SWO_RTN_VOID:
                    iob.RctSwo += qty;
                    totalIob.RctSwo++;
                    break;
                case CodeMaster.TransactionType.RCT_TR:
                case CodeMaster.TransactionType.RCT_TR_VOID:
                case CodeMaster.TransactionType.RCT_TR_RTN:
                case CodeMaster.TransactionType.RCT_TR_RTN_VOID:
                    iob.RctTr += qty;
                    totalIob.RctTr++;
                    break;
                case CodeMaster.TransactionType.RCT_UNP:
                case CodeMaster.TransactionType.RCT_UNP_VOID:
                    iob.RctUnp += qty;
                    totalIob.RctUnp++;
                    break;
                case CodeMaster.TransactionType.RCT_WO:
                case CodeMaster.TransactionType.RCT_WO_VOID:
                case CodeMaster.TransactionType.RCT_WO_RTN:
                case CodeMaster.TransactionType.RCT_WO_RTN_VOID:
                    iob.RctWo += qty;
                    totalIob.RctWo++;
                    break;
                //其他-库存初始化
                case CodeMaster.TransactionType.LOC_INI:
                    iob.Other += qty;
                    totalIob.Other++;
                    break;
                default:
                    iob.Other += qty;
                    totalIob.Other++;
                    break;
            }
            switch (qualityType)
            {
                case CodeMaster.QualityType.Inspect:
                    iob.StartInp -= qty;
                    break;
                case CodeMaster.QualityType.Qualified:
                    iob.StartNml -= qty;
                    break;
                case CodeMaster.QualityType.Reject:
                    iob.StartRej -= qty;
                    break;
            }
            iob.Start -= qty;
            iob.StartCs -= csqty;
        }

        #endregion

        public Dictionary<string, decimal> GetInvATPQty(string location, List<string> items)
        {
            string hqlInv = string.Empty;
            foreach (var item in items)
            {
                if (hqlInv == string.Empty)
                {
                    hqlInv = @"select l.Item,sum(l.ATPQty) as ATPQty from VIEW_LocationDet as l with(nolock) where location =? and item in(?";
                }
                else
                {
                    hqlInv += ",?";
                }
            }
            hqlInv += @") group by l.Item";
            var paramList = new List<string>();
            paramList.Add(location);
            paramList.AddRange(items);
            return (this.genericMgr.FindAllWithNativeSql<object[]>(hqlInv, paramList.ToArray()) ?? new List<object[]>())
                  .ToDictionary(d => (string)d[0], d => (decimal)d[1]);
        }

        public Dictionary<string, decimal> GetPurchaseInTransQty(string location, List<string> items)
        {
            var paramList = new List<string>();
            paramList.Add(location);
            paramList.AddRange(items);
            paramList.Add(location);
            paramList.AddRange(items);
            string hqlInv1 = string.Empty;
            foreach (var item in items)
            {
                if (hqlInv1 == string.Empty)
                {
                    hqlInv1 = @"select  d.Item,SUM(d.OrderQty-d.RecQty) as Qty from ORD_OrderDet_1 d
                                join ORD_OrderMstr_1 m on d.OrderNo = m.OrderNo
                                where m.Status in(1,2) and m.SubType=0 and d.OrderQty>0 and d.OrderQty>d.RecQty
                                and m.CreateDate>DATEADD(day,-365,getdate()) and d.LocTo=? and Item in(?";
                }
                else
                {
                    hqlInv1 += ",?";
                }
            }
            hqlInv1 += @") group by d.Item UNION ALL ";

            string hqlInv7 = string.Empty;
            foreach (var item in items)
            {
                if (hqlInv7 == string.Empty)
                {
                    hqlInv7 = @"select  d.Item,SUM(d.OrderQty-d.RecQty) as Qty from ORD_OrderDet_7 d
                                join ORD_OrderMstr_7 m on d.OrderNo = m.OrderNo
                                where m.Status in(1,2) and m.SubType=0 and d.OrderQty>0 and d.OrderQty>d.RecQty
                                and m.CreateDate>DATEADD(day,-365,getdate()) and d.LocTo=? and Item in(?";
                }
                else
                {
                    hqlInv7 += ",?";
                }
            }
            hqlInv7 += @") group by d.Item";

            return (this.genericMgr.FindAllWithNativeSql<object[]>(hqlInv1 + hqlInv7, paramList.ToArray()) ?? new List<object[]>())
                  .GroupBy(p => p[0], (k, g) => new { Item = (string)k, Qty = g.Sum(q => (decimal)q[1]) })
                  .ToDictionary(d => d.Item, d => d.Qty);
        }




        [Transaction(TransactionMode.Requires)]
        public IList<Hu> Repack(string repackHuId, IList<ProductBarCode> checkedProductBarCodeList, IList<ProductBarCode> uncheckedProductBarCodeList)
        {
            IList<Hu> oldHuList = new List<Hu>();
            IList<Hu> newHuList = new List<Hu>();
            Hu hu = genericMgr.FindById<Hu>(repackHuId);
            oldHuList.Add(hu);

            #region 创建条码
            Hu hu1 = huMgr.CloneHu(hu, checkedProductBarCodeList.Sum(p => p.Qty));
            Hu hu2 = huMgr.CloneHu(hu, uncheckedProductBarCodeList.Sum(p => p.Qty));
            newHuList.Add(hu1);
            newHuList.Add(hu2);
            genericMgr.Create(hu1);
            genericMgr.Create(hu2);
            this.genericMgr.FlushSession();
            #endregion


            #region 翻包
            if (hu.Qty != hu1.Qty + hu2.Qty)
            {
                throw new BusinessException(Resources.INV.Hu.Hu_QtyNotMatch);
            }

            var inventoryPackList = new List<Entity.INV.InventoryRePack>();
            foreach (var h in oldHuList)
            {
                var inventoryUnPack = new Entity.INV.InventoryRePack();
                inventoryUnPack.HuId = h.HuId;
                inventoryUnPack.Type = CodeMaster.RePackType.Out;
                inventoryPackList.Add(inventoryUnPack);
            }
            foreach (var h in newHuList)
            {
                var inventoryUnPack = new Entity.INV.InventoryRePack();
                inventoryUnPack.HuId = h.HuId;
                inventoryUnPack.Type = CodeMaster.RePackType.In;
                inventoryPackList.Add(inventoryUnPack);
            }

            InventoryRePack(inventoryPackList);
            #endregion


            #region 重建对照关系
            foreach (ProductBarCode p in checkedProductBarCodeList)
            {
                p.HuId = hu1.HuId;
                genericMgr.Update(p);
            }

            foreach (ProductBarCode p in uncheckedProductBarCodeList)
            {
                p.HuId = hu2.HuId;
                genericMgr.Update(p);

            }

            #endregion

            return newHuList;

        }

        #endregion

        #region private methods
        #region 更新库存明细方法
        private IList<InventoryTransaction> RecordInventory(InventoryIO inventoryIO)
        {
            #region 判断库存生效日期是否有效
            FinanceCalendar financeCalendar = financeCalendarMgr.GetNowEffectiveFinanceCalendar();
            //前开后闭
            if (financeCalendar.StartDate >= inventoryIO.EffectiveDate)
            {
                throw new BusinessException(string.Format("库存的生效日期{0}小于当前财政月的开始日期{1}，不能进行库存操作。", inventoryIO.EffectiveDate, financeCalendar.StartDate));
            }
            #endregion

            IList<InventoryTransaction> inventoryTransactionList = new List<InventoryTransaction>();

            if (inventoryIO.LocationLotDetailId.HasValue)
            {
                #region 指定库存明细出库
                LocationLotDetail locationLotDetail = this.genericMgr.FindById<LocationLotDetail>(inventoryIO.LocationLotDetailId.Value);

                if (!string.IsNullOrWhiteSpace(locationLotDetail.HuId))
                {
                    throw new TechnicalException("条码库存不能指定库存明细出库。");
                }

                //记录被回冲的记录
                inventoryTransactionList.Add(CreateInventoryTransaction(locationLotDetail, -locationLotDetail.Qty, false, null));

                //更新库存数量
                //locationLotDetail.Qty += inventoryIO.Qty;
                locationLotDetail.Qty = 0;
                this.genericMgr.Update(locationLotDetail);
                #endregion
            }
            else if (!string.IsNullOrWhiteSpace(inventoryIO.HuId))
            {
                #region 有Hu
                //寄售/非寄售处理逻辑相同
                if (inventoryIO.Qty > 0)
                {
                    #region 入库数量 > 0
                    HuStatus huStatus = this.huMgr.GetHuStatus(inventoryIO.HuId);
                    if (huStatus.Status != CodeMaster.HuStatus.NA)
                    {
                        //if (huStatusList.Count > 1)
                        //{
                        //    throw new TechnicalException("HuId " + inventoryIO.HuId + "exist in two different location or ipdetail.");
                        //}

                        //HuStatus huStatus = huStatusList[0];

                        //同一个条码在所有库位中不能出现两次
                        if (huStatus.Status == CodeMaster.HuStatus.Location)
                        {
                            throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuIsAlreadyInLocation, inventoryIO.HuId, huStatus.Location);
                        }
                        else
                        {
                            throw new BusinessException("条码{0}是从库位{1}到库位{2}的在途库存，不能重复入库。", inventoryIO.HuId, huStatus.LocationFrom, huStatus.LocationTo);
                        }
                    }

                    inventoryIO.LotNo = huStatus.LotNo;
                    inventoryIO.SupplierLotNo = huStatus.SupplierLotNo;
                    CreateNewLocationLotDetail(inventoryIO, inventoryTransactionList);
                    #endregion
                }
                else if (inventoryIO.Qty < 0)
                {
                    #region 入库数量 < 0 / 出库
                    LocationLotDetail locationLotDetail = this.GetHuLocationLotDetail(inventoryIO.HuId);

                    if (locationLotDetail == null)
                    {
                        //任意库位没有找到指定的HU
                        throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuNotInAnyLocation, inventoryIO.HuId);
                    }

                    if (locationLotDetail.Location != inventoryIO.Location)
                    {
                        //HU不在指定的库位
                        throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuNotInSpecifiedLocation, inventoryIO.HuId, inventoryIO.Location);
                    }

                    if (locationLotDetail.Qty + inventoryIO.Qty != 0)
                    {
                        //Hu中Item的数量不等于出库数
                        throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuQtyNotEqualShipQty, inventoryIO.HuId, locationLotDetail.Qty.ToString("0.###"), inventoryIO.Qty.ToString("0.###"));
                    }

                    #region 判断条码冻结
                    if (locationLotDetail.IsFreeze)
                    {
                        throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuFrozenCantShip, inventoryIO.HuId);
                    }
                    #endregion

                    #region 判断订单占用
                    BusinessException businessException = new BusinessException();
                    if (inventoryIO.OccupyType != locationLotDetail.OccupyType)
                    {
                        if (inventoryIO.OccupyType == com.Sconit.CodeMaster.OccupyType.None)
                        {
                            //要求订单没有被占用，实际被占用
                            CheckLocationLotDetailOccupied(locationLotDetail, businessException);
                        }
                        else if (inventoryIO.OccupyType == com.Sconit.CodeMaster.OccupyType.Inspect)
                        {
                            //检查是否被检验单占用
                            CheckLocationLotDetailOccupiedByInspectOrder(locationLotDetail, inventoryIO.OccupyReferenceNo, businessException);
                        }
                        else if (inventoryIO.OccupyType == com.Sconit.CodeMaster.OccupyType.Pick)
                        {
                            //检查是否被捡货单占用
                            CheckLocationLotDetailOccupiedByPickList(locationLotDetail, inventoryIO.OccupyReferenceNo, businessException);
                        }
                        else if (inventoryIO.OccupyType == com.Sconit.CodeMaster.OccupyType.Sequence)
                        {
                            //检查是否被排序单占用
                            CheckLocationLotDetailOccupiedBySequenceOrder(locationLotDetail, inventoryIO.OccupyReferenceNo, businessException);
                        }
                        else if (inventoryIO.OccupyType == com.Sconit.CodeMaster.OccupyType.MiscOrder)
                        {
                            //检查是否被计划出入库单占用
                            CheckLocationLotDetailOccupiedByMiscOrder(locationLotDetail, inventoryIO.OccupyReferenceNo, businessException);
                        }
                    }

                    if (businessException.HasMessage)
                    {
                        throw businessException;
                    }
                    #endregion

                    #region 处理寄售
                    SettleBillTransaction billTransaction = null;
                    if (locationLotDetail.IsConsignment && locationLotDetail.PlanBill.HasValue                 //是否有RCT类型的出库？
                        && (inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_WO      //生产出库
                            || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_SWO      //委外生产出库
                            || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_UNP  //计划外出库
                            || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_MIN  //投料 李秋云
                            || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_SO   //销售出库
                        //|| inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_PO   //采购退库  采购退货出库出口库不需要处理寄售，在采购退货收货时处理
                            || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.CYC_CNT))    //盘亏
                    {
                        //寄售库存需要进行结算，对被回冲的库存进行负数结算
                        PlanBill pb = this.genericMgr.FindById<PlanBill>(locationLotDetail.PlanBill.Value);
                        //if (inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_PO
                        //    && inventoryIO.IsConsignment && inventoryIO.PlanBill.HasValue)
                        //{
                        //    //采购退货，如果退的是寄售库存，和退货的PlanBill冲销
                        //    inventoryIO.IsCreatePlanBill = false;
                        //    inventoryIO.IsConsignment = false;
                        //    inventoryIO.PlanBill = null;

                        //    pb.CurrentVoidQty = inventoryIO.Qty;
                        //    this.billMgr.VoidPlanBill(pb);
                        //}
                        //else
                        //{
                        pb.CurrentActingQty = -inventoryIO.Qty / pb.UnitQty; //按负数结算
                        pb.CurrentLocation = locationLotDetail.Location;
                        pb.CurrentHuId = locationLotDetail.HuId;
                        billTransaction = this.billMgr.SettleBill(pb, inventoryIO.EffectiveDate);
                        //}
                    }
                    //else if (locationLotDetail.IsConsignment && locationLotDetail.PlanBill.HasValue
                    //   && (inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_PO     //采购退货
                    //       || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_SL))   //计划协议退货
                    //{
                    //    //退货，寄售库存需要取消寄售数量
                    //    PlanBill pb = this.genericMgr.FindById<PlanBill>(locationLotDetail.PlanBill.Value);
                    //    pb.CurrentVoidQty = -inventoryIO.Qty / pb.UnitQty;
                    //    this.billMgr.VoidPlanBill(pb);
                    //}
                    #endregion

                    #region 处理冲销寄售和结算信息
                    if (billTransaction == null)
                    {
                        billTransaction = VoidBill(inventoryIO);
                    }
                    #endregion

                    //记录被回冲的记录
                    inventoryTransactionList.Add(CreateInventoryTransaction(locationLotDetail, inventoryIO.Qty, inventoryIO.IsCreatePlanBill, billTransaction));

                    //更新库存数量
                    locationLotDetail.Qty += inventoryIO.Qty;
                    this.genericMgr.Update(locationLotDetail);
                    #endregion
                }
                #endregion
            }
            else
            {
                #region 没有Hu
                IList<LocationLotDetail> locationLotDetailList = null;
                if (inventoryIO.IsConsignment && inventoryIO.PlanBill.HasValue)
                {
                    #region 寄售处理。
                    if (inventoryIO.Qty > 0)
                    {
                        #region 入库数量 > 0

                        //如果是收货后检验或者收货结算，不回冲库存。
                        //if (!(needInspection
                        //    || plannedBill.SettleTerm == BusinessConstants.CODE_MASTER_BILL_SETTLE_TERM_VALUE_RECEIVING_SETTLEMENT))
                        //{
                        //#region 回冲指定收货单号的寄售库存
                        //if (refOrderNo != null && refOrderNo.Trim() != string.Empty)
                        //{
                        //    IList<LocationLotDetail> locationLotDetailList = this.locationLotDetailMgrE.GetLocationLotDetail(location.Code, item.Code, true, false, BusinessConstants.MINUS_INVENTORY, false, refOrderNo, flushbackIgnoreHu);
                        //    BackFlushInventory(locationLotDetailList, ref qty, inventoryTransactionList, plannedBill, user, transType);
                        //}
                        //#endregion

                        if (inventoryIO.OccupyType == com.Sconit.CodeMaster.OccupyType.None) //占用库存入库不能回冲负数库存
                        {
                            #region 回冲寄售库存
                            if (inventoryIO.Qty > 0)
                            {
                                //locationLotDetailList = genericMgr.FindAllWithNamedQuery<LocationLotDetail>("USP_Busi_GetMinusInventory", new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, true });
                                locationLotDetailList = genericMgr.FindAllWithNamedQuery<LocationLotDetail>("USP_Busi_GetMinusInventory",
                                    new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, true },
                                    new IType[] { NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.Boolean });
                                //locationLotDetailList = genericMgr.FindAll<LocationLotDetail>(SelectMinusInventoryStatement, new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, true });
                                BackflushInventory(inventoryIO, locationLotDetailList, inventoryTransactionList);
                            }
                            #endregion

                            #region 回冲非寄售库存
                            if (inventoryIO.Qty > 0)
                            {
                                locationLotDetailList = genericMgr.FindAllWithNamedQuery<LocationLotDetail>("USP_Busi_GetMinusInventory", new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, false },
                                    new IType[] { NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.Boolean });
                                //locationLotDetailList = genericMgr.FindAll<LocationLotDetail>(SelectMinusInventoryStatement, new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, false });
                                BackflushInventory(inventoryIO, locationLotDetailList, inventoryTransactionList);
                            }
                            #endregion
                        }
                        //}

                        #region 记录库存
                        if (inventoryIO.Qty > 0)
                        {
                            CreateNewLocationLotDetail(inventoryIO, inventoryTransactionList);
                        }
                        #endregion
                        #endregion
                    }
                    else if (inventoryIO.Qty < 0)
                    {
                        #region 入库数量 < 0，出库

                        //#region 回冲指定收货单号的寄售库存
                        //if (refOrderNo != null && refOrderNo.Trim() != string.Empty)
                        //{
                        //    IList<LocationLotDetail> locationLotDetailList = this.locationLotDetailMgrE.GetLocationLotDetail(location.Code, item.Code, true, false, BusinessConstants.PLUS_INVENTORY, false, refOrderNo, flushbackIgnoreHu);
                        //    BackFlushInventory(locationLotDetailList, ref qty, inventoryTransactionList, plannedBill, user, transType);
                        //}
                        //#endregion
                        #region 非占用出库
                        if (inventoryIO.OccupyType == com.Sconit.CodeMaster.OccupyType.None)
                        {
                            if (inventoryIO.IsVoid)
                            {
                                #region 冲销
                                //入库时是寄售库存，冲销时出库的也应该是对应的寄售库存
                                locationLotDetailList = genericMgr.FindAllWithNamedQuery<LocationLotDetail>("USP_Busi_GetVoidInventory", new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.PlanBill.Value, inventoryIO.QualityType, inventoryIO.OccupyType },
                                    new IType[] { NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.Int32, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.Int16 });
                                //locationLotDetailList = genericMgr.FindAll<LocationLotDetail>(SelectVoidInventoryStatement, new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.PlanBill.Value, inventoryIO.QualityType, inventoryIO.OccupyType });
                                BackflushInventory(inventoryIO, locationLotDetailList, inventoryTransactionList);
                                #endregion
                            }
                            else
                            {
                                #region 非冲销
                                #region 回冲寄售库存
                                if (inventoryIO.Qty < 0)
                                {
                                    locationLotDetailList = genericMgr.FindAllWithNamedQuery<LocationLotDetail>("USP_Busi_GetVoidInventory", new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.PlanBill.Value, inventoryIO.QualityType, inventoryIO.OccupyType },
                                    new IType[] { NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.Int32, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.Int16 });
                                    //locationLotDetailList = genericMgr.FindAll<LocationLotDetail>(SelectPlusInventoryStatement, new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, true });
                                    BackflushInventory(inventoryIO, locationLotDetailList, inventoryTransactionList);
                                }
                                #endregion

                                //#region 回冲非寄售库存
                                //if (inventoryIO.Qty < 0)
                                //{
                                //    locationLotDetailList = genericMgr.FindAllWithNamedQuery<LocationLotDetail>("USP_Busi_GetPlusInventory", new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, false });
                                //    //locationLotDetailList = genericMgr.FindAll<LocationLotDetail>(SelectPlusInventoryStatement, new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, false });
                                //    BackflushInventory(inventoryIO, locationLotDetailList, inventoryTransactionList);
                                //}
                                //#endregion
                                #endregion
                            }
                        }
                        #endregion

                        #region 占用出库
                        else
                        {
                            //指定Planbill出库
                            locationLotDetailList = genericMgr.FindAllWithNamedQuery<LocationLotDetail>("USP_Busi_GetVoidOccupyInventory", new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.PlanBill.Value, inventoryIO.QualityType, inventoryIO.OccupyType, inventoryIO.OccupyReferenceNo },
                                new IType[] { NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.Int32, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.String });
                            //locationLotDetailList = genericMgr.FindAll<LocationLotDetail>(SelectVoidOccupyInventoryStatement, new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.PlanBill.Value, inventoryIO.QualityType, inventoryIO.OccupyType, inventoryIO.OccupyReferenceNo });
                            BackflushInventory(inventoryIO, locationLotDetailList, inventoryTransactionList);
                        }
                        #endregion

                        #region 记录库存
                        if (inventoryIO.Qty < 0)
                        {
                            //指定Planbill库存不足
                            PlanBill planBill = this.genericMgr.FindById<PlanBill>(inventoryIO.PlanBill.Value);
                            throw new BusinessException(string.Format("供应商{0}的物料{1}在库位{2}中库存不足。", planBill.Party, inventoryIO.Item, inventoryIO.Location));
                            //CreateNewLocationLotDetail(inventoryIO, inventoryTransactionList);
                        }
                        #endregion
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    #region 非寄售处理
                    if (inventoryIO.Qty > 0)
                    {
                        #region 入库数量 > 0

                        if (inventoryIO.OccupyType == com.Sconit.CodeMaster.OccupyType.None) //占用库存入库不能回冲负数库存
                        {
                            //收货后检验的不能回冲库存
                            //if (!needInspection)
                            //{
                            #region 回冲非寄售库存
                            locationLotDetailList = genericMgr.FindAllWithNamedQuery<LocationLotDetail>("USP_Busi_GetMinusInventory", new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, false },
                                new IType[] { NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.Boolean });
                            //locationLotDetailList = genericMgr.FindAll<LocationLotDetail>(SelectMinusInventoryStatement, new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, false });
                            BackflushInventory(inventoryIO, locationLotDetailList, inventoryTransactionList);
                            #endregion
                            //}
                        }

                        #region 记录库存
                        if (inventoryIO.Qty > 0)
                        {
                            CreateNewLocationLotDetail(inventoryIO, inventoryTransactionList);
                        }
                        #endregion
                        #endregion
                    }
                    else if (inventoryIO.Qty < 0)
                    {
                        #region 入库数量 < 0

                        #region 非占用出库
                        if (inventoryIO.OccupyType == com.Sconit.CodeMaster.OccupyType.None)
                        {
                            if (!string.IsNullOrWhiteSpace(inventoryIO.ManufactureParty))
                            {
                                #region 指定供应商出库USP_Busi_GetManufacturePartyConsignmentInventory
                                locationLotDetailList = genericMgr.FindAllWithNamedQuery<LocationLotDetail>("USP_Busi_GetManufacturePartyConsignmentInventory",
                                    new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, true, inventoryIO.ManufactureParty },
                                new IType[] { NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.Boolean, NHibernate.NHibernateUtil.String });
                                //locationLotDetailList = genericMgr.FindAll<LocationLotDetail>(SelectManufacturePartyConsignmentInventoryStatement, new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, true, inventoryIO.ManufactureParty });
                                BackflushInventory(inventoryIO, locationLotDetailList, inventoryTransactionList);

                                if (inventoryIO.Qty < 0)
                                {
                                    if (!TryLoadLocation(inventoryIO).AllowNegative)
                                    {
                                        throw new BusinessException("物料{0}在库位{1}中的供应商{2}寄售库存不足。", inventoryIO.Item, inventoryIO.Location, inventoryIO.ManufactureParty);
                                    }
                                }

                                //直接扣减为负库存
                                #endregion
                            }
                            else if (inventoryIO.IsVoid)
                            {
                                #region 冲销
                                //入库时是非寄售库存，冲销时出库的也应该是非寄售库存
                                //如果是入库结算的应该先冲销结算，再把寄售信息记录至IpLocationDet中。
                                locationLotDetailList = genericMgr.FindAllWithNamedQuery<LocationLotDetail>("USP_Busi_GetPlusInventory", new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, false },
                                    new IType[] { NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.Boolean });
                                //locationLotDetailList = genericMgr.FindAll<LocationLotDetail>(SelectPlusInventoryStatement, new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, false });
                                BackflushInventory(inventoryIO, locationLotDetailList, inventoryTransactionList);
                                #endregion
                            }
                            else
                            {
                                #region 回冲非寄售库存
                                if (inventoryIO.Qty < 0)
                                {
                                    locationLotDetailList = genericMgr.FindAllWithNamedQuery<LocationLotDetail>("USP_Busi_GetPlusInventory", new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, false },
                                    new IType[] { NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.Boolean });
                                    //locationLotDetailList = genericMgr.FindAll<LocationLotDetail>(SelectPlusInventoryStatement, new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, false });
                                    BackflushInventory(inventoryIO, locationLotDetailList, inventoryTransactionList);
                                }
                                #endregion

                                #region 回冲寄售库存
                                if (inventoryIO.Qty < 0 && inventoryIO.TransactionType != CodeMaster.TransactionType.ISS_IIC)  //物料库存替换不能冲寄售库存
                                {
                                    locationLotDetailList = genericMgr.FindAllWithNamedQuery<LocationLotDetail>("USP_Busi_GetPlusInventory", new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, true },
                                    new IType[] { NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.Boolean });
                                    //locationLotDetailList = genericMgr.FindAll<LocationLotDetail>(SelectPlusInventoryStatement, new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, true });
                                    BackflushInventory(inventoryIO, locationLotDetailList, inventoryTransactionList);
                                }
                                #endregion
                            }
                        }
                        #endregion

                        #region 占用出库
                        else
                        {
                            #region 回冲非寄售库存
                            if (inventoryIO.Qty < 0)
                            {
                                locationLotDetailList = genericMgr.FindAllWithNamedQuery<LocationLotDetail>("USP_Busi_GetOccupyInventory", new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, inventoryIO.OccupyReferenceNo, false },
                                    new IType[] { NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.Boolean });
                                //locationLotDetailList = genericMgr.FindAll<LocationLotDetail>(SelectOccupyInventoryStatement, new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, inventoryIO.OccupyReferenceNo });
                                BackflushInventory(inventoryIO, locationLotDetailList, inventoryTransactionList);
                            }
                            #endregion

                            #region 回冲寄售库存
                            if (inventoryIO.Qty < 0)
                            {
                                locationLotDetailList = genericMgr.FindAllWithNamedQuery<LocationLotDetail>("USP_Busi_GetOccupyInventory", new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, inventoryIO.OccupyReferenceNo, true },
                                    new IType[] { NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.Int16, NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.Boolean });
                                //locationLotDetailList = genericMgr.FindAll<LocationLotDetail>(SelectOccupyInventoryStatement, new Object[] { inventoryIO.Location, inventoryIO.Item, inventoryIO.QualityType, inventoryIO.OccupyType, inventoryIO.OccupyReferenceNo });
                                BackflushInventory(inventoryIO, locationLotDetailList, inventoryTransactionList);
                            }
                            #endregion
                        }
                        #endregion

                        #region 记录库存
                        //if (inputLocationLotDetail.Qty < 0 && flushToNegative)
                        if (inventoryIO.Qty < 0)
                        {
                            CreateNewLocationLotDetail(inventoryIO, inventoryTransactionList);
                        }
                        #endregion

                        #endregion
                    }
                    #endregion
                }
                #endregion
            }

            return inventoryTransactionList;
        }

        private void BackflushInventory(InventoryIO inventoryIO, IList<LocationLotDetail> backFlushLocLotDetList, IList<InventoryTransaction> inventoryTransactionList)
        {
            Boolean isVoidedActBill = false;
            SettleBillTransaction voidBillTransaction = new SettleBillTransaction();
            if (backFlushLocLotDetList != null && backFlushLocLotDetList.Count > 0)
            {
                foreach (LocationLotDetail backFlushLocLotDet in backFlushLocLotDetList)
                {
                    #region 只有发生在按条码发货，按数量收货的情况，因为有些待回冲的库存已经被上一个条码冲掉，这里就不继续回冲了。
                    if (backFlushLocLotDet.Qty == 0)
                    {
                        continue;
                    }
                    #endregion

                    #region 判断是否满足回冲条件
                    if (inventoryIO.Qty == 0)
                    {
                        return;
                    }

                    PlanBill backFlushPlanBill = backFlushLocLotDet.IsConsignment && backFlushLocLotDet.PlanBill.HasValue ? this.genericMgr.FindById<PlanBill>(backFlushLocLotDet.PlanBill.Value) : null;
                    PlanBill inputPlanBill = inventoryIO.IsConsignment ? TryLoadPlanBill(inventoryIO) : null;

                    //如果被回冲和入库的都是寄售库存并且是同一个供应商，一定要回冲
                    if (backFlushLocLotDet.IsConsignment && inputPlanBill != null
                        && backFlushPlanBill.BillAddress == inputPlanBill.BillAddress)
                    {

                    }
                    else
                    {
                        //被回冲的寄售库存，和入库的寄售库存不是一个供应商，不能回冲
                        if (backFlushLocLotDet.IsConsignment && inputPlanBill != null
                            && backFlushPlanBill.BillAddress != inputPlanBill.BillAddress)
                        {
                            return;
                        }

                        //被回冲的寄售库存的结算方式是上线结算，判断是否当前事务类型是否是ISS-*，不满足不能回冲
                        //if (backFlushLocLotDet.IsConsignment
                        //    && backFlushLocLotDet.PlannedBill.SettleTerm == BusinessConstants.CODE_MASTER_BILL_SETTLE_TERM_VALUE_ONLINE_BILLING)
                        //{
                        //    if (!transType.StartsWith("ISS-"))
                        //    {
                        //        return;
                        //    }
                        //}

                        //被回冲的寄售库存的结算方式是下线结算，判断是否当前事务类型是否是ISS-*并且不等于ISS-TR，不满足不能回冲
                        //if (backFlushLocLotDet.IsConsignment
                        //   && backFlushLocLotDet.PlannedBill.SettleTerm == BusinessConstants.CODE_MASTER_BILL_SETTLE_TERM_VALUE_LINEAR_CLEARING)
                        //{
                        //    if (!(transType.StartsWith("ISS-") && transType != BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_ISS_TR))
                        //    {
                        //        return;
                        //    }
                        //}
                    }
                    #endregion

                    #region 回冲库存
                    decimal currentBFQty = 0; //本次回冲数
                    if (inventoryIO.Qty > 0)
                    {
                        if (backFlushLocLotDet.Qty + inventoryIO.Qty < 0)
                        {
                            //本次入库数 < 库存数量，全部回冲，回冲数量等于本次入库数
                            currentBFQty = inventoryIO.Qty;
                        }
                        else
                        {
                            //本次入库数 >= 库存数量，按负的库存数回冲
                            currentBFQty = 0 - backFlushLocLotDet.Qty;
                        }
                    }
                    else
                    {
                        if (backFlushLocLotDet.Qty + inventoryIO.Qty > 0)
                        {
                            //本次出库数 < 库存数量，全部回冲，回冲数量等于本次出库数
                            currentBFQty = inventoryIO.Qty;
                        }
                        else
                        {
                            //本次出库数 >= 库存数量，按正的库存数回冲
                            currentBFQty = 0 - backFlushLocLotDet.Qty;
                        }
                    }

                    //更新库存数量
                    backFlushLocLotDet.Qty += currentBFQty;
                    this.genericMgr.Update(backFlushLocLotDet);

                    #endregion

                    #region 结算
                    SettleBillTransaction billTransaction = null;

                    if (inventoryIO.Qty < 0 && inputPlanBill == null && backFlushLocLotDet.IsConsignment)
                    {
                        if (inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_SO            //销售出库
                                || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_WO      //生产出库
                            //|| inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_MIN  //投料 李秋云
                                || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_SWO      //委外生产出库
                                || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_UNP    //计划外出库
                                || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.CYC_CNT)     //盘亏
                        //// (backFlushPlannedBill.SettleTerm == BusinessConstants.CODE_MASTER_BILL_SETTLE_TERM_VALUE_ONLINE_BILLING   //上线结算条件，发生移库出库事务才结算，可以避免收货检验时入检验库位就结算
                        ////&& (transType == BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_ISS_TR
                        ////    || transType == BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_ISS_MATERIAL_IN)) 
                        ////|| 
                        //(backFlushPlanBill.SettleTerm == BusinessConstants.CODE_MASTER_BILL_SETTLE_TERM_VALUE_LINEAR_CLEARING   //下线结算条件
                        //    && transType == BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_ISS_WO)
                        ////|| (backFlushLocLotDet.PlannedBill.SettleTerm == BusinessConstants.CODE_MASTER_BILL_SETTLE_TERM_VALUE_INSPECTION         //检验结算条件，从检验库位出库，并且发生ISS-INP事务
                        ////    && backFlushLocLotDet.Location.Code == BusinessConstants.SYSTEM_LOCATION_INSPECT
                        ////        && transType == BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_ISS_INP)
                        ////|| (backFlushLocLotDet.PlannedBill.SettleTerm == BusinessConstants.CODE_MASTER_BILL_SETTLE_TERM_VALUE_INSPECTION         //检验结算，从不合格品库位出库，立即结算
                        ////    && backFlushLocLotDet.Location.Code == BusinessConstants.SYSTEM_LOCATION_REJECT)
                        //|| transType == BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_ISS_UNP                              //如果发生ISS_UNP或者CYC_CNT事务，强行结算
                        //|| transType == BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_CYC_CNT
                        //|| transType == BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_ISS_SO
                        //|| transType == BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_ISS_WO
                        //|| transType.StartsWith(BusinessConstants.CODE_MASTER_LOCATION_TRANSACTION_TYPE_VALUE_RCT))
                        {
                            //寄售库存需要进行结算，对被回冲的库存进行负数结算
                            backFlushPlanBill.CurrentActingQty = (0 - currentBFQty) / backFlushPlanBill.UnitQty; //按负数结算
                            backFlushPlanBill.CurrentLocation = backFlushLocLotDet.Location;
                            billTransaction = this.billMgr.SettleBill(backFlushPlanBill, inventoryIO.EffectiveDate);
                        }
                        //else if (inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_PO      //采购退货
                        //        || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_SL)     //计划协议退货
                        //{
                        //    //退货，寄售库存需要取消寄售数量
                        //    backFlushPlanBill.CurrentVoidQty = (0 - currentBFQty) / backFlushPlanBill.UnitQty; //按负数结算
                        //    this.billMgr.VoidPlanBill(backFlushPlanBill);
                        //}
                    }
                    else
                    {
                        //如果出库PlanBill和被回冲PlanBill的Id相同，即指定PlanBill出库，不发生回冲
                        if (!(backFlushLocLotDet.IsConsignment && backFlushLocLotDet.PlanBill == inputPlanBill.Id))
                        {
                            if (backFlushLocLotDet.IsConsignment)
                            {
                                //寄售库存需要进行结算，对被回冲的库存进行负数结算
                                backFlushPlanBill.CurrentActingQty = (0 - currentBFQty) / backFlushPlanBill.UnitQty; //按负数结算
                                backFlushPlanBill.CurrentLocation = backFlushLocLotDet.Location;
                                billTransaction = this.billMgr.SettleBill(backFlushPlanBill, inventoryIO.EffectiveDate);
                            }

                            if (inputPlanBill != null)
                            {
                                if (inventoryIO.QualityType == CodeMaster.QualityType.Inspect)
                                {
                                    throw new BusinessException("待验物料{0}不能结算。", inventoryIO.Item);
                                }
                                else if (inventoryIO.QualityType == CodeMaster.QualityType.Reject)
                                {
                                    //throw new BusinessException("不合格物料{0}不能结算。", inventoryIO.Item);
                                }

                                //对入库的库存进行结算
                                inputPlanBill.CurrentActingQty = currentBFQty / inputPlanBill.UnitQty;  //按正数结算
                                inputPlanBill.CurrentLocation = backFlushLocLotDet.Location;
                                billTransaction = this.billMgr.SettleBill(inputPlanBill, inventoryIO.EffectiveDate);
                            }
                        }
                    }
                    #endregion

                    #region 处理冲销寄售和结算信息
                    if (billTransaction == null)
                    {
                        if (isVoidedActBill == false)
                        {
                            billTransaction = VoidBill(inventoryIO);
                            isVoidedActBill = true;
                            voidBillTransaction = billTransaction;
                        }
                        else
                        {
                            billTransaction = voidBillTransaction;
                        }
                    }
                    #endregion

                    //记录被回冲的记录
                    if (inventoryIO.Qty > 0)
                    {
                        //回冲负数库存，按inventoryIO记录库存事务
                        inventoryTransactionList.Add(CreateInventoryTransaction(inventoryIO, currentBFQty, backFlushLocLotDet, billTransaction));
                    }
                    else
                    {
                        //回冲正数库存，按backFlushLocLotDet记录库存事务
                        //因为是出库，不可能有创建Planbill的情况
                        inventoryTransactionList.Add(CreateInventoryTransaction(backFlushLocLotDet, currentBFQty, false, billTransaction));
                    }

                    inventoryIO.Qty -= currentBFQty;
                }
            }
        }

        private LocationLotDetail CreateNewLocationLotDetail(InventoryIO inventoryIO, IList<InventoryTransaction> inventoryTransactionList)
        {
            #region 是否允许负库存，占用不能出现负数库存，待验和不合格品不允许出现负库存
            if (inventoryIO.Qty < 0)
            {
                if (//inventoryIO.OccupyType != com.Sconit.CodeMaster.OccupyType.None || 
                    inventoryIO.QualityType != CodeMaster.QualityType.Qualified
                    || !TryLoadLocation(inventoryIO).AllowNegative)
                {
                    throw new BusinessException(Resources.INV.LocationLotDetail.Errors_NotEnoughInventory, inventoryIO.Item, inventoryIO.Location);
                }
            }
            #endregion

            #region 冲销时按指定PlanBill出库数量不足
            //只有冲销才会出现这种情况？
            if (inventoryIO.Qty < 0 && inventoryIO.IsConsignment && inventoryIO.PlanBill.HasValue)
            {
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_VoidFailNotEnoughSpecifyInventory, inventoryIO.Location, inventoryIO.Item);
            }
            #endregion

            //todo disabled的零件能不能做库存事务

            #region 虚零件和套件不能做库存事务不能做库存事务
            Item item = TryLoadItem(inventoryIO);
            if (item.IsVirtual)
            {
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_VirtualItemCannotProcessInventory, inventoryIO.Item);
            }

            if (item.IsKit)
            {
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_KitItemCannotProcessInventory, inventoryIO.Item);
            }
            #endregion

            #region 收货结算/入库结算/检验结算
            SettleBillTransaction billTransaction = null;
            if (inventoryIO.IsConsignment && inventoryIO.PlanBill.HasValue)
            {
                PlanBill pb = TryLoadPlanBill(inventoryIO);

                if (pb.BillTerm == com.Sconit.CodeMaster.OrderBillTerm.ReceivingSettlement  //收货结算
                    || (pb.BillTerm == com.Sconit.CodeMaster.OrderBillTerm.OnlineBilling && TryLoadLocation(inventoryIO).IsConsignment) //上线结算
                    || (pb.BillTerm == com.Sconit.CodeMaster.OrderBillTerm.AfterInspection     //检验结算，检验合格收货事务或者让步使用收货事务
                    //&& inventoryIO.QualityType == com.Sconit.CodeMaster.QualityType.Qualified
                        && (inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_INP_QDII
                        || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_INP_CCS))
                    || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_SO)   //销售退货收货立即结算
                {
                    if (inventoryIO.QualityType == CodeMaster.QualityType.Inspect)
                    {
                        throw new BusinessException("待验物料{0}不能结算。", inventoryIO.Item);
                    }
                    else if (inventoryIO.QualityType == CodeMaster.QualityType.Reject)
                    {
                        //throw new BusinessException("不合格物料{0}不能结算。", inventoryIO.Item);
                    }

                    pb.CurrentActingQty = inventoryIO.Qty / pb.UnitQty;
                    pb.CurrentLocation = inventoryIO.Location;
                    billTransaction = this.billMgr.SettleBill(pb, inventoryIO.EffectiveDate);

                    inventoryIO.IsConsignment = false;
                    //inventoryIO.PlanBill = null;
                }
            }
            #endregion

            #region 处理冲销寄售和结算信息
            if (billTransaction == null)
            {
                //已经做了结算不可能发生冲销
                billTransaction = VoidBill(inventoryIO);
            }
            #endregion

            #region 创建库存明细
            LocationLotDetail newLocationLotDetail = Mapper.Map<InventoryIO, LocationLotDetail>(inventoryIO);

            if (!string.IsNullOrWhiteSpace(newLocationLotDetail.HuId))
            {
                #region 更新条码首次入库日期
                Hu hu = inventoryIO.CurrentHu;
                if (hu == null)
                {
                    hu = this.genericMgr.FindById<Hu>(newLocationLotDetail.HuId);
                }

                if (!hu.FirstInventoryDate.HasValue)
                {
                    hu.FirstInventoryDate = DateTime.Now;
                    this.genericMgr.Update(hu);
                }
                #endregion
                //  huId = huId.ToUpper();
                //newLocationLotDetail.Hu = this.huMgrE.LoadHu(huId);

                ////库存数量和条码上的数量有差异，更新条码上的数量
                //if (newLocationLotDetail.Hu.Qty * newLocationLotDetail.Hu.UnitQty != qty)
                //{
                //    newLocationLotDetail.Hu.Qty = qty / newLocationLotDetail.Hu.UnitQty;
                //}
                //newLocationLotDetail.Hu.Location = location.Code;
                //newLocationLotDetail.Hu.Status = BusinessConstants.CODE_MASTER_HU_STATUS_VALUE_INVENTORY;

                //this.huMgrE.UpdateHu(newLocationLotDetail.Hu);

                if (newLocationLotDetail.Qty < 0)
                {
                    throw new TechnicalException("Barcode qty can't less than 0.");
                }
            }

            //入库

            LocationLotDetail mergeLocationLotDetail = null;
            if (string.IsNullOrWhiteSpace(newLocationLotDetail.HuId) && newLocationLotDetail.IsConsignment == false
                && newLocationLotDetail.IsFreeze == false && newLocationLotDetail.IsATP == true
                && newLocationLotDetail.OccupyType == CodeMaster.OccupyType.None)
            {
                #region 查找库存明细是否可以合并，有条码的不能合并(负库存，整数可用非寄售库存合并)
                mergeLocationLotDetail = this.genericMgr.FindEntityWithNativeSql<LocationLotDetail>("exec USP_Busi_GetMergeInventory ?,?,?,?,?,?"
                    , new object[] { newLocationLotDetail.Location, newLocationLotDetail.Item, newLocationLotDetail.Qty,
                                     newLocationLotDetail.IsFreeze, newLocationLotDetail.IsATP, newLocationLotDetail.OccupyType}
                    , new IType[] { NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.String, NHibernate.NHibernateUtil.Decimal, 
                                    NHibernate.NHibernateUtil.Boolean,NHibernate.NHibernateUtil.Boolean, NHibernate.NHibernateUtil.Int16 }).FirstOrDefault();
                #endregion
            }

            if (mergeLocationLotDetail != null)
            {
                mergeLocationLotDetail.Qty += newLocationLotDetail.Qty;
                this.genericMgr.Update(mergeLocationLotDetail);
                //inventoryTransactionList.Add(CreateInventoryTransaction(inventoryIO, inventoryIO.Qty, mergeLocationLotDetail, billTransaction));
            }
            else
            {
                this.genericMgr.Create(newLocationLotDetail);
                //inventoryTransactionList.Add(CreateInventoryTransaction(inventoryIO, inventoryIO.Qty, newLocationLotDetail, billTransaction));
            }
            //this.genericMgr.Create(newLocationLotDetail);
            #endregion

            //inventoryTransactionList.Add(CreateInventoryTransaction(newLocationLotDetail, newLocationLotDetail.Qty, inventoryIO.IsCreatePlanBill, billTransaction));
            inventoryTransactionList.Add(CreateInventoryTransaction(inventoryIO, inventoryIO.Qty, newLocationLotDetail, billTransaction));

            return newLocationLotDetail;
        }

        private InventoryTransaction CreateInventoryTransaction(LocationLotDetail locationLotDetail, decimal qty, bool isCreatePlanBill, SettleBillTransaction billTransaction)
        {
            InventoryTransaction inventoryTransaction = new InventoryTransaction();
            inventoryTransaction.LocationLotDetailId = locationLotDetail.Id;
            inventoryTransaction.Location = locationLotDetail.Location;
            inventoryTransaction.Bin = locationLotDetail.Bin;
            inventoryTransaction.Item = locationLotDetail.Item;
            inventoryTransaction.HuId = locationLotDetail.HuId;
            inventoryTransaction.LotNo = locationLotDetail.LotNo;
            inventoryTransaction.IsConsignment = locationLotDetail.IsConsignment;
            inventoryTransaction.IsCreatePlanBill = isCreatePlanBill;
            inventoryTransaction.PlanBill = locationLotDetail.PlanBill;
            if (locationLotDetail.IsConsignment)
            {
                //寄售库存
                inventoryTransaction.PlanBillQty = qty;
            }
            if (billTransaction != null)   //发生了结算，记录结算数量
            {
                inventoryTransaction.BillTransactionId = billTransaction.Id;
                //inventoryTransaction.IsConsignment = false;
                inventoryTransaction.ActingBill = billTransaction.ActingBill;
                inventoryTransaction.ActingBillQty = qty;
                inventoryTransaction.PlanBillQty = 0;
            }
            inventoryTransaction.Qty = qty;
            inventoryTransaction.QualityType = locationLotDetail.QualityType;
            inventoryTransaction.IsATP = locationLotDetail.IsATP;
            inventoryTransaction.IsFreeze = locationLotDetail.IsFreeze;
            inventoryTransaction.OccupyType = locationLotDetail.OccupyType;
            inventoryTransaction.OccupyReferenceNo = locationLotDetail.OccupyReferenceNo;

            return inventoryTransaction;
        }

        private InventoryTransaction CreateInventoryTransaction(InventoryIO inventoryIO, decimal qty, LocationLotDetail backFlushLocLotDet, SettleBillTransaction billTransaction)
        {
            InventoryTransaction inventoryTransaction = new InventoryTransaction();
            inventoryTransaction.LocationLotDetailId = backFlushLocLotDet.Id;
            inventoryTransaction.Location = inventoryIO.Location;
            inventoryTransaction.Bin = inventoryIO.Bin;
            inventoryTransaction.Item = inventoryIO.Item;
            inventoryTransaction.HuId = inventoryIO.HuId;
            inventoryTransaction.LotNo = inventoryIO.LotNo;
            inventoryTransaction.IsCreatePlanBill = inventoryIO.IsCreatePlanBill;
            inventoryTransaction.IsConsignment = inventoryIO.IsConsignment;
            inventoryTransaction.PlanBill = inventoryIO.PlanBill;
            if (inventoryIO.IsConsignment)
            {
                //寄售库存
                inventoryTransaction.PlanBillQty = qty;
            }
            if (billTransaction != null)   //发生了结算，记录结算数量
            {
                inventoryTransaction.BillTransactionId = billTransaction.Id;
                inventoryTransaction.IsConsignment = false;
                inventoryTransaction.ActingBill = billTransaction.ActingBill;
                inventoryTransaction.ActingBillQty = qty;             //基本单位
                inventoryTransaction.PlanBillQty = inventoryIO.Qty - qty;
            }
            inventoryTransaction.Qty = qty;
            inventoryTransaction.QualityType = inventoryIO.QualityType;
            inventoryTransaction.IsATP = inventoryIO.IsATP;
            inventoryTransaction.IsFreeze = inventoryIO.IsFreeze;
            inventoryTransaction.OccupyType = inventoryIO.OccupyType;
            inventoryTransaction.OccupyReferenceNo = inventoryIO.OccupyReferenceNo;

            return inventoryTransaction;
        }

        private SettleBillTransaction VoidBill(InventoryIO inventoryIO)
        {
            #region 处理冲销寄售和结算

            //只有收货单冲销才会发生冲销寄售和结算问题，也就是只有RCT事务才有，ISS没有。
            if (inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_PO_VOID  //采购收货冲销
                || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_SL_VOID  //计划协议收货冲销
                || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_SO_VOID //销售收货冲销
                || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_TR_VOID //移库收货冲销
                || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_TR_RTN_VOID //移库退货收货冲销
                || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_STR_VOID //委外移库收货冲销
                || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_STR_RTN_VOID //委外移库退货收货冲销
                || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_UNP_VOID   //计划外出库冲销
                || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_UNP_VOID  //计划外入库冲销   //入的都是非寄售物品，不存在冲销的问题 //现在支持入库寄售库存
                || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.RCT_WO_VOID  //生产收货冲销
                || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.CYC_CNT_VOID //盘盈冲销出库，出负数；盘亏冲销入库，入正数          
                //inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_PO_VOID //采购退货冲销入库 //因为采购退货
                || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_SO_VOID  //销售冲销入库
                //|| inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_TR_VOID  //移库冲销入库
                //|| inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_TR_RTN_VOID //移库退货冲销入库
                || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_WO_BF_VOID   //投料冲销入库             //下线结算的东西要冲销回来
                || inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_WO_VOID      //生产投料冲销入库
                )
            {
                SettleBillTransaction billTransaction = null;
                if (inventoryIO.ActingBill.HasValue)
                {
                    ActingBill actingBill = TryLoadActingBill(inventoryIO);
                    PlanBill planbill = inventoryIO.PlanBill.HasValue ? TryLoadPlanBill(inventoryIO) : null;
                    if (inventoryIO.TransactionType == com.Sconit.CodeMaster.TransactionType.ISS_SO_VOID)
                    {
                        actingBill.CurrentVoidQty = inventoryIO.Qty / actingBill.UnitQty;
                    }
                    else
                    {
                        actingBill.CurrentVoidQty = -inventoryIO.Qty / actingBill.UnitQty;   //转为订单单位
                    }
                    inventoryIO.CurrentPlanBill.CurrentLocation = inventoryIO.Location;
                    billTransaction = this.billMgr.VoidSettleBill(actingBill, planbill, inventoryIO.IsCreatePlanBill);
                    //inventoryIO.ActingBill = null;

                    if (inventoryIO.IsCreatePlanBill)
                    {
                        inventoryIO.PlanBill = null;
                        inventoryIO.IsConsignment = false;
                        inventoryIO.IsCreatePlanBill = false;
                    }
                }
                else if (inventoryIO.IsCreatePlanBill)
                {
                    PlanBill planBill = TryLoadPlanBill(inventoryIO);
                    planBill.CurrentVoidQty = -inventoryIO.Qty / planBill.UnitQty;   //转为订单单位
                    this.billMgr.VoidPlanBill(planBill);
                    inventoryIO.PlanBill = null;
                    inventoryIO.IsConsignment = false;
                    inventoryIO.IsCreatePlanBill = false;
                }

                return billTransaction;
            }

            return null;
            #endregion
        }
        #endregion

        #region 检查库存占用类型
        #region 检查库存是否被占用
        private void CheckLocationLotDetailOccupied(LocationLotDetail locationLotDetail, BusinessException businessException)
        {
            if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.Inspect)
            {
                //被检验单占用
                businessException.AddMessage(Resources.INV.LocationLotDetail.Errors_HuOccupiedByInspectOrder, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
            }
            else if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.Pick)
            {
                //被捡货单占用
                businessException.AddMessage(Resources.INV.LocationLotDetail.Errors_HuOccupiedByPickList, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
            }
            else if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.Sequence)
            {
                //被排序单占用
                businessException.AddMessage(Resources.INV.LocationLotDetail.Errors_HuOccupiedBySequenceOrder, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
            }
            else if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.MiscOrder)
            {
                //被计划外出入库单占用
                businessException.AddMessage(Resources.INV.LocationLotDetail.Errors_HuOccupiedByMiscOrder, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
            }
        }
        #endregion

        #region 检查库存是否被检验单占用
        private void CheckLocationLotDetailOccupiedByInspectOrder(LocationLotDetail locationLotDetail, string occupyReferenceNo, BusinessException businessException)
        {
            if (locationLotDetail.OccupyType == CodeMaster.OccupyType.Inspect && locationLotDetail.OccupyReferenceNo != occupyReferenceNo)
            {
                //要求被检验单占用，实际被其它检验单占用
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuOccupiedByInspectOrder, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
            }
            else if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.None)
            {
                //要求被检验单占用，实际没有被占用
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuNotOccupiedByInspectOrder, locationLotDetail.HuId, occupyReferenceNo);
            }
            else if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.Pick)
            {
                //要求被检验单占用，实际被捡货单占用
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuOccupiedByPickList, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
            }
            else if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.Sequence)
            {
                //要求被检验单占用，实际被排序单占用
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuOccupiedBySequenceOrder, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
            }
            else if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.MiscOrder)
            {
                //要求被检验单占用，实际被计划外出入库单占用
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuOccupiedByMiscOrder, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
            }
        }
        #endregion

        #region 检查库存是否被捡货单占用
        private void CheckLocationLotDetailOccupiedByPickList(LocationLotDetail locationLotDetail, string occupyReferenceNo, BusinessException businessException)
        {
            if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.Pick)
            {
                if (locationLotDetail.OccupyReferenceNo != occupyReferenceNo)
                {
                    if (locationLotDetail.OccupyReferenceNo != null || occupyReferenceNo != null)
                    {
                        //要求被捡货单占用，实际被其它捡货占用
                        throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuOccupiedByPickList, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
                    }
                }
            }
            else if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.None)
            {
                //要求被捡货单占用，实际没有被占用
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuNotOccupiedByPickList, locationLotDetail.HuId, occupyReferenceNo);
            }
            else if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.Inspect)
            {
                //要求被捡货单占用，实际被检验单占用
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuOccupiedByInspectOrder, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
            }
            else if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.Sequence)
            {
                //要求被捡货单占用，实际被排序单占用
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuOccupiedBySequenceOrder, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
            }
            else if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.MiscOrder)
            {
                //要求被捡货单占用，实际被计划外出入库单占用
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuOccupiedByMiscOrder, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
            }
        }
        #endregion

        #region 检查库存是否被排序单占用
        private void CheckLocationLotDetailOccupiedBySequenceOrder(LocationLotDetail locationLotDetail, string occupyReferenceNo, BusinessException businessException)
        {
            if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.Sequence && locationLotDetail.OccupyReferenceNo != occupyReferenceNo)
            {
                //要求被捡货单占用，实际被其它捡货占用
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuOccupiedBySequenceOrder, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
            }
            else if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.None)
            {
                //要求被排序单占用，实际没有被占用
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuNotOccupiedBySequenceOrder, locationLotDetail.HuId, occupyReferenceNo);
            }
            else if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.Pick)
            {
                //要求被排序单占用，实际被拣货单占用
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuOccupiedByPickList, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
            }
            else if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.Inspect)
            {
                //要求被排序单占用，实际被检验单占用
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuOccupiedByInspectOrder, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
            }
            else if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.MiscOrder)
            {
                //要求被排序单占用，实际被计划外出入库单占用
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuOccupiedByMiscOrder, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
            }
        }
        #endregion

        #region 检查库存是否被计划外出入库单占用
        private void CheckLocationLotDetailOccupiedByMiscOrder(LocationLotDetail locationLotDetail, string occupyReferenceNo, BusinessException businessException)
        {
            if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.MiscOrder && locationLotDetail.OccupyReferenceNo != occupyReferenceNo)
            {
                //要求被计划外出入库单占用，实际被其它捡货占用
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuOccupiedByMiscOrder, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
            }
            else if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.None)
            {
                //要求被计划外出入库单占用，实际没有被占用
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuNotOccupiedByMiscOrder, locationLotDetail.HuId, occupyReferenceNo);
            }
            else if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.Pick)
            {
                //要求被计划外出入库单占用，实际被拣货单占用
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuOccupiedByPickList, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
            }
            else if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.Inspect)
            {
                //要求被计划外出入库单占用，实际被检验单占用
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuOccupiedByInspectOrder, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
            }
            else if (locationLotDetail.OccupyType == com.Sconit.CodeMaster.OccupyType.Sequence)
            {
                //要求被计划外出入库单占用，实际被排序单占用
                throw new BusinessException(Resources.INV.LocationLotDetail.Errors_HuOccupiedBySequenceOrder, locationLotDetail.HuId, locationLotDetail.OccupyReferenceNo);
            }
        }
        #endregion
        #endregion

        #region 记录库存明细事务方法
        private void RecordLocationTransactionDetail(LocationTransaction locationTransaction, IList<InventoryTransaction> inventoryTransactionList)
        {
            if (bool.Parse(systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.IsRecordLocatoinTransactionDetail)))
            {
                foreach (InventoryTransaction inventoryTransaction in inventoryTransactionList)
                {
                    LocationTransactionDetail locationTransactionDetail = Mapper.Map<LocationTransaction, LocationTransactionDetail>(locationTransaction);
                    locationTransactionDetail.Qty = inventoryTransaction.Qty;  //库存单位
                    locationTransactionDetail.PlanBillQty = inventoryTransaction.PlanBillQty;
                    locationTransactionDetail.ActingBillQty = inventoryTransaction.ActingBillQty;
                    locationTransactionDetail.BillTransactionId = inventoryTransaction.BillTransactionId;
                    locationTransactionDetail.LocationLotDetailId = inventoryTransaction.LocationLotDetailId;
                    locationTransactionDetail.Bin = inventoryTransaction.Bin;
                    locationTransactionDetail.PlanBackflushId = inventoryTransaction.PlanBackflushId;

                    this.genericMgr.Create(locationTransactionDetail);
                }
            }
        }
        #endregion

        #region 加载库存明细中业务对象的方法
        private Location TryLoadLocation(InventoryIO inventoryIO)
        {
            if (inventoryIO.CurrentLocation == null)
            {
                inventoryIO.CurrentLocation = genericMgr.FindById<Location>(inventoryIO.Location);
            }

            return inventoryIO.CurrentLocation;
        }

        private PlanBill TryLoadPlanBill(InventoryIO inventoryIO)
        {
            if (inventoryIO.CurrentPlanBill == null)
            {
                inventoryIO.CurrentPlanBill = genericMgr.FindById<PlanBill>(inventoryIO.PlanBill);
            }

            return inventoryIO.CurrentPlanBill;
        }

        private ActingBill TryLoadActingBill(InventoryIO inventoryIO)
        {
            if (inventoryIO.CurrentActingBill == null)
            {
                inventoryIO.CurrentActingBill = genericMgr.FindById<ActingBill>(inventoryIO.ActingBill);
            }

            return inventoryIO.CurrentActingBill;
        }

        private Item TryLoadItem(InventoryIO inventoryIO)
        {
            if (inventoryIO.CurrentItem == null)
            {
                inventoryIO.CurrentItem = genericMgr.FindById<Item>(inventoryIO.Item);
            }

            return inventoryIO.CurrentItem;
        }
        #endregion

        #region 批量加载库位
        private IList<Location> BatchLoadLocations(IList<string> locationCodeList)
        {
            #region 查找库位
            string hql = string.Empty;
            IList<object> paras = new List<object>();
            foreach (string locationCode in locationCodeList)
            {
                if (hql == string.Empty)
                {
                    hql = "from Location where Code in (?";
                }
                else
                {
                    hql += ", ?";
                }
                paras.Add(locationCode);
            }
            hql += ")";
            return this.genericMgr.FindAll<Location>(hql, paras.ToArray());
            #endregion
        }
        #endregion

        #region 翻箱强制结算
        private void TryManualSettleBill(IList<InventoryTransaction> inventoryTransactionList, DateTime effectiveDate)
        {
            if (inventoryTransactionList.Select(i => i.IsConsignment).Distinct().Count() > 1
                || inventoryTransactionList.Where(i => i.IsConsignment && i.PlanBill.HasValue).Count() > 1)
            {
                foreach (InventoryTransaction inventoryTransaction in inventoryTransactionList)
                {
                    if (inventoryTransaction.IsConsignment)
                    {
                        PlanBill pb = this.genericMgr.FindById<PlanBill>(inventoryTransaction.PlanBill.Value);
                        pb.CurrentLocation = inventoryTransaction.Location;
                        pb.CurrentActingQty = -inventoryTransaction.Qty / pb.UnitQty; //结算单位
                        SettleBillTransaction billTransaction = this.billMgr.SettleBill(pb, effectiveDate);

                        inventoryTransaction.IsConsignment = false;
                        inventoryTransaction.PlanBillQty = 0;
                        inventoryTransaction.ActingBill = billTransaction.ActingBill;
                        inventoryTransaction.ActingBillQty = pb.CurrentActingQty * pb.UnitQty; //库存单位
                    }
                }
            }
        }
        #endregion

        [Transaction(TransactionMode.Requires)]
        public void SettleLocaitonLotDetail(List<Int64> locationLotDetIdList)
        {
            var datetimeNow = DateTime.Now;
            var user = SecurityContextHolder.Get();
            var locationLotDetailList = this.genericMgr.FindAllIn<LocationLotDetail>(" from LocationLotDetail where Id in(?",
                locationLotDetIdList.Select(p => (object)p));
            if (locationLotDetailList.Any(p => !p.IsConsignment || !p.PlanBill.HasValue))
            {
                throw new BusinessException("不是寄售库存不能结算");
            }
            var planBillDic = this.genericMgr.FindAllIn<PlanBill>(" from PlanBill where Id in(?",
                locationLotDetailList.Select(p => (object)p.PlanBill.Value))
                .ToDictionary(d => d.Id);
            var locationDic = this.genericMgr.FindAllIn<Location>(" from Location where Code in(?",
                locationLotDetailList.Select(p => p.Location).Distinct())
                .ToDictionary(d => d.Code);
            foreach (var locationLotDetail in locationLotDetailList)
            {
                //结算
                var planBill = planBillDic[locationLotDetail.PlanBill.Value];

                planBill.CurrentActingQty = -locationLotDetail.Qty / planBill.UnitQty; //按负数结算
                planBill.CurrentLocation = locationLotDetail.Location;
                planBill.CurrentHuId = locationLotDetail.HuId;

                var settleBillTrans = billMgr.SettleBill(planBill, datetimeNow);
                //记录库存事务
                //出库
                var location = locationDic[locationLotDetail.Location];
                if (string.IsNullOrWhiteSpace(locationLotDetail.BaseUom))
                {
                    locationLotDetail.BaseUom = itemMgr.GetCacheItem(locationLotDetail.Item).Uom;
                }
                LocationTransaction locationTransactionOut = new LocationTransaction();
                locationTransactionOut.OrderNo = locationLotDetail.Id.ToString();
                locationTransactionOut.Item = locationLotDetail.Item;
                locationTransactionOut.Uom = locationLotDetail.BaseUom;
                locationTransactionOut.BaseUom = locationLotDetail.BaseUom;
                locationTransactionOut.Qty = -locationLotDetail.Qty;
                locationTransactionOut.UnitQty = 1;
                locationTransactionOut.IsConsignment = true;
                locationTransactionOut.PlanBill = locationLotDetail.PlanBill.Value;
                locationTransactionOut.PlanBillQty = locationLotDetail.Qty;
                locationTransactionOut.ActingBill = 0;
                locationTransactionOut.ActingBillQty = 0;
                locationTransactionOut.QualityType = locationLotDetail.QualityType;
                locationTransactionOut.HuId = locationLotDetail.HuId;
                locationTransactionOut.LotNo = locationLotDetail.LotNo;
                locationTransactionOut.LocationIOReason = "寄售出";
                locationTransactionOut.TransactionType = CodeMaster.TransactionType.ISS_IIC;
                locationTransactionOut.IOType = CodeMaster.TransactionIOType.Out;
                locationTransactionOut.PartyFrom = location.Region;
                locationTransactionOut.PartyTo = location.Region;
                locationTransactionOut.LocationFrom = locationLotDetail.Location;
                locationTransactionOut.LocationTo = locationLotDetail.Location;
                locationTransactionOut.EffectiveDate = datetimeNow;
                locationTransactionOut.CreateUserId = user.Id;
                locationTransactionOut.CreateDate = datetimeNow;

                this.genericMgr.Create(locationTransactionOut);
                //入库
                LocationTransaction locationTransactionIn = new LocationTransaction();
                locationTransactionIn.OrderNo = locationLotDetail.Id.ToString();
                locationTransactionIn.Item = locationLotDetail.Item;
                locationTransactionIn.Uom = locationLotDetail.BaseUom;
                locationTransactionIn.BaseUom = locationLotDetail.BaseUom;
                locationTransactionIn.Qty = locationLotDetail.Qty;
                locationTransactionIn.UnitQty = 1;
                locationTransactionIn.IsConsignment = false;
                locationTransactionIn.PlanBill = 0;
                locationTransactionIn.PlanBillQty = 0;
                locationTransactionIn.ActingBill = settleBillTrans.ActingBill;
                locationTransactionIn.ActingBillQty = locationLotDetail.Qty;
                locationTransactionIn.QualityType = locationLotDetail.QualityType;
                locationTransactionIn.HuId = locationLotDetail.HuId;
                locationTransactionIn.LotNo = locationLotDetail.LotNo;
                locationTransactionIn.LocationIOReason = "自有入";
                locationTransactionIn.TransactionType = CodeMaster.TransactionType.RCT_IIC;
                locationTransactionIn.IOType = CodeMaster.TransactionIOType.In;
                locationTransactionIn.PartyFrom = location.Region;
                locationTransactionIn.PartyTo = location.Region;
                locationTransactionIn.LocationFrom = locationLotDetail.Location;
                locationTransactionIn.LocationTo = locationLotDetail.Location;
                locationTransactionIn.EffectiveDate = datetimeNow;
                locationTransactionIn.CreateUserId = user.Id;
                locationTransactionIn.CreateDate = datetimeNow;
                this.genericMgr.Create(locationTransactionIn);

                //更新库存
                locationLotDetail.IsConsignment = false;
                locationLotDetail.PlanBill = null;
                this.genericMgr.Update(locationLotDetail);
            }
        }
        #endregion
    }
}
