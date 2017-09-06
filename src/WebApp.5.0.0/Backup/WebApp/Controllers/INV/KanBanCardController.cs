using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.INV;
using com.Sconit.Entity.MD;
using com.Sconit.PrintModel.INV;
using com.Sconit.Service;
using com.Sconit.Utility.Report;
using com.Sconit.Web.Models;
using com.Sconit.Web.Models.SearchModels.INV;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Utility.Report.Operator;

namespace com.Sconit.Web.Controllers.INV
{
    public class KanBanCardController : WebAppBaseController
    {

        private static string selectCountStatement = "select count(*) from KanBanCard  ka";

        private static string selectStatement = "select ka from KanBanCard as ka";

        //public IGenericMgr genericMgr { get; set; }
        public IKanBanCardMgr kanBanCardMgr { get; set; }
        //public IReportGen reportGen { get; set; }

        public ActionResult Index()
        {
            return View();
        }
        

        [HttpPost]
        public string KanBanCardExport(string cardNoStr, string CodeValue)
        {
            try
            {
                KanBanCard Card = queryMgr.FindById<KanBanCard>(CodeValue);
                IList<KanBanCardInfo> KanBanCardInfoList = new List<KanBanCardInfo>();
                if (!string.IsNullOrEmpty(cardNoStr))
                {
                    string[] cardNoArray = cardNoStr.Split(',');
                    for (int i = 0; i < cardNoArray.Count(); i++)
                    {
                        KanBanCardInfo kanBanCard = this.genericMgr.FindById<KanBanCardInfo>(cardNoArray[i]);
                        KanBanCardInfoList.Add(kanBanCard);
                    }
                }
                Card.KanBanDetails = KanBanCardInfoList;
                PrintKanBanCard printKanBanCard = Mapper.Map<KanBanCard, PrintKanBanCard>(Card);
                IList<object> data = new List<object>();
                data.Add(printKanBanCard);
                data.Add(printKanBanCard.KanBanDetails);
                var CardTemplate = "";
                if (Card.IsEleKanBan)
                {
                    CardTemplate = "KanBanCard.xls";
                }
                else
                {
                    CardTemplate = "EKanBanCard.xls";
                }
                    reportGen.WriteToClient(CardTemplate, data, CardTemplate);
                    return ""; 
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return null;
            }
        }
        [HttpPost]
        public string KanBanCardPrint(string cardNoStr, string Code)
        {
            try
            {
                KanBanCard Card = queryMgr.FindById<KanBanCard>(Code);
                IList<KanBanCardInfo> KanBanCardInfoList = new List<KanBanCardInfo>();
                if (!string.IsNullOrEmpty(cardNoStr))
                {
                    string[] cardNoArray = cardNoStr.Split(',');
                    for (int i = 0; i < cardNoArray.Count(); i++)
                    {
                        KanBanCardInfo kanBanCard = this.genericMgr.FindById<KanBanCardInfo>(cardNoArray[i]);
                        KanBanCardInfoList.Add(kanBanCard);
                    }
                }
                Card.KanBanDetails = KanBanCardInfoList;
                PrintKanBanCard printKanBanCard = Mapper.Map<KanBanCard, PrintKanBanCard>(Card);
                IList<object> data = new List<object>();
                data.Add(printKanBanCard);
                data.Add(printKanBanCard.KanBanDetails);
                var CardTemplate = "";
                if (Card.IsEleKanBan)
                {
                    CardTemplate = "KanBanCard.xls";
                }
                else
                {
                    CardTemplate = "EKanBanCard.xls";
                }
                    string reportFileUrl = reportGen.WriteToFile(CardTemplate, data);
                    return reportFileUrl; ;
            }
            catch (BusinessException ex)
            {
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return null;
            }
        }
        public string PrintCard(KanBanCard Card, string huTemplate)
        {
            PrintKanBanCard card = Mapper.Map<KanBanCard, PrintKanBanCard>(Card);
            IList<object> data = new List<object>();
            data.Add(card);
            data.Add(CurrentUser.FullName);
            return reportGen.WriteToFile(huTemplate, data);
        }

        #region Print New
        [HttpGet]
        public string KanBanCardPrint(string codeStr)
        {
            try
            {
                if (string.IsNullOrEmpty(codeStr))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_PleaseChoosePrintedDetail);
                }
                string[] codeArray = codeStr.Split(',');
                string cardHql = string.Empty;
                string reportFileUrlStr = string.Empty;
               // string infoHql = string.Empty;
                IList<object> pare = new List<object>();
                for (int i = 0; i < codeArray.Count(); i++)
                {
                    if (string.IsNullOrEmpty(cardHql))
                    {
                        cardHql = "select c from KanBanCard as c where c.Code in (?";
                       // infoHql = "select c from KanBanCardInfo as k where k.Code in (?";
                    }
                    else
                    {
                        cardHql += ",?";
                       // infoHql += ",?";
                    }
                    pare.Add(codeArray[i]);
                }
                IList<KanBanCard> kanBanCardList = this.genericMgr.FindAll<KanBanCard>(cardHql + ")", pare.ToArray());
                // IList<KanBanCardInfo> kanBanCardInfoList = this.genericMgr.FindAll<KanBanCardInfo>(infoHql+")",pare.ToArray());
                IList<KanBanCard> EKanBanCardList = new List<KanBanCard>();
                IList<KanBanCard> KanBanCardList = new List<KanBanCard>();
                foreach (KanBanCard Card in kanBanCardList)
                {
                    IList<KanBanCardInfo> kanBanCardInfoList = this.genericMgr.FindAll<KanBanCardInfo>("from KanBanCardInfo where KBICode=?", Card.Code);
                    Card.KanBanDetails = kanBanCardInfoList;
                    if (Card.IsEleKanBan)
                    {
                        EKanBanCardList.Add(Card);
                    }
                    else {
                        KanBanCardList.Add(Card);
                    }
                }

                IList<PrintKanBanCard> printEKanBanCard = Mapper.Map<IList<KanBanCard>, IList<PrintKanBanCard>>(EKanBanCardList);
                IList<PrintKanBanCard> printKanBanCard = Mapper.Map<IList<KanBanCard>, IList<PrintKanBanCard>>(KanBanCardList);
               // IList<object> data = new List<object>();
                if (printEKanBanCard.Count > 0)
                {
                    IList<object> Edata = new List<object>();
                    Edata.Add(printEKanBanCard);
                    reportFileUrlStr += reportGen.WriteToFile("EKanBanCard.xls", Edata) + ",";
                }
                if (printKanBanCard.Count > 0)
                {
                    IList<object> data = new List<object>();
                    data.Add(printKanBanCard);
                    reportFileUrlStr += reportGen.WriteToFile("KanBanCard.xls", data) + ",";
                }
               // data.Add(printKanBanCard);
               //// data.Add(printKanBanCard.KanBanDetails);
               // var CardTemplate = "";
               // if (Card.IsEleKanBan)
               // {
               //     CardTemplate = "KanBanCard.xls";
               // }
               // else
               // {
               //     CardTemplate = "EKanBanCard.xls";
               // }
               //     reportFileUrlStr += reportGen.WriteToFile(CardTemplate, data)+",";
                
                return reportFileUrlStr.Substring(0,reportFileUrlStr.Length-1);
            }
            catch (BusinessException ex)
            {
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return null;
            }
        }

        public string CardLabelPrint(string codeStr)
        {
            try
            {
                if (string.IsNullOrEmpty(codeStr))
                {
                    throw new BusinessException(Resources.EXT.ControllerLan.Con_PleaseChoosePrintedDetail);
                }
                string[] codeArray = codeStr.Split(',');
                string cardHql = string.Empty;
                IList<object> pare = new List<object>();
                for (int i = 0; i < codeArray.Count(); i++)
                {
                    if (string.IsNullOrEmpty(cardHql))
                    {
                        cardHql = "select c from KanBanCard as c where c.Code in (?";
                    }
                    else
                    {
                        cardHql += ",?";
                    }
                    pare.Add(codeArray[i]);
                }
                IList<KanBanCard> kanBanCardList = this.genericMgr.FindAll<KanBanCard>(cardHql + ")", pare.ToArray());
                IList<PrintKanBanCard> printKanBanCardList = Mapper.Map<IList<KanBanCard>,IList<PrintKanBanCard>>(kanBanCardList);
                IList<object> data = new List<object>();
                data.Add(printKanBanCardList);
               return reportGen.WriteToFile("KBLabel.xls", data);
            }
            catch (BusinessException ex)
            {
                Response.StatusCode = 500;
                Response.Write(ex.GetMessages()[0].GetMessageString());
                return null;
            }
        }
        #endregion

        #region  New
        [SconitAuthorize(Permissions = "Url_KanBanCard_View")]
        public ActionResult New()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_KanBanCard_View")]
        public ActionResult List(GridCommand command, KanBanCardSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_KanBanCard_View")]
        public ActionResult _AjaxList(GridCommand command, KanBanCardSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<KanBanCard>(searchStatementModel, command));
        }

        [SconitAuthorize(Permissions = "Url_KanBanCard_View")]
        public ActionResult Edit(string cardNo)
        {
            KanBanCard kanBanCard = this.genericMgr.FindById<KanBanCard>(cardNo);
            return View(kanBanCard);
        }

        public ActionResult _GetItemDetail(string itemCode)
        {
            Item item = genericMgr.FindById<Item>(itemCode);
            if (item != null)
            {
                item.MinUnitCount = item.UnitCount;
            }

            return this.Json(item);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_KanBanCard_New")]
        public ActionResult New(KanBanCard kanBanCard)
        {
            if (ModelState.IsValid)
            {
                try
                {
                   //string hql = " from KanBanCard as k where k.Flow = ? and k.LocationTo = ? and k.Item = ?";
                   //  IList<object> param = new List<object>();
                   // param.Add(kanBanCard.Flow);
                   // param.Add(kanBanCard.LocationTo);
                   // param.Add(kanBanCard.Item);
                   //if (kanBanCard.ManufactureParty!=null)
                   //{
                   //    hql += " and k.ManufactureParty=?";
                   //    param.Add(kanBanCard.ManufactureParty);
                   // }
                   //IList<KanBanCard>  kanBanCardSearch = genericMgr.FindAll<KanBanCard>(hql, param.ToArray());
                   //if (kanBanCardSearch != null && kanBanCardSearch.Count > 0)
                   //{
                   //    SaveSuccessMessage("已经存在，请重新输入。");
                   //    return View(kanBanCard);
                   //}
                    Item item = genericMgr.FindById<Item>(kanBanCard.Item);
                    kanBanCard.ItemDescription = item.Description;
                   if(string.IsNullOrEmpty(kanBanCard.ReferenceItemCode))
                   {
                       kanBanCard.ReferenceItemCode = item.ReferenceCode;
                   }
                    kanBanCard.ReferenceItemCode = item.ReferenceCode;
                    if (string.IsNullOrEmpty(kanBanCard.Uom))
                    {
                        kanBanCard.Uom = item.Uom;
                    }
                    kanBanCardMgr.CreateKanBanCard(kanBanCard, kanBanCard.Qty);
                    SaveSuccessMessage(Resources.INV.KanBanCard.KanBanCard_Added);
                    return RedirectToAction("Edit", new { cardNo = kanBanCard.Code });
                }
                catch (BusinessException ex)
                {
                    SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                }
            }
            return View(kanBanCard);
        }
        #endregion

        #region Edit
        [SconitAuthorize(Permissions = "Url_KanBanCard_New")]
        [HttpPost]
        public ActionResult Edit(KanBanCard kanbanCard)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    kanbanCard.ItemDescription = genericMgr.FindById<Item>(kanbanCard.Item).Description;
                    kanBanCardMgr.UpdateKanBanCard(kanbanCard, kanbanCard.Qty);
                    SaveSuccessMessage(Resources.INV.KanBanCard.KanBanCard_Added);
                    return RedirectToAction("Edit", new { cardNo = kanbanCard.Code });
                }
                catch (BusinessException ex)
                {
                    SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
                }
            }
            return View(kanbanCard);
        }

        [SconitAuthorize(Permissions = "Url_KanBanCard_New")]
        public ActionResult DeleteKanBanCard(string Code,int? Qty)
        {
            try
            {
                KanBanCard kanBanCard=genericMgr.FindById<KanBanCard>(Code);
                kanBanCardMgr.DeleteKanBanCard(kanBanCard, Qty);
                SaveSuccessMessage(Resources.INV.KanBanCard.KanBanCard_Deleted);
                
            }
            catch (BusinessException ex )
            {
                SaveErrorMessage(ex.GetMessages()[0].GetMessageString());
            }
            return RedirectToAction("Edit", new { cardNo = Code });
            

        }

        public ActionResult _KanBanCardInfo(string KBICode)
        {
            if (string.IsNullOrEmpty(KBICode))
            {
                return HttpNotFound();
            }
            else
            {
                string selectStatement = "from KanBanCardInfo as k where k.KBICode='"+KBICode+"' ";
                IList<KanBanCardInfo> kanBanCardInfoList = genericMgr.FindAll<KanBanCardInfo>(selectStatement);
                return PartialView(kanBanCardInfoList);
            }
        }
        #endregion
        



        #region 打印导出

        public string PrintCardList(IList<KanBanCard> kanBanCardList)
        {
            IList<PrintKanBanCard> printKanBanCardList = Mapper.Map<IList<KanBanCard>, IList<PrintKanBanCard>>(kanBanCardList);

            IList<object> data = new List<object>();
            data.Add(printKanBanCardList);
            data.Add(CurrentUser.FullName);
            string templateName = "";
            if (kanBanCardList[0].ThumbNo > 0)
            {
                templateName = "EKanBanCard.xls";
            }
            else
            {
                templateName = "KanBanCard.xls";
            }
            return reportGen.WriteToFile(templateName, data);
        }
        #endregion

        private SearchStatementModel PrepareSearchStatement(GridCommand command, KanBanCardSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "ka", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Flow", searchModel.Flow, "ka", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "ka", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("LocationTo", searchModel.LocatoinTo, "ka", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Note", searchModel.Note, HqlStatementHelper.LikeMatchMode.Start, "ka", ref whereStatement, param);

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
