/// <summary>
/// Summary description for AddressController
/// </summary>
namespace com.Sconit.Web.Controllers.MD
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
    #endregion

    /// <summary>
    /// This controller response to control the Address.
    /// </summary>
    public class AddressController : WebAppBaseController
    {
        #region Properties
        /// <summary>
        /// Gets or sets the this.GeneMgr which main consider the address security 
        /// </summary>
        //public IGenericMgr genericMgr { get; set; }
        #endregion

        /// <summary>
        /// hql to get count of the address
        /// </summary>
        private static string selectCountStatement = "select count(*) from Address as u";

        /// <summary>
        /// hql to get all of the address
        /// </summary>
        private static string selectStatement = "select u from Address as u";

        /// <summary>
        /// hql to get count of the address by address's code
        /// </summary>
        private static string duiplicateVerifyStatement = @"select count(*) from Address as u where u.Code = ?";

        #region public actions
        /// <summary>
        /// Index action for Address controller
        /// </summary>
        /// <returns>Index view</returns>
        [SconitAuthorize(Permissions = "Url_Address_View")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// List action
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Address Search model</param>
        /// <returns>return the result view</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Address_View")]
        public ActionResult List(GridCommand command, AddressSearchModel searchModel)
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
        /// <param name="searchModel">Address Search Model</param>
        /// <returns>return the result action</returns>
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_Address_View")]
        public ActionResult _AjaxList(GridCommand command, AddressSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            SearchStatementModel searchStatementModel = this.PrepareSearchStatement(command, searchModel);
            return PartialView(GetAjaxPageData<Address>(searchStatementModel, command));
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <returns>New view</returns>
        [SconitAuthorize(Permissions = "Url_Address_Edit")]
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// New action
        /// </summary>
        /// <param name="address">Address Model</param>
        /// <returns>return the result view</returns>
        [HttpPost]
        [SconitAuthorize(Permissions = "Url_Address_Edit")]
        public ActionResult New(Address address)
        {
            if (ModelState.IsValid)
            {
                if (this.genericMgr.FindAll<long>(duiplicateVerifyStatement, new object[] { address.Code })[0] > 0)
                {
                    SaveErrorMessage(Resources.SYS.ErrorMessage.Errors_Existing_Code, address.Code);
                }
                else
                {
                    this.genericMgr.CreateWithTrim(address);
                    SaveSuccessMessage(Resources.MD.Address.Address_Added);
                    return RedirectToAction("Edit/" + address.Code);
                }
            }

            return View(address);
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id">address id for edit</param>
        /// <returns>return the result view</returns>
        [HttpGet]
        [SconitAuthorize(Permissions = "Url_Address_View")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                Address address = this.genericMgr.FindById<Address>(id);
                return View(address);
            }
        }

        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="address">Address Model</param>
        /// <returns>return the result view</returns>
        [SconitAuthorize(Permissions = "Url_Address_Edit")]
        public ActionResult Edit(Address address)
        {
            if (ModelState.IsValid)
            {
                this.genericMgr.UpdateWithTrim(address);
                SaveSuccessMessage(Resources.MD.Address.Address_Updated);
            }

            return View(address);
        }

        /// <summary>
        /// Delete action
        /// </summary>
        /// <param name="id">address id for delete</param>
        /// <returns>return to List action</returns>
        [SconitAuthorize(Permissions = "Url_Address_Delete")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            else
            {
                this.genericMgr.DeleteById<Address>(id);
                SaveSuccessMessage(Resources.MD.Address.Address_Deleted);
                return RedirectToAction("List");
            }
        }
        #endregion

        /// <summary>
        /// Search Statement
        /// </summary>
        /// <param name="command">Telerik GridCommand</param>
        /// <param name="searchModel">Address Search Model</param>
        /// <returns>return address search model</returns>
        private SearchStatementModel PrepareSearchStatement(GridCommand command, AddressSearchModel searchModel)
        {            
            string whereStatement = string.Empty;

            IList<object> param = new List<object>();

            HqlStatementHelper.AddLikeStatement("Code", searchModel.Code, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("AddressContent", searchModel.AddressContent, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("PostCode", searchModel.PostCode, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("TelPhone", searchModel.TelPhone, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("MobilePhone", searchModel.MobilePhone, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Fax", searchModel.Fax, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("Email", searchModel.Email, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("ContactPersonName", searchModel.ContactPersonName, HqlStatementHelper.LikeMatchMode.Start, "u", ref whereStatement, param);
            //HqlStatementHelper.AddEqStatement("Type", searchModel.Type, "u", ref whereStatement, param);

            //if (command.SortDescriptors.Count > 0)
            //{
            //    if (command.SortDescriptors[0].Member == "AddressTypeDescription")
            //    {
            //        command.SortDescriptors[0].Member = "Type";
            //    }
            //}
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
