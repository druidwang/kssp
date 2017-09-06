using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.INV;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepBarCodeFI2DOperator : RepTemplate2
    {
        public RepBarCodeFI2DOperator()
        {

            //明细部分的行数
            this.pageDetailRowCount = 10;
            //列数   1起始
            this.columnCount = 5;

            this.rowCount = 10;
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
            this.SetMergedRegionColumn(pageIndex, 2, 3, 2, 4);

            this.SetMergedRegionColumn(pageIndex, 3, 3, 3, 4);
            this.SetMergedRegionColumn(pageIndex, 5, 0, 5, 2);
            this.SetMergedRegionColumn(pageIndex, 4, 3, 5, 4);
            this.SetMergedRegionColumn(pageIndex, 6, 1, 6, 4);
            this.SetMergedRegionColumn(pageIndex, 7, 1, 7, 4);
            this.SetMergedRegionColumn(pageIndex, 8, 1, 8, 3);
            this.SetMergedRegionColumn(pageIndex, 8, 4, 9, 4);


            this.SetColumnCell(pageIndex, 0, 2, "制造商");
            this.SetColumnCell(pageIndex, 0, 3, "申雅密封件有限公司");
            this.SetColumnCell(pageIndex, 1, 2, "物料");
            this.SetColumnCell(pageIndex, 2, 2, "数量");
            this.SetColumnCell(pageIndex, 3, 2, "备注");
            this.SetColumnCell(pageIndex, 6, 0, "制造\r\n日期");
            this.SetColumnCell(pageIndex, 7, 0, "参考号");
            this.SetColumnCell(pageIndex, 8, 0, "物料描述");
            this.SetColumnCell(pageIndex, 9, 0, "打印人");
            this.SetColumnCell(pageIndex, 9, 2, "打印时间");
            this.SetColumnCell(pageIndex, 8, 4, "贴合格证");
            //this.CopyCellColumn(pageIndex, 0, 0, "A1");
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
                    // 制造商
                    //this.SetColumnCell(pageIndex, 0, 3, hu.ManufacturePartyDescription);
                    // 物料号
                    this.SetColumnCell(pageIndex, 1, 3, hu.Item);
                    //数  量
                    this.SetColumnCell(pageIndex, 2, 3, string.Format("{0} {1}", hu.Qty.ToString("0.###"), hu.Uom));
                    //班  次
                    //this.SetColumnCell(pageIndex, 2, 4, GetShiftName(hu.Shift));
                    //备注 如果是克隆条码,则显示参考条码号,否则显示生产单号和班次号
                    if (!string.IsNullOrWhiteSpace(hu.RefHu))
                    {
                        this.SetColumnCell(pageIndex, 3, 3, hu.RefHu);
                    }
                    else
                    {
                        this.SetColumnCell(pageIndex, 3, 3, string.Format("{0} {1}", hu.OrderNo, hu.Shift));
                    }
                    //车  型
                    this.SetColumnCell(pageIndex, 4, 3, hu.MaterialsGroup);
                    //条码号
                    this.SetColumnCell(pageIndex, 5, 0, hu.HuId);
                    //制造日期 
                    this.SetColumnCell(pageIndex, 6, 1, string.Format("{0}", hu.ManufactureDate.ToString("yyyyMMdd")));
                    //参考号
                    this.SetColumnCell(pageIndex, 7, 1, hu.ReferenceItemCode);
                    // 物料名称
                    this.SetColumnCell(pageIndex, 8, 1, hu.ItemDescription);
                    ////包装物
                    //this.SetColumnCell(pageIndex, 1, 4, hu.ManufactureParty);
                    ////// 条形码
                    ////string barCode = Utility.BarcodeHelper.GetBarcodeStr(hu.HuId, this.barCodeFontName);
                    ////this.SetColumnCell(pageIndex, 5, 0, barCode);
                    //打印人
                    this.SetColumnCell(pageIndex, 9, 1, userName);
                    ////备注
                    //this.SetColumnCell(pageIndex, 2, 3, hu.Remark);
                    // 打印时间
                    this.SetColumnCell(pageIndex, 9, 3, DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
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
