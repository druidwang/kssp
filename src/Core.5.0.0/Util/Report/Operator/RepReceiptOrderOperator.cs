using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.ORD;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepReceiptOrderOperator : RepTemplate1
    {
        public RepReceiptOrderOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 20;
            //列数   1起始
            this.columnCount = 10;
            //报表头的行数  1起始
            this.headRowCount = 16;
            //报表尾的行数  1起始
            this.bottomRowCount = 2;
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

                this.barCodeFontName = this.GetBarcodeFontName(0, 6);
                //this.SetRowCellBarCode(0, 2, 5);
                this.CopyPage(receiptDetailList.Count);
                //get location infromation from detail list
                receiptMaster.LocationFrom = receiptDetailList.First().LocationFrom;
                receiptMaster.LocationFromName = receiptDetailList.First().LocationFromName;
                receiptMaster.LocationTo = receiptDetailList.First().LocationTo;
                receiptMaster.LocationToName = receiptDetailList.First().LocationToName;

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
                    //this.SetRowCell(pageIndex, rowIndex, 3, receiptDetail.ReferenceItemCode);
                    //描述
                    this.SetRowCell(pageIndex, rowIndex, 3, receiptDetail.ItemDescription);
                    //单位
                    this.SetRowCell(pageIndex, rowIndex, 5, receiptDetail.Uom);
                    //单包装
                    this.SetRowCell(pageIndex, rowIndex, 6, receiptDetail.UnitCount.ToString("0.########"));
                    //差异
                    this.SetRowCell(pageIndex, rowIndex, 7, receiptDetail.IpDetailType == (int)com.Sconit.CodeMaster.IpDetailType.Gap ? "√" : "");
                    //目的库位
                    //decimal shippedQty = receiptDetail.
                    //this.SetRowCell(pageIndex, rowIndex, 6, receiptDetail.LocationTo);
                    //发货数
                    // this.SetRowCell(pageIndex, rowIndex, 4, receiptDetail.);
                    //实收数	数量
                    decimal receivedQty = receiptDetail.ReceivedQty;
                    this.SetRowCell(pageIndex, rowIndex, 9, receivedQty.ToString("0.########"));
                    //实收数  包装
                    //decimal UC = receiptDetail.UnitCount > 0 ? receiptDetail.UnitCount : 1;
                    //int UCs = (int)Math.Ceiling(receivedQty / UC);
                    this.SetRowCell(pageIndex, rowIndex, 8, receiptDetail.BoxQty);

                    if (this.isPageBottom(rowIndex, rowTotal))//页的最后一行
                    {
                        //实际到货时间:
                        //this.SetRowCell(pageIndex, rowIndex, , "");

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
                if (receiptMaster.QualityType == 0)
                {
                    this.SetRowCell(2, 5, "退货");
                }
                else
                {
                    this.SetRowCell(2, 5, "不合格品退货");
                }
            }
            //订单号:
            string orderCode = Utility.BarcodeHelper.GetBarcodeStr(receiptMaster.ReceiptNo, this.barCodeFontName);
            this.SetRowCell(0, 6, orderCode);
            //Order No.:
            this.SetRowCell(2, 6, receiptMaster.ReceiptNo);
            ////采购送货单:
            this.SetRowCell(4, 1, receiptMaster.IpNo);
            //收货时间
            this.SetRowCell(4, 6, receiptMaster.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"));

            //*****收货方信息
            //目的(收货)区域名称 Region To:	
            if (receiptMaster.OrderSubType == 1)
            {
                this.SetRowCell(6, 0, "收货方:");
            }
            this.SetRowCell(6, 1, string.Format("{0} {1}", receiptMaster.PartyTo, receiptMaster.PartyToName));

            //收货库位
            string LocTo = receiptDetail.LocationTo;
            if (!string.IsNullOrWhiteSpace(receiptDetail.LocationToName))
            {
                LocTo = LocTo + " " + receiptDetail.LocationToName;
            }
            this.SetRowCell(7, 1, LocTo);

            //收货地址/道口 Address:	
            string AddressDock = receiptMaster.ShipToAddress + (receiptMaster.Dock == null || receiptMaster.Dock.Trim() == "" ? "" : "/" + receiptMaster.Dock);
            this.SetRowCell(8, 1, AddressDock);

            //收货方联系人 Contact:	
            this.SetRowCell(6, 6, receiptMaster.ShipToContact);

            //收货方电话 Telephone:		
            this.SetRowCell(7, 6, receiptMaster.ShipToTel);

            //收货方传真 Fax:	
            this.SetRowCell(8, 6, receiptMaster.ShipToFax);


            //*****发货方信息
            //供应商代码 Supplier Code:	
            if (receiptMaster.OrderSubType == 1)
            {
                this.SetRowCell(10, 0, "发货方编码:");
                this.SetRowCell(11, 0, "发货方:");
            }

            this.SetRowCell(10, 1, receiptMaster.PartyFrom);

            //供应商名称 Supplier Name:		
            this.SetRowCell(11, 1, receiptMaster.PartyFromName);

            //供应商地址 Address:	
            this.SetRowCell(12, 1, receiptMaster.ShipFromAddress);

            //供应商联系人 Contact:	
            this.SetRowCell(10, 6, receiptMaster.ShipFromContact);
            //供应商电话
            this.SetRowCell(11, 6, receiptMaster.ShipFromTel);
            //传真 Fax:
            this.SetRowCell(12, 6, receiptMaster.ShipFromFax);
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
            this.CopyCell(pageIndex, 36, 0, "A37");

            this.CopyCell(pageIndex, 36, 6, "G37");

            this.CopyCell(pageIndex, 37, 0, "A38");

            this.CopyCell(pageIndex, 37, 6, "G38");
        }
    }
}
