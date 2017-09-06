namespace com.Sconit.Utility.Report.Operator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Entity;
    using com.Sconit.PrintModel.ORD;

    public class RepORDPurchaseOrderOperator : RepTemplate1
    {
        public RepORDPurchaseOrderOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 20;
            //列数   1起始
            this.columnCount = 11;
            //报表头的行数  1起始
            this.headRowCount = 25;
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

                this.barCodeFontName = this.GetBarcodeFontName(0, 7);
                this.CopyPage(orderDetails.Count);

                this.FillHead(orderHead);


                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;
                foreach (PrintOrderDetail orderDetail in orderDetails)
                {
                    // No.	
                    this.SetRowCell(pageIndex, rowIndex, 0, "" + orderDetail.Sequence);

                    //零件号 Item Code
                    this.SetRowCell(pageIndex, rowIndex, 1, orderDetail.Item);

                    string itemDescription = orderDetail.ItemDescription;
                    if (!string.IsNullOrWhiteSpace(orderDetail.ReferenceItemCode))
                    {
                        itemDescription = itemDescription + "[" + orderDetail.ReferenceItemCode + "]";
                    }
                    //参考号 Ref No.
                    this.SetRowCell(pageIndex, rowIndex, 2, itemDescription);

                    //描述Description
                    //this.SetRowCell(pageIndex, rowIndex, 3, orderDetail.ItemDescription);

                    //单位Unit
                    this.SetRowCell(pageIndex, rowIndex, 5, orderDetail.Uom);

                    //单包装UC
                    this.SetRowCell(pageIndex, rowIndex, 6, orderDetail.UnitCount.ToString("0.####"));

                    //需求 Request	包装
                    //int UCs = (int)Math.Ceiling(orderDetail.OrderedQty / orderDetail.UnitCount);
                    //this.SetRowCell(pageIndex, rowIndex, 6, UCs.ToString());

                    //需求 Request	零件数
                    this.SetRowCell(pageIndex, rowIndex, 4, orderDetail.OrderedQty.ToString("0.####"));

                    ////单价
                    //decimal unitPrice = orderDetail.UnitPrice.HasValue ? orderDetail.UnitPrice.Value : 0M;
                    //string up = unitPrice.ToString("0.####");
                    //if (orderDetail.IsProvisionalEstimate)
                    //{
                    //    up = "*" + up;
                    //}

                    //this.SetRowCell(pageIndex, rowIndex, 7, up);
                    ////金额
                    //this.SetRowCell(pageIndex, rowIndex, 8, (orderDetail.OrderedQty * unitPrice).ToString("0.####"));

                    //发货数
                    if (orderDetail.ShippedQty == 0)
                    {
                        this.SetRowCell(pageIndex, rowIndex, 7, "");
                    }
                    else
                    {
                        this.SetRowCell(pageIndex, rowIndex, 7, orderDetail.ShippedQty.ToString("0.####"));
                    }
                    //实收 Received	零件数
                    if (orderDetail.ReceivedQty == 0)
                    {
                        this.SetRowCell(pageIndex, rowIndex, 8, "");
                    }
                    else
                    {
                        this.SetRowCell(pageIndex, rowIndex, 8, orderDetail.ReceivedQty.ToString("0.####"));
                    }

                    // 批号/备注
                    this.SetRowCell(pageIndex, rowIndex, 9, orderDetail.Remark);

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
        private void FillHead(PrintOrderMaster orderHead)
        {
            if (orderHead.SubType == 1)
            {
                if (orderHead.QualityType == 0)
                {
                    this.SetRowCell(2, 6, "退货");
                }
                else
                {
                    this.SetRowCell(2, 6, "不合格品退货");
                }
            }

            //订单号:
            string orderCode = Utility.BarcodeHelper.GetBarcodeStr(orderHead.OrderNo, this.barCodeFontName);//orderHead.IpNo can not be null or error will appear
            this.SetRowCell(0, 7, orderCode);
            //Order No.:
            this.SetRowCell(2, 7, orderHead.OrderNo);

            if (orderHead.Priority == (short)com.Sconit.CodeMaster.OrderPriority.Normal)
            {
                this.SetRowCell(4, 2, "☑正常");
            }
            else
            {
                this.SetRowCell(4, 2, "☑紧急");
            }

            //打印时间
            this.SetRowCell(6, 2, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            //制单时间 Create Time:
            this.SetRowCell(4, 7, orderHead.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"));

            //窗口时间 Window Time:
            this.SetRowCell(6, 7, orderHead.WindowTime.ToString("yyyy-MM-dd HH:mm:ss"));

            //*****收货方信息
            //目的(收货)区域名称 Region To:	
            this.SetRowCell(9, 2, string.Format("{0} {1}", orderHead.PartyTo, orderHead.PartyToName));

            //收货库位
            string LocTo = orderHead.LocationTo;
            if (!string.IsNullOrWhiteSpace(orderHead.LocationToName))
            {
                LocTo = LocTo + " " + orderHead.LocationToName;
            }
            this.SetRowCell(11, 2, LocTo);
            //供应商地址/道口 Address:	
            string AddressDock = string.Format("{0}/{1}", orderHead.ShipToAddress, orderHead.Dock);
            this.SetRowCell(13, 2, AddressDock);

            //收货方联系人 Contact:	
            this.SetRowCell(9, 7, orderHead.ShipToContact);

            //收货方电话 Telephone:		
            this.SetRowCell(11, 7, string.Format("{0} {1}", orderHead.ShipToTel, orderHead.ShipToCell));

            //收货方传真 Fax:	
            this.SetRowCell(13, 7, orderHead.ShipToFax);

            //*****发货方信息
            //供应商代码 Supplier Code:	
            this.SetRowCell(16, 2, orderHead.PartyFrom);

            //供应商名称 Supplier Name:		
            this.SetRowCell(18, 2, orderHead.PartyFromName);

            //供应商地址 Address:	
            this.SetRowCell(20, 2, orderHead.ShipFromAddress);

            //目的区域地址
            //this.SetRowCell(11, 2, orderHead.ShipToAddress);

            //供应商联系人 Contact:	
            this.SetRowCell(16, 7, orderHead.ShipFromContact);
            //供应商电话
            this.SetRowCell(18, 7, orderHead.ShipFromCell);
            //YFV传真 Fax:
            this.SetRowCell(20, 7, orderHead.ShipFromFax);

            //this.SetRowCell(9, 2, LocTo);

            ////物流协调员 Follow Up:
            //this.SetRowCell(12, 6, orderHead.ShipFromContact);

            ////收货方电话 Telephone:		
            //this.SetRowCell(9, 6, orderHead.ShipToTel);
            ////发货方电话 Telephone:
            //this.SetRowCell(13, 6, orderHead.ShipFromTel);

            //string LocFrom = orderHead.LocationFrom;
            //if (!string.IsNullOrWhiteSpace(orderHead.LocationFromName))
            //{
            //    LocFrom = LocFrom + "[" + orderHead.LocationFromName + "]";
            //}
            //this.SetRowCell(13, 2, LocFrom);
            ////供应商传真 Fax:	
            //this.SetRowCell(10, 6, orderHead.ShipToFax);

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
                this.SetMergedRegionColumn(pageIndex, i, 2, i, 3);
            }
            //备注也要合并
            for (i = 0; i < this.pageDetailRowCount; i++)
            {
                this.SetMergedRegionColumn(pageIndex, i, 9, i, 10);
            }
            this.CopyCell(pageIndex, 45, 2, "C46");
            this.CopyCell(pageIndex, 45, 6, "G46");
            //this.CopyCell(pageIndex, 48, 9, "J49");
            //this.CopyCell(pageIndex, 49, 0, "A50");
        }
    }
}
