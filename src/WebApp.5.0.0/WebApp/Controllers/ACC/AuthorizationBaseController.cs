
namespace com.Sconit.Web.Controllers.ACC
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using com.Sconit.Entity.ACC;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;

    public class AuthorizationBaseController : WebAppBaseController
    {
        public ISecurityMgr securityMgr { get; set; }

        protected static string selectPermissionCategoryStatement = "select pc from PermissionCategory as pc where pc.Type = ? order by pc.Sequence";

        public ActionResult _AjaxLoadingPermissionCategory(com.Sconit.CodeMaster.PermissionCategoryType permissionCategoryType)
        {
            IList<PermissionCategory> permissionCategoryList = this.queryMgr.FindAll<PermissionCategory>(selectPermissionCategoryStatement, permissionCategoryType);
            if (permissionCategoryList == null)
            {
                permissionCategoryList = new List<PermissionCategory>();
            }

            this.TranslatePermissionCategory(permissionCategoryList);

            return new JsonResult { Data = new SelectList(permissionCategoryList, "Code", "Description") };
        }

        protected PermissionCategory Prepare4AssignPermissions()
        {
            IList<CodeDetail> codeDetailList = systemMgr.GetCodeDetails(com.Sconit.CodeMaster.CodeMaster.PermissionCategoryType);
            ViewBag.PermissionCategoryTypeList = base.Transfer2DropDownList(com.Sconit.CodeMaster.CodeMaster.PermissionCategoryType, codeDetailList);

            IList<PermissionCategory> permissionCategoryList = this.queryMgr.FindAll<PermissionCategory>(selectPermissionCategoryStatement, codeDetailList[0].Value);
            if (permissionCategoryList == null)
            {
                permissionCategoryList = new List<PermissionCategory>();
            }

            this.TranslatePermissionCategory(permissionCategoryList);

            //todo permissioncategory多语言
            ViewBag.PermissionCategoryCodeList = new SelectList(permissionCategoryList, "Code", "Description");

            return permissionCategoryList.Count > 0 ? permissionCategoryList[0] : null;
        }

        protected void TranslatePermission(IList<Permission> permissionList)
        {
            this.securityMgr.TranslatePermission(permissionList);
        }

        protected void TranslatePermissionCategory(IList<PermissionCategory> permissionCategoryList)
        {
            this.securityMgr.TranslatePermissionCategory(permissionCategoryList);
        }
    }
}
