using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Utility;
using Telerik.Web.Mvc;
using com.Sconit.Web.Models;
using com.Sconit.Entity.INP;
using com.Sconit.Service;
using com.Sconit.Entity;
using System.IO;
using com.Sconit.Web.Models.SearchModels.INV;
using com.Sconit.Entity.INV;
using System.Data.SqlClient;
using System.Data;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MD;

namespace com.Sconit.Web.Controllers.INP
{
    public class HuIdFreezeController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }
        public IInspectMgr inspectMgr { get; set; }
        public ILocationDetailMgr locationMgr { get; set; }
        #region  public
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_CUST_HuIdFreeze_View")]
        public ActionResult List(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
            ViewBag.IsFreeze = searchModel.IsFreeze;
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (string.IsNullOrEmpty(searchModel.Location))
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_LocationCanNotBeEmpty);
                return View();
            }
            if (string.IsNullOrEmpty(searchModel.LotNoFrom) || string.IsNullOrEmpty(searchModel.LotNoFrom))
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_SearchConditionNeedManufactureDateAndBatchNo);
                return View();
            }
            if (string.IsNullOrEmpty(searchModel.LotNoFrom) && !string.IsNullOrEmpty(searchModel.LotNoTo))
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_SearchConditionNeedManufactureDate);
                return View();
            }



            if (!string.IsNullOrEmpty(searchModel.Location)&&!string.IsNullOrEmpty(searchModel.LotNoFrom))
            {

                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_SearchConditionNeedLocationAndBatchNo);
            }

            return View();
        }

        [SconitAuthorize(Permissions = "Url_CUST_HuIdFreeze_View")]
        [GridAction(EnableCustomBinding = true)]
        public ActionResult _AjaxList(GridCommand command, LocationLotDetailSearchModel searchModel)
        {

            if (string.IsNullOrEmpty(searchModel.Location))
            {
                return PartialView(new GridModel(new List<LocationLotDetail>()));
            }
            if (string.IsNullOrEmpty(searchModel.LotNoFrom))
            {
                return PartialView(new GridModel(new List<LocationLotDetail>())); ;
            }
            if (string.IsNullOrEmpty(searchModel.LotNoFrom) && !string.IsNullOrEmpty(searchModel.LotNoTo))
            {
                return PartialView(new GridModel(new List<LocationLotDetail>()));
            }

            ReportSearchStatementModel reportSearchStatementModel = PrepareSearchStatement(command, searchModel);
            GridModel<LocationLotDetail> gridModel = GetAuditInspectionPageData<LocationLotDetail>(reportSearchStatementModel);
            IList<FreezeTransaction> freezeTrans = new List<FreezeTransaction>();
            if (searchModel.IsFreeze && gridModel.Data.Count() != 0 )
            {
                freezeTrans = genericMgr.FindAllIn<FreezeTransaction>
                        ("from FreezeTransaction where Freeze=1 and HuId in(? ", gridModel.Data.Select(p => (object)p.HuId));
            }
            var freezeTransDic = freezeTrans.GroupBy(p => p.HuId, (k, g) => new { k, Code = g.OrderBy(p=>p.Id).First() })
                    .ToDictionary(d => d.k, d => d);
            var itemlist = itemMgr.GetCacheAllItem();
            foreach (var locationLotDetail in gridModel.Data)
            {
                locationLotDetail.ItemDescription = itemlist.ValueOrDefault(locationLotDetail.Item).FullDescription;
                if (searchModel.IsFreeze)
                {
                    locationLotDetail.Reason = freezeTransDic.ValueOrDefault(locationLotDetail.HuId).Code.Reason;
                }
            }
            return PartialView(gridModel);
        }


        #endregion

        private ReportSearchStatementModel PrepareSearchStatement(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
           
            ReportSearchStatementModel reportSearchStatementModel = new ReportSearchStatementModel();
            reportSearchStatementModel.ProcedureName = "USP_Busi_GetPlusInventoryHuId";
            SqlParameter[] parameters = new SqlParameter[7];
            parameters[0] = new SqlParameter("@Location", SqlDbType.VarChar, 50);
            parameters[0].Value = searchModel.Location;

            parameters[1] = new SqlParameter("@Item", SqlDbType.VarChar, 50);
            parameters[1].Value = searchModel.Item;

            parameters[2] = new SqlParameter("@LotNoFrom", SqlDbType.VarChar, 50);
            parameters[2].Value = searchModel.LotNoFrom;

            parameters[3] = new SqlParameter("@LotNoTo", SqlDbType.VarChar,50);
            parameters[3].Value = searchModel.LotNoTo;


            parameters[4] = new SqlParameter("@IsFrozen", SqlDbType.Bit);
            parameters[4].Value = searchModel.IsFreeze;

            parameters[5] = new SqlParameter("@IsConsignment", SqlDbType.Bit);
            parameters[5].Value = searchModel.IsConsignment;

            parameters[6] = new SqlParameter("@ProductType", SqlDbType.VarChar, 50);
            parameters[6].Value = searchModel.ProductType;
            reportSearchStatementModel.Parameters = parameters;
            return reportSearchStatementModel;
        }


        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_CUST_HuIdFreeze_View")]
        public string UpdateIsFreeze(string checkedIds, string IsFreezeStatus,string reason)
        {
            IList<string> HuIdList=new List<string>();
            string[] strArray=checkedIds.Split(',');
            foreach (string str in strArray)
	        {
		       HuIdList.Add(str);
	        }

            BusinessException businessException = new BusinessException();
            try
            {
                if (IsFreezeStatus == "1")
                {
                    locationMgr.InventoryFreeze(HuIdList,reason);
                    return string.Format(Resources.EXT.ControllerLan.Con_BarcodeFreezeSuccessfully, checkedIds);
                }
                else
                {
                    locationMgr.InventoryUnFreeze(HuIdList,reason);
                    return string.Format(Resources.EXT.ControllerLan.Con_BarcodeCancellFreezeSuccessfully, checkedIds);
                }
            }
            catch (BusinessException ex)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                string messagesStr = "";
                IList<Message> messageList = ex.GetMessages();
                foreach (Message message in messageList)
                {
                    messagesStr +=  message.GetMessageString() ;
                }
                Response.Write(messagesStr);
                return string.Empty;
            }

        }

    }
}
