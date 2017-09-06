/*********************************
 * 
 * 
 * *******************************/
namespace com.Sconit.Web.Controllers.ACC
{
    using System.Collections;
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

    /// <summary>
    /// RoleController
    /// </summary>

    public class RoleController : AuthorizationBaseController
    {
        /// <summary>
        /// select  the  count of  role 
        /// </summary>
        private static string selectCountStatement = "select count(*) from Role as u";

        /// <summary>
        /// select whole  role 
        /// </summary>
        private static string selectStatement = "select u from Role as u";

        /// <summary>
        /// select  role  count  by  RoleCode
        /// </summary>
        private static string roleDuiplicateVerifyStatement = @"select count(*) from Role as u where u.Code = ?";

        /// <summary>
        /// select  PermissionGroup By RoleId
        /// </summary>
        private static string selectPermissionGroupByRoleId = "select rp.PermissionGroup from RolePermissionGroup as rp where rp.Role.Id = ? order by Code";

        /// <summary>
        ///  select PermissionGroups Not In Role
        /// </summary>
        private static string selectPermissionGroupsNotInRole = @"select p from PermissionGroup as p where p.Id not in 
            (select  rp.PermissionGroup.Id from RolePermissionGroup as rp where rp.Role.Id = ?)";

        /// <summary>
        /// select Users By RoleId
        /// </summary>
        private static string selectUsersByRoleId = "select ur.User from UserRole as ur where ur.Role.Id = ?";

        /// <summary>
        /// select Users  Not In  Role
        /// </summary>
        private static string selectUsersNotInRole = @"select u from User as u where u.Id not in 
            (select ur.User.Id from UserRole as ur where ur.Role.Id = ?)";

        /// <summary>
        /// select  Permissions  By  RoleIdAndCategory
        /// </summary>
        private static string selectPermissionsByRoleIdAndCategory = @"select p from RolePermission as rp 
            join rp.Permission as p where p.PermissionCategory = ? and rp.Role.Id = ? and p.Sequence>=0 order by p.Sequence,p.Code";

        /// <summary>
        /// select Permissions Not InRole ByCategory
        /// </summary>
        private static string selectPermissionsNotInRoleByCategory = @"select p from Permission as p
             where p.PermissionCategory = ? and p.Id not in (select p.Id  from RolePermission as rp 
             join rp.Permission as p where rp.Role.Id = ? and p.PermissionCategory = ?) and p.Sequence>=0 order by p.Sequence,p.Code";

        /// <summary>
        /// genericMgr
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }

        /// <summary>
        /// View  Role
        /// </summary>
        /// <returns>View</returns>
        [SconitAuthorize(Permissions = "Url_Role_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// select  Role  List  
        /// </summary>
        /// <param name="command">Grid Command</param>
        /// <param name="searchModel">Role SearchModel</param>
        /// <returns>View</returns>
        [GridAction]
        [SconitAuthorize(Permissions = "Url_Role_View")]
        public ActionResult List(GridCommand command, RoleSearchModel searchModel)
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
        /// Refresh   Role  List   
        /// </summary>
        /// <param name="command">GridCommand</param>
        /// <param name="searchModel">RoleSearchModel</param>
        /// <returns>PartialView</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Role_View")]
        public ActionResult _AjaxList(GridCommand command, RoleSearchModel searchModel)
        {
            string replaceFrom = "_AjaxList";
            string replaceTo = "List";
            this.GetCommand(ref command, searchModel, replaceFrom, replaceTo);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<Role>(searchStatementModel, command));
        }

        /// <summary>
        /// New  Role  View 
        /// </summary>
        /// <returns>View</returns>
        [SconitAuthorize(Permissions = "Url_Role_Edit")]
        public ActionResult New()
        {
            ViewBag.NewType = com.Sconit.CodeMaster.RoleType.Normal.ToString();
            return View();
        }

        /// <summary>
        /// Add   New  Role
        /// </summary>
        /// <param name="role">Role Model</param>
        /// <returns>View</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Role_Edit")]
        public ActionResult New(Role role)
        {
            if (ModelState.IsValid)
            {
                ////判断描述不能重复
                if (this.genericMgr.FindAll<long>(roleDuiplicateVerifyStatement, new object[] { role.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.ACC.Role.Errors_Existing_Role, role.Code);
                }
                else
                {
                    this.genericMgr.CreateWithTrim(role);
                    SaveSuccessMessage(Resources.ACC.Role.Role_Added);
                    return RedirectToAction("Edit/" + role.Id);
                }
            }

            return View(role);
        }

        /// <summary>
        /// Edit  role  view
        /// </summary>
        /// <param name="id">role id</param>
        /// <returns>View</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Role_Edit")]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }

            return View(id);
        }

        /// <summary>
        /// Edit  role  
        /// </summary>
        /// <param name="id">Role id </param>
        /// <returns>Partial View</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Role_Edit")]
        public ActionResult _Edit(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                Role role = this.genericMgr.FindById<Role>(id);
                return PartialView(role);
            }
        }

        /// <summary>
        /// Edit  role  
        /// </summary>
        /// <param name="role">Role Model</param>
        /// <returns>RouteResult</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Role_Edit")]
        public ActionResult _Edit(Role role)
        {
            if (string.IsNullOrWhiteSpace(role.Code ) )
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_TheRoleCodeCanNotBeEmpty);
                return PartialView(role);
            }
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(role);
                SaveSuccessMessage(Resources.ACC.Role.Role_Updated);
            }
            return PartialView(role);
        }

        /// <summary>
        ///Show  the Assigned and Unassigned PermissionGroups by roleid
        /// </summary>
        /// <param name="id">role id</param>
        /// <returns>PartialView</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Role_Edit")]
        public ActionResult _RolePermissionGroups(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.AssignedPermissionGroup = this.genericMgr.FindAll<PermissionGroup>(selectPermissionGroupByRoleId, id.Value);
                ViewBag.UnAssignedPermissionGroup = this.genericMgr.FindAll<PermissionGroup>(selectPermissionGroupsNotInRole, id.Value);
                return PartialView();
            }
        }

        /// <summary>
        /// Update Role  PermissionGroups
        /// </summary>
        /// <param name="id">role  id</param>
        /// <param name="assignedRoleIds">Assigned  RoleIds</param>
        /// <returns>RouteResul</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Role_Edit")]
        public ActionResult _RolePermissionGroups(int? id, IList<int> assignedRoleIds)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                this.securityMgr.UpdateRolePermissionGroups(id.Value, assignedRoleIds);
                TempData["TabIndex"] = 2;
                SaveSuccessMessage(Resources.ACC.Role.Role_PermissionGroupAssigned);
                return new RedirectToRouteResult(new RouteValueDictionary { { "action", "Edit" }, { "controller", "Role" }, { "id", id } });
            }
        }

        /// <summary>
        /// Show  the  Assigned   and  Unassigned Users  by  roleid
        /// </summary>
        /// <param name="id">role id</param>
        /// <returns>PartialView</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Role_Edit")]

        public ActionResult _RoleUsers(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.AssignedUsers = this.genericMgr.FindAll<User>(selectUsersByRoleId, id.Value);
                ViewBag.UnAssignedUsers = this.genericMgr.FindAll<User>(selectUsersNotInRole, id.Value);
                return PartialView();
            }
        }

        /// <summary>
        ///  Update Role  Users
        /// </summary>
        /// <param name="id">role  id</param>
        /// <param name="assignedRoleIds"> Assigned RoleIds</param>
        /// <returns>RouteResult</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Role_Edit")]
        public ActionResult _RoleUsers(int? id, IList<int> assignedUserIds)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                this.securityMgr.UpdateRoleUsers(id.Value, assignedUserIds);
                TempData["TabIndex"] = 1;
                SaveSuccessMessage(Resources.ACC.Role.Role_UserAssigned);
                return new RedirectToRouteResult(new RouteValueDictionary { { "action", "Edit" }, { "controller", "Role" }, { "id", id } });
            }
        }

        /// <summary>
        /// Show  Permissions  by role id
        /// </summary>
        /// <param name="id">role id</param>
        /// <returns>PartialView</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Role_Edit")]
        public ActionResult _RolePermissions(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {

                this.Prepare4AssignUserPermissions(id.Value);
                return PartialView();
            }
        }

        /// <summary>
        /// Update Role  Permissions 
        /// </summary>
        /// <param name="id">role id</param>
        /// <param name="permissionCategoryCode">permissionCategoryCode</param>
        /// <param name="assignedPermissionIds">Assigned PermissionIds</param>
        /// <returns>RouteResult</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Role_Edit")]
        public void _RolePermissions(int? id, string permissionCategoryCode, IList<int> assignedPermissionIds)
        {
            if (!id.HasValue)
            {
                //return HttpNotFound();
            }
            else
            {
                TempData["TabIndex"] = 3;
                this.securityMgr.UpdateRolePermissions(id.Value, permissionCategoryCode, assignedPermissionIds);
                SaveSuccessMessage(Resources.ACC.Role.Role_PermissionAssigned);
                //return View();
                //return new RedirectToRouteResult(new RouteValueDictionary { { "action", "Edit" }, { "controller", "Role" }, { "id", id } });
            }
        }

        /// <summary>
        /// delete  role by  roleid 
        /// </summary>
        /// <param name="id">role id</param>
        /// <returns>Delete  Role  View </returns>
        [SconitAuthorize(Permissions = "Url_Role_Delete")]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<Role>(id);
                SaveSuccessMessage(Resources.ACC.Role.Role_Deleted);
                return RedirectToAction("List");
            }
        }

        /// <summary>
        /// Loading  Permissions
        /// </summary>
        /// <param name="roleId">role Id</param>
        /// <param name="permissionCategoryCode">permissionCategory Code</param>
        /// <returns>Json Result  of Assigned  and  Unassigned  Permissions </returns>
        public ActionResult _AjaxLoadingPermissions(int roleId, string permissionCategoryCode)
        {
            return new JsonResult
            {
                Data = new SelectList[] { new SelectList(this.genericMgr.FindAll<Permission>(selectPermissionsNotInRoleByCategory, new object[] { permissionCategoryCode, roleId, permissionCategoryCode }), "Id", "Description"), new SelectList(this.genericMgr.FindAll<Permission>(selectPermissionsByRoleIdAndCategory, new object[] { permissionCategoryCode, roleId }), "Id", "Description") }
            };
        }

        /// <summary>
        /// the  method  for  Serach Roles  by  Condition
        /// </summary>
        /// <param name="command">GridCommand</param>
        /// <param name="searchModel">RoleSearchModel</param>
        /// <returns>searchStatementModel</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, RoleSearchModel searchModel)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();
            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Type", searchModel.Type, "u", ref whereStatement, param);

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
        /// Prepare Assign  RolePermissions
        /// </summary>
        /// <param name="roleId">role id </param>
        private void Prepare4AssignUserPermissions(int roleId)
        {
            PermissionCategory permissionCategory = Prepare4AssignPermissions();
            if (permissionCategory != null)
            {
                ViewBag.AssignedPermissions = this.genericMgr.FindAll<Permission>(selectPermissionsByRoleIdAndCategory, new object[] { permissionCategory.Code, roleId });
                ViewBag.UnAssignedPermissions = this.genericMgr.FindAll<Permission>(selectPermissionsNotInRoleByCategory, new object[] { permissionCategory.Code, roleId, permissionCategory.Code });
            }
        }
    }
}
