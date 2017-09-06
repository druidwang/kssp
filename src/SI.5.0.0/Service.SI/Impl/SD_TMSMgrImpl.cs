using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeanEngine.Entity;
using com.Sconit.Entity.SI.SD_TMS;
using AutoMapper;

namespace com.Sconit.Service.SI.Impl
{
    public class SD_TMSMgrImpl : BaseMgr, ISD_TMSMgr  
    {
        public IOrderMgr orderMgr { get; set; }

        public TransportOrderMaster GetTransOrder(string orderNo)
        {
            var transOrderMaster = new Entity.TMS.TransportOrderMaster();
            if (!string.IsNullOrEmpty(orderNo))
            {
                transOrderMaster = this.genericMgr.FindById<Entity.TMS.TransportOrderMaster>(orderNo);
                if (transOrderMaster == null || string.IsNullOrEmpty(transOrderMaster.OrderNo))
                {
                    throw new BusinessException("运单号错误。");
                }
                var sdTransOrderMaster = Mapper.Map<Entity.TMS.TransportOrderMaster, TransportOrderMaster>(transOrderMaster);
                transOrderMaster.TransportOrderDetailList = this.genericMgr.FindAll<Entity.TMS.TransportOrderDetail>("from TransportOrderDetail to where to.OrderNo=?", orderNo);
                if (transOrderMaster.TransportOrderDetailList.Count > 0)
                {
                    Mapper.Map<IList<Entity.TMS.TransportOrderDetail>, List<TransportOrderDetail>>(transOrderMaster.TransportOrderDetailList).OrderBy(s => s.Sequence).ToList();
                }
                return sdTransOrderMaster;
            }
            else
            {
                throw new BusinessException("请输入运单号。");
            }
        }

        public void Ship(string transOrder, List<string> huIds)
        {
            orderMgr.ProcessShipPlanResult4Hu(transOrder, huIds, DateTime.Now);
        }
    }
}
