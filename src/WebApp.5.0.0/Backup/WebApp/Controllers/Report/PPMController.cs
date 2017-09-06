using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using com.Sconit.Utility;
using com.Sconit.Web.Models;
using com.Sconit.Entity.SCM;
using com.Sconit.Service;
using com.Sconit.Entity.INP;
using com.Sconit.Entity.SYS;
using com.Sconit.Utility.Report;
using com.Sconit.Entity.ORD;
using com.Sconit.Web.Models.ReportModels;
using com.Sconit.Entity.INV;

namespace com.Sconit.Web.Controllers.Report
{
    public class PPMController : WebAppBaseController
    {

        //public IGenericMgr genericMgr { get; set; }
        //public IReportGen reportGen { get; set; }

        #region procurement ppm
        [SconitAuthorize(Permissions = "Url_PPM_Procurement")]
        public ActionResult ProcurementIndex()
        {
            return View();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_PPM_Procurement")]
        public ActionResult _SearchResult(GridCommand command, string item, string supplier, DateTime? dateFrom, DateTime? dateTo)
        {
            string sqlStr = PrepareSearchStatement(command, item, supplier, dateFrom, dateTo);
            IList<object[]> objectList = this.queryMgr.FindAllWithNativeSql<object[]>(sqlStr);

            var inspectDetailList = (from inp in objectList
                                     select new InspectDetail
                                     {
                                         IpNo = (string)inp[0],
                                         ManufactureParty = (string)inp[1],
                                         ManufacturePartyName = (string)inp[2],
                                         Item = (string)inp[3],
                                         ItemDescription = (string)inp[4],
                                         ReferenceItemCode = (string)inp[5],
                                         UnitCount = (decimal)inp[6],
                                         Uom = (string)inp[7],
                                         RejectQty = (decimal)inp[8],
                                         InspectQty = (decimal)inp[9]
                                     }).ToList();

            int count = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            if (inspectDetailList.Count > count)
            {
                SaveWarningMessage(string.Format(Resources.EXT.ControllerLan.Con_DataExceedRowTheSpecifiedRows, count));
            }
            return View(inspectDetailList.Take(count));
        }
        #endregion



        #region procurement return ppm
        [SconitAuthorize(Permissions = "Url_PPM_ProcurementReturn")]
        public ActionResult ReturnIndex()
        {
            return View();
        }



        [GridAction]
        [SconitAuthorize(Permissions = "Url_PPM_ProcurementReturn")]
        public ActionResult _ReturnSearchResult(GridCommand command, string item, string supplier, DateTime? dateFrom, DateTime? dateTo)
        {
            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;
            return PartialView();
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_PPM_ProcurementReturn")]
        public ActionResult _ReturnHierarchyAjax(GridCommand command, string item, string supplier, DateTime? dateFrom, DateTime? dateTo)
        {
            string sqlStr = PrepareReturnSearchStatement(command, item, supplier, dateFrom, dateTo);
            IList<object[]> objectList = this.queryMgr.FindAllWithNativeSql<object[]>(sqlStr);

            var receiveReturnList = (from rr in objectList
                                     select new ReceiveReturnModel
                                     {
                                         Supplier = (string)rr[0],
                                         Item = (string)rr[1],
                                         ReceivedQty = rr[2] == null ? 0 : (decimal)rr[2],
                                         RejectedQty = rr[3] == null ? 0 : (decimal)rr[3],
                                         ItemDescription = (string)rr[4],
                                         ReferenceItemCode = (string)rr[5],
                                         Uom = (string)rr[6],
                                         SupplierName = (string)rr[7]
                                     }).ToList();

            int count = Convert.ToInt32(base.systemMgr.GetEntityPreferenceValue(EntityPreference.CodeEnum.MaxRowSizeOnPage));
            if (receiveReturnList.Count > count)
            {
                SaveWarningMessage(string.Format(Resources.EXT.ControllerLan.Con_DataExceedRowTheSpecifiedRows, count));
            }
            return PartialView(new GridModel(receiveReturnList));
        }

        [GridAction]
        [SconitAuthorize(Permissions = "Url_PPM_ProcurementReturn")]
        public ActionResult _ReturnDetailHierarchyAjax(string item, string supplier, DateTime? dateFrom, DateTime? dateTo)
        {
            string hql = "select l from LocationTransaction as l where l.Item = ? and ((l.TransactionType in(?,?,?,?) and l.PartyFrom = ?) or (l.TransactionType in (?,?,?,?) and l.PartyTo = ? and l.QualityType = ?)) ";
            IList<object> paramList = new List<object>();
            paramList.Add(item);
            paramList.Add(com.Sconit.CodeMaster.TransactionType.RCT_PO);
            paramList.Add(com.Sconit.CodeMaster.TransactionType.RCT_PO_VOID);
            paramList.Add(com.Sconit.CodeMaster.TransactionType.RCT_SL);
            paramList.Add(com.Sconit.CodeMaster.TransactionType.RCT_SL_VOID);
            paramList.Add(supplier);
            paramList.Add(com.Sconit.CodeMaster.TransactionType.ISS_PO);
            paramList.Add(com.Sconit.CodeMaster.TransactionType.ISS_PO_VOID);
            paramList.Add(com.Sconit.CodeMaster.TransactionType.ISS_SL);
            paramList.Add(com.Sconit.CodeMaster.TransactionType.ISS_SL_VOID);
            paramList.Add(supplier);
            paramList.Add(com.Sconit.CodeMaster.QualityType.Reject);
            if (dateFrom != null)
            {
                hql += " and l.EffectiveDate >= ?";
                paramList.Add(dateFrom);
            }

            if (dateTo != null)
            {
                hql += " and l.EffectiveDate <= ?";
                paramList.Add(dateTo);
            }

            IList<LocationTransaction> loctransList = genericMgr.FindAll<LocationTransaction>(hql, paramList.ToArray());
            base.FillCodeDetailDescription<LocationTransaction>(loctransList);
            return View(new GridModel(loctransList));
        }
        #endregion

        private string PrepareSearchStatement(GridCommand command, string item, string supplier, DateTime? dateFrom, DateTime? dateTo)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            string sqlStr = "select i.IpNo,i.PartyFrom,p.Name,d.Item,d.Itemdesc,d.Refitemcode,d.Uc,d.Uom,sum(d.RejectQty) as RejectQty,sum(d.InspQty) as InspectQty from View_Ipmstr i left join Inp_inspectmstr m on i.IpNo=m.IpNo inner join Inp_inspectdet d on m.InpNo=d.InpNo inner join Md_Party p on i.PartyFrom = p.Code ";
            sqlStr += "where d.RejectQty > 0 and m.Type = " + (int)com.Sconit.CodeMaster.OrderType.Procurement;
            if (!string.IsNullOrEmpty(supplier))
            {
                sqlStr += " and i.PartyFrom = '" + supplier + "'";
            }
            if (!string.IsNullOrEmpty(item))
            {
                sqlStr += " and d.Item = '" + item + "'";
            }
            if (dateFrom != null)
            {
                sqlStr += " and m.CreateDate >= '" + dateFrom + "'";
            }

            if (dateTo != null)
            {
                sqlStr += " and m.CreateDate <= '" + dateTo + "'";
            }
            sqlStr += "  group by i.IpNo,i.PartyFrom,p.Name,d.Item,d.Itemdesc,d.Refitemcode,d.Uc,d.Uom";
            return sqlStr;
        }

        private string PrepareReturnSearchStatement(GridCommand command, string item, string supplier, DateTime? dateFrom, DateTime? dateTo)
        {
            string whereStatement = string.Empty;
            IList<object> param = new List<object>();

            string sqlStr = "select C.*,i.Desc1,i.RefCode,i.Uom,p.Name from (select case when A.Supplier is null then B.Supplier else A.Supplier end as Supplier,case when A.Item is null then B.Item else A.Item end as Item,B.Qty as ReceivedQty,A.Qty as RejectedQty ";
            sqlStr += " from (select l.PartyTo as Supplier,l.Item,0-sum(l.Qty) as Qty from View_Loctrans  l  where l.QualityType=" + (int)com.Sconit.CodeMaster.QualityType.Reject + " and l.TransType in (" + (int)com.Sconit.CodeMaster.TransactionType.ISS_PO + "," + (int)com.Sconit.CodeMaster.TransactionType.ISS_PO_VOID + "," + (int)com.Sconit.CodeMaster.TransactionType.ISS_SL + "," + (int)com.Sconit.CodeMaster.TransactionType.ISS_SL_VOID + ")";
            if (!string.IsNullOrEmpty(supplier))
            {
                sqlStr += " and l.PartyTo = '" + supplier + "'";
            }
            if (!string.IsNullOrEmpty(item))
            {
                sqlStr += " and l.Item = '" + item + "'";
            }
            if (dateFrom != null)
            {
                sqlStr += " and l.EffDate >= '" + dateFrom + "'";
            }

            if (dateTo != null)
            {
                sqlStr += " and l.EffDate <= '" + dateTo + "'";
            }
            sqlStr += " group by l.PartyTo,l.Item)A ";

            sqlStr += " full join (select l.PartyFrom as Supplier,l.Item,sum(l.Qty) as Qty from View_Loctrans  l where  l.TransType in (" + (int)com.Sconit.CodeMaster.TransactionType.RCT_PO + "," + (int)com.Sconit.CodeMaster.TransactionType.RCT_PO_VOID + "," + (int)com.Sconit.CodeMaster.TransactionType.RCT_SL + "," + (int)com.Sconit.CodeMaster.TransactionType.RCT_SL_VOID + ")";
            if (!string.IsNullOrEmpty(supplier))
            {
                sqlStr += " and l.PartyFrom = '" + supplier + "'";
            }
            if (!string.IsNullOrEmpty(item))
            {
                sqlStr += " and l.Item = '" + item + "'";
            }
            if (dateFrom != null)
            {
                sqlStr += " and l.EffDate >= '" + dateFrom + "'";
            }

            if (dateTo != null)
            {
                sqlStr += " and l.EffDate <= '" + dateTo + "'";
            }
            sqlStr += " group by l.PartyFrom,l.Item)B ";

            sqlStr += " on A.Supplier = B.Supplier and A.Item = B.Item) C inner join MD_Party p on C.Supplier = p.Code inner join MD_Item i on C.Item = i.Code";

            return sqlStr;
        }

    }
}
