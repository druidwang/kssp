


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
    using com.Sconit.Web.Controllers.ACC;
    using com.Sconit.Entity.ISS;
    using com.Sconit.Service;
    using com.Sconit.Entity.ACC;
    using NHibernate.Type;
    using NHibernate;

    public class IssueTypeToController : WebAppBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        private static string selectCountStatement = "select count(*) from IssueTypeToMaster as ittm  ";



        /// <summary>
        /// 
        /// </summary>
        private static string selectStatement = "select ittm from IssueTypeToMaster as ittm  ";


        /// <summary>
        /// 
        /// </summary>
        private static string selectUserCountStatement = "select count(*) from User as u ";

        /// <summary>
        /// 
        /// </summary>
        private static string selectUserStatement = "select u from User as u ";


        /// <summary>
        /// 
        /// </summary>
        private static string codeDuiplicateVerifyStatement = @"select count(*) from IssueTypeToMaster as ittm where ittm.Code = ? ";

        /// <summary>
        /// 
        /// </summary>
        private static string selectIssueTypeToUserDetailStatement = "select ittud from IssueTypeToUserDetail as ittud join ittud.IssueTypeTo itt where itt.Code= ? ";

        /// <summary>
        /// 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IIssueMgr issueMgr { get; set; }

        #region IssueTypeToMaster

        //
        // GET: /IssueTypeTo/

        [SconitAuthorize(Permissions = "Url_IssueTypeTo_View")]
        public ActionResult Index()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_IssueTypeTo_View")]
        public ActionResult List(GridCommand command, IssueTypeToMasterSearchModel searchModel)
        {
            TempData["IssueTypeToMasterSearchModel"] = searchModel;
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_IssueTypeTo_Edit")]
        public ActionResult RemoveUser(string code, string idStr)
        {
            if (!string.IsNullOrWhiteSpace(code) && !string.IsNullOrWhiteSpace(idStr))
            {
                ViewBag.Code = code;

                IList<string> idList = (idStr.Split(',')).ToList<string>();

                if (idList != null && idList.Count > 0)
                {
                    string hql = string.Empty;
                    object[] para = new object[idList.Count];
                    IType[] type = new IType[idList.Count];

                    for (int i = 0; i < idList.Count; i++)
                    {
                        if (i == 0)
                        {
                            hql += "delete from IssueTypeToUserDetail  where id = ? ";
                        }
                        else
                        {
                            hql += " or id = ? ";
                        }
                        para[i] = idList[i];
                        type[i] = NHibernateUtil.UInt32;
                    }
                    if (hql != string.Empty)
                    {
                        this.genericMgr.Update(hql, para, type);
                    }
                    SaveSuccessMessage(Resources.ISS.IssueTypeToUserDetail.IssueTypeToUserDetail_Deleted);
                }
            }
            return this.RedirectToAction("_UserDetailList/" + code);
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_IssueTypeTo_Edit")]
        public ActionResult Subscibe(string code, string userIdStr, string emailStr, string smsStr)
        {
            if (!string.IsNullOrWhiteSpace(code) && !string.IsNullOrWhiteSpace(userIdStr) && (!string.IsNullOrWhiteSpace(emailStr) || !string.IsNullOrWhiteSpace(smsStr)))
            {
                ViewBag.Code = code;

                IList<string> userIdList = (userIdStr.Split(',')).ToList<string>();
                IList<string> emailList = (emailStr.Split(',')).ToList<string>();
                IList<string> smsList = (smsStr.Split(',')).ToList<string>();
                if (userIdList != null && (emailList != null || smsList != null))
                {
                    for (int i = 0; i < userIdList.Count; i++)
                    {
                        IssueTypeToUserDetail issueTypeToUserDetail = new IssueTypeToUserDetail();
                        IssueTypeToMaster issueTypeToMaster =new IssueTypeToMaster();
                        issueTypeToMaster.Code = code;
                        issueTypeToUserDetail.IssueTypeTo = issueTypeToMaster;
                        User user = new User();
                        user.Id = Int32.Parse(userIdList[i]);
                        issueTypeToUserDetail.User = user;
                        if (!string.IsNullOrWhiteSpace(emailList[i]) && bool.Parse(emailList[i]) == true)
                        {
                            issueTypeToUserDetail.IsEmail = true;
                        }
                        else
                        {
                            issueTypeToUserDetail.IsEmail = false;
                        }
                        if (!string.IsNullOrWhiteSpace(smsList[i]) && bool.Parse(smsList[i]) == true)
                        {
                            issueTypeToUserDetail.IsSMS = true;
                        }
                        else
                        {
                            issueTypeToUserDetail.IsSMS = false;
                        }
                        genericMgr.CreateWithTrim(issueTypeToUserDetail);
                    }
                    SaveSuccessMessage(Resources.ISS.IssueTypeToUserDetail.User_Subscibed);
                }
            }
            return this.RedirectToAction("ChooseUser/" + code);
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_IssueTypeTo_Edit")]
        public ActionResult ChooseUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.Code = id;
                return View();
            }
        }

        [HttpPost]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_IssueTypeTo_Edit")]
        public ActionResult ChooseUser(GridCommand command, IssueTypeToUserDetailSearchModel searchModel, string code)
        {
            command.PageSize = int.MaxValue;
            ViewBag.Code = code;
            SearchCacheModel searchCacheModel = ProcessSearchModel(command, searchModel);
            SearchStatementModel searchStatementModel = this.UserPrepareSearchStatement(command, (IssueTypeToUserDetailSearchModel)searchCacheModel.SearchObject, code);
            return this.View(GetPageData<User>(searchStatementModel, command));
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_IssueTypeTo_View")]
        public ActionResult _AjaxList(GridCommand command, IssueTypeToMasterSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<IssueTypeToMaster>(searchStatementModel, command));
        }

        
        [SconitAuthorize(Permissions = "Url_IssueTypeTo_Edit")]
        public ActionResult _UserDetailList(string id)
        {
            ViewBag.Code = id;
            IList<IssueTypeToUserDetail> issueTypeToUserDetailList = new List<IssueTypeToUserDetail>();
            
            issueTypeToUserDetailList = genericMgr.FindAll<IssueTypeToUserDetail>(selectIssueTypeToUserDetailStatement, id);
            return this.PartialView(issueTypeToUserDetailList);
        }

        [SconitAuthorize(Permissions = "Url_IssueTypeTo_Edit")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_IssueTypeTo_Edit")]
        public ActionResult New(IssueTypeToMaster issueTypeTo)
        {

            if (!string.IsNullOrWhiteSpace(issueTypeTo.IssueTypeCode))
            {
                ViewBag.IssueTypeCode = issueTypeTo.IssueTypeCode;
                IssueType issueType = new IssueType();//this.genericMgr.FindById<IssueType>(issueTypeTo.IssueTypeCode);
                issueType.Code = issueTypeTo.IssueTypeCode;
                issueTypeTo.IssueType = issueType;
                ModelState.Remove("IssueType");
            }
            if (!string.IsNullOrWhiteSpace(issueTypeTo.IssueLevelCode))
            {
                ViewBag.IssueLevelCode = issueTypeTo.IssueLevelCode;
                IssueLevel issueLevel = new IssueLevel();//this.genericMgr.FindById<IssueLevel>(issueTypeTo.IssueLevelCode);
                issueLevel.Code = issueTypeTo.IssueLevelCode;
                issueTypeTo.IssueLevel = issueLevel;
                ModelState.Remove("IssueLevel");
            }
            if (ModelState.IsValid)
            {
                //判断用户名不能重复
                if (this.genericMgr.FindAll<long>(codeDuiplicateVerifyStatement, new object[] { issueTypeTo.Code })[0] > 0)
                {
                    base.SaveErrorMessage(Resources.ISS.IssueTypeToMaster.Errors_Existing_IssueTypeToMaster, issueTypeTo.Code);
                }
                else
                {
                    genericMgr.CreateWithTrim(issueTypeTo);
                    SaveSuccessMessage(Resources.ISS.IssueTypeToMaster.IssueTypeToMaster_Added);
                    return RedirectToAction("Edit/" + issueTypeTo.Code);
                }
            }
            ViewBag.Code = issueTypeTo.Code;
            return View(issueTypeTo);
        }

        [HttpGet]
        [SconitAuthorize(Permissions = "Url_IssueTypeTo_Edit")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                IssueTypeToMaster issueTypeTo = this.genericMgr.FindById<IssueTypeToMaster>(id);
                ViewBag.Code = issueTypeTo.Code;
                return View(issueTypeTo);
            }
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">IssueTypeTo id for delete</param>
        /// <returns>return to List action</returns>
        [SconitAuthorize(Permissions = "Url_IssueTypeTo_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.issueMgr.DeleteIssueTypeTo(id);
                SaveSuccessMessage(Resources.ISS.IssueTypeToMaster.IssueTypeToMaster_Deleted);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        [SconitAuthorize(Permissions = "Url_IssueTypeTo_Edit")]
        public ActionResult Edit(IssueTypeToMaster issueTypeTo)
        {
            if (!string.IsNullOrWhiteSpace(issueTypeTo.IssueTypeCode))
            {
                ViewBag.IssueTypeCode = issueTypeTo.IssueTypeCode;
                IssueType issueType = new IssueType();//this.genericMgr.FindById<IssueType>(issueTypeTo.IssueTypeCode);
                issueType.Code = issueTypeTo.IssueTypeCode;
                issueTypeTo.IssueType = issueType;
                ModelState.Remove("IssueType");
            }
            if (!string.IsNullOrWhiteSpace(issueTypeTo.IssueLevelCode))
            {
                ViewBag.IssueLevelCode = issueTypeTo.IssueLevelCode;
                IssueLevel issueLevel = new IssueLevel();//this.genericMgr.FindById<IssueLevel>(issueTypeTo.IssueLevelCode);
                issueLevel.Code = issueTypeTo.IssueLevelCode;
                issueTypeTo.IssueLevel = issueLevel;
                ModelState.Remove("IssueLevel");
            }
            if (ModelState.IsValid)
            {
                genericMgr.UpdateWithTrim(issueTypeTo);
                SaveSuccessMessage(Resources.ISS.IssueTypeToMaster.IssueTypeToMaster_Updated);
            }

            return View(issueTypeTo);
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_IssueTypeTo_Edit")]
        public ActionResult _AjaxUserResultList(GridCommand command, IssueTypeToUserDetailSearchModel searchModel, string id)
        {
            command.PageSize = int.MaxValue;
            ViewBag.Code = id;
            SearchStatementModel searchStatementModel = this.UserPrepareSearchStatement(command, searchModel, id);
            return PartialView(GetAjaxPageData<User>(searchStatementModel, command));
        }

        private SearchStatementModel UserPrepareSearchStatement(GridCommand command, IssueTypeToUserDetailSearchModel searchModel, string id)
        {
            string whereStatement = " where u.Id not in (select ittudu.Id from IssueTypeToUserDetail ittud join ittud.User ittudu  where ittud.IssueTypeTo = '" + id + "') ";

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.UserCode, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Email", searchModel.Email, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("MobilePhone", searchModel.MobilePhone, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            if (searchModel.HasEmail)
            {
                whereStatement += " and u.Email is not null and u.Email !='' ";
            }
            if (searchModel.HasMobilePhone)
            {
                whereStatement += " and u.MobilePhone is not null and u.MobilePhone !='' ";
            }

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);

            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectUserCountStatement;
            searchStatementModel.SelectStatement = selectUserStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        private SearchStatementModel PrepareSearchStatement(GridCommand command, IssueTypeToMasterSearchModel searchModel)
        {
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "ittm", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Description", searchModel.Description, HqlStatementHelper.LikeMatchMode.Start, "ittm", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsActive", searchModel.IsActive, "ittm", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Code", searchModel.IssueLevel, "il", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Code", searchModel.IssueType, "it", ref whereStatement, param);

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

        #region IssueTypeToUserDetail

        #endregion

        #region IssueTypeToRoleDetail

        #endregion
    }
}
