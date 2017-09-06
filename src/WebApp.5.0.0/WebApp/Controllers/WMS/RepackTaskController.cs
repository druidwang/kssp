
namespace com.Sconit.Web.Controllers.WMS
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using Telerik.Web.Mvc;
    using System.Web.Routing;
    using com.Sconit.Web.Models;
    using com.Sconit.Utility;
    using com.Sconit.Web.Models.SearchModels.WMS;
    using com.Sconit.Entity.WMS;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Entity.INV;


    public class RepackTaskController : WebAppBaseController
    {
        #region 翻箱任务

        private static string selectCountStatement = "select count(*) from RepackTask as p";

        private static string selectStatement = "select p from RepackTask as p";

        public IRepackTaskMgr repackTaskMgr { get; set; }
        public IHuMgr huMgr { get; set; }


        #region 查看
        [SconitAuthorize(Permissions = "Url_RepackTask_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_RepackTask_View")]
        public ActionResult List(GridCommand command, RepackTaskSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_RepackTask_View")]
        public ActionResult _AjaxList(GridCommand command, RepackTaskSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<RepackTask>(searchStatementModel, command));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SconitAuthorize(Permissions = "Url_RepackTask_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_RepackTask_Edit")]
        public ActionResult Edit(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                RepackTask repackTask = genericMgr.FindAll<RepackTask>("from RepackTask where Id = ?", id).SingleOrDefault();
                return View(repackTask);
            }

        }


        #endregion

        #region 分派
        [SconitAuthorize(Permissions = "Url_RepackTask_Assign")]
        public ActionResult Assign()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_RepackTask_Assign")]
        public ActionResult _RepackTaskList(string pickGroupCode)
        {

            ViewBag.pickGroupCode = pickGroupCode;

            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_RepackTask_Assign")]
        public ActionResult _SelectAssignBatchEditing(string pickGroupCode)
        {
            IList<RepackTask> pickTaskList = new List<RepackTask>();

            if (!string.IsNullOrEmpty(pickGroupCode))
            {
                string repackGroupSql = "select r from PickRule as r where r.PickGroupCode = ?";
                IList<PickRule> pickRuleList = genericMgr.FindAll<PickRule>(repackGroupSql, pickGroupCode);

                if (pickRuleList != null && pickRuleList.Count > 0)
                {
                    string pickRuleSql = string.Empty;
                    IList<object> param = new List<object>();

                    foreach (PickRule r in pickRuleList)
                    {
                        if (string.IsNullOrEmpty(pickRuleSql))
                        {
                            pickRuleSql += "select p from RepackTask as p where p.RepackUserId is null and p.Location in (?";
                            param.Add(r.Location);
                        }
                        else
                        {
                            pickRuleSql += ",?";
                            param.Add(r.Location);
                        }
                    }

                    if (!string.IsNullOrEmpty(pickRuleSql))
                    {
                        pickRuleSql += ")";
                    }

                    pickTaskList = genericMgr.FindAll<RepackTask>(pickRuleSql, param.ToList());
                }




            }
            return View(new GridModel(pickTaskList));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SconitAuthorize(Permissions = "Url_RepackTask_Assign")]
        public ActionResult AssignRepackTask(String checkedRepackTasks, String assignUser)
        {
            try
            {

                IList<RepackTask> repackTaskList = new List<RepackTask>();
                if (!string.IsNullOrEmpty(checkedRepackTasks))
                {

                    string[] idArray = checkedRepackTasks.Split(',');


                    for (int i = 0; i < idArray.Count(); i++)
                    {

                        RepackTask rt = genericMgr.FindById<RepackTask>(idArray[i]);

                        repackTaskList.Add(rt);

                    }
                }
                repackTaskMgr.AssignRepackTask(repackTaskList, assignUser);
                SaveSuccessMessage(Resources.WMS.RepackTask.RepackTask_Assigned);
                object obj = new { SuccessMessage = string.Format(Resources.WMS.RepackTask.RepackTask_Assigned, checkedRepackTasks), SuccessData = checkedRepackTasks };
                return Json(obj);

            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return Json(null);
            }
        }
        #endregion

        #region 翻包
        [SconitAuthorize(Permissions = "Url_RepackTask_Repack")]
        public ActionResult RepackIndex()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_RepackTask_Repack")]
        public ActionResult RepackList(GridCommand command, RepackTaskSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_RepackTask_Repack")]
        public ActionResult _RepackAjaxList(GridCommand command, RepackTaskSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareRepackSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<RepackTask>(searchStatementModel, command));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_RepackTask_Repack")]
        public ActionResult RepackEdit(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                RepackTask repackTask = genericMgr.FindAll<RepackTask>("from RepackTask where Id = ?", Convert.ToInt32(id)).SingleOrDefault();
                try
                {
                    IList<Hu> huList = repackTaskMgr.SuggestRepackHu(Convert.ToInt32(id));
                    if (huList != null)
                    {
                        string huStr = huList.Select(h => h.HuId).ToString();
                    }
                }
                catch (BusinessException ex)
                {
                    repackTask.Remark = (string)ex.GetMessages().First().GetMessageString();
                }
                return View(repackTask);
            }

        }

        [SconitAuthorize(Permissions = "Url_RepackTask_Repack")]
        public ActionResult _RepackHuList(string RepackInHu, string RepackOutHu, string HuType, string InQty)
        {

            ViewBag.repackInHu = RepackInHu;
            ViewBag.repackOutHu = RepackOutHu;
            ViewBag.huType = (HuType == null || HuType == "") ? "0" : HuType;
            ViewBag.inQty = string.IsNullOrEmpty(InQty) ? "0" : InQty;
            return PartialView();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_RepackTask_Repack")]
        public ActionResult HuIdScan(string repackTaskId, string huId, string huType, string repackInHu, string repackOutHu, string inQty)
        {
            try
            {
                RepackTask repackTask = genericMgr.FindAll<RepackTask>("from RepackTask where Id = ?", repackTaskId).SingleOrDefault();
                if (huType == "0")
                {
                    #region 翻包前条码
                    if (repackInHu.Contains(huId))
                    {
                        //throw new BusinessException(string.Format(Resources.WMS.RepackTask.RepackTask_HuIdExist, huId));
                        repackInHu = repackInHu.Replace(huId, "");
                        if (repackInHu.Contains(",,"))
                        {
                            repackInHu = repackInHu.Replace(",,", ",");
                        }
                        Hu inHu = genericMgr.FindAll<Hu>("from Hu where HuId = ?", huId).SingleOrDefault();
                        Decimal qty = String.IsNullOrEmpty(inQty) ? 0 : Convert.ToDecimal(inQty);
                        inQty = (qty - inHu.Qty).ToString("0.##");
                    }
                    else
                    {
                        Hu inHu = genericMgr.FindAll<Hu>("from Hu where HuId = ?", huId).SingleOrDefault();
                        {
                            if (inHu == null)
                            {
                                throw new BusinessException(string.Format(Resources.WMS.RepackTask.RepackTask_HuIdNotExist, huId));
                            }
                            //if (inHu.Location != repackTask.Location)
                            //{
                            //    errorMsg = "条码库位和拣货库位不一致";
                            //}
                        }


                        if (String.IsNullOrEmpty(repackInHu))
                        {
                            repackInHu = huId;
                        }
                        else
                        {
                            repackInHu = repackInHu + "," + huId;
                        }

                        Decimal qty = String.IsNullOrEmpty(inQty) ? 0 : Convert.ToDecimal(inQty);
                        inQty = (qty + inHu.Qty).ToString("0.##");
                        if (qty + inHu.Qty >= (repackTask.Qty - repackTask.RepackQty))
                        {
                            huType = "1";
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 翻包后条码
                    if (repackOutHu.Contains(huId))
                    {
                        repackOutHu = repackOutHu.Replace(huId, "");
                        if (repackOutHu.Contains(",,"))
                        {
                            repackOutHu = repackOutHu.Replace(",,", ",");
                        }
                    }
                    else
                    {
                        Hu outHu = genericMgr.FindAll<Hu>("from Hu where HuId = ?", huId).SingleOrDefault();
                        {
                            if (outHu == null)
                            {
                                throw new BusinessException(string.Format(Resources.WMS.RepackTask.RepackTask_HuIdNotExist, huId));
                            }
                            //判断条码状态
                        }

                        if (String.IsNullOrEmpty(repackOutHu))
                        {
                            repackOutHu = huId;
                        }
                        else
                        {
                            repackOutHu = repackOutHu + "," + huId;
                        }

                    }
                    #endregion
                }
                object obj = new { RepackInHu = repackInHu, RepackOutHu = repackOutHu, HuType = huType, InQty = inQty };
                return Json(obj);
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return Json(null);
            }

        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_RepackTask_Repack")]
        public ActionResult _SelectRepackInHu(string repackInHu)
        {
            IList<Hu> huList = new List<Hu>();
            if (!string.IsNullOrEmpty(repackInHu))
            {
                string inSqlStr = string.Empty;
                IList<object> param = new List<object>();
                string[] repackInHuArray = repackInHu.Split(',');

                foreach (string h in repackInHuArray)
                {
                    if (string.IsNullOrEmpty(inSqlStr))
                    {
                        inSqlStr += "select h from Hu as h where h.HuId in (?";
                        param.Add(h);
                    }
                    else
                    {
                        inSqlStr += ",?";
                        param.Add(h);
                    }
                }
                if (!string.IsNullOrEmpty(inSqlStr))
                {
                    inSqlStr += ")";
                }

                huList = genericMgr.FindAll<Hu>(inSqlStr, param.ToList());
            }
            return PartialView(new GridModel(huList));
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_RepackTask_Repack")]
        public ActionResult _SelectRepackOutHu(string repackOutHu)
        {
            IList<Hu> huList = new List<Hu>();
            if (!string.IsNullOrEmpty(repackOutHu))
            {
                string outSqlStr = string.Empty;
                IList<object> param = new List<object>();
                string[] repackOutHuArray = repackOutHu.Split(',');

                foreach (string h in repackOutHuArray)
                {
                    if (string.IsNullOrEmpty(outSqlStr))
                    {
                        outSqlStr += "select h from Hu as h where h.HuId in (?";
                        param.Add(h);
                    }
                    else
                    {
                        outSqlStr += ",?";
                        param.Add(h);
                    }
                }
                if (!string.IsNullOrEmpty(outSqlStr))
                {
                    outSqlStr += ")";
                }

                huList = genericMgr.FindAll<Hu>(outSqlStr, param.ToList());
            }
            return PartialView(new GridModel(huList));
        }

        [SconitAuthorize(Permissions = "Url_RepackTask_Assign")]
        public ActionResult RepackTaskRepack(string repackTaskId, string repackInHu, string repackOutHu)
        {
            try
            {
                IList<string> repackInResult = repackInHu.Split(',').ToList<string>();
                IList<string> repackOutResult = repackOutHu.Split(',').ToList<string>();
                repackTaskMgr.ProcessRepackResult(Convert.ToInt32(repackTaskId), repackInResult, repackOutResult, DateTime.Now);
                return Json(null);

            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return Json(null);
            }
        }
        #endregion


        #region private method
        private SearchStatementModel PrepareSearchStatement(GridCommand command, RepackTaskSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("RepackUserId", searchModel.RepackUser, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Location", searchModel.Location, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsActive", searchModel.IsActive, "p", ref whereStatement, param);

            if (searchModel.DateFrom != null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.DateFrom, "p", ref whereStatement, param);
            }
            if (searchModel.DateTo != null)
            {
                HqlStatementHelper.AddLtStatement("CreateDate", searchModel.DateTo.Value.AddDays(1), "p", ref whereStatement, param);
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }

        private SearchStatementModel PrepareAssignSearchStatement(GridCommand command, RepackTaskSearchModel searchModel)
        {
            string whereStatement = " where p.RepackUserId is null ";
            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("Location", searchModel.Location, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsActive", searchModel.IsActive, "p", ref whereStatement, param);

            if (searchModel.DateFrom != null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.DateFrom, "p", ref whereStatement, param);
            }
            if (searchModel.DateTo != null)
            {
                HqlStatementHelper.AddLtStatement("CreateDate", searchModel.DateTo.Value.AddDays(1), "p", ref whereStatement, param);
            }


            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }

        private SearchStatementModel PrepareRepackSearchStatement(GridCommand command, RepackTaskSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            HqlStatementHelper.AddEqStatement("Location", searchModel.Location, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "p", ref whereStatement, param);

            //默认只能选自己的有效的
            HqlStatementHelper.AddEqStatement("RepackUserId", searchModel.RepackUser, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsActive", true, "p", ref whereStatement, param);

            if (searchModel.DateFrom != null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.DateFrom, "p", ref whereStatement, param);
            }
            if (searchModel.DateTo != null)
            {
                HqlStatementHelper.AddLtStatement("CreateDate", searchModel.DateTo.Value.AddDays(1), "p", ref whereStatement, param);
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }
        #endregion

        #endregion


    }
}
