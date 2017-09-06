using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.INV;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepBarCodeOperator : RepTemplate2
    {
        public RepBarCodeOperator()
        {

            //明细部分的行数
            this.pageDetailRowCount = 9;
            //列数   1起始
            this.columnCount = 4;

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
            this.SetMergedRegionColumn(pageIndex, 0, 1, 0, 3);
            this.SetMergedRegionColumn(pageIndex, 1, 0, 2, 0);
            this.SetMergedRegionColumn(pageIndex, 6, 1, 6, 3);
            this.SetMergedRegionColumn(pageIndex, 7, 0, 7, 3);
            this.SetMergedRegionColumn(pageIndex, 8, 0, 8, 3);

            this.CopyCellColumn(pageIndex, 0, 0, "A1");
            this.CopyCellColumn(pageIndex, 1, 0, "A2");
            this.CopyCellColumn(pageIndex, 3, 0, "A4");
            this.CopyCellColumn(pageIndex, 4, 0, "A5");
            this.CopyCellColumn(pageIndex, 5, 0, "A6");
            this.CopyCellColumn(pageIndex, 6, 0, "A7");
            this.CopyCellColumn(pageIndex, 1, 2, "C2");
            this.CopyCellColumn(pageIndex, 2, 2, "C3");
            this.CopyCellColumn(pageIndex, 3, 2, "C4");
            this.CopyCellColumn(pageIndex, 4, 2, "C5");
            this.CopyCellColumn(pageIndex, 5, 2, "C6");

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

                this.barCodeFontName = this.GetBarcodeFontName(7, 1);

                //加页删页
                //纵向打印
                this.CopyPageCloumn(count, columnCount, 1);

                int pageIndex = 1;

                foreach (PrintHu hu in huList)
                {

                    // 物料名称
                    this.SetColumnCell(pageIndex, 0, 1, hu.ItemDescription);

                    // 物料号（新图号）
                    this.SetColumnCell(pageIndex, 1, 1, hu.Item);

                    // 物料号（旧图号）
                    this.SetColumnCell(pageIndex, 2, 1, hu.ReferenceItemCode);

                    // 交货地点
                    this.SetColumnCell(pageIndex, 1, 3, hu.LocationTo);

                    //数量
                    //this.SetRowCell(pageIndex, 2, 3, hu.Qty.ToString("0.########"));
                    //if (hu.IsChangeUnitCount)
                    //{
                    if (hu.IsOdd)
                    {
                        this.SetColumnCell(pageIndex, 2, 3, hu.Qty.ToString("0.########") + "（零）");
                    }
                    else
                    {
                        this.SetColumnCell(pageIndex, 2, 3, hu.Qty.ToString("0.########"));
                    }

                    //批   号
                
                        this.SetColumnCell(pageIndex, 3, 1, hu.LotNo);
                        //包   装 描述（铁包装、木包装）
                        this.SetColumnCell(pageIndex, 3, 3, hu.ContainerDesc);
                        //if (hu.IsChangeUnitCount)
                        //{
                        //    this.SetColumnCell(pageIndex, 3, 3, hu.UnitCount + "(非)");
                        //}
                        //else
                        //{
                        //    this.SetColumnCell(pageIndex, 3, 3, hu.UnitCount.ToString());
                        //}

                    //毛重
                    this.SetColumnCell(pageIndex, 4, 1, "");

                    //外形尺寸
                    this.SetColumnCell(pageIndex, 4, 3, hu.ContainerDesc);

                    //供应商代码
                    this.SetColumnCell(pageIndex, 5, 1, hu.ManufactureParty);

                    //供应商批号
                    this.SetColumnCell(pageIndex, 5, 3, hu.SupplierLotNo);

                    //供应商名称
                    this.SetColumnCell(pageIndex, 6, 1, hu.ManufacturePartyDescription);

                    //hu id内容
                    string barCode = Utility.BarcodeHelper.GetBarcodeStr(hu.HuId, this.barCodeFontName);
                    this.SetColumnCell(pageIndex, 7, 0, barCode);

                    //hu id内容
                    this.SetColumnCell(pageIndex, 8, 0, hu.HuId);

                    //this.sheet.SetRowBreak(this.GetRowIndexAbsolute(pageIndex, ROW_COUNT - 1));
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
