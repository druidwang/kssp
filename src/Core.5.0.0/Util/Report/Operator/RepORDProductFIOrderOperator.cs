namespace com.Sconit.Utility.Report.Operator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Entity;
    using com.Sconit.PrintModel.ORD;

    public class RepORDProductFIOrderOperator : RepTemplate1
    {
        public RepORDProductFIOrderOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 25;
            //列数   1起始
            this.columnCount = 7;
            //报表头的行数  1起始
            this.headRowCount = 8;
            //报表尾的行数  1起始
            this.bottomRowCount = 1;
        }

        /**
         * 填充报表
         * 
         * Param list [0]OrderHead
         * Param list [0]IList<OrderDetail>           
         */

        protected override bool FillValuesImpl(String templateFileName, IList<object> list)
        {
            try
            {
                if (list == null || list.Count < 2) return false;

                PrintOrderMaster orderHead = (PrintOrderMaster)(list[0]);
                IList<PrintOrderDetail> orderDetails = (IList<PrintOrderDetail>)(list[1]);

                orderDetails = orderDetails.OrderBy(o => o.Sequence).ThenBy(o => o.Item).ToList();

                if (orderHead == null
                    || orderDetails == null || orderDetails.Count == 0)
                {
                    return false;
                }

                this.barCodeFontName = this.GetBarcodeFontName(0, 4);
                this.CopyPage(orderDetails.Count);
                //Get Remark for head area
                orderHead.remark = orderDetails.ToList().First().Remark;
                this.FillHead(orderHead);


                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;
                foreach (PrintOrderDetail orderDetail in orderDetails)
                {
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
                    //收货数---Close,Complete,Cancel显示
                    if (orderHead.Status == 3 || orderHead.Status == 4 || orderHead.Status == 5)
                    {
                        this.SetRowCell(pageIndex, rowIndex, 5, orderDetail.ReceivedQty.ToString("0.##"));
                    }
                    //备注	
                    this.SetRowCell(pageIndex, rowIndex, 6, orderDetail.Remark);
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

                    if (this.isPageBottom(rowIndex, rowTotal))//页的最后一行
                    {
                        pageIndex++;
                        rowIndex = 0;
                    }
                    else
                    {
                        rowIndex++;
                    }
                    rowTotal++;
                }

                this.sheet.DisplayGridlines = false;
                this.sheet.IsPrintGridlines = false;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        /*
         * 填充报表头
         * 
         * Param repack
         */
        private void FillHead(PrintOrderMaster orderHead)
        {
            #region 报表头
            if (orderHead.SubType == 1)
            {
                this.SetRowCell(1, 3, "退货");
            }
            //this.SetRowCell(pageIndex, 0, 4, orderHead.Sequence.ToString());

            //工单号码Order code
            string orderCode = Utility.BarcodeHelper.GetBarcodeStr(orderHead.OrderNo, this.barCodeFontName);
            this.SetRowCell(0, 4, orderCode);
            this.SetRowCell(1, 4, orderHead.OrderNo);
            // "生产线：Prodline："
            //Flow flow = this.flowMgr.LoadFlow(orderHead.Flow);
            this.SetRowCell(2, 1, orderHead.FlowDescription + "(" + orderHead.Flow + ")");
            //"生产班组：Shift："
            this.SetRowCell(2, 4, orderHead.Shift == null ? string.Empty : orderHead.Shift);
            //"发单人：Issuer："
            this.SetRowCell(3, 1, orderHead.CreateUserName);
            //"交货地点：Shipto："
            this.SetRowCell(3, 4, orderHead.PartyToName);
            //开始时间
            this.SetRowCell(4, 1, orderHead.StartTime.ToString("yyyy-MM-dd HH:mm"));
            //结束时间
            this.SetRowCell(4, 4, orderHead.WindowTime.ToString("yyyy-MM-dd HH:mm"));
            //"注意事项：Remarks："
            this.SetRowCell(5, 1, orderHead.Dock);
            //"联系电话：Tel："
            //this.SetRowCell(pageIndex, 6, 1, string.Empty);


            // "生产单号：No. PO："
            //this.SetRowCell(pageIndex, 3, 7, orderHead.OrderNo);
            //"交货日期：Deli. Date："
            //this.SetRowCell(pageIndex, 4, 7, orderHead.WindowTime.ToString("yyyy-MM-dd"));
            //"发出日期：Release Date："
            //this.SetRowCell(pageIndex, 4, 1, orderHead.CreateDate.ToString("yyyy-MM-dd hh:mm"));
            //"窗口时间：Win Time:"
            //this.SetRowCell(pageIndex, 5, 7, orderHead.WindowTime.ToString("HH:mm"));

            //正常 紧急 返工
            //if ((CodeMaster.OrderPriority)orderHead.Priority == CodeMaster.OrderPriority.Urgent)
            //{
            //    this.SetRowCell(pageIndex, 4, 2, "■");
            //}
            //else
            //{
            //    this.SetRowCell(pageIndex, 3, 2, "■");
            //}

            ////返工
            //if ((CodeMaster.OrderSubType)orderHead.SubType == CodeMaster.OrderSubType.Return)
            //{
            //    this.SetRowCell(pageIndex, 5, 2, "■");
            //}

            #endregion
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            //this.SetMergedRegionColumn(pageIndex, 0, 2, 0, 3);
            //this.SetMergedRegionColumn(pageIndex, 1, 2, 1, 3);
            ////合并整页的明细
            //int i;
            //for (i = 0; i < this.pageDetailRowCount; i++)
            //{
            //    this.SetMergedRegionColumn(pageIndex, i, 2, i, 3);
            //}
            this.CopyCell(pageIndex, 33, 0, "A34");
            this.CopyCell(pageIndex, 33, 2, "C34");

        }
    }
}
