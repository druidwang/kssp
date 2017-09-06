using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.INV;
using NPOI.SS.UserModel;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepEKanBanCardOperator : ReportBase
    {
        private static readonly int PAGE_DETAIL_ROW_COUNT = 2;

        private static readonly int ROW_COUNT = 56;
        //列数   1起始
        private static readonly int COLUMN_COUNT = 9;

        /**
         * 需要拷贝的数据与合并单元格操作
         * 
         * Param pageIndex 页号
         */
        public override void CopyPageValues(int pageIndex)
        {

            //二行 二个
            for (int rowNum = 0; rowNum < 29; rowNum += 28)
            {
                //第一个
                this.SetMergedRegion(pageIndex, rowNum + 0, 5, rowNum + 1, 6);
                this.SetMergedRegion(pageIndex, rowNum + 1, 0, rowNum + 2, 3);
                this.SetMergedRegion(pageIndex, rowNum + 2, 5, rowNum + 11, 8);
                this.SetMergedRegion(pageIndex, rowNum + 4, 0, rowNum + 5, 1);

                this.SetMergedRegion(pageIndex, rowNum + 4, 2, rowNum +5, 3);
                this.SetMergedRegion(pageIndex, rowNum + 7, 0, rowNum + 8, 1);
                this.SetMergedRegion(pageIndex, rowNum + 7, 2, rowNum + 8, 3);
                this.SetMergedRegion(pageIndex, rowNum + 10, 0, rowNum + 12, 3);
                this.SetMergedRegion(pageIndex, rowNum + 12, 5, rowNum + 25, 8);
                this.SetMergedRegion(pageIndex, rowNum + 14, 0, rowNum + 16, 3);
                this.SetMergedRegion(pageIndex, rowNum + 18, 0, rowNum + 19, 1);
                this.SetMergedRegion(pageIndex, rowNum + 18, 2, rowNum + 19, 3);
                this.SetMergedRegion(pageIndex, rowNum + 21, 0, rowNum + 22, 3);
                //this.SetMergedRegion(pageIndex, rowNum + 22, 2, rowNum + 23, 3);
                this.SetMergedRegion(pageIndex, rowNum + 24, 0, rowNum + 25, 1);
                this.SetMergedRegion(pageIndex, rowNum + 24, 2, rowNum + 25, 3);
                this.SetMergedRegion(pageIndex, rowNum + 26, 0, rowNum + 26, 3);
                this.CopyCell(pageIndex, rowNum + 0, 0, "A" + (rowNum + 1));
                this.CopyCell(pageIndex, rowNum + 0, 5, "F" + (rowNum + 1));
                this.CopyCell(pageIndex, rowNum + 3, 0, "A" + (rowNum + 4));
                this.CopyCell(pageIndex, rowNum + 3, 2, "C" + (rowNum + 4));
                this.CopyCell(pageIndex, rowNum + 6, 0, "A" + (rowNum + 7));
                this.CopyCell(pageIndex, rowNum + 6, 2, "C" + (rowNum + 7));
                this.CopyCell(pageIndex, rowNum + 9, 0, "A" + (rowNum + 10));
                this.CopyCell(pageIndex, rowNum + 13, 0, "A" + (rowNum + 14));

                this.CopyCell(pageIndex, rowNum + 17, 0, "A" + (rowNum + 18));
                this.CopyCell(pageIndex, rowNum + 17, 2, "C" + (rowNum + 18));
                this.CopyCell(pageIndex, rowNum + 20, 0, "A" + (rowNum + 21));
                //this.CopyCell(pageIndex, rowNum + 21, 2, "C" + (rowNum + 22));
                this.CopyCell(pageIndex, rowNum + 23, 0, "A" + (rowNum + 24));
                this.CopyCell(pageIndex, rowNum + 23, 2, "C" + (rowNum + 24));


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

               // PrintKanBanCard kanBanCard = (PrintKanBanCard)list[0];
                List<PrintKanBanCard> kanBanCardList = (List<PrintKanBanCard>)list[0];

                int count = 0;

                foreach (PrintKanBanCard kanBanCard in kanBanCardList)
                {
                    count += kanBanCard.KanBanDetails.Count;
                }
                if (count == 0) return false;

                this.barCodeFontName = this.GetBarcodeFontName(15, 0);

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

                foreach (PrintKanBanCard card in kanBanCardList)
                {
                    foreach (PrintKanBanCardInfo cardInfo in card.KanBanDetails)
                    {
                        //3行每行3个
                        this.writeContent(string.Empty, pageIndex, num, cardInfo, card, count);

                        if (num == count || num % PAGE_DETAIL_ROW_COUNT == 0)
                        {
                            pageIndex++;
                        }
                        num++;
                    }
                }

            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        private void writeContent(string companyCode, int pageIndex, int num, PrintKanBanCardInfo cardInfo, PrintKanBanCard kanBanCard, int count)
        {

            this.SetRowCell(pageIndex, 1, 0, kanBanCard.Flow, num);
            this.SetRowCell(pageIndex, 2, 5, kanBanCard.ThumbNo == null ? "" : kanBanCard.ThumbNo.ToString(), num);
            this.SetRowCell(pageIndex, 4, 0, cardInfo.CardNo, num);
            this.SetRowCell(pageIndex, 4, 2, kanBanCard.LocationTo, num);
            this.SetRowCell(pageIndex, 7, 0, kanBanCard.Item, num);
            this.SetRowCell(pageIndex, 10, 0, kanBanCard.ItemDescription, num);
           
            string barCode = Utility.BarcodeHelper.GetBarcodeStr("$K" + cardInfo.CardNo, this.barCodeFontName);
            this.SetRowCell(pageIndex, 14, 0, barCode, num);

            this.SetRowCell(pageIndex, 18, 0, kanBanCard.PackType, num);
            this.SetRowCell(pageIndex, 18, 2, kanBanCard.UnitCount.ToString("0.########"), num);
            this.SetRowCell(pageIndex, 21, 0, kanBanCard.MultiStation, num);

            this.SetRowCell(pageIndex, 24, 0, kanBanCard.ItemCategory, num);
            this.SetRowCell(pageIndex, 24, 2, DateTime.Now.ToString("yyyy-MM-dd"), num);
            this.SetRowCell(pageIndex, 7, 2, kanBanCard.ReferenceItemCode, num);
            this.SetRowCell(pageIndex, 26, 0, num + "-" + count, num);

        }


        protected void SetMergedRegion(int pageIndex, int row1, int column1, int row2, int colunm2, int num)
        {
            this.SetMergedRegion(pageIndex, getRowIndex(row1, num), getColumnIndex(column1, num), getRowIndex(row2, num), getColumnIndex(colunm2, num));
        }

        private int getRowIndex(int rowIndexRelative, int num)
        {
            //return ((int)(Math.Ceiling(num / 3.0)) - 1) * 12 + rowIndexRelative;

            return (num - 1) / 1 % 2 * 28 + rowIndexRelative;
        }

        private int getColumnIndex(int cellIndex, int num)
        {
            return cellIndex + ((num - 1) % 1) * 9;
        }

        public void SetRowCell(int pageIndex, int rowIndexRelative, int cellIndex, String value, int num)
        {

            this.SetRowCell(pageIndex, getRowIndex(rowIndexRelative, num), getColumnIndex(cellIndex, num), value);
        }
    }
}