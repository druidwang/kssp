using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using com.Sconit.Entity.INP;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.SCM;
using com.Sconit.Persistence;
using System.Linq;
using com.Sconit.Entity.CUST;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.BIL;
using com.Sconit.Entity;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.TMS;
using com.Sconit.Entity.WMS;
using com.Sconit.Entity.FMS;

namespace com.Sconit.Service.Impl
{
    public class NumberControlMgrImpl : BaseMgr, INumberControlMgr
    {
        #region 变量
        public ISqlDao sqlDao { get; set; }
        #endregion

        #region public methods
        #region 获取序列号
        public string GetNextSequenceo(string codePrefix)
        {
            SqlParameter[] parm = new SqlParameter[2];

            parm[0] = new SqlParameter("@CodePrefix", SqlDbType.VarChar, 50);
            parm[0].Value = codePrefix;


            parm[1] = new SqlParameter("@NextSequence", SqlDbType.VarChar, 50);
            parm[1].Direction = ParameterDirection.InputOutput;

            sqlDao.ExecuteStoredProcedure("USP_SYS_GetNextSeq", parm);

            return parm[1].Value.ToString();
        }
        #endregion

        #region 获取订单号
        public string GetOrderNo(OrderMaster orderMaster)
        {
            SqlParameter[] parm = new SqlParameter[13];

            parm[0] = new SqlParameter("@Flow", SqlDbType.VarChar, 50);
            parm[0].Value = orderMaster.Flow;

            parm[1] = new SqlParameter("@OrderStrategy", SqlDbType.TinyInt);
            parm[1].Value = orderMaster.OrderStrategy;

            parm[2] = new SqlParameter("@Type", SqlDbType.TinyInt);
            parm[2].Value = orderMaster.Type;

            parm[3] = new SqlParameter("@SubType", SqlDbType.TinyInt);
            parm[3].Value = orderMaster.SubType;

            parm[4] = new SqlParameter("@QualityType", SqlDbType.TinyInt);
            parm[4].Value = orderMaster.QualityType;

            parm[5] = new SqlParameter("@Priority", SqlDbType.TinyInt);
            parm[5].Value = orderMaster.Priority;

            parm[6] = new SqlParameter("@PartyFrom", SqlDbType.VarChar, 50);
            parm[6].Value = orderMaster.PartyFrom;

            parm[7] = new SqlParameter("@PartyTo", SqlDbType.VarChar, 50);
            parm[7].Value = orderMaster.PartyTo;

            parm[8] = new SqlParameter("@LocTo", SqlDbType.VarChar, 50);
            parm[8].Value = orderMaster.LocationTo;

            parm[9] = new SqlParameter("@LocFrom", SqlDbType.VarChar, 50);
            parm[9].Value = orderMaster.LocationFrom;

            parm[10] = new SqlParameter("@Dock", SqlDbType.VarChar, 50);
            parm[10].Value = orderMaster.Dock;

            parm[11] = new SqlParameter("@IsQuick", SqlDbType.Bit);
            parm[11].Value = orderMaster.IsQuick;

            parm[12] = new SqlParameter("@OrderNo", SqlDbType.VarChar, 100);
            parm[12].Direction = ParameterDirection.InputOutput;

            sqlDao.ExecuteStoredProcedure("USP_GetDocNo_ORD", parm);

            return parm[12].Value.ToString();
        }
        #endregion

        #region 获取送货单号
        public string GetIpNo(Entity.ORD.IpMaster ipMaster)
        {
            //@DocumentsType, @OrderType, @QualityType, @PartyFrom, @PartyTo, @Dock, @ASNNo
            SqlParameter[] parm = new SqlParameter[7];

            parm[0] = new SqlParameter("@OrderType", SqlDbType.TinyInt);
            parm[0].Value = ipMaster.OrderType;

            parm[1] = new SqlParameter("@OrderSubType", SqlDbType.TinyInt);
            parm[1].Value = ipMaster.OrderSubType;

            parm[2] = new SqlParameter("@QualityType", SqlDbType.TinyInt);
            parm[2].Value = ipMaster.QualityType;

            parm[3] = new SqlParameter("@PartyFrom", SqlDbType.VarChar, 50);
            parm[3].Value = ipMaster.PartyFrom;

            parm[4] = new SqlParameter("@PartyTo", SqlDbType.VarChar, 50);
            parm[4].Value = ipMaster.PartyTo;

            parm[5] = new SqlParameter("@Dock", SqlDbType.VarChar, 50);
            parm[5].Value = ipMaster.Dock;

            parm[6] = new SqlParameter("@ASNNo", SqlDbType.VarChar, 100);
            parm[6].Direction = ParameterDirection.InputOutput;

            sqlDao.ExecuteStoredProcedure("USP_GetDocNo_ASN", parm);

            return parm[6].Value.ToString();
        }
        #endregion

        #region 获取收货单号
        public string GetReceiptNo(Entity.ORD.ReceiptMaster receiptMaster)
        {
            //@DocumentsType, @OrderType, @QualityType, @PartyFrom, @PartyTo, @Dock, @OrderNo
            SqlParameter[] parm = new SqlParameter[7];

            parm[0] = new SqlParameter("@OrderType", SqlDbType.TinyInt);
            parm[0].Value = receiptMaster.OrderType;

            parm[1] = new SqlParameter("@OrderSubType", SqlDbType.TinyInt);
            parm[1].Value = receiptMaster.OrderSubType;

            parm[2] = new SqlParameter("@QualityType", SqlDbType.TinyInt);
            parm[2].Value = receiptMaster.QualityType;

            parm[3] = new SqlParameter("@PartyFrom", SqlDbType.VarChar, 50);
            parm[3].Value = receiptMaster.PartyFrom;

            parm[4] = new SqlParameter("@PartyTo", SqlDbType.VarChar, 50);
            parm[4].Value = receiptMaster.PartyTo;

            parm[5] = new SqlParameter("@Dock", SqlDbType.VarChar, 50);
            parm[5].Value = receiptMaster.Dock;

            parm[6] = new SqlParameter("@RecNo", SqlDbType.VarChar, 100);
            parm[6].Direction = ParameterDirection.InputOutput;

            sqlDao.ExecuteStoredProcedure("USP_GetDocNo_REC", parm);

            return parm[6].Value.ToString();
        }
        #endregion

        #region 获取异常管理代码
        public string GetIssueNo(Entity.ISS.IssueMaster issueMaster)
        {
            return "ISS" + DateTime.Now.ToString("yyMMddHHmmssfff");
            throw new NotImplementedException();
        }
        #endregion

        #region 获取盘点单号
        public string GetStockTakeNo(StockTakeMaster stockTakeMaster)
        {
            SqlParameter[] parm = new SqlParameter[4];

            parm[0] = new SqlParameter("@Type", SqlDbType.TinyInt);
            parm[0].Value = stockTakeMaster.Type;

            parm[1] = new SqlParameter("@Location", SqlDbType.VarChar, 50);
            parm[1].Value = stockTakeMaster.Region;

            parm[2] = new SqlParameter("@IsScanHu", SqlDbType.Bit);
            parm[2].Value = stockTakeMaster.IsScanHu;

            parm[3] = new SqlParameter("@STTNo", SqlDbType.VarChar, 100);
            parm[3].Direction = ParameterDirection.InputOutput;

            sqlDao.ExecuteStoredProcedure("USP_GetDocNo_STT", parm);

            return parm[3].Value.ToString();
        }
        #endregion

        #region 获取检验单号
        public string GetInspectNo(InspectMaster inspectMaster)
        {
            SqlParameter[] parm = new SqlParameter[4];

            parm[0] = new SqlParameter("@Type", SqlDbType.TinyInt);
            parm[0].Value = inspectMaster.Type;

            parm[1] = new SqlParameter("@Region", SqlDbType.VarChar, 50);
            parm[1].Value = inspectMaster.Region;

            parm[2] = new SqlParameter("@IsAtp", SqlDbType.Bit);
            parm[2].Value = inspectMaster.IsATP;

            parm[3] = new SqlParameter("@InsNo", SqlDbType.VarChar, 100);
            parm[3].Direction = ParameterDirection.InputOutput;

            sqlDao.ExecuteStoredProcedure("USP_GetDocNo_INS", parm);

            return parm[3].Value.ToString();
        }
        #endregion

        #region 获取Rej单号
        public string GetRejNo(RejectMaster rejectMaster)
        {
            SqlParameter[] parm = new SqlParameter[3];

            parm[0] = new SqlParameter("@HandleResult", SqlDbType.TinyInt);
            parm[0].Value = rejectMaster.HandleResult;

            parm[1] = new SqlParameter("Region", SqlDbType.VarChar, 50);
            parm[1].Value = rejectMaster.Region;

            parm[2] = new SqlParameter("@RejNo", SqlDbType.VarChar, 100);
            parm[2].Direction = ParameterDirection.InputOutput;

            sqlDao.ExecuteStoredProcedure("USP_GetDocNo_REJ", parm);

            return parm[2].Value.ToString();

        }
        #endregion

        #region 获取排序单号
        public string GetSequenceNo(SequenceMaster sequenceMaster)
        {
            SqlParameter[] parm = new SqlParameter[9];

            parm[0] = new SqlParameter("@Flow", SqlDbType.VarChar, 50);
            parm[0].Value = sequenceMaster.Flow;

            parm[1] = new SqlParameter("@OrderType", SqlDbType.TinyInt);
            parm[1].Value = sequenceMaster.OrderType;

            parm[2] = new SqlParameter("@QualityType", SqlDbType.TinyInt);
            parm[2].Value = sequenceMaster.QualityType;

            parm[3] = new SqlParameter("@PartyFrom", SqlDbType.VarChar, 50);
            parm[3].Value = sequenceMaster.PartyFrom;

            parm[4] = new SqlParameter("@PartyTo", SqlDbType.VarChar, 50);
            parm[4].Value = sequenceMaster.PartyTo;

            parm[5] = new SqlParameter("@LocTo", SqlDbType.VarChar, 50);
            parm[5].Value = sequenceMaster.LocationTo;

            parm[6] = new SqlParameter("@LocFrom", SqlDbType.VarChar, 50);
            parm[6].Value = sequenceMaster.LocationFrom;

            parm[7] = new SqlParameter("@Dock", SqlDbType.VarChar, 50);
            parm[7].Value = sequenceMaster.Dock;

            parm[8] = new SqlParameter("@SeqNo", SqlDbType.VarChar, 100);
            parm[8].Direction = ParameterDirection.InputOutput;

            sqlDao.ExecuteStoredProcedure("USP_GetDocNo_SEQ", parm);

            return parm[8].Value.ToString();

        }
        #endregion

        #region 获取拣货单号
        public string GetPickListNo(PickListMaster pickListMaster)
        {
            //@DocumentsType, @OrderType, @PartyFrom, @PartyTo, @Dock, @OrderNo

            SqlParameter[] parm = new SqlParameter[5];

            parm[0] = new SqlParameter("@OrderType", SqlDbType.TinyInt);
            parm[0].Value = pickListMaster.OrderType;

            parm[1] = new SqlParameter("@PartyFrom", SqlDbType.VarChar, 50);
            parm[1].Value = pickListMaster.PartyFrom;

            parm[2] = new SqlParameter("@PartyTo", SqlDbType.VarChar, 50);
            parm[2].Value = pickListMaster.PartyTo;

            parm[3] = new SqlParameter("@Dock", SqlDbType.VarChar, 50);
            parm[3].Value = pickListMaster.Dock;

            parm[4] = new SqlParameter("@PikNo", SqlDbType.VarChar, 100);
            parm[4].Direction = ParameterDirection.InputOutput;

            sqlDao.ExecuteStoredProcedure("USP_GetDocNo_PIK", parm);

            return parm[4].Value.ToString();

        }
        #endregion

        #region 获取条码号


        public IDictionary<string, decimal> GetHuId(FlowDetail flowDetail)
        {
            decimal uc = flowDetail.UnitCount;

            if (flowDetail.HuQty <= flowDetail.MaxUc)
            {
                uc = flowDetail.HuQty;
            }

            var HuIds = GetHuId(flowDetail.LotNo, flowDetail.Item, flowDetail.ManufactureParty, flowDetail.HuQty, uc);

            return HuIds;
        }


        public IDictionary<string, decimal> GetHuId(OrderDetail orderDetail)
        {
            decimal uc = orderDetail.UnitCount;

            //if (orderDetail.HuQty <= orderDetail.MaxUnitCount)
            //{
            //    uc = orderDetail.HuQty;
            //}

            var HuIds = GetHuId(orderDetail.LotNo, orderDetail.Item, orderDetail.ManufactureParty, orderDetail.HuQty, uc);

            return HuIds;
        }

        public IDictionary<string, decimal> GetHuId(IpDetail ipDetail)
        {
            decimal uc = ipDetail.UnitCount;

            if (ipDetail.HuQty <= ipDetail.MaxUc)
            {
                uc = ipDetail.HuQty;
            }

            var HuIds = GetHuId(ipDetail.LotNo, ipDetail.Item, ipDetail.ManufactureParty, ipDetail.HuQty, uc);

            return HuIds;
        }

        public IDictionary<string, decimal> GetHuId(ReceiptDetail receiptDetail)
        {
            decimal uc = receiptDetail.UnitCount;
            if (receiptDetail.ReceivedQty <= receiptDetail.MaxUc && receiptDetail.ReceivedQty > 0)
            {
                uc = receiptDetail.ReceivedQty;
            }

            var HuIds = GetHuId(receiptDetail.LotNo, receiptDetail.Item, receiptDetail.ManufactureParty, receiptDetail.ReceivedQty, uc);

            return HuIds;
        }

        public IDictionary<string, decimal> GetHuId(Item item)
        {
            var HuIds = GetHuId(item.LotNo, item.Code, item.ManufactureParty, item.HuQty, item.HuUnitCount);

            return HuIds;
        }

        public IDictionary<string, decimal> GetHuId(string lotNo, string item, string manufactureParty, decimal qty, decimal unitCount)
        {
            if (qty / unitCount > 1000)
            {
                throw new BusinessException("一次创建的条码不能超过1000条");
            }
            if (unitCount == 1)
            {
                unitCount = qty;
            }

            IDictionary<string, decimal> HuIds = new Dictionary<string, decimal>();

            SqlParameter[] parm = new SqlParameter[5];

            parm[0] = new SqlParameter("@LotNo", SqlDbType.VarChar, 50);
            parm[0].Value = lotNo;

            parm[1] = new SqlParameter("@Item", SqlDbType.VarChar, 50);
            parm[1].Value = item;

            parm[2] = new SqlParameter("@Qty", SqlDbType.Decimal);
            parm[2].Precision = 18;
            parm[2].Scale = 8;
            parm[2].Value = qty;

            parm[3] = new SqlParameter("@UC", SqlDbType.Decimal);
            parm[3].Precision = 18;
            parm[3].Scale = 8;
            parm[3].Value = unitCount;

            parm[4] = new SqlParameter("@ManufactureParty", SqlDbType.VarChar, 50);
            parm[4].Value = manufactureParty;

            DataSet ds = sqlDao.GetDatasetByStoredProcedure("USP_GetDocNo_HUID", parm);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                HuIds.Add((string)dr.ItemArray[0], (decimal)dr.ItemArray[1]);
            }

            return HuIds;
        }


        #endregion

        #region 获取不合格品处理单号
        public string GetRejectNo(RejectMaster rejectMaster)
        {
            SqlParameter[] parm = new SqlParameter[3];

            parm[0] = new SqlParameter("@Region", SqlDbType.VarChar);
            parm[0].Value = rejectMaster.Region;

            parm[1] = new SqlParameter("@HandleResult", SqlDbType.TinyInt);
            parm[1].Value = rejectMaster.HandleResult;

            parm[2] = new SqlParameter("@RejNo", SqlDbType.VarChar, 100);
            parm[2].Direction = ParameterDirection.InputOutput;

            sqlDao.ExecuteStoredProcedure("USP_GetDocNo_REJ", parm);

            return parm[2].Value.ToString();
        }
        #endregion

        #region 获取让步使用单号
        public string GetConcessionNo(ConcessionMaster concessionOrder)
        {
            SqlParameter[] parm = new SqlParameter[2];

            parm[0] = new SqlParameter("@Region", SqlDbType.VarChar, 50);
            parm[0].Value = concessionOrder.Region;

            parm[1] = new SqlParameter("@ConNo", SqlDbType.VarChar, 100);
            parm[1].Direction = ParameterDirection.InputOutput;

            sqlDao.ExecuteStoredProcedure("USP_GetDocNo_CON", parm);

            return parm[1].Value.ToString();
        }
        #endregion

        #region 获取计划外出入库单号
        public string GetMiscOrderNo(MiscOrderMaster miscOrderMaster)
        {
            SqlParameter[] parm = new SqlParameter[6];

            parm[0] = new SqlParameter("@Type", SqlDbType.TinyInt);
            parm[0].Value = miscOrderMaster.Type;

            parm[1] = new SqlParameter("@IsScanHu", SqlDbType.Bit);
            parm[1].Value = miscOrderMaster.IsScanHu;

            parm[2] = new SqlParameter("@QualityType", SqlDbType.TinyInt);
            parm[2].Value = miscOrderMaster.QualityType;

            parm[3] = new SqlParameter("@Region", SqlDbType.VarChar, 50);
            parm[3].Value = miscOrderMaster.Region;

            parm[4] = new SqlParameter("@Location", SqlDbType.VarChar, 50);
            parm[4].Value = miscOrderMaster.Location;

            parm[5] = new SqlParameter("@MisNo", SqlDbType.VarChar, 100);
            parm[5].Direction = ParameterDirection.InputOutput;

            sqlDao.ExecuteStoredProcedure("USP_GetDocNo_MIS", parm);

            return parm[5].Value.ToString();

        }
        #endregion

        #region 获取车辆进出凭证单号
        public string GetVehicleInFactoryNo(VehicleInFactoryMaster vehicleInFactoryMaster)
        {
            SqlParameter[] parm = new SqlParameter[3];

            parm[0] = new SqlParameter("@Plant", SqlDbType.VarChar, 50);
            parm[0].Value = vehicleInFactoryMaster.Plant;

            parm[1] = new SqlParameter("@Status", SqlDbType.Int);
            parm[1].Value = vehicleInFactoryMaster.Status;

            parm[2] = new SqlParameter("@VehNo", SqlDbType.VarChar, 100);
            parm[2].Direction = ParameterDirection.InputOutput;

            sqlDao.ExecuteStoredProcedure("USP_GetDocNo_VEH", parm);

            return parm[2].Value.ToString();
        }
        #endregion

        #region 获取看板卡序号
        public string GetKanBanCardNo()
        {
            string cardNo = DateTime.Now.ToString("yyMMddHHmmss");
            // int seekSeek = unchecked((int)DateTime.Now.Ticks);
            // Random seekRand = new Random(seekSeek);
            // return cardNo + ("0000"+seekRand.Next(1, 9999).ToString()).Substring(0, 4);
            return cardNo;
        }
        #endregion


        #region 获取容器号


        public string GetContainerId(string prefix)
        {
            var containerId = GetNextSequenceo(prefix);

            containerId = containerId.PadLeft(10, '0');
            return (prefix + containerId);
        }

        #endregion

        public string GetBillNo(BillMaster billMaster)
        {
            if (billMaster.Type == CodeMaster.BillType.Procurement)
            {
                string numberSuffix = GetNextSequence(BusinessConstants.NUMBERCONTROL_PROCUREMENTBILL);
                numberSuffix = numberSuffix.PadLeft(9, '0');
                return ("B1" + numberSuffix);
            }
            else if (billMaster.Type == CodeMaster.BillType.Distribution)
            {
                string numberSuffix = GetNextSequence(BusinessConstants.NUMBERCONTROL_DISTRIBUTIONBILL);
                numberSuffix = numberSuffix.PadLeft(9, '0');
                return ("B2" + numberSuffix);
            }
            else
            {
                throw new Exception("没有定义此账单类型");
            }
        }

        public string GetShipmentNo()
        {
            string numberSuffix = GetNextSequence(BusinessConstants.NUMBERCONTROL_SHIPMENT);
            numberSuffix = numberSuffix.PadLeft(10, '0');
            return ("T" + numberSuffix);
        }

        public string GetTaskNo(string prefix)
        {
            string numberSuffix = GetNextSequence(prefix);
            numberSuffix = numberSuffix.PadLeft(10, '0');
            return prefix + numberSuffix;
        }
        #endregion

        #region 获取运输单号
        public string GetTransportOrderNo(TransportOrderMaster transportOrderMaster)
        {
            string numberSuffix = GetNextSequence(BusinessConstants.NUMBERCONTROL_TRANSPORTATION);
            numberSuffix = numberSuffix.PadLeft(10, '0');
            return "T" + numberSuffix;
        }
        #endregion


        #region 获取配送标签号


        public IDictionary<string, decimal> GetDeliveryBarCode(ShipPlan shipPlan)
        {

            var barCodes = GetDeliveryBarCode(shipPlan.Item, shipPlan.ToDeliveryBarCodeQty, shipPlan.UnitCount);

            return barCodes;
        }


        public IDictionary<string, decimal> GetDeliveryBarCode(string item, decimal qty, decimal unitCount)
        {
            if (qty / unitCount > 1000)
            {
                throw new BusinessException("一次创建的配送标签不能超过1000条");
            }

            IDictionary<string, decimal> HuIds = new Dictionary<string, decimal>();

            SqlParameter[] parm = new SqlParameter[5];

            parm[0] = new SqlParameter("@Item", SqlDbType.VarChar, 50);
            parm[0].Value = item;

            parm[1] = new SqlParameter("@Qty", SqlDbType.Decimal);
            parm[1].Precision = 18;
            parm[1].Scale = 8;
            parm[1].Value = qty;

            parm[2] = new SqlParameter("@UC", SqlDbType.Decimal);
            parm[2].Precision = 18;
            parm[2].Scale = 8;
            parm[2].Value = unitCount;



            DataSet ds = sqlDao.GetDatasetByStoredProcedure("USP_GetDocNo_DBC", parm);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                HuIds.Add((string)dr.ItemArray[0], (decimal)dr.ItemArray[1]);
            }

            return HuIds;
        }


        #endregion


        #region 获取装箱单号
        public string GetPackingListCode()
        {
            string numberSuffix = GetNextSequence(BusinessConstants.NUMBERCONTROL_PACKINGLIST);
            numberSuffix = numberSuffix.PadLeft(10, '0');
            return "PL" + numberSuffix;
        }
        #endregion


        #region 运输账单编号
        public string GetTransportBillNo(TransportBillMaster billMaster)
        {
            string numberSuffix = GetNextSequence(BusinessConstants.NUMBERCONTROL_PROCUREMENTBILL);
            numberSuffix = numberSuffix.PadLeft(9, '0');
            return ("B3" + numberSuffix);
        }
        #endregion


        #region 设施编号
        public string GetFCID(FacilityMaster facilityMaster)
        {
            {
                string numberSuffix = GetNextSequence(BusinessConstants.NUMBERCONTROL_FACILITY);
                numberSuffix = numberSuffix.PadLeft(9, '0');
                return ("FC" + numberSuffix);
            }
        }
        #endregion

        public string GetTraceCode()
        {
            {
                string numberSuffix = GetNextSequence(BusinessConstants.NUMBERCONTROL_TRACECODE);
                numberSuffix = numberSuffix.PadLeft(9, '0');
                return ("T" + numberSuffix);
            }
        }


        /// <summary>
        /// 托盘编号
        /// </summary>
        /// <returns></returns>
        public string GetPalletCode()
        {
            {
                string numberSuffix = GetNextSequence(BusinessConstants.NUMBERCONTROL_PALLETCODE);
                numberSuffix = numberSuffix.PadLeft(9, '0');
                return ("TP" + numberSuffix);
            }
        }

        #region private methods
        public String GetNextSequence(string code)
        {
            SqlParameter[] parm = new SqlParameter[2];

            parm[0] = new SqlParameter("@CodePrefix", SqlDbType.VarChar, 50);
            parm[0].Value = code;

            parm[1] = new SqlParameter("@NextSequence", SqlDbType.Int, 50);
            parm[1].Direction = ParameterDirection.InputOutput;

            sqlDao.ExecuteStoredProcedure("USP_SYS_GetNextSeq", parm);

            return parm[1].Value.ToString();
        }
        #endregion
    }
}
