using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using com.Sconit.Entity;
using com.Sconit.Entity.ACC;
using com.Sconit.Entity.MRP.TRANS;
using com.Sconit.Entity.SI.BAT;
using com.Sconit.Service.MRP;
using com.Sconit.Service.SI.MES;

namespace com.Sconit.Service.SI.Impl
{
    public partial class JobRunMgrImpl
    {
        public ILeanEngineMgr leanEngineMgr { get; set; }
        public ISecurityMgr securityMgr { get; set; }
        public IEDI_ScheduleMgr scheduleMgr { get; set; }
        public IMrpMgr mrpMgr { get; set; }
        public IRccpMgr rccpMgr { get; set; }
        public IOrderMgr orderMgr { get; set; }

        public IMESServicesMgr mesServicesMgr { get; set; }
        
        private void RunJob(string serviceType, JobDataMap dataMap)
        {
            User user = null;
            DateTime curDate=genericMgr.FindAllWithNativeSql<DateTime>("Select GetDate()").FirstOrDefault();
            if (dataMap.ContainKey("UserCode"))
            {
                string userCode = dataMap.GetStringValue("UserCode");
                user = securityMgr.GetUser(userCode);
                SecurityContextHolder.Set(user);
            }
            switch (serviceType)
            {
                case "MaterialIO":
                    mesServicesMgr.GenBusinessOrderData(DateTime.Now);
                    mesServicesMgr.TransBusinessOrderData();
                    break;
                //case "LeanEngineJob":
                //    leanEngineMgr.RunLeanEngine();
                //    break;
                //case "EdiImportJob":
                //    scheduleMgr.LoadEDI();
                //    break;
                //case "Edi2PlanJob":
                //    scheduleMgr.EDI2Plan();
                //    break;
                //case "MrpSnapShotJob":
                //    mrpMgr.GenMrpSnapShot(DateTime.Now, user, true, CodeMaster.SnapType.Mrp);
                //    break;
                //case "RccSnapShotJob":
                //    mrpMgr.GenMrpSnapShot(DateTime.Now, user, true, CodeMaster.SnapType.Rccp);
                //    break;
                //case "RunMrpPurchasePlanJob":
                //    mrpMgr.RunMrpPurchasePlan(user);
                //    break;
                //case "RccpJob":
                //    mrpMgr.GenMrpSnapShot(DateTime.Now, user, true, CodeMaster.SnapType.Rccp);
                //    rccpMgr.RunRccp((CodeMaster.TimeUnit)(int.Parse(dataMap.GetStringValue("DateType"))));
                //    break;
                //case "CleanOrderJob":
                //    if (dataMap.ContainKey("FlowCode"))
                //    {
                //        string flowCode = dataMap.GetStringValue("FlowCode");
                //        if (flowCode != null && flowCode != string.Empty)
                //        {
                //            var flowCodeList = flowCode.Split(',').ToList();
                //            orderMgr.CleanOrder(flowCodeList);
                //        }
                //    }
                //    break;
                //case "SIOfAllDataJob":
                //    masterDataMgr.GetSAPItem(null,null, false);
                //    masterDataMgr.GetSAPBom(null, null, false);
                //    masterDataMgr.GetSAPUomConv(null, null, false);
                //    masterDataMgr.GetSAPSupplier(null, null, false);
                //    masterDataMgr.GetSAPCustomer(null, null, false);
                //    masterDataMgr.GetSAPPriceList(null, null, null, false);
                //    break;
                //case "SIOfBomAndUomConvDataJob":
                //    masterDataMgr.GetSAPBom(null, null, true);
                //    masterDataMgr.GetSAPUomConv(null, null, true);
                //    break;
                //case "SIOfBusinessDataJob":
                //    curDate = sAPInterfaceCommonMgr.GenMesQtyData();
                //    sAPInterfaceCommonMgr.GenBusinessOrderData(curDate);
                //    sAPInterfaceCommonMgr.TransBusinessOrderData();
                //    break;
                //case "SIOfBusinessDataJobAdjust":
                //    //curDate = sAPInterfaceCommonMgr.GenMesQtyData();
                //    sAPInterfaceCommonMgr.GenBusinessAdjustOrderData(curDate);
                //    sAPInterfaceCommonMgr.TransBusinessOrderData();
                //    break;
                //case "SIOfBusinessDataJobAdjustTail":
                //    //curDate = sAPInterfaceCommonMgr.GenMesQtyData();
                //    sAPInterfaceCommonMgr.GenBusinessAdjustTailOrderData(curDate);
                //    sAPInterfaceCommonMgr.TransBusinessOrderData();
                //    break;
                //case "SIOfBusinessDataJobMonthEnd":
                //    curDate = sAPInterfaceCommonMgr.GenMesQtyData();
                //    curDate = DateTime.Parse(curDate.ToString("yyyy-MM-dd"));
                //    sAPInterfaceCommonMgr.GenBusinessOrderData(curDate);
                //    sAPInterfaceCommonMgr.TransBusinessOrderData();
                //    break;
                default:
                    break;
            }
        }
    }
}
