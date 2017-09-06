using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.INV;
using NPOI.SS.UserModel;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepKanBanCardOperator : ReportBase
    {
        //显示几个
        private static readonly int PAGE_DETAIL_ROW_COUNT = 4;
        //所有行减一
        private static readonly int ROW_COUNT = 62;
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
            for (int rowNum = 0; rowNum < 32; rowNum += 31)
            {
                //第一个合并单元格
                this.SetMergedRegion(pageIndex, rowNum + 1, 0, rowNum + 2, 1);
                this.SetMergedRegion(pageIndex, rowNum + 1, 2, rowNum + 2, 3);
                this.SetMergedRegion(pageIndex, rowNum + 4, 0, rowNum + 5, 1);
                this.SetMergedRegion(pageIndex, rowNum + 4, 2, rowNum + 5, 3);
                this.SetMergedRegion(pageIndex, rowNum + 6, 2, rowNum + 11, 3);
                this.SetMergedRegion(pageIndex, rowNum + 7, 0, rowNum + 8, 1);
                this.SetMergedRegion(pageIndex, rowNum + 10, 0, rowNum +11, 1);

                this.SetMergedRegion(pageIndex, rowNum + 13, 0, rowNum + 15, 3);
                this.SetMergedRegion(pageIndex, rowNum + 17, 0, rowNum + 19, 3);
                this.SetMergedRegion(pageIndex, rowNum + 21, 0, rowNum + 22, 1);
                this.SetMergedRegion(pageIndex, rowNum + 21, 2, rowNum + 22, 3);
                this.SetMergedRegion(pageIndex, rowNum + 24, 0, rowNum + 25, 3);
                //this.SetMergedRegion(pageIndex, rowNum + 21, 2, rowNum + 22, 3);
                this.SetMergedRegion(pageIndex, rowNum + 27, 0, rowNum + 28, 1);
                this.SetMergedRegion(pageIndex, rowNum + 27, 2, rowNum + 28, 3);
                this.SetMergedRegion(pageIndex, rowNum + 29, 0, rowNum + 29, 3);
                //copy中文
                this.CopyCell(pageIndex, rowNum, 0, "A" + (rowNum + 1));
                this.CopyCell(pageIndex, rowNum, 2, "C" + (rowNum + 1));
                this.CopyCell(pageIndex, rowNum + 3, 0, "A" + (rowNum + 4));
                this.CopyCell(pageIndex, rowNum + 3, 2, "C" + (rowNum + 4));
                this.CopyCell(pageIndex, rowNum + 6, 0, "A" + (rowNum + 7));
                //this.CopyCell(pageIndex, rowNum + 6, 2, "C" + (rowNum + 7));
                this.CopyCell(pageIndex, rowNum + 9, 0, "A" + (rowNum + 10));
                this.CopyCell(pageIndex, rowNum + 12, 0, "A" + (rowNum + 13));

                this.CopyCell(pageIndex, rowNum + 16, 0, "A" + (rowNum + 17));
                //this.CopyCell(pageIndex, rowNum + 17, 2, "C" + (rowNum + 18));
                this.CopyCell(pageIndex, rowNum + 20, 0, "A" + (rowNum + 21));
                this.CopyCell(pageIndex, rowNum + 20, 2, "C" + (rowNum + 21));
                this.CopyCell(pageIndex, rowNum + 23, 0, "A" + (rowNum + 24));
                this.CopyCell(pageIndex, rowNum + 26, 0, "A" + (rowNum + 27));
                this.CopyCell(pageIndex, rowNum + 26, 2, "C" + (rowNum + 27));
                //第二个
                this.SetMergedRegion(pageIndex, rowNum + 1, 5, rowNum + 2, 6);
                this.SetMergedRegion(pageIndex, rowNum + 1, 7, rowNum + 2, 8);

                //this.SetMergedRegion(pageIndex, rowNum + 1, 5, rowNum + 2, 8);
                this.SetMergedRegion(pageIndex, rowNum + 4, 5, rowNum + 5, 6);
                this.SetMergedRegion(pageIndex, rowNum + 4, 7, rowNum + 5, 8);
                this.SetMergedRegion(pageIndex, rowNum + 6, 7, rowNum + 11, 8);
                this.SetMergedRegion(pageIndex, rowNum + 7, 5, rowNum + 8, 6);
                this.SetMergedRegion(pageIndex, rowNum + 10, 5, rowNum + 11, 6);

                this.SetMergedRegion(pageIndex, rowNum + 13, 5, rowNum + 15, 8);
                this.SetMergedRegion(pageIndex, rowNum + 17, 5, rowNum + 19, 8);
                this.SetMergedRegion(pageIndex, rowNum + 21, 5, rowNum + 22, 6);
                this.SetMergedRegion(pageIndex, rowNum + 21, 7, rowNum + 22, 8);
                this.SetMergedRegion(pageIndex, rowNum + 24, 5, rowNum + 25, 8);
                //this.SetMergedRegion(pageIndex, rowNum + 21, 2, rowNum + 22, 3);
                this.SetMergedRegion(pageIndex, rowNum + 27, 5, rowNum + 28, 6);
                this.SetMergedRegion(pageIndex, rowNum + 27, 7, rowNum + 28, 8);
                this.SetMergedRegion(pageIndex, rowNum + 29, 5, rowNum + 29, 8);
                this.CopyCell(pageIndex, rowNum + 0, 5, "F" + (rowNum + 1));
                this.CopyCell(pageIndex, rowNum + 3, 5, "F" + (rowNum + 4));
                this.CopyCell(pageIndex, rowNum + 3, 7, "H" + (rowNum + 4));
                this.CopyCell(pageIndex, rowNum + 6, 5, "F" + (rowNum + 7));
                //this.CopyCell(pageIndex, rowNum + 6, 2, "C" + (rowNum + 7));
                this.CopyCell(pageIndex, rowNum + 9, 5, "F" + (rowNum + 10));
                this.CopyCell(pageIndex, rowNum + 12, 5, "F" + (rowNum + 13));

                this.CopyCell(pageIndex, rowNum + 16, 5, "F" + (rowNum + 17));
                //this.CopyCell(pageIndex, rowNum + 17, 2, "C" + (rowNum + 18));
                this.CopyCell(pageIndex, rowNum + 20, 5, "F" + (rowNum + 21));
                this.CopyCell(pageIndex, rowNum + 20, 7, "H" + (rowNum + 21));
                this.CopyCell(pageIndex, rowNum + 23, 5, "F" + (rowNum + 24));
                this.CopyCell(pageIndex, rowNum + 26, 5, "F" + (rowNum + 27));
                this.CopyCell(pageIndex, rowNum + 26, 7, "H" + (rowNum + 27));


            }
        }

        /**v
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

                IList<PrintKanBanCard> kanBanCardList = (List<PrintKanBanCard>)list[0];
               // List<PrintKanBanCardInfo> cardInfolist = (List<PrintKanBanCardInfo>)list[1];

               // int count = cardInfolist.Count;
                int count = 0;

                foreach (PrintKanBanCard kanBanCard in kanBanCardList)
                {
                    count += kanBanCard.KanBanDetails.Count;
                }
                if (count == 0) return false;

                //foreach (PrintKanBanCard kanBanCard in kanBanCardList)
                //{
                //    if (string.IsNullOrEmpty(hu.Item))
                //    {
                //        count++;
                //    }
                //}
                //if (count == 0) return false;

                this.barCodeFontName = this.GetBarcodeFontName(18, 0);

                int pageCount = (int)Math.Ceiling(count / (PAGE_DETAIL_ROW_COUNT * 1.0));
                /*
                for (int rowNum = 0; rowNum < 25; rowNum += 12)
                {
                    this.SetRowCellBarCode(1, rowNum, 0);
                    this.SetRowCellBarCode(1, rowNum, 7);
                    this.SetRowCellBarCode(1, rowNum, 14);
                }
                 */
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
                   

                    /*
                    if ( num == huList.Count + 1)
                    {
                        for (int i = 1; i <= (PAGE_DETAIL_ROW_COUNT - (num % PAGE_DETAIL_ROW_COUNT)); i++)
                        {
                            //YFKSS
                            //this.SetRowCell(pageIndex - 1, 1, 4, string.Empty, i);
                            //PART NO.
                            this.CopyCell(pageIndex - 1, this.getRowIndex(2, num - 1 + i), 0, "");
                            this.SetRowCell(pageIndex - 1, 2, 0, string.Empty, num - 1 + i);
                            //LOT/SERIAL NO.
                            this.CopyCell(pageIndex - 1, this.getRowIndex(4, num - 1 + i), 0, "");
                            this.SetRowCell(pageIndex - 1, 4, 0, string.Empty, num - 1 + i);
                            //PRINTED DATE:
                            this.CopyCell(pageIndex - 1, this.getRowIndex(10, num - 1 + i), 0, "");
                            this.SetRowCell(pageIndex - 1, 10, 0, string.Empty, num - 1 + i);
                            //PRINTER USER:
                            this.CopyCell(pageIndex - 1, this.getRowIndex(10, num - 1 + i), 2, "");
                            this.SetRowCell(pageIndex - 1, 10, 2, string.Empty, num - 1 + i);
                        }

                    }
                    */
                }

            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        private void writeContent(string companyCode, int pageIndex, int num, PrintKanBanCardInfo cardInfo, PrintKanBanCard kanBanCard,int count)
        {
            //this.SetMergedRegion(pageIndex, 1, 4, 9, 4, num);
            //ICell cell = this.GetCell(this.GetRowIndexAbsolute(pageIndex, getRowIndex(1, num)), getColumnIndex(4, num));
            //ICellStyle cellStyle = workbook.CreateCellStyle();
            //IFont font = workbook.CreateFont();
            //font.FontName = "宋体";
            //font.FontHeightInPoints = 24;
            //font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;
            //cellStyle.SetFont(font);
            //cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            //cellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.TOP;
            //cellStyle.Rotation = (short)-90;
            //cell.CellStyle = workbook.CreateCellStyle();
            //cell.CellStyle.CloneStyleFrom(cellStyle);
            //this.SetRowCell(pageIndex, 1, 4, companyCode, num);

            this.SetRowCell(pageIndex, 1, 0, kanBanCard.Flow, num);
            this.SetRowCell(pageIndex, 1, 2, kanBanCard.Routing, num);
            this.SetRowCell(pageIndex, 4, 0, cardInfo.CardNo, num);
            this.SetRowCell(pageIndex, 4, 2, kanBanCard.LocationTo, num);
            this.SetRowCell(pageIndex, 7, 0, kanBanCard.Item, num);
            this.SetRowCell(pageIndex, 7, 2, "IMAGE", num);//此行图片
            this.SetRowCell(pageIndex, 10, 0, kanBanCard.ReferenceItemCode, num);

            string barCode = Utility.BarcodeHelper.GetBarcodeStr("$K" + cardInfo.CardNo, this.barCodeFontName);
            this.SetRowCell(pageIndex, 13, 0, kanBanCard.ItemDescription, num);
            this.SetRowCell(pageIndex, 16, 3, cardInfo.Sequence.ToString() + "/" + kanBanCard.Qty.ToString(), num);
            this.SetRowCell(pageIndex, 17, 0, barCode, num);
            
           // this.SetRowCell(pageIndex, 17, 2, kanBanCard.UnitCount.ToString("0.########"), num);
            this.SetRowCell(pageIndex, 21, 0, kanBanCard.PackType, num);//PackType
            this.SetRowCell(pageIndex, 21, 2, kanBanCard.UnitCount.ToString("0.########"), num);//UnitCount
            this.SetRowCell(pageIndex, 24, 0, kanBanCard.MultiStation, num);//多工位
            this.SetRowCell(pageIndex, 27, 0, kanBanCard.ItemCategory, num);
            this.SetRowCell(pageIndex, 27, 2, DateTime.Now.ToString("yyyy-MM-dd"), num);

            this.SetRowCell(pageIndex, 29, 0,num+"-"+count,num);

        }


        protected void SetMergedRegion(int pageIndex, int row1, int column1, int row2, int colunm2, int num)
        {
            this.SetMergedRegion(pageIndex, getRowIndex(row1, num), getColumnIndex(column1, num), getRowIndex(row2, num), getColumnIndex(colunm2, num));
        }

        private int getRowIndex(int rowIndexRelative, int num)
        {
            //return ((int)(Math.Ceiling(num / 3.0)) - 1) * 12 + rowIndexRelative;
            //return (num-1)/一行几个/几行*一个占多少行
            return (num - 1) / 2 % 2 * 31 + rowIndexRelative;
        }

        private int getColumnIndex(int cellIndex, int num)
        {
            //return ((num - 1) % 几行)*一个占多少列
            return cellIndex + ((num - 1) % 2) * 5;
        }

        public void SetRowCell(int pageIndex, int rowIndexRelative, int cellIndex, String value, int num)
        {

            this.SetRowCell(pageIndex, getRowIndex(rowIndexRelative, num), getColumnIndex(cellIndex, num), value);
        }
    }
}