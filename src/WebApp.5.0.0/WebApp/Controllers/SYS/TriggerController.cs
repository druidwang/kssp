using System.Data;
using System.Web.Mvc;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using System.Data.SqlClient;
using System;
using System.Linq;
using com.Sconit.Web.Models;
using com.Sconit.Entity.SI.BAT;
using System.Collections.Generic;
using com.Sconit.Service;
using com.Sconit.Entity.SYS;
using com.Sconit.Service.SI.MES;

namespace com.Sconit.Web.Controllers.SYS
{
    public class TriggerController : WebAppBaseController
    {

        public IMESServicesMgr mesServiceMgr { get; set; }

        //public IGenericMgr genericMgr { get; set; }
        private static string selectCountStatement = "select count(*) from Trigger as t";

        /// <summary>
        /// 
        /// </summary>
        private static string selectStatement = "select t from Trigger as t";


        public TriggerController()
        {

        }

        [SconitAuthorize(Permissions = "Url_BAT_Trigger_View")]
        public ActionResult Index()
        {
            ViewBag.PageSize = 20;
            IList<Trigger> Triggerlist = genericMgr.FindAll<Trigger>(selectStatement);
            foreach (Trigger trigger in Triggerlist)
            {
                if (0 == trigger.Status)
                    trigger.StatusName = Resources.EXT.ControllerLan.Con_StartUp;
                else
                    trigger.StatusName = Resources.EXT.ControllerLan.Con_Pause;
            }
            IList<CodeDetail> codeDetail = systemMgr.GetCodeDetails(Sconit.CodeMaster.CodeMaster.TimeUnit);
            foreach (CodeDetail codedet in codeDetail)
            {
                codedet.Description = systemMgr.TranslateCodeDetailDescription(codedet.Description);
            }
            ViewData["CodeDetail"] = codeDetail;

            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult _Index(GridCommand command)
        {
            ViewBag.PageSize = 20;
            IList<Trigger> Triggerlist = genericMgr.FindAll<Trigger>(selectStatement);
            foreach (Trigger trigger in Triggerlist)
            {
                if (0 == trigger.Status)
                    trigger.StatusName = Resources.EXT.ControllerLan.Con_StartUp;
                else
                    trigger.StatusName = Resources.EXT.ControllerLan.Con_Pause;
            }
            return PartialView(new GridModel(Triggerlist));

        }
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_BAT_Trigger_View")]
        public ActionResult Index(string id, string NextFireTime, string Interval, string IntervalType, GridCommand command)
        {
            if (string.IsNullOrWhiteSpace(NextFireTime))
            {
                NextFireTime = DateTime.Now.ToString();
            }
            ViewBag.PageSize = 20;
            string hql = string.Format(" update Trigger set NextFireTime='{0}',Interval='{1}',IntervalType='{2}' where Name='{3}'", NextFireTime
                , Interval, IntervalType, id);
            genericMgr.Update(hql);

            IList<Trigger> Triggerlist = genericMgr.FindAll<Trigger>(selectStatement);
            foreach (Trigger trigger in Triggerlist)
            {
                if (0 == trigger.Status)
                    trigger.StatusName = Resources.EXT.ControllerLan.Con_StartUp;
                else
                    trigger.StatusName = Resources.EXT.ControllerLan.Con_Pause;
            }
            IList<CodeDetail> codeDetail = systemMgr.GetCodeDetails(Sconit.CodeMaster.CodeMaster.TimeUnit);

            ViewData["CodeDetail"] = codeDetail;

            return PartialView(new GridModel(Triggerlist));
        }

        [SconitAuthorize(Permissions = "Url_BAT_Trigger_View")]
        public ActionResult TriggerUpdateStatus(string id)
        {
            ViewBag.PageSize = 20;
            string hql = string.Format(" update Trigger set Status='1' where Name='{0}'", id);
            genericMgr.Update(hql);

            return RedirectToAction("Index", "Trigger");
        }

        [SconitAuthorize(Permissions = "Url_BAT_Trigger_View")]
        public ActionResult TriggerUpdateStatusrecovery(string id)
        {
            ViewBag.PageSize = 20;
            string hql = string.Format(" update Trigger set Status='0' where Name='{0}'", id);
            genericMgr.Update(hql);

            return RedirectToAction("Index", "Trigger");
        }

        [SconitAuthorize(Permissions = "Url_BAT_Trigger_View")]
        public ActionResult TestIOTrans()
        {
            mesServiceMgr.GenBusinessOrderData(DateTime.Now);
            mesServiceMgr.TransBusinessOrderData();

            return RedirectToAction("Index", "Trigger");
        }

    }
}
