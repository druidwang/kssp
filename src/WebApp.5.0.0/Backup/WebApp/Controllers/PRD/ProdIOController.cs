namespace com.Sconit.Web.Controllers.PRD
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using com.Sconit.Entity.PRD;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.PRD;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using com.Sconit.Entity.SCM;
    using System;
    using System.Text;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Entity.INV;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Web.Models.SearchModels.ORD;
    using System.Data.SqlClient;
    using System.Data;

    public class ProdIOController : WebAppBaseController
    {
        #region Properties
        //public IGenericMgr genericMgr { get; set; }
        #endregion

        private static string selectDetailCountStatement = "select count(*) from OrderDetail as d";

        private static string selectDetailStatement = "select d from OrderDetail as d";

        private static string selectOrderBackflushDetailStatement = "select b from OrderBackflushDetail as b";

        [SconitAuthorize(Permissions = "Url_Production_ProdIO")]
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Production_ProdIO")]
        public ActionResult List(GridCommand command, OrderMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (this.CheckSearchModelIsNull(searchCacheModel.SearchObject))
            {
                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.SYS.ErrorMessage.Errors_NoConditions);
            }
            ViewBag.DisplayType = searchModel.DisplayType;
            return View();
        }

        #region detail
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Production_ProdIO")]
        public ActionResult _OrderDetailHierarchyAjax(GridCommand command, OrderMasterSearchModel searchModel)
        {
            if (!this.CheckSearchModelIsNull(searchModel))
            {
                return PartialView(new GridModel(new List<OrderDetail>()));
            }
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel, false);
            return PartialView(GetAjaxPageData<OrderDetail>(searchStatementModel, command));

        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Production_ProdIO")]
        public ActionResult _OrderBackflushDetailHierarchyAjax(int orderDetailId)
        {
            string hql = "select f from OrderBackflushDetail as f where f.OrderDetailId = ?";
            IList<OrderBackflushDetail> orderBackflushDetailList = genericMgr.FindAll<OrderBackflushDetail>(hql, orderDetailId);
            return PartialView(new GridModel(orderBackflushDetailList));
        }
        #endregion

        #region summary
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Production_ProdIO")]
        public ActionResult _OrderSummaryHierarchyAjax(GridCommand command, OrderMasterSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel, true);
            IList<OrderDetail> orderDetailList = queryMgr.FindAll<OrderDetail>(searchStatementModel.GetSearchStatement(), searchStatementModel.Parameters);
            var summaryOrderDetailList = from d in orderDetailList
                                         group d by new { d.Item, d.ItemDescription, d.ReferenceItemCode, d.Uom } into result
                                         select new OrderDetail
                                         {
                                             Item = result.Key.Item,
                                             ItemDescription = result.Key.ItemDescription,
                                             ReferenceItemCode = result.Key.ReferenceItemCode,
                                             Uom = result.Key.Uom,
                                             ReceivedQty = result.Sum(r => r.ReceivedQty)
                                         };

            return PartialView(new GridModel(summaryOrderDetailList));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Production_ProdIO")]
        public ActionResult _OrderBackflushSummaryHierarchyAjax(GridCommand command, OrderMasterSearchModel searchModel, string item)
        {
            searchModel.Item = item;
            SearchStatementModel searchStatementModel = this.PrepareSearchSummaryStatement(command, searchModel);
            IList<OrderBackflushDetail> orderBackflushDetailList = queryMgr.FindAll<OrderBackflushDetail>(searchStatementModel.GetSearchStatement(), searchStatementModel.Parameters);
            var summaryOrderBackflushDetailList = from d in orderBackflushDetailList
                                                  group d by new { d.Item, d.ItemDescription, d.ReferenceItemCode, d.Uom } into result
                                                  select new OrderBackflushDetail
                                         {
                                             Item = result.Key.Item,
                                             ItemDescription = result.Key.ItemDescription,
                                             ReferenceItemCode = result.Key.ReferenceItemCode,
                                             Uom = result.Key.Uom,
                                             BackflushedQty = result.Sum(r => r.BackflushedQty)
                                         };

            return PartialView(new GridModel(summaryOrderBackflushDetailList));
        }
        #endregion

        private SearchStatementModel PrepareSearchStatement(GridCommand command, OrderMasterSearchModel searchModel, bool isSummary)
        {
            string whereStatement = "where d.OrderType = ?  and d.ReceivedQty > 0 and exists(select 1 from OrderMaster as m where d.OrderNo = m.OrderNo ";

            IList<object> param = new List<object>();

            param.Add((int)com.Sconit.CodeMaster.OrderType.Production);

            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyTo", searchModel.PartyTo, "m", ref whereStatement, param);

            if (searchModel.DateFrom != null)
            {
                HqlStatementHelper.AddGeStatement("StartDate", searchModel.DateFrom, "m", ref whereStatement, param);
            }
            if (searchModel.DateTo != null)
            {
                HqlStatementHelper.AddLeStatement("StartDate", searchModel.DateTo, "m", ref whereStatement, param);
            }
            SecurityHelper.AddRegionPermissionStatement(ref whereStatement, "m", "PartyTo");
            whereStatement += ")";

            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "d", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (!isSummary)
            {
                if (command.SortDescriptors.Count == 0)
                {
                    sortingStatement = " order by d.OrderNo desc";
                }
            }

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectDetailCountStatement;
            searchStatementModel.SelectStatement = selectDetailStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }


        private SearchStatementModel PrepareSearchSummaryStatement(GridCommand command, OrderMasterSearchModel searchModel)
        {
            string whereStatement = "where b.Item = ? and exists(select 1 from OrderMaster as m,OrderDetail as d where d.OrderNo = m.OrderNo and d.OrderType = m.Type and m.Type = ? and d.Item = ? and b.OrderDetailId = d.Id";
            IList<object> param = new List<object>();
            param.Add(searchModel.Item);
            param.Add((int)com.Sconit.CodeMaster.OrderType.Production);
            param.Add(searchModel.Item);

            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "m", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("PartyTo", searchModel.PartyTo, "m", ref whereStatement, param);

            if (searchModel.DateFrom != null)
            {
                HqlStatementHelper.AddGeStatement("StartDate", searchModel.DateFrom, "m", ref whereStatement, param);
            }
            if (searchModel.DateTo != null)
            {
                HqlStatementHelper.AddLeStatement("StartDate", searchModel.DateTo, "m", ref whereStatement, param);
            }
            SecurityHelper.AddRegionPermissionStatement(ref whereStatement, "m", "PartyTo");
            whereStatement += ")";


            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = string.Empty;
            searchStatementModel.SelectStatement = selectOrderBackflushDetailStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = string.Empty;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }


        [SconitAuthorize(Permissions = "Url_Production_ProdIOFI")]
        public ActionResult ProdIOFI()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Production_ProdIOMI")]
        public ActionResult ProdIOMI()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Production_ProdIOEX")]
        public ActionResult ProdIOEX()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Production_ForceMaterial")]
        public ActionResult ForceMaterial()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_Production_ProdIOMI,Url_Production_ProdIOFI,Url_Production_ProdIOEX")]
        public string _GetProductionInOut(string ResourceGroup, string Flow, DateTime DateFrom, DateTime DateTo,string SPName,string SearchType)
        {
            SqlParameter[] sqlParams = new SqlParameter[5];
            sqlParams[0] = new SqlParameter(SPName.Substring(23,2)=="EX"?"@OrderNo":"@ResourceGroup", ResourceGroup);
            sqlParams[1] = new SqlParameter("@Flow", Flow);
            sqlParams[2] = new SqlParameter("@DateFrom", DateFrom);
            sqlParams[3] = new SqlParameter("@DateTo", DateTo);
            if (!string.IsNullOrWhiteSpace(SearchType))
            {
                sqlParams[4] = new SqlParameter("@SearchType", SearchType);
            }

            DataSet ds = genericMgr.GetDatasetByStoredProcedure(SPName, sqlParams);
            //var q= from p in ds group p by p.
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            //库位	物料	描述	单位	

            #region Head
            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_RowCount);
            str.Append("</th>");

            str.Append("<th>");
            str.Append((SearchType == "1" ? Resources.EXT.ControllerLan.Con_FinishGood : Resources.EXT.ControllerLan.Con_SourceMaterial));
            str.Append("</th>");

            str.Append("<th>");
            str.Append((SearchType == "1" ? Resources.EXT.ControllerLan.Con_FinishGoodDescription : Resources.EXT.ControllerLan.Con_SourceMaterialDescription));
            str.Append("</th>");

            str.Append("<th>");
            str.Append((SearchType == "1" ? Resources.EXT.ControllerLan.Con_FinishGoodUom : Resources.EXT.ControllerLan.Con_SourceMaterialUom));
            str.Append("</th>");
            if (SearchType == "1" || SearchType == null)
            {
                str.Append("<th>");
                str.Append(Resources.EXT.ControllerLan.Con_OutputQuantity);
                str.Append("</th>");
            }
            //成品描述	成品单位	产出数量	原材料	描述	零件单位	理论用量	实际用量	偏差
            str.Append("<th>");
            str.Append((SearchType == "1" ? Resources.EXT.ControllerLan.Con_SourceMaterial : Resources.EXT.ControllerLan.Con_FinishGood));
            str.Append("</th>");

            str.Append("<th>");
            str.Append((SearchType == "1" ? Resources.EXT.ControllerLan.Con_SourceMaterialDescription : Resources.EXT.ControllerLan.Con_FinishGoodDescription));
            str.Append("</th>");

            str.Append("<th>");
            str.Append((SearchType == "1" ? Resources.EXT.ControllerLan.Con_SourceMaterialUom : Resources.EXT.ControllerLan.Con_FinishGoodUom));
            str.Append("</th>");

            if (SearchType == "2")
            {
                str.Append("<th>");
                str.Append(Resources.EXT.ControllerLan.Con_OutputQuantity);
                str.Append("</th>");
            }

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_TheoreticalAmount);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_ActualAmount);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.EXT.ControllerLan.Con_Deviation);
            str.Append("</th>");

            str.Append("</tr></thead><tbody>");
            #endregion

            int l = 0;
            int m = 0;//变合并单元格的颜色
            string style = string.Empty;
            //SP 结果集多返回两列，倒数第一列是以成品为单位给零件号排序，倒数第二列是每个成品的两件个数，即需要合并的行数。
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                l++;
                str.Append("<tr>");

                if ((int)dr.ItemArray[11] == 1)
                {
                    m++;

                    if (m % 2 != 0)//奇数单元格为灰色
                    {
                        style = "<td bgcolor=#E0E0E0 rowspan=" + (int)dr.ItemArray[10] + ">";
                    }
                    else//偶数不变色
                    {
                        style = "<td rowspan=" + (int)dr.ItemArray[10] + ">";
                    }
                    if ((string)dr.ItemArray[0] == Resources.EXT.ControllerLan.Con_NotBeenUsedStockTakeMaterial)//未被使用的盘点材料背景为黄色
                    {
                        style = "<td bgcolor=yellow rowspan=" + (int)dr.ItemArray[10] + ">";
                    }
                    str.Append(style);
                    str.Append(m);
                    str.Append("</td>");

                    str.Append(style);
                    str.Append(dr.ItemArray[0]);
                    str.Append("</td>");

                    str.Append(style);
                    str.Append(dr.ItemArray[1]);
                    str.Append("</td>");

                    str.Append(style);
                    str.Append(dr.ItemArray[2]);
                    str.Append("</td>");
                    if (SearchType == "1" || SearchType == null)
                    {
                        str.Append(style);
                        str.Append(dr.ItemArray[3]);
                        str.Append("</td>");
                    }
                }
                if (l % 2 == 0)//明细的偶数单元格为灰色
                {
                    style = "<td bgcolor=#E0E0E0>";
                }
                else//明细奇数不变色
                {
                    style = "<td>";
                }
                if ((string)dr.ItemArray[0] == Resources.EXT.ControllerLan.Con_NotBeenUsedStockTakeMaterial)//未被使用的盘点材料背景为黄色
                {
                    style = "<td bgcolor=yellow>";
                }
                str.Append(style);
                str.Append(dr.ItemArray[4]);
                str.Append("</td>");

                str.Append(style);
                str.Append(dr.ItemArray[5]);
                str.Append("</td>");

                str.Append(style);
                str.Append(dr.ItemArray[6]);
                str.Append("</td>");

                if (SearchType == "2")
                {
                    str.Append(style);
                    str.Append(dr.ItemArray[3]);
                    str.Append("</td>");
                }

                str.Append(style);
                str.Append(((decimal)dr.ItemArray[7]).ToString("0.##"));
                str.Append("</td>");

                str.Append(style);
                str.Append(((decimal)dr.ItemArray[8]).ToString("0.##"));
                str.Append("</td>");

                str.Append(style);
                str.Append(dr.ItemArray[9]);
                str.Append("</td>");

                str.Append("</tr>");
            }
            str.Append("</tbody></table>");
            return str.ToString();

        }
        #region Force Material
        [SconitAuthorize(Permissions = "Url_Mrp_MrpSnap_MachineInstance")]
        public string _GetForceMaterialView(string orderNo, DateTime? DateFrom, DateTime? DateTo)
        {
            int tableColumnCount; int mergerRows;
            SqlParameter[] sqlParams = new SqlParameter[3];
            sqlParams[0] = new SqlParameter("@OrderNo", orderNo);
            sqlParams[1] = new SqlParameter("@DateFrom", DateFrom);
            sqlParams[2] = new SqlParameter("@DateTo", DateTo);

            DataSet ds = genericMgr.GetDatasetByStoredProcedure("USP_Report_InPutOutPut_ForceMaterial", sqlParams);
            //table returned from SP is a temporary table ,so colculate columns in SP.
            tableColumnCount = (int)ds.Tables[0].Rows[0][0]; mergerRows = (int)ds.Tables[1].Rows[0][0];
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
            for (int i = 1; i < tableColumnCount; i++)
            {
                //SP return each column's length
                str.Append("<th style='min-width:" + (int)ds.Tables[2].Rows[0][i] + "px'>");
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
                for (int j = 1; j < tableColumnCount; j++)
                {
                    if (markMerge == 0 && ( j == 1||j == 2||j == 3 ||j == 4))
                    {
                        if (j == 1)
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
                        }
                        str.Append("<td rowspan=\"" + mergerRows + "\" style=\"text-align:center\" >"); str.Append(ds.Tables[1].Rows[i][j]); str.Append("</td>");
                    }
                    if (j > 4)
                    {
                        if (markMerge != 0 && j==5)
                        {
                            str.Append("<tr class=\"");
                            str.Append(trcss);
                            str.Append("\" >");
                        }
                            str.Append("<td>");
                            str.Append(ds.Tables[1].Rows[i][j]);
                            str.Append("</td>");

                    }
                }
                str.Append("</tr>");
                markMerge++;
                if (markMerge == mergerRows && i != ds.Tables[1].Rows.Count-1)
                {
                    markMerge = 0;
                    mergerRows = (int)ds.Tables[1].Rows[i+1][0];
                }
            }
            str.Append("</tbody></table>");
            return str.ToString();
        }
        #endregion

    }
}
