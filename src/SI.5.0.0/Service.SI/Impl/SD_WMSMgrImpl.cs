using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.SI.SD_WMS;
using AutoMapper;
using Castle.Services.Transaction;
using com.Sconit.Entity.VIEW;
using LeanEngine.Entity;
using NHibernate;
using com.Sconit.Entity;

namespace com.Sconit.Service.SI.Impl
{
    public class SD_WMSMgrImpl : BaseMgr, ISD_WMSMgr  
    {
        public IPickTaskMgr pickTaskMgr { get; set; }

        public List<PickTask> GetPickTaskByUser(int pickUserId, bool isPickByHu)
        {
            int pickBy = isPickByHu?(int)CodeMaster.PickBy.Hu:(int)CodeMaster.PickBy.LotNo;
            IList<com.Sconit.Entity.WMS.PickTask> pickTaskList = this.genericMgr.FindAll<com.Sconit.Entity.WMS.PickTask>("from PickTask p where p.PickUserId=? and p.PickBy=? and p.IsActive = 1 and p.OrderQty > PickQty", new object[] { pickUserId, pickBy });

            return Mapper.Map<List<Entity.WMS.PickTask>, List<Entity.SI.SD_WMS.PickTask>>(pickTaskList.ToList());
        }

        [Transaction(TransactionMode.Requires)]
        public Entity.SI.SD_INV.Hu GetPickHu(string huId)
        {
            try
            {
                HuStatus huStatus = huMgr.GetHuStatus(huId.ToUpper());
                var hu = Mapper.Map<HuStatus, Entity.SI.SD_INV.Hu>(huStatus);
                if (string.IsNullOrEmpty(hu.Location))
                {
                    throw new BusinessException(string.Format("条码{0}不在库存中。", huId));
                }
                var occupy = this.genericMgr.FindAll<com.Sconit.Entity.WMS.BufferInventory>("from BufferInventory bi where bi.HuId = ? ", hu.HuId);
                if (occupy != null && occupy.Count > 0)
                {
                    throw new BusinessException(string.Format("条码{0}已被其他拣货任务占用。", huId));
                }
                return hu;
            }
            catch (ObjectNotFoundException)
            {
                throw new BusinessException(string.Format("条码{0}不存在。", huId));
            }
        }


        [Transaction(TransactionMode.Requires)]
        public Entity.SI.SD_INV.Hu GetDeliverMatchHu(string huId)
        {
            try
            {
                HuStatus huStatus = huMgr.GetHuStatus(huId.ToUpper());
                var hu = Mapper.Map<HuStatus, Entity.SI.SD_INV.Hu>(huStatus);
                if (string.IsNullOrEmpty(hu.Location))
                {
                    throw new BusinessException(string.Format("条码{0}不在库存中。", huId));
                }
                var inBuffer = this.genericMgr.FindAll<com.Sconit.Entity.WMS.BufferInventory>("from BufferInventory bi where bi.HuId = ?", hu.HuId);
                if (inBuffer == null && inBuffer.Count == 0)
                {
                    throw new BusinessException(string.Format("条码{0}未被拣货任务占用。", huId));
                }
                var occupy = this.genericMgr.FindEntityWithNativeSql<com.Sconit.Entity.WMS.BufferOccupy>("select bo.* from WMS_BuffOccupy bo left join WMS_BuffInv bi on bo.UUID=bi.UUID where bi.HuId = ?", hu.HuId);
                if (occupy != null && occupy.Count > 0)
                {
                    throw new BusinessException(string.Format("条码{0}已被其他发货计划锁定。", huId));
                }
                hu.AgingLocation = inBuffer.FirstOrDefault().Dock;
                return hu;
            }
            catch (ObjectNotFoundException)
            {
                throw new BusinessException(string.Format("条码{0}不存在。", huId));
            }
        }

        public void DoPickTask(List<Entity.SI.SD_INV.Hu> huList)
        {
            try
            {
                Dictionary<int, List<string>> pickResult = new Dictionary<int, List<string>>();
                foreach (var hu in huList)
                {
                    if (pickResult.ContainsKey(hu.OrderDetId))
                    {
                        pickResult[hu.OrderDetId].Add(hu.HuId);
                    }
                    else
                    {
                        var values = new List<string>();
                        values.Add(hu.HuId);
                        pickResult.Add(hu.OrderDetId, values);
                    }
                }
                pickTaskMgr.PorcessPickResult4PickLotNoAndHu(pickResult);
                if (MessageHolder.GetErrorMessages()!=null && MessageHolder.GetErrorMessages().Count > 0)
                {
                    throw new BusinessException(MessageHolder.GetErrorMessages().FirstOrDefault().GetMessageString());
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }


        public Entity.SI.SD_WMS.DeliverBarCode GetDeliverBarCode(string barCode)
        { 
            try
            {
                var deliverBarCode = this.genericMgr.FindById<Entity.WMS.DeliveryBarCode>(barCode);
                if (!string.IsNullOrEmpty(deliverBarCode.HuId))
                {
                    throw new BusinessException(string.Format("配送标签{0}已关联条码{1}。", barCode, deliverBarCode.HuId));
                }
                if (deliverBarCode.IsActive == true)
                {
                    throw new BusinessException(string.Format("配送标签{0}已被占用。", barCode));
                }
                return Mapper.Map<Entity.WMS.DeliveryBarCode, Entity.SI.SD_WMS.DeliverBarCode>(deliverBarCode);
            }
            catch (ObjectNotFoundException)
            {
                throw new BusinessException(string.Format("配送标签{0}不存在。", barCode));
            }
        }

        public void MatchDCToHU(string huId, string barCode)
        {
            try
            {
                this.pickTaskMgr.PorcessDeliverBarCode2Hu(barCode, huId);
                if (MessageHolder.GetErrorMessages() != null && MessageHolder.GetErrorMessages().Count > 0)
                {
                    throw new BusinessException(MessageHolder.GetErrorMessages().FirstOrDefault().GetMessageString());
                }
            }
            catch (ObjectNotFoundException)
            {
                throw new BusinessException(string.Format("配送标签{0}不存在。", barCode));
            }
        }

        public void TransferToDock(List<string> huIds, string dock)
        {
            try
            {
                string sql = "update BufferInventory set Dock = ? where HuId in(";
                List<object> param = new List<object>();
                param.Add(dock);
                for(int i=0;i< huIds.Count;++i)
                {
                    if(i==0)
                    {
                        sql+="?";
                    }
                    else
                    {
                        sql+=",?";
                    }
                    param.Add(huIds[i]);
                }
                this.genericMgr.FindAllWithNativeSql(sql,param);
            }
            catch (Exception ex)
            {
                throw new BusinessException(ex.Message);
            }
        }


        public Entity.SI.SD_INV.Hu GetShipHu(string huId, string deliverBarCode)
        {
            if (!string.IsNullOrEmpty(huId))
            {
                var dcs = this.genericMgr.FindAll<Entity.WMS.DeliveryBarCode>("from DeliveryBarCode dc where dc.HuId=?", huId);
                if (dcs == null || dcs.Count == 0)
                {
                    throw new BusinessException("条码未匹配配送标签。");
                }
                else
                {
                    var dc = dcs.FirstOrDefault();
                    deliverBarCode = dc.BarCode;
                }
            }
            else
            {
                var dc = this.genericMgr.FindById<Entity.WMS.DeliveryBarCode>(deliverBarCode);
                if (string.IsNullOrEmpty(dc.HuId))
                {
                    throw new BusinessException(string.Format("配送标签{0}未关联条码。", deliverBarCode));
                }
                else
                {
                    huId = dc.HuId;
                }
            }
            var inBuffer = this.genericMgr.FindAll<com.Sconit.Entity.WMS.BufferInventory>("from BufferInventory bi where bi.HuId = ?", huId);
            if (inBuffer == null && inBuffer.Count == 0)
            {
                throw new BusinessException(string.Format("条码{0}不在拣货区库存中。", huId));
            }
            HuStatus huStatus = huMgr.GetHuStatus(huId.ToUpper());
            var hu = Mapper.Map<HuStatus, Entity.SI.SD_INV.Hu>(huStatus);
            return hu;
        }
    }
}
