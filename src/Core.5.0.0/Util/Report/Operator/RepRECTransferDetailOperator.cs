using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.ORD;
using com.Sconit.Entity.ORD;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepRECTransferDetailOperator : RepTemplate1
    {
        public RepRECTransferDetailOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 35;
            //列数   1起始
            this.columnCount = 6;
            //报表头的行数  1起始
            this.headRowCount = 6;
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

                IList<ReceiptDetail> receiptDetailList = (IList<ReceiptDetail>)list[0];

                this.CopyPage(receiptDetailList.Count);

                this.FillHead(list);
                int seq =1;
                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;
                receiptDetailList = receiptDetailList.OrderBy(r => r.Sequence)
                    .ThenBy(r => r.Item).ToList();
                foreach (ReceiptDetail receiptDetail in receiptDetailList)
                {
                    //序号	物料号	物料描述[参考物料号]	单位  包数    数量
                    //序号
                    this.SetRowCell(pageIndex, rowIndex, 0, seq);
                    //物料号
                    this.SetRowCell(pageIndex, rowIndex, 1, receiptDetail.Item);
                    //物料描述[参考物料号]
                    string itemDescription = receiptDetail.ItemDescription;
                    if (!string.IsNullOrWhiteSpace(receiptDetail.ReferenceItemCode))
                    {
                        itemDescription = itemDescription + "[" + receiptDetail.ReferenceItemCode + "]";
                    }
                    this.SetRowCell(pageIndex, rowIndex, 2, receiptDetail.ItemDescription);
                    //单位
                    this.SetRowCell(pageIndex, rowIndex, 3, receiptDetail.Uom);

                    //实收数	数量
                    decimal receivedQty = receiptDetail.ReceivedQty;
                    this.SetRowCell(pageIndex, rowIndex, 5, receivedQty.ToString("0.########"));
                    //实收数  包装
                    //decimal UC = receiptDetail.UnitCount > 0 ? receiptDetail.UnitCount : 1;
                    //int UCs = (int)Math.Ceiling(receivedQty / UC);
                    this.SetRowCell(pageIndex, rowIndex, 4, receiptDetail.BoxQty);
                    seq += 1;
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
        private void FillHead(IList<object> list)
        {
            ////路线:
            this.SetRowCell(2, 2, (string)list[2]);
            //统计时间段
            this.SetRowCell(3, 2,  (string)list[3]);
            //制单人
            this.SetRowCell(2, 4, (string)list[1]);
            //制单时间
            this.SetRowCell(3, 4, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            //合并整页的明细
            //int i;
            //for (i = 0; i < this.pageDetailRowCount; i++)
            //{
            //    this.SetMergedRegionColumn(pageIndex, i, 1, i,2);
            //}
            this.CopyCell(pageIndex, 41, 1, "B42");

            this.CopyCell(pageIndex, 41, 2, "C42");

        }
    }
}
