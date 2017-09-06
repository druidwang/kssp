using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.ORD;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepKitOrderOperator: RepTemplate1
    {
        public RepKitOrderOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 30;
            //列数   1起始
            this.columnCount = 13;
            //报表头的行数  1起始
            this.headRowCount = 7;
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

                PrintOrderMaster orderMaster = (PrintOrderMaster)(list[0]);
                IList<PrintOrderDetail> orderDetails = (IList<PrintOrderDetail>)(list[1]);
                
                orderDetails = orderDetails.OrderBy(o => o.ZOPID).ThenBy(o => o.Item).ToList();

                if (orderMaster == null
                    || orderDetails == null || orderDetails.Count == 0)
                {
                    return false;
                }


                //this.SetRowCellBarCode(0, 2, 8);
                this.barCodeFontName = this.GetBarcodeFontName(1,3);
                this.CopyPage(orderDetails.Count);

                this.FillHead(orderMaster);


                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;
                foreach (PrintOrderDetail orderDetail in orderDetails)
                {
                    this.SetRowCell(pageIndex, rowIndex, 0, orderDetail.ZOPID);
                    
                    this.SetRowCell(pageIndex, rowIndex, 1, orderDetail.ZOPDS);

                    this.SetRowCell(pageIndex, rowIndex, 2, orderDetail.Item);

                    this.SetRowCell(pageIndex, rowIndex, 4, orderDetail.ReferenceItemCode);

                    this.SetRowCell(pageIndex, rowIndex, 5, orderDetail.ItemDescription);

                    this.SetRowCell(pageIndex, rowIndex, 8, orderDetail.ManufactureParty);

                    this.SetRowCell(pageIndex, rowIndex, 10, orderDetail.Uom);

                    this.SetRowCell(pageIndex, rowIndex, 11, orderDetail.OrderedQty.ToString("0.########"));

                    this.SetRowCell(pageIndex, rowIndex, 12, orderDetail.IsScanHu ? "√" : string.Empty);

                    //if (orderDetail.IsScanHu == true)
                    //{
                    //    this.SetRowCell(pageIndex, rowIndex, 10, "√");
                    //}

                    //批号/备注
                   // this.SetRowCell(pageIndex, rowIndex, 11, "");

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
         * Param repack 报验单头对象
         */
        private void FillHead(PrintOrderMaster orderMaster)
        {
            //顺序号:
            this.SetRowCell(0, 5, orderMaster.Sequence.ToString());
            //订单号:
            if (!string.IsNullOrEmpty(orderMaster.TraceCode))
            {
                string vanCode = Utility.BarcodeHelper.GetBarcodeStr(orderMaster.TraceCode, this.barCodeFontName);
                this.SetRowCell(1, 3, vanCode);
            }
            //Order No.:
            this.SetRowCell(2, 3, orderMaster.TraceCode);

            //订单号:
            string orderCode = Utility.BarcodeHelper.GetBarcodeStr(orderMaster.OrderNo, this.barCodeFontName);
            this.SetRowCell(1, 7, orderCode);
            //Order No.:
            this.SetRowCell(2, 7, orderMaster.OrderNo);

            this.SetRowCell(3, 2, orderMaster.Flow+"["+orderMaster.FlowDescription+"]");

  
            //来源库位
           // this.SetRowCell(3, 5, orderMaster.OrderDetails[0].LocationFrom);

            //发出时间 Create Time:
            this.SetRowCell(3, 8, orderMaster.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"));


            //目的库位
            //this.SetRowCell(4, 5, orderMaster.OrderDetails[0].LocationTo);

            //窗口时间 
            this.SetRowCell(4, 8, orderMaster.WindowTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            this.SetMergedRegion(pageIndex, 7, 5, 7, 7);
            this.SetMergedRegion(pageIndex, 8, 5, 8, 7);
            this.SetMergedRegion(pageIndex, 9, 5, 9, 7);
            this.SetMergedRegion(pageIndex, 10, 5, 10, 7);
            this.SetMergedRegion(pageIndex, 11, 5, 11, 7);
            this.SetMergedRegion(pageIndex, 12, 5, 12, 7);
            this.SetMergedRegion(pageIndex, 13, 5, 13, 7);
            this.SetMergedRegion(pageIndex, 14, 5, 14, 7);
            this.SetMergedRegion(pageIndex, 15, 5, 15, 7);
            this.SetMergedRegion(pageIndex, 16, 5, 16, 7);
            this.SetMergedRegion(pageIndex, 17, 5, 17, 7);
            this.SetMergedRegion(pageIndex, 18, 5, 18, 7);
            this.SetMergedRegion(pageIndex, 19, 5, 19, 7);
            this.SetMergedRegion(pageIndex, 20, 5, 20, 7);
            this.SetMergedRegion(pageIndex, 21, 5, 21, 7);
            this.SetMergedRegion(pageIndex, 22, 5, 22, 7);
            this.SetMergedRegion(pageIndex, 23, 5, 23, 7);
            this.SetMergedRegion(pageIndex, 24, 5, 24, 7);
            this.SetMergedRegion(pageIndex, 25, 5, 25, 7);
            this.SetMergedRegion(pageIndex, 26, 5, 26, 7);
            this.SetMergedRegion(pageIndex, 27, 5, 27, 7);
            this.SetMergedRegion(pageIndex, 28, 5, 28, 7);
            this.SetMergedRegion(pageIndex, 29, 5, 29, 7);
            this.SetMergedRegion(pageIndex, 30, 5, 30, 7);
            this.SetMergedRegion(pageIndex, 31, 5, 31, 7);
            this.SetMergedRegion(pageIndex, 32, 5, 32, 7);
            this.SetMergedRegion(pageIndex, 33, 5, 33, 7);
            this.SetMergedRegion(pageIndex, 34, 5, 34, 7);
            this.SetMergedRegion(pageIndex, 35, 5, 35, 7);
            this.SetMergedRegion(pageIndex, 36, 5, 36, 7);
            this.CopyCell(pageIndex, 37, 0, "A38");
            this.CopyCell(pageIndex, 37, 4, "E38");
            this.CopyCell(pageIndex, 37, 7, "I38");
            
            //this.CopyCell(pageIndex, 50, 1, "B51");
            //this.CopyCell(pageIndex, 50, 5, "F51");
            //this.CopyCell(pageIndex, 50, 9, "J51");
            //this.CopyCell(pageIndex, 51, 0, "A52");
           // this.SetMergedRegion(pageIndex,7, 4, 35, 6);
        }


    }
}
