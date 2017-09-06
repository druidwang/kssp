using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.INV;
using NPOI.SS.UserModel;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepBarCodeA4Operator : ReportBase
    {
        private static readonly int PAGE_DETAIL_ROW_COUNT = 9;

        private static readonly int ROW_COUNT = 30;
        //列数   1起始
        private static readonly int COLUMN_COUNT = 14;

        /**
         * 需要拷贝的数据与合并单元格操作
         * 
         * Param pageIndex 页号
         */
        public override void CopyPageValues(int pageIndex)
        {

            //三行 三个
            for (int rowNum = 0; rowNum < 21; rowNum += 10)
            {
                //第一个
                //hu.id
                this.SetMergedRegion(pageIndex, rowNum + 0, 1, rowNum + 0, 3);

                this.SetMergedRegion(pageIndex, rowNum + 1, 0, rowNum + 2, 0);

                this.SetMergedRegion(pageIndex, rowNum + 6, 1, rowNum + 6, 3);
                //PART NO.
                this.SetMergedRegion(pageIndex, rowNum + 7, 0, rowNum + 7, 3);
                //LOT/SERUAL	
                this.SetMergedRegion(pageIndex, rowNum + 8, 0, rowNum + 8, 3);
                //PART NO.
                this.CopyCell(pageIndex, rowNum + 0, 0, "A" + (rowNum + 1));

                this.CopyCell(pageIndex, rowNum + 1, 0, "A" + (rowNum + 2));

                this.CopyCell(pageIndex, rowNum + 1, 2, "C" + (rowNum + 2));

                this.CopyCell(pageIndex, rowNum + 2, 2, "C" + (rowNum + 3));

                this.CopyCell(pageIndex, rowNum + 3, 0, "A" + (rowNum + 4));

                this.CopyCell(pageIndex, rowNum + 3, 2, "C" + (rowNum + 4));

                this.CopyCell(pageIndex, rowNum + 4, 0, "A" + (rowNum + 5));

                this.CopyCell(pageIndex, rowNum + 4, 2, "C" + (rowNum + 5));

                this.CopyCell(pageIndex, rowNum + 5, 0, "A" + (rowNum + 6));

                this.CopyCell(pageIndex, rowNum + 5, 2, "C" + (rowNum + 6));

                this.CopyCell(pageIndex, rowNum + 6, 0, "A" + (rowNum + 7));


                //第二个
                //hu.id
                this.SetMergedRegion(pageIndex, rowNum + 0, 6, rowNum + 0, 8);

                this.SetMergedRegion(pageIndex, rowNum + 1, 5, rowNum + 2, 5);

                this.SetMergedRegion(pageIndex, rowNum + 6, 6, rowNum + 6, 8);
                //PART NO.
                this.SetMergedRegion(pageIndex, rowNum + 7, 5, rowNum + 7, 8);
                //LOT/SERUAL	
                this.SetMergedRegion(pageIndex, rowNum + 8, 5, rowNum + 8, 8);
                //PART NO.
                this.CopyCell(pageIndex, rowNum + 0, 5, "F" + (rowNum + 1));

                this.CopyCell(pageIndex, rowNum + 1, 5, "F" + (rowNum + 2));

                this.CopyCell(pageIndex, rowNum + 1, 7, "H" + (rowNum + 2));

                this.CopyCell(pageIndex, rowNum + 2, 7, "H" + (rowNum + 3));

                this.CopyCell(pageIndex, rowNum + 3, 5, "F" + (rowNum + 4));

                this.CopyCell(pageIndex, rowNum + 3, 7, "H" + (rowNum + 4));

                this.CopyCell(pageIndex, rowNum + 4, 5, "F" + (rowNum + 5));

                this.CopyCell(pageIndex, rowNum + 4, 7, "H" + (rowNum + 5));

                this.CopyCell(pageIndex, rowNum + 5, 5, "F" + (rowNum + 6));

                this.CopyCell(pageIndex, rowNum + 5, 7, "H" + (rowNum + 6));

                this.CopyCell(pageIndex, rowNum + 6, 5, "F" + (rowNum + 7));


                //第三个
                //hu.id
                this.SetMergedRegion(pageIndex, rowNum + 0, 11, rowNum + 0, 13);

                this.SetMergedRegion(pageIndex, rowNum + 1, 10, rowNum + 2, 10);

                this.SetMergedRegion(pageIndex, rowNum + 6, 11, rowNum + 6, 13);
                //PART NO.
                this.SetMergedRegion(pageIndex, rowNum + 7, 10, rowNum + 7, 13);
                //LOT/SERUAL	
                this.SetMergedRegion(pageIndex, rowNum + 8, 10, rowNum + 8, 13);
                //PART NO.
                this.CopyCell(pageIndex, rowNum + 0, 10, "K" + (rowNum + 1));

                this.CopyCell(pageIndex, rowNum + 1, 10, "K" + (rowNum + 2));

                this.CopyCell(pageIndex, rowNum + 1, 12, "M" + (rowNum + 2));

                this.CopyCell(pageIndex, rowNum + 2, 12, "M" + (rowNum + 3));

                this.CopyCell(pageIndex, rowNum + 3, 10, "K" + (rowNum + 4));

                this.CopyCell(pageIndex, rowNum + 3, 12, "M" + (rowNum + 4));

                this.CopyCell(pageIndex, rowNum + 4, 10, "K" + (rowNum + 5));

                this.CopyCell(pageIndex, rowNum + 4, 12, "M" + (rowNum + 5));

                this.CopyCell(pageIndex, rowNum + 5, 10, "K" + (rowNum + 6));

                this.CopyCell(pageIndex, rowNum + 5, 12, "M" + (rowNum + 6));

                this.CopyCell(pageIndex, rowNum + 6, 10, "K" + (rowNum + 7));

            }
        }

        /**
         * 填充报表
         * 
         * Param list [0]huDetailList
         */
        public override bool FillValues(string templateFileFolder, String templateFileName, IList<object> list)
        {
            try
            {
                this.init(templateFileFolder.Replace("\r\n\t\t", "") + templateFileName, ROW_COUNT);

                if (list == null || list.Count == 0) return false;

                IList<PrintHu> huList = (IList<PrintHu>)list[0];
                

                int count = huList.Count;
              

                if (count == 0) return false;

                this.barCodeFontName = this.GetBarcodeFontName(0, 0);

                int pageCount = (int)Math.Ceiling(count / (PAGE_DETAIL_ROW_COUNT * 1.0));

                this.CopyPage(pageCount, COLUMN_COUNT, 1);


                this.sheet.DisplayGridlines = false;
                this.sheet.IsPrintGridlines = false;

                ICellStyle cellStyleT = workbook.CreateCellStyle();
                IFont fontT = workbook.CreateFont();
                fontT.FontHeightInPoints = (short)9;
                fontT.FontName = "宋体";
                fontT.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;
                cellStyleT.SetFont(fontT);

                int pageIndex = 1;
                int num = 1;

                foreach (PrintHu hu in huList)
                {

                    //3行每行3个
                    this.writeContent(string.Empty, pageIndex, num, hu);

                    if (num == count || num % PAGE_DETAIL_ROW_COUNT == 0)
                    {
                        pageIndex++;
                    }
                    num++;

               
                }

            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        private void writeContent(string companyCode, int pageIndex, int num, PrintHu hu)
        {
            // 物料名称
            this.SetRowCell(pageIndex, 0, 1, hu.ItemDescription, num);

            // 物料号（新图号）
            this.SetRowCell(pageIndex, 1, 1, hu.Item, num);

            // 物料号（旧图号）
            this.SetRowCell(pageIndex, 2, 1, hu.ReferenceItemCode, num);

            // 交货地点
            this.SetRowCell(pageIndex, 1, 3, hu.LocationTo, num);

            //数量
            //this.SetRowCell(pageIndex, 2, 3, hu.Qty.ToString("0.########"));
         
            if (hu.IsOdd)
            {
                this.SetRowCell(pageIndex, 2, 3, hu.Qty.ToString("0.########") + "（零）",num);
            }
            else
            {
                this.SetRowCell(pageIndex, 2, 3, hu.Qty.ToString("0.########"),num);
            }

            //批   号
            this.SetRowCell(pageIndex, 3, 1, hu.LotNo, num);

            //包   装
            if (hu.IsChangeUnitCount)
            {
                this.SetRowCell(pageIndex, 3, 3, hu.UnitCount + "(非)", num);
            }
            else
            {
                this.SetRowCell(pageIndex, 3, 3, hu.UnitCount.ToString(), num);
            }
            //毛重
            this.SetRowCell(pageIndex, 4, 1, "", num);

            //外形尺寸
            this.SetRowCell(pageIndex, 4, 3, hu.ContainerDesc, num);

            //供应商代码
            this.SetRowCell(pageIndex, 5, 1, hu.ManufactureParty, num);

            //供应商批号
            this.SetRowCell(pageIndex, 5, 3, hu.SupplierLotNo, num);

            //供应商名称
            this.SetRowCell(pageIndex, 6, 1, hu.ManufacturePartyDescription, num);

            //hu id内容
            string barCode = Utility.BarcodeHelper.GetBarcodeStr(hu.HuId, this.barCodeFontName);
            this.SetRowCell(pageIndex, 7, 0, barCode, num);

            //hu id内容
            this.SetRowCell(pageIndex, 8, 0, hu.HuId, num);
            //}
            /*
            else if (hu.Item.Type.Equals("P")) //原材料
            {
                //画方框
                Cell cell1 = this.GetCell(this.GetRowIndexAbsolute(pageIndex, getRowIndex(2, num)), getColumnIndex(4, num));
                CellStyle cellStyle1 = workbook.CreateCellStyle();
                cellStyle1.BorderBottom = NPOI.SS.UserModel.CellBorderType.NONE;
                cellStyle1.BorderLeft = NPOI.SS.UserModel.CellBorderType.THIN;
                cellStyle1.BorderRight = NPOI.SS.UserModel.CellBorderType.THIN;
                cellStyle1.BorderTop = NPOI.SS.UserModel.CellBorderType.THIN;
                cell1.CellStyle = workbook.CreateCellStyle();
                cell1.CellStyle.CloneStyleFrom(cellStyle1);

                CellStyle cellStyle2 = workbook.CreateCellStyle();
                Cell cell2 = this.GetCell(this.GetRowIndexAbsolute(pageIndex, getRowIndex(3, num)), getColumnIndex(4, num));
                cellStyle2.BorderLeft = NPOI.SS.UserModel.CellBorderType.THIN;
                cellStyle2.BorderRight = NPOI.SS.UserModel.CellBorderType.THIN;
                cellStyle2.BorderBottom = NPOI.SS.UserModel.CellBorderType.THIN;
                cellStyle2.BorderTop = NPOI.SS.UserModel.CellBorderType.NONE;
                cell2.CellStyle = workbook.CreateCellStyle();
                cell2.CellStyle.CloneStyleFrom(cellStyle2);

                //hu id内容
                string barCode = Utility.BarcodeHelper.GetBarcodeStr(hu.HuId, this.barCodeFontName);
                this.SetRowCell(pageIndex, 0, 0, barCode, num);
                //hu id内容
                this.SetRowCell(pageIndex, 1, 0, hu.HuId, num);
                //PART NO.内容
                this.SetRowCell(pageIndex, 3, 0, hu.Item.Code, num);
                //批号LotNo
                this.SetRowCell(pageIndex, 5, 0, hu.LotNo, num);
                //QUANTITY.
                this.SetRowCell(pageIndex, 4, 2, "QUANTITY.", num);
                //QUANTITY.
                this.SetRowCell(pageIndex, 5, 2, hu.Qty.ToString("0.########"), num);
                //DESCRIPTION	
                this.SetRowCell(pageIndex, 6, 0, "DESCRIPTION.", num);
                //DESCRIPTION内容
                this.SetRowCell(pageIndex, 7, 0, hu.Item.Description, num);
                //SUPPLIER
                this.SetRowCell(pageIndex, 8, 0, "SUPPLIER.", num);
                //SUPPLIER内容
                this.SetRowCell(pageIndex, 9, 0, hu.ManufactureParty == null ? string.Empty : hu.ManufactureParty.Name, num);
                //PRINTED DATE:内容
                this.SetRowCell(pageIndex, 10, 1, DateTime.Now.ToString("MM/dd/yy"), num);
                //print name 内容
                this.SetRowCell(pageIndex, 10, 3, userName, num);

            }*/
        }


        protected void SetMergedRegion(int pageIndex, int row1, int column1, int row2, int colunm2, int num)
        {
            this.SetMergedRegion(pageIndex, getRowIndex(row1, num), getColumnIndex(column1, num), getRowIndex(row2, num), getColumnIndex(colunm2, num));
        }

        private int getRowIndex(int rowIndexRelative, int num)
        {
            //return ((int)(Math.Ceiling(num / 3.0)) - 1) * 12 + rowIndexRelative;

            return (num - 1) / 3 % 3 * 10 + rowIndexRelative;
        }

        private int getColumnIndex(int cellIndex, int num)
        {
            return cellIndex + ((num - 1) % 3) * 5;
        }

        public void SetRowCell(int pageIndex, int rowIndexRelative, int cellIndex, String value, int num)
        {

            this.SetRowCell(pageIndex, getRowIndex(rowIndexRelative, num), getColumnIndex(cellIndex, num), value);
        }
    }
}
