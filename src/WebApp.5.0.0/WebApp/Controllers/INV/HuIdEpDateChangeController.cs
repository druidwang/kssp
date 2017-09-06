using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Service;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models;
using com.Sconit.Entity.MD;
using NHibernate.Type;
using com.Sconit.Entity.Exception;
using com.Sconit.Utility;
using com.Sconit.Web.Models.SearchModels.MD;
using com.Sconit.Entity.SCM;
using com.Sconit.Entity.INV;
using com.Sconit.Web.Models.SearchModels.INV;
using com.Sconit.Entity.ACC;

namespace com.Sconit.Web.Controllers.INV
{
    public class HuIdEpDateChangeController : WebAppBaseController
    {

        private static string selectCountStatement = "select count(*) from Hu as f";

        private static string selectStatement = "from Hu as f";

        public ILocationDetailMgr locationMgr { get; set; }

        #region  public
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_CUST_HuIdEpDateChange_View")]
        [GridAction]
        public ActionResult List(GridCommand command, HuTransSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            ViewBag.PageSize = base.ProcessPageSize(command.PageSize);
            return View();
        }

        [SconitAuthorize(Permissions = "Url_CUST_HuIdEpDateChange_View")]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxList(GridCommand command, HuTransSearchModel searchModel)
        {
            if (string.IsNullOrWhiteSpace(searchModel.Item))
            {
                searchModel.Item = "";
            }
            if (string.IsNullOrWhiteSpace(searchModel.Location))
            {
                searchModel.Location = "";
            }
            TempData["GridCommand"] = command;
            TempData["searchModel"] = searchModel;
            string isIncludeAllSelect = "";
            if (!searchModel.IsIncludeAll)
            {
                isIncludeAllSelect = "(a.ExpireDate <GETDATE() and  a.ExpireDate is not null) and";
            }
            string sql = @"Select a.HuId,b.Qty,a.Item ,a.RefItemCode As ReferenceItemCode,a.ItemDesc As ItemDescription ,
                    a.LotNo,b.Location,a.UC As UnitCount,a.Uom, dbo.FormatDate(a.ExpireDate,'YYYY-MM-DD') As ExpireDateValue,
                    Case when a.RemindExpireDate is null then '' else dbo.FormatDate(a.RemindExpireDate,'YYYY-MM-DD') End As RemindExpireDateValue from INV_Hu a ,VIEW_LocationLotDet b
					where a.HuId =b.HuId and " + isIncludeAllSelect + " a.HuId is not null and " +
                    "a.Item like '" + searchModel.Item + "%' and b.Location like '" + searchModel.Location + "%'and a.LotNo between '" + searchModel.LotNo + "' and '" + searchModel.LotNoTo + "'";
            int total = this.genericMgr.FindAllWithNativeSql<int>("select count(*) from (" + sql + ") as r1").First();
            string sortingStatement = " Order by r1.Location,r1.Item";
            sql = string.Format("select * from (select RowId=ROW_NUMBER()OVER({0}),r1.* from ({1}) as r1 ) as rt where rt.RowId between {2} and {3}", sortingStatement, sql, (command.Page - 1) * command.PageSize + 1, command.PageSize * command.Page);
            var ds = this.genericMgr.GetDatasetBySql(sql, null);
            var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();
            IList<Hu> huTypeList = Utility.IListHelper.DataTableToList<Hu>(ds.Tables[0]);
            foreach (var huData in huTypeList)
            {
                huData.MaterialsGroup = itemMgr.GetCacheItem(huData.Item).MaterialsGroup;
                huData.MaterialsGroupDesc = GetItemCategory(huData.MaterialsGroup, itemCategoryList).Description;
            }
            GridModel<Hu> gridModel = new GridModel<Hu>();
            gridModel.Total = total;
            gridModel.Data = huTypeList;
            return PartialView(gridModel);
        }

        [GridAction]
        public ActionResult _Update(Hu Hu)
        {
            if (Hu.ExpireDateValue == null)
            {
                SaveErrorMessage(Resources.EXT.ControllerLan.Con_OverTimeTimeCanNotBeEmpty);
            }
            //else if (DateTime.Parse(Hu.ExpireDateValue) < DateTime.Now)
            //{
            //    SaveErrorMessage("过期时间不能小于当前日期。");
            //}
            else
            {
                Hu upHu = base.genericMgr.FindById<Hu>(Hu.HuId);
                DateTime oldEptime = upHu.ExpireDate.Value;
                upHu.ExpireDate = DateTime.Parse(Hu.ExpireDateValue);
                this.genericMgr.Update(upHu);
                SaveSuccessMessage(Resources.EXT.ControllerLan.Con_ModificateSuccessfully);
                RecordModifyExpiredDateTransaction(Hu, oldEptime);
            }
            GridCommand command = (GridCommand)TempData["GridCommand"];
            HuTransSearchModel searchModel = (HuTransSearchModel)TempData["searchModel"];
            TempData["GridCommand"] = command;
            TempData["searchModel"] = searchModel;
            string isIncludeAllSelect = "";
            if (!searchModel.IsIncludeAll)
            {
                isIncludeAllSelect = "(a.ExpireDate <GETDATE() and  a.ExpireDate is not null) and";
            }
            string sql = @"Select a.HuId,b.Qty,a.Item ,a.RefItemCode As ReferenceItemCode,a.ItemDesc As ItemDescription ,
                    a.LotNo,b.Location,a.UC As UnitCount,a.Uom, dbo.FormatDate(a.ExpireDate,'YYYY-MM-DD') As ExpireDateValue,
                    Case when a.RemindExpireDate is null then '' else dbo.FormatDate(a.RemindExpireDate,'YYYY-MM-DD') End As RemindExpireDateValue from INV_Hu a ,VIEW_LocationLotDet b
					where a.HuId =b.HuId and " + isIncludeAllSelect + " a.HuId is not null and " +
                    "a.Item like '" + searchModel.Item + "%' and b.Location like '" + searchModel.Location + "%'and a.LotNo between '" + searchModel.LotNo + "' and '" + searchModel.LotNoTo + "'";
            int total = this.genericMgr.FindAllWithNativeSql<int>("select count(*) from (" + sql + ") as r1").First();
            string sortingStatement = " Order by r1.Location,r1.Item";
            sql = string.Format("select * from (select RowId=ROW_NUMBER()OVER({0}),r1.* from ({1}) as r1 ) as rt where rt.RowId between {2} and {3}", sortingStatement, sql, (command.Page - 1) * command.PageSize + 1, command.PageSize * command.Page);
            var ds = this.genericMgr.GetDatasetBySql(sql, null);
            var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();
            IList<Hu> huTypeList = Utility.IListHelper.DataTableToList<Hu>(ds.Tables[0]);
            foreach (var huData in huTypeList)
            {
                huData.MaterialsGroup = itemMgr.GetCacheItem(huData.Item).MaterialsGroup;
                huData.MaterialsGroupDesc = GetItemCategory(huData.MaterialsGroup, itemCategoryList).Description;
            }
            GridModel<Hu> gridModel = new GridModel<Hu>();
            gridModel.Total = total;
            gridModel.Data = huTypeList;
            return PartialView(gridModel);
        }
        #region RecordModifyExpiredDateTrans
        private void RecordModifyExpiredDateTransaction(Hu huId, DateTime oldEptime)
        {
            User user = com.Sconit.Entity.SecurityContextHolder.Get();
            HuIdEpDateChangeTransaction huIdEpDateChangeTransaction = new HuIdEpDateChangeTransaction();
            huIdEpDateChangeTransaction.CreateDate = DateTime.Now;
            huIdEpDateChangeTransaction.CreateUserName = user.FullName;
            huIdEpDateChangeTransaction.CreateUserId = user.Id;
            huIdEpDateChangeTransaction.HuId = huId.HuId;
            huIdEpDateChangeTransaction.LotNo = huId.LotNo;
            huIdEpDateChangeTransaction.Bin = huId.Bin;
            huIdEpDateChangeTransaction.Item = huId.Item;
            huIdEpDateChangeTransaction.OldExpiredDate = oldEptime;
            huIdEpDateChangeTransaction.NewExpiredDate = DateTime.Parse(huId.ExpireDateValue);
            huIdEpDateChangeTransaction.Location = huId.Location;
            this.genericMgr.Create(huIdEpDateChangeTransaction);
        }
        #endregion
        #endregion

    }
}
