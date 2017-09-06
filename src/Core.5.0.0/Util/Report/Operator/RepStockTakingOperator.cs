using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.INV;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepStockTakingOperator : RepTemplate2
    {
        public RepStockTakingOperator()
        {

            //明细部分的行数
            this.pageDetailRowCount = 8;
            //列数   1起始
            this.columnCount = 4;

            this.rowCount = 8;
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
            //this.SetMergedRegionColumn(pageIndex, 0, 1, 0, 3);
            //this.SetMergedRegionColumn(pageIndex, 1, 0, 2, 0);
            //this.SetMergedRegionColumn(pageIndex, 5, 1, 5, 3);
            ////this.SetMergedRegionColumn(pageIndex, 6, 1, 6, 3);
            //this.SetMergedRegionColumn(pageIndex, 6, 0, 6, 3);
            //this.SetMergedRegionColumn(pageIndex, 7, 0, 7, 3);

            //this.CopyCellColumn(pageIndex, 0, 0, "A1");
            //this.CopyCellColumn(pageIndex, 1, 0, "A2");
            //this.CopyCellColumn(pageIndex, 3, 0, "A4");
            //this.CopyCellColumn(pageIndex, 4, 0, "A5");
            //this.CopyCellColumn(pageIndex, 5, 0, "A6");
            //this.CopyCellColumn(pageIndex, 1, 2, "C2");
            //this.CopyCellColumn(pageIndex, 2, 2, "C3");
            //this.CopyCellColumn(pageIndex, 3, 2, "C4");
            //this.CopyCellColumn(pageIndex, 4, 2, "C5");
        }

        /**
         * 填充报表
         * 
         * Param list [0]huDetailList
         */
        protected override bool FillValuesImpl(String templateFileName, IList<object> data)
        {
            try
            {
                PrintStockTakeMaster printStockTakeMaster = (PrintStockTakeMaster)data[0];
                if (printStockTakeMaster == null)
                {
                    return false;
                }

                this.sheet.DisplayGridlines = false;
                this.sheet.IsPrintGridlines = false;

                this.barCodeFontName = this.GetBarcodeFontName(0, 0);
                int pageIndex = 1;

                string barCode = Utility.BarcodeHelper.GetBarcodeStr(printStockTakeMaster.StNo, this.barCodeFontName);
                this.SetColumnCell(pageIndex, 0, 0, barCode);

                this.SetColumnCell(pageIndex, 1, 0, printStockTakeMaster.StNo);

                this.SetColumnCell(pageIndex, 2, 2, printStockTakeMaster.Region);

                this.SetColumnCell(pageIndex, 3, 2, printStockTakeMaster.Type == 0 ? "抽盘" : "全盘");

                this.SetColumnCell(pageIndex, 4, 2, printStockTakeMaster.EffectiveDate.HasValue ? printStockTakeMaster.EffectiveDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty);

                this.SetColumnCell(pageIndex, 5, 2, printStockTakeMaster.IsScanHu ? "√" : "×");

                this.SetColumnCell(pageIndex, 6, 2, printStockTakeMaster.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"));

                this.SetColumnCell(pageIndex, 7, 2, printStockTakeMaster.CreateUserName);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}
