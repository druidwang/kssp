using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Utility;
using com.Sconit.Entity.ORD;
using com.Sconit.Service;
using Telerik.Web.Mvc;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity;

namespace com.Sconit.Web.Controllers.ORD
{
    public class OrderBindingController : WebAppBaseController
    {
        //
        // GET: /OrderBinding/

        //public IGenericMgr genericMgr { get; set; }
        //public IOrderMgr orderMgr { get; set; }
        public ActionResult Index()
        {
            return View();
        }

        #region Bind
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_View,Url_OrderMstr_Procurement_View,Url_OrderMstr_Distribution_View")]
        public ActionResult _OrderBinding(string orderNo, string controlName)
        {
            OrderMaster orderMaster = genericMgr.FindById<OrderMaster>(orderNo);
            ViewBag.orderNo = orderNo;
            ViewBag.controlName = controlName;
            ViewBag.status = orderMaster.Status;

            return PartialView();
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Edit,Url_OrderMstr_Procurement_Edit,Url_OrderMstr_Distribution_Edit")]
        public ActionResult _OrderBinded(string id)
        {
            ViewBag.orderNo = id;
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_View,Url_OrderMstr_Procurement_View,Url_OrderMstr_Distribution_View")]
        public ActionResult _AjaxOrderBinding(string orderNo)
        {
            string hql = "select o from OrderBinding as o where o.OrderNo = ?";
            IList<OrderBinding> orderBindingList = genericMgr.FindAll<OrderBinding>(hql, orderNo);
            base.FillCodeDetailDescription<OrderBinding>(orderBindingList);
            return View(new GridModel(orderBindingList));
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Edit,Url_OrderMstr_Procurement_Edit,Url_OrderMstr_Distribution_Edit")]
        public ActionResult _AjaxOrderBindingDelete(string id, string orderNo)
        {
            genericMgr.DeleteById<OrderBinding>(Convert.ToInt32(id));
            SaveSuccessMessage(Resources.ORD.OrderBinding.OrderBinding_Deleted);
            string hql = "select o from OrderBinding as o where o.OrderNo = ?";
            IList<OrderBinding> orderBindingList = genericMgr.FindAll<OrderBinding>(hql, orderNo);
            base.FillCodeDetailDescription<OrderBinding>(orderBindingList);
            return View(new GridModel(orderBindingList));
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_View,Url_OrderMstr_Procurement_View,Url_OrderMstr_Distribution_View")]
        public ActionResult _AjaxOrderBinded(string id)
        {
            string hql = "select o from OrderBinding as o where o.BindOrderNo = ?";
            IList<OrderBinding> orderBindingList = genericMgr.FindAll<OrderBinding>(hql, id);
            base.FillCodeDetailDescription<OrderBinding>(orderBindingList);
            return View(new GridModel(orderBindingList));
        }

        [HttpGet]
        [SconitAuthorize(Permissions ="Url_OrderMstr_Production_Edit,Url_OrderMstr_Procurement_Edit,Url_OrderMstr_Distribution_Edit")]
        public ActionResult _OrderBindingEdit(int id,string controlName)
        {
            OrderBinding orderBinding = this.genericMgr.FindById<OrderBinding>(id);
            orderBinding.ControlName = controlName;
            return PartialView(orderBinding);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Edit,Url_OrderMstr_Procurement_Edit,Url_OrderMstr_Distribution_Edit")]
        public ActionResult _OrderBindingEdit(OrderBinding orderBinding,string controlName)
        {
            OrderBinding newOrderBinding = genericMgr.FindById<OrderBinding>(orderBinding.Id);
            newOrderBinding.BindFlow = orderBinding.BindFlow;
            newOrderBinding.BindType = orderBinding.BindType;
            genericMgr.UpdateWithTrim(newOrderBinding);
            SaveSuccessMessage(Resources.ORD.OrderBinding.OrderBinding_Updated);
            TempData["TabIndex"] = 1;
            return RedirectToAction("Edit/" + orderBinding.OrderNo,controlName);
        }

        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Edit,Url_OrderMstr_Procurement_Edit,Url_OrderMstr_Distribution_Edit")]
        public ActionResult _OrderBindingNew(string id,string controlName)
        {
            OrderBinding orderBinding = new OrderBinding();
            orderBinding.OrderNo = id;
            orderBinding.ControlName = controlName;
            return PartialView(orderBinding);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_OrderMstr_Production_Edit,Url_OrderMstr_Procurement_Edit,Url_OrderMstr_Distribution_Edit")]
        public ActionResult _OrderBindingNew(OrderBinding orderBinding)
        {
            if (ModelState.IsValid)
            {
                this.genericMgr.CreateWithTrim(orderBinding);
                SaveSuccessMessage(Resources.ORD.OrderBinding.OrderBinding_Added);
                TempData["TabIndex"] = 1;
                return RedirectToAction("Edit", orderBinding.ControlName, new { orderNo = orderBinding.OrderNo });
            }
            return PartialView(orderBinding);
        }

        public JsonResult RebindingOrder(int? Id)
        {
            try
            {
                OrderBinding orderBinding = genericMgr.FindById<OrderBinding>(Id.Value);
                this.orderMgr.ReCreateBindOrder(orderBinding);
                return Json(new { orderNo = orderBinding.OrderNo });
            }
            catch(BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                string errorStr = string.Empty;
                foreach (Message message in ex.GetMessages())
                {
                    errorStr += message.GetMessageString();
                }
                Response.Write(errorStr);
                return null;

                //string errorStr=string.Empty;
                //foreach (Message message in ex.GetMessages())
                //{
                //   errorStr+= message.GetMessageString();
                //}
                //Response.Write(errorStr);
                //return null;
            }
        }
        #endregion

    }
}
