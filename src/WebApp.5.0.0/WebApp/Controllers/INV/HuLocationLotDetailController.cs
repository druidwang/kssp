using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Service;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models.SearchModels.INV;
using com.Sconit.Entity.INV;
using com.Sconit.Web.Models;
using com.Sconit.Utility;
using com.Sconit.Entity.MRP.MD;
using com.Sconit.Entity.SYS;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using com.Sconit.Entity.MD;

namespace com.Sconit.Web.Controllers.INV
{
    public class HuLocationLotDetailController : WebAppBaseController
    {
        private static string selectCountStatement = "select count(*) from LocationLotDetail as l";
        private static string selectStatement = "from  LocationLotDetail as l";

        private static string selectHuTransCountStatement = "select count(*) from PickTransaction as l";
        private static string selectHuTransStatement = "from  PickTransaction as l";

        public ActionResult Index()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Url_HuTrans_View")]
        public ActionResult HuTrans()
        {
            return View();
        }
        [SconitAuthorize(Permissions = "Menu.Hu.Inventory")]
        public ActionResult List(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (searchCacheModel.isBack == true)
            {
                ViewBag.Page = searchCacheModel.Command.Page == 0 ? 1 : searchCacheModel.Command.Page;
            }
            ViewBag.PageSize = this.ProcessPageSize(command.PageSize);
            return View();
        }

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Menu.Hu.Inventory")]
        public ActionResult _AjaxList(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
            this.GetCommand(ref command, searchModel);
            string sql = PrepareSqlSearchStatement(searchModel);
            int total = this.genericMgr.FindAllWithNativeSql<int>("select count(*) from (" + sql + ") as r1").First();
            string sortingStatement = string.Empty;

            #region
            ////ld.HuId,ld.LotNo,ld.Location,ld.Bin,ld.Item,i.Desc1,i.RefCode,ld.UC,ld.OccupyType,ld.QualityType,ld.IsCS,
            //ld.IsFreeze,ld.IsATP,ld.HuQty,ld.HuUom,ld.Qty,ld.BaseUom,hu.HuOption,ld.Direction,ht.Desc1
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
                sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
                TempData["sortingStatement"] = sortingStatement;
            }
            #endregion

            if (string.IsNullOrWhiteSpace(sortingStatement))
            {
                sortingStatement = "   order by Location,Item,LotNo,HuId,Direction";
            }
            sql = string.Format("select * from (select RowId=ROW_NUMBER()OVER({0}),r1.* from ({1}) as r1 ) as rt where rt.RowId between {2} and {3}", sortingStatement, sql, (command.Page - 1) * command.PageSize + 1, command.PageSize * command.Page);
            IList<object[]> searchResult = this.genericMgr.FindAllWithNativeSql<object[]>(sql);
            IList<LocationLotDetail> locationLotDetailList = new List<LocationLotDetail>();
            if (searchResult != null && searchResult.Count > 0)
            {
                #region
                //var productTypes = genericMgr.FindAll<ProductType>().ToDictionary(d => d.Code, d => d);
                //ld.HuId,ld.LotNo,ld.Location,ld.Bin,ld.Item,i.Desc1,i.RefCode,ld.UC,ld.OccupyType,ld.QualityType,ld.IsCS,
                //ld.IsFreeze,ld.IsATP,ld.HuQty,ld.HuUom,ld.Qty,ld.BaseUom,hu.HuOption,ld.Direction,ht.Desc1
                locationLotDetailList = (from tak in searchResult
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
                                             //OccupyType = (string)tak[9],
                                             HuStatusOccupyTypeDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.OccupyType, int.Parse((tak[9]).ToString())),
                                             //QualityType = (string)tak[10],
                                             QualityTypeDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.QualityType, int.Parse((tak[10]).ToString())),
                                             IsConsignment = (bool)tak[11],
                                             IsFreeze = (bool)tak[12],
                                             IsATP = (bool)tak[13],
                                             HuQty = (decimal)tak[14],
                                             HuUom = (string)tak[15],
                                             Qty = (decimal)tak[16],
                                             BaseUom = (string)tak[17],
                                             HuOptionDesc = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.HuOption, int.Parse((tak[18]).ToString())),
                                             Direction = (string)tak[19],
                                             DirectionDescription = (string)tak[20],
                                             MaterialsGroup = (string)tak[21],
                                             MaterialsGroupDesc = (string)tak[22],
                                             ItemVersion = (string)tak[23], //+ (string.IsNullOrWhiteSpace((string)tak[23]) ? "" : "[" + productTypes.ValueOrDefault((string)tak[23]).Description + "]")
                                             Remark = (string)tak[24],
                                             ManufactureParty = (string)tak[25]
                                         }).ToList();
                #endregion
            }
            GridModel<LocationLotDetail> gridModel = new GridModel<LocationLotDetail>();
            gridModel.Total = total;
            gridModel.Data = locationLotDetailList;
            return PartialView(gridModel);
        }
        #region
        public void ExportXLS(LocationLotDetailSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;

            string sql = PrepareSqlSearchStatement(searchModel);
            string sortingStatement = TempData["sortingStatement"] as string;
            TempData["sortingStatement"] = sortingStatement;
            if (string.IsNullOrWhiteSpace(sortingStatement))
            {
                sortingStatement = "   order by Location,Item,LotNo,HuId,Direction";
            }
            IList<object[]> searchResult = this.genericMgr.FindAllWithNativeSql<object[]>("select top " + value + " 1,r.* from (" + sql + ") as r ");
            IList<LocationLotDetail> locationLotDetailList = new List<LocationLotDetail>();
            if (searchResult != null && searchResult.Count > 0)
            {
                #region
                //var productTypes = genericMgr.FindAll<ProductType>().ToDictionary(d => d.Code, d => d);
                //ld.HuId,ld.LotNo,ld.Location,ld.Bin,ld.Item,i.Desc1,i.RefCode,ld.UC,ld.OccupyType,ld.QualityType,ld.IsCS,
                //ld.IsFreeze,ld.IsATP,ld.HuQty,ld.HuUom,ld.Qty,ld.BaseUom,hu.HuOption,ld.Direction,ht.Desc1
                locationLotDetailList = (from tak in searchResult
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
                                             //OccupyType = (string)tak[9],
                                             HuStatusOccupyTypeDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.OccupyType, int.Parse((tak[9]).ToString())),
                                             //QualityType = (string)tak[10],
                                             QualityTypeDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.QualityType, int.Parse((tak[10]).ToString())),
                                             IsConsignment = (bool)tak[11],
                                             IsFreeze = (bool)tak[12],
                                             IsATP = (bool)tak[13],
                                             HuQty = (decimal)tak[14],
                                             HuUom = (string)tak[15],
                                             Qty = (decimal)tak[16],
                                             BaseUom = (string)tak[17],
                                             HuOptionDesc = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.HuOption, int.Parse((tak[18]).ToString())),
                                             Direction = (string)tak[19],
                                             DirectionDescription = (string)tak[20],
                                             MaterialsGroup = (string)tak[21],
                                             MaterialsGroupDesc = (string)tak[22],
                                             ItemVersion = (string)tak[23], //+ (string.IsNullOrWhiteSpace((string)tak[23]) ? "" : "[" + productTypes.ValueOrDefault((string)tak[23]).Description + "]")
                                             Remark = (string)tak[24],
                                             ManufactureParty = (string)tak[25]
                                         }).ToList();
                #endregion
            }
            ExportToXLS<LocationLotDetail>("LocationLotDetail.xls", locationLotDetailList);
        }
        #endregion
        private SearchStatementModel PrepareSearchStatement(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
            string whereStatement = "where HuId is not Null";

            IList<object> param = new List<object>();

            HqlStatementHelper.AddEqStatement("Location", searchModel.Location, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Bin", searchModel.Bin, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("HuId", searchModel.HuId, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("OccupyType", searchModel.OccupyType, "l", ref whereStatement, param);
            if (searchModel.LotNo != null & searchModel.LotNoTo != null)
            {
                HqlStatementHelper.AddBetweenStatement("LotNo", searchModel.LotNo, searchModel.LotNoTo, "l", ref whereStatement, param);
            }
            else if (searchModel.LotNo != null & searchModel.LotNoTo == null)
            {
                HqlStatementHelper.AddGeStatement("LotNo", searchModel.LotNo, "l", ref whereStatement, param);
            }
            else if (searchModel.LotNo == null & searchModel.LotNoTo != null)
            {
                HqlStatementHelper.AddLeStatement("LotNo", searchModel.LotNoTo, "l", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("IsFreeze", searchModel.IsFreeze, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("IsConsignment", searchModel.IsConsignment, "l", ref whereStatement, param);
            //HqlStatementHelper.AddEqStatement("IsATP", searchModel.IsATP, "l", ref whereStatement, param);

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by l.Location,l.Item,l.LotNo,l.HuId,l.Direction";
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectCountStatement;
            searchStatementModel.SelectStatement = selectStatement;
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }

        private string PrepareSqlSearchStatement(LocationLotDetailSearchModel searchModel)
        {
            string whereStatement = @"select ld.HuId,ld.LotNo,ld.Location,ld.Bin,ld.Item,i.Desc1,i.RefCode,ld.UC,ld.OccupyType,ld.QualityType,ld.IsCS,ld.IsFreeze,ld.IsATP,ld.HuQty,ld.HuUom,ld.Qty,ld.BaseUom,hu.HuOption,ld.Direction,ht.Desc1 as DirectionDesc,ic.Code as MaterialsGroup,ic.Desc1 as MaterialsGroupDesc,hu.ItemVersion,hu.Remark,hu.ManufactureParty
                                    from VIEW_LocationLotDet  as ld 
                                    inner join MD_Item as i on ld.Item=i.Code
                                    inner join INV_Hu as hu on ld.HuId=hu.HuId
                                    left join MRP_HuTo as ht on ld.Direction=ht.Code 
                                    left join (select * from MD_ItemCategory where SubCategory=5) as ic on ic.Code=i.MaterialsGroup 
                                    where ld.HuId is not null  ";
            if (!string.IsNullOrEmpty(searchModel.Location))
            {
                whereStatement += string.Format(" and ld.Location = '{0}'", searchModel.Location);
            }
            if (!string.IsNullOrEmpty(searchModel.Bin))
            {
                whereStatement += string.Format(" and ld.Bin = '{0}'", searchModel.Bin);
            }
            if (!string.IsNullOrEmpty(searchModel.Item))
            {
                whereStatement += string.Format(" and ld.Item = '{0}'", searchModel.Item);
            }
            if (!string.IsNullOrEmpty(searchModel.HuId))
            {
                whereStatement += string.Format(" and ld.HuId like '%{0}%'", searchModel.HuId);
            }
            if (searchModel.OccupyType != null)
            {
                whereStatement += string.Format(" and ld.OccupyType ={0}", searchModel.OccupyType);
            }
            if (searchModel.IsFreeze2 != null)
            {
                whereStatement += string.Format(" and ld.IsFreeze ={0}", searchModel.IsFreeze2);
            }
            if (searchModel.IsATP2 != null)
            {
                whereStatement += string.Format(" and ld.IsATP ={0}", searchModel.IsATP2);
            }
            if (searchModel.IsConsignment2 != null)
            {
                whereStatement += string.Format(" and ld.IsCS ={0}", searchModel.IsConsignment2);
            }
            if (searchModel.QualityType != null)
            {
                whereStatement += string.Format(" and ld.QualityType ={0}", searchModel.QualityType);
            }
            if (!string.IsNullOrEmpty(searchModel.ManufactureParty))
            {
                whereStatement += string.Format(" and hu.ManufactureParty = '{0}'", searchModel.ManufactureParty);
            }
            //if (!string.IsNullOrWhiteSpace(searchModel.HuOptionHuLot))
            //{
            //    string huOPtionSql = " and hu.HuOption in( ";
            //    string[] huOptionHuLots = searchModel.HuOptionHuLot.Split(',');
            //    for (int ir = 0; ir < huOptionHuLots.Length; ir++)
            //    {
            //        huOPtionSql += "'" + huOptionHuLots[ir] + "',";
            //    }
            //    whereStatement += huOPtionSql.Substring(0, huOPtionSql.Length - 1) + ")";
            //}
            if (!string.IsNullOrWhiteSpace(searchModel.LotNoFrom) && !string.IsNullOrWhiteSpace(searchModel.LotNoTo))
            {
                whereStatement += string.Format(" and ld.LotNo between '{0}' and '{1}' ", searchModel.LotNoFrom, searchModel.LotNoTo);

            }
            else if (!string.IsNullOrWhiteSpace(searchModel.LotNoFrom) && string.IsNullOrWhiteSpace(searchModel.LotNoTo))
            {
                whereStatement += " and ld.LotNo >='" + searchModel.LotNoFrom + "'";
            }
            else if (string.IsNullOrWhiteSpace(searchModel.LotNoFrom) && !string.IsNullOrWhiteSpace(searchModel.LotNoTo))
            {
                whereStatement += " and ld.LotNo <= '" + searchModel.LotNoTo + "'";
            }

            return whereStatement;
        }

        #region HuTrans
        private SearchStatementModel PrepareHuTransSearchStatement(GridCommand command, HuTransSearchModel searchModel)
        {
            string whereStatement = "where 1=1";

            IList<object> param = new List<object>();

            HqlStatementHelper.AddEqStatement("Location", searchModel.Location, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Bin", searchModel.Bin, "l", ref whereStatement, param);
            HqlStatementHelper.AddEqStatement("Item", searchModel.Item, "l", ref whereStatement, param);
            HqlStatementHelper.AddLikeStatement("HuId", searchModel.HuId, HqlStatementHelper.LikeMatchMode.End, "l", ref whereStatement, param);
            if (searchModel.LotNo != null & searchModel.LotNoTo != null)
            {
                HqlStatementHelper.AddBetweenStatement("LotNo", searchModel.LotNo, searchModel.LotNoTo, "l", ref whereStatement, param);
            }
            else if (searchModel.LotNo != null & searchModel.LotNoTo == null)
            {
                HqlStatementHelper.AddGeStatement("LotNo", searchModel.LotNo, "l", ref whereStatement, param);
            }
            else if (searchModel.LotNo == null & searchModel.LotNoTo != null)
            {
                HqlStatementHelper.AddLeStatement("LotNo", searchModel.LotNoTo, "l", ref whereStatement, param);
            }
            HqlStatementHelper.AddEqStatement("CreateUserName", searchModel.CreateUserName, "l", ref whereStatement, param);
            if (searchModel.StartDate != null)
            {
                HqlStatementHelper.AddBetweenStatement("CreateDate", searchModel.StartDate, searchModel.EndDate, "l", ref whereStatement, param);
            }

            string sortingStatement = HqlStatementHelper.GetSortingStatement(command.SortDescriptors);
            if (command.SortDescriptors.Count == 0)
            {
                sortingStatement = " order by l.CreateDate desc";
            }
            SearchStatementModel searchStatementModel = new SearchStatementModel();
            searchStatementModel.SelectCountStatement = selectHuTransCountStatement;
            searchStatementModel.SelectStatement = selectHuTransStatement;
            if (searchModel.SearchType == "Freeze")
            {
                searchStatementModel.SelectCountStatement = selectHuTransCountStatement.Replace("PickTransaction", "FreezeTransaction");
                searchStatementModel.SelectStatement = selectHuTransStatement.Replace("PickTransaction", "FreezeTransaction");
            }
            else if (searchModel.SearchType == "EpChange")
            {
                searchStatementModel.SelectCountStatement = selectHuTransCountStatement.Replace("PickTransaction", "HuIdEpDateChangeTransaction");
                searchStatementModel.SelectStatement = selectHuTransStatement.Replace("PickTransaction", "HuIdEpDateChangeTransaction");
            }
            searchStatementModel.WhereStatement = whereStatement;
            searchStatementModel.SortingStatement = sortingStatement;
            searchStatementModel.Parameters = param.ToArray<object>();

            return searchStatementModel;
        }
        #region HuTrans View
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_HuTrans_View")]
        public ActionResult _PickTrans(GridCommand command, HuTransSearchModel searchModel)
        {
            ViewBag.Item = searchModel.Item;
            ViewBag.Location = searchModel.Location;
            ViewBag.Bin = searchModel.Bin;
            ViewBag.HuId = searchModel.HuId;
            ViewBag.LotNo = searchModel.LotNo;
            ViewBag.LotNoTo = searchModel.LotNoTo;
            ViewBag.StartDate = searchModel.StartDate;
            ViewBag.EndDate = searchModel.EndDate;
            ViewBag.SearchType = searchModel.SearchType;
            ViewBag.PageSize = this.ProcessPageSize(command.PageSize);
            return PartialView();
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_HuTrans_View")]
        public ActionResult _EpChangeTrans(GridCommand command, HuTransSearchModel searchModel)
        {
            ViewBag.Item = searchModel.Item;
            ViewBag.Location = searchModel.Location;
            ViewBag.Bin = searchModel.Bin;
            ViewBag.HuId = searchModel.HuId;
            ViewBag.LotNo = searchModel.LotNo;
            ViewBag.LotNoTo = searchModel.LotNoTo;
            ViewBag.StartDate = searchModel.StartDate;
            ViewBag.EndDate = searchModel.EndDate;
            ViewBag.SearchType = searchModel.SearchType;
            ViewBag.PageSize = this.ProcessPageSize(command.PageSize);
            return PartialView();
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_HuTrans_View")]
        public ActionResult _FreezeTrans(GridCommand command, HuTransSearchModel searchModel)
        {
            ViewBag.Item = searchModel.Item;
            ViewBag.Location = searchModel.Location;
            ViewBag.Bin = searchModel.Bin;
            ViewBag.HuId = searchModel.HuId;
            ViewBag.LotNo = searchModel.LotNo;
            ViewBag.LotNoTo = searchModel.LotNoTo;
            ViewBag.StartDate = searchModel.StartDate;
            ViewBag.EndDate = searchModel.EndDate;
            ViewBag.SearchType = searchModel.SearchType;
            ViewBag.SearchModel = searchModel;
            ViewBag.PageSize = this.ProcessPageSize(command.PageSize);
            return PartialView();
        }
        #endregion

        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_HuTrans_View")]
        public ActionResult _AjaxPickTransactionList(GridCommand command, HuTransSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareHuTransSearchStatement(command, searchModel);
            GridModel<PickTransaction> List = GetAjaxPageData<PickTransaction>(searchStatementModel, command);
            var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();
            foreach (var ListData in List.Data)
            {
                if (ListData.IsPick)
                {
                    ListData.HuAction = Resources.EXT.ControllerLan.Con_UnPick;
                }
                else
                {
                    ListData.HuAction = Resources.EXT.ControllerLan.Con_Pick;
                }
                ListData.ItemDescription = itemMgr.GetCacheItem(ListData.Item).FullDescription;
                ListData.MaterialsGroup = itemMgr.GetCacheItem(ListData.Item).MaterialsGroup;
                ListData.MaterialsGroupDesc = GetItemCategory(ListData.MaterialsGroup, itemCategoryList).Description;
            }
            return PartialView(List);
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_HuTrans_View")]
        public ActionResult _AjaxFreezeTransactionList(GridCommand command, HuTransSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareHuTransSearchStatement(command, searchModel);
            GridModel<FreezeTransaction> List = GetAjaxPageData<FreezeTransaction>(searchStatementModel, command);
            var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();
            foreach (var ListData in List.Data)
            {
                if (ListData.Freeze)
                {
                    ListData.HuAction = Resources.EXT.ControllerLan.Con_Freeze;
                }
                else
                {
                    ListData.HuAction = Resources.EXT.ControllerLan.Con_UnFreeze;
                }
                ListData.ItemDescription = itemMgr.GetCacheItem(ListData.Item).FullDescription;
                ListData.MaterialsGroup = itemMgr.GetCacheItem(ListData.Item).MaterialsGroup;
                ListData.MaterialsGroupDesc = GetItemCategory(ListData.MaterialsGroup, itemCategoryList).Description;
            }
            return PartialView(List);
        }
        [GridAction(EnableCustomBinding = true)]
        [SconitAuthorize(Permissions = "Url_HuTrans_View")]
        public ActionResult _AjaxEpChangeTransactionList(GridCommand command, HuTransSearchModel searchModel)
        {
            SearchStatementModel searchStatementModel = this.PrepareHuTransSearchStatement(command, searchModel);
            GridModel<HuIdEpDateChangeTransaction> List = GetAjaxPageData<HuIdEpDateChangeTransaction>(searchStatementModel, command);
            var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();
            foreach (var ListData in List.Data)
            {
                ListData.ItemDescription = itemMgr.GetCacheItem(ListData.Item).FullDescription;
                ListData.MaterialsGroup = itemMgr.GetCacheItem(ListData.Item).MaterialsGroup;
                ListData.MaterialsGroupDesc = GetItemCategory(ListData.MaterialsGroup,  itemCategoryList).Description;
            }
            return PartialView(List);
        }
        #region 导出条码事务
        [SconitAuthorize(Permissions = "Url_HuTrans_View")]
        public void Export(HuTransSearchModel searchModel)
        {
            int value = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            GridCommand command = new GridCommand();
            command.Page = 1;
            command.PageSize = value;

            SearchStatementModel searchStatementModel = this.PrepareHuTransSearchStatement(command, searchModel);
            GridModel<FreezeTransaction> List = new GridModel<FreezeTransaction>();
            GridModel<PickTransaction> List1 = new GridModel<PickTransaction>();
            GridModel<HuIdEpDateChangeTransaction> List2 = new GridModel<HuIdEpDateChangeTransaction>();
            var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();
            if (searchModel.SearchType == "Freeze")
            {
                List = GetAjaxPageData<FreezeTransaction>(searchStatementModel, command);
                foreach (var ListData in List.Data)
                {
                    if (ListData.Freeze)
                    {
                        ListData.HuAction = Resources.EXT.ControllerLan.Con_Freeze;
                    }
                    else
                    {
                        ListData.HuAction = Resources.EXT.ControllerLan.Con_UnFreeze;
                    }
                    ListData.ItemDescription = itemMgr.GetCacheItem(ListData.Item).FullDescription;
                    ListData.MaterialsGroup = itemMgr.GetCacheItem(ListData.Item).MaterialsGroup;
                    ListData.MaterialsGroupDesc = GetItemCategory(ListData.MaterialsGroup, itemCategoryList).Description;
                }

            }
            else if (searchModel.SearchType == "Pick")
            {
                List1 = GetAjaxPageData<PickTransaction>(searchStatementModel, command);
                foreach (var ListData in List1.Data)
                {
                    if (ListData.IsPick)
                    {
                        ListData.HuAction = Resources.EXT.ControllerLan.Con_UnPick;
                    }
                    else
                    {
                        ListData.HuAction = Resources.EXT.ControllerLan.Con_Pick;
                    }
                    ListData.ItemDescription = itemMgr.GetCacheItem(ListData.Item).FullDescription;
                    ListData.MaterialsGroup = itemMgr.GetCacheItem(ListData.Item).MaterialsGroup;
                    ListData.MaterialsGroupDesc = GetItemCategory(ListData.MaterialsGroup,  itemCategoryList).Description;
                }

            }
            else if (searchModel.SearchType == "EpChange")
            {
                List2 = GetAjaxPageData<HuIdEpDateChangeTransaction>(searchStatementModel, command);
                foreach (var ListData in List2.Data)
                {
                    ListData.ItemDescription = itemMgr.GetCacheItem(ListData.Item).FullDescription;
                    ListData.MaterialsGroup = itemMgr.GetCacheItem(ListData.Item).MaterialsGroup;
                    ListData.MaterialsGroupDesc = GetItemCategory(ListData.MaterialsGroup, itemCategoryList).Description;
                }

            }
            var fileName = string.Format("{0}Trans.xls", searchModel.SearchType);
            if (searchModel.SearchType == "Freeze")
            {
                ExportToXLS<FreezeTransaction>(fileName, List.Data.ToList());
            }
            else if (searchModel.SearchType == "Pick")
            {
                ExportToXLS<PickTransaction>(fileName, List1.Data.ToList());
            }
            else
            {
                ExportToXLS<HuIdEpDateChangeTransaction>(fileName, List2.Data.ToList());
            }
        }

        #endregion

        public string _GetHuTransView(DateTime startDate, DateTime endDate, string item, string huId, string lotNo, string bin, string location, string createUserName, string sType)
        {
            int tableColumnCount;

            SqlParameter[] sqlParams = new SqlParameter[9];
            string reqUrl = HttpContext.Request.Url.Authority + HttpContext.Request.ApplicationPath;
            sqlParams[0] = new SqlParameter("@Item", item);
            sqlParams[1] = new SqlParameter("@HuId", huId);
            sqlParams[2] = new SqlParameter("@Bin", bin);
            sqlParams[3] = new SqlParameter("@Location", location);
            sqlParams[4] = new SqlParameter("@LotNo", lotNo);
            sqlParams[5] = new SqlParameter("@CreateUser", createUserName);
            sqlParams[6] = new SqlParameter("@StartDate", startDate);
            sqlParams[7] = new SqlParameter("@EndDate", endDate);
            sqlParams[8] = new SqlParameter("@Type", sType);
            DataSet ds = genericMgr.GetDatasetByStoredProcedure("USP_Report_PickTransRecords", sqlParams);
            //table returned from SP is a temporary table ,so colculate columns in SP.
            tableColumnCount = (int)ds.Tables[0].Rows[0][0];
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            #region Head
            for (int i = 0; i < tableColumnCount; i++)
            {
                //SP return each column's length
                str.Append("<th style='min-width:" + (int)ds.Tables[1].Rows[0][i] + "px'>");
                str.Append(ds.Tables[2].Columns[i].ColumnName);
                str.Append("</th>");
            }
            str.Append("</tr></thead><tbody>");
            #endregion
            int l = 0;
            string trcss = string.Empty;
            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
            {
                l++;
                trcss = "";
                if (l % 2 == 0)
                {
                    trcss = "t-alt";
                }
                str.Append("<tr class=\"");
                str.Append(trcss);
                str.Append("\">");
                for (int j = 0; j < tableColumnCount; j++)
                {
                    str.Append("<td>");
                    str.Append(ds.Tables[1].Rows[i][j]);
                    str.Append("</td>");
                }

                str.Append("</tr>");
            }

            str.Append("</tbody></table>");
            return str.ToString();


        }
        #endregion

        #region
        //public void ExportXLS(LocationTransactionSearchModel searchModel)
        //{
        //    string sql = PrepareSqlSearchStatement(searchModel);

        //    string sortingStatement = TempData["sortingStatement"] != null ? TempData["sortingStatement"] as string : string.Empty;
        //    TempData["sortingStatement"] = sortingStatement;
        //    if (string.IsNullOrWhiteSpace(sortingStatement))
        //    {
        //        sortingStatement = " order by lt.EffDate desc";
        //    }
        //    IList<object[]> searchResult = this.genericMgr.FindAllWithNativeSql<object[]>(sql + sortingStatement);
        //    IList<LocationTransaction> locationTransactionList = new List<LocationTransaction>();
        //    if (searchResult != null && searchResult.Count > 0)
        //    {
        //        #region
        //        //lt.TransType,lt.EffDate,lt.OrderNo,lt.IpNo,lt.RecNo,lt.PartyFrom,lt.PartyTo,lt.Item,lt.IOType,lt.HuId,lt.LotNo,lt.Qty,au.CreateUserNm,
        //        //lt.CreateDate,om.ExtOrderNo,bp.Party,ba.Party 
        //        locationTransactionList = (from tak in searchResult
        //                                   select new LocationTransaction
        //                                   {
        //                                       TransactionTypeDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.TransactionType, int.Parse((tak[0]).ToString())),
        //                                       EffectiveDate = (DateTime)tak[1],
        //                                       OrderNo = (string)tak[2],
        //                                       IpNo = (string)tak[3],
        //                                       ReceiptNo = (string)tak[4],
        //                                       PartyFrom = (string)tak[5],
        //                                       PartyTo = (string)tak[6],
        //                                       LocationFrom = (string)tak[7],
        //                                       LocationTo = (string)tak[8],
        //                                       Item = (string)tak[9],
        //                                       IOTypeDescription = systemMgr.GetCodeDetailDescription(Sconit.CodeMaster.CodeMaster.TransactionIOType, int.Parse((tak[10]).ToString())),
        //                                       HuId = (string)tak[11],
        //                                       LotNo = (string)tak[12],
        //                                       Qty = (decimal)tak[13],
        //                                       CreateUserName = (string)tak[14],
        //                                       CreateDate = (DateTime)tak[15],
        //                                       SapOrderNo = (string)tak[16],
        //                                       Supplier = tak[17] != null ? (string)tak[17] : string.Empty,
        //                                   }).ToList();
        //        #endregion
        //    }

        //    ExportToXLS<LocationTransaction>("ExportList", "xls", locationTransactionList);
        //}
        #endregion
    }
}
