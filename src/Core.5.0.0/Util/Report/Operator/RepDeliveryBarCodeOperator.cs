using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.INV;
using com.Sconit.Entity.WMS;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepDeliveryBarCodeOperator : RepTemplate2
    {
        public RepDeliveryBarCodeOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 8;
            //列数   1起始
            this.columnCount = 5;

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
            this.SetMergedRegionColumn(pageIndex, 0, 1, 0, 3);
            this.SetMergedRegionColumn(pageIndex, 1, 0, 2, 0);
            this.SetMergedRegionColumn(pageIndex, 5, 1, 5, 3);
            this.SetMergedRegionColumn(pageIndex, 6, 0, 6, 3);
            this.SetMergedRegionColumn(pageIndex, 7, 0, 7, 3);

            this.CopyCellColumn(pageIndex, 0, 0, "A1");
            this.CopyCellColumn(pageIndex, 1, 0, "A2");
            this.CopyCellColumn(pageIndex, 3, 0, "A4");
            this.CopyCellColumn(pageIndex, 4, 0, "A5");
            this.CopyCellColumn(pageIndex, 5, 0, "A6");
            this.CopyCellColumn(pageIndex, 1, 2, "C2");
            this.CopyCellColumn(pageIndex, 2, 2, "C3");
            this.CopyCellColumn(pageIndex, 3, 2, "C4");
            this.CopyCellColumn(pageIndex, 4, 2, "C5");
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

                if (list == null || list.Count < 2) return false;

                IList<DeliveryBarCode> deliveryBarCodeList = (IList<DeliveryBarCode>)(list[0]);
                string userName = (string)list[1];

                this.sheet.DisplayGridlines = false;
                this.sheet.IsPrintGridlines = false;

                //this.sheet.DisplayGuts = false;

                int count = deliveryBarCodeList.Count();
                if (count == 0) return false;

                this.barCodeFontName = this.GetBarcodeFontName(6, 0);

                //加页删页
                //纵向打印
                this.CopyPageCloumn(count, columnCount, 1);

                int pageIndex = 1;

                foreach (DeliveryBarCode deliveryBarCode in deliveryBarCodeList)
                {

                    // 物料名称
                    this.SetColumnCell(pageIndex, 0, 1, deliveryBarCode.ItemDescription);

                    // 物料号（新图号）
                    this.SetColumnCell(pageIndex, 1, 1, deliveryBarCode.Item);

                    // 物料号（旧图号）
                    this.SetColumnCell(pageIndex, 2, 1, deliveryBarCode.ReferenceItemCode);

                    // 交货地点
                    this.SetColumnCell(pageIndex, 1, 3, deliveryBarCode.ShipTo);

                    //数量
                    this.SetColumnCell(pageIndex, 2, 3, deliveryBarCode.Qty.ToString("0.########"));
                 
                    //厂商代码
                    this.SetColumnCell(pageIndex, 3, 1, deliveryBarCode.PartyFrom);

                    //包装
                    this.SetColumnCell(pageIndex, 3, 3, deliveryBarCode.UnitCount.ToString("0.########"));
             
                    //道口
                    this.SetColumnCell(pageIndex, 4, 1, deliveryBarCode.Dock);

                    //配送工位
                    this.SetColumnCell(pageIndex, 4, 3, deliveryBarCode.Station);

                    //供应商名称
                    //this.SetColumnCell(pageIndex, 5, 1, hu.ManufacturePartyDescription);

                    //旧条码
                    this.SetColumnCell(pageIndex, 5, 1, deliveryBarCode.HuId);

                    //配送标签内容
                    string barCode = Utility.BarcodeHelper.GetBarcodeStr(deliveryBarCode.BarCode, this.barCodeFontName);
                    this.SetColumnCell(pageIndex, 6, 0, barCode);

                    //配送标签内容
                    this.SetColumnCell(pageIndex, 7, 0, deliveryBarCode.BarCode);

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
