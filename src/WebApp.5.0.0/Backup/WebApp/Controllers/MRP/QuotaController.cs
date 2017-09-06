
namespace com.Sconit.Web.Controllers.MRP
{
    #region Retrive
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using com.Sconit.Web.Models.SearchModels.MRP;
    using com.Sconit.Web.Models;
    using com.Sconit.Entity.SCM;
    using com.Sconit.Entity.Exception;
    using com.Sconit.Service;
    #endregion

    public class QuotaController : WebAppBaseController
    {

        private static string selectCountStatement = "select count(*) from FlowDetail as fd";

        private static string selectStatement = "select fd from FlowDetail as fd";

        public IGenericMgr GenericMgr { get; set; }

        public IQueryMgr QueryMgr { get; set; }

        [SconitAuthorize(Permissions = "Url_Quota_View")]
        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Quota_View")]
        public ActionResult List(GridCommand command, QuotaSearchModel searchModel)
        {
            if (!string.IsNullOrEmpty(searchModel.Item) && !string.IsNullOrEmpty(searchModel.Location))
            {
                TempData["QuotaSearchModel"] = searchModel;
            }

            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Quota_View")]
        public ActionResult _AjaxList(GridCommand command, QuotaSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<FlowDetail>(searchStatementModel, command));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Quota_View")]
        //public string Inproportion(string idStr, string weightStr, string totalQtyStr, string adjQtyStr, string Location, string Item)
        public string Inproportion(string idStr, string Location, string Item)
        {
            QuotaSearchModel searchModel=new QuotaSearchModel{Location=Location,Item=Item};
            TempData["QuotaSearchModel"] = searchModel;
            try
            {
                if (string.IsNullOrEmpty(idStr))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_FlowDetailCanNotBeEmpty);
                }
                string[] idArr = idStr.Split(',');
                //string[] qtyArr = adjQtyStr.Split(',');
                //string[] weightArr = weightStr.Split(',');
                //string[] totalQtyArr = totalQtyStr.Split(',');

                decimal allWeight = 0;
                decimal allTotalQty = 0;
                decimal maxProportion = 0;
                IList<FlowDetail> FlowDetailList = new List<FlowDetail>();
                FlowDetail noNeedAdjDetail = new FlowDetail();
                for (int i = 0; i < idArr.Count(); i++)
                {
                    FlowDetail flowDetail = QueryMgr.FindById<FlowDetail>(Convert.ToInt32(idArr[i]));
                    allWeight += flowDetail.MrpWeight;
                    allTotalQty += flowDetail.MrpTotal;
                    if (flowDetail.MrpTotal / flowDetail.MrpWeight > maxProportion)
                    {
                        maxProportion = flowDetail.MrpTotal / flowDetail.MrpWeight;
                        noNeedAdjDetail = flowDetail;
                    }

                    FlowDetailList.Add(flowDetail);  
                }
                FlowDetailList.Remove(noNeedAdjDetail);
                if (noNeedAdjDetail.MrpTotalAdjust != 0)
                {
                    noNeedAdjDetail.MrpTotalAdjust = 0;
                    GenericMgr.Update(noNeedAdjDetail);
                }

                foreach (var flowDetail in FlowDetailList)
                {
                    flowDetail.MrpTotalAdjust = (noNeedAdjDetail.MrpTotal * flowDetail.MrpWeight) / noNeedAdjDetail.MrpWeight - flowDetail.MrpTotal;
                    GenericMgr.Update(flowDetail);
                }

                //pickListMgr.CreatePickList(orderDetailList);
                SaveSuccessMessage(Resources.MRP.Quota.Quota_Adjusted);
                return "Succeed";
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return string.Empty;
            }
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Quota_View")]
        public string Clear(string idStr, string Location, string Item)
        {
            QuotaSearchModel searchModel = new QuotaSearchModel { Location = Location, Item = Item };
            TempData["QuotaSearchModel"] = searchModel;
            try
            {
                if (string.IsNullOrEmpty(idStr))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_FlowDetailCanNotBeEmpty);
                }
                string[] idArr = idStr.Split(',');

                for (int i = 0; i < idArr.Count(); i++)
                {
                    FlowDetail flowDetail = QueryMgr.FindById<FlowDetail>(Convert.ToInt32(idArr[i]));
                    flowDetail.MrpTotalAdjust = 0;
                    GenericMgr.Update(flowDetail);

                }

                SaveSuccessMessage(Resources.MRP.Quota.Quota_Cleared);
                return "Succeed";
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true; 
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return string.Empty;
            }
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Quota_View")]
        public string Save(string idStr, string adjQtyStr, string Location, string Item)
        {
            QuotaSearchModel searchModel = new QuotaSearchModel { Location = Location, Item = Item };
            TempData["QuotaSearchModel"] = searchModel;
            try
            {
                if (string.IsNullOrEmpty(idStr))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_FlowDetailCanNotBeEmpty);
                }
                string[] idArr = idStr.Split(',');
                string[] qtyArr = adjQtyStr.Split(',');

                for (int i = 0; i < idArr.Count(); i++)
                {
                    FlowDetail flowDetail = QueryMgr.FindById<FlowDetail>(Convert.ToInt32(idArr[i]));
                    flowDetail.MrpTotalAdjust = decimal.Parse(qtyArr[i]);
                    GenericMgr.Update(flowDetail);

                }

                //pickListMgr.CreatePickList(orderDetailList);
                SaveSuccessMessage(Resources.MRP.Quota.Quota_Saved);
                return "Succeed";
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return string.Empty;
            }
        }



        private SearchStatementModel PrepareSearchStatement(GridCommand command, QuotaSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            whereStatement = " where fd.MrpWeight!=0 and fd.LocationTo = ? and fd.Item = ?";
            param.Add(string.IsNullOrEmpty(searchModel.Location) ? string.Empty : searchModel.Location);
            param.Add(string.IsNullOrEmpty(searchModel.Item) ? string.Empty : searchModel.Item);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
    }
}
