using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Utility;
using com.Sconit.Service.MRP;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.MRP;
using com.Sconit.Web.Models;
using com.Sconit.Entity.MRP.TRANS;
using com.Sconit.Entity.Exception;
using com.Sconit.Service;
using com.Sconit.Entity;
using com.Sconit.Entity.MRP.MD;
using System.IO;
using System.Text.RegularExpressions;
using com.Sconit.Entity.MRP.ORD;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.PRD;
using com.Sconit.Entity.BIL;
using com.Sconit.Entity.SCM;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace com.Sconit.Web.Controllers.MRP
{
    public class MrpSnapController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }
        public IMrpMgr mrpMgr { get; set; }
        //public IItemMgr itemMgr { get; set; }

        private static string selectCountStatement = "select count(*) from MrpSnapMaster as m";

        private static string selectStatement = "select m from MrpSnapMaster as m";

        private static string selectCountMrpFlowDetailStatement = "select count(*) from MrpFlowDetail as m";

        private static string selectMrpFlowDetailStatement = "select m from MrpFlowDetail as m";

        private static string selectCountInventoryBalanceStatement = "select count(*) from InventoryBalance as i";

        private static string selectInventoryBalanceStatement = "select i from InventoryBalance as i";

        private static string selectCountTransitOrderStatement = "select count(*) from TransitOrder as t";

        private static string selectTransitOrderStatement = "select t from TransitOrder as t";

        private static string selectCountActiveOrderStatement = "select count(*) from ActiveOrder as i";

        private static string selectActiveOrderStatement = "select i from ActiveOrder as i";

        #region MrpSnap
        [SconitAuthorize(Permissions = "Url_MrpSnap_Index")]
        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_MrpSnap_Index")]
        public ActionResult List(GridCommand command, MrpSnapMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_MrpSnap_Index")]
        public ActionResult _AjaxList(GridCommand command, MrpSnapMasterSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<MrpSnapMaster>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_MrpSnap_Index")]
        public ActionResult Edit(string snapTime)
        {
            DateTime dt = Convert.ToDateTime(snapTime);
            MrpSnapMaster mrpSnapMaster = this.genericMgr.FindAll<MrpSnapMaster>("select m from MrpSnapMaster as m where m.SnapTime=? ", new object[] { dt })[0];
            return View(mrpSnapMaster);

        }

        private SearchStatementModel PrepareSearchStatement(GridCommand command, MrpSnapMasterSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddEqStatement("IsRelease", searchModel.IsRelease, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Type", searchModel.SnapType, "m", ref whereStatement, param);
            if (searchModel.SnapTimeStart != null & searchModel.SnapTimeEnd != null)
            {
                HqlStatementHelper.AddBetweenStatement("SnapTime", searchModel.SnapTimeStart, searchModel.SnapTimeEnd, "m", ref whereStatement, param);
            }
            else if (searchModel.SnapTimeStart != null & searchModel.SnapTimeEnd == null)
            {
                HqlStatementHelper.AddGeStatement("SnapTime", searchModel.SnapTimeStart, "m", ref whereStatement, param);
            }
            else if (searchModel.SnapTimeStart == null & searchModel.SnapTimeEnd != null)
            {
                HqlStatementHelper.AddLeStatement("SnapTime", searchModel.SnapTimeEnd, "m", ref whereStatement, param);
            }

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by SnapTime desc";
            }

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_MrpSnap_Index")]
        public JsonResult _RunMrpSnap(CodeMaster.SnapType snapType)
        {
            try
            {
                DateTime snapTime = DateTime.Now;
                AsyncRun(snapTime, this.CurrentUser, snapType);
                object obj = new { };
                SaveSuccessMessage(string.Format(Resources.EXT.ControllerLan.Con_PrepareingDataForPlan, snapTime));
                return Json(obj);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        private delegate void Async(DateTime snapTime, Sconit.Entity.ACC.User user, bool isRelease, CodeMaster.SnapType snapType);
        private void AsyncRun(DateTime snapTime, Sconit.Entity.ACC.User user, CodeMaster.SnapType snapType)
        {
            Async async = new Async(mrpMgr.GenMrpSnapShot);
            async.BeginInvoke(snapTime, user, false, snapType, null, null);
        }

        [SconitAuthorize(Permissions = "Url_MrpSnap_Index")]
        public ActionResult Save(string snapTimeShow, bool isRelease)
        {
            DateTime dt = Convert.ToDateTime(snapTimeShow);
            MrpSnapMaster mrpSnapMaster = this.genericMgr.FindAll<MrpSnapMaster>("select m from MrpSnapMaster as m where m.SnapTime=? ", new object[] { dt })[0];

            mrpSnapMaster.IsRelease = isRelease;

            mrpSnapMaster.LastModifyUserId = this.CurrentUser.Id;
            mrpSnapMaster.LastModifyUserName = this.CurrentUser.FullName;
            mrpSnapMaster.LastModifyDate = DateTime.Now;

            genericMgr.Update(mrpSnapMaster);
            SaveSuccessMessage(Resources.EXT.ControllerLan.Con_SavedSuccessfully);
            return RedirectToAction("Edit", new { snapTime = snapTimeShow });
        }

        #endregion

        #region  MrpFlowDetail
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MrpFlowDetail")]
        public ActionResult MrpFlowDetail()
        {
            return View();
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MrpFlowDetail")]
        public ActionResult MrpFlowDetailList(GridCommand command, MrpFlowDetailSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            if (searchModel.SnapTime == DateTime.MinValue)
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_SnapTimeCanNotBeEmpty);
            }
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MrpFlowDetail")]
        public ActionResult _AjaxMrpFlowDetailList(GridCommand command, MrpFlowDetailSearchModel searchModel)
        {
            if (searchModel.SnapTime == DateTime.MinValue)
            {
                return PartialView(new GridModel(new List<MrpFlowDetail>()));
            }
            SearchStatementModel searchStatementModel = this.PrepareMrpFlowDetailSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<MrpFlowDetail>(searchStatementModel, command));
        }


        private SearchStatementModel PrepareMrpFlowDetailSearchStatement(GridCommand command, MrpFlowDetailSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("LocationFrom", searchModel.LocationFrom, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LocationTo", searchModel.LocationTo, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyFrom", searchModel.PartyFrom, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyTo", searchModel.PartyTo, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("SnapTime", searchModel.SnapTime, "m", ref whereStatement, param);


            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);


            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountMrpFlowDetailStatement;
            searchStatementModel.SelectStatement = selectMrpFlowDetailStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
        #endregion

        #region  InventoryBalance
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_InventoryBalance")]
        public ActionResult InventoryBalance()
        {
            return View();
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_InventoryBalance")]
        public ActionResult InventoryBalanceList(GridCommand command, InventoryBalanceSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.Location = searchModel.Location;
            ViewBag.Item = searchModel.Item;
            ViewBag.SnapTime = searchModel.SnapTime;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            if (searchModel.SnapTime == DateTime.MinValue)
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_SnapTimeCanNotBeEmpty);
            }
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_InventoryBalance")]
        public ActionResult _AjaxInventoryBalanceList(GridCommand command, InventoryBalanceSearchModel searchModel)
        {
            if (searchModel.SnapTime == DateTime.MinValue)
            {
                return PartialView(new GridModel(new List<InventoryBalance>()));
            }
            SearchStatementModel searchStatementModel = this.PrepareInventoryBalanceSearchStatement(command, searchModel);
            var dataList = GetAjaxPageData<InventoryBalance>(searchStatementModel, command);
            foreach (var data in dataList.Data)
            {
                data.ItemDescription = itemMgr.GetCacheItem(data.Item).FullDescription;
            }
            return PartialView(dataList);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_InventoryBalance")]
        public ActionResult _Update(GridCommand command, InventoryBalance inventoryBalance, string locationTo, string itemTo, DateTime? snapTimeTo
            )
        {
            InventoryBalanceSearchModel searchModel = new InventoryBalanceSearchModel();

            searchModel.Location = locationTo;
            searchModel.Item = itemTo;
            searchModel.SnapTime = snapTimeTo.Value;
            InventoryBalance newInventoryBalance = genericMgr.FindAll<InventoryBalance>(" from InventoryBalance as i where  i.Id=? ", new object[] { inventoryBalance.Id })[0];
            if (inventoryBalance.Qty != newInventoryBalance.Qty)
            {
                newInventoryBalance.Qty = inventoryBalance.Qty;
                genericMgr.Update(newInventoryBalance);
            }

            SearchStatementModel searchStatementModel = PrepareInventoryBalanceSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<InventoryBalance>(searchStatementModel, command));
        }


        private SearchStatementModel PrepareInventoryBalanceSearchStatement(GridCommand command, InventoryBalanceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("Location", searchModel.Location, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("SnapTime", searchModel.SnapTime, "i", ref whereStatement, param);



            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);


            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountInventoryBalanceStatement;
            searchStatementModel.SelectStatement = selectInventoryBalanceStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
        #endregion

        #region  TransitOrder
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_TransitOrder")]
        public ActionResult TransitOrder()
        {
            return View();
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_TransitOrder")]
        public ActionResult TransitOrderList(GridCommand command, TransitOrderSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            if (searchModel.SnapTime == DateTime.MinValue)
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_SnapTimeCanNotBeEmpty);
            }
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_TransitOrder")]
        public ActionResult _AjaxTransitOrderList(GridCommand command, TransitOrderSearchModel searchModel)
        {

            if (searchModel.SnapTime == DateTime.MinValue)
            {
                return PartialView(new GridModel(new List<TransitOrder>()));
            }

            SearchStatementModel searchStatementModel = this.PrepareTransitOrderSearchStatement(command, searchModel);
            var dataList = GetAjaxPageData<TransitOrder>(searchStatementModel, command);
            foreach (var data in dataList.Data)
            {
                data.ItemDescription = itemMgr.GetCacheItem(data.Item).FullDescription;
            }
            return PartialView(dataList);
        }


        private SearchStatementModel PrepareTransitOrderSearchStatement(GridCommand command, TransitOrderSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("Location", searchModel.Location, "t", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("OrderNo", searchModel.OrderNo, "t", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IpNo", searchModel.IpNo, "t", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "t", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "t", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("SnapTime", searchModel.SnapTime, "t", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);


            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountTransitOrderStatement;
            searchStatementModel.SelectStatement = selectTransitOrderStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
        #endregion

        #region  ActiveOrder
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_ActiveOrder")]
        public ActionResult ActiveOrder()
        {
            return View();
        }
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_ActiveOrder")]
        public ActionResult ActiveOrderList(GridCommand command, ActiveOrderSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            if (searchModel.SnapTime == DateTime.MinValue)
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_SnapTimeCanNotBeEmpty);
            }

            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_ActiveOrder")]
        public ActionResult _AjaxActiveOrderList(GridCommand command, ActiveOrderSearchModel searchModel)
        {
            if (searchModel.SnapTime == DateTime.MinValue)
            {
                return PartialView(new GridModel(new List<ActiveOrder>()));
            }
            SearchStatementModel searchStatementModel = this.PrepareActiveOrderSearchStatement(command, searchModel);
            var dataList = GetAjaxPageData<ActiveOrder>(searchStatementModel, command);
            foreach (var data in dataList.Data)
            {
                data.ItemDescription = itemMgr.GetCacheItem(data.Item).FullDescription;
            }
            return PartialView(dataList);
        }


        private SearchStatementModel PrepareActiveOrderSearchStatement(GridCommand command, ActiveOrderSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();
            HqlStatementHelper.AddEqStatement("Location", searchModel.Location, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("OrderNo", searchModel.OrderNo, "i", ref whereStatement, param);

            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "i", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("SnapTime", searchModel.SnapTime, "i", ref whereStatement, param);
            if (searchModel.IsIndepentDemand)
            {
                HqlStatementHelper.AddEqStatement("IsIndepentDemand", searchModel.IsIndepentDemand, "i", ref whereStatement, param);
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountActiveOrderStatement;
            searchStatementModel.SelectStatement = selectActiveOrderStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
        #endregion

        [SconitAuthorize(Permissions = "Url_MrpSnap_Index")]
        public ActionResult _MrpSnapError(string snapTimeShow)
        {
            IList<MrpSnapLog> mrpSnapLogList = genericMgr.FindAll<MrpSnapLog>(" from MrpSnapLog as m where m.SnapTime=?", Convert.ToDateTime(snapTimeShow));
            return PartialView(mrpSnapLogList);
        }

        public string _GetRunLog()
        {
            try
            {
                FileStream fs = new FileStream(@"D:\logs\Sconit5_Shenya\GenMrpSnapShot.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
                string log = sr.ReadToEnd();
                string[] lines = Regex.Split(log, "\r\n", RegexOptions.IgnoreCase);
                string testLog = string.Empty;
                if (lines.Length > 0)
                {
                    var startIndex = lines.Select((n, i) => new { n = n, i = i + 1 }).Where(n => n.n.Contains("---**---")).Last().i;
                    foreach (var line in lines.Skip(startIndex - 1).Reverse())
                    {
                        testLog += line + "</br>";
                    }
                }
                return testLog;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        #region MasterDataCheck
        [SconitAuthorize(Permissions = "Url_MrpSnap_MasterDataCheck")]
        public ActionResult MasterDataCheck()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_MrpSnap_MasterDataCheck")]
        public string _CheckUom()
        {
            IList<Item> items = this.genericMgr.FindAll<Item>();
            IList<BomDetail> bomDetails = this.genericMgr.FindAll<BomDetail>()
                .Where(p => (!p.EndDate.HasValue || p.EndDate.Value > DateTime.Now) && p.StartDate <= DateTime.Now).ToList();
            IList<BomMaster> bomMasters = this.genericMgr.FindAll<BomMaster>();
            IList<UomConversion> uomConversions = this.genericMgr.FindAll<UomConversion>();
            IList<PriceListDetail> priceListDetails = this.genericMgr.FindAll<PriceListDetail>();
            IList<FlowDetail> flowDetails = this.genericMgr.FindAll<FlowDetail>();

            var uomCheckViews = new List<UomCheckView>();
            //BomDetail
            var q_bd = bomDetails.Join(items, d => d.Item, i => i.Code, (d, i) => new { d, i }).Where(q1 => q1.d.Uom != q1.i.Uom &&
                     (uomConversions.Where(u => u.AlterUom == q1.d.Uom && u.BaseUom == q1.i.Uom && (u.Item == null || u.Item.Code == q1.i.Code)).Count() == 0
                   && uomConversions.Where(u => u.BaseUom == q1.d.Uom && u.AlterUom == q1.i.Uom && (u.Item == null || u.Item.Code == q1.i.Code)).Count() == 0))
                   .Select(q2 => new UomCheckView { Code = q2.d.Bom, Description = q2.d.Bom, Type = "BomDetail", Item = q2.i.Code, AlterUom = q2.d.Uom })
                   .ToList();
            uomCheckViews.AddRange(q_bd);

            //FlowDetail
            var q_fd = flowDetails.Join(items, d => d.Item, i => i.Code, (d, i) => new { d, i }).Where(q1 => q1.d.Uom != q1.i.Uom &&
                     (uomConversions.Where(u => u.AlterUom == q1.d.Uom && u.BaseUom == q1.i.Uom && (u.Item == null || u.Item.Code == q1.i.Code)).Count() == 0
                   && uomConversions.Where(u => u.BaseUom == q1.d.Uom && u.AlterUom == q1.i.Uom && (u.Item == null || u.Item.Code == q1.i.Code)).Count() == 0))
                   .Select(q2 => new UomCheckView { Code = q2.d.Flow, Description = q2.d.Flow, SubType = q2.d.Flow, Type = "FlowDetail", Item = q2.i.Code, AlterUom = q2.d.Uom })
                   .ToList();
            uomCheckViews.AddRange(q_fd);

            //PriceListDetail
            var q_pd = priceListDetails.Join(items, d => d.Item, i => i.Code, (d, i) => new { d, i }).Where(q1 => q1.d.Uom != q1.i.Uom &&
                     (uomConversions.Where(u => u.AlterUom == q1.d.Uom && u.BaseUom == q1.i.Uom && (u.Item == null || u.Item.Code == q1.i.Code)).Count() == 0
                   && uomConversions.Where(u => u.BaseUom == q1.d.Uom && u.AlterUom == q1.i.Uom && (u.Item == null || u.Item.Code == q1.i.Code)).Count() == 0))
                   .Select(q2 => new UomCheckView { Code = q2.d.PriceList.Code, Description = q2.d.PriceList.Party, SubType = q2.d.PriceList.Party, Type = "PriceListDetail", Item = q2.i.Code, AlterUom = q2.d.Uom })
                   .ToList();
            uomCheckViews.AddRange(q_pd);

            //BomMstr 取值item  
            var q_bm1 = bomMasters.Join(items, d => d.Code, i => i.Code, (d, i) => new { d, i }).Where(q1 => q1.d.Uom != q1.i.Uom &&
                     (uomConversions.Where(u => u.AlterUom == q1.d.Uom && u.BaseUom == q1.i.Uom && (u.Item == null || u.Item.Code == q1.i.Code)).Count() == 0
                   && uomConversions.Where(u => u.BaseUom == q1.d.Uom && u.AlterUom == q1.i.Uom && (u.Item == null || u.Item.Code == q1.i.Code)).Count() == 0))
                   .Select(q2 => new UomCheckView { Code = q2.d.Code, Description = q2.d.Description, Type = "BomMaster", Item = q2.i.Code, AlterUom = q2.d.Uom })
                   .ToList();
            uomCheckViews.AddRange(q_bm1);

            //BomMstr 取值 flowdet 
            var q_bm2 = bomMasters.Join(flowDetails.Where(f => f.Bom != null), d => d.Code, i => i.Bom, (d, i) => new { d, i }).Where(q1 => q1.d.Uom != q1.i.Uom &&
                     (uomConversions.Where(u => u.AlterUom == q1.d.Uom && u.BaseUom == q1.i.Uom && (u.Item == null || u.Item.Code == q1.i.Item)).Count() == 0
                   && uomConversions.Where(u => u.BaseUom == q1.d.Uom && u.AlterUom == q1.i.Uom && (u.Item == null || u.Item.Code == q1.i.Item)).Count() == 0))
                   .Select(q2 => new UomCheckView { Code = q2.d.Code, Description = q2.d.Description, Type = "BomMaster", Item = q2.i.Item, AlterUom = q2.d.Uom })
                   .ToList();
            uomCheckViews.AddRange(q_bm2);

            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            #region Head
            str.Append("<th >");
            str.Append(Resources.EXT.ControllerLan.Con_RowCount);
            str.Append("</th>");

            str.Append("<th >");
            str.Append(Resources.EXT.ControllerLan.Con_Type);
            str.Append("</th>");

            str.Append("<th >");
            str.Append(Resources.EXT.ControllerLan.Con_Code);
            str.Append("</th>");

            str.Append("<th >");
            str.Append(Resources.EXT.ControllerLan.Con_Item);
            str.Append("</th>");


            str.Append("<th >");
            str.Append(Resources.EXT.ControllerLan.Con_Description);
            str.Append("</th>");

            str.Append("<th >");
            str.Append(Resources.EXT.ControllerLan.Con_BaseUom);
            str.Append("</th>");

            str.Append("<th >");
            str.Append(Resources.EXT.ControllerLan.Con_TrandformUom);
            str.Append("</th>");

            str.Append("</tr></thead><tbody>");
            #endregion

            int l = 0;
            foreach (var view in uomCheckViews.Distinct())
            {
                l++;
                if (l % 2 == 0)
                {
                    str.Append("<tr class=\"t-alt\">");
                }
                else
                {
                    str.Append("<tr>");
                }
                str.Append("<td>");
                str.Append(l);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(view.Type);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(view.Code);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(view.Item);
                str.Append("</td>");

                Item item = itemMgr.GetCacheItem(view.Item);
                str.Append("<td>");
                str.Append(item.Description);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(item.Uom);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(view.AlterUom);
                str.Append("</td>");

                str.Append("</tr>");
            }
            str.Append("</tbody></table>");
            return str.ToString();
        }

        [SconitAuthorize(Permissions = "Url_MrpSnap_MasterDataCheck")]
        public string _CheckFlow()
        {
            try
            {
                mrpMgr.CheckFlow();
                //SaveSuccessMessage(string.Format("物流路线的完整性检查成功"));
                return Resources.EXT.ControllerLan.Con_LogisticsFlowIntegrityCheckSuccessfully;
            }
            catch (BusinessException ex)
            {
                StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

                #region Head
                str.Append("<th >");
                str.Append(Resources.EXT.ControllerLan.Con_RowCount);
                str.Append("</th>");

                str.Append("<th >");
                str.Append(Resources.EXT.ControllerLan.Con_Flow);
                str.Append("</th>");

                str.Append("<th >");
                str.Append(Resources.EXT.ControllerLan.Con_FlowDescription);
                str.Append("</th>");

                str.Append("<th >");
                str.Append(Resources.EXT.ControllerLan.Con_Type);
                str.Append("</th>");

                str.Append("<th >");
                str.Append(Resources.EXT.ControllerLan.Con_Item);
                str.Append("</th>");

                str.Append("<th >");
                str.Append(Resources.EXT.ControllerLan.Con_ItemDescription);
                str.Append("</th>");

                str.Append("<th >");
                str.Append(Resources.EXT.ControllerLan.Con_LocationFrom);
                str.Append("</th>");

                str.Append("<th >");
                str.Append(Resources.EXT.ControllerLan.Con_LocationTo);
                str.Append("</th>");

                str.Append("</tr></thead><tbody>");
                #endregion

                int l = 0;
                var messages = ex.GetMessages().Select(p => p.GetMessageString()).Distinct()
                    .Select(p => p.Split('|')).OrderBy(p => p[0]);

                foreach (var msg in messages)
                {
                    l++;
                    if (l % 2 == 0)
                    {
                        str.Append("<tr class=\"t-alt\">");
                    }
                    else
                    {
                        str.Append("<tr>");
                    }

                    str.Append("<td>");
                    str.Append(l);
                    str.Append("</td>");

                    if (msg.Length == 6)
                    {

                        str.Append("<td>");
                        str.Append(msg[0]);
                        str.Append("</td>");

                        str.Append("<td>");
                        str.Append(msg[1]);
                        str.Append("</td>");

                        str.Append("<td>");
                        str.Append(msg[2]);
                        str.Append("</td>");

                        str.Append("<td>");
                        str.Append(msg[3]);
                        str.Append("</td>");

                        string description = string.Empty;
                        var itemCodes = msg[3].Split('/');
                        if (itemCodes.Length > 1)
                        {
                            var item = itemMgr.GetCacheItem(itemCodes[0]);
                            description = item.Description;
                            description += " / " + itemMgr.GetCacheItem(itemCodes[1]).Description;
                        }
                        else
                        {
                            Item item = itemMgr.GetCacheItem(msg[3]);
                            description = item.Description;
                        }
                        str.Append("<td>");
                        str.Append(description);
                        str.Append("</td>");

                        str.Append("<td>");
                        str.Append(msg[4]);
                        str.Append("</td>");

                        str.Append("<td>");
                        str.Append(msg[5]);
                        str.Append("</td>");
                    }
                    else
                    {
                        str.Append("<td colspan=\"7\">");
                        str.Append(msg[0]);
                        str.Append("</td>");
                    }

                    str.Append("</tr>");
                }
                str.Append("</tbody></table>");
                return str.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [SconitAuthorize(Permissions = "Url_MrpSnap_MasterDataCheck")]
        public string _CheckCheckRedundanceFlow()
        {
            //SqlParameter[] sqlParam = new SqlParameter[5];
            //sqlParam[0] = new SqlParameter("@p1", itemCategory);
            //sqlParam[1] = new SqlParameter("@p2", startTime);
            //sqlParam[2] = new SqlParameter("@p3", endTime);
            //sqlParam[3] = new SqlParameter("@p4", itemCode);
            //sqlParam[4] = new SqlParameter("@p5", flowCode);

            DataSet ds = genericMgr.GetDatasetBySql(
                    @"select max(Id) as Id,item,m.LocTo,count(Id) as count into #a from SCM_FlowDet d
                    join SCM_FlowMstr m on d.Flow = m.Code 
                    where m.LocTo is not null and m.IsActive = 1 and m.IsMRP = 1 
                    and (d.StartDate is null or d.StartDate<=getdate()) and(d.EndDate is null or d.EndDate>getdate())
                    group by m.LocTo,item
                    having count(Id)>1
                    select d.Flow,m.Desc1,l.Name,d.Item,i.Desc1 from #a 
                    join SCM_FlowMstr m on m.LocTo = #a.locto
                    join SCM_FlowDet d on d.Item = #a.item and m.Code = d.Flow
                    left join MD_Item i on i.Code = d.Item
                    left join MD_Location l on l.Code = m.LocTo
                    where (d.StartDate is null or d.StartDate<=getdate()) and(d.EndDate is null or d.EndDate>getdate())
                    order by d.Item,m.Code
                    ", null);

            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            #region Head
            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_RowCount);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_Flow);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_FlowDescription);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_LocationTo);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_Item);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_ItemDescription);
            str.Append("</th>");

            str.Append("</tr></thead><tbody>");
            #endregion

            int l = 0;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                //(string)dr.ItemArray[0], (decimal)dr.ItemArray[1]
                l++;
                if (l % 2 == 0)
                {
                    str.Append("<tr class=\"t-alt\">");
                }
                else
                {
                    str.Append("<tr>");
                }
                str.Append("<td>");
                str.Append(l);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[0]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[1]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[2]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[3]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[4]);
                str.Append("</td>");

                str.Append("</tr>");
            }
            str.Append("</tbody></table>");
            return str.ToString();
        }

        [SconitAuthorize(Permissions = "Url_MrpSnap_MasterDataCheck")]
        public string _CheckOnlyInProductLine()
        {
            string sql = @"select m.Code as 路线,m.Desc1 as 路线描述,i.Code as 物料,i.Desc1 as 物料描述 from SCM_FlowDet d
                    join SCM_FlowMstr m on d.Flow = m.Code
                    join MD_Item  i on i.Code = d.Item
                    where m.Type =4
                    and Item not in(
                    select distinct(item) from SCM_FlowDet d
                    join SCM_FlowMstr m on d.Flow = m.Code
                    where m.Type !=4)";
            return GetTableHtmlBySql(sql);

            
        }

        #region
        [SconitAuthorize(Permissions = "Url_MrpSnap_MasterDataCheck")]
        public string _CheckProdExFlowDet()
        {
            DataSet ds = genericMgr.GetDatasetBySql(
                      @"Select a.ProductLine,b.Bom,a.productline as flow,a.Item As section into #BomOfFlow from MRP_ProdLineEx a with(nolock) ,PRD_BomDet b with(nolock) where a.Item=b.Item
                            and a.StartDate<=GETDATE() and a.EndDate>GETDATE() and b.StartDate<=GETDATE() and (b.EndDate is null or b.EndDate>GETDATE());
                        Select '断面BOM有,路线明细没有' as 类型,a.ProductLine As 生产线,a.section As 断面,SPACE(200) As 断面描述,a.Bom As 物料,b.Desc1 As 物料描述,b.Uom As 单位,b.UC As 单包装 
                            into #ReturnTable from #BomOfFlow a ,MD_Item b with(nolock) where  a.bom=b.code and not exists
                            (select 1 from SCM_FlowDet b join SCM_FlowMstr m on m.Code = b.Flow where a.flow=b.flow  and a.bom =b.item and m.ResourceGroup =20)
                        Insert into #ReturnTable 
                        Select '路线明细有,断面BOM没有' as 类型,Flow As 生产线,'' As 断面,SPACE(200) As 断面描述,Item As 物料,c.Desc1 As 物料描述,a.Uom As 单位,a.UC As 单包装  
                            from SCM_FlowDet a ,MD_Item c,SCM_FlowMstr m where a.bom=c.code and m.Code = a.Flow and m.ResourceGroup =20 and  not exists  
	                        (select * from #BomOfFlow b where a.flow=b.flow  and a.item =b.bom)
                        Update a
	                        Set a.断面描述=b.Desc1 from #ReturnTable a,MD_Item b with(nolock) where a.断面=b.Code
                        Select * from #ReturnTable
                    ", null);
            return GetTableHtml(ds);
        }
        #endregion
        [SconitAuthorize(Permissions = "Url_MrpSnap_MasterDataCheck")]
        public string _CheckOnlyNotInProductLine()
        {
            DataSet ds = genericMgr.GetDatasetBySql(
                     @"select m.Code,m.Desc1,i.Code,i.Desc1 from SCM_FlowDet d
                        join SCM_FlowMstr m on d.Flow = m.Code
                        join MD_Item  i on i.Code = d.Item
                        where m.Type not in(4,1,5)
                        and Item not in(
                        select distinct(item) from SCM_FlowDet d
                        join SCM_FlowMstr m on d.Flow = m.Code
                        where m.Type in(4,1,5))
                    ", null);

            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            #region Head
            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_RowCount);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_Flow);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_FlowDescription);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_Item);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_ItemDescription);
            str.Append("</th>");

            str.Append("</tr></thead><tbody>");
            #endregion

            int l = 0;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                //(string)dr.ItemArray[0], (decimal)dr.ItemArray[1]
                l++;
                if (l % 2 == 0)
                {
                    str.Append("<tr class=\"t-alt\">");
                }
                else
                {
                    str.Append("<tr>");
                }
                str.Append("<td>");
                str.Append(l);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[0]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[1]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[2]);
                str.Append("</td>");

                str.Append("<td>");
                str.Append(dr.ItemArray[3]);
                str.Append("</td>");

                str.Append("</tr>");
            }
            str.Append("</tbody></table>");
            return str.ToString();
        }

        class UomCheckView
        {
            public string Code { get; set; }
            public string Description { get; set; }
            public string SubType { get; set; }
            public string Type { get; set; }
            public string Item { get; set; }
            public string BaseUom { get; set; }
            public string AlterUom { get; set; }
        }
        #endregion
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MrpPlanView")]
        public ActionResult MrpPlanSnapView()
        {
            return View();
        }
        #region SnapMrpPlan
        public object _GetMaxPlanVersion(string flow, DateTime snapTime)
        {
            IList<object> param = new List<object>();
            param.Add(flow);
            param.Add(snapTime);
            IList<object> planVersions = this.queryMgr.FindAllWithNativeSql<object>
                 ("select MAX(PlanVersion) from MRP_MrpPlan where Flow =? and snaptime= ? ", param);
            return planVersions[0];
        }

        public string _GetMrpPlanView(DateTime snaptime, string flow, string item)
        {
            MrpPlanSearchModel searchModel = new MrpPlanSearchModel();
            searchModel.Flow = flow;
            searchModel.SnapTime = snaptime;
            searchModel.Item = item;
            TempData["MrpPlanSearchModel"] = searchModel;
            com.Sconit.Entity.ACC.User user = SecurityContextHolder.Get();
            int tableColumnCount;
            SqlParameter[] sqlParams = new SqlParameter[4];
            string reqUrl = HttpContext.Request.Url.Authority + HttpContext.Request.ApplicationPath;

            sqlParams[0] = new SqlParameter("@Flow", flow);
            sqlParams[1] = new SqlParameter("@Item", item);
            sqlParams[2] = new SqlParameter("@SnapTime", snaptime);
            DataSet ds = genericMgr.GetDatasetByStoredProcedure("USP_Busi_MRP_SnapHitory_MrpPlan", sqlParams);
            //table returned from SP is a temporary table ,so colculate columns in SP.
            tableColumnCount = (int)ds.Tables[0].Rows[0][0];
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            #region Head
            if (tableColumnCount == 0)
            {
                str.Clear();
                str.Append("<p>");
                str.Append(Resources.EXT.ControllerLan.Con_NoData);
                str.Append("</p>");
                return str.ToString();
            }
            //DateTime dt = DateTime.ParseExact(ds.Tables[1].Columns[4].ColumnName, "yyyy-MM-dd", null);
            for (int i = 0; i < tableColumnCount; i++)
            {
                if (i < 5)
                {
                    int width = 0;
                    switch (i)
                    {
                        case 0:
                            width = 85; break;
                        case 1:
                            width = 30; break;
                        case 2:
                            width = 40; break;
                        case 3:
                            width = 200; break;
                        case 4:
                            width = 30; break;
                        default:
                            width = 40; break;

                    }
                    str.Append("<th  style='min-width:" + width + "px'>");
                    str.Append(ds.Tables[1].Columns[i].ColumnName);
                    str.Append("</th>");
                }
                else
                {
                    str.Append("<th  style='min-width:40px'>");
                    //if (user.UrlPermissions.Contains("Url_OrderMstr_Distribution_New"))
                    //{
                    //    ///DistributionOrder/NewFromPlan?flow=O100031&planDate=30207
                    //    string url = string.Format("<a href='http://{0}DistributionOrder/NewFromPlan?Flow={1}&PlanDate={2}&StartDate={3}&BackUrl={4}'>{5}</a>",
                    //        reqUrl, flow, dt.ToString("yyyy-MM-dd"), ds.Tables[1].Columns[i].ColumnName, "~/MrpSnap/MrpPlanSnapView", dt.ToString("MM-dd"));
                    //    str.Append(url);
                    //}
                    //else
                    //{
                    str.Append(ds.Tables[1].Columns[i].ColumnName.Substring(5, 5));
                    //}
                    str.Append("</th>");
                    //dt = dt.AddDays(1);
                }
            }
            str.Append("</tr>");
            #endregion
            int l = 0;
            string trcss = string.Empty;
            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
            {
                l++;
                trcss = "";
                if (l % 2 == 0)
                {
                    trcss = "t-alt";
                }
                str.Append("<tr class=\"");
                str.Append(trcss);
                str.Append("\">");
                for (int j = 0; j < tableColumnCount; j++)
                {
                    str.Append("<td>");
                    str.Append(ds.Tables[1].Rows[i][j]);
                    str.Append("</td>");
                }

                str.Append("</tr>");
            }

            str.Append("</tbody></table>");
            return str.ToString();


        }
        #endregion
        #region Export SnapMrpPlan
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MrpPlanView")]
        public ActionResult ExportMrpPlanSnapView(DateTime snaptime, string flow, string item)
        {
            var table = _GetMrpPlanView(snaptime, flow, item);
            return new DownloadFileActionResult(table, "MrpPlanView.xls");
        }
        #endregion
        #region MrpSnapMachine
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_Machine")]
        public ActionResult Machine()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_Machine")]
        public string _GetMachineInstanceView(DateTime snapTime, com.Sconit.CodeMaster.TimeUnit dateType,
          string code, string island, bool isShiftQuota, bool isShiftPerDay, bool isNormalWorkDayPerWeek, bool isMaxWorkDayPerWeek
            , bool isQty, bool isIslandQty, bool isShiftType, bool isMachineType)
        {
            int tableColumnCount; int mergerRows;
            SqlParameter[] sqlParams = new SqlParameter[12];
            string reqUrl = HttpContext.Request.Url.Authority + HttpContext.Request.ApplicationPath;

            sqlParams[0] = new SqlParameter("@DateType", dateType);
            sqlParams[1] = new SqlParameter("@SnapTime", snapTime);
            sqlParams[2] = new SqlParameter("@Island", island);
            sqlParams[3] = new SqlParameter("@Machine", code);
            sqlParams[4] = new SqlParameter("@isShiftQuota", isShiftQuota);
            sqlParams[5] = new SqlParameter("@isShiftPerDay", isShiftPerDay);
            sqlParams[6] = new SqlParameter("@isNormalWorkDayPerWeek", isNormalWorkDayPerWeek);
            sqlParams[7] = new SqlParameter("@isMaxWorkDayPerWeek", isMaxWorkDayPerWeek);
            sqlParams[8] = new SqlParameter("@isQty", isQty);
            sqlParams[9] = new SqlParameter("@isIslandQty", isIslandQty);
            sqlParams[10] = new SqlParameter("@isShiftType", isShiftType);
            sqlParams[11] = new SqlParameter("@isMachineType", isMachineType);

            DataSet ds = genericMgr.GetDatasetByStoredProcedure("USP_Busi_MRP_SnapHitory_Machine", sqlParams);
            //table returned from SP is a temporary table ,so colculate columns in SP.
            tableColumnCount = (int)ds.Tables[0].Rows[0][0]; mergerRows = (int)ds.Tables[0].Rows[0][1];
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");
            StringBuilder strItem = new StringBuilder();
            if (tableColumnCount == 0)
            {
                str.Clear();
                str.Append("<p>");
                str.Append(Resources.EXT.ControllerLan.Con_NoData);
                str.Append("</p>");
                return str.ToString();
            }
            var flowDetailDic = this.genericMgr.FindAll<FlowDetail>
                (" from FlowDetail where Flow like ? and Machine is not null", "FI%")
                .GroupBy(p => p.Machine, (k, g) => new { k, g }).ToDictionary(d => d.k, d => d.g);
            #region Head
            for (int i = 0; i < tableColumnCount; i++)
            {
                int width = 0;
                switch (i)
                {
                    case 0:
                        width = 50; break;
                    case 1:
                        width = 75; break;
                    case 2:
                        width = 50; break;
                    case 3:
                        width = 60; break;
                    default:
                        width = 45; break;

                }
                str.Append("<th style='min-width:" + width + "px'>");

                str.Append(ds.Tables[1].Columns[i].ColumnName);
                str.Append("</th>");
            }
            str.Append("</tr></thead><tbody>");
            #endregion
            int l = 0;
            int markMerge = 0;
            string trcss = string.Empty;
            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
            {
                for (int j = 0; j < tableColumnCount; j++)
                {
                    if (markMerge == 0 && j == 0)
                    {
                        l++;
                        trcss = "";
                        if (l % 2 == 0)
                        {
                            trcss = "t-alt";
                        }
                        if (true)
                        {
                            strItem.Clear();
                            strItem.Append("<table>");
                            var machine = ds.Tables[1].Rows[i][1].ToString().Split('<')[0];
                            var flowDetails = flowDetailDic.ValueOrDefault(machine) ?? new List<FlowDetail>();

                            foreach (var flowDetail in flowDetails)
                            {
                                strItem.Append("<tr><td>");
                                strItem.Append(flowDetail.Item);
                                strItem.Append("</td></tr>");
                            }
                            strItem.Append("</table></td>");
                        }
                        str.Append("<tr class=\"");
                        str.Append(trcss);
                        str.Append("\">");
                        str.Append("<td rowspan=\"" + mergerRows + "\" style=\"text-align:center\" >"); str.Append(ds.Tables[1].Rows[i][0]); str.Append("</td>");
                        str.Append("<td rowspan=\"" + mergerRows + "\" style=\"text-align:center\" >"); str.Append(ds.Tables[1].Rows[i][1]); str.Append("</td>");
                        str.Append("<td rowspan=\"" + mergerRows + "\" style=\"text-align:center\" >"); str.Append(strItem.ToString()); str.Append("</td>");
                    }
                    else if (j > 2)
                    {
                        if (j == 3 && markMerge != 0)
                        {
                            str.Append("<tr class=\"");
                            str.Append(trcss);
                            str.Append("\" >");
                            str.Append("<td>");
                            str.Append(ds.Tables[1].Rows[i][j].ToString().Substring(1));
                            str.Append("</td>");
                        }
                        else
                        {
                            str.Append("<td>");
                            str.Append(j == 3 ? ds.Tables[1].Rows[i][j].ToString().Substring(1) : ds.Tables[1].Rows[i][j]);
                            str.Append("</td>");
                        }
                    }
                }
                str.Append("</tr>");
                markMerge++;
                if (markMerge == mergerRows)
                {
                    markMerge = 0;
                }
                str.Append("</tr>");
            }

            str.Append("</tbody></table>");
            return str.ToString();
        }

        #endregion
        #region Export MrpSnapMachine
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_Machine")]
        public ActionResult ExportMachine(DateTime snapTime, com.Sconit.CodeMaster.TimeUnit dateType,
          string code, string island, bool isShiftQuota, bool isShiftPerDay, bool isNormalWorkDayPerWeek, bool isMaxWorkDayPerWeek
            , bool isQty, bool isIslandQty, bool isShiftType, bool isMachineType)
        {
            var table = _GetMachineInstanceView(snapTime, dateType,
          code, island, isShiftQuota, isShiftPerDay, isNormalWorkDayPerWeek, isMaxWorkDayPerWeek
            , isQty, isIslandQty, isShiftType, isMachineType);
            return new DownloadFileActionResult(table, "Machine.xls");
        }
        #endregion
        #region  ProdLineEx
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_ProdLineEx")]
        public ActionResult ProdLineEx()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_ProdLineEx")]
        public string _GetProdLineExInstanceView(DateTime snapTime, com.Sconit.CodeMaster.TimeUnit dateType,
            string productLine, string item, bool isMrpSpeed, bool isRccpSpeed, bool isApsPriority, bool isQuota,
            bool isSwichTime, bool isSpeedTimes, bool isMinLotSize, bool isEconomicLotSize, bool isMaxLotSize,
            bool isTurnQty, bool isCorrection, bool isShiftType)
        {
            int tableColumnCount; int mergerRows;
            SqlParameter[] sqlParams = new SqlParameter[16];
            string reqUrl = HttpContext.Request.Url.Authority + HttpContext.Request.ApplicationPath;

            sqlParams[0] = new SqlParameter("@DateType", (int)dateType);
            sqlParams[1] = new SqlParameter("@SnapTime", snapTime);
            sqlParams[2] = new SqlParameter("@ProductLine", productLine);
            sqlParams[3] = new SqlParameter("@Item", item);
            sqlParams[4] = new SqlParameter("@isMrpSpeed", isMrpSpeed);
            sqlParams[5] = new SqlParameter("@isRccpSpeed", isRccpSpeed);
            sqlParams[6] = new SqlParameter("@isApsPriority", isApsPriority);
            sqlParams[7] = new SqlParameter("@isQuota", isQuota);
            sqlParams[8] = new SqlParameter("@isSwichTime", isSwichTime);
            sqlParams[9] = new SqlParameter("@isSpeedTimes", isSpeedTimes);
            sqlParams[10] = new SqlParameter("@isMinLotSize", isMinLotSize);
            sqlParams[11] = new SqlParameter("@isEconomicLotSize", isEconomicLotSize);
            sqlParams[12] = new SqlParameter("@isMaxLotSize", isMaxLotSize);
            sqlParams[13] = new SqlParameter("@isTurnQty", isTurnQty);
            sqlParams[14] = new SqlParameter("@isCorrection", isCorrection);
            sqlParams[15] = new SqlParameter("@isShiftType", isShiftType);

            DataSet ds = genericMgr.GetDatasetByStoredProcedure("USP_Busi_MRP_SnapHitory_ProdLineEx", sqlParams);
            //table returned from SP is a temporary table ,so colculate columns in SP.
            tableColumnCount = (int)ds.Tables[0].Rows[0][0]; mergerRows = (int)ds.Tables[0].Rows[0][1];
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");
            if (tableColumnCount == 0)
            {
                str.Clear();
                str.Append("<p>");
                str.Append(Resources.EXT.ControllerLan.Con_NoData);
                str.Append("</p>");
                return str.ToString();
            }
            #region Head
            for (int i = 0; i < tableColumnCount; i++)
            {
                str.Append("<th style='min-width:" + (i > 1 ? 45 : 80) + "px'>");
                str.Append(ds.Tables[1].Columns[i].ColumnName);
                str.Append("</th>");
            }
            str.Append("</tr></thead><tbody>");
            #endregion
            int l = 0;
            int markMerge = 0;
            string trcss = string.Empty;
            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
            {
                for (int j = 0; j < tableColumnCount; j++)
                {
                    if (markMerge == 0 && j == 0)
                    {
                        l++;
                        trcss = "";
                        if (l % 2 == 0)
                        {
                            trcss = "t-alt";
                        }
                        str.Append("<tr class=\"");
                        str.Append(trcss);
                        str.Append("\">");
                        str.Append("<td rowspan=\"" + mergerRows + "\" style=\"text-align:center\" >"); str.Append(ds.Tables[1].Rows[i][0]); str.Append("</td>");
                    }
                    if (j > 0)
                    {
                        if (j == 1 && markMerge != 0)
                        {
                            str.Append("<tr class=\"");
                            str.Append(trcss);
                            str.Append("\" >");
                            str.Append("<td>");
                            str.Append(ds.Tables[1].Rows[i][j].ToString().Substring(1));
                            str.Append("</td>");
                        }
                        else
                        {
                            str.Append("<td>");
                            str.Append(j == 1 ? ds.Tables[1].Rows[i][j].ToString().Substring(1) : ds.Tables[1].Rows[i][j]);
                            str.Append("</td>");
                        }
                    }
                }
                str.Append("</tr>");
                markMerge++;
                if (markMerge == mergerRows)
                {
                    markMerge = 0;
                }
                str.Append("</tr>");
            }
            str.Append("</tbody></table>");
            return str.ToString();
        }
        #endregion
        #region Export ProdLineEx
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_ProdLineEx")]
        public ActionResult ExportProdLineEx(DateTime snapTime, com.Sconit.CodeMaster.TimeUnit dateType,
            string productLine, string item, bool isMrpSpeed, bool isRccpSpeed, bool isApsPriority, bool isQuota,
            bool isSwichTime, bool isSpeedTimes, bool isMinLotSize, bool isEconomicLotSize, bool isMaxLotSize,
            bool isTurnQty, bool isCorrection, bool isShiftType)
        {
            var table = _GetProdLineExInstanceView(snapTime, dateType,
            productLine, item, isMrpSpeed, isRccpSpeed, isApsPriority, isQuota,
            isSwichTime, isSpeedTimes, isMinLotSize, isEconomicLotSize, isMaxLotSize,
            isTurnQty, isCorrection, isShiftType);
            return new DownloadFileActionResult(table, "ProdLineEx.xls");
        }
        #endregion
        #region MachineInstance
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MachineInstance")]
        public ActionResult MachineInstance()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MachineInstance")]
        public string _GetMachineInstance(string island, string machine, string item, DateTime date)
        {
            int tableColumnCount; int mergerRows;
            SqlParameter[] sqlParams = new SqlParameter[16];
            string dateIndex = date.ToString("yyyy-MM-dd");
            sqlParams[0] = new SqlParameter("@Island", island);
            sqlParams[1] = new SqlParameter("@Machine", machine);
            sqlParams[2] = new SqlParameter("@Item", item);
            sqlParams[3] = new SqlParameter("@DateIndex", dateIndex);

            DataSet ds = genericMgr.GetDatasetByStoredProcedure("USP_Busi_MRP_FiInstance", sqlParams);
            //table returned from SP is a temporary table ,so colculate columns in SP.
            tableColumnCount = (int)ds.Tables[0].Rows[0][0]; mergerRows = (int)ds.Tables[0].Rows[0][1];
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");
            if (tableColumnCount == 0)
            {
                str.Clear();
                str.Append("<p>");
                str.Append(Resources.EXT.ControllerLan.Con_NoData);
                str.Append("</p>");
                return str.ToString();
            }
            #region Head
            for (int i = 0; i < tableColumnCount; i++)
            {
                //SP return each column's length
                str.Append("<th style='min-width:" + (int)ds.Tables[1].Rows[0][i] + "px'>");
                str.Append(ds.Tables[2].Columns[i].ColumnName);
                str.Append("</th>");
            }
            str.Append("</tr></thead><tbody>");
            #endregion
            int l = 0;
            int mergeIsland = 0;
            int mergeMachine = 0;
            int markMergeIsland = 0;
            int markMergeMachine = 0;
            string trcss = string.Empty;
            for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
            {
                if (markMergeIsland == 0)
                {
                    mergeIsland = (int)ds.Tables[2].Rows[i][tableColumnCount];
                }
                if (markMergeMachine == 0)
                {
                    mergeMachine = (int)ds.Tables[2].Rows[i][tableColumnCount + 1];
                }
                for (int j = 0; j < tableColumnCount; j++)
                {
                    if (markMergeIsland == 0 && markMergeMachine == 0 && j == 0)
                    {
                        l++;
                        trcss = "";
                        if (l % 2 == 0)
                        {
                            trcss = "t-alt";
                        }
                        str.Append("<tr class=\"");
                        str.Append(trcss);
                        str.Append("\">");
                        str.Append("<td rowspan=\"" + mergeIsland + "\" style=\"text-align:center\" >"); str.Append(ds.Tables[2].Rows[i][0]); str.Append("</td>");
                        str.Append("<td rowspan=\"" + mergeMachine + "\" style=\"text-align:center\" >"); str.Append(ds.Tables[2].Rows[i][1]); str.Append("</td>");
                        str.Append("<td rowspan=\"" + mergeMachine + "\" style=\"text-align:center\" >"); str.Append(ds.Tables[2].Rows[i][2]); str.Append("</td>");
                    }
                    if (markMergeMachine == 0 && markMergeIsland != 0 && j == 1)
                    {
                        str.Append("<tr class=\"");
                        str.Append(trcss);
                        str.Append("\">");
                        str.Append("<td rowspan=\"" + mergeMachine + "\" style=\"text-align:center\" >"); str.Append(ds.Tables[2].Rows[i][1]); str.Append("</td>");
                        str.Append("<td rowspan=\"" + mergeMachine + "\" style=\"text-align:center\" >"); str.Append(ds.Tables[2].Rows[i][2]); str.Append("</td>");
                    }

                    if (j > 2)
                    {
                        if (j == 3 && markMergeMachine != 0 && markMergeIsland != 0)
                        {
                            str.Append("<tr class=\"");
                            str.Append(trcss);
                            str.Append("\" >");
                        }
                        str.Append("<td>");
                        str.Append(ds.Tables[2].Rows[i][j]);
                        str.Append("</td>");
                    }
                }
                str.Append("</tr>");
                markMergeIsland++;
                markMergeMachine++;
                if (markMergeIsland == mergeIsland)
                {
                    markMergeIsland = 0;
                }
                if (markMergeMachine == mergeMachine)
                {
                    markMergeMachine = 0;
                }
                str.Append("</tr>");
            }
            str.Append("</tbody></table>");
            return str.ToString();
        }
        #endregion
        #region Export MachineInstance
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MachineInstance")]
        public ActionResult ExportMachineInstance(string island, string machine, string item, DateTime date)
        {
            var table = _GetMachineInstance(island, machine, item, date);
            return new DownloadFileActionResult(table, "MachineInstance.xls");
        }
        #endregion




    }
}
