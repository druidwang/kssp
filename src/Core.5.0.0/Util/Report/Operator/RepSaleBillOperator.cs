using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.BILL;

namespace com.Sconit.Utility.Report.Operator
{
    /// <summary>
    /// 销售账单
    /// </summary>
    public class RepSaleBillOperator : RepTemplate1
    {
        public RepSaleBillOperator()
        {

            //明细部分的行数
            this.pageDetailRowCount = 32;
            //列数   1起始
            this.columnCount = 12;
            //报表头的行数  1起始
            this.headRowCount = 5;
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
  
                    //客户回单号	
                    this.SetRowCell(pageIndex, rowIndex, 0, billDetail.ExternalReceiptNo);
                    //销售单号
                    this.SetRowCell(pageIndex, rowIndex, 1, billDetail.ReceiptNo);
                    //送货单号	
                    this.SetRowCell(pageIndex, rowIndex, 2, billDetail.IpNo);
                    //物料代码	
                    this.SetRowCell(pageIndex, rowIndex, 3, billDetail.Item);
                    //客户零件号	
                    this.SetRowCell(pageIndex, rowIndex, 4, billDetail.ReferenceItemCode);
                    //零件名称	
                    this.SetRowCell(pageIndex, rowIndex, 5, billDetail.ItemDescription);
                    //出库数量 
                    this.SetRowCell(pageIndex, rowIndex, 6, billDetail.Qty.ToString("#,##0.########"));
                    //单位	
                    this.SetRowCell(pageIndex, rowIndex, 7, billDetail.Uom);
                    //单价
                    this.SetRowCell(pageIndex, rowIndex, 8, billDetail.UnitPrice.ToString("#,##0.########"));
                    //货币
                    this.SetRowCell(pageIndex, rowIndex, 9, billDetail.Currency);
                    //金额
                    this.SetRowCell(pageIndex, rowIndex,10, billDetail.Amount.ToString("#,##0.########"));
                    totalAmount += billDetail.Amount;
                    //生效日期
                    this.SetRowCell(pageIndex, rowIndex, 11, billDetail.EffectiveDate.ToString("yyyy-MM-dd"));

                    if (!this.isPageBottom(rowIndex, rowTotal))//页的最后一行
                    {
                        rowIndex++;
                    }
                    else
                    {
                        pageIndex++;
                        rowIndex = 0;
                        this.SetRowCell(37, 6, totalAmount.ToString("#,##0.########"));
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
            this.SetRowCell(2, 2, bill.CreateDate.ToString("yyyy-MM-dd HH:mm"));

            //最后修改时间：
            this.SetRowCell(2, 6, bill.LastModifyDate.ToString("yyyy-MM-dd HH:mm"));

            //客户
            this.SetRowCell(3, 2, bill.Party);
            //账单号
            this.SetRowCell(3, 8, bill.BillNo);
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            //供应商：
            this.CopyCell(pageIndex, 37, 0, "A38");
            //this.CopyCell(pageIndex, 36, 1, "B37");
            //合计:
            this.CopyCell(pageIndex, 37, 5, "F38");
            // this.CopyCell(pageIndex, 36, 3, "D37");
        }
    }
}
