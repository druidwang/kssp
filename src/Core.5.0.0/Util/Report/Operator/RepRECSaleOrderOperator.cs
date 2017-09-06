using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.ORD;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepRECSaleOrderOperator : RepTemplate1
    {
        public RepRECSaleOrderOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 20;
            //列数   1起始
            this.columnCount = 11;
            //报表头的行数  1起始
            this.headRowCount = 17;
            //报表尾的行数  1起始
            this.bottomRowCount = 1;
        }

        /**
         * 填充报表
         * 
         * Param list [0]Receipt
         *            [1]ReceiptDetailList
         */
        protected override bool FillValuesImpl(String templateFileName, IList<object> list)
        {
            try
            {
                if (list == null || list.Count < 2) return false;

                PrintReceiptMaster receiptMaster = (PrintReceiptMaster)list[0];
                IList<PrintReceiptDetail> receiptDetailList = (IList<PrintReceiptDetail>)list[1];

                if (receiptMaster == null
                    || receiptDetailList == null || receiptDetailList.Count == 0)
                {
                    return false;
                }

                this.barCodeFontName = this.GetBarcodeFontName(0, 5);
                //this.SetRowCellBarCode(0, 2, 5);
                this.CopyPage(receiptDetailList.Count);

                this.FillHead(receiptMaster, receiptDetailList[0]);

                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;
                receiptDetailList = receiptDetailList.OrderBy(r => r.Sequence)
                    .ThenBy(r => r.Item).ToList();
                foreach (PrintReceiptDetail receiptDetail in receiptDetailList)
                {
                    //订单号	序号	零件号	参考号	描述	单位	单包装	发货数	实收数	数量	包数
                    this.SetRowCell(pageIndex, rowIndex, 0, receiptDetail.OrderNo);
                    //序号
                    this.SetRowCell(pageIndex, rowIndex, 1, receiptDetail.Sequence);
                    //零件号
                    this.SetRowCell(pageIndex, rowIndex, 2, receiptDetail.Item);

                    //this.SetRowCell(pageIndex, rowIndex, 1, receiptDetail.ManufactureParty);
                    //参考号
                    this.SetRowCell(pageIndex, rowIndex, 3, receiptDetail.ReferenceItemCode);
                    //描述
                    this.SetRowCell(pageIndex, rowIndex, 4, receiptDetail.ItemDescription);
                    //单位
                    this.SetRowCell(pageIndex, rowIndex, 6, receiptDetail.Uom);
                    //单包装
                    this.SetRowCell(pageIndex, rowIndex, 7, receiptDetail.UnitCount.ToString("0.###"));
                    //差异
                    this.SetRowCell(pageIndex, rowIndex, 8, receiptDetail.IpDetailType == (int)com.Sconit.CodeMaster.IpDetailType.Gap ? "√" : "");
                    //实收数	数量
                    decimal receivedQty = receiptDetail.ReceivedQty;
                    this.SetRowCell(pageIndex, rowIndex, 10, receivedQty.ToString("0.###"));
                    //实收数  包装
                    //decimal UC = receiptDetail.UnitCount > 0 ? receiptDetail.UnitCount : 1;
                    //int UCs = (int)Math.Ceiling(receivedQty / UC);
                    this.SetRowCell(pageIndex, rowIndex, 9, receiptDetail.BoxQty);

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
         * Param pageIndex 页号
         * Param orderHead 订单头对象
         * Param orderDetails 订单明细对象
         */
        private void FillHead(PrintReceiptMaster receiptMaster, PrintReceiptDetail receiptDetail)
        {
            if (receiptMaster.OrderSubType == 1)
            {
                this.SetRowCell(2, 5, "退货");
            }

            string receiptCode = Utility.BarcodeHelper.GetBarcodeStr(receiptMaster.ReceiptNo, this.barCodeFontName);
            //收货单号条码:
            this.SetRowCell(0, 6, receiptCode);
            this.SetRowCell(2, 6, receiptMaster.ReceiptNo);
            //销售送货单:	4.1					
            this.SetRowCell(4, 1, receiptMaster.IpNo);
            //收货时间:	4.6	
            this.SetRowCell(4, 6, receiptMaster.CreateDate.ToString("yyyy-MM-dd HH:mm"));
            //外部订单:	5.1	
            this.SetRowCell(5, 1, receiptMaster.ExternalReceiptNo);

            //收货方
            //客户号/名称:	7.1	
            this.SetRowCell(7, 1, string.Format("{0} {1}", receiptMaster.PartyTo, receiptMaster.PartyToName));
            //联系人:	7.6		
            this.SetRowCell(7, 6, receiptMaster.ShipToContact);
            //收货地址:	8.1		
            this.SetRowCell(8, 1, receiptMaster.ShipToAddress);
            //电话:	8.6		
            this.SetRowCell(8, 6, string.Format("{0} {1}", receiptMaster.ShipToTel, receiptMaster.ShipToCell));
            //卸货道口:	9.1		
            if (receiptMaster.OrderSubType == 1)
            {
                this.SetRowCell(9, 0, "收货库位:");
                this.SetRowCell(9, 1, string.Format("{0} {1}", receiptDetail.LocationTo, receiptDetail.LocationToName));
            }
            else
            {
                this.SetRowCell(9, 1, receiptMaster.Dock);
            }
            //传真:	9.6		
            this.SetRowCell(9, 6, receiptMaster.ShipToFax);

            //发货方
            //发货区域:	11.1	
            this.SetRowCell(11, 1, string.Format("{0} {1}", receiptMaster.PartyFrom, receiptMaster.PartyFromName));
            //联系人:	11.6	
            this.SetRowCell(11, 6, receiptMaster.ShipFromContact);
            //发货库位:	12.1	
            if (receiptMaster.OrderSubType == 1)
            {
                this.SetRowCell(12, 0, "卸货道口:");
                this.SetRowCell(12, 1, receiptMaster.Dock);
            }
            else
            {
                this.SetRowCell(12, 1, string.Format("{0} {1}", receiptDetail.LocationFrom, receiptDetail.LocationFromName));
            }
            //电话:	12.6	
            this.SetRowCell(12, 6, string.Format("{0} {1}", receiptMaster.ShipFromTel, receiptMaster.ShipFromCell));
            //发货地址:	13.1	
            this.SetRowCell(13, 1, receiptMaster.ShipFromAddress);
            //传真:	13.6	
            this.SetRowCell(13, 6, receiptMaster.ShipFromFax);
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            int i;
            for (i = 0; i < this.pageDetailRowCount; i++)
            {
                this.SetMergedRegionColumn(pageIndex, i, 4, i, 5);
            }
            //实际收货时间: 
            this.CopyCell(pageIndex, 37, 1, "B38");
            //收货人签字:
            this.CopyCell(pageIndex, 37, 6, "G38");
        }
    }
}
