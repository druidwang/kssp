using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.BILL;

namespace com.Sconit.Utility.Report.Operator
{
    /// <summary>
    /// 采购账单
    /// </summary>
    public class RepPurchaseBillOperator : RepTemplate1
    {
        public RepPurchaseBillOperator()
        {

            //明细部分的行数
            this.pageDetailRowCount = 26;
            //列数   1起始
            this.columnCount = 11;
            //报表头的行数  1起始
            this.headRowCount = 10;
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

                PrintBillMaster billMaster = (PrintBillMaster)(list[0]);
                IList<PrintBillDetail> billDetails = (IList<PrintBillDetail>)(list[1]);


                if (billMaster == null
                    || billDetails == null || billDetails.Count == 0)
                {
                    return false;
                }

                this.CopyPage(billDetails.Count);

                this.FillHead(billMaster);

                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;
                decimal totalAmount = 0;
                foreach (PrintBillDetail billDetail in billDetails)
                {

                    //采购单	
                    this.SetRowCell(pageIndex, rowIndex, 0, billDetail.OrderNo);
                    //送货单号
                    this.SetRowCell(pageIndex, rowIndex, 1, billDetail.IpNo);
                    //收货单号	
                    this.SetRowCell(pageIndex, rowIndex, 2, billDetail.ReceiptNo);
                    //零件号	
                    this.SetRowCell(pageIndex, rowIndex, 3, billDetail.Item);
                    //零件名称	
                    this.SetRowCell(pageIndex, rowIndex, 4, billDetail.ItemDescription);
                    //单位	
                    this.SetRowCell(pageIndex, rowIndex, 5, billDetail.Uom);
                    //入库数量 
                    this.SetRowCell(pageIndex, rowIndex, 6, billDetail.Qty.ToString("#,##0.########"));
                    //采购单价	
                    this.SetRowCell(pageIndex, rowIndex, 7, billDetail.UnitPrice.ToString("#,##0.########"));
                    //货币
                    this.SetRowCell(pageIndex, rowIndex, 8, billDetail.Currency);
                    //金额
                    this.SetRowCell(pageIndex, rowIndex, 9, billDetail.Amount.ToString("#,##0.########"));
                    totalAmount += billDetail.Amount;
                    //入库日期
                    this.SetRowCell(pageIndex, rowIndex, 10, billDetail.EffectiveDate.ToString("yyyy-MM-dd"));
                   
                    if (!this.isPageBottom(rowIndex, rowTotal))//页的最后一行
                    {
                        rowIndex++;
                    }
                    else
                    {
                        pageIndex++;
                        rowIndex = 0;
                        this.SetRowCell(36, 5, totalAmount.ToString("#,##0.########"));
                        totalAmount = 0;
                    }
                    rowTotal++;
                }
                totalAmount = 0;
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
         * Param repack 头对象
         */
        private void FillHead(PrintBillMaster bill)
        {
            //创建时间：
            this.SetRowCell(5, 3, bill.CreateDate.ToString("yyyy-MM-dd HH:mm"));

            //最后修改时间：
            this.SetRowCell(6, 3, bill.LastModifyDate.ToString("yyyy-MM-dd HH:mm"));

            //供应商
            this.SetRowCell(8, 2, bill.Party);
            //账单号
            this.SetRowCell(8, 5, bill.BillNo);
            //对账员
            this.SetRowCell(8, 10, bill.CreateUserName);
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            //供应商：
            this.CopyCell(pageIndex, 36, 0, "A37");
            //this.CopyCell(pageIndex, 36, 1, "B37");
            //合计:
            this.CopyCell(pageIndex, 36, 4, "E37");
           // this.CopyCell(pageIndex, 36, 3, "D37");
        }
    }
}
