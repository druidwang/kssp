
namespace com.Sconit.Web.Controllers.SP
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.ORD;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using com.Sconit.Entity.SCM;
    using System;
    using AutoMapper;
    using com.Sconit.Entity.MD;
    using NHibernate.Criterion;
    using com.Sconit.Entity.Exception;
    using System.Text;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Entity;

    public class SupplierPortalController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }

        public SupplierPortalController()
        {
        }

        [SconitAuthorize(Permissions = "Url_Supplier_SettlePrint")]
        public ActionResult SettlePrint()
        {
            ViewBag.SupplierCode = GetEncryptDencryptPortalUserName();
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Supplier_SettleDetail")]
        public ActionResult SettleDetail()
        {
            ViewBag.SupplierCode = GetEncryptDencryptPortalUserName();
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Supplier_ASNCreate")]
        public ActionResult ASNCreate()
        {
            ViewBag.SupplierCode = GetEncryptDencryptPortalUserName();
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Supplier_ASNPrint")]
        public ActionResult ASNPrint()
        {
            ViewBag.SupplierCode = GetEncryptDencryptPortalUserName();
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Supplier_WPM")]
        public ActionResult WPM()
        {
            ViewBag.SupplierCode = GetEncryptDencryptPortalUserName();
            return View();
        }

        private string GetEncryptDencryptPortalUserName()
        {
            string supplierCode = string.Empty;
            try
            {
                com.Sconit.Entity.ACC.User user = SecurityContextHolder.Get();
                Supplier supplier = genericMgr.FindById<Supplier>(user.Name);
            }
            catch (Exception ex)
            {

            }
            return supplierCode;
        }
    }
}
