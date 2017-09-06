namespace com.Sconit.WebService
{
    using System.Collections.Generic;
    using System.Web.Services;
    using System.Web.Services.Protocols;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Entity.SI.SD_INV;
    using com.Sconit.Entity.SI.SD_ORD;
    using com.Sconit.Entity.SI.SD_WMS;
    using com.Sconit.Service.SI;
    using com.Sconit.Entity;
    using System;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Entity.SI.SD_TMS;

    [WebService(Namespace = "http://com.Sconit.WebService.SD.SmartDeviceService/")]
    public class SD_SmartDeviceService : BaseWebService
    {
        private ISD_OrderMgr orderMgr { get { return GetService<ISD_OrderMgr>(); } }

        private ISD_InventoryMgr inventoryMgr { get { return GetService<ISD_InventoryMgr>(); } }

        private ISD_SecurityMgr sdSecurityMgr { get { return GetService<ISD_SecurityMgr>(); } }

        private ISD_FlowMgr flowMgr { get { return GetService<ISD_FlowMgr>(); } }

        private ISD_MasterDataMgr masterDataMgr { get { return GetService<ISD_MasterDataMgr>(); } }

        private ISD_WMSMgr wmsMgr { get { return GetService<ISD_WMSMgr>(); } }

        private ISD_TMSMgr tmsMgr { get { return GetService<ISD_TMSMgr>(); } }

        private void ProcessException(Exception ex)
        {
            if (ex is BusinessException)
            {
                throw new SoapException(ex.Message, SoapException.ServerFaultCode, string.Empty);
            }
            else
            {
                throw new SoapException(ex.Message, SoapException.ServerFaultCode, string.Empty);
            }
        }

        #region public methods
        [WebMethod]
        public Entity.SI.SD_ACC.User GetUser(string userCode, string hashedPassword)
        {
            try
            {
                var user = sdSecurityMgr.GetUser(userCode, hashedPassword, Context.Request.UserHostAddress);
                AccessLog accessLog = new AccessLog();
                accessLog.CreateDate = DateTime.Now;
                accessLog.CsBrowser = "SmartDevice";
                accessLog.UserAgent = Context.Request.UserAgent;
                accessLog.CsIP = Context.Request.UserHostAddress;
                accessLog.PageUrl = Context.Request.RawUrl;
                accessLog.PageName = "用户登录成功";
                accessLog.UserCode = userCode;
                accessLog.UserName = string.Format("{0}{1}", user.FirstName, user.LastName);
                sdSecurityMgr.CreateAccessLog(accessLog);

                return user;
            }
            catch (BusinessException ex)
            {
                string errorMessage = GetBusinessExMessage(ex);
                AccessLog accessLog = new AccessLog();
                accessLog.CreateDate = DateTime.Now;
                accessLog.CsBrowser = "SmartDevice";
                accessLog.UserAgent = Context.Request.UserAgent;
                accessLog.CsIP = Context.Request.UserHostAddress;
                accessLog.PageUrl = errorMessage;
                accessLog.PageName = "用户登录失败";
                accessLog.UserCode = userCode;
                //accessLog.UserName = string.Format("{0}{1}", user.FirstName, user.LastName);
                sdSecurityMgr.CreateAccessLog(accessLog);
                throw new SoapException(errorMessage, SoapException.ServerFaultCode, string.Empty);
            }
        }

        #region Get
        [WebMethod]
        public OrderMaster GetOrder(string orderNo, bool includeDetail)
        {
            try
            {
                return this.orderMgr.GetOrder(orderNo, includeDetail);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public OrderMaster GetOrderByOrderNoAndExtNo(string orderNo, bool includeDetail)
        {
            try
            {
                return this.orderMgr.GetOrderByOrderNoAndExtNo(orderNo, includeDetail);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public Hu GetMaterialInHu(string orderNo, string hu, string station)
        {
            try
            {
                return this.inventoryMgr.GetHu(hu);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public IpMaster GetIp(string ipNo, bool includeDetail)
        {
            try
            {
                return this.orderMgr.GetIp(ipNo, includeDetail);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public PickListMaster GetPickList(string pickListNo, bool includeDetail)
        {
            try
            {
                return this.orderMgr.GetPickList(pickListNo, includeDetail);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public InspectMaster GetInspect(string inspectNo, bool includeDetail)
        {
            try
            {
                return this.orderMgr.GetInspect(inspectNo, includeDetail);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public Hu GetHu(string huId)
        {
            try
            {
                return this.inventoryMgr.GetHu(huId);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public List<Entity.SI.SD_INV.Hu> GetHuListByPallet(string palletCode)
        {
            try
            {
                return this.inventoryMgr.GetPalletHu(palletCode);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public Hu CloneHu(string huId, decimal qty, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                return this.inventoryMgr.CloneHu(huId, qty);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public MiscOrderMaster GetMisOrder(string misNo)
        {
            try
            {
                return this.orderMgr.GetMis(misNo);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void BatchUpdateMiscOrderDetails(string miscOrderNo,
            List<string> addHuIdList, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.BatchUpdateMiscOrderDetails(miscOrderNo, addHuIdList);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void ConfirmMiscOrder(string miscOrderNo,
            List<string> addHuIdList, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.ConfirmMiscOrder(miscOrderNo, addHuIdList);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void QuickCreateMiscOrder(List<string> addHuIdList, string locationCode, string binCode, int type, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.QuickCreateMiscOrder(addHuIdList, locationCode, binCode, type);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public List<string> GetItemTraces(string orderNo)
        {
            return orderMgr.GetItemTraces(orderNo);
        }
        #endregion

        #region Do
        [WebMethod]
        public string DoShipOrder(List<Entity.SI.SD_ORD.OrderDetailInput> orderDetailInputList, DateTime? effDate, string userCode,bool isOpPallet)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                return this.orderMgr.DoShipOrder(orderDetailInputList, effDate,isOpPallet);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void DoPickList(List<Entity.SI.SD_ORD.PickListDetailInput> pickListDetailInputList, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.DoPickList(pickListDetailInputList);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public string DoReceiveOrder(List<Entity.SI.SD_ORD.OrderDetailInput> orderDetailInputList, DateTime? effDate, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                return this.orderMgr.DoReceiveOrder(orderDetailInputList, effDate);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public string DoReceiveIp(List<Entity.SI.SD_ORD.IpDetailInput> ipDetailInputList, DateTime? effDate, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                return this.orderMgr.DoReceiveIp(ipDetailInputList, effDate);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void DoPutAway(string huId, string binCode, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.inventoryMgr.DoPutAway(huId, binCode);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void DoPickUp(string huId, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.inventoryMgr.DoPickUp(huId);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void DoPack(List<string> huIdList, string location, DateTime? effDate, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.inventoryMgr.DoPack(huIdList, location, effDate);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void DoUnPack(List<string> huIdList, DateTime? effDate, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.inventoryMgr.DoUnPack(huIdList, effDate);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void DoRePack(List<string> oldHuList, List<string> newHuList, DateTime? effDate, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.inventoryMgr.DoRePack(oldHuList, newHuList, effDate);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }


        [WebMethod]
        public void DoInspect(List<string> huIdList, DateTime? effDate, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.DoInspect(huIdList, effDate);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void DoWorkersWaste(List<string> huIdList, DateTime? effDate, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.DoWorkersWaste(huIdList, effDate);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void DoRepackAndShipOrder(List<Entity.SI.SD_INV.Hu> huList, DateTime? effDate, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.DoRepackAndShipOrder(huList, effDate);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void DoItemTrace(string orderNo, List<string> huIdList, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.DoItemTrace(orderNo, huIdList);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void CancelItemTrace(string orderNo, List<string> huIdList, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.CancelItemTrace(orderNo, huIdList);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }
        #endregion

        #region MasterData
        [WebMethod]
        public string GetEntityPreference(Entity.SYS.EntityPreference.CodeEnum entityEnum)
        {
            try
            {
                return this.masterDataMgr.GetEntityPreference(entityEnum);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public Entity.SI.SD_MD.Bin GetBin(string binCode)
        {
            try
            {
                return this.masterDataMgr.GetBin(binCode);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public Entity.SI.SD_MD.Location GetLocation(string locationCode)
        {
            try
            {
                return this.masterDataMgr.GetLocation(locationCode);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public Entity.SI.SD_MD.Item GetItem(string itemCode)
        {
            try
            {
                return this.masterDataMgr.GetItem(itemCode);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public DateTime GetEffDate(string date)
        {
            try
            {
                return this.masterDataMgr.GetEffDate(date);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }
        #endregion


        [WebMethod]
        public Entity.SI.SD_SCM.FlowMaster GetFlowMaster(string flowCode, bool includeDetail)
        {
            try
            {
                return this.flowMgr.GetFlowMaster(flowCode, includeDetail);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public Entity.SI.SD_MD.Pallet GetPallet(string palletCode)
        {
            try
            {
                return this.masterDataMgr.GetPallet(palletCode);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        #endregion


        [WebMethod]
        public void DoTransfer(Entity.SI.SD_SCM.FlowMaster flowMaster, List<Entity.SI.SD_SCM.FlowDetailInput> flowDetailInputList, string userCode,bool isFifo = true,bool isOpPallet = false)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.DoTransfer(flowMaster, flowDetailInputList, isFifo, isOpPallet);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void DoAnDon(List<AnDonInput> anDonInputList, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.DoAnDon(anDonInputList);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void StartPickList(string pickListNo, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.StartPickList(pickListNo);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public string ShipPickList(string pickListNo, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                return this.orderMgr.ShipPickList(pickListNo);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public Entity.SI.SD_INV.StockTakeMaster GetStockTake(string stNo)
        {
            try
            {
                return this.inventoryMgr.GetStockTake(stNo);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void DoStockTake(string stNo, string[][] stockTakeDetails, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.inventoryMgr.DoStockTake(stNo, stockTakeDetails);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        //投料到生产线
        [WebMethod]
        public void FeedFlowRawMaterial(string productLine, string productLineFacility, string location, List<com.Sconit.Entity.SI.SD_INV.Hu> hus, bool isForceFeed, string userCode, DateTime? effectiveDate)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.FeedProdLineRawMaterial(productLine, productLineFacility, location, hus, isForceFeed, effectiveDate);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }

        }

        //投料到生产单
        [WebMethod]
        public void FeedOrderRawMaterial(string orderNo, string location, List<com.Sconit.Entity.SI.SD_INV.Hu> hus, bool isForceFeed, string userCode, DateTime? effectiveDate)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.FeedOrderRawMaterial(orderNo, location, hus, isForceFeed, effectiveDate);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }
        //KIT投料到生产单
        [WebMethod]
        public void FeedKitOrder(string orderNo, string kitOrderNo, bool isForceFeed, string userCode, DateTime? effectiveDate)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.FeedKitOrder(orderNo, kitOrderNo, isForceFeed, effectiveDate);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        //生产单投料到生产单
        [WebMethod]
        public void FeedProductOrder(string orderNo, string productOrderNo, bool isForceFeed, string userCode, DateTime? effectiveDate)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.FeedProductOrder(orderNo, productOrderNo, isForceFeed, effectiveDate);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void ReturnProdLineRawMaterial(string productLine, string productLineFacility, string[][] huDetails, DateTime? effectiveDate, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.ReturnProdLineRawMaterial(productLine, productLineFacility, huDetails, effectiveDate);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void ReturnOrderRawMaterial(string orderNo, string traceCode, int? operation, string opReference, string[][] huDetails, DateTime? effectiveDate, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.ReturnOrderRawMaterial(orderNo, traceCode, operation, opReference, huDetails, effectiveDate);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void DoJudgeInspect(InspectMaster inspectMaster, List<string> HuIdList, DateTime? effDate, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.DoJudgeInspect(inspectMaster, HuIdList, effDate);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void InventoryFreeze(List<string> huIds, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.inventoryMgr.InventoryFreeze(huIds);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void InventoryUnFreeze(List<string> huIds, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.inventoryMgr.InventoryUnFreeze(huIds);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }


        [WebMethod]
        public void DoReturnOrder(string flowCode, List<string> huIdList, DateTime? effectiveDate, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.DoReturnOrder(flowCode, huIdList, effectiveDate);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public Hu ResolveHu(string extHuId, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                return this.inventoryMgr.ResolveHu(extHuId);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public Boolean IsValidLotNo(string lotNo)
        {
            return Utility.LotNoHelper.IsValidateLotNo(lotNo);
        }

        private string GetBusinessExMessage(BusinessException ex)
        {
            string messageString = "";
            IList<Message> messages = ex.GetMessages();
            foreach (Message message in messages)
            {
                messageString += message.GetMessageString();
            }
            return messageString;
        }

        [WebMethod]
        public Entity.SI.SD_INV.Hu CancelReceiveProdOrder(string huId, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                return this.orderMgr.CancelReceiveProdOrder(huId);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public Entity.SI.SD_INV.Hu DoReceiveProdOrder(string huId, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode, true));
                return this.orderMgr.DoReceiveProdOrder(huId);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public void DoFiReceipt(List<string> huIdList, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode, true));
                this.orderMgr.DoReceiveProdOrder(huIdList);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public Entity.SI.SD_INV.Hu StartAging(string huId, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                return this.orderMgr.StartAging(huId);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public Entity.SI.SD_INV.Hu DoAging(string huId, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                return this.orderMgr.DoAging(huId);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public Entity.SI.SD_INV.Hu DoFilter(string huId, decimal outQty, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                return this.orderMgr.DoFilter(huId, outQty);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }
        [WebMethod]
        public void RecSmallChkSparePart(string huId, string spareItem, string userCode)
        {
            try
            {
                SecurityContextHolder.Set(sdSecurityMgr.GetBaseUser(userCode));
                this.orderMgr.RecSmallChkSparePart(huId, spareItem, userCode);
            }
            catch (BusinessException ex)
            {
                throw new SoapException(GetBusinessExMessage(ex), SoapException.ServerFaultCode, string.Empty);
            }
        }

        [WebMethod]
        public List<PickTask> GetPickTasks(string userCode, bool isPickByHus)
        {
            var user = sdSecurityMgr.GetBaseUser(userCode);
            SecurityContextHolder.Set(user);
            return this.wmsMgr.GetPickTaskByUser(user.Id, isPickByHus);
        }

        [WebMethod]
        public Entity.SI.SD_INV.Hu GetPickHu(string huId, string userCode)
        {
            var user = sdSecurityMgr.GetBaseUser(userCode);
            SecurityContextHolder.Set(user);
            return this.wmsMgr.GetPickHu(huId);
        }

        [WebMethod]
        public void DoPickTask(List<Hu> huList,string userCode)
        {
            var user = sdSecurityMgr.GetBaseUser(userCode);
            SecurityContextHolder.Set(user);
            this.wmsMgr.DoPickTask(huList);
        }

        [WebMethod]
        public Entity.SI.SD_INV.Hu GetDeliverMatchHu(string huId, string userCode)
        {
            var user = sdSecurityMgr.GetBaseUser(userCode);
            SecurityContextHolder.Set(user);
            return this.wmsMgr.GetDeliverMatchHu(huId);
        }

        [WebMethod]
        public Entity.SI.SD_WMS.DeliverBarCode GetDeliverBarCode(string barCode, string userCode)
        {
            var user = sdSecurityMgr.GetBaseUser(userCode);
            SecurityContextHolder.Set(user);
            return this.wmsMgr.GetDeliverBarCode(barCode);
        }

        [WebMethod]
        public void MatchDCToHU(string huId, string barCode, string userCode)
        {
            var user = sdSecurityMgr.GetBaseUser(userCode);
            SecurityContextHolder.Set(user);
            this.wmsMgr.MatchDCToHU(huId,barCode);
        }

        [WebMethod]
        public void TransferToDock(List<string> huIds, string dock, string userCode)
        {
            var user = sdSecurityMgr.GetBaseUser(userCode);
            SecurityContextHolder.Set(user);
            this.wmsMgr.TransferToDock(huIds, dock);
        }

        [WebMethod]
        public TransportOrderMaster GetTransOrder(string orderNo)
        {
            return this.tmsMgr.GetTransOrder(orderNo);
        }

        [WebMethod]
        public Hu GetShipHu(string huId, string deliverBarCode)
        {
            return this.wmsMgr.GetShipHu(huId, deliverBarCode);
        }

        [WebMethod]
        public void DoShipWMS(string transOrder, List<string> huIds, string userCode)
        {
            var user = sdSecurityMgr.GetBaseUser(userCode);
            SecurityContextHolder.Set(user);
            this.tmsMgr.Ship(transOrder, huIds);
        }

        [WebMethod]
        public com.Sconit.Entity.SI.SD_INV.ContainerDetail GetContainerDetail(string containerId)
        {
            return this.inventoryMgr.GetContainerDetail(containerId);
        }

        [WebMethod]
        public List<Entity.SI.SD_INV.Hu> GetContainerHu(string containerId)
        {
            return this.inventoryMgr.GetContainerHu(containerId);
        }


        [WebMethod]
        public bool ContainerBind(string containerId, string huId, string userCode)
        { 
            var user = sdSecurityMgr.GetBaseUser(userCode);
            SecurityContextHolder.Set(user);
            return this.inventoryMgr.ContainerBind(containerId,huId);
        }

        [WebMethod]
        public bool ContainerUnBind(string containerId, string huId, string userCode)
        { 
            var user = sdSecurityMgr.GetBaseUser(userCode);
            SecurityContextHolder.Set(user);
            return this.inventoryMgr.ContainerUnBind(containerId,huId);
        }

        [WebMethod]
        public bool OnBin(string binCode, List<string> huIds, string userCode)
        { 
            var user = sdSecurityMgr.GetBaseUser(userCode);
            SecurityContextHolder.Set(user);
            return this.inventoryMgr.OnBin(binCode, huIds);
        }

        [WebMethod]
        public bool OffBin(List<string> huIds, string userCode)
        {
            var user = sdSecurityMgr.GetBaseUser(userCode);
            SecurityContextHolder.Set(user);
            return this.inventoryMgr.OffBin(huIds);
        }

        [WebMethod]
        public bool IsHuInContainer(string huId)
        {
            return this.inventoryMgr.IsHuInContainer(huId);
        }

        [WebMethod]
        public bool IsHuInPallet(string huId)
        {
            return this.inventoryMgr.IsHuInPallet(huId);
        }

        [WebMethod]
        public bool PalletBind(string palletCode, string huId, string userCode)
        {
            var user = sdSecurityMgr.GetBaseUser(userCode);
            SecurityContextHolder.Set(user);
            return this.inventoryMgr.PalletBind(palletCode, huId);
        }

        [WebMethod]
        public bool PalletUnBind(string containerId, string huId, string userCode)
        {
            var user = sdSecurityMgr.GetBaseUser(userCode);
            SecurityContextHolder.Set(user);
            return this.inventoryMgr.PalletUnBind(containerId, huId);
        }
    }
}
