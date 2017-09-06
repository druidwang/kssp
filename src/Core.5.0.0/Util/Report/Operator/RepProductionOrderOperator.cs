using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.ORD;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepProductionOrderOperator : ReportBase
    {
        private static readonly int PAGE_DETAIL_ROW_COUNT = 15;

        private static readonly int COLUMN_COUNT = 10;

        private static readonly int ROW_COUNT = 40;

        /*
         * 填充报表头
         * 
         * Param pageIndex 页号
         * Param orderHead 订单头对象
         * Param orderDetails 订单明细对象
         */
        protected void FillHead(int pageIndex, PrintOrderMaster orderMaster, IList<PrintOrderDetail> orderDetails)
        {

            #region 报表头

            this.SetRowCell(pageIndex, 0, 7, orderMaster.Sequence.ToString());

            //工单号码Order code
            string orderCode = Utility.BarcodeHelper.GetBarcodeStr(orderMaster.OrderNo, this.barCodeFontName);
            this.SetRowCell(pageIndex, 2, 5, orderCode);

            // "生产线：Prodline："
            //Flow flow = this.flowMgr.LoadFlow(orderHead.Flow);
            this.SetRowCell(pageIndex, 2, 1, orderMaster.FlowDescription + "(" + orderMaster.Flow + ")");
            //"生产班组：Shift："
            this.SetRowCell(pageIndex, 3, 1, orderMaster.Shift == null ? string.Empty : orderMaster.Shift);
            //"发出日期：Release Date："
            this.SetRowCell(pageIndex, 4, 1, orderMaster.CreateDate.ToString("yyyy-MM-dd hh:mm"));
            //"发单人：Issuer："
            this.SetRowCell(pageIndex, 5, 1, orderMaster.CreateUserName);
            //"联系电话：Tel："
            this.SetRowCell(pageIndex, 6, 1, string.Empty);


            // "生产单号：No. PO："
            this.SetRowCell(pageIndex, 3, 7, orderMaster.OrderNo);
            //"交货日期：Deli. Date："
            this.SetRowCell(pageIndex, 4, 7, orderMaster.WindowTime.ToString("yyyy-MM-dd"));
            //"窗口时间：Win Time:"
            this.SetRowCell(pageIndex, 5, 7, orderMaster.WindowTime.ToString("HH:mm"));
            //"交货地点：Shipto："
            this.SetRowCell(pageIndex, 6, 7, orderMaster.PartyToName);

            //"注意事项：Remarks："
            //this.SetRowCell(pageIndex, 7, 1, orderMaster.n);


            //正常 紧急 返工
            if ((CodeMaster.OrderPriority)orderMaster.Priority == CodeMaster.OrderPriority.Urgent)
            {
                this.SetRowCell(pageIndex, 4, 2, "■");
            }
            else
            {
                this.SetRowCell(pageIndex, 3, 2, "■");
            }

            //返工
            if ((CodeMaster.OrderSubType)orderMaster.SubType == CodeMaster.OrderSubType.Return)
            {
                this.SetRowCell(pageIndex, 5, 2, "■");
            }

            #endregion


            int rowIndex = 10;
            #region 产品信息  Product Information
            if (pageIndex == 1)//首页
            {
                foreach (PrintOrderDetail orderDetail in orderDetails)
                {

                    if (rowIndex == 15)
                    {
                        break;
                    }
                    //"成品物料号 FG Item Code"	
                    this.SetRowCell(pageIndex, rowIndex, 0, orderDetail.Item);
                    //"描述Description"	
                    this.SetRowCell(pageIndex, rowIndex, 1, orderDetail.ItemDescription);
                    //"单位Unit"	
                    this.SetRowCell(pageIndex, rowIndex, 2, orderDetail.Uom);
                    //"包装UC"	
                    this.SetRowCell(pageIndex, rowIndex, 3, orderDetail.UnitCount.ToString("0.########"));
                    //"计划数Dmd Qty"	
                    this.SetRowCell(pageIndex, rowIndex, 4, orderDetail.OrderedQty.ToString("0.########"));
                    //"合格数Conf Qty"	
                    //this.SetRowCell(pageIndex, rowIndex, 5, string.Empty);
                    //"不合格数NC Qty"	
                    //this.SetRowCell(pageIndex, rowIndex, 6, string.Empty);
                    //"废品数Scrap Qty"	
                    //this.SetRowCell(pageIndex, rowIndex, 7, string.Empty);
                    //"收货人Receiver"	
                    //this.SetRowCell(pageIndex, rowIndex, 8, string.Empty);
                    // "收货日期Rct Date"
                    //this.SetRowCell(pageIndex, rowIndex, 9, string.Empty);

                    rowIndex++;


                }
            }
            #endregion

        }

        /**
         * 需要拷贝的数据与合并单元格操作
         * 
         * Param pageIndex 页号
         */
        public override void CopyPageValues(int pageIndex)
        {


        }


        /**
         * 填充报表
         * 
         * Param list [0]订单头对象
         *            [1]订单明细对象
         *            [2]订单库位事物对象
         */
        public override bool FillValues(string templateFileFolder, String templateFileName, IList<object> list)
        {
            try
            {
                this.init(templateFileFolder.Replace("\r\n\t\t", "") + templateFileName, ROW_COUNT);
                if (list == null || list.Count < 3) return false;

                PrintOrderMaster orderMaster = (PrintOrderMaster)(list[0]);
                IList<PrintOrderDetail> orderDetails = (IList<PrintOrderDetail>)(list[1]);
                IList<PrintOrderBomDetail> orderBomDetails = (IList<PrintOrderBomDetail>)(list[2]);

                int pageIndex = 1;
                int pageCount = 1;

                this.barCodeFontName = this.GetBarcodeFontName(2, 5);
                this.FillHead(pageIndex, orderMaster, orderDetails);

                #region 物料信息  Material Information

                int rowIndex = 17;

                foreach (var orderBomDetail in orderBomDetails)
                {
                    //"物料号Item Code"	
                    this.SetRowCell(pageIndex, rowIndex, 0, orderBomDetail.Item);
                    // "描述Description"	
                    this.SetRowCell(pageIndex, rowIndex, 1, orderBomDetail.ItemDescription);
                    // "单位Unit"	
                    this.SetRowCell(pageIndex, rowIndex, 2, orderBomDetail.Uom);
                    // "单位用量Unit Qty"	
                    this.SetRowCell(pageIndex, rowIndex, 3, orderBomDetail.UnitQty.ToString("0.########"));
                    // "计划数Dmd Qty"	
                    this.SetRowCell(pageIndex, rowIndex, 4, orderBomDetail.OrderedQty.ToString("0.########"));
                    //decimal orderedQty = orderLocationTransactions.Where(olt => olt.IOType == BusinessConstants.IO_TYPE_OUT).Where(olt => olt.Item.Code == orderLocationTransaction.Item.Code).Sum(olt => olt.OrderedQty);
                    //this.SetRowCell(pageIndex, rowIndex, 4, orderedQty.ToString("0.########"));

                    this.SetRowCell(pageIndex, rowIndex, 8, orderBomDetail.IsScanHu ? "√" : string.Empty);
                    rowIndex++;
                    // }
                    if ((rowIndex - 17) == PAGE_DETAIL_ROW_COUNT)
                    {
                        break;
                    }


                }
                /*foreach (OrderLocationTransaction orderLocationTransaction in orderLocationTransactions)
                {
                    // if (orderLocationTransaction.IOType.Equals(BusinessConstants.IO_TYPE_OUT))
                    // {

                    //"物料号Item Code"	
                    this.SetRowCell(pageIndex, rowIndex, 0, orderLocationTransaction.Item.Code);
                    // "描述Description"	
                    this.SetRowCell(pageIndex, rowIndex, 1, orderLocationTransaction.Item.Description);
                    // "单位Unit"	
                    this.SetRowCell(pageIndex, rowIndex, 2, orderLocationTransaction.Item.Uom.Code);
                    // "单位用量Unit Qty"	
                    this.SetRowCell(pageIndex, rowIndex, 3, orderLocationTransaction.UnitQty.ToString("0.########"));
                    // "计划数Dmd Qty"	
                    this.SetRowCell(pageIndex, rowIndex, 4, orderLocationTransaction.OrderedQty.ToString("0.########"));
                    //decimal orderedQty = orderLocationTransactions.Where(olt => olt.IOType == BusinessConstants.IO_TYPE_OUT).Where(olt => olt.Item.Code == orderLocationTransaction.Item.Code).Sum(olt => olt.OrderedQty);
                    //this.SetRowCell(pageIndex, rowIndex, 4, orderedQty.ToString("0.########"));

                    // "实消耗Act Qty"	
                    //this.SetRowCell(pageIndex, rowIndex, 5, string.Empty);
                    // "批号Lot"			
                    //this.SetRowCell(pageIndex, rowIndex, 6, string.Empty);
                    //"备注Remark"
                    //this.SetRowCell(pageIndex, rowIndex, 9, string.Empty);

                    rowIndex++;
                    // }
                    if ((rowIndex - 17) == PAGE_DETAIL_ROW_COUNT)
                    {
                        break;
                    }
                 * 
                }*/

                #endregion

                //人员信息  People Information
                //空

                //生产记录  Production Information		
                //空
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }
}



