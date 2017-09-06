
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
    using com.Sconit.Service.Impl;
    using com.Sconit.Entity.INV;

    public class PackingListController : WebAppBaseController
    {
        #region 拣货任务

        private static string selectCountStatement = "select count(*) from PackingList as p";

        private static string selectStatement = "select p from PackingList as p";

        public IPackingListMgr packingListMgr { get; set; }

        #region 查询
        [SconitAuthorize(Permissions = "Url_PackingList_View")]
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
        [SconitAuthorize(Permissions = "Url_PackingList_View")]
        public ActionResult List(GridCommand command, PackingListSearchModel searchModel)
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
        [SconitAuthorize(Permissions = "Url_PackingList_View")]
        public ActionResult _AjaxList(GridCommand command, PackingListSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<PackingList>(searchStatementModel, command));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_PackingList_Edit")]
        public ActionResult Edit(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                PackingList packingList = base.genericMgr.FindById<PackingList>(id);
                return View(packingList);
            }

        }
        #endregion

        #region 创建
        [SconitAuthorize(Permissions = "Url_PackingList_New")]
        public ActionResult New(string PackingListHu, string Flow)
        {

            ViewBag.packingListHu = PackingListHu;
            ViewBag.flow = Flow;
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_PackingList_Repack")]
        public ActionResult HuIdScan(string huId, string packingListHu, string flow)
        {
            try
            {
                #region 扫描条码
                if (packingListHu.Contains(huId))
                {
                    packingListHu = packingListHu.Replace(huId, "");
                    if (packingListHu.Contains(",,"))
                    {
                        packingListHu = packingListHu.Replace(",,", ",");
                    }
                }
                else
                {
                    BufferInventory buffInv = genericMgr.FindAll<BufferInventory>("from BufferInventory where Qty > 0 and IsLock = ? and IsPack = ? and HuId = ?", new object[] { true, false, huId }).SingleOrDefault();
                    {
                        if (buffInv == null)
                        {
                            throw new BusinessException(string.Format(Resources.WMS.PackingList.PackingList_HuIdNotExist, huId));
                        }
                    }
                    BufferOccupy buffOccupy = genericMgr.FindAll<BufferOccupy>("from BufferOccupy where UUId = ?", buffInv.UUID).SingleOrDefault();
                    if (buffOccupy == null)
                    {
                        throw new BusinessException(string.Format(Resources.WMS.PackingList.PackingList_HuIdNotLock, huId));
                    }

                    if (string.IsNullOrEmpty(flow))
                    {
                        flow = buffOccupy.Flow;
                    }
                    else
                    {
                        if (flow != buffOccupy.Flow)
                        {
                            throw new BusinessException(string.Format(Resources.WMS.PackingList.PackingList_FlowNotMatch, huId, buffOccupy.Flow, flow));
                        }
                    }

                    if (String.IsNullOrEmpty(packingListHu))
                    {
                        packingListHu = huId;
                    }
                    else
                    {
                        packingListHu = packingListHu + "," + huId;
                    }
                }
                #endregion



                object obj = new { PackingListHu = packingListHu, Flow = flow };
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
        [SconitAuthorize(Permissions = "Url_PackingList_Repack")]
        public ActionResult _SelectPackingListHu(string packingListHu)
        {
            IList<BufferInventory> bufferInvList = new List<BufferInventory>();
            if (!string.IsNullOrEmpty(packingListHu))
            {
                string sqlStr = string.Empty;
                IList<object> param = new List<object>();
                string[] packingListHuArray = packingListHu.Split(',');

                foreach (string h in packingListHuArray)
                {
                    if (string.IsNullOrEmpty(sqlStr))
                    {
                        sqlStr += "select b from BufferInventory as b where b.Qty >0 and b.HuId in (?";
                        param.Add(h);
                    }
                    else
                    {
                        sqlStr += ",?";
                        param.Add(h);
                    }
                }
                if (!string.IsNullOrEmpty(sqlStr))
                {
                    sqlStr += ")";
                }

                bufferInvList = genericMgr.FindAll<BufferInventory>(sqlStr, param.ToList());
            }
            return PartialView(new GridModel(bufferInvList));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_PackingList_New")]
        public ActionResult CreatePackingList(string flow, string packingListHu)
        {
            try
            {
                string[] huArray = packingListHu.Split(',');
                PackingList packingList = packingListMgr.CreatePackingList(flow, huArray.ToList());
                SaveSuccessMessage(Resources.WMS.PackingList.PackingList_Created, packingList.PackingListCode);
                object obj = new { SuccessMessage = string.Format(Resources.WMS.PackingList.PackingList_Created, packingList.PackingListCode), SuccessData = packingList.PackingListCode };
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

        #region 发货
        [SconitAuthorize(Permissions = "Url_PackingList_Ship")]
        public ActionResult Ship()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_PackingList_Ship")]
        public ActionResult _PackingList(string flow)
        {

            ViewBag.flow = flow;
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_PackingList_Ship")]
        public ActionResult _SelectBatchEditing(string flow)
        {
            IList<PackingList> packingList = new List<PackingList>();
            String sqlStatement = String.Empty;

            if (!string.IsNullOrEmpty(flow))
            {
                sqlStatement = "select p from PackingList as p where p.Flow = '" + flow + "' and p.IsActive = 1 ";
            }


            packingList = genericMgr.FindAll<PackingList>(sqlStatement);
            return View(new GridModel(packingList));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_PackingList_Ship")]
        public JsonResult ShipPackingList(String checkedPackingLists)
        {
            try
            {
                if (string.IsNullOrEmpty(checkedPackingLists))
                {
                    throw new BusinessException("装箱明细不能为空");
                }
                string[] idArray = checkedPackingLists.Split(',');

                packingListMgr.Ship(idArray.ToList());
                SaveSuccessMessage(Resources.WMS.PackingList.PackingList_Shipped);
                object obj = new { SuccessMessage = string.Format(Resources.WMS.PackingList.PackingList_Shipped) };
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


        #region private method
        private SearchStatementModel PrepareSearchStatement(GridCommand command, PackingListSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("CreateUserName", searchModel.PackUser, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PackingListCode", searchModel.PackingListCode, "p", ref whereStatement, param);

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
