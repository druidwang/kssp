/// <summary>
/// Summary description for AddressController
/// </summary>
namespace com.Sconit.Web.Controllers.MRP
{
    #region reference
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.MD;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using com.Sconit.Entity.MRP.MD;
    using System;
    using com.Sconit.Entity.MRP.TRANS;
    using com.Sconit.Entity.PRD;
    #endregion

    /// <summary>
    /// This controller response to control the Currency.
    /// </summary>
    public class MrpExPlanItemRateController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the MrpExPlanItemRate security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        #endregion

        /// <summary>
        /// hql to get count of the MrpExPlanItemRate 
        /// </summary>
        private static string selectCountStatement = "select count(distinct Item) from ProdLineEx as i";

        /// <summary>
        /// hql to get all of the MrpExPlanItemRate
        /// </summary>
        private static string selectStatement = "select distinct Item from ProdLineEx as i";

        /// <summary>
        /// hql to get count of the MrpExPlanItemRate by MrpExPlanItemRate's code
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from ProdLineEx as i where i.Code = ?";

        /// <summary>
        /// Index action for MrpExPlanItemRate controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_MrpExPlanItemRate_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">MrpExPlanItemRate Search model</param>
        /// <returns>return the result view</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_MrpExPlanItemRate_View")]
        public ActionResult List(GridCommand command, MrpExPlanItemRateSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        /// AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">MrpExPlanItemRate Search Model</param>
        /// <returns>return the result Model</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_MrpExPlanItemRate_View")]
        public ActionResult _AjaxList(GridCommand command, MrpExPlanItemRateSearchModel searchModel)
        {
            var prodLineExList = this.genericMgr.FindAll<ProdLineEx>();
            var sections = prodLineExList.GroupBy(p => p.Item)
                .Select(p =>
                {
                    var q = p.First();
                    q.ItemDesc = itemMgr.GetCacheItem(q.Item).Description;
                    return q;
                })
                .Where(p => string.IsNullOrWhiteSpace(searchModel.Section) || p.Item.Contains(searchModel.Section))
                .Where(p => string.IsNullOrWhiteSpace(searchModel.SectionDesc) || p.ItemDesc.Contains(searchModel.SectionDesc))
                .OrderBy(p => p.Item);

            GridModel<ProdLineEx> groupModel = new GridModel<ProdLineEx>();
            groupModel.Total = sections.Count();
            groupModel.Data = sections.Skip((command.Page - 1) * command.PageSize).Take(command.PageSize);
            ViewBag.Total = groupModel.Total;
            return PartialView(groupModel);
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult _SelectSectionDet(string section)
        {
            IList<MrpExPlanItemRate> MrpExPlanItemRates = this.genericMgr.FindAll<MrpExPlanItemRate>(" from MrpExPlanItemRate as i where i.Section=? ", section);

            IList<BomDetail> SectionItemLists = this.genericMgr.FindAll<BomDetail>(" from BomDetail as i where i.Item=? ", section);
            IList<SectionItems> groupProdLineExList = (from l in SectionItemLists
                                                       group l by new { l.Bom, l.Item }
                                                           into g
                                                           select new SectionItems
                                                           {
                                                               Section = g.Key.Item,
                                                               Item = g.Key.Bom
                                                           }).ToList();
            var quey = from c in MrpExPlanItemRates where !(from o in groupProdLineExList select o.Item).Contains(c.Item) select c;
            if (quey.ToList().Count() > 0)
            {
                foreach (var MrpExPlanItemRate in quey)
                {
                    this.genericMgr.DeleteById<MrpExPlanItemRate>(MrpExPlanItemRate.Id);
                    MrpExPlanItemRate.IsDeleted = true;
                }
            }
            var quey1 = from c in groupProdLineExList where !(from o in MrpExPlanItemRates select o.Item).Contains(c.Item) select c;
            if (quey1.ToList().Count() > 0)
            {
                foreach (var groupProdLineEx in quey1)
                {
                    MrpExPlanItemRate MrpExPlanItemRateNew = new MrpExPlanItemRate();
                    MrpExPlanItemRateNew.Item = groupProdLineEx.Item;
                    MrpExPlanItemRateNew.Section = groupProdLineEx.Section;
                    MrpExPlanItemRateNew.ItemRate = 0;
                    //this.genericMgr.CreateWithTrim(MrpExPlanItemRateNew);
                    MrpExPlanItemRates.Add(MrpExPlanItemRateNew);
                }
            }
            foreach (var MrpExPlanItemRate in MrpExPlanItemRates)
            {
                MrpExPlanItemRate.SectionDesc = itemMgr.GetCacheItem(MrpExPlanItemRate.Section).Description;
                MrpExPlanItemRate.ItemDesc = itemMgr.GetCacheItem(MrpExPlanItemRate.Item).Description;
            }
            return View(new GridModel(MrpExPlanItemRates.Where(o => o.IsDeleted == false).OrderBy(p => p.Item)));
        }
        [SconitAuthorize(Permissions = "Url_MrpExPlanItemRate_Edit")]
        public ActionResult _SectionList(string section)
        {
            @ViewBag.Section = section;
            return PartialView();
        }

        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="id">MrpExPlanItemRate id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_MrpExPlanItemRate_View")]
        public ActionResult Edit(string section)
        {
            if (string.IsNullOrEmpty(section))
            {
                return HttpNotFound();
            }
            else
            {
                MrpExPlanItemRate MrpExPlanItemRate = new MrpExPlanItemRate();
                MrpExPlanItemRate.Section = section;
                MrpExPlanItemRate.SectionDesc = itemMgr.GetCacheItem(section).Description;
                @ViewBag.Section = section;
                return View(MrpExPlanItemRate);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_MrpExPlanItemRate_Edit")]
        public JsonResult SaveItemRates(string section, decimal[] ItemRates, string[] Items)
        {
            try
            {
                if (Items == null || Items.Length == 0)
                {
                    SaveErrorMessage(Resources.EXT.ControllerLan.Con_LackDetailCanNotUpdatedRate);
                    return Json(null);
                }
                IList<MrpExPlanItemRate> MrpExPlanItemRates = this.genericMgr.FindAll<MrpExPlanItemRate>(" from MrpExPlanItemRate as i where i.Section=? ", section);

                IList<BomDetail> SectionItemLists = this.genericMgr.FindAll<BomDetail>(" from BomDetail as i where i.Item=? ", section);
                IList<SectionItems> groupProdLineExList = (from l in SectionItemLists
                                                           group l by new { l.Bom, l.Item }
                                                               into g
                                                               select new SectionItems
                                                               {
                                                                   Section = g.Key.Item,
                                                                   Item = g.Key.Bom
                                                               }).ToList();
                var quey = from c in groupProdLineExList where !(from o in MrpExPlanItemRates select o.Item).Contains(c.Item) select c;
                if (quey.ToList().Count() > 0)
                {
                    foreach (var groupProdLineEx in quey)
                    {
                        MrpExPlanItemRate MrpExPlanItemRateNew = new MrpExPlanItemRate();
                        MrpExPlanItemRateNew.Item = groupProdLineEx.Item;
                        MrpExPlanItemRateNew.Section = groupProdLineEx.Section;
                        MrpExPlanItemRateNew.ItemRate = 0;
                        MrpExPlanItemRateNew.IsNew = true;

                        MrpExPlanItemRates.Add(MrpExPlanItemRateNew);
                    }
                }

                for (int i = 0; i < Items.Length; i++)
                {
                    MrpExPlanItemRate MrpExPlanItemRate = MrpExPlanItemRates.Where(o => o.Item == Items[i]).FirstOrDefault() ?? new MrpExPlanItemRate();
                    if (!string.IsNullOrWhiteSpace(MrpExPlanItemRate.Item))
                    {
                        MrpExPlanItemRate.ItemRate = ItemRates[i];
                    }
                    if (ModelState.IsValid)
                    {
                        if (!MrpExPlanItemRate.IsNew)
                        {
                            this.genericMgr.UpdateWithTrim(MrpExPlanItemRate);
                            //SaveSuccessMessage(MrpExPlanItemRate.Item+"," +Resources.MRP.MrpExPlanItemRate.MrpExPlanItemRate_Updated);
                        }
                        else
                        {
                            this.genericMgr.CreateWithTrim(MrpExPlanItemRate);
                        }

                    }
                }
                //object obj = new { newOrder.OrderNo };
                return Json(new { SuccessMessages = Resources.EXT.ControllerLan.Con_RateEditSuccessfully });
            }
            catch (Exception ex)
            {
                SaveErrorMessage(ex);
                return Json(null);
            }
        }
        /// <summary>
        /// Edit action
        /// </summary>
        /// <param name="MrpExPlanItemRate">MrpExPlanItemRate model</param>
        /// <returns>return the result view</returns>
        [SconitAuthorize(Permissions = "Url_MrpExPlanItemRate_Edit")]
        public ActionResult Edit(MrpExPlanItemRate MrpExPlanItemRate)
        {
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(MrpExPlanItemRate);
                SaveSuccessMessage(Resources.MRP.MrpExPlanItemRate.MrpExPlanItemRate_Updated);
            }

            return View(MrpExPlanItemRate);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">MrpExPlanItemRate id for delete</param>
        /// <returns>return to list view</returns>
        [SconitAuthorize(Permissions = "Url_MrpExPlanItemRate_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<MrpExPlanItemRate>(id);
                SaveSuccessMessage(Resources.MRP.MrpExPlanItemRate.MrpExPlanItemRate_Deleted);
                return RedirectToAction("List");
            }
        }


        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">MrpExPlanItemRate Search Model</param>
        /// <returns>return Search Statement</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, MrpExPlanItemRateSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Item", searchModel.Section, HqlStatementHelper.LikeMatchMode.Start, "i", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (string.IsNullOrWhiteSpace(sortingStatement))
            {
                sortingStatement = " order by i.Item ";
            }

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }
        class SectionItems
        {
            public string Section { get; set; }
            public string Item { get; set; }
        }
    }
}
