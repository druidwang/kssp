using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using com.Sconit.Service;
using com.Sconit.Entity.VIEW;
using System.Text;
using com.Sconit.Utility;
using com.Sconit.Entity.MD;

namespace com.Sconit.Web.Controllers.Report
{
    public class IOBController : WebAppBaseController
    {
        public ILocationDetailMgr locDetMgr { get; set; }

        public IBillMgr billMgr { get; set; }
        //
        // GET: /ReceiptShipStore/

        /// <summary>
        /// 库存收发存
        /// </summary>
        /// <returns></returns>
        [SconitAuthorize(Permissions = "Url_IOB_LocationDetailIOB")]
        public ActionResult LocationDetailIOBIndex()
        {
            return View();
        }

        /// <summary>
        /// 寄售库存收发存
        /// </summary>
        /// <returns></returns>
        [SconitAuthorize(Permissions = "Url_IOB_SupplierPlanBillIOB")]
        public ActionResult SupplierPlanBillIOBIndex()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_IOB_CustomerPlanBillIOB")]
        public ActionResult CustomerPlanBillIOBIndex()
        {
            return View();
        }

        [SconitAuthorize(Permissions = "Url_IOB_CustomerPlanBillIOB,Url_IOB_SupplierPlanBillIOB")]
        public string _GetBillIOB(CodeMaster.BillType billType, string party, string location, string item,
            DateTime startDate, DateTime endDate)
        {
            IList<BillIOB> iobList = billMgr.GetBillIOB(billType, party, location, item, startDate, endDate);
            var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();
            foreach (var listdata in iobList)
            {
                listdata.MaterialsGroup = itemMgr.GetCacheItem(listdata.Item).FullDescription;
                listdata.MaterialsGroup = itemMgr.GetCacheItem(listdata.Item).MaterialsGroup;
                listdata.MaterialsGroupDesc = GetItemCategory(listdata.MaterialsGroup, Sconit.CodeMaster.SubCategory.MaterialsGroup, itemCategoryList).Description;
            } 
            return GetBillIOB(billType, iobList);
        }
        [SconitAuthorize(Permissions = "Url_IOB_CustomerPlanBillIOB,Url_IOB_SupplierPlanBillIOB")]
        public ActionResult ExportCustomerPlan(string party, string location, string item,
            DateTime startDate, DateTime endDate)
        {
            CodeMaster.BillType billType = CodeMaster.BillType.Distribution;
            var table = _GetBillIOB(billType, party, location, item, startDate, endDate);
            return new DownloadFileActionResult(table, "CustomerPlanBillIOB.xls");
        }
        [SconitAuthorize(Permissions = "Url_IOB_CustomerPlanBillIOB,Url_IOB_SupplierPlanBillIOB")]
        public ActionResult ExportSupplierPlan(  string party, string location, string item,
            DateTime startDate, DateTime endDate)
        {
            CodeMaster.BillType billType = CodeMaster.BillType.Procurement;
            var table = _GetBillIOB(billType, party, location, item, startDate, endDate);
            return new DownloadFileActionResult(table, "SupplierPlanBillIOB.xls");
        }
        private string GetBillIOB(CodeMaster.BillType billType, IList<BillIOB> billList)
        {
            if (billList.Count == 0)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            var firstIob = billList.First();
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            #region Head

            str.Append("<th>");
            str.Append(Resources.Report.BillIOB.BillIOB_Item);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.Report.BillIOB.BillIOB_ItemDescription);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.MD.Item.Item_MaterialsGroup);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.MD.Item.Item_MaterialsGroupDesc);
            str.Append("</th>");

            str.Append("<th>");
            if (billType == CodeMaster.BillType.Distribution)
            {
                str.Append(Resources.EXT.ControllerLan.Con_Customer);
            }
            else
            {
                str.Append(Resources.EXT.ControllerLan.Con_Supplier);
            }
            str.Append("</th>");

            str.Append("<th>");
            if (billType == CodeMaster.BillType.Distribution)
            {
                str.Append(Resources.EXT.ControllerLan.Con_CustomerName);
            }
            else
            {
                str.Append(Resources.EXT.ControllerLan.Con_SupplierName);
            }
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.Report.BillIOB.BillIOB_Location);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.Report.BillIOB.BillIOB_LocationName);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.Report.BillIOB.BillIOB_Uom);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.Report.BillIOB.BillIOB_StartQty);
            str.Append("</th>");

            //str.Append("<th>");
            //str.Append(Resources.Report.BillIOB.BillIOB_StartAmount);
            //str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.Report.BillIOB.BillIOB_InQty);
            str.Append("</th>");

            //str.Append("<th>");
            //str.Append(Resources.Report.BillIOB.BillIOB_InAmount);
            //str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.Report.BillIOB.BillIOB_OutQty);
            str.Append("</th>");

            //str.Append("<th>");
            //str.Append(Resources.Report.BillIOB.BillIOB_OutAmount);
            //str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.Report.BillIOB.BillIOB_EndQty);
            str.Append("</th>");

            //str.Append("<th>");
            //str.Append(Resources.Report.BillIOB.BillIOB_EndAmount);
            //str.Append("</th>");
            ///


            #endregion

            str.Append("</tr></thead><tbody>");

            if (billList != null && billList.Count > 0)
            {
                for (int i = 0; i < billList.Count; i++)
                {
                    var billIOB = billList[i];
                    #region Row
                    str.Append("<tr>");
                    str.Append("<td>");
                    str.Append(billIOB.Item);
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(billIOB.ItemDescription);
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(billIOB.MaterialsGroup);
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(billIOB.MaterialsGroupDesc);
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(billIOB.Party);
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(billIOB.PartyName);
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(billIOB.Location);
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(billIOB.LocationName);
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(billIOB.Uom);
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(billIOB.StartQty.ToString("0.########"));
                    str.Append("</td>");

                    //str.Append("<td>");
                    //str.Append(billIOB.StartAmount.ToString("0.########"));
                    //str.Append("</td>");

                    str.Append("<td>");
                    str.Append(billIOB.InQty.ToString("0.########"));
                    str.Append("</td>");

                    //str.Append("<td>");
                    //str.Append(billIOB.InAmount.ToString("0.########"));
                    //str.Append("</td>");

                    str.Append("<td>");
                    str.Append(billIOB.OutQty.ToString("0.########"));
                    str.Append("</td>");

                    //str.Append("<td>");
                    //str.Append(billIOB.OutAmount.ToString("0.########"));
                    //str.Append("</td>");

                    str.Append("<td>");
                    str.Append(billIOB.EndQty.ToString("0.########"));
                    str.Append("</td>");

                    //str.Append("<td>");
                    //str.Append(billIOB.EndAmount.ToString("0.########"));
                    //str.Append("</td>");

                    str.Append("</tr>");
                    #endregion
                }
            }

            //表尾
            str.Append("</tbody>");
            str.Append("</table>");
            return str.ToString();
        }

        [SconitAuthorize(Permissions = "Url_IOB_LocationDetailIOB")]
        public string _GetLocationDetailIOB(string location, string item, DateTime startDate, DateTime endDate)
        {
            IList<LocationDetailIOB> iobList = locDetMgr.GetLocationDetailIOB(location, item, startDate, endDate);
            var itemCategoryList = this.genericMgr.FindAll<ItemCategory>();
            foreach (var listdata in iobList)
            {
                if (!string.IsNullOrWhiteSpace(listdata.Item))
                listdata.MaterialsGroup = itemMgr.GetCacheItem(listdata.Item).MaterialsGroup;
                listdata.MaterialsGroupDesc = GetItemCategory(listdata.MaterialsGroup, Sconit.CodeMaster.SubCategory.MaterialsGroup, itemCategoryList).Description;
            }
            return GetStringLocationDetailIOB(iobList);
        }

        [SconitAuthorize(Permissions = "Url_IOB_LocationDetailIOB")]
        public ActionResult Export(string location, string item, DateTime startDate, DateTime endDate)
        {
            var table = _GetLocationDetailIOB(location, item, startDate, endDate);
            return new DownloadFileActionResult(table, "LocationDetailIOB.xls");
        }

        #region private
        private string GetStringLocationDetailIOB(IList<LocationDetailIOB> iobList)
        {
            if (iobList.Count == 1)
            {
                return Resources.EXT.ControllerLan.Con_NoRecord;
            }
            var firstIob = iobList.First();
            StringBuilder str = new StringBuilder("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\" id=\"datatable\" width=\"100%\"><thead><tr>");

            #region Head

            str.Append("<th rowspan=\"2\">");
            str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_Item);
            str.Append("</th>");

            str.Append("<th rowspan=\"2\">");
            str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_ItemDescription);
            str.Append("</th>");

            str.Append("<th rowspan=\"2\">");
            str.Append(Resources.MD.Item.Item_MaterialsGroup);
            str.Append("</th>");

            str.Append("<th rowspan=\"2\">");
            str.Append(Resources.MD.Item.Item_MaterialsGroupDesc);
            str.Append("</th>");
            //str.Append("<th rowspan=\"2\">");
            //str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_Location);
            //str.Append("</th>");

            //str.Append("<th rowspan=\"2\">");
            //str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_LocationName);
            //str.Append("</th>");

            str.Append("<th rowspan=\"2\">");
            str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_Uom);
            str.Append("</th>");

            ////
            str.Append("<th colspan=\"5\" style=\"text-align:center\">");
            str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_Start);
            str.Append("</th>");

            ///
            if (firstIob.RctPo != 0)
            {
                str.Append("<th rowspan=\"2\">");
                str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_RctPo);
                str.Append("</th>");
            }
            if (firstIob.RctWo != 0)
            {
                str.Append("<th rowspan=\"2\">");
                str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_RctWo);
                str.Append("</th>");
            }
            if (firstIob.RctSwo != 0)
            {
                str.Append("<th rowspan=\"2\">");
                str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_RctSwo);
                str.Append("</th>");
            }
            if (firstIob.RctTr != 0)
            {
                str.Append("<th rowspan=\"2\">");
                str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_RctTr);
                str.Append("</th>");
            }
            if (firstIob.RctStr != 0)
            {
                str.Append("<th rowspan=\"2\">");
                str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_RctStr);
                str.Append("</th>");
            }
            if (firstIob.RctUnp != 0)
            {
                str.Append("<th rowspan=\"2\">");
                str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_RctUnp);
                str.Append("</th>");
            }
            if (firstIob.RctSo != 0)
            {
                str.Append("<th rowspan=\"2\">");
                str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_RctSo);
                str.Append("</th>");
            }
            if (firstIob.RctIic != 0)
            {
                str.Append("<th rowspan=\"2\">");
                str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_RctIic);
                str.Append("</th>");
            }
            if (firstIob.IssSo != 0)
            {
                str.Append("<th rowspan=\"2\">");
                str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_IssSo);
                str.Append("</th>");
            }
            if (firstIob.IssWo != 0)
            {
                str.Append("<th rowspan=\"2\">");
                str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_IssWo);
                str.Append("</th>");
            }
            if (firstIob.IssSwo != 0)
            {
                str.Append("<th rowspan=\"2\">");
                str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_IssSwo);
                str.Append("</th>");
            }
            if (firstIob.IssTr != 0)
            {
                str.Append("<th rowspan=\"2\">");
                str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_IssTr);
                str.Append("</th>");
            }
            if (firstIob.IssStr != 0)
            {
                str.Append("<th rowspan=\"2\">");
                str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_IssStr);
                str.Append("</th>");
            }
            if (firstIob.IssUnp != 0)
            {
                str.Append("<th rowspan=\"2\">");
                str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_IssUnp);
                str.Append("</th>");
            }
            if (firstIob.IssPo != 0)
            {
                str.Append("<th rowspan=\"2\">");
                str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_IssPo);
                str.Append("</th>");
            }
            if (firstIob.IssIic != 0)
            {
                str.Append("<th rowspan=\"2\">");
                str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_IssIic);
                str.Append("</th>");
            }
            //if (firstIob.Inp != 0)
            //{
            //    str.Append("<th rowspan=\"2\">");
            //    str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_Inp);
            //    str.Append("</th>");
            //}
            //if (firstIob.Qdii != 0)
            //{
            //    str.Append("<th rowspan=\"2\">");
            //    str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_Qdii);
            //    str.Append("</th>");
            //}
            //if (firstIob.Rej != 0)
            //{
            //    str.Append("<th rowspan=\"2\">");
            //    str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_Rej);
            //    str.Append("</th>");
            //}
            //if (firstIob.Ccs != 0)
            //{
            //    str.Append("<th rowspan=\"2\">");
            //    str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_Ccs);
            //    str.Append("</th>");
            //}
            if (firstIob.CycCnt != 0)
            {
                str.Append("<th rowspan=\"2\">");
                str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_CycCnt);
                str.Append("</th>");
            }
            if (firstIob.Other != 0)
            {
                str.Append("<th rowspan=\"2\">");
                str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_Other);
                str.Append("</th>");
            }
            ///
            str.Append("<th colspan=\"5\" style=\"text-align:center\">");
            str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_End);
            str.Append("</th>");

            str.Append("</tr><tr>");

            ////////////
            str.Append("<th >");
            str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_StartTotal);
            str.Append("</th>");

            str.Append("<th >");
            str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_StartCs);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_StartNml);
            str.Append("</th>");

            str.Append("<th >");
            str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_StartInp);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_StartRej);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_EndNml);
            str.Append("</th>");

            str.Append("<th >");
            str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_EndInp);
            str.Append("</th>");

            str.Append("<th>");
            str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_EndRej);
            str.Append("</th>");

            str.Append("<th >");
            str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_EndCs);
            str.Append("</th>");

            str.Append("<th >");
            str.Append(Resources.Report.LocationDetailIOB.LocationDetailIOB_EndTotal);
            str.Append("</th>");
            #endregion

            str.Append("</tr></thead><tbody>");

            if (iobList != null && iobList.Count > 0)
            {
                for (int i = 1; i < iobList.Count; i++)
                {
                    var locationDetailIOB = iobList[i];
                    #region Row
                    str.Append("<tr>");
                    str.Append("<td>");
                    str.Append(locationDetailIOB.Item);
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(locationDetailIOB.ItemDescription);
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(locationDetailIOB.MaterialsGroup);
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(locationDetailIOB.MaterialsGroupDesc);
                    str.Append("</td>");
                    //str.Append("<td>");
                    //str.Append(locationDetailIOB.Location);
                    //str.Append("</td>");

                    //str.Append("<td>");
                    //str.Append(locationDetailIOB.LocationName);
                    //str.Append("</td>");

                    str.Append("<td>");
                    str.Append(locationDetailIOB.Uom);
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(locationDetailIOB.Start.ToString("0.########"));
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(locationDetailIOB.StartCs.ToString("0.########"));
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(locationDetailIOB.StartNml.ToString("0.########"));
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(locationDetailIOB.StartInp.ToString("0.########"));
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(locationDetailIOB.StartRej.ToString("0.########"));
                    str.Append("</td>");

                    if (firstIob.RctPo != 0)
                    {
                        str.Append("<td>");
                        str.Append(locationDetailIOB.RctPo.ToString("0.########"));
                        str.Append("</td>");
                    }
                    if (firstIob.RctWo != 0)
                    {
                        str.Append("<td>");
                        str.Append(locationDetailIOB.RctWo.ToString("0.########"));
                        str.Append("</td>");
                    }
                    if (firstIob.RctSwo != 0)
                    {
                        str.Append("<td>");
                        str.Append(locationDetailIOB.RctSwo.ToString("0.########"));
                        str.Append("</td>");
                    }
                    if (firstIob.RctTr != 0)
                    {
                        str.Append("<td>");
                        str.Append(locationDetailIOB.RctTr.ToString("0.########"));
                        str.Append("</td>");
                    }
                    if (firstIob.RctStr != 0)
                    {
                        str.Append("<td>");
                        str.Append(locationDetailIOB.RctStr.ToString("0.########"));
                        str.Append("</td>");
                    }
                    if (firstIob.RctUnp != 0)
                    {
                        str.Append("<td>");
                        str.Append(locationDetailIOB.RctUnp.ToString("0.########"));
                        str.Append("</td>");
                    }
                    if (firstIob.RctSo != 0)
                    {
                        str.Append("<td>");
                        str.Append(locationDetailIOB.RctSo.ToString("0.########"));
                        str.Append("</td>");
                    }
                    if (firstIob.RctIic != 0)
                    {
                        str.Append("<td>");
                        str.Append(locationDetailIOB.RctIic.ToString("0.########"));
                        str.Append("</td>");
                    }
                    if (firstIob.IssSo != 0)
                    {
                        str.Append("<td>");
                        str.Append(locationDetailIOB.IssSo.ToString("0.########"));
                        str.Append("</td>");
                    }
                    if (firstIob.IssWo != 0)
                    {
                        str.Append("<td>");
                        str.Append(locationDetailIOB.IssWo.ToString("0.########"));
                        str.Append("</td>");
                    }
                    if (firstIob.IssSwo != 0)
                    {
                        str.Append("<td>");
                        str.Append(locationDetailIOB.IssSwo.ToString("0.########"));
                        str.Append("</td>");
                    }
                    if (firstIob.IssTr != 0)
                    {
                        str.Append("<td>");
                        str.Append(locationDetailIOB.IssTr.ToString("0.########"));
                        str.Append("</td>");
                    }
                    if (firstIob.IssStr != 0)
                    {
                        str.Append("<td>");
                        str.Append(locationDetailIOB.IssStr.ToString("0.########"));
                        str.Append("</td>");
                    }
                    if (firstIob.IssUnp != 0)
                    {
                        str.Append("<td>");
                        str.Append(locationDetailIOB.IssUnp.ToString("0.########"));
                        str.Append("</td>");
                    }
                    if (firstIob.IssPo != 0)
                    {
                        str.Append("<td>");
                        str.Append(locationDetailIOB.IssPo.ToString("0.########"));
                        str.Append("</td>");
                    }
                    if (firstIob.IssIic != 0)
                    {
                        str.Append("<td>");
                        str.Append(locationDetailIOB.IssIic.ToString("0.########"));
                        str.Append("</td>");
                    }
                    //if (firstIob.Inp != 0)
                    //{
                    //    str.Append("<td>");
                    //    str.Append(locationDetailIOB.Inp.ToString("0.########"));
                    //    str.Append("</td>");
                    //}
                    //if (firstIob.Qdii != 0)
                    //{
                    //    str.Append("<td>");
                    //    str.Append(locationDetailIOB.Qdii.ToString("0.########"));
                    //    str.Append("</td>");
                    //}
                    //if (firstIob.Rej != 0)
                    //{
                    //    str.Append("<td>");
                    //    str.Append(locationDetailIOB.Rej.ToString("0.########"));
                    //    str.Append("</td>");
                    //}
                    //if (firstIob.Ccs != 0)
                    //{
                    //    str.Append("<td>");
                    //    str.Append(locationDetailIOB.Ccs.ToString("0.########"));
                    //    str.Append("</td>");
                    //}
                    if (firstIob.CycCnt != 0)
                    {
                        str.Append("<td>");
                        str.Append(locationDetailIOB.CycCnt.ToString("0.########"));
                        str.Append("</td>");
                    }
                    if (firstIob.Other != 0)
                    {
                        str.Append("<td>");
                        str.Append(locationDetailIOB.Other.ToString("0.########"));
                        str.Append("</td>");
                    }

                    str.Append("<td>");
                    str.Append(locationDetailIOB.EndNml.ToString("0.########"));
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(locationDetailIOB.EndInp.ToString("0.########"));
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(locationDetailIOB.EndRej.ToString("0.########"));
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(locationDetailIOB.EndCs.ToString("0.########"));
                    str.Append("</td>");

                    str.Append("<td>");
                    str.Append(locationDetailIOB.End.ToString("0.########"));
                    str.Append("</td>");

                    str.Append("</tr>");
                    #endregion
                }
            }

            //表尾
            str.Append("</tbody>");
            str.Append("</table>");
            return str.ToString();
        }

        #endregion


    }
}
