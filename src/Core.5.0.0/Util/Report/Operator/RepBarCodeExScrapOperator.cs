using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.INV;
using NPOI.HSSF.UserModel;
//using NPOI.Util.IO;
using ThoughtWorks.QRCode.Codec;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepBarCodeExScrapOperator : RepTemplate2
    {
        public RepBarCodeExScrapOperator()
        {

            //明细部分的行数
            this.pageDetailRowCount = 7;
            //列数   1起始
            this.columnCount = 4;

            this.rowCount = 7;
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
            this.SetMergedRegionColumn(pageIndex, 2, 1, 1, 3);

            this.SetMergedRegionColumn(pageIndex, 4, 1, 4, 3);
            this.SetMergedRegionColumn(pageIndex, 5, 0, 5, 3);

            this.SetColumnCell(pageIndex, 0, 0, "废品类型");
            this.SetColumnCell(pageIndex, 0, 2, "重  量");
            this.SetColumnCell(pageIndex, 1, 0, "生产线");
            this.SetColumnCell(pageIndex, 1, 2, "物料");
            this.SetColumnCell(pageIndex, 2, 0, "名  称");
            this.SetColumnCell(pageIndex, 3, 0, "生产日期");
            this.SetColumnCell(pageIndex, 3, 2, "班  次");
            this.SetColumnCell(pageIndex, 4, 0, "生产单");
            this.SetColumnCell(pageIndex, 6, 0, "打印人");
            this.SetColumnCell(pageIndex, 6, 2, "打印时间");
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

                this.barCodeFontName = this.GetBarcodeFontName(5, 0);

                //加页删页
                //纵向打印
                this.CopyPageCloumn(count, columnCount, 1);

                int pageIndex = 1;

                foreach (PrintHu hu in huList)
                {

                    this.SetColumnCell(pageIndex, 1, 1, hu.Flow);
                    this.SetColumnCell(pageIndex, 1, 3, hu.Item);
                    // 物料名称
                    this.SetColumnCell(pageIndex, 2, 1, hu.ItemDescription);

                    //生产日期
                    this.SetColumnCell(pageIndex, 3, 1, hu.ManufactureDate.ToString("yyyy/MM/dd"));
                    //班  次
                    this.SetColumnCell(pageIndex, 3, 3, hu.Shift);
                    //生产单
                    this.SetColumnCell(pageIndex, 4, 1, string.Format("{0} {1}", hu.OrderNo, hu.Remark));

                    // 条形码
                    string barCode = Utility.BarcodeHelper.GetBarcodeStr(hu.OrderNo, this.barCodeFontName);
                    this.SetColumnCell(pageIndex, 5, 0, barCode);
                    //打印人
                    this.SetColumnCell(pageIndex, 6, 1, userName);
                    // 打印时间
                    this.SetColumnCell(pageIndex, 6, 3, DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
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
