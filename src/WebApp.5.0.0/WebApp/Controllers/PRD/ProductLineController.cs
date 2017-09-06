/// <summary>
/// Summary description for LocationController
/// </summary>
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
    using com.Sconit.Entity.Exception;
    using com.Sconit.Entity.INV;
    using com.Sconit.Entity.MD;
    using System.Web;

    public class ProductLineController : WebAppBaseController
    {
        #region Properties
        //public IGenericMgr genericMgr { get; set; }
        public IBomMgr bomMgr { get; set; }
        //public IFlowMgr flowMgr { get; set; }
        public IProductionLineMgr productionLineMgr { get; set; }

        #endregion

        #region hql string
        private static string selectCountStatement = "select count(*) from ProductLineLocationDetail as p";
        private static string selectStatement = "select p from ProductLineLocationDetail as p";
        private static string selectOneFlowDetailStatement = "select d from FlowDetail as d where d.Flow = ? and d.Item = ?";
        #endregion

        #region MaterialIn
        [SconitAuthorize(Permissions = "Url_ProductLine_MaterialIn_View")]
        public ActionResult MaterialInIndex()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_ProductLine_MaterialIn_View")]
        public ActionResult ForceMaterialInFeedIndex()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_FeedProductLine")]
        public ActionResult MaterialInFeedIndex()
        {
            return View();
        }


        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_BackFlush")]
        public ActionResult MaterialInBackFlushIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_ProductLine_MaterialIn_View")]
        public ActionResult MaterialInList(GridCommand command, ProductLineLocationDetailSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProductLine_MaterialIn_View")]
        public ActionResult _AjaxMaterialInList(GridCommand command, ProductLineLocationDetailSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<ProductLineLocationDetail>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_FeedProductLine")]
        public ActionResult MaterialInFeedQty()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_ForceFeedProductLine")]
        public ActionResult ForceMaterialInFeedQty()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_FeedProductLine")]
        public ActionResult _FeedQtyDetailList(string flow)
        {
            IList<Uom> uoms = genericMgr.FindAll<Uom>();
            ViewData.Add("uoms", uoms);

            #region 选默认库位的
            FlowMaster flowMaster = this.flowMgr.GetAuthenticFlow(flow);
            if (flowMaster != null)
            {
                ViewBag.Region = flowMaster.PartyFrom;
            }
            #endregion

            ViewBag.Flow = flow;
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_FeedProductLine")]
        public ActionResult _SelectFeedQtyDetailList(string flow)
        {
            IList<BomDetail> flowDetailBomDetailList = new List<BomDetail>();
            FlowMaster flowMaster = this.flowMgr.GetAuthenticFlow(flow);
            if (flowMaster != null)
            {
                flowDetailBomDetailList = bomMgr.GetProductLineWeightAverageBomDetail(flow);
                if (flowDetailBomDetailList != null && flowDetailBomDetailList.Count > 0)
                {
                    foreach (BomDetail bomDetail in flowDetailBomDetailList)
                    {
                        Item item = genericMgr.FindById<Item>(bomDetail.Item);
                        if (item != null)
                        {
                            bomDetail.Item = item.Code;
                            bomDetail.ReferenceItemCode = item.ReferenceCode;
                            bomDetail.ItemDescription = item.Description;
                            bomDetail.UnitCount = item.UnitCount;
                            bomDetail.Uom = item.Uom;
                            bomDetail.MinUnitCount = item.MinUnitCount;
                        }
                    }
                }
            }
            return View(new GridModel(flowDetailBomDetailList == null ? new List<BomDetail>() : flowDetailBomDetailList));
        }

        public ActionResult _WebOrderDetail(string flow, string itemCode)
        {
            if (!string.IsNullOrEmpty(itemCode))
            {

                WebOrderDetail webOrderDetail = new WebOrderDetail();
                Item item = genericMgr.FindById<Item>(itemCode);
                if (item != null)
                {
                    webOrderDetail.Item = item.Code;
                    webOrderDetail.ReferenceItemCode = item.ReferenceCode;
                    webOrderDetail.ItemDescription = item.Description;
                    webOrderDetail.UnitCount = item.UnitCount;
                    webOrderDetail.Uom = item.Uom;
                    webOrderDetail.MinUnitCount = item.MinUnitCount;
                    webOrderDetail.Container = item.Container;
                    webOrderDetail.ManufactureParty = item.ManufactureParty;
                }
                return this.Json(webOrderDetail);
            }
            return null;
        }

        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_BackFlush")]
        public ActionResult _BackFlushDetailList(string productLine, string productLineFacility)
        {

            IList<ProductLineLocationDetail> productLineLocationDetailList = new List<ProductLineLocationDetail>();
            
            FlowMaster flowMaster = flowMgr.GetAuthenticFlow(productLine);
            if (flowMaster == null)
            {
                return PartialView(productLineLocationDetailList);
            }
            else
            {
                string hql = "select p from ProductLineLocationDetail as p where p.IsClose = ? and p.ProductLine = ?";
                IList<object> para = new List<object>();
                para.Add(false);
                para.Add(productLine);
                if (!string.IsNullOrWhiteSpace(productLineFacility))
                {
                    hql += " and p.ProductLineFacility = ?";
                    para.Add(productLineFacility);
                }

                productLineLocationDetailList = genericMgr.FindAll<ProductLineLocationDetail>(hql, para.ToArray());
                var q = from p in productLineLocationDetailList
                        group p by p.Item into g
                        select new ProductLineLocationDetail
                            {
                                Item = g.Key,
                                Qty = g.Sum(p => p.Qty),
                                RemainQty = g.Sum(p => p.RemainBackFlushQty)
                            };
                if (genericMgr != null && q.Count() > 0)
                {
                    productLineLocationDetailList = q.ToList<ProductLineLocationDetail>();
                    foreach (ProductLineLocationDetail pLLDetail in productLineLocationDetailList)
                    {
                        Item item = genericMgr.FindById<Item>(pLLDetail.Item);
                        if (item != null)
                        {
                            pLLDetail.Item = item.Code;
                            pLLDetail.ReferenceItemCode = item.ReferenceCode;
                            pLLDetail.ItemDescription = item.Description;
                            pLLDetail.UnitCount = item.UnitCount;
                            pLLDetail.Uom = item.Uom;
                        }
                    }

                }
                return PartialView(q.ToList<ProductLineLocationDetail>());
            }
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_ForceFeedProductLine")]
        public ActionResult _SelectForceFeedQtyBatchEditing()
        {
            return PartialView(new GridModel(new List<BomDetail>()));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_ForceFeedProductLine")]
        public ActionResult _ForceFeedQtyDetailList(string flow)
        {
            IList<Uom> uoms = genericMgr.FindAll<Uom>();
            ViewData.Add("uoms", uoms);

            #region 选默认库位的
            FlowMaster flowMaster = flowMgr.GetAuthenticFlow(flow);
            if (flowMaster != null)
            {
                ViewBag.Region = flowMaster.PartyFrom;
            }
            #endregion
            return PartialView();
        }

        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_FeedProductLine")]
        public JsonResult FeedQty(ProductLineLocationDetail productLineLocationDetail, [Bind(Prefix =
             "updated")]IEnumerable<BomDetail> updatedBomDetails)
        {
            try
            {

                if (productLineLocationDetail.ProductLine == null || productLineLocationDetail.ProductLine == string.Empty)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ProductionLineCanNotBeEmpty);
                }

                FlowMaster flowMaster = flowMgr.GetAuthenticFlow(productLineLocationDetail.ProductLine);
                if (flowMaster == null)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ProductionLineIsWrongOrLackPermission);
                }

                IList<FeedInput> feedInputList = new List<FeedInput>();

                if (updatedBomDetails != null && updatedBomDetails.Count() > 0)
                {
                    foreach (BomDetail bomDetail in updatedBomDetails)
                    {
                        FeedInput feedInput = new FeedInput();

                        if (!string.IsNullOrEmpty(bomDetail.FeedLocation))
                        {
                            string locationcheckStr = "from Location as l where l.Region = ? and l.Code = ?";
                            Location location = genericMgr.FindAll<Location>(locationcheckStr, new object[] { flowMaster.PartyFrom, bomDetail.FeedLocation }).SingleOrDefault<Location>();
                            if (location == null)
                            {
                                throw new BusinessException(Resources.EXT.ControllerLan.Con_Item + bomDetail.Item + Resources.EXT.ControllerLan.Con_LocationIsWrong);
                            }
                            feedInput.LocationFrom = bomDetail.FeedLocation;
                        }
                        else
                        {
                            feedInput.LocationFrom = flowMaster.LocationFrom;
                        }
                        feedInput.Item = bomDetail.Item;
                        if (string.IsNullOrEmpty(bomDetail.Uom))
                        {
                            Item item = genericMgr.FindById<Item>(bomDetail.Item);
                            feedInput.Uom = item.Uom;
                        }
                        else
                        {
                            feedInput.Uom = bomDetail.Uom;
                        }
                        feedInput.Qty = bomDetail.FeedQty;
                        feedInput.QualityType = CodeMaster.QualityType.Qualified;
                        feedInputList.Add(feedInput);
                    }
                }
                if (feedInputList.Count == 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_BackFlushDetailIsEmpty);
                }

                productionLineMgr.FeedRawMaterial(productLineLocationDetail.ProductLine, productLineLocationDetail.ProductLineFacility, feedInputList);
                object obj = new { SuccessMessage = string.Format(Resources.PRD.ProductLineLocationDetail.ProductLineLocationDetail_FeedIn) };
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

        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_ForceFeedProductLine")]
        public JsonResult ForceFeedQty(ProductLineLocationDetail productLineLocationDetail, [Bind(Prefix =
             "inserted")]IEnumerable<BomDetail> insertedBomDetails, [Bind(Prefix =
             "updated")]IEnumerable<BomDetail> updatedBomDetails)
        {
            try
            {
                if (string.IsNullOrEmpty(productLineLocationDetail.ProductLine))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ProductionLineCanNotBeEmpty);
                }
                FlowMaster flowMaster = genericMgr.FindById<FlowMaster>(productLineLocationDetail.ProductLine);
                List<BomDetail> bomDetailList = new List<BomDetail>();

                if (insertedBomDetails != null && insertedBomDetails.Count() > 0)
                {
                    bomDetailList.AddRange(insertedBomDetails);

                }
                if (updatedBomDetails != null && updatedBomDetails.Count() > 0)
                {
                    bomDetailList.AddRange(updatedBomDetails);

                }
                IList<FeedInput> feedInputList = new List<FeedInput>();
                if (bomDetailList != null && bomDetailList.Count() > 0)
                {
                    foreach (BomDetail bomDetail in bomDetailList)
                    {
                        FeedInput feedInput = new FeedInput();
                        if (!string.IsNullOrEmpty(bomDetail.FeedLocation))
                        {
                            string locationcheckStr = "from Location as l where l.Region = ? and l.Code = ?";
                            Location location = genericMgr.FindAll<Location>(locationcheckStr, new object[] { flowMaster.PartyFrom, bomDetail.FeedLocation }).SingleOrDefault<Location>();
                            if (location == null)
                            {
                                throw new BusinessException(Resources.EXT.ControllerLan.Con_Item + bomDetail.Item + Resources.EXT.ControllerLan.Con_LocationIsWrong);
                            }
                            feedInput.LocationFrom = bomDetail.FeedLocation;
                        }
                        else
                        {
                            feedInput.LocationFrom = flowMaster.LocationFrom;
                        }

                        feedInput.Item = bomDetail.Item;
                        if (string.IsNullOrEmpty(bomDetail.Uom))
                        {
                            Item item = genericMgr.FindById<Item>(bomDetail.Item);
                            feedInput.Uom = item.Uom;
                        }
                        else
                        {
                            feedInput.Uom = bomDetail.Uom;
                        }
                        feedInput.Qty = bomDetail.FeedQty;
                        feedInput.QualityType = CodeMaster.QualityType.Qualified;
                        feedInputList.Add(feedInput);
                    }
                }
                if (feedInputList.Count == 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_BackFlushDetailIsEmpty);
                }

                productionLineMgr.FeedRawMaterial(productLineLocationDetail.ProductLine, productLineLocationDetail.ProductLineFacility, feedInputList, true);
                object obj = new { SuccessMessage = string.Format(Resources.PRD.ProductLineLocationDetail.ProductLineLocationDetail_FeedIn), SuccessData = productLineLocationDetail.ProductLine };
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

        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_BackFlush")]
        public JsonResult BackFlush(ProductLineLocationDetail productLineLocationDetail, string itemStr, string qtyStr)
        {
            try
            {

                if (string.IsNullOrEmpty(itemStr))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_DetailCanNotBeEmpty);
                }

                string[] itemArr = itemStr.Split(',');
                string[] qtyArr = qtyStr.Split(',');

                IList<WeightAverageBackflushInput> backFlushInputList = new List<WeightAverageBackflushInput>();
                for (int i = 0; i < itemArr.Count(); i++)
                {

                    WeightAverageBackflushInput input = new WeightAverageBackflushInput();
                    input.Item = itemArr[i];
                    input.Qty = Convert.ToDecimal(qtyArr[i]);

                    backFlushInputList.Add(input);
                }
                productionLineMgr.BackflushWeightAverage(productLineLocationDetail.ProductLine, productLineLocationDetail.ProductLineFacility, backFlushInputList);
                object obj = new { SuccessMessage = string.Format(Resources.PRD.ProductLineLocationDetail.ProductLineLocationDetail_BackFlushed) };
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

        #region return
        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_ReturnProductLine")]
        public ActionResult MaterialInReturnIndex()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_ReturnProductLine")]
        public ActionResult _ReturnQtyDetailList(string flow)
        {

            IList<Uom> uoms = genericMgr.FindAll<Uom>();
            ViewData.Add("uoms", uoms);

            ViewBag.Flow = flow;

            #region 选默认库位的
            FlowMaster flowMaster = flowMgr.GetAuthenticFlow(flow);
            if (flowMaster != null)
            {
                ViewBag.Region = flowMaster.PartyFrom;
            }
            #endregion

            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_ReturnProductLine")]
        public ActionResult _SelectReturnQtyDetailList()
        {
            return PartialView(new GridModel(new List<BomDetail>()));
        }

        [SconitAuthorize(Permissions = "Url_Production_MaterialIn_ReturnProductLine")]
        public JsonResult ReturnQty(ProductLineLocationDetail productLineLocationDetail, [Bind(Prefix =
             "inserted")]IEnumerable<BomDetail> insertedBomDetails, [Bind(Prefix =
             "updated")]IEnumerable<BomDetail> updatedBomDetails)
        {
            try
            {
                if (productLineLocationDetail.ProductLine == null || productLineLocationDetail.ProductLine == string.Empty)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ProductionLineCanNotBeEmpty);
                }

                FlowMaster flowMaster = flowMgr.GetAuthenticFlow(productLineLocationDetail.ProductLine);
                if (flowMaster == null)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ProductionLineIsWrongOrLackPermission);
                }

                IList<ReturnInput> returnInputList = new List<ReturnInput>();
                List<BomDetail> bomDetailList = new List<BomDetail>();

                if (insertedBomDetails != null && insertedBomDetails.Count() > 0)
                {
                    bomDetailList.AddRange(insertedBomDetails);

                }
                if (updatedBomDetails != null && updatedBomDetails.Count() > 0)
                {
                    bomDetailList.AddRange(updatedBomDetails);

                }
                if (bomDetailList != null && bomDetailList.Count() > 0)
                {
                    foreach (BomDetail bomDetail in bomDetailList)
                    {
                        ReturnInput returnInput = new ReturnInput();

                        if (!string.IsNullOrEmpty(bomDetail.FeedLocation))
                        {
                            string locationcheckStr = "from Location as l where l.Region = ? and l.Code = ?";
                            Location location = genericMgr.FindAll<Location>(locationcheckStr, new object[] { flowMaster.PartyFrom, bomDetail.FeedLocation }).SingleOrDefault<Location>();
                            if (location == null)
                            {
                                throw new BusinessException(Resources.EXT.ControllerLan.Con_Item + bomDetail.Item + Resources.EXT.ControllerLan.Con_LocationIsWrong);
                            }
                            returnInput.LocationTo = bomDetail.FeedLocation;
                        }
                        else
                        {
                            returnInput.LocationTo = flowMaster.LocationTo;
                        }
                        returnInput.Item = bomDetail.Item;
                        if (string.IsNullOrEmpty(bomDetail.Uom))
                        {
                            Item item = genericMgr.FindById<Item>(bomDetail.Item);
                            returnInput.Uom = item.Uom;
                        }
                        else
                        {
                            returnInput.Uom = bomDetail.Uom;
                        }
                        returnInput.Qty = bomDetail.FeedQty;
                        returnInput.QualityType = CodeMaster.QualityType.Qualified;
                        returnInputList.Add(returnInput);
                    }
                }
                if (returnInputList.Count == 0)
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ReturnMaterialDetailBeEmpty);
                }

                productionLineMgr.ReturnRawMaterial(productLineLocationDetail.ProductLine, productLineLocationDetail.ProductLineFacility, returnInputList);
                object obj = new { SuccessMessage = string.Format(Resources.PRD.ProductLineLocationDetail.ProductLineLocationDetail_ReturnOut) };
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

        private SearchStatementModel PrepareSearchStatement(GridCommand command, ProductLineLocationDetailSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddEqStatement("ProductLine", searchModel.ProductLine, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("ProductLineFacility", searchModel.ProductLineFacility, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("CreateUserName", searchModel.CreateUserName, "p", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("OrderNo", searchModel.OrderNo, "p", ref whereStatement, param);

            SecurityHelper.AddProductLinePermissionStatement(ref whereStatement, "p", "ProductLine");

            if (!searchModel.IncludeClose)
            {
                HqlStatementHelper.AddEqStatement("IsClose", searchModel.IncludeClose, "p", ref whereStatement, param);
            }
            if (searchModel.StartDate != null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.StartDate, "p", ref whereStatement, param);
            }
            if (searchModel.EndDate != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.EndDate, "p", ref whereStatement, param);
            }
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by p.CreateDate desc";
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        /// <summary>
        /// 生产线强制投料
        /// </summary>
        /// <param name="attachments"></param>
        /// <param name="ProductLine"></param>
        /// <param name="ProductLineFacility"></param>
        /// <returns></returns>
        [SconitAuthorize(Permissions = "Url_ProductLine_MaterialIn_View")]
        public ActionResult ImportForceProductLineDetail(IEnumerable<HttpPostedFileBase> attachments, string ProductLine, string ProductLineFacility)
        {
            try
            {
                if (string.IsNullOrEmpty(ProductLine))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_ProductionLineCanNotBeEmpty);
                }

                foreach (var file in attachments)
                {
                    productionLineMgr.FeedRawMaterialFromXls(file.InputStream, ProductLine, string.IsNullOrEmpty(ProductLineFacility) ? null : ProductLineFacility, true, DateTime.Now);
                    object obj = Resources.EXT.ControllerLan.Con_BackFlushDetailInputSuccessfully;
                    return Json(new { status = obj }, "text/plain");
                }
            }
            catch (BusinessException ex)
            {
                Response.Write(ex.GetMessages()[0].GetMessageString());
            }
            return Content("");

        }
      
    }
}
