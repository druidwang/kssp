using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.ORD;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepORDSaleOrderOperator : RepTemplate1
    {
        public RepORDSaleOrderOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 20;
            //列数   1起始
            this.columnCount = 11;
            //报表头的行数  1起始
            this.headRowCount = 16;
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

                orderDetails = orderDetails.OrderBy(o => o.Sequence).ThenBy(o => o.Item).ToList();

                if (orderMaster == null
                    || orderDetails == null || orderDetails.Count == 0)
                {
                    return false;
                }


                //this.SetRowCellBarCode(0, 2, 8);
                this.barCodeFontName = this.GetBarcodeFontName(2, 8);
                this.CopyPage(orderDetails.Count);

                this.FillHead(orderMaster);


                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;
                foreach (PrintOrderDetail orderDetail in orderDetails)
                {
                    // No.	
                    this.SetRowCell(pageIndex, rowIndex, 0, "" + orderDetail.Sequence);

                    //零件号 Item Code
                    this.SetRowCell(pageIndex, rowIndex, 1, orderDetail.Item);

                    //参考号 Ref No.
                    this.SetRowCell(pageIndex, rowIndex, 2, orderDetail.ReferenceItemCode);

                    //描述Description
                    this.SetRowCell(pageIndex, rowIndex, 3, orderDetail.ItemDescription);

                    //单位Unit
                    this.SetRowCell(pageIndex, rowIndex, 5, orderDetail.Uom);

                    //单包装UC
                    this.SetRowCell(pageIndex, rowIndex, 6, orderDetail.UnitCount.ToString("0.########"));

                    ////需求 Request	包装
                    //int UCs = (int)Math.Ceiling(orderDetail.OrderedQty / orderDetail.UnitCount);
                    //this.SetRowCell(pageIndex, rowIndex, 6, UCs.ToString());

                    //需求 Request	零件数交货数量
                    this.SetRowCell(pageIndex, rowIndex, 7, orderDetail.OrderedQty.ToString("0.########"));

                    //实发数
                    this.SetRowCell(pageIndex, rowIndex, 8, orderDetail.ShippedQty.ToString("0.########"));

                    //实收 Received	包装
                    //this.SetRowCell(pageIndex, rowIndex, 9, "");

                    //实收 Received	零件数
                    this.SetRowCell(pageIndex, rowIndex, 9, orderDetail.ReceivedQty.ToString("0.########"));

                    //批号/备注
                    this.SetRowCell(pageIndex, rowIndex, 10, orderDetail.Remark);

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
            if (orderMaster.SubType == 1)
            {
                this.SetRowCell(2, 6, "退货");
            }
            //订单号:
            string orderCode = Utility.BarcodeHelper.GetBarcodeStr(orderMaster.OrderNo, this.barCodeFontName);
            this.SetRowCell(0, 7, orderCode);
            //Order No.:
            this.SetRowCell(2, 7, orderMaster.OrderNo);
            //参考订单号
            //this.SetRowCell(6, 2, orderMaster.ReferenceOrderNo);
            //外部订单号
            //this.SetRowCell(6, 6, orderMaster.ExternalOrderNo);
            //if (orderMaster.SubType == (short)com.Sconit.CodeMaster.OrderSubType.Return)
            //{
            //    this.SetRowCell(0, 6, "采购退货单：");
            //}
            if (orderMaster.Priority == (short)com.Sconit.CodeMaster.OrderPriority.Normal)
            {
                this.SetRowCell(4, 2, "☑正常");
            }
            else
            {
                this.SetRowCell(4, 2, "☑紧急");
            }

            //打印时间
            this.SetRowCell(5, 2, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            //目的(收货)区域代码	
            //this.SetRowCell(8, 2, orderMaster.PartyTo);
            //发单时间 Start Time:
            this.SetRowCell(4, 7, orderMaster.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));


            ////目的(收货)区域名称 Region To:	 (Equal with 客户号,名称收货。。)
            this.SetRowCell(7, 2, string.Format("{0} {1}", orderMaster.PartyTo, orderMaster.PartyToName));
            //发货区域名称 Region To:	
            this.SetRowCell(11, 2, string.Format("{0} {1}", orderMaster.PartyFrom, orderMaster.PartyFromName));
            //窗口时间 Window Time:
            this.SetRowCell(5, 7, orderMaster.WindowTime.ToString("yyyy-MM-dd HH:mm:ss"));

            //供应商地址/道口 Address:	
            //string AddressDock = orderMaster.ShipToAddress + (orderMaster.Dock == null || orderMaster.Dock.Trim() == "" ? "" : "/" + orderMaster.Dock);
            this.SetRowCell(8, 2, orderMaster.ShipToAddress);
            this.SetRowCell(9, 2, orderMaster.Dock);
            //发货地址 Address:	
            this.SetRowCell(13, 2, orderMaster.ShipFromAddress);
            //道口:
            //this.SetRowCell(10, 2, orderMaster.Dock);

            //收货方联系人 Contact:	
            this.SetRowCell(7, 7, orderMaster.ShipToContact);
            //物流协调员 Follow Up:
            this.SetRowCell(11, 7, orderMaster.ShipFromContact);

            //收货方电话 Telephone:		
            this.SetRowCell(8, 7, string.Format("{0} {1}", orderMaster.ShipToTel, orderMaster.ShipToCell));
            //发货方电话 Telephone:
            this.SetRowCell(12, 7, string.Format("{0} {1}", orderMaster.ShipFromTel, orderMaster.ShipFromCell));

            //收货库位
            //string LocTo = orderMaster.LocationTo;
            //if (!string.IsNullOrWhiteSpace(orderMaster.LocationToName))
            //{
            //    LocTo = LocTo + "[" + orderMaster.LocationToName + "]";
            //}
            //this.SetRowCell(9, 2, LocTo);
            //发货库位
            string LocFrom = orderMaster.LocationFrom;
            if (!string.IsNullOrWhiteSpace(orderMaster.LocationFromName))
            {
                LocFrom = LocFrom + " " + orderMaster.LocationFromName ;
            }
            this.SetRowCell(12, 2, LocFrom);
            //供应商传真 Fax:	
            this.SetRowCell(9, 7, orderMaster.ShipToFax);
            //YFV传真 Fax:
            this.SetRowCell(13, 7, orderMaster.ShipFromFax);

            //系统号 SysCode:
            //this.SetRowCell(++rowNum, 3, "");
            //版本号 Version:
            //this.SetRowCell(rowNum, 8, "");
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            //合并整页的明细
            int i;
            for (i = 0; i < this.pageDetailRowCount; i++)
            {
                this.SetMergedRegionColumn(pageIndex, i, 3, i, 4);
            }
            this.CopyCell(pageIndex, 36, 1, "B37");
            this.CopyCell(pageIndex, 36, 6, "G37");
            //this.CopyCell(pageIndex, 37, 7, "H38");
        }


    }
}
