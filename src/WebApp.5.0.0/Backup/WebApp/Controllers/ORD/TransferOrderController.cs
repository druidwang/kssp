
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
    using com.Sconit.Entity.ORD;


    public class TransferOrderController : WebAppBaseController
    {

        //public IGenericMgr genericMgr { get; set; }
        public IIpMgr ipMgr { get; set; }
        //public IOrderMgr orderMgr { get; set; }
        public IStockTakeMgr stockTakeMgr { get; set; }
        public TransferOrderController()
        {
        }

        [SconitAuthorize(Permissions = "Url_TransferOrder_View")]
        public ActionResult Index()
        {
            IList<Uom> uoms = genericMgr.FindAll<Uom>();
            ViewData.Add("uoms", uoms);
            ViewBag.Status = com.Sconit.CodeMaster.StockTakeStatus.Create;
            return View();
        }
        [SconitAuthorize(Permissions = "Url_TransferOrder_View")]
        public ActionResult ImportFreeLocationDetail(IEnumerable<HttpPostedFileBase> attachments, string PartyFrom, string PartyTo, DateTime? StartTime)
        {
            try
            {
                if (string.IsNullOrEmpty(PartyFrom))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_SourceAreaCanNotBeEmpty);
                }

                if (StartTime == null)
                {
                    StartTime = DateTime.Now;
                }
                if (string.IsNullOrEmpty(PartyTo))
                {
                    PartyTo = PartyFrom;
                }
                foreach (var file in attachments)
                {
                    string str = orderMgr.CreateTransferOrderFromXls(file.InputStream, PartyFrom,PartyTo, Convert.ToDateTime(StartTime));
                    object obj = Resources.EXT.ControllerLan.Con_TransferOrderNumber + str + Resources.EXT.ControllerLan.Con_GeneratedSuccessfully;
                    return Json(new { status = obj }, "text/plain");
                }
            }
            catch (BusinessException ex)
            {
                Response.Write(ex.GetMessages()[0].GetMessageString());
            }
            return Content("");
         
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_TransferOrder_View")]
        public ActionResult _Select()
        {
            return PartialView(new GridModel(new List<OrderDetail>()));
        }

        public ActionResult _OrderDetailList()
        {
            IList<Uom> uoms = genericMgr.FindAll<Uom>();
            ViewData.Add("uoms", uoms);
            return PartialView();
        }


        [SconitAuthorize(Permissions = "Url_TransferOrder_View")]
        public ActionResult _WebInserintDetail(string itemCode)
        {
            if (!string.IsNullOrEmpty(itemCode))
            {
                Item item = genericMgr.FindById<Item>(itemCode);
              
                return this.Json(item);
            }
            return null;
        }
        [SconitAuthorize(Permissions = "Url_TransferOrder_View")]
        public JsonResult CreateOrder(string PartyFrom,string PartyTo, string StartTime, [Bind(Prefix =
                  "inserted")]IEnumerable<OrderDetail> insertedOrderDetails, [Bind(Prefix =
                  "updated")]IEnumerable<OrderDetail> updatedOrderDetails)
        {
            try
            {
                if (string.IsNullOrEmpty(PartyFrom))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_SourceAreaCanNotBeEmpty);
                }

                if (string.IsNullOrEmpty(StartTime))
                    StartTime = DateTime.Now.ToString();
                if (string.IsNullOrEmpty(PartyTo)) {
                    PartyTo = PartyFrom;
                }

                IList<OrderDetail> orderDetailList = insertedOrderDetails as List<OrderDetail>;
                string orderNo = orderMgr.CreateFreeTransferOrderMaster(PartyFrom,PartyTo, orderDetailList, Convert.ToDateTime(StartTime));
               
                return Json(new { message = orderNo });
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return Json(null);
            }
        }


      
    }
}
