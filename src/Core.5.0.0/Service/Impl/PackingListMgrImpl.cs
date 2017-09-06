using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.INV;
using System.Collections;
using AutoMapper;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using NHibernate;
using com.Sconit.PrintModel.ORD;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.WMS;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class PackingListMgrImpl : BaseMgr, IPackingListMgr
    {
        #region 变量
        public IGenericMgr genericMgr { get; set; }
        public INumberControlMgr numberControlMgr { get; set; }
        public IOrderMgr orderMgr { get; set; }
        #endregion

        #region public methods
        [Transaction(TransactionMode.Requires)]
        public PackingList CreatePackingList(string flow, IList<string> huIdList)
        {
            #region 查找对应库存
            string sqlStr = string.Empty;
            IList<object> param = new List<object>();

            foreach (string h in huIdList)
            {
                if (string.IsNullOrEmpty(sqlStr))
                {
                    sqlStr += "select b from BufferInventory as b where b.HuId in (?";
                    param.Add(h);
                }
                else
                {
                    sqlStr += ",?";
                    param.Add(h);
                }
            }
            if (!string.IsNullOrEmpty(sqlStr))
            {
                sqlStr += ") and b.Qty > 0";
            }
            IList<BufferInventory> bufferInvList = genericMgr.FindAll<BufferInventory>(sqlStr, param.ToList());


            string oSqlStr = string.Empty;
            IList<object> oParam = new List<object>();

            foreach (BufferInventory b in bufferInvList)
            {
                if (string.IsNullOrEmpty(oSqlStr))
                {
                    oSqlStr += "select b from BufferOccupy as b where b.UUID in (?";
                    oParam.Add(b.UUID);
                }
                else
                {
                    oSqlStr += ",?";
                    oParam.Add(b.UUID);
                }
            }
            if (!string.IsNullOrEmpty(oSqlStr))
            {
                oSqlStr += ")";
            }
            IList<BufferOccupy> bufferOccupyList = genericMgr.FindAll<BufferOccupy>(oSqlStr, oParam.ToList());
            #endregion

            PackingList packingList = new PackingList();

            string code = numberControlMgr.GetPackingListCode();
            packingList.PackingListCode = code;
            packingList.Flow = flow;
            packingList.IsActive = false;
            genericMgr.Create(packingList);

            #region 明细
            foreach (BufferInventory bufferInv in bufferInvList)
            {
                PackingListDetail PackingListDetail = new PackingListDetail();
                PackingListDetail.PackingListCode = code;
                BufferOccupy bufferOccupy = bufferOccupyList.Where(p => p.UUID == bufferInv.UUID).FirstOrDefault();
                PackingListDetail.Dock = bufferInv.Dock;
                PackingListDetail.HuId = bufferInv.HuId;
                PackingListDetail.Item = bufferInv.Item;
                PackingListDetail.Location = bufferInv.Location;
                PackingListDetail.LotNo = bufferInv.LotNo;
                PackingListDetail.Qty = bufferInv.Qty;
                PackingListDetail.UnitCount = bufferInv.UnitCount;
                PackingListDetail.Uom = bufferInv.Uom;

                PackingListDetail.OrderNo = bufferOccupy.OrderNo;
                PackingListDetail.OrderSeq = bufferOccupy.OrderSeq;
                PackingListDetail.ShipPlanId = bufferOccupy.ShipPlanId;
                genericMgr.Create(PackingListDetail);


                bufferInv.IsPack = true;
                genericMgr.Update(bufferInv);
            }
            #endregion
            return packingList;
        }

        [Transaction(TransactionMode.Requires)]
        public void Ship(IList<string> packingListCodeList)
        {
            #region 查找对应条码
            string sqlStr = string.Empty;
            IList<object> param = new List<object>();

            foreach (string p in packingListCodeList)
            {
                if (string.IsNullOrEmpty(sqlStr))
                {
                    sqlStr += "select d from PackingListDetail as d where d.PackingListCode in (?";
                    param.Add(p);
                }
                else
                {
                    sqlStr += ",?";
                    param.Add(p);
                }
            }
            if (!string.IsNullOrEmpty(sqlStr))
            {
                sqlStr += ")";
            }
            IList<PackingListDetail> packingListDetailList = genericMgr.FindAll<PackingListDetail>(sqlStr, param.ToList());
            IList<string> huIdList = packingListDetailList.Select(p => p.HuId).ToList();
            #endregion

            orderMgr.ProcessShipPlanResult4Hu(string.Empty, huIdList, DateTime.Now);
        }
        #endregion

    }
}
