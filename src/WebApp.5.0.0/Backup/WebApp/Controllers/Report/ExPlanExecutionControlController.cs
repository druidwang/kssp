/// <summary>
/// Summary description 
/// </summary>

#region reference
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using com.Sconit.Entity.ORD;
using com.Sconit.Entity.SYS;
using com.Sconit.Service;
using com.Sconit.Web.Models;
using Telerik.Web.Mvc;
using com.Sconit.Service.Impl;
using com.Sconit.Entity.Exception;
using AutoMapper;
using com.Sconit.Utility;
using System.Text;
using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;
using com.Sconit.Persistence;
using com.Sconit.Web.Models.SearchModels.MRP;
using com.Sconit.Entity.MRP.TRANS;

#endregion

namespace com.Sconit.Web.Controllers.Report
{
    public class ExPlanExecutionControlController : WebAppBaseController
    {

        #region Properties

        public ISqlDao sqlDao { get; set; }

        #endregion

        #region public actions

        [SconitAuthorize(Permissions = "Url_ExPlanExecutionControl_View")]
        public ActionResult Index()
        {
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ExPlanExecutionControl_View")]
        public ActionResult List(GridCommand command, MrpPlanSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            if (searchModel.StartDate == null || searchModel.EndDate == null)
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_PlanTimeSpanCanNotBeEmpty);
            }
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxList(GridCommand command, MrpPlanSearchModel searchModel)
        {
            if (searchModel.StartDate == null || searchModel.EndDate == null)
            {
                return PartialView(new GridModel<MrpExShiftPlan>(new List<MrpExShiftPlan>()));
            }
            this.GetCommand(ref command, searchModel);
            if (command.SortDescriptors.Count > 0)
            {
                //if (command.SortDescriptors[0].Member == "ExternalOrderNo")
                //{
                //    command.SortDescriptors[0].Member = "ExtNo";
                //}
                //else if (command.SortDescriptors[0].Member == "OrderedQty")
                //{
                //    command.SortDescriptors[0].Member = "OrderQty";
                //}

            }
            SqlParameter[] parameters = new SqlParameter[7];
            parameters[0] = new SqlParameter("@ProdLine", System.Data.SqlDbType.VarChar, 50);
            parameters[0].Value = searchModel.ProdLine;

            parameters[1] = new SqlParameter("@PlanStartDate", System.Data.SqlDbType.DateTime);
            parameters[1].Value = searchModel.StartDate;

            parameters[2] = new SqlParameter("@PlanEndDate", System.Data.SqlDbType.DateTime);
            parameters[2].Value = searchModel.EndDate;

            parameters[3] = new SqlParameter("@SortCloumn", System.Data.SqlDbType.VarChar, 50);
            parameters[3].Value = command.SortDescriptors.Count > 0 ? command.SortDescriptors[0].Member : string.Empty;

            parameters[4] = new SqlParameter("@SortRule", System.Data.SqlDbType.VarChar, 50);
            parameters[4].Value = command.SortDescriptors.Count > 0 ? command.SortDescriptors[0].SortDirection == ListSortDirection.Descending ? "desc" : "asc" : string.Empty;

            parameters[5] = new SqlParameter("@PageSize", SqlDbType.Int);
            parameters[5].Value = command.PageSize;

            parameters[6] = new SqlParameter("@Page", SqlDbType.Int);
            parameters[6].Value = command.Page;

            //parameters[7] = new SqlParameter("@RowCount", System.Data.SqlDbType.VarChar, 50);
            //parameters[7].Direction = ParameterDirection.Output;
            int totalCount = 0;
            IList<MrpExShiftPlan> returList = new List<MrpExShiftPlan>();
            try
            {
                DataSet dataSet = sqlDao.GetDatasetByStoredProcedure("USP_Search_ExPlanExecutionControl", parameters);

                //  Section, Item, ItemDescription, Shift, Uom,PlanDate,ProductLine,Name, Qty, 
                //CorrectionQty, ReceivedQty,Remark,Sequence,StartTime,WindowTime,IsNew,IsFreeze
                if (dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        MrpExShiftPlan mrpShiftPlan = new MrpExShiftPlan();
                        mrpShiftPlan.Section = row.ItemArray[0].ToString();
                        mrpShiftPlan.Item = row.ItemArray[1].ToString();
                        mrpShiftPlan.ItemDescription = row.ItemArray[2].ToString();
                        //mrpShiftPlan.Shift = row.ItemArray[3].ToString();
                        mrpShiftPlan.Uom = row.ItemArray[4].ToString();
                        mrpShiftPlan.PlanDate =Convert.ToDateTime( row.ItemArray[5].ToString());
                        mrpShiftPlan.ProductLine = row.ItemArray[6].ToString();
                        mrpShiftPlan.Shift = row.ItemArray[7].ToString();
                        mrpShiftPlan.Qty = Convert.ToDouble(row.ItemArray[8].ToString());
                        mrpShiftPlan.IsCorrection = Convert.ToBoolean(row.ItemArray[9].ToString());
                        mrpShiftPlan.ReceivedQty = Convert.ToDouble(row.ItemArray[10].ToString());
                        mrpShiftPlan.Remark = row.ItemArray[11].ToString();
                        mrpShiftPlan.Sequence = Convert.ToInt32(row.ItemArray[12].ToString());
                        mrpShiftPlan.StartTime = Convert.ToDateTime(row.ItemArray[13].ToString());
                        mrpShiftPlan.WindowTime = Convert.ToDateTime(row.ItemArray[14].ToString());
                        mrpShiftPlan.IsNew = Convert.ToBoolean(row.ItemArray[15].ToString());
                        mrpShiftPlan.IsFreeze = Convert.ToBoolean(row.ItemArray[16].ToString());

                        returList.Add(mrpShiftPlan);
                    }
                    totalCount = (int)dataSet.Tables[1].Rows[0][0];
                }
            }
            catch (BusinessException be)
            {
                SaveBusinessExceptionMessage(be);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException != null)
                    {
                        SaveErrorMessage(ex.InnerException.InnerException.Message);
                    }
                    else
                    {
                        SaveErrorMessage(ex.InnerException.Message);
                    }
                }
                else
                {
                    SaveErrorMessage(ex.Message);
                }
            }
            GridModel<MrpExShiftPlan> gridModel = new GridModel<MrpExShiftPlan>();
            gridModel.Total = totalCount;
            gridModel.Data = returList;
            TempData["DetailList"] = returList;
            return PartialView(gridModel);
        }


        #region 查询导出
        public void ExportXLS(MrpPlanSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
           
            SqlParameter[] parameters = new SqlParameter[8];
            parameters[0] = new SqlParameter("@ProdLine", System.Data.SqlDbType.VarChar, 50);
            parameters[0].Value = searchModel.ProdLine;

            parameters[1] = new SqlParameter("@PlanStartDate", System.Data.SqlDbType.DateTime);
            parameters[1].Value = searchModel.StartDate;

            parameters[2] = new SqlParameter("@PlanEndDate", System.Data.SqlDbType.DateTime);
            parameters[2].Value = searchModel.EndDate;

            parameters[3] = new SqlParameter("@SortCloumn", System.Data.SqlDbType.VarChar, 50);
            parameters[3].Value = command.SortDescriptors.Count > 0 ? command.SortDescriptors[0].Member : string.Empty;

            parameters[4] = new SqlParameter("@SortRule", System.Data.SqlDbType.VarChar, 50);
            parameters[4].Value = command.SortDescriptors.Count > 0 ? command.SortDescriptors[0].SortDirection == ListSortDirection.Descending ? "desc" : "asc" : string.Empty;

            parameters[5] = new SqlParameter("@PageSize", SqlDbType.Int);
            parameters[5].Value = command.PageSize;

            parameters[6] = new SqlParameter("@Page", SqlDbType.Int);
            parameters[6].Value = command.Page;

            parameters[7] = new SqlParameter("@RowCount", System.Data.SqlDbType.VarChar, 50);
            parameters[7].Direction = ParameterDirection.Output;

            IList<MrpExShiftPlan> returList = new List<MrpExShiftPlan>();
            try
            {
                DataSet dataSet = sqlDao.GetDatasetByStoredProcedure("USP_Search_ExPlanExecutionControl", parameters);

                //  Section, Item, ItemDescription, Shift, Uom,PlanDate,ProductLine,Name, Qty, 
                //CorrectionQty, ReceivedQty,Remark,Sequence,StartTime,WindowTime,IsNew,IsFreeze
                if (dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        MrpExShiftPlan mrpShiftPlan = new MrpExShiftPlan();
                        mrpShiftPlan.Section = row.ItemArray[0].ToString();
                        mrpShiftPlan.Item = row.ItemArray[1].ToString();
                        mrpShiftPlan.ItemDescription = row.ItemArray[2].ToString();
                        //mrpShiftPlan.Shift = row.ItemArray[3].ToString();
                        mrpShiftPlan.Uom = row.ItemArray[4].ToString();
                        mrpShiftPlan.PlanDate = Convert.ToDateTime(row.ItemArray[5].ToString());
                        mrpShiftPlan.ProductLine = row.ItemArray[6].ToString();
                        mrpShiftPlan.Shift = row.ItemArray[7].ToString();
                        mrpShiftPlan.Qty = Convert.ToDouble(row.ItemArray[8].ToString());
                        mrpShiftPlan.IsCorrection = Convert.ToBoolean(row.ItemArray[9].ToString());
                        mrpShiftPlan.ReceivedQty = Convert.ToDouble(row.ItemArray[10].ToString());
                        mrpShiftPlan.Remark = row.ItemArray[11].ToString();
                        mrpShiftPlan.Sequence = Convert.ToInt32(row.ItemArray[12].ToString());
                        mrpShiftPlan.StartTime = Convert.ToDateTime(row.ItemArray[13].ToString());
                        mrpShiftPlan.WindowTime = Convert.ToDateTime(row.ItemArray[14].ToString());
                        mrpShiftPlan.IsNew = Convert.ToBoolean(row.ItemArray[15].ToString());
                        mrpShiftPlan.IsFreeze = Convert.ToBoolean(row.ItemArray[16].ToString());
                        returList.Add(mrpShiftPlan);
                    }
                }
            }
            catch (BusinessException be)
            {
                SaveBusinessExceptionMessage(be);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException != null)
                    {
                        SaveErrorMessage(ex.InnerException.InnerException.Message);
                    }
                    else
                    {
                        SaveErrorMessage(ex.InnerException.Message);
                    }
                }
                else
                {
                    SaveErrorMessage(ex.Message);
                }
            }
            ExportToXLS<MrpExShiftPlan>("ExportMrpExShiftPlan.xls", returList);
        }
        #endregion

        #endregion

    }
}
