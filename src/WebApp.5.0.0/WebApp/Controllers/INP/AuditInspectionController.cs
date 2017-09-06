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
    public class AuditInspectionController : WebAppBaseController
    {
        //public IGenericMgr genericMgr { get; set; }
        public IInspectMgr inspectMgr { get; set; }
        #region  public
        public ActionResult Index()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_CUST_HuIdAuditInspection_View")]
        public ActionResult List(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
            SearchCacheModel searchCacheModel = this.ProcessSearchModel(command, searchModel);
            if (string.IsNullOrEmpty(searchModel.Location))
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_LocationCanNotBeEmpty);
                return View();
            }
            if (string.IsNullOrEmpty(searchModel.LotNoFrom))
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_FirstBatchNumberCanNotBeEmpty);
                return View();
            }
            if (string.IsNullOrEmpty(searchModel.LotNoFrom) && !string.IsNullOrEmpty(searchModel.LotNoTo))
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_FirstBatchNumberBeEmptyCanNotInputSecondBatchNumber);
                return View();
            }



            if (!string.IsNullOrEmpty(searchModel.Location)&&!string.IsNullOrEmpty(searchModel.LotNoFrom))
            {
                ViewBag.Region = searchModel.Region;
                TempData["_AjaxMessage"] = "";
            }
            else
            {
                SaveWarningMessage(Resources.EXT.ControllerLan.Con_SearchConditionNeedLocationAndBatchNo);
            }

            return View();
        }

        [SconitAuthorize(Permissions = "Url_CUST_HuIdAuditInspection_View")]
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
            foreach (var locationLotDetail in gridModel.Data)
            {
              locationLotDetail.ItemDescription=genericMgr.FindById<Item>(locationLotDetail.Item).Description;
            }

            return PartialView(gridModel);
        }


        #endregion

        private ReportSearchStatementModel PrepareSearchStatement(GridCommand command, LocationLotDetailSearchModel searchModel)
        {
           
            ReportSearchStatementModel reportSearchStatementModel = new ReportSearchStatementModel();
            reportSearchStatementModel.ProcedureName = "USP_Busi_GetPlusInventoryHuId";
            SqlParameter[] parameters = new SqlParameter[6];
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
            reportSearchStatementModel.Parameters = parameters;
            return reportSearchStatementModel;
        }


        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [SconitAuthorize(Permissions = "Url_CUST_HuIdAuditInspection_View")]
        public string New(string checkedIds, string Region)
        {
            IList<LocationLotDetail> viewLocationList = new List<LocationLotDetail>();

            string[] checkedIdArray = checkedIds.Split(',');
            string selectStatement = string.Empty;
            IList<object> selectPartyPara = new List<object>();
            foreach (var para in checkedIdArray)
            {
                if (selectStatement == string.Empty)
                {
                    selectStatement = "from LocationLotDetail where Id in (?";
                }
                else
                {
                    selectStatement += ",?";
                }
                selectPartyPara.Add(para);
            }
            selectStatement += ")";

            viewLocationList = genericMgr.FindAll<LocationLotDetail>(selectStatement, selectPartyPara.ToArray());

            BusinessException businessException = new BusinessException();
            try
            {
                #region orderDetailList

                IList<InspectDetail> inspectDetailList = new List<InspectDetail>();
                if (viewLocationList != null && viewLocationList.Count() > 0)
                {
                    foreach (LocationLotDetail locationlotdetail in viewLocationList)
                    {
                        InspectDetail inspectDetail = new InspectDetail();
                        Item item = this.genericMgr.FindById<Item>(locationlotdetail.Item);
                        inspectDetail.Item = locationlotdetail.Item;
                        inspectDetail.HuId = locationlotdetail.HuId;
                        inspectDetail.LotNo = locationlotdetail.LotNo;
                        inspectDetail.ItemDescription = item.Description;
                        inspectDetail.UnitCount = locationlotdetail.UnitCount;
                        inspectDetail.ReferenceItemCode = item.ReferenceCode;
                        inspectDetail.Uom = item.Uom;
                        inspectDetail.LocationFrom =locationlotdetail.Location;
                        inspectDetail.CurrentLocation = locationlotdetail.Location;
                        inspectDetail.BaseUom = item.Uom;
                        inspectDetail.UnitQty = 1;

                        inspectDetailList.Add(inspectDetail);

                    }
                }
                #endregion
                if (businessException.HasMessage)
                {
                    throw businessException;
                }
                if (inspectDetailList != null && inspectDetailList.Count == 0)
                {
                    throw new BusinessException(Resources.INP.InspectDetail.Errors_InspectDetail_Required);
                }
             

                InspectMaster inspectMaster = new InspectMaster();
                inspectMaster.Region = Region;
                inspectMaster.InspectDetails = inspectDetailList;

                inspectMaster.Type = com.Sconit.CodeMaster.InspectType.Barcode;
                inspectMaster.IsATP = false;

                inspectMgr.CreateInspectMaster(inspectMaster);
                SaveSuccessMessage(Resources.INP.InspectMaster.InspectMaster_Added);
                return inspectMaster.InspectNo;

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
