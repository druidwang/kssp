/// <summary>
/// Summary description for SNRuleController
/// </summary>
namespace com.Sconit.Web.Controllers.SYS
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.SYS;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using System;

    /// <summary>
    /// This controller response to control the EntityPreference.
    /// </summary>
    public class SNRuleController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the EntityPreference security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        /// <summary>
        /// Gets or sets the this.SystemMgr which main consider the EntityPreference security 
        /// </summary>
        //public ISystemMgr systemMgr { get; set; }
        #endregion

        /// <summary>
        /// hql to get count of the Item 
        /// </summary>
        private static string selectCountStatement = "select count(*) from SNRule as s";

        /// <summary>
        /// hql to get all of the Item
        /// </summary>
        private static string selectStatement = "select s from SNRule as s";


        private static string selectSNRuleExtStatement = "select s from SNRuleExt as s where s.Code = ?";
        /// <summary>
        /// hql to get count of the SNRule by SNRule's code
        /// </summary>
        //private static string duiplicateVerifyStatement = @"select count(*) from SNRule as u where u.Code = ?";

        #region public actions
        /// <summary>
        /// Index action for SNRule controller
        /// </summary>
        /// <returns>Index view</returns>
        //[SconitAuthorize(Permission = "Url_SNRule_View")]
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ChooseSNRuleBlock(string id)
        {
            SNRule snRule = this.genericMgr.FindById<SNRule>(int.Parse(id));
            ViewBag.BlockSeq = snRule.BlockSeq;
            ViewBag.Id = id;
            return PartialView();
        }

        [HttpPost]
        public ActionResult ChooseSNRuleBlock(string Code, bool YearCode, bool MonthCode, bool DayCode, bool SequnceNum, bool FiledRef)
        {
            IList<char> ChoosedBlockList = new List<char>();
            IList<char> UnChoosedBlockList = new List<char>();

            //BlockList.Add(FixCode);
            if (YearCode)
            {              
                ChoosedBlockList.Add('2');
            }
            else
            {
                UnChoosedBlockList.Add('2');
            }

            if (MonthCode)
            {
                ChoosedBlockList.Add('3');
            }
            else
            {
                UnChoosedBlockList.Add('3');
            }

            if (DayCode)
            {
                ChoosedBlockList.Add('4');
            }
            else
            {
                UnChoosedBlockList.Add('4');
            }

            if (FiledRef)
            {
                ChoosedBlockList.Add('6');
            }
            else
            {
                UnChoosedBlockList.Add('6');
            }

            if (SequnceNum)
            {
                ChoosedBlockList.Add('5');
            }
            else
            {
                UnChoosedBlockList.Add('5');
            }

            TempData["ChoosedBlockList"] = ChoosedBlockList;
            TempData["UnChoosedBlockList"] = UnChoosedBlockList;
            //return RedirectToAction("Edit", new { id = Code, blockList = BlockList });
            return RedirectToAction("Edit/" + Code);
            //return View("New");
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">SNRule Search model</param>
        /// <returns>return the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        //[SconitAuthorize(Permission = "Url_SNRule_View")]
        public ActionResult List(GridCommand command, SNRuleSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page==0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        /// <summary>
        ///  AjaxList action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">SNRule Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        //[SconitAuthorize(Permission = "Url_SNRule_View")]
        public ActionResult _AjaxList(GridCommand command, SNRuleSearchModel searchModel)
        {
            string replaceFrom = "_AjaxList";
            string replaceTo = "List";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<SNRule>(searchStatementModel, command));
            
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id">SNRule id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
       // [SconitAuthorize(Permission = "Url_SNRule_Edit")]
        public ActionResult Edit(string id)
        {
            string addedItems = "";
            string removedItems = "";
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                IList<char> ChoosedBlockList = (IList<char>)TempData["ChoosedBlockList"];
                IList<char> UnChoosedBlockList = (IList<char>)TempData["UnChoosedBlockList"];
                SNRule snRule = this.genericMgr.FindById<SNRule>(int.Parse(id));
                foreach (var item in ChoosedBlockList)
                {
                    if (!snRule.BlockSeq.Contains(item))
                    {
                        addedItems = addedItems + item.ToString();
                    }
                }

                foreach (var item in UnChoosedBlockList)
                {
                    if (snRule.BlockSeq.Contains(item))
                    {
                        removedItems = removedItems + item.ToString();
                    }
                }

                TempData["ChoosedBlockList"] = ChoosedBlockList;
                TempData["UnChoosedBlockList"] = UnChoosedBlockList;

                ViewBag.AddedItems = addedItems;
                ViewBag.RemovedItems = removedItems;

                return PartialView(snRule);
            }
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="SNRule">SNRule Model</param>
        /// <returns>return the result view</returns>
        [HttpPost]
        //[SconitAuthorize(Permission = "Url_SNRule_Edit")]
        public ActionResult Edit(string Code, string YearCode, string MonthCode, string DayCode, string SeqLength,string BlockSeq)
        {
            SNRule snRule = this.genericMgr.FindById<SNRule>(int.Parse(Code));
            snRule.YearCode = YearCode;
            snRule.MonthCode = MonthCode;
            snRule.DayCode = DayCode;
            snRule.SeqLength = Int16.Parse(SeqLength);
            snRule.BlockSeq = BlockSeq;
            this.genericMgr.Update(snRule);
            return RedirectToAction("List");
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">SNRule id for delete</param>
        /// <returns>return to List action</returns>
        //[SconitAuthorize(Permission = "Url_SNRule_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<SNRule>(int.Parse(id));
                SaveSuccessMessage("deleted success");
                return RedirectToAction("List");
            }
        }

        [HttpGet]
        public ActionResult ExtRuleEdit(string Id)
        {
            string ChoosedBlocks = "";
            string UnChoosedBlocks = "";
            foreach (var item in (IList<char>)TempData["ChoosedBlockList"])
            {
                ChoosedBlocks = ChoosedBlocks + item.ToString();
            }

            foreach (var item in (IList<char>)TempData["UnChoosedBlockList"])
            {
                UnChoosedBlocks = UnChoosedBlocks + item.ToString();
            }

            IList<SNRuleExt> snRuleExtList = new List<SNRuleExt>();
            snRuleExtList = this.genericMgr.FindAll<SNRuleExt>(selectSNRuleExtStatement, int.Parse(Id));
            snRuleExtList = snRuleExtList.OrderByDescending(i => i.IsChoosed).ThenBy(i=>i.FieldSeq).ToList();
            //snRuleExtList = snRuleExtList.OrderBy(i => i.FieldSeq).ToList();
            ViewBag.ChoosedBlocks = ChoosedBlocks;
            ViewBag.UnChoosedBlocks = UnChoosedBlocks;
            return PartialView(snRuleExtList);
        }

        [HttpPost]
        public ActionResult ExtRuleEdit(string Code, string SelectedValue, string UnSelectedValue, string ChoosedBlocks, string UnChoosedBlocks)
        {
            IList<char> ChoosedBlockList = new List<char>();
            IList<char> UnChoosedBlockList = new List<char>();
            for (int i = 0; i < ChoosedBlocks.Length; i++)
            {
                ChoosedBlockList.Add(char.Parse(ChoosedBlocks.Substring(0,1)));
                ChoosedBlocks = ChoosedBlocks.Substring(1, ChoosedBlocks.Length - 1);
            }

            for (int i = 0; i < UnChoosedBlocks.Length; i++)
            {
                UnChoosedBlockList.Add(char.Parse(UnChoosedBlocks.Substring(0, 1)));
                UnChoosedBlocks = UnChoosedBlocks.Substring(1, UnChoosedBlocks.Length - 1);
            }

            string[] choosedIds = SelectedValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            string[] unChoosedIds = UnSelectedValue.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < choosedIds.Length; ++i)
            {
                UpdateChoosedSNRuleExt(i+1,int.Parse(choosedIds[i]));
            }

            for (int i = 0; i < unChoosedIds.Length; ++i)
            {
                UpdateUnChoosedSNRuleExt(int.Parse(unChoosedIds[i]));
            }

            TempData["ChoosedBlockList"] = ChoosedBlockList;
            TempData["UnChoosedBlockList"] = UnChoosedBlockList;

            return RedirectToAction("Edit/" + Code);
            ////IList<SNRuleExt> snRuleExtList = new List<SNRuleExt>();
            ////snRuleExtList = this.genericMgr.FindAll<SNRuleExt>(selectSNRuleExtStatement, Id);
            ////return PartialView(snRuleExtList);
        }
        #endregion


        private void UpdateChoosedSNRuleExt(int FieldSeq,int Id)
        {
            SNRuleExt snRuleExt = new SNRuleExt();
            snRuleExt = this.genericMgr.FindById<SNRuleExt>(Id);
            snRuleExt.FieldSeq = FieldSeq;
            snRuleExt.IsChoosed = true;
            this.genericMgr.Update(snRuleExt);
        }

        private void UpdateUnChoosedSNRuleExt(int Id)
        {
            SNRuleExt snRuleExt = new SNRuleExt();
            snRuleExt = this.genericMgr.FindById<SNRuleExt>(Id);
            snRuleExt.FieldSeq = 1000;
            snRuleExt.IsChoosed = false;
            this.genericMgr.Update(snRuleExt);
        }

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">SNRule Search Model</param>
        /// <returns>return SNRule search model</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, SNRuleSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddEqStatement("Code", searchModel.Code, "s", ref whereStatement, param);
            //HqlStatementHelper.AddLikeStatement("SNRuleContent", searchModel.SNRuleContent, HqlStatementHelper.LikeMatchMode.Anywhere, "u", ref whereStatement, param);
            //HqlStatementHelper.AddLikeStatement("PostCode", searchModel.PostCode, HqlStatementHelper.LikeMatchMode.Anywhere, "u", ref whereStatement, param);
            //HqlStatementHelper.AddLikeStatement("TelPhone", searchModel.TelPhone, HqlStatementHelper.LikeMatchMode.Anywhere, "u", ref whereStatement, param);
            //HqlStatementHelper.AddLikeStatement("MobilePhone", searchModel.MobilePhone, HqlStatementHelper.LikeMatchMode.Anywhere, "u", ref whereStatement, param);
            //HqlStatementHelper.AddLikeStatement("Fax", searchModel.Fax, HqlStatementHelper.LikeMatchMode.Anywhere, "u", ref whereStatement, param);
            //HqlStatementHelper.AddLikeStatement("Email", searchModel.Email, HqlStatementHelper.LikeMatchMode.Anywhere, "u", ref whereStatement, param);
            //HqlStatementHelper.AddLikeStatement("ContactPersonName", searchModel.ContactPersonName, HqlStatementHelper.LikeMatchMode.Anywhere, "u", ref whereStatement, param);
            //HqlStatementHelper.AddEqStatement("Type", searchModel.Type, "u", ref whereStatement, param);

            if (command.SortDescriptors.Count > 0)
            {
                if (command.SortDescriptors[0].Member == "DocumentsTypeDescription")
                {
                    command.SortDescriptors[0].Member = "Code";
                }
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
    }
}
