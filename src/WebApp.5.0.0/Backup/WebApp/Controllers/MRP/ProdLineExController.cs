using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Services.Transaction;
using com.Sconit.Entity.MRP.MD;
using com.Sconit.Service;
using com.Sconit.Web.Models;
using com.Sconit.Web.Models.SearchModels.MRP;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Entity.SCM;
using System.Text;
using com.Sconit.Entity.MD;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace com.Sconit.Web.Controllers.MRP
{
    public class ProdLineExController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }

        private static string selectCountStatement = "select count(*) from ProdLineEx as pi";

        private static string selectStatement = "select pi from ProdLineEx as pi";

        private static string selectInstanceCountStatement = "select count(*) from ProdLineExInstance as p";

        private static string selectInstanceStatement = "select p from ProdLineExInstance as p";
        //
        // GET: /ProdLineEx/

        #region ProdLineEx


        [SconitAuthorize(Permissions = "Url_MRP_ProdLineEx_View")]
        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_MRP_ProdLineEx_View")]
        public ActionResult List(GridCommand command, ProdLineExSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_MRP_ProdLineEx_View")]
        public ActionResult _AjaxList(GridCommand command, ProdLineExSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            var list = GetAjaxPageData<ProdLineEx>(searchStatementModel, command);
            var productTypes = genericMgr.FindAll<ProductType>();
            foreach (var listdata in list.Data)
            {
                listdata.ItemDesc = itemMgr.GetCacheItem(listdata.Item).Description;
                listdata.ProductTypeDescription =
                    string.Format("{0}[{1}]", listdata.ProductType,
                    productTypes.Where(p => p.Code == listdata.ProductType).FirstOrDefault().Description);
            }

            return PartialView(list);
        }


        #region Edit
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_MRP_ProdLineEx_View")]
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return HttpNotFound();
            }
            else
            {
                ProdLineEx prodLineEx = this.genericMgr.FindById<ProdLineEx>(id);
                return View(prodLineEx);
            }
        }

        [SconitAuthorize(Permissions = "Url_MRP_ProdLineEx_View")]
        public ActionResult Edit(ProdLineEx prodLineEx)
        {
            if (ModelState.IsValid)
            {
                prodLineEx.Region = genericMgr.FindById<FlowMaster>(prodLineEx.ProductLine).PartyFrom;
                this.genericMgr.UpdateWithTrim(prodLineEx);
                SaveSuccessMessage(Resources.MRP.ProdLineEx.ProdLineEx_Updated);
            }

            return View(prodLineEx);
        }


        [SconitAuthorize(Permissions = "Url_MRP_ProdLineEx_View")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_MRP_ProdLineEx_View")]
        public ActionResult New(ProdLineEx prodLineEx)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    prodLineEx.Region = genericMgr.FindById<FlowMaster>(prodLineEx.ProductLine).PartyFrom;
                    this.genericMgr.CreateWithTrim(prodLineEx);
                    SaveSuccessMessage(Resources.MRP.ProdLineEx.ProdLineEx_Added);
                    string ProductLine = prodLineEx.ProductLine;
                    string Item = prodLineEx.Item;
                    //return RedirectToAction("Edit", new object[] { ProductLine, Item });
                    return new RedirectToRouteResult(new RouteValueDictionary { { "action", "Edit" }, { "controller", "ProdLineEx" }, { "id", prodLineEx.Id } });
                }
            }
            catch (Exception e)
            {
                if (e is CommitResourceException)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_AlreadyExitsSameData);
                }

            }

            return View(prodLineEx);
        }

        [SconitAuthorize(Permissions = "Url_MRP_ProdLineEx_View")]
        public ActionResult Delete(string ProductLine, string Item)
        {
            if (string.IsNullOrEmpty(ProductLine) || string.IsNullOrWhiteSpace(Item))
            {
                return HttpNotFound();
            }
            else
            {
                ProdLineEx prodLineEx = this.genericMgr.FindAll<ProdLineEx>("select pi from ProdLineEx as pi where pi.ProductLine=? and pi.Item=?", new object[] { ProductLine, Item })[0];
                this.genericMgr.Delete(prodLineEx);
                SaveSuccessMessage(Resources.MRP.ProdLineEx.ProdLineEx_Deleted);
                return RedirectToAction("List");
                // return new RedirectToRouteResult(new RouteValueDictionary { { "action", "Edit" }, { "controller", "ProdLineEx" }, { "id", itemEx.Code } });
            }
        }

        #endregion


        private SearchStatementModel PrepareSearchStatement(GridCommand command, ProdLineExSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("ProductLine", searchModel.ProductLine, HqlStatementHelper.LikeMatchMode.Start, "pi", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Item", searchModel.Item, HqlStatementHelper.LikeMatchMode.Start, "pi", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by pi.CreateDate desc";
            }

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
        #endregion

        #region Public Method

        #region View

        public ActionResult InstanceIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_MRP_ProdLineEx_View")]
        public ActionResult InstanceList(GridCommand command, ProdLineExInstanceSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            ViewBag.DateType = searchModel.DateType == null ? null : searchModel.DateType.ToString();
            return View();
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_MRP_ProdLineEx_View")]
        public ActionResult _AjaxInstanceList(GridCommand command, ProdLineExInstanceSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareInstanceSearchStatement(command, searchModel);
            var list = GetAjaxPageData<ProdLineEx>(searchStatementModel, command);
            var productType = genericMgr.FindAll<ProductType>("from ProductType as i");
            foreach (var listdata in list.Data)
            {
                listdata.ItemDesc = itemMgr.GetCacheItem(listdata.Item).Description;
                listdata.ProductTypeDescription = productType.Where(p => p.Code == listdata.ProductType).FirstOrDefault().Description;
            }

            return PartialView(list);
        }
        #endregion
        #region 导出挤出资源日历
        [SconitAuthorize(Permissions = "Url_MRP_ProdLineEx_View")]
        public void ExportXLS(ProdLineExInstanceSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(com.Sconit.Entity.SYS.EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;
            SearchStatementModel searchStatementModel = this.PrepareInstanceSearchStatement(command, searchModel);
            var fileName = string.Format("EXCalendar.xls");
            ExportToXLS<ProdLineExInstance>(fileName, GetAjaxPageData<ProdLineExInstance>(searchStatementModel, command).Data.ToList());
        }

        #endregion
        #region Edit
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_MRP_ProdLineEx_View")]
        public ActionResult InstanceEdit(string productLine, string item, string dateIndex, string dateType)
        {
            if (string.IsNullOrEmpty(productLine) || string.IsNullOrWhiteSpace(item) || string.IsNullOrEmpty(dateIndex) || string.IsNullOrWhiteSpace(dateType))
            {
                return HttpNotFound();
            }
            else
            {
                ProdLineExInstance prodLineExInstance = this.genericMgr.FindAll<ProdLineExInstance>("select p from ProdLineExInstance as p where p.ProductLine=? and p.Item=? and p.DateIndex=? and  p.DateType=?", new object[] { productLine, item, dateIndex, dateType })[0];

                return View(prodLineExInstance);
            }
        }


        [SconitAuthorize(Permissions = "Url_MRP_ProdLineExInstance_Edit")]
        public ActionResult InstanceEdit(ProdLineExInstance prodLineExInstance)
        {
            if (ModelState.IsValid)
            {
                prodLineExInstance.Region = genericMgr.FindById<FlowMaster>(prodLineExInstance.ProductLine).PartyFrom;
                this.genericMgr.UpdateWithTrim(prodLineExInstance);
                SaveSuccessMessage(Resources.MRP.ProdLineExInstance.ProdLineExInstance_Updated);
            }

            return View(prodLineExInstance);
        }


        [SconitAuthorize(Permissions = "Url_MRP_ProdLineEx_View")]
        public ActionResult InstanceNew()
        {
            ViewBag.DateType = "0";
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_MRP_ProdLineExInstance_Edit")]
        public ActionResult InstanceNew(ProdLineExInstance prodLineExInstance)
        {
            try
            {
                if (prodLineExInstance.DateType == com.Sconit.CodeMaster.TimeUnit.Day)
                {
                    ModelState.Remove("DateIndex");
                }
                if (ModelState.IsValid)
                {

                    if (prodLineExInstance.DateType == com.Sconit.CodeMaster.TimeUnit.Day)
                    {
                        prodLineExInstance.DateIndex = Convert.ToDateTime(prodLineExInstance.DateIndex).ToString("yyyy-MM-dd");
                    }
                    if (this.genericMgr.FindAll<long>("select count(*)  from ProdLineExInstance as p where p.ProductLine=? and p.Item=? and p.DateIndex=? and  p.DateType=? ",
                        new object[] { prodLineExInstance.ProductLine, prodLineExInstance.Item, prodLineExInstance.DateIndex, prodLineExInstance.DateType })[0] > 0)
                    {
                        base.SaveErrorMessage(string.Format(Resources.EXT.ControllerLan.Con_AlreadyExistsSameFlowItemCodePlanTypeTypeTimeIndex, prodLineExInstance.ProductLine, prodLineExInstance.Item, prodLineExInstance.DateType, prodLineExInstance.DateIndex));
                        return View(prodLineExInstance);
                    }
                    prodLineExInstance.Region = genericMgr.FindById<FlowMaster>(prodLineExInstance.ProductLine).PartyFrom;
                    prodLineExInstance.IsManualCreate = true;
                    this.genericMgr.CreateWithTrim(prodLineExInstance);
                    SaveSuccessMessage(Resources.MRP.ProdLineExInstance.ProdLineExInstance_Added);
                    string productLine = prodLineExInstance.ProductLine;
                    string item = prodLineExInstance.Item;
                    string dateIndex = prodLineExInstance.DateIndex;
                    int dateType = (int)prodLineExInstance.DateType;
                    return new RedirectToRouteResult(new RouteValueDictionary { { "action", "InstanceEdit" }, { "controller", "ProdLineEx" },
                     { "productLine", productLine },{"item",item}, { "dateIndex", dateIndex },{"dateType",dateType}});
                }
            }
            catch (Exception e)
            {
                if (e is CommitResourceException)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_AlreadyExitsSameData);
                }

            }

            return View(prodLineExInstance);
        }

        [SconitAuthorize(Permissions = "Url_MRP_ProdLineExInstance_Edit")]
        public ActionResult InstanceDelete(string productLine, string item, string dateIndex, com.Sconit.CodeMaster.TimeUnit dateType)
        {
            if (string.IsNullOrEmpty(productLine) || string.IsNullOrWhiteSpace(item) || string.IsNullOrEmpty(dateIndex))
            {
                return HttpNotFound();
            }
            else
            {
                ProdLineExInstance prodLineExInstance = this.genericMgr.FindAll<ProdLineExInstance>("  from ProdLineExInstance as p where p.ProductLine=? and p.Item=? and p.DateIndex=? and  p.DateType=?",
                         new object[] { productLine, item, dateIndex, dateType })[0];
                this.genericMgr.Delete(prodLineExInstance);
                SaveSuccessMessage(Resources.MRP.ProdLineExInstance.ProdLineExInstance_Deleted);
                return RedirectToAction("InstanceList");
                // return new RedirectToRouteResult(new RouteValueDictionary { { "action", "Edit" }, { "controller", "ProdLineEx" }, { "id", itemEx.Code } });
            }
        }

        #endregion
        #endregion

        private SearchStatementModel PrepareInstanceSearchStatement(GridCommand command, ProdLineExInstanceSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            if (searchModel.DateType != null)
            {
                if (searchModel.DateType.Value == 4)
                {
                    if (searchModel.DateIndexDate != null & searchModel.DateIndexToDate != null)
                    {
                        HqlStatementHelper.AddBetweenStatement("DateIndex", searchModel.DateIndexDate.Value.ToString("yyyy-MM-dd"), searchModel.DateIndexToDate.Value.ToString("yyyy-MM-dd"), "p", ref whereStatement, param);
                    }
                    else if (searchModel.DateIndexDate != null & searchModel.DateIndexToDate == null)
                    {
                        HqlStatementHelper.AddGeStatement("DateIndex", searchModel.DateIndexDate.Value.ToString("yyyy-MM-dd"), "p", ref whereStatement, param);
                    }
                    else if (searchModel.DateIndexDate == null & searchModel.DateIndexToDate != null)
                    {
                        HqlStatementHelper.AddLeStatement("DateIndex", searchModel.DateIndexToDate.Value.ToString("yyyy-MM-dd"), "p", ref whereStatement, param);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(searchModel.DateIndex) & !string.IsNullOrEmpty(searchModel.DateIndexTo))
                    {
                        HqlStatementHelper.AddBetweenStatement("DateIndex", searchModel.DateIndex, searchModel.DateIndexTo, "p", ref whereStatement, param);
                    }
                    else if (!string.IsNullOrEmpty(searchModel.DateIndex) & string.IsNullOrEmpty(searchModel.DateIndexTo))
                    {
                        HqlStatementHelper.AddGeStatement("DateIndex", searchModel.DateIndex, "p", ref whereStatement, param);
                    }
                    else if (string.IsNullOrEmpty(searchModel.DateIndex) & !string.IsNullOrEmpty(searchModel.DateIndexTo))
                    {
                        HqlStatementHelper.AddLeStatement("DateIndex", searchModel.DateIndexTo, "p", ref whereStatement, param);
                    }
                }
            }

            HqlStatementHelper.AddEqStatement("ProductLine", searchModel.ProductLine, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ShiftType", searchModel.ShiftType, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ApsPriority", searchModel.ApsPriority, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("DateType", searchModel.DateType, "p", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by p.DateType,p.ProductLine,p.DateIndex,p.Item,p.CreateDate desc";
            }

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectInstanceCountStatement;
            searchStatementModel.SelectStatement = selectInstanceStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }


        #region
        [SconitAuthorize(Permissions = "Url_MRP_ProdLineExInstanceView_View")]
        public ActionResult InstanceViewIndex()
        {
            ViewBag.DateIndex = com.Sconit.Utility.DateTimeHelper.GetWeekOfYear(DateTime.Now);
            return View();
        }

        [SconitAuthorize(Permissions = "Url_MRP_ProdLineExInstanceView_View")]
        public ActionResult _ExportProdLineExInstanceView(DateTime startTime, string dateIndex, com.Sconit.CodeMaster.TimeUnit dateType,
            string productLine, string item, bool isMrpSpeed, bool isRccpSpeed, bool isApsPriority, bool isQuota,
            bool isSwichTime, bool isSpeedTimes, bool isMinLotSize, bool isEconomicLotSize, bool isMaxLotSize,
            bool isTurnQty, bool isCorrection, bool isShiftType)
        {
            int checkboxcheckedCount = 0;
            if (isMrpSpeed)
            {
                checkboxcheckedCount++;
            }
            if (isRccpSpeed)
            {
                checkboxcheckedCount++;
            }
            if (isApsPriority)
            {
                checkboxcheckedCount++;
            }
            if (isQuota)
            {
                checkboxcheckedCount++;
            }
            if (isSwichTime)
            {
                checkboxcheckedCount++;
            }
            if (isSpeedTimes)
            {
                checkboxcheckedCount++;
            }
            if (isMinLotSize)
            {
                checkboxcheckedCount++;
            }
            if (isEconomicLotSize)
            {
                checkboxcheckedCount++;
            }
            if (isMaxLotSize)
            {
                checkboxcheckedCount++;
            }
            if (isTurnQty)
            {
                checkboxcheckedCount++;
            }
            if (isCorrection)
            {
                checkboxcheckedCount++;
            }
            if (isShiftType)
            {
                checkboxcheckedCount++;
            }
            string html = _GetProdLineExInstanceView(startTime, dateIndex, dateType,
             productLine, item, isMrpSpeed, isRccpSpeed, isApsPriority, isQuota,
             isSwichTime, isSpeedTimes, isMinLotSize, isEconomicLotSize, isMaxLotSize,
             isTurnQty, isCorrection, isShiftType, checkboxcheckedCount);
            // if your view don't have any model then you can pass as null

            return new DownloadFileActionResult(html, "ProdLineEx.xls");
        }

        [SconitAuthorize(Permissions = "Url_MRP_ProdLineExInstanceView_View")]
        public string _GetProdLineExInstanceView(DateTime startTime, string dateIndex, com.Sconit.CodeMaster.TimeUnit dateType,
            string productLine, string item, bool isMrpSpeed, bool isRccpSpeed, bool isApsPriority, bool isQuota,
            bool isSwichTime, bool isSpeedTimes, bool isMinLotSize, bool isEconomicLotSize, bool isMaxLotSize,
            bool isTurnQty, bool isCorrection, bool isShiftType, int checkboxcheckedCount)
        {
            string startDateIndex = string.Empty;
            string endDateIndex = string.Empty;
            if (dateType == com.Sconit.CodeMaster.TimeUnit.Day)
            {
                startDateIndex = startTime.ToString("yyyy-MM-dd");
                endDateIndex = startTime.AddDays(14).ToString("yyyy-MM-dd");
            }
            else if (dateType == com.Sconit.CodeMaster.TimeUnit.Week)
            {
                startDateIndex = dateIndex;
                endDateIndex = Utility.DateTimeHelper.GetWeekOfYear((Utility.DateTimeHelper.GetWeekIndexDateFrom(dateIndex).AddDays(7 * 16)));
            }
            else if (dateType == com.Sconit.CodeMaster.TimeUnit.Month)
            {
                startDateIndex = dateIndex;
                endDateIndex = DateTime.Parse(dateIndex + "-01").AddMonths(12).ToString("yyyy-MM");
            }

            IList<object> param = new List<object>();
            string hql = "  from ProdLineExInstance as p where p.DateType=? and  DateIndex between ? and ? ";
            param.Add(dateType);
            param.Add(startDateIndex);
            param.Add(endDateIndex);
            if (!string.IsNullOrEmpty(item))
            {
                hql += " and p.Item=?";
                param.Add(item);
            }
            if (!string.IsNullOrEmpty(productLine))
            {
                hql += " and p.ProductLine=?";
                param.Add(productLine);
            }
            IList<ProdLineExInstance> prodLineExInstanceList = genericMgr.FindAll<ProdLineExInstance>(hql, param.ToArray());
            foreach (var prodLineExInstance in prodLineExInstanceList)
            {
                prodLineExInstance.ApsPriorityDescription = (int)prodLineExInstance.ApsPriority + "[" + systemMgr.GetCodeDetailDescription(CodeMaster.CodeMaster.ApsPriorityType, (int)prodLineExInstance.ApsPriority) + "]";
                prodLineExInstance.ShiftTypeDescription = (int)prodLineExInstance.ShiftType + "[" + systemMgr.GetCodeDetailDescription(CodeMaster.CodeMaster.ShiftType, (int)prodLineExInstance.ShiftType) + "]";
            }

            return GetStringProdLineExInstance(prodLineExInstanceList, dateType, startTime, dateIndex, isMrpSpeed, isRccpSpeed, isApsPriority, isQuota, isSwichTime, isSpeedTimes, isMinLotSize,
             isEconomicLotSize, isMaxLotSize, isTurnQty, isCorrection, isShiftType, checkboxcheckedCount);
        }

        private string GetStringProdLineExInstance(IList<ProdLineExInstance> prodLineExInstanceList, com.Sconit.CodeMaster.TimeUnit DateType,
            DateTime startTime, string dataIndex, bool isMrpSpeed, bool isRccpSpeed, bool isApsPriority, bool isQuota, bool isSwichTime, bool isSpeedTimes, bool isMinLotSize,
            bool isEconomicLotSize, bool isMaxLotSize, bool isTurnQty, bool isCorrection, bool isShiftType, int checkboxcheckedCount)
        {
            if (prodLineExInstanceList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            #region Head
            var itemList = this.genericMgr.FindAllIn<Item>(" from Item where Code in(? ",
                prodLineExInstanceList.Select(p => p.Item).Distinct());
            str.Append("<th >");
            str.Append(Resources.MRP.ProdLineExInstance.ProdLineExInstance_Item);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.MRP.ProdLineExInstance.ProdLineExInstance_Type);
            str.Append("</th>");

            if (DateType == com.Sconit.CodeMaster.TimeUnit.Day)
            {
                //14天
                DateTime dt = startTime;
                for (int i = 0; i < 14; i++)
                {
                    str.Append("<th>");
                    str.Append(dt.ToString("MM-dd"));
                    str.Append("</th>");
                    dt = dt.AddDays(1);
                }
            }
            else if (DateType == com.Sconit.CodeMaster.TimeUnit.Month)
            {
                //12月
                string[] strArray = dataIndex.Split('-');
                int years = Convert.ToInt32(strArray[0]);
                int month = Convert.ToInt32(strArray[1]);
                if (month == 12)
                {
                    month = 1;
                    ++years;
                }
                for (int i = 0; i < 12; i++)
                {
                    str.Append("<th >");
                    str.Append(years + "-" + month.ToString("D2"));
                    str.Append("</th>");
                    if (month == 12)
                    {
                        month = 0;
                        ++years;
                    }
                    month++;
                }
            }
            else
            {
                //16周
                string[] wky = dataIndex.Split('-');
                int weekIndex = int.Parse(wky[1]);
                int yearIndex = int.Parse(wky[0]);
                string newWeekOfyear = string.Empty;
                for (int i = 0; i < 16; i++)
                {
                    if (weekIndex <= 0)
                    {
                        newWeekOfyear = (yearIndex - 1).ToString();
                        newWeekOfyear += "-" + (52 + weekIndex).ToString("D2");
                    }
                    else if (weekIndex > 52)
                    {
                        newWeekOfyear = (yearIndex + 1).ToString();
                        newWeekOfyear += "-" + (weekIndex - 52).ToString("D2");
                    }
                    else
                    {
                        newWeekOfyear = yearIndex.ToString();
                        newWeekOfyear += "-" + weekIndex.ToString("D2");
                    }
                    str.Append("<th>");
                    str.Append(newWeekOfyear);
                    str.Append("</th>");
                    weekIndex++;
                }
            }

            #endregion

            str.Append("</tr></thead><tbody>");

            if (prodLineExInstanceList != null && prodLineExInstanceList.Count > 0)
            {
                var prodLineExInstanceListGruopby = from mi in prodLineExInstanceList
                                                    group mi by
                                                    new
                                                    {
                                                        ProductLine = mi.ProductLine,
                                                        Item = mi.Item
                                                    } into g
                                                    select new
                                                    {
                                                        ProductLine = g.Key.ProductLine,
                                                        Item = g.Key.Item,
                                                        List = g,
                                                    };
                int l = 0;
                foreach (var prodLineExInstanceGroup in prodLineExInstanceListGruopby)
                {
                    l++;
                    #region  一个多少行
                    for (int k = 0; k < 13; k++)
                    {
                        #region 开始tr有没有
                        if (l % 2 == 0)
                        {
                            if (k == 0)
                            {
                                if (isMrpSpeed)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }
                            else if (k == 1)
                            {
                                if (isRccpSpeed)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }
                            else if (k == 2)
                            {
                                if (isApsPriority)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }
                            else if (k == 3)
                            {

                            }
                            else if (k == 4)
                            {
                                if (isQuota)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }
                            else if (k == 5)
                            {
                                if (isSwichTime)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }
                            else if (k == 6)
                            {
                                if (isSpeedTimes)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }
                            else if (k == 7)
                            {
                                if (isMinLotSize)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }
                            else if (k == 8)
                            {
                                if (isEconomicLotSize)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }
                            else if (k == 9)
                            {
                                if (isMaxLotSize)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }
                            else if (k == 10)
                            {
                                if (isTurnQty)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }
                            else if (k == 11)
                            {
                                if (isCorrection)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }
                            else if (k == 12)
                            {
                                if (isShiftType)
                                {
                                    str.Append("<tr class=\"t-alt\">");
                                }
                            }

                        }
                        else
                        {
                            if (k == 0)
                            {
                                if (isMrpSpeed)
                                {
                                    str.Append("<tr>");
                                }
                            }
                            else if (k == 1)
                            {
                                if (isRccpSpeed)
                                {
                                    str.Append("<tr>");
                                }
                            }
                            else if (k == 2)
                            {
                                if (isApsPriority)
                                {
                                    str.Append("<tr>");
                                }
                            }
                            else if (k == 3)
                            {

                            }
                            else if (k == 4)
                            {
                                if (isQuota)
                                {
                                    str.Append("<tr>");
                                }
                            }
                            else if (k == 5)
                            {
                                if (isSwichTime)
                                {
                                    str.Append("<tr>");
                                }
                            }
                            else if (k == 6)
                            {
                                if (isSpeedTimes)
                                {
                                    str.Append("<tr>");
                                }
                            }
                            else if (k == 7)
                            {
                                if (isMinLotSize)
                                {
                                    str.Append("<tr>");
                                }
                            }
                            else if (k == 8)
                            {
                                if (isEconomicLotSize)
                                {
                                    str.Append("<tr>");
                                }
                            }
                            else if (k == 9)
                            {
                                if (isMaxLotSize)
                                {
                                    str.Append("<tr>");
                                }
                            }
                            else if (k == 10)
                            {
                                if (isTurnQty)
                                {
                                    str.Append("<tr>");
                                }
                            }
                            else if (k == 11)
                            {
                                if (isCorrection)
                                {
                                    str.Append("<tr>");
                                }
                            }
                            else if (k == 12)
                            {
                                if (isShiftType)
                                {
                                    str.Append("<tr>");
                                }
                            }
                        }
                        #endregion

                        #region 判断是从哪行开始显示
                        if (k == 0)
                        {
                            if (isMrpSpeed)
                            {
                                var item = itemList.First(i => i.Code == prodLineExInstanceGroup.Item);
                                str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                str.Append(prodLineExInstanceGroup.Item + "<br>" + item.Description);
                                str.Append("</td>");
                                str.Append("<td>");
                                str.Append(Resources.MRP.ProdLineExInstance.ProdLineExInstance_MrpSpeed);
                                str.Append("</td>");
                            }
                        }
                        else if (k == 1)
                        {
                            if (isRccpSpeed)
                            {
                                if (!isMrpSpeed)
                                {
                                    var item = itemList.First(i => i.Code == prodLineExInstanceGroup.Item);
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(prodLineExInstanceGroup.Item + "<br>" + item.Description);
                                }
                                str.Append("<td>");
                                str.Append(Resources.MRP.ProdLineExInstance.ProdLineExInstance_RccpSpeed);
                                str.Append("</td>");
                            }
                        }
                        else if (k == 2)
                        {
                            if (isApsPriority)
                            {
                                if (!isMrpSpeed && !isRccpSpeed)
                                {
                                    var item = itemList.First(i => i.Code == prodLineExInstanceGroup.Item);
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(prodLineExInstanceGroup.Item + "<br>" + item.Description);
                                }
                                str.Append("<td>");
                                str.Append(Resources.MRP.ProdLineExInstance.ProdLineExInstance_ApsPriority);
                                str.Append("</td>");
                            }
                        }
                        else if (k == 3)
                        {

                        }
                        else if (k == 4)
                        {
                            if (isQuota)
                            {
                                if (!isMrpSpeed && !isRccpSpeed && !isApsPriority)
                                {
                                    var item = itemList.First(i => i.Code == prodLineExInstanceGroup.Item);
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(prodLineExInstanceGroup.Item + "<br>" + item.Description);
                                }
                                str.Append("<td>");
                                str.Append(Resources.MRP.ProdLineExInstance.ProdLineExInstance_Quota);
                                str.Append("</td>");
                            }
                        }
                        else if (k == 5)
                        {
                            if (isSwichTime)
                            {
                                if (!isMrpSpeed && !isRccpSpeed && !isApsPriority && !isQuota)
                                {
                                    var item = itemList.First(i => i.Code == prodLineExInstanceGroup.Item);
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(prodLineExInstanceGroup.Item + "<br>" + item.Description);
                                }
                                str.Append("<td>");
                                str.Append(Resources.MRP.ProdLineExInstance.ProdLineExInstance_SwichTime);
                                str.Append("</td>");
                            }
                        }
                        else if (k == 6)
                        {
                            if (isSpeedTimes)
                            {
                                if (!isMrpSpeed && !isRccpSpeed && !isApsPriority && !isQuota && !isSwichTime)
                                {
                                    var item = itemList.First(i => i.Code == prodLineExInstanceGroup.Item);
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(prodLineExInstanceGroup.Item + "<br>" + item.Description);
                                }
                                str.Append("<td>");
                                str.Append(Resources.MRP.ProdLineExInstance.ProdLineExInstance_SpeedTimes);
                                str.Append("</td>");
                            }
                        }
                        else if (k == 7)
                        {
                            if (isMinLotSize)
                            {
                                if (!isMrpSpeed && !isRccpSpeed && !isApsPriority && !isQuota && !isSwichTime && !isSpeedTimes)
                                {
                                    var item = itemList.First(i => i.Code == prodLineExInstanceGroup.Item);
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(prodLineExInstanceGroup.Item + "<br>" + item.Description);
                                }
                                str.Append("<td>");
                                str.Append(Resources.MRP.ProdLineExInstance.ProdLineExInstance_MinLotSize);
                                str.Append("</td>");
                            }
                        }
                        else if (k == 8)
                        {
                            if (isEconomicLotSize)
                            {
                                if (!isMrpSpeed && !isRccpSpeed && !isApsPriority && !isQuota && !isSwichTime && !isSpeedTimes
                                    && !isMinLotSize)
                                {
                                    var item = itemList.First(i => i.Code == prodLineExInstanceGroup.Item);
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(prodLineExInstanceGroup.Item + "<br>" + item.Description);
                                }
                                str.Append("<td>");
                                str.Append(Resources.MRP.ProdLineExInstance.ProdLineExInstance_EconomicLotSize);
                                str.Append("</td>");
                            }
                        }
                        else if (k == 9)
                        {
                            if (isMaxLotSize)
                            {
                                if (!isMrpSpeed && !isRccpSpeed && !isApsPriority && !isQuota && !isSwichTime && !isSpeedTimes
                                    && !isMinLotSize && !isEconomicLotSize)
                                {
                                    var item = itemList.First(i => i.Code == prodLineExInstanceGroup.Item);
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(prodLineExInstanceGroup.Item + "<br>" + item.Description);
                                }
                                str.Append("<td>");
                                str.Append(Resources.MRP.ProdLineExInstance.ProdLineExInstance_MaxLotSize);
                                str.Append("</td>");
                            }
                        }
                        else if (k == 10)
                        {
                            if (isTurnQty)
                            {
                                if (!isMrpSpeed && !isRccpSpeed && !isApsPriority && !isQuota && !isSwichTime && !isSpeedTimes
                                    && !isMinLotSize && !isEconomicLotSize && !isMaxLotSize)
                                {
                                    var item = itemList.First(i => i.Code == prodLineExInstanceGroup.Item);
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(prodLineExInstanceGroup.Item + "<br>" + item.Description);
                                }
                                str.Append("<td>");
                                str.Append(Resources.MRP.ProdLineExInstance.ProdLineExInstance_TurnQty);
                                str.Append("</td>");
                            }
                        }
                        else if (k == 11)
                        {
                            if (isCorrection)
                            {
                                if (!isMrpSpeed && !isRccpSpeed && !isApsPriority && !isQuota && !isSwichTime && !isSpeedTimes
                                    && !isMinLotSize && !isEconomicLotSize && !isMaxLotSize && !isTurnQty)
                                {
                                    var item = itemList.First(i => i.Code == prodLineExInstanceGroup.Item);
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(prodLineExInstanceGroup.Item + "<br>" + item.Description);
                                }
                                str.Append("<td>");
                                str.Append(Resources.MRP.ProdLineExInstance.ProdLineExInstance_Correction);
                                str.Append("</td>");
                            }
                        }
                        else
                        {
                            if (isShiftType)
                            {
                                if (!isMrpSpeed && !isRccpSpeed && !isApsPriority && !isQuota && !isSwichTime && !isSpeedTimes
                                    && !isMinLotSize && !isEconomicLotSize && !isMaxLotSize && !isTurnQty && !isCorrection)
                                {
                                    var item = itemList.First(i => i.Code == prodLineExInstanceGroup.Item);
                                    str.Append("<td  rowspan=\"" + checkboxcheckedCount + "\"  style=\"text-align:center\">");
                                    str.Append(prodLineExInstanceGroup.Item + "<br>" + item.Description);
                                }
                                str.Append("<td>");
                                str.Append(Resources.MRP.ProdLineExInstance.ProdLineExInstance_ShiftType);
                                str.Append("</td>");
                            }
                        }
                        #endregion
                        #region 如果是天
                        if (DateType == com.Sconit.CodeMaster.TimeUnit.Day)
                        {
                            //14天
                            DateTime dt = startTime;
                            for (int j = 0; j < 14; j++)
                            {

                                var prodLineExInstanceFirst = prodLineExInstanceGroup.List.FirstOrDefault(m => m.DateIndex == dt.ToString("yyyy-MM-dd")
                                    & m.Item == prodLineExInstanceGroup.Item & m.ProductLine == prodLineExInstanceGroup.ProductLine);
                                if (k == 0)
                                {
                                    if (isMrpSpeed)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.MrpSpeed);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                else if (k == 1)
                                {
                                    if (isRccpSpeed)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.RccpSpeed);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                else if (k == 2)
                                {
                                    if (isApsPriority)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.ApsPriorityDescription);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append(" ");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                else if (k == 3)
                                {

                                }

                                else if (k == 4)
                                {
                                    if (isQuota)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.Quota);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }


                                else if (k == 5)
                                {
                                    if (isSwichTime)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.SwitchTime);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }
                                else if (k == 6)
                                {
                                    if (isSpeedTimes)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.SpeedTimes);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                else if (k == 7)
                                {
                                    if (isMinLotSize)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.MinLotSize);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }


                                else if (k == 8)
                                {
                                    if (isEconomicLotSize)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.EconomicLotSize);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }


                                else if (k == 9)
                                {
                                    if (isMaxLotSize)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.MaxLotSize);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }


                                else if (k == 10)
                                {
                                    if (isTurnQty)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.TurnQty);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                else if (k == 11)
                                {
                                    if (isCorrection)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.Correction);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                else if (k == 12)
                                {
                                    if (isShiftType)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.ShiftTypeDescription);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                dt = dt.AddDays(1);

                            }

                        }
                        #endregion
                        #region 如果是年月
                        else if (DateType == com.Sconit.CodeMaster.TimeUnit.Month)
                        {
                            //12月
                            string[] strArray = dataIndex.Split('-');
                            int years = Convert.ToInt32(strArray[0]);
                            int month = Convert.ToInt32(strArray[1]);
                            if (month == 12)
                            {
                                month = 1;
                                ++years;
                            }
                            for (int j = 0; j < 12; j++)
                            {
                                string yearsmonthTime = years + "-" + month.ToString("D2");
                                var prodLineExInstanceFirst = prodLineExInstanceGroup.List.FirstOrDefault(m => m.DateIndex == yearsmonthTime & m.Item == prodLineExInstanceGroup.Item
                                    & m.ProductLine == prodLineExInstanceGroup.ProductLine);
                                if (k == 0)
                                {
                                    if (isMrpSpeed)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.MrpSpeed);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                else if (k == 1)
                                {
                                    if (isRccpSpeed)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.RccpSpeed);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                else if (k == 2)
                                {
                                    if (isApsPriority)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.ApsPriorityDescription);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append(" ");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                else if (k == 3)
                                {

                                }

                                else if (k == 4)
                                {
                                    if (isQuota)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.Quota);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }


                                else if (k == 5)
                                {
                                    if (isSwichTime)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.SwitchTime);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }
                                else if (k == 6)
                                {
                                    if (isSpeedTimes)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.SpeedTimes);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                else if (k == 7)
                                {
                                    if (isMinLotSize)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.MinLotSize);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }


                                else if (k == 8)
                                {
                                    if (isEconomicLotSize)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.EconomicLotSize);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }


                                else if (k == 9)
                                {
                                    if (isMaxLotSize)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.MaxLotSize);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }


                                else if (k == 10)
                                {
                                    if (isTurnQty)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.TurnQty);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                else if (k == 11)
                                {
                                    if (isCorrection)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.Correction);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                else if (k == 12)
                                {
                                    if (isShiftType)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.ShiftTypeDescription);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("");
                                            str.Append("</td>");
                                        }
                                    }
                                }
                                if (month == 12)
                                {
                                    month = 0;
                                    ++years;
                                }
                                month++;
                            }
                        }
                        #endregion
                        #region 如果是周
                        else
                        {
                            //16周
                            string[] wky = dataIndex.Split('-');
                            int weekIndex = int.Parse(wky[1]);
                            int yearIndex = int.Parse(wky[0]);
                            string newWeekOfyear = string.Empty;
                            for (int j = 0; j < 16; j++)
                            {
                                if (weekIndex <= 0)
                                {
                                    newWeekOfyear = (yearIndex - 1).ToString();
                                    newWeekOfyear += "-" + (52 + weekIndex).ToString("D2");
                                }
                                else if (weekIndex > 52)
                                {
                                    newWeekOfyear = (yearIndex + 1).ToString();
                                    newWeekOfyear += "-" + (weekIndex - 52).ToString("D2");
                                }
                                else
                                {
                                    newWeekOfyear = yearIndex.ToString();
                                    newWeekOfyear += "-" + weekIndex.ToString("D2");
                                }



                                var prodLineExInstanceFirst = prodLineExInstanceGroup.List.FirstOrDefault(m => m.DateIndex == newWeekOfyear & m.Item == prodLineExInstanceGroup.Item
                                    & m.ProductLine == prodLineExInstanceGroup.ProductLine);
                                if (k == 0)
                                {
                                    if (isMrpSpeed)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.MrpSpeed);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                else if (k == 1)
                                {
                                    if (isRccpSpeed)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.RccpSpeed);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                else if (k == 2)
                                {
                                    if (isApsPriority)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.ApsPriorityDescription);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append(" ");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                else if (k == 3)
                                {

                                }

                                else if (k == 4)
                                {
                                    if (isQuota)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.Quota);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }


                                else if (k == 5)
                                {
                                    if (isSwichTime)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.SwitchTime);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }
                                else if (k == 6)
                                {
                                    if (isSpeedTimes)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.SpeedTimes);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                else if (k == 7)
                                {
                                    if (isMinLotSize)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.MinLotSize);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }


                                else if (k == 8)
                                {
                                    if (isEconomicLotSize)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.EconomicLotSize);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }


                                else if (k == 9)
                                {
                                    if (isMaxLotSize)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.MaxLotSize);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }


                                else if (k == 10)
                                {
                                    if (isTurnQty)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.TurnQty);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                else if (k == 11)
                                {
                                    if (isCorrection)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.Correction);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("0");
                                            str.Append("</td>");
                                        }
                                    }
                                }

                                else if (k == 12)
                                {
                                    if (isShiftType)
                                    {
                                        if (prodLineExInstanceFirst != null)
                                        {
                                            str.Append("<td>");
                                            str.Append(prodLineExInstanceFirst.ShiftTypeDescription);
                                            str.Append("</td>");
                                        }
                                        else
                                        {
                                            str.Append("<td>");
                                            str.Append("");
                                            str.Append("</td>");
                                        }
                                    }
                                }
                                weekIndex++;



                            }
                        }
                        #endregion

                        #region 是否显示此行
                        if (k == 0)
                        {
                            if (isMrpSpeed)
                            {
                                str.Append("</tr>");
                            }
                        }
                        else if (k == 1)
                        {
                            if (isRccpSpeed)
                            {
                                str.Append("</tr>");
                            }
                        }
                        else if (k == 2)
                        {
                            if (isApsPriority)
                            {
                                str.Append("</tr>");
                            }
                        }
                        else if (k == 3)
                        {

                        }
                        else if (k == 4)
                        {
                            if (isQuota)
                            {
                                str.Append("</tr>");
                            }
                        }
                        else if (k == 5)
                        {
                            if (isSwichTime)
                            {
                                str.Append("</tr>");
                            }
                        }
                        else if (k == 6)
                        {
                            if (isSpeedTimes)
                            {
                                str.Append("</tr>");
                            }
                        }
                        else if (k == 7)
                        {
                            if (isMinLotSize)
                            {
                                str.Append("</tr>");
                            }
                        }
                        else if (k == 8)
                        {
                            if (isEconomicLotSize)
                            {
                                str.Append("</tr>");
                            }
                        }
                        else if (k == 9)
                        {
                            if (isMaxLotSize)
                            {
                                str.Append("</tr>");
                            }
                        }
                        else if (k == 10)
                        {
                            if (isTurnQty)
                            {
                                str.Append("</tr>");
                            }
                        }
                        else if (k == 11)
                        {
                            if (isCorrection)
                            {
                                str.Append("</tr>");
                            }
                        }
                        else if (k == 12)
                        {
                            if (isShiftType)
                            {
                                str.Append("</tr>");
                            }
                        }
                        #endregion

                    }
                    #endregion
                }

                //表尾
                str.Append("</tbody>");
                str.Append("</table>");
            }
            return str.ToString();
        }

        #endregion
    }
}
