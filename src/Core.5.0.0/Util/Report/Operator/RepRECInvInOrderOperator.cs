using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.ORD;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepRECInvInOrderOperator : RepTemplate1
    {
        public RepRECInvInOrderOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 20;
            //列数   1起始
            this.columnCount = 9;
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
                    string itemDescription = receiptDetail.ItemDescription;
                    if (!string.IsNullOrWhiteSpace(receiptDetail.ReferenceItemCode))
                    {
                        itemDescription = itemDescription + "[" + receiptDetail.ReferenceItemCode + "]";
                    }
   
                    this.SetRowCell(pageIndex, rowIndex, 3, itemDescription);
                    //描述
                    //this.SetRowCell(pageIndex, rowIndex, 4, receiptDetail.ItemDescription);
                    //单位
                    this.SetRowCell(pageIndex, rowIndex, 5, receiptDetail.Uom);
                    //单包装
                    this.SetRowCell(pageIndex, rowIndex, 6, receiptDetail.UnitCount.ToString("0.########"));
                    //差异
                   // this.SetRowCell(pageIndex, rowIndex, 7, receiptDetail.IpDetailType==(int)com.Sconit.CodeMaster.IpDetailType.Gap?"√":"");
                    //目的库位
                    //decimal shippedQty = receiptDetail.
                    //this.SetRowCell(pageIndex, rowIndex, 6, receiptDetail.LocationTo);
                    //发货数
                    // this.SetRowCell(pageIndex, rowIndex, 7, receiptDetail.ShippedQty.ToString("0.########"));
                    //实收数	数量
                    decimal receivedQty = receiptDetail.ReceivedQty;
                    this.SetRowCell(pageIndex, rowIndex, 8, receivedQty.ToString("0.########"));
                    //实收数  包装
                    //decimal UC = receiptDetail.UnitCount > 0 ? receiptDetail.UnitCount : 1;
                    //int UCs = (int)Math.Ceiling(receivedQty / UC);
                    this.SetRowCell(pageIndex, rowIndex, 7, receiptDetail.BoxQty);

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
                this.SetRowCell(2, 5, "退货");
            }
            //订单号:
            string orderCode = Utility.BarcodeHelper.GetBarcodeStr(receiptMaster.ReceiptNo, this.barCodeFontName);
            this.SetRowCell(0, 6, orderCode);
            //Order No.:
            this.SetRowCell(2, 6, receiptMaster.ReceiptNo);
            //ASN 号
            this.SetRowCell(4, 2, receiptMaster.IpNo);
            //收货时间
            this.SetRowCell(4, 6, receiptMaster.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"));
            //质量状态
            if (receiptMaster.QualityType == 0)
            {
                this.SetRowCell(5, 2, "合格");
            }
            else
            {
                this.SetRowCell(5, 2, "不合格");
            }
            //外部单据号
            this.SetRowCell(5, 6, receiptMaster.ExternalReceiptNo);

            //*****收货方信息
            //目的(收货)区域名称 Region To:	
            this.SetRowCell(7, 2, string.Format("{0} {1}", receiptMaster.PartyTo, receiptMaster.PartyToName));

            //收货库位
            string LocTo = receiptDetail.LocationTo;
            if (!string.IsNullOrWhiteSpace(receiptDetail.LocationToName))
            {
                LocTo = LocTo + " " + receiptDetail.LocationToName;
            }
            this.SetRowCell(8, 2, LocTo);

            //供应商地址/道口 Address:	
            string AddressDock = string.Format("{0}/{1}", receiptMaster.ShipToAddress, receiptMaster.Dock);
            this.SetRowCell(9, 2, AddressDock);

            //收货方联系人 Contact:	
            this.SetRowCell(7, 6, receiptMaster.ShipToContact);

            //收货方电话 Telephone:		
            this.SetRowCell(8, 6, string.Format("{0} {1}", receiptMaster.ShipToTel, receiptMaster.ShipToCell));

            //收货方传真 Fax:	
            this.SetRowCell(9, 6, receiptMaster.ShipToFax);

            //*****发货方信息
            //供应商代码 Supplier Code:	
            this.SetRowCell(11, 2, string.Format("{0} {1}",receiptMaster.PartyFrom,receiptMaster.PartyFromName));

            //供应商名称 Supplier Name:		
            //this.SetRowCell(6, 1, receiptMaster.PartyFromName);

            //发货库位
            string LocFrom = receiptDetail.LocationFrom;
            if (!string.IsNullOrWhiteSpace(receiptDetail.LocationFromName))
            {
                LocFrom = LocFrom + " " + receiptDetail.LocationFromName;
            }
            this.SetRowCell(12, 2, LocFrom);

            //供应商地址 Address:	
            this.SetRowCell(13, 2, receiptMaster.ShipFromAddress);

            //目的区域地址
            //this.SetRowCell(11, 2, receiptMaster.ShipToAddress);

            //供应商联系人 Contact:	
            this.SetRowCell(11, 6, receiptMaster.ShipFromContact);
            //供应商电话
            this.SetRowCell(12, 6, string.Format("{0} {1}", receiptMaster.ShipFromTel, receiptMaster.ShipFromCell));
            //传真 Fax:
            this.SetRowCell(13, 6, receiptMaster.ShipFromFax);
            
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
            //实际入库时间:
            this.CopyCell(pageIndex, 37, 1, "B38");
            //收货方签字:
            this.CopyCell(pageIndex, 37, 5, "F38");
 
        }
    }
}
