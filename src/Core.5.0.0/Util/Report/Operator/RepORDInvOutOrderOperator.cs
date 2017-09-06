namespace com.Sconit.Utility.Report.Operator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Entity;
    using com.Sconit.PrintModel.ORD;

    public class RepORDInvOutOrderOperator : RepTemplate1
    {
        public RepORDInvOutOrderOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 20;
            //列数   1起始
            this.columnCount = 11;
            //报表头的行数  1起始
            this.headRowCount = 17;
            //报表尾的行数  1起始
            this.bottomRowCount = 2;
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
                    this.SetRowCell(pageIndex, rowIndex, 7, orderDetail.OrderedQty.ToString("0.####"));

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
                        this.SetRowCell(pageIndex, rowIndex, 8, "");
                    }
                    else
                    {
                        this.SetRowCell(pageIndex, rowIndex, 8, orderDetail.ShippedQty.ToString("0.####"));
                    }
                    //实收 Received	零件数
                    if (orderDetail.ReceivedQty == 0)
                    {
                        this.SetRowCell(pageIndex, rowIndex, 9, "");
                    }
                    else
                    {
                        this.SetRowCell(pageIndex, rowIndex, 9, orderDetail.ReceivedQty.ToString("0.####"));
                    }


                    // 批号/备注
                    this.SetRowCell(pageIndex, rowIndex, 10, string.Format("{0} {1}", orderDetail.Direction, orderDetail.Remark));

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
                this.SetRowCell(2, 6, "退货");
            }
            //订单号:
            string orderCode = Utility.BarcodeHelper.GetBarcodeStr(orderHead.OrderNo, this.barCodeFontName);
            this.SetRowCell(0, 7, orderCode);
            //Order No.:
            this.SetRowCell(2, 7, orderHead.OrderNo);
            //参考订单号
            this.SetRowCell(6, 2, orderHead.ReferenceOrderNo);
            //外部订单号
            this.SetRowCell(6, 7, orderHead.ExternalOrderNo);
            //if (orderHead.SubType == (short)com.Sconit.CodeMaster.OrderSubType.Return)
            //{
            //    this.SetRowCell(0, 6, "采购退货单：");
            //}
            if (orderHead.Priority == (short)com.Sconit.CodeMaster.OrderPriority.Normal)
            {
                this.SetRowCell(4, 2, "☑正常");
            }
            else
            {
                this.SetRowCell(4, 2, "☑紧急");
            }
            if (orderHead.QualityType == 0)
            {
                this.SetRowCell(4, 3, "质量状态:合格");
            }
            else
            {
                this.SetRowCell(4, 3, "质量状态:不合格");
            }

            //制单时间 Create Time:
            this.SetRowCell(5, 2, orderHead.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"));

            //目的(收货)区域代码	
            //this.SetRowCell(8, 2, orderHead.PartyTo);
            //开始时间 Start Time:
            this.SetRowCell(4, 8, orderHead.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));


            //目的(收货)区域名称 Region To:	
            this.SetRowCell(8, 2, string.Format("{0} {1}", orderHead.PartyTo, orderHead.PartyToName));
            //发货区域名称 Region To:	
            this.SetRowCell(12, 2, string.Format("{0} {1}", orderHead.PartyFrom, orderHead.PartyFromName));
            //窗口时间 Window Time:
            this.SetRowCell(5, 7, orderHead.WindowTime.ToString("yyyy-MM-dd HH:mm:ss"));

            //供应商地址/道口 Address:	
            string AddressDock = string.Format("{0}/{1}", orderHead.ShipToAddress, orderHead.Dock);
            this.SetRowCell(10, 2, AddressDock);
            //发货地址 Address:	
            this.SetRowCell(14, 2, orderHead.ShipFromAddress);
            //道口:
            //this.SetRowCell(10, 2, orderHead.Dock);

            //收货方联系人 Contact:	
            this.SetRowCell(8, 7, orderHead.ShipToContact);
            //物流协调员 Follow Up:
            this.SetRowCell(12, 7, orderHead.ShipFromContact);

            //收货方电话 Telephone:		
            this.SetRowCell(9, 7, string.Format("{0} {1}", orderHead.ShipToTel, orderHead.ShipToCell));
            //发货方电话 Telephone:
            this.SetRowCell(13, 7, string.Format("{0} {1}", orderHead.ShipFromTel, orderHead.ShipFromCell));

            //收货库位
            string LocTo = orderHead.LocationTo;
            if (!string.IsNullOrWhiteSpace(orderHead.LocationToName))
            {
                LocTo = LocTo + " " + orderHead.LocationToName;
            }
            this.SetRowCell(9, 2, LocTo);
            //发货库位
            string LocFrom = orderHead.LocationFrom;
            if (!string.IsNullOrWhiteSpace(orderHead.LocationFromName))
            {
                LocFrom = LocFrom + " " + orderHead.LocationFromName;
            }
            this.SetRowCell(13, 2, LocFrom);
            //供应商传真 Fax:	
            this.SetRowCell(10, 7, orderHead.ShipToFax);
            //YFV传真 Fax:
            this.SetRowCell(14, 7, orderHead.ShipFromFax);

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
                this.SetMergedRegionColumn(pageIndex, i, 2, i, 4);
            }

            this.CopyCell(pageIndex, 37, 1, "B38");
            this.CopyCell(pageIndex, 37, 7, "G38");
            this.CopyCell(pageIndex, 38, 1, "B39");
            this.CopyCell(pageIndex, 38, 7, "G39");
        }
    }
}
