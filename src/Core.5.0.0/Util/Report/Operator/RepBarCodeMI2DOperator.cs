using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.INV;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepBarCodeMI2DOperator : RepTemplate2
    {
        public RepBarCodeMI2DOperator()
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
            this.SetMergedRegionColumn(pageIndex, 2, 3, 2, 4);

            this.SetMergedRegionColumn(pageIndex, 3, 3, 3, 4);
            this.SetMergedRegionColumn(pageIndex, 4, 3, 4, 4);
            this.SetMergedRegionColumn(pageIndex, 5, 0, 5, 2);
            this.SetMergedRegionColumn(pageIndex, 5, 3, 5, 4);
            this.SetMergedRegionColumn(pageIndex, 6, 1, 6, 4);
            this.SetMergedRegionColumn(pageIndex, 7, 1, 7, 3);
            this.SetMergedRegionColumn(pageIndex, 7, 4, 8, 4);

            this.SetColumnCell(pageIndex, 0, 2, "物料");
            this.SetColumnCell(pageIndex, 1, 2, "数量");
            this.SetColumnCell(pageIndex, 2, 2, "备注");
            this.SetColumnCell(pageIndex, 3, 2, "状态");
            this.SetColumnCell(pageIndex, 4, 2, "FIFO期限");
            //this.SetColumnCell(pageIndex, 5, 3, "去向");
            this.SetColumnCell(pageIndex, 6, 0, "制造\r\n日期");
            this.SetColumnCell(pageIndex, 7, 0, "物料\r\n描述");
            this.SetColumnCell(pageIndex, 8, 0, "打印人");
            this.SetColumnCell(pageIndex, 8, 2, "打印时间");
            this.SetColumnCell(pageIndex, 7, 4, "贴合格证");
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
                    //物料代码
                    this.SetColumnCell(pageIndex, 0, 3, hu.Item);
                    //数  量
                    this.SetColumnCell(pageIndex, 1, 3, string.Format("{0} {1}", hu.Qty.ToString("0.###"), hu.Uom));
                    //班  次
                    this.SetColumnCell(pageIndex, 1, 4, GetShiftName(hu.Shift));
                    //备注
                    this.SetColumnCell(pageIndex, 2, 3, string.Format("{0} {1}", hu.Remark, hu.RefHu));

                    //过滤情况 取Createdate 8位
                    if (hu.HuOption == 0)
                    {
                        this.SetColumnCell(pageIndex, 3, 3, "无需过滤");
                    }
                    else if (hu.HuOption == 3)
                    {
                        this.SetColumnCell(pageIndex, 3, 3, "未过滤");
                    }
                    else if (hu.HuOption == 4)
                    {
                        this.SetColumnCell(pageIndex, 3, 3, "已过滤");
                    }
                    else
                    {
                        this.SetColumnCell(pageIndex, 3, 3, string.Empty);
                    }
                    //FIFO期限
                    if (hu.ExpireDate.HasValue)
                    {
                        this.SetColumnCell(pageIndex, 4, 3, hu.ExpireDate.Value.ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        this.SetColumnCell(pageIndex, 4, 2, string.Empty);
                        this.SetColumnCell(pageIndex, 4, 3, string.Empty);
                    }

                    // 条码号
                    this.SetColumnCell(pageIndex, 5, 0, hu.HuId);
                    //去向
                    if (string.IsNullOrWhiteSpace(hu.Direction))
                    {
                        this.SetColumnCell(pageIndex, 5, 3, "--");
                    }
                    else
                    {
                        this.SetColumnCell(pageIndex, 5, 3, hu.Direction);
                    }
                    //制造时间,生产日期
                    this.SetColumnCell(pageIndex, 6, 1, hu.LotNo);
                    // 物料名称
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
