namespace com.Sconit.Web.Controllers.BIL
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using com.Sconit.Entity.BIL;
    using com.Sconit.Entity.MD;
    using com.Sconit.Entity.SYS;
    using com.Sconit.Service;
    using com.Sconit.Web.Models;
    using com.Sconit.Web.Models.SearchModels.BIL;
    using com.Sconit.Utility;
    using Telerik.Web.Mvc;
    using com.Sconit.Web.Models.SearchModels.INV;
    using com.Sconit.Entity.INV;
    using System;

    public class MauallySettleCsBillController : WebAppBaseController
    {
        // Fields
        private static string selectCountStatement = "select count(*) from LocationLotDetail as l";
        private static string selectStatement = "from  LocationLotDetail as l";
        // Properties
        public ILocationDetailMgr locationDetailMgr { get; set; }
        public ActionResult Index()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_ProcurementBill_MauallySettleCsBill")]
        public ActionResult List(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
            SearchCacheModel model = base.ProcessSearchModel(command, searchModel);
            if (model.isBack == true)
            {
                ((dynamic)base.ViewBag).Page = (model.Command.Page == 0) ? 1 : model.Command.Page;
            }
            if (!string.IsNullOrEmpty(searchModel.Location))
            {
                base.TempData["_AjaxMessage"] = "";
            }
            else
            {
                base.SaveWarningMessage(Resources.EXT.ControllerLan.Con_LocationSearchConditionCanNotBeEmpty);
            }
            ((dynamic)base.ViewBag).PageSize = base.ProcessPageSize(command.PageSize);
            return base.View();
        }
        // Methods
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_ProcurementBill_MauallySettleCsBill")]
        public ActionResult _AjaxList(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            string sql = PrepareSqlSearchStatement(searchModel);
            int num = base.genericMgr.FindAllWithNativeSql<int>("select count(*) from (" + sql + ") as r1").First<int>();
            string sortingStatement = string.Empty;
            if (command.SortDescriptors.Count != 0)
            {
                if (command.SortDescriptors[0].Member == "UnitCount")
                {
                    command.SortDescriptors[0].Member = "UC";
                }
                else if (command.SortDescriptors[0].Member == "HuStatusOccupyTypeDescription")
                {
                    command.SortDescriptors[0].Member = "OccupyType";
                }
                else if (command.SortDescriptors[0].Member == "HuOptionDesc")
                {
                    command.SortDescriptors[0].Member = "HuOption";
                }
                else if (command.SortDescriptors[0].Member == "QualityTypeDescription")
                {
                    command.SortDescriptors[0].Member = "QualityType";
                }
                else if (command.SortDescriptors[0].Member == "IsConsignment")
                {
                    command.SortDescriptors[0].Member = "IsCS";
                }
                else if (command.SortDescriptors[0].Member == "ManufactureParty")
                {
                    command.SortDescriptors[0].Member = "Party";
                }
                sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
                base.TempData["sortingStatement"] = sortingStatement;
            }
            if (string.IsNullOrWhiteSpace(sortingStatement))
            {
                sortingStatement = "   order by Location,Item,HuId";
            }
            sql = string.Format("select * from (select RowId=ROW_NUMBER()OVER({0}),r1.* from ({1}) as r1 ) as rt where rt.RowId between {2} and {3}", new object[] { sortingStatement, sql, ((command.Page - 1) * command.PageSize) + 1, command.PageSize * command.Page });
            IList<object[]> list = base.genericMgr.FindAllWithNativeSql<object[]>(sql);
            IList<LocationLotDetail> list2 = new List<LocationLotDetail>();
            if ((list != null) && (list.Count > 0))
            {
                list2 = (from tak in list
                         select new LocationLotDetail
                         {
                             HuId = (string)tak[1],
                             LotNo = (string)tak[2],
                             Location = (string)tak[3],
                             Bin = (string)tak[4],
                             Item = (string)tak[5],
                             ItemDescription = (string)tak[6],
                             ReferenceItemCode = (string)tak[7],
                             UnitCount = (decimal)tak[8],
                             IsConsignment = (bool)tak[11],
                             IsFreeze = (bool)tak[12],
                             IsATP = (bool)tak[13],
                             HuQty = (decimal)tak[14],
                             HuUom = (string)tak[15],
                             Qty = (decimal)tak[16],
                             BaseUom = (string)tak[17],
                             ManufactureParty = (string)tak[24],
                             Id = (int)tak[25]
                         }).ToList<LocationLotDetail>();
            }
            GridModel<LocationLotDetail> model = new GridModel<LocationLotDetail>
            {
                Total = num,
                Data = list2
            };
            return base.PartialView(model);
        }

        private string PrepareSqlSearchStatement(LocationLotDetailSearchModel searchModel)
        {
            string str = @"Select isnull(a.HuId,'') HuId,isnull(a.LotNo,'') LotNo,a.Location,isnull(a.Bin,'') Bin,                    a.Item, c.Desc1,c.RefCode,Isnull(a.UC,0) UC,a.OccupyType,a.QualityType,a.IsCS,                     a.IsFreeze,a.IsATP,isnull(a.HuQty,0)HuQty,isnull(a.HuUom,'')HuUom,a.Qty,isnull (a.BaseUom,'')BaseUom,  '' As HuOption,a.Direction,'' as DirectionDesc,                    '' as MaterialsGroup,'' as MaterialsGroupDesc,'' As ItemVersion,b.Party As Party ,                    a.Id from VIEW_LocationLotDet a with(nolock),BIL_PlanBill b                     with(nolock),  MD_Item c with(nolock),MD_Location d with                    (nolock)  where a.PlanBill =b.Id and a.Item =c.Code and                     a.Location =d.Code  and a.IsCS=1 ";
            if (!string.IsNullOrEmpty(searchModel.Location))
            {
                str = str + string.Format(" and d.Code = '{0}'", searchModel.Location);
                if (searchModel.Location == "9103")
                {
                    str = str + " and b.Party in ('30003','30037','30029','30038','30023','30010','30224','30008')";
                }
            }
            else
            {
                str = str + " and 1=2 ";
            }
            if (!string.IsNullOrEmpty(searchModel.Party))
            {
                str = str + string.Format(" and b.Party = '{0}'", searchModel.Party);
            }
            if (!string.IsNullOrEmpty(searchModel.Item))
            {
                str = str + string.Format(" and c.Code = '{0}'", searchModel.Item);
            }
            if (!string.IsNullOrEmpty(searchModel.HuId))
            {
                str = str + string.Format(" and a.HuId = '{0}'", searchModel.HuId);
            }
            return str;
        }

        [HttpPost]
        public JsonResult SettleCsBill(string checkedLocDetId)
        {
            string message = Resources.EXT.ControllerLan.Con_SettledSuccessfully;
            try
            {
                string[] strArray = checkedLocDetId.Split(new char[] { ',' });
                List<long> locationLotDetIdList = new List<long>();
                foreach (string str3 in strArray)
                {
                    locationLotDetIdList.Add((long)int.Parse(str3));
                }
                this.locationDetailMgr.SettleLocaitonLotDetail(locationLotDetIdList);
            }
            catch (Exception exception)
            {
                base.SaveErrorMessage(exception);
                message = exception.Message;
            }
            return base.Json(new { Message = message });
        }
    }
}
