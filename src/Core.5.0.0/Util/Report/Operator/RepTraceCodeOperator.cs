using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.INV;
using com.Sconit.PrintModel.ORD;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepTraceCodeOperator : RepTemplate2
    {
        public RepTraceCodeOperator()
        {

            //明细部分的行数
            this.pageDetailRowCount = 5;
            //列数   1起始
            this.columnCount = 1;

            this.rowCount = 2;
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
            //this.SetMergedRegionColumn(pageIndex, 0, 0, 0, 3);
            //this.SetMergedRegionColumn(pageIndex, 1, 0, 1, 3);

            //this.CopyCellColumn(pageIndex, 2, 0, "A3");
            //this.CopyCellColumn(pageIndex, 3, 0, "A4");
            //this.CopyCellColumn(pageIndex, 4, 0, "A5");
            //this.CopyCellColumn(pageIndex, 2, 2, "C3");
            //this.CopyCellColumn(pageIndex, 3, 2, "C4");
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
                if (list == null || list.Count < 1) return false;

                string traceCode = (string)(list[0]);


                if (string.IsNullOrEmpty(traceCode))
                {
                    return false;
                }


                this.sheet.DisplayGridlines = false;
                this.sheet.IsPrintGridlines = false;

                //this.sheet.DisplayGuts = false;

                int count = 1;

                if (count == 0) return false;

                this.barCodeFontName = this.GetBarcodeFontName(0, 0);

                //加页删页
                //纵向打印
                this.CopyPageCloumn(count, columnCount, 1);

                int pageIndex = 1;

                string barCode = Utility.BarcodeHelper.GetBarcodeStr(traceCode, this.barCodeFontName);
                this.SetColumnCell(pageIndex, 0, 0, barCode);

                this.SetColumnCell(pageIndex, 1, 0, traceCode);

            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}
