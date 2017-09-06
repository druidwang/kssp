


namespace com.Sconit.Web.Controllers.ISS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.ISS;
    using com.Sconit.Entity.ISS;
    using com.Sconit.Service;

    public class IssueController : WebAppBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        private static string selectCountStatement = "select count(*) from IssueMaster as im join im.IssueType as it left join im.IssueNo ino ";

        /// <summary>
        /// 
        /// </summary>
        private static string selectStatement = "select im from IssueMaster as im join im.IssueType as it left join im.IssueNo ino ";

        /// <summary>
        /// 
        /// </summary>
        private static string selectStatementByCode = "select det from IssueDetail det where IssueCode = ? ";


        /// <summary>
        /// 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IIssueMgr issueMgr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IIssueUtilMgr issueUtilMgr { get; set; }
        //
        // GET: /Issue/

        [SconitAuthorize(Permissions = "Url_Issue_View")]
        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Issue_View")]
        public ActionResult List(GridCommand command, IssueMasterSearchModel searchModel)
        {
            TempData["IssueMasterSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Issue_View")]
        public ActionResult _AjaxList(GridCommand command, IssueMasterSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<IssueMaster>(searchStatementModel, command));
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Issue_Edit")]
        public ActionResult New()
        {
            IssueMaster issue = (IssueMaster)this.TempData["Issue"];
            if (issue == null)
            {
                issue = new IssueMaster();
                issue.MobilePhone = this.CurrentUser.MobilePhone;
                issue.Email = this.CurrentUser.Email;
                issue.UserName = this.CurrentUser.FullName;
            }
            else
            {
                this.TempData["Issue"] = null;
            }

            return View(issue);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Issue_Edit")]
        public ActionResult New(IssueMaster issue)
        {
            if (!string.IsNullOrWhiteSpace(issue.IssueTypeCode))
            {
                ViewBag.IssueTypeCode = issue.IssueTypeCode;
                IssueType issueType = new IssueType();//this.genericMgr.FindById<IssueType>(issueTypeTo.IssueTypeCode);
                issueType.Code = issue.IssueTypeCode;
                issue.IssueType = issueType;
                ModelState.Remove("IssueType");
            }

            /*
            if (!string.IsNullOrWhiteSpace(issue.IssueAddressCode))
            {
                ViewBag.IssueAddressCode = issue.IssueAddressCode;
                IssueAddress issueAddress = new IssueAddress();//this.genericMgr.FindById<IssueType>(issueTypeTo.IssueTypeCode);
                issueAddress.Code = issue.IssueAddressCode;
                issue.IssueAddress = issueAddress;
                ModelState.Remove("IssueAddress");
            }
            */
            if (!string.IsNullOrWhiteSpace(issue.IssueNoCode))
            {
                ViewBag.IssueNoCode = issue.IssueNoCode;
                IssueNo issueNo = new IssueNo();//this.genericMgr.FindById<IssueType>(issueTypeTo.IssueTypeCode);
                issueNo.Code = issue.IssueNoCode;
                issue.IssueNo = issueNo;
                //ModelState.Remove("IssueNo");
            }

            if (!ModelState.IsValid)
            {
                return View(issue);
            }
            else
            {
                IssueMaster newIssue = new IssueMaster();

                newIssue.IssueNo = issue.IssueNo;
                newIssue.IssueAddress = issue.IssueAddress;
                newIssue.IssueSubject = issue.IssueSubject;
                newIssue.IssueType = issue.IssueType;
                newIssue.MobilePhone = issue.MobilePhone;
                newIssue.Email = issue.Email;
                newIssue.Type = issue.Type;
                newIssue.Priority = issue.Priority;
                newIssue.UserName = issue.UserName;
                newIssue.BackYards = issue.BackYards;
                newIssue.ReleaseIssue = issue.ReleaseIssue;

                issueMgr.Create(newIssue);
                SaveSuccessMessage(Resources.ISS.IssueMaster.Issue_Added);
                if (issue.ContinuousCreate)
                {
                    //清空不必要的值
                    IssueMaster newIssue2 = new IssueMaster();
                    newIssue2.BackYards = issue.BackYards;
                    newIssue2.IssueNo = issue.IssueNo;
                    //newIssue2.IssueNoCode = issue.IssueNo != null ? issue.IssueNo.Code : string.Empty;
                    //newIssue2.IssueTypeCode = issue.IssueType.Code;
                    newIssue2.IssueType = issue.IssueType;
                    newIssue2.IssueAddress = issue.IssueAddress;
                    newIssue2.Priority = issue.Priority;
                    newIssue2.Type = issue.Type;
                    newIssue2.UserName = issue.UserName;
                    newIssue2.MobilePhone = issue.MobilePhone;
                    newIssue2.Email = issue.Email;
                    newIssue2.ContinuousCreate = issue.ContinuousCreate;
                    newIssue2.IsRedirect = true;
                    newIssue2.ReleaseIssue = issue.ReleaseIssue;

                    this.TempData["Issue"] = newIssue2;

                    return this.RedirectToAction("New/");
                }
                else
                {
                    return RedirectToAction("Edit/" + newIssue.Code);
                }
            }

        }

        public ActionResult _IssueDetailList(string code)
        {
            IList<IssueDetail> issueDetailList = this.genericMgr.FindAll<IssueDetail>(selectStatementByCode, code);
            return PartialView(issueDetailList);
        }

        [SconitAuthorize(Permissions = "Url_Issue_Edit")]
        public ActionResult Submit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.issueMgr.Release(id);

                SaveSuccessMessage(Resources.ISS.IssueMaster.Issue_Submited);
                return RedirectToAction("Edit/" + id);
            }
        }
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Issue_Edit")]
        public ActionResult Start(string id, string finishedUserCode, DateTime? finishedDate, string solution)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                issueMgr.Start(id, finishedUserCode, finishedDate, solution);
                SaveSuccessMessage(Resources.ISS.IssueMaster.Issue_Started);
                this.TempData["Status"] = com.Sconit.CodeMaster.IssueStatus.InProcess;
                return RedirectToAction("Edit/" + id);
            }
        }

        [SconitAuthorize(Permissions = "Url_Issue_Edit")]
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
                    issue.StartDate = DateTime.Now;
                    issue.StartUser = this.CurrentUser.Id;
                    issue.StartUserName = this.CurrentUser.FullName;
                    issue.Status = com.Sconit.CodeMaster.IssueStatus.InProcess;
                    this.genericMgr.Update(issue);
                    SaveSuccessMessage(Resources.ISS.IssueMaster.Issue_Started);
                }
                else
                {
                    this.SaveErrorMessage(Resources.ISS.IssueMaster.Errors_StatusErrorWhenStart, new string[] { issue.Status.ToString(), issue.Code });
                }
                return RedirectToAction("Edit/" + id);
            }
        }

        [SconitAuthorize(Permissions = "Url_Issue_Edit")]
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
                    issue.CompleteDate = DateTime.Now;
                    issue.CompleteUser = this.CurrentUser.Id;
                    issue.CompleteUserName = this.CurrentUser.FullName;
                    issue.Status = com.Sconit.CodeMaster.IssueStatus.Complete;
                    this.genericMgr.Update(issue);
                    SaveSuccessMessage(Resources.ISS.IssueMaster.Issue_Completed);
                }
                else
                {
                    this.SaveErrorMessage(Resources.ISS.IssueMaster.Errors_StatusErrorWhenComplete, new string[] { issue.Status.ToString(), issue.Code });
                }
                return RedirectToAction("Edit/" + id);
            }
        }

        [SconitAuthorize(Permissions = "Url_Issue_Edit")]
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
                    issue.CloseDate = DateTime.Now;
                    issue.CloseUserName = this.CurrentUser.FullName;
                    issue.CloseUser = this.CurrentUser.Id;
                    issue.Status = com.Sconit.CodeMaster.IssueStatus.Close;
                    this.genericMgr.Update(issue);
                    SaveSuccessMessage(Resources.ISS.IssueMaster.Issue_Closed);
                }
                else
                {
                    this.SaveErrorMessage(Resources.ISS.IssueMaster.Errors_StatusErrorWhenClose, new string[] { issue.Status.ToString(), issue.Code });
                }
                return RedirectToAction("Edit/" + id);
            }

        }

        [SconitAuthorize(Permissions = "Url_Issue_Edit")]
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
                    issue.CancelDate = DateTime.Now;
                    issue.CancelUser = this.CurrentUser.Id;
                    issue.CancelUserName = this.CurrentUser.FullName;
                    issue.Status = com.Sconit.CodeMaster.IssueStatus.Cancel;
                    this.genericMgr.Update(issue);
                    SaveSuccessMessage(Resources.ISS.IssueMaster.Issue_Canceled);
                }
                else
                {
                    this.SaveErrorMessage(Resources.ISS.IssueMaster.Errors_StatusErrorWhenCancel, new string[] { issue.Status.ToString(), issue.Code });
                }
                return RedirectToAction("Edit/" + id);
            }
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Issue_Edit")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                IssueMaster issue = this.genericMgr.FindById<IssueMaster>(id);

                InitPage(issue);

                if (this.TempData["Status"] == null)
                {
                    return View(issue);
                }
                else
                {
                    this.TempData["Status"] = null;
                    return this.PartialView(issue);
                }
            }
        }

        private void InitPage(IssueMaster issue)
        {
            ViewBag.Code = issue.Code;
            ViewBag.isEditable = issue.Status == com.Sconit.CodeMaster.IssueStatus.Create && issue.CreateUserId == this.CurrentUser.Id;
            ViewBag.isEditable2 = (issue.Status == com.Sconit.CodeMaster.IssueStatus.Submit || issue.Status == com.Sconit.CodeMaster.IssueStatus.InProcess) && HaveAdminPermission();
            ViewBag.editorTemplate = issue.Status == com.Sconit.CodeMaster.IssueStatus.Create && issue.CreateUserId == this.CurrentUser.Id ? "" : "ReadonlyTextBox";
            //ViewBag.editorTemplate2 = !(issue.Status == com.Sconit.CodeMaster.IssueStatus.Create && issue.CreateUserId == this.CurrentUser.Id) && issue.Status != com.Sconit.CodeMaster.IssueStatus.Close && issue.Status == com.Sconit.CodeMaster.IssueStatus.Cancel ? "" : "ReadonlyTextBox";

            ViewBag.showRelease = issue.Status == com.Sconit.CodeMaster.IssueStatus.Create && (issue.CreateUserId == this.CurrentUser.Id || HaveAdminPermission());
            ViewBag.showDelete = issue.Status == com.Sconit.CodeMaster.IssueStatus.Create && (issue.CreateUserId == this.CurrentUser.Id || HaveAdminPermission());
            ViewBag.showStart = issue.Status == com.Sconit.CodeMaster.IssueStatus.Submit && (HaveAdminPermission() || issueUtilMgr.HavePermission(issue.Code, com.Sconit.CodeMaster.IssueStatus.Submit));
            ViewBag.showComplete = issue.Status == com.Sconit.CodeMaster.IssueStatus.InProcess && (HaveAdminPermission() || issueUtilMgr.HavePermission(issue.Code, com.Sconit.CodeMaster.IssueStatus.InProcess));
            ViewBag.showClose = issue.Status == com.Sconit.CodeMaster.IssueStatus.Complete && (issue.CreateUserId == this.CurrentUser.Id || HaveAdminPermission());
            ViewBag.showCancel = issue.Status == com.Sconit.CodeMaster.IssueStatus.Submit && (issue.CreateUserId == this.CurrentUser.Id || HaveAdminPermission());
        }

        private bool HaveAdminPermission()
        {
            return this.CurrentUser.UrlPermissions.IndexOf(IssueMaster.Menu_Issue_Admin) != -1;
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Issue_Edit")]
        public ActionResult Edit(IssueMaster issue)
        {
            if (!string.IsNullOrWhiteSpace(issue.IssueTypeCode))
            {
                ViewBag.IssueTypeCode = issue.IssueTypeCode;
                IssueType issueType = new IssueType();//this.genericMgr.FindById<IssueType>(issueTypeTo.IssueTypeCode);
                issueType.Code = issue.IssueTypeCode;
                issue.IssueType = issueType;
                ModelState.Remove("IssueType");
            }
            /*
            if (!string.IsNullOrWhiteSpace(issue.IssueAddressCode))
            {
                ViewBag.IssueAddressCode = issue.IssueAddressCode;
                IssueAddress issueAddress = new IssueAddress();//this.genericMgr.FindById<IssueType>(issueTypeTo.IssueTypeCode);
                issueAddress.Code = issue.IssueAddressCode;
                issue.IssueAddress = issueAddress;
                ModelState.Remove("IssueAddress");
            }
            */
            if (!string.IsNullOrWhiteSpace(issue.IssueNoCode))
            {
                ViewBag.IssueNoCode = issue.IssueNoCode;
                IssueNo issueNo = new IssueNo();//this.genericMgr.FindById<IssueType>(issueTypeTo.IssueTypeCode);
                issueNo.Code = issue.IssueNoCode;
                issue.IssueNo = issueNo;
                //ModelState.Remove("IssueNo");
            }

            if (ModelState.IsValid)
            {
                genericMgr.Update(issue);
                SaveSuccessMessage(Resources.ISS.IssueMaster.Issue_Updated);
            }

            return RedirectToAction("Edit/" + issue.Code);
        }


        [SconitAuthorize(Permissions = "Url_Issue_Batch")]
        public ActionResult BatchProcessIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Issue_Batch")]
        public ActionResult BatchProcessList(GridCommand command, IssueMasterSearchModel searchModel)
        {
            //SearchCacheModel searchCacheModel = ProcessSearchModel(command, searchModel);
            //SearchStatementModel searchStatementModel = PrepareSearchStatement(command, (IssueMasterSearchModel)searchCacheModel.SearchObject);
            //return View(GetPageData<IssueMaster>(searchStatementModel, command));
            TempData["IssueMasterSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_Issue_Batch")]
        public ActionResult BatchProcessSearchResult(GridCommand command, IssueMasterSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = ProcessSearchModel(command, searchModel);
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, (IssueMasterSearchModel)searchCacheModel.SearchObject);
            return this.PartialView(GetPageData<IssueMaster>(searchStatementModel, command));
        }


        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Issue_Batch")]
        public ActionResult _AjaxBatchProcessList(GridCommand command, IssueMasterSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<IssueMaster>(searchStatementModel, command));
        }


        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Issue_Batch")]
        public ActionResult BatchDelete(string codeStr)
        {
            if (!string.IsNullOrWhiteSpace(codeStr))
            {
                int count = this.issueMgr.BatchDelete(codeStr, this.HaveAdminPermission());
                SaveSuccessMessage(Resources.ISS.IssueMaster.Issue_BatchDeleted, count.ToString());
            }
            return this.RedirectToAction("BatchProcessList");
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Issue_Batch")]
        public ActionResult BatchClose(string codeStr)
        {
            if (!string.IsNullOrWhiteSpace(codeStr))
            {
                int count = this.issueMgr.BatchClose(codeStr, this.HaveAdminPermission());
                SaveSuccessMessage(Resources.ISS.IssueMaster.Issue_BatchClosed, count.ToString());
            }
            return this.RedirectToAction("BatchProcessSearchResult/");
        }

        [GridAction]
        public ActionResult _IssueDetailsHierarchyAjax(string issueCode)
        {
            IList<IssueDetail> issueDetailList =
                genericMgr.FindAll<IssueDetail>("select i from IssueDetail as i where i.IssueCode = ?", issueCode);
            return View(new GridModel(issueDetailList));
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">Issue id for delete</param>
        /// <returns>return to List action</returns>
        [SconitAuthorize(Permissions = "Url_Issue_Delete")]
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
                    SaveSuccessMessage(Resources.ISS.IssueMaster.Issue_Deleted);
                }
                else
                {
                    this.SaveErrorMessage(Resources.ISS.IssueMaster.Errors_StatusErrorWhenDelete, new string[] { issue.Status.ToString(), issue.Code });
                }
                return RedirectToAction("List");
            }
        }

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
            HqlStatementHelper.AddLikeStatement("Solution", searchModel.Solution, HqlStatementHelper.LikeMatchMode.Start, "im", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Content", searchModel.Content, HqlStatementHelper.LikeMatchMode.Start, "im", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Email", searchModel.Email, HqlStatementHelper.LikeMatchMode.Start, "im", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("MobilePhone", searchModel.MobilePhone, HqlStatementHelper.LikeMatchMode.Start, "im", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("UserName", searchModel.UserName, HqlStatementHelper.LikeMatchMode.Start, "im", ref whereStatement, param);


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
            HqlStatementHelper.AddEqStatement("Priority", searchModel.Priority, "im", ref whereStatement, param);

            HqlStatementHelper.AddEqStatement("Code", searchModel.IssueTypeCode, "it", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Code", searchModel.IssueNoCode, "ino", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IssueAddress", searchModel.IssueAddressCode, "im", ref whereStatement, param);

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
