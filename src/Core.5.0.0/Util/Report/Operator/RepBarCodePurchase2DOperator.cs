using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.INV;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepBarCodePurchase2DOperator : RepTemplate2
    {
        public RepBarCodePurchase2DOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 9;
            //列数   1起始
            this.columnCount = 5;

            this.rowCount = 9;
            //报表头的行数  1起始
            this.leftColumnHeadCount = 0;
            //报表尾的行数  1起始
            this.bottomRowCount = 0;

            this.headRowCount = 0;
        }

        /**
         * 需要拷贝的数据与合并单元格操作
         * 
         * Param pageIndex 页号
         */
        public override void CopyPageValues(int pageIndex)
        {
            this.SetMergedRegionColumn(pageIndex, 0, 3, 0, 4);
            this.SetMergedRegionColumn(pageIndex, 1, 3, 1, 4);
            this.SetMergedRegionColumn(pageIndex, 4, 3, 4, 4);
            this.SetMergedRegionColumn(pageIndex, 3, 3, 3, 4);

            this.SetMergedRegionColumn(pageIndex, 5, 0, 5, 2);
            this.SetMergedRegionColumn(pageIndex, 6, 1, 6, 4);
            this.SetMergedRegionColumn(pageIndex, 7, 1, 7, 4);
            this.SetMergedRegionColumn(pageIndex, 8, 4, 8, 4);

            this.SetColumnCell(pageIndex, 0, 2, "供应商");
            this.SetColumnCell(pageIndex, 1, 2, "物料");
            this.SetColumnCell(pageIndex, 2, 2, "数量");
            this.SetColumnCell(pageIndex, 3, 2, "参考号");
            this.SetColumnCell(pageIndex, 4, 2, "FIFO期限");
            this.SetColumnCell(pageIndex, 5, 3, "供应商批号");
            this.SetColumnCell(pageIndex, 6, 0, "制造\r\n日期");
            this.SetColumnCell(pageIndex, 7, 0, "物料描述");
            this.SetColumnCell(pageIndex, 8, 0, "打印人");
            this.SetColumnCell(pageIndex, 8, 2, "打印时间");
        }

        /**
         * 填充报表
         * 
         * Param list [0]huDetailList
         */
        protected override bool FillValuesImpl(String templateFileName, IList<object> list)
        {
            try
            {
                IList<PrintHu> huList = null;
                if (list[0].GetType() == typeof(PrintHu))
                {
                    huList = new List<PrintHu>();
                    huList.Add((PrintHu)list[0]);
                }
                else if (list[0].GetType() == typeof(List<PrintHu>))
                {
                    huList = (IList<PrintHu>)list[0];
                }
                else
                {
                    return false;
                }

                string userName = "";
                if (list.Count == 2)
                {
                    userName = (string)list[1];
                }

                this.sheet.DisplayGridlines = false;
                this.sheet.IsPrintGridlines = false;

                //this.sheet.DisplayGuts = false;

                int count = 0;
                foreach (PrintHu hu in huList)
                {
                    count++;
                }

                if (count == 0) return false;

                this.barCodeFontName = this.GetBarcodeFontName(6, 0);

                //加页删页
                //纵向打印
                this.CopyPageCloumn(count, columnCount, 1);

                int pageIndex = 1;

                foreach (PrintHu hu in huList)
                {
                    this.Fill2DBarCodeImage(pageIndex, 0, 0, 1, 3, hu.HuId);
                    //code供应商
                    this.SetColumnCell(pageIndex, 0, 3, string.Format("{0} {1}", hu.ManufactureParty, hu.ManufacturePartyDescription));
                    //物料代码
                    this.SetColumnCell(pageIndex, 1, 3, hu.Item);

                    //数量+单位
                    this.SetColumnCell(pageIndex, 2, 3, string.Format("{0} {1}", hu.Qty.ToString("0.###"), hu.Uom));
                    //送货单,如果是克隆条码,则显示参考条码号
                    if (!string.IsNullOrWhiteSpace(hu.RefHu))
                    {
                        this.SetColumnCell(pageIndex, 2, 4, hu.RefHu);
                    }
                    else
                    {
                        this.SetColumnCell(pageIndex, 2, 4, hu.IpNo);
                    }
                    //参考物料号
                    this.SetColumnCell(pageIndex, 3, 3, string.Format("{0}/{1}", hu.ReferenceItemCode,hu.Remark));
                    //fifo时间
                    this.SetColumnCell(pageIndex, 4, 3, hu.ExpireDate != null ? hu.ExpireDate.Value.ToString("yyyy-MM-dd") : string.Empty);
                    // 条码号
                    this.SetColumnCell(pageIndex, 5, 0, hu.HuId);
                    //供应商批号
                    this.SetColumnCell(pageIndex, 5, 4, hu.SupplierLotNo);

                    //制造时间
                    this.SetColumnCell(pageIndex, 6, 1, hu.LotNo);

                    //描述
                    this.SetColumnCell(pageIndex, 7, 1, hu.ItemDescription);
                    //打印人
                    this.SetColumnCell(pageIndex, 8, 1, userName);
                    // 打印时间
                    this.SetColumnCell(pageIndex, 8, 3, DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

                    pageIndex++;
                }

            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}
