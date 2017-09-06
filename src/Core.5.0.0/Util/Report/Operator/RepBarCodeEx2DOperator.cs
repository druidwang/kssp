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
    public class RepBarCodeEx2DOperator : RepTemplate2
    {
        public RepBarCodeEx2DOperator()
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
            this.SetMergedRegionColumn(pageIndex, 2, 3, 2, 4);

            this.SetMergedRegionColumn(pageIndex, 4, 3, 4, 4);
            this.SetMergedRegionColumn(pageIndex, 5, 3, 5, 4);
            this.SetMergedRegionColumn(pageIndex, 4, 0, 5, 2);
            this.SetMergedRegionColumn(pageIndex, 6, 1, 6, 4);
            this.SetMergedRegionColumn(pageIndex, 7, 1, 7, 4);
            this.SetMergedRegionColumn(pageIndex, 8, 3, 8, 4);

            //this.SetColumnCell(pageIndex, 0, 2, "物料");
            //this.SetColumnCell(pageIndex, 1, 2, "数量");
            //this.SetColumnCell(pageIndex, 2, 2, "备注");
            //this.SetColumnCell(pageIndex, 3, 2, "计划");
            //this.SetColumnCell(pageIndex, 6, 0, "制造\r\n日期");
            //this.SetColumnCell(pageIndex, 7, 0, "物料描述");
            //this.SetColumnCell(pageIndex, 8, 0, "打印人");
            //this.SetColumnCell(pageIndex, 8, 2, "打印时间");

            this.CopyCellColumn(pageIndex, 0, 2, "C1");
            this.CopyCellColumn(pageIndex, 1, 2, "C2");
            this.CopyCellColumn(pageIndex, 2, 2, "C3");
            this.CopyCellColumn(pageIndex, 3, 2, "C4");
            this.CopyCellColumn(pageIndex, 6, 0, "A7");
            this.CopyCellColumn(pageIndex, 7, 0, "A8");
            this.CopyCellColumn(pageIndex, 8, 0, "A9");
            this.CopyCellColumn(pageIndex, 8, 2, "C9");


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

                if (count == 0)
                {
                    return false;
                }

                //this.barCodeFontName = this.GetBarcodeFontName(6, 0);

                //加页删页
                //纵向打印
                this.CopyPageCloumn(count, columnCount, 1);

                int pageIndex = 1;

                foreach (PrintHu hu in huList)
                {
                    this.Fill2DBarCodeImage(pageIndex, 0, 0, 1, 3, hu.HuId);
                    // 物料号
                    this.SetColumnCell(pageIndex, 0, 3, hu.Item);
                    //数  量
                    this.SetColumnCell(pageIndex, 1, 3, string.Format("{0} {1}", hu.Qty.ToString("0.###"), hu.Uom));
                    //备注
                    this.SetColumnCell(pageIndex, 2, 3, hu.Remark);
                    //班  次
                    this.SetColumnCell(pageIndex, 3, 3, GetShiftName(hu.Shift));
                    //老化
                    if (hu.HuOption == 0)
                    {
                        // 老  化
                        this.SetColumnCell(pageIndex, 3, 4, "无需老化"); ;
                    }
                    else if (hu.HuOption == 1)
                    {
                        this.SetColumnCell(pageIndex, 3, 4, "未老化");
                    }
                    else if (hu.HuOption == 2)
                    {
                        this.SetColumnCell(pageIndex, 3, 4, "已老化");
                    }
                    else
                    {
                        this.SetColumnCell(pageIndex, 3, 4, string.Empty);
                    }

                    if (hu.HuOption == 2)
                    {
                        //老化开始
                        this.SetColumnCell(pageIndex, 4, 3,string.Format("{0} {1}","老化开始", hu.AgingStartTime.ToString("yyyy-MM-dd HH:mm")));
                        //老化结束
                        this.SetColumnCell(pageIndex, 5, 3, string.Format("{0} {1}", "老化结束", hu.AgingEndTime.ToString("yyyy-MM-dd HH:mm")));
                    }
                    else
                    {
                        //老化开始
                        this.SetColumnCell(pageIndex, 4, 3, string.Empty);
                        //老化结束
                        this.SetColumnCell(pageIndex, 5, 3, string.Empty);
                    }

                    //条码
                    this.SetColumnCell(pageIndex, 4, 0, hu.HuId);
                    //生产日期
                    this.SetColumnCell(pageIndex, 6, 1, hu.LotNo);
                    // 物料名称
                    this.SetColumnCell(pageIndex, 7, 1, hu.ItemDescription);
                    //打印人
                    this.SetColumnCell(pageIndex, 8, 1, userName);
                    // 打印时间
                    this.SetColumnCell(pageIndex, 8, 3, DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    //this.SetColumnCell(pageIndex, 1, 3, hu.PrintCount.ToString("0.########"));
                    //参考号
                    //this.SetColumnCell(pageIndex, 2, 1, hu.ReferenceItemCode);
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
