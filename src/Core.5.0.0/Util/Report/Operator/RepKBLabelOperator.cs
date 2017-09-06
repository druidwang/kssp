using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.INV;
using NPOI.SS.UserModel;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepKBLabelOperator : ReportBase
    {
        private static readonly int PAGE_DETAIL_ROW_COUNT = 6;

        private static readonly int ROW_COUNT = 36;
        //列数   1起始
        private static readonly int COLUMN_COUNT = 4;

        /**
         * 需要拷贝的数据与合并单元格操作
         * 
         * Param pageIndex 页号
         */
        public override void CopyPageValues(int pageIndex)
        {
            //五行 一个
            for (int rowNum = 0; rowNum < 31; rowNum += 6)
            {
                //第一个
                this.SetMergedRegion(pageIndex, rowNum + 0, 0, rowNum + 4, 0);

                this.CopyCell(pageIndex, rowNum + 0, 1, "B" + (rowNum + 1));

                this.CopyCell(pageIndex, rowNum + 0, 3, "D" + (rowNum + 1));

                this.CopyCell(pageIndex, rowNum + 1, 1, "B" + (rowNum + 2));
                this.CopyCell(pageIndex, rowNum + 2, 1, "B" + (rowNum + 3));
                this.CopyCell(pageIndex, rowNum + 3, 1, "B" + (rowNum + 4));
                this.CopyCell(pageIndex, rowNum + 4, 1, "B" + (rowNum + 5));

                this.CopyCell(pageIndex, rowNum + 2, 3, "D" + (rowNum + 3));

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

                List<PrintKanBanCard> printKanBanCardList = (List<PrintKanBanCard>)list[0];
                printKanBanCardList.AddRange(printKanBanCardList);
                printKanBanCardList = printKanBanCardList.OrderBy(o => o.Code).ToList();

                int count = printKanBanCardList.Count;
              

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

                foreach (PrintKanBanCard card in printKanBanCardList)
                {
                    this.writeContent(string.Empty, pageIndex, num, card, count);

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

        private void writeContent(string companyCode, int pageIndex, int num, PrintKanBanCard kanBanCard, int count)
        {
            // HY号:（旧图号）
            this.SetRowCell(pageIndex, 0, 2, kanBanCard.ReferenceItemCode, num);

            // PRP号（新图号）
            this.SetRowCell(pageIndex, 1, 2, kanBanCard.Item, num);

            this.SetRowCell(pageIndex, 1, 3, kanBanCard.Flow, num);
            this.SetRowCell(pageIndex, 2, 2, kanBanCard.ItemDescription, num);
            this.SetRowCell(pageIndex, 3, 2, kanBanCard.LocationTo, num);

            this.SetRowCell(pageIndex, 4, 2, kanBanCard.Code, num);

            this.SetRowCell(pageIndex, 3, 3, kanBanCard.PackType, num);
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

            return (num - 1) / 1 % 6 * 6 + rowIndexRelative;
        }

        private int getColumnIndex(int cellIndex, int num)
        {
            return cellIndex + ((num - 1) % 1) * 4;
        }

        public void SetRowCell(int pageIndex, int rowIndexRelative, int cellIndex, String value, int num)
        {

            this.SetRowCell(pageIndex, getRowIndex(rowIndexRelative, num), getColumnIndex(cellIndex, num), value);
        }
    }
}
