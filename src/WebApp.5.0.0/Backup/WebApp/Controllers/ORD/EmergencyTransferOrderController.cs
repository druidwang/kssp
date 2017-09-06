
namespace com.Sconit.Web.Controllers.ORD
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using com.Sconit.Service;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using System;
    using AutoMapper;
    using com.Sconit.Entity.MD;
    using NHibernate.Criterion;
    using com.Sconit.Entity.Exception;
    using Telerik.Web.Mvc.UI;
    using System.Collections;
    using System.Web;


    public class EmergencyTransferOrderController : WebAppBaseController
    {

        //public IGenericMgr genericMgr { get; set; }
        public IIpMgr ipMgr { get; set; }
        //public IOrderMgr orderMgr { get; set; }
        public IStockTakeMgr stockTakeMgr { get; set; }
        public EmergencyTransferOrderController()
        {
        }

        [SconitAuthorize(Permissions = "Url_EmergencyTransferOrder_View")]
        public ActionResult Index()
        {
           
            return View();
        }
        [SconitAuthorize(Permissions = "Url_EmergencyTransferOrder_View")]
        public ActionResult ImportEmergencyFeederDetail(IEnumerable<HttpPostedFileBase> attachments)
        {
            try
            {

                foreach (var file in attachments)
                {
                    string[] str = orderMgr.CreateEmTransferOrderFromXls(file.InputStream);
                   object obj=null;
                   if (str[0] ==""&&str[1]=="")
                       throw new BusinessException(Resources.EXT.ControllerLan.Con_LackProductionPurchaseOrderImportDataError);
                   else
                       obj = Resources.EXT.ControllerLan.Con_PurchaseOrderNumber + str[0] + Resources.EXT.ControllerLan.Con_GeneratedSuccessfullyItemCodeIs + str[1] + Resources.EXT.ControllerLan.Con_NotImport;
                    return Json(new { status = obj }, "text/plain");
                }
            }
            catch (BusinessException ex)
            {
                Response.Write(ex.GetMessages()[0].GetMessageString());
            }
            return Content("");
         
        }


      
    }
}
