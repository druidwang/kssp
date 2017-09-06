using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Utility;
using com.Sconit.Web.Models.SearchModels.ISS;
using Telerik.Web.Mvc;
using com.Sconit.Entity.ISS;
using com.Sconit.Web.Models;
using com.Sconit.Service;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.CUST;

namespace com.Sconit.Web.Controllers.INP
{
    public class QualityDoorController : WebAppBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        private static string selectCountStatement = "select count(*) from IssueMaster as im join im.IssueType as it left join im.IssueNo ino ";

        /// <summary>
        /// 
        /// </summary>
        private static string selectStatement = "select im from IssueMaster as im join im.IssueType as it left join im.IssueNo ino ";


        //public IGenericMgr genericMgr { get; set; }

        public IIssueMgr issueMgr { get; set; }
        //
        // GET: /QualityDoor/
        #region public

        [SconitAuthorize(Permissions = "Url_QualityDoor_View")]
        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_QualityDoor_View")]
        public ActionResult List(GridCommand command, IssueMasterSearchModel searchModel)
        {
            this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_QualityDoor_View")]
        public ActionResult _AjaxList(GridCommand command, IssueMasterSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            GridModel<IssueMaster> List = GetAjaxPageData<IssueMaster>(searchStatementModel, command);
            foreach (IssueMaster issueMaster in List.Data)
            {
                issueMaster.FailCode = genericMgr.FindById<FailCode>(issueMaster.FailCode).CodeDescription;
            }
            return PartialView(List);
        }

        [GridAction]
        public ActionResult _IssueDetailList(string Code)
        {
            IList<IssueDetail> issueDetailList =
                genericMgr.FindAll<IssueDetail>("select i from IssueDetail as i where i.IssueCode = ?", Code);
            return PartialView(issueDetailList);
        }

        #region Edit
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_QualityDoor_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.Code = id;
                IssueMaster issue = this.genericMgr.FindById<IssueMaster>(id);
                return View(issue);
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_QualityDoor_View")]
        public ActionResult Edit(IssueMaster issue)
        {
            try
            {
                IssueMaster newIssue = genericMgr.FindById<IssueMaster>(issue.Code);
                newIssue.Content = issue.Content;
                genericMgr.Update(newIssue);
                SaveSuccessMessage(Resources.ISS.IssueMaster.Issue_Updated);
            }
            catch (BusinessException ex)
            {
                SaveErrorMessage(ex.Message);
            }
            return RedirectToAction("Edit/" + issue.Code);
        }


        [HttpGet]
        [SconitAuthorize(Permissions = "Url_QualityDoor_New")]
        public ActionResult New()
        {
            IssueMaster issue = new IssueMaster();
            if (TempData["issue"] != null)
            {
                issue.BackYards = ((IssueMaster)TempData["issue"]).BackYards;
                issue.ReleaseIssue = ((IssueMaster)TempData["issue"]).ReleaseIssue;
                issue.ContinuousCreate = ((IssueMaster)TempData["issue"]).ContinuousCreate;
                TempData["issue"] = null;
            }
            else {
                issue.ReleaseIssue = true;
                issue.ContinuousCreate = true;
            }
            return View(issue);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_QualityDoor_New")]
        public ActionResult New(IssueMaster issue, string Assemblies, string ProductCode)
        {
            TempData["issue"] = issue;
            IssueType issueType = genericMgr.FindById<IssueType>("ISS_QA");
            issue.IssueType = issueType;
            ModelState.Remove("IssueType");

            if (!string.IsNullOrWhiteSpace(issue.IssueNoCode))
            {
                ViewBag.IssueNoCode = issue.IssueNoCode;
                IssueNo issueNo = new IssueNo();
                issueNo.Code = issue.IssueNoCode;
                issue.IssueNo = issueNo;
            }
           
           
            if (!ModelState.IsValid)
            {
                ViewBag.Assemblies = Assemblies;
                ViewBag.ProductCode = ProductCode;
                return View(issue);
            }
            else
            {
                issueMgr.Create(issue);
                SaveSuccessMessage(Resources.ISS.IssueMaster.QD_Issue_Added);
                if(issue.ContinuousCreate){
                    return RedirectToAction("New");
                }
                return RedirectToAction("Edit/" + issue.Code);
                
            }

        }
        #endregion

        #region Edit Status

        [SconitAuthorize(Permissions = "Url_QualityDoor_View")]
        public ActionResult Submit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    this.issueMgr.Release(id);
                    SaveSuccessMessage(Resources.ISS.IssueMaster.QD_Issue_Submited);
                }
                catch (BusinessException ex)
                {
                    SaveErrorMessage(ex.Message);
                }
                return RedirectToAction("Edit/" + id);
            }
        }

        [SconitAuthorize(Permissions = "Url_QualityDoor_View")]
        public ActionResult Start(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                IssueMaster issue = this.genericMgr.FindById<IssueMaster>(id);
                if (issue == null)
                {
                    return HttpNotFound();
                }
                if (issue.Status == com.Sconit.CodeMaster.IssueStatus.Submit)
                {
                    try
                    {
                        issue.StartDate = DateTime.Now;
                        issue.StartUser = this.CurrentUser.Id;
                        issue.StartUserName = this.CurrentUser.FullName;
                        issue.Status = com.Sconit.CodeMaster.IssueStatus.InProcess;
                        this.genericMgr.Update(issue);
                        SaveSuccessMessage(Resources.ISS.IssueMaster.QD_Issue_Started);
                    }
                    catch (BusinessException ex)
                    {
                        SaveErrorMessage(ex.Message);
                    }
                }
                return RedirectToAction("Edit/" + id);
            }
        }

        [SconitAuthorize(Permissions = "Url_QualityDoor_View")]
        public ActionResult Complete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                IssueMaster issue = this.genericMgr.FindById<IssueMaster>(id);
                if (issue == null)
                {
                    return HttpNotFound();
                }
                if (issue.Status == com.Sconit.CodeMaster.IssueStatus.InProcess)
                {
                    try
                    {
                        issue.CompleteDate = DateTime.Now;
                        issue.CompleteUser = this.CurrentUser.Id;
                        issue.CompleteUserName = this.CurrentUser.FullName;
                        issue.Status = com.Sconit.CodeMaster.IssueStatus.Complete;
                        this.genericMgr.Update(issue);
                        SaveSuccessMessage(Resources.ISS.IssueMaster.QD_Issue_Completed);
                    }
                    catch (BusinessException ex)
                    {
                        SaveErrorMessage(ex.Message);
                    }
                }
                return RedirectToAction("Edit/" + id);
            }
        }

        [SconitAuthorize(Permissions = "Url_QualityDoor_View")]
        public ActionResult Close(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                IssueMaster issue = this.genericMgr.FindById<IssueMaster>(id);
                if (issue == null)
                {
                    return HttpNotFound();
                }
                if (issue.Status == com.Sconit.CodeMaster.IssueStatus.Complete)
                {
                    try
                    {
                        issue.CloseDate = DateTime.Now;
                        issue.CloseUserName = this.CurrentUser.FullName;
                        issue.CloseUser = this.CurrentUser.Id;
                        issue.Status = com.Sconit.CodeMaster.IssueStatus.Close;
                        this.genericMgr.Update(issue);
                        SaveSuccessMessage(Resources.ISS.IssueMaster.QD_Issue_Closed);


                    }
                    catch (BusinessException ex)
                    {
                        SaveErrorMessage(ex.Message);
                    }
                }
                return RedirectToAction("Edit/" + id);
            }

        }

        [SconitAuthorize(Permissions = "Url_QualityDoor_View")]
        public ActionResult Cancel(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                IssueMaster issue = this.genericMgr.FindById<IssueMaster>(id);
                if (issue == null)
                {
                    return HttpNotFound();
                }
                if (issue.Status == com.Sconit.CodeMaster.IssueStatus.Submit || issue.Status == com.Sconit.CodeMaster.IssueStatus.InProcess)
                {
                    try
                    {
                    issue.CancelDate = DateTime.Now;
                    issue.CancelUser = this.CurrentUser.Id;
                    issue.CancelUserName = this.CurrentUser.FullName;
                    issue.Status = com.Sconit.CodeMaster.IssueStatus.Cancel;
                    this.genericMgr.Update(issue);
                    SaveSuccessMessage(Resources.ISS.IssueMaster.QD_Issue_Canceled);

                    }
                    catch (BusinessException ex)
                    {
                        SaveErrorMessage(ex.Message);
                    }
                  
                }

                return RedirectToAction("Edit/" + id);
            }
        }

        [SconitAuthorize(Permissions = "Url_QualityDoor_View")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                IssueMaster issue = this.genericMgr.FindById<IssueMaster>(id);
                if (issue == null)
                {
                    return HttpNotFound();
                }
                if (issue.Status == com.Sconit.CodeMaster.IssueStatus.Create)
                {
                    this.genericMgr.DeleteById<IssueMaster>(id);
                    SaveSuccessMessage(Resources.ISS.IssueMaster.QD_Issue_Deleted);
                }
               
                return RedirectToAction("List");
            }
        }

        #endregion

        #endregion

        #region private

        private SearchStatementModel PrepareSearchStatement(GridCommand command, IssueMasterSearchModel searchModel)
        {
            string whereStatement = " where ( ";
            whereStatement += "                 im.CreateUserId = " + this.CurrentUser.Id;
            whereStatement += "                  or im.LastModifyUserId = " + this.CurrentUser.Id;
            whereStatement += "                  or im.ReleaseUser = " + this.CurrentUser.Id;
            whereStatement += "                  or im.StartUser = " + this.CurrentUser.Id;
            whereStatement += "                  or im.CloseUser = " + this.CurrentUser.Id;
            whereStatement += "                  or im.CancelUser = " + this.CurrentUser.Id;
            whereStatement += "                  or im.CompleteUser = " + this.CurrentUser.Id;
            whereStatement += "                  or exists(select count(det.Id) from IssueDetail det join det.User u where det.IssueCode = im.Code and u.Id = " + this.CurrentUser.Id + " ) ";
            whereStatement += "             ) ";
            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "im", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("IssueSubject", searchModel.IssueSubject, HqlStatementHelper.LikeMatchMode.Start, "im", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("BackYards", searchModel.BackYards, HqlStatementHelper.LikeMatchMode.Start, "im", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Content", searchModel.Content, HqlStatementHelper.LikeMatchMode.Start, "im", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("MobilePhone", searchModel.MobilePhone, HqlStatementHelper.LikeMatchMode.Start, "im", ref whereStatement, param);


            if (searchModel.DateFrom != null & searchModel.DateTo != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.DateFrom, searchModel.DateTo, "im", ref whereStatement, param);
            }
            else if (searchModel.DateFrom != null & searchModel.DateTo == null)
            {
                HqlStatementHelper.AddGeStatement("CreateDate", searchModel.DateFrom, "im", ref whereStatement, param);
            }
            else if (searchModel.DateFrom == null & searchModel.DateTo != null)
            {
                HqlStatementHelper.AddLeStatement("CreateDate", searchModel.DateTo, "im", ref whereStatement, param);
            }

            HqlStatementHelper.AddEqStatement("Status", searchModel.Status, "im", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Type", searchModel.Type, "im", ref whereStatement, param);


            HqlStatementHelper.AddEqStatement("Code", searchModel.IssueTypeCode, "it", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Code", searchModel.IssueNoCode, "ino", ref whereStatement, param);


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

    }
}
