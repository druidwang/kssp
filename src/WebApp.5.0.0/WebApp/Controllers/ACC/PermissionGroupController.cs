
/**************************************************
 * 
 * 
 * ************************************************/
namespace com.Sconit.Web.Controllers.ACC
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using com.Sconit.Entity.ACC;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.ACC;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using System;
    using System.Data.SqlClient;

    /// <summary>
    ///  PermissionGroup   Controller
    /// </summary>
    public class PermissionGroupController : AuthorizationBaseController
    {
        /// <summary>
        /// select  the  count  of PermissionGroup 
        /// </summary>
        private static string selectCountStatement = "select count(*) from PermissionGroup as u";

        /// <summary>
        /// select  the  whole  PermissionGroup
        /// </summary>
        private static string selectStatement = "select u from PermissionGroup as u";

        /// <summary>
        /// select the  count  of  PermissionGroup by  PermissionGroup Code
        /// </summary>
        private static string userNameDuiplicateVerifyStatement = @"select count(*) from PermissionGroup as u where u.Code = ?";

        /// <summary>
        /// select Role By PermissionGroupId
        /// </summary>
        private static string selectRoleByPermissionGroupId = "select rp.Role from RolePermissionGroup as rp where rp.PermissionGroup.Id = ? Order by Code ";

        /// <summary>
        /// select Roles Not In PermissionGroup
        /// </summary>
        private static string selectRolesNotInPermissionGroup = @"select r from Role as r where r.Id not in 
            (select  rp.Role.Id from RolePermissionGroup as rp where rp.PermissionGroup.Id = ?)";

        /// <summary>
        /// select Users By PermissionGroupId
        /// </summary>
        private static string selectUsersByPermissionGroupId = "select up.User from UserPermissionGroup as up where up.PermissionGroup.Id = ?";

        /// <summary>
        /// select Users Not In PermissionGroup
        /// </summary>
        private static string selectUsersNotInPermissionGroup = @"select u from User as u where u.Id not in 
            (select  up.User.Id from UserPermissionGroup as up where up.PermissionGroup.Id = ?)";

        /// <summary>
        /// select Permissions By PermissionGroupIdAndCategory
        /// </summary>
        private static string selectPermissionsByPermissionGroupIdAndCategory = @"select p from PermissionGroupPermission as pp 
             join pp.Permission as p where p.PermissionCategory = ? and pp.PermissionGroup.Id = ? and p.Sequence>=0 order by p.Sequence,p.Code";

        /// <summary>
        /// select Permissions Not In PermissionGroups By PermissionCategory
        /// </summary>
        private static string selectPermissionsNotInPermissionGroupsByCategory = @"select p from Permission as p
          where p.PermissionCategory = ? and p.Id not in (select p.Id  from PermissionGroupPermission as pp 
           join pp.Permission as p where pp.PermissionGroup.Id = ? and p.PermissionCategory = ?) and p.Sequence>=0 order by p.Sequence,p.Code";

        /// <summary>
        /// Gets or sets the generice management for the Object CRUD operator
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        /// <summary>
        /// Gets or sets the security management for the user permission
        /// </summary>
        public ISecurityMgr SecurityMgr { get; set; }

        /// <summary>
        /// PermissionGroup  View
        /// </summary>
        /// <returns>View  of PermissionGroup </returns>
        [SconitAuthorize(Permissions = "Url_Permission_View")]
        public ActionResult Index()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_PermissionReport_View")]
        public ActionResult PermissionReport()
        {
            return View();
        }
        /// <summary>
        ///obtain the PermissionGroups by PermissionGroupSearchModel
        /// </summary>
        /// <param name="command">GridCommand</param>
        /// <param name="searchModel">PermissionGroup SearchModel</param>
        /// <returns>Grid View</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Permission_View")]
        public ActionResult List(GridCommand command, PermissionGroupSearchModel searchModel)
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
        /// Loading ermissionGroup  
        /// </summary>
        /// <param name="command">Grid Command</param>
        /// <param name="searchModel">PermissionGroup SearchModel</param>
        /// <returns>Grid PartialView</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Permission_View")]
        public ActionResult _AjaxList(GridCommand command, PermissionGroupSearchModel searchModel)
        {
            string replaceFrom = "_AjaxList";
            string replaceTo = "List";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<PermissionGroup>(searchStatementModel, command));
        }

        /// <summary>
        ///   Add   PermissionGroup
        /// </summary>
        /// <returns>PermissionGroup  View</returns>
        [SconitAuthorize(Permissions = "Url_Permission_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        ///  Add   PermissionGroup
        /// </summary>
        /// <param name="pgroup">PermissionGroup  Model</param>
        /// <returns>PermissionGroup  View </returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Permission_Edit")]
        public ActionResult New(PermissionGroup pgroup)
        {
            if (ModelState.IsValid)
            {
                ////判断权限名不能重复
                if (this.genericMgr.FindAll<long>(userNameDuiplicateVerifyStatement, new object[] { pgroup.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.ACC.PermissionGroup.Errors_Existing_PermissionGroup, pgroup.Code.ToString());
                }
                else
                {
                    this.genericMgr.CreateWithTrim(pgroup);
                    SaveSuccessMessage(Resources.ACC.PermissionGroup.PermissionGroup_Added);
                    return RedirectToAction("List");
                }
            }
            return View(pgroup);
        }

        /// <summary>
        /// Edit  PermissionGroup
        /// </summary>
        /// <param name="id">PermissionGroup  id</param>
        /// <returns>View  of  Edit  PermissionGroup</returns>
        [SconitAuthorize(Permissions = "Url_Permission_Edit")]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            return View(id);
        }

        /// <summary>
        /// Edit  PermissionGroup
        /// </summary>
        /// <param name="id">PermissionGroup id </param>
        /// <returns>PartialView  of  Edit </returns>
        [SconitAuthorize(Permissions = "Url_Permission_Edit")]
        public ActionResult _Edit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                PermissionGroup pgroup = this.genericMgr.FindById<PermissionGroup>(id);
                return PartialView(pgroup);
            }
        }

        /// <summary>
        /// Update  PermissionGroup
        /// </summary>
        /// <param name="pgroup">PermissionGroup Model</param>
        /// <returns>RouteResult of  Edit</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Permission_Edit")]
        public ActionResult _Edit(PermissionGroup pgroup)
        {
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(pgroup);
                SaveSuccessMessage(Resources.ACC.PermissionGroup.PermissionGroup_Updated);
            }

            return PartialView(pgroup);
        }

        /// <summary>
        /// Delete  PermissionGroup by Id 
        /// </summary>
        /// <param name="id">PermissionGroup Id</param>
        /// <returns>Permission  List  View </returns>
        [SconitAuthorize(Permissions = "Url_Permission_Delete")]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    this.genericMgr.DeleteById<PermissionGroup>(id);
                    SaveSuccessMessage(Resources.ACC.PermissionGroup.PermissionGroup_Deleted);
                    return RedirectToAction("List");
                }
                catch (Exception ex)
                {
                    SaveErrorMessage(((ex.InnerException).InnerException).Message);
                    return RedirectToAction("List");
                }
            }
        }

        /// <summary>
        ///Edit    PermissionGroup  Permission   
        /// </summary>
        /// <param name="id">PermissionGroup  id</param>
        /// <returns>PartialView  of PermissionGroupPermission  </returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Permission_Edit")]
        public ActionResult _PermissionGroupPermission(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                if (!id.HasValue)
                {
                    return HttpNotFound();
                }
                else
                {
                    this.Prepare4AssignPermissiongroupPermissions(id.Value);
                    return PartialView();
                }
            }
        }
        #region PermissionReport
        [SconitAuthorize(Permissions = "Url_PermissionReport_View")]
        public string _GetPermissionReportView(string code, string depart, string position, int searchType)
        {
            string ProcedureName = searchType == 1 ? "USP_Report_MD_PermissionReport_ByUser" : "USP_Report_MD_PermissionReport_ByRole";
            SqlParameter[] sqlParams = new SqlParameter[4];
            sqlParams[0] = new SqlParameter("@Code", code);
            if (searchType == 1)
            {
                sqlParams[1] = new SqlParameter("@Depart", depart);
                sqlParams[2] = new SqlParameter("@Position", position);
            }
            //table returned from SP is a temporary table ,so colculate columns in SP.
            return GetTableHtmlByStoredProcedure(ProcedureName, sqlParams);
        }
        #endregion
        #region Export Permission Report
        [SconitAuthorize(Permissions = "Url_PermissionReport_View")]
        public ActionResult ExportPermissionReport(string code, string depart, string position, int searchType)
        {
            var table = _GetPermissionReportView(code, depart, position, searchType);
            return new DownloadFileActionResult(table, "PermissionReport.xls");
        }
        #endregion
        /// <summary>
        /// Edit PermissionGroup Permission
        /// </summary>
        /// <param name="id">PermissionGroup id</param>
        /// <param name="permissionCategoryCode">permissionCategory Code</param>
        /// <param name="assignedPermissions">Assigned PermissionIds</param>
        /// <returns>the RouteResult</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Permission_Edit")]
        public void _PermissionGroupPermission(int? id, string permissionCategoryCode, IList<int> assignedPermissionIds)
        {
            if (!id.HasValue)
            {
                //return HttpNotFound();
            }
            else
            {
                this.SecurityMgr.UpdatePermissionGroupPermissions(id.Value, permissionCategoryCode, assignedPermissionIds);
                TempData["TabIndex"] = 3;
                SaveSuccessMessage(Resources.ACC.PermissionGroup.PermissionGroup_PermissionAssigned);
                //return new RedirectToRouteResult(new RouteValueDictionary { { "action", "Edit" }, { "controller", "PermissionGroup" }, { "id", id } });
            }
        }

        /// <summary>
        /// Edit   PermissionGroup  Users
        /// </summary>
        /// <param name="id">PermissionGroup  id</param>
        /// <returns>Partial View</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Permission_Edit")]
        public ActionResult _PermissionGroupUsers(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.AssignedUser = this.genericMgr.FindAll<User>(selectUsersByPermissionGroupId, id.Value);
                ViewBag.UnAssignedUser = this.genericMgr.FindAll<User>(selectUsersNotInPermissionGroup, id.Value);
                return PartialView();
            }
        }

        /// <summary>
        /// Update  PermissionGroup  Users
        /// </summary>
        /// <param name="id">PermissionGroup  id</param>
        /// <param name="users">Assigned UserIds</param>
        /// <returns>PermissionGroupUsers  RouteResult</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Permission_Edit")]
        public ActionResult _PermissionGroupUsers(int? id, IList<int> assignedUserIds)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                this.SecurityMgr.UpdatePermissionGroupUsers(id.Value, assignedUserIds);
                TempData["TabIndex"] = 1;
                SaveSuccessMessage(Resources.ACC.PermissionGroup.PermissionGroup_UserAssigned);
                return new RedirectToRouteResult(new RouteValueDictionary { { "action", "Edit" }, { "controller", "PermissionGroup" }, { "id", id } });
            }
        }

        /// <summary>
        ///  the  view  of  Edit  PermissionGroup Role  
        /// </summary>
        /// <param name="id">PermissionGroup Id</param>
        /// <returns>Partial View</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Permission_Edit")]
        public ActionResult _PermissionGroupRole(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.AssignedRoles = this.genericMgr.FindAll<Role>(selectRoleByPermissionGroupId, id.Value);
                ViewBag.UnAssignedRoles = this.genericMgr.FindAll<Role>(selectRolesNotInPermissionGroup, id.Value);
                return PartialView();
            }
        }

        /// <summary>
        /// Update  the  Role  PermissionGroup
        /// </summary>
        /// <param name="id">PermissionGroup id</param>
        /// <param name="roles">Assigned  roles</param>
        /// <returns>PermissionGroupRole  View</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Permission_Edit")]
        public ActionResult _PermissionGroupRole(int? id, IList<int> assignedRoleIds)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                this.SecurityMgr.UpdatePermissionGroupRoles(id.Value, assignedRoleIds);
                TempData["TabIndex"] = 2;
                SaveSuccessMessage(Resources.ACC.PermissionGroup.PermissionGroup_RoleAssigned);
                return new RedirectToRouteResult(new RouteValueDictionary { { "action", "Edit" }, { "controller", "PermissionGroup" }, { "id", id } });
            }
        }

        /// <summary>
        /// Loading Assigned  and  Unassigned    Permissions  by  permissiongroup Id
        /// </summary>
        /// <param name="permissiongroupId">permissiongroup Id</param>
        /// <param name="permissionCategoryCode">Assigned  Permissions</param>
        /// <returns></returns>
        public ActionResult _AjaxLoadingPermissions(int permissiongroupId, string permissionCategoryCode)
        {
            return new JsonResult
            {
                Data = new SelectList[]
                {
                    new SelectList(this.genericMgr.FindAll<Permission>(selectPermissionsNotInPermissionGroupsByCategory , new object[] { permissionCategoryCode, permissiongroupId, permissionCategoryCode }), "Id", "Description"),
                                        new SelectList(this.genericMgr.FindAll<Permission>(selectPermissionsByPermissionGroupIdAndCategory , new object[] { permissionCategoryCode, permissiongroupId }), "Id", "Description")
                }
            };
        }

        /// <summary>
        /// the  Prepare Search Statements  for  PermissionGroup by  Some  Condition
        /// </summary>
        /// <param name="command">Grid Command</param>
        /// <param name="searchModel">PermissionGroup SearchModel</param>
        /// <returns>searchStatementModel  of  PermissionGroup </returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, PermissionGroupSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            if (string.IsNullOrWhiteSpace(sortingStatement))
            {
                searchStatementModel.SortingStatement = " order by u.Code ";
            }
            searchStatementModel.Parameters = param.ToArray<object>();
            return searchStatementModel;
        }

        /// <summary>
        /// Obtain  the    Assigned   and  Unassigned  PermissionsB  by  permissiongroupId
        /// </summary>
        /// <param name="permissiongroupId">permissiongroup Id</param>
        private void Prepare4AssignPermissiongroupPermissions(int permissiongroupId)
        {
            PermissionCategory permissionCategory = Prepare4AssignPermissions();
            if (permissionCategory != null)
            {
                ViewBag.AssignedPermissions = this.genericMgr.FindAll<Permission>(selectPermissionsByPermissionGroupIdAndCategory, new object[] { permissionCategory.Code, permissiongroupId });
                ViewBag.UnAssignedPermissions = this.genericMgr.FindAll<Permission>(selectPermissionsNotInPermissionGroupsByCategory, new object[] { permissionCategory.Code, permissiongroupId, permissionCategory.Code });
            }
        }
    }
}
