namespace com.Sconit.Utility.Report
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NPOI.HSSF.UserModel;
    using System.IO;
    using NPOI.HPSF;
    using NPOI.SS.UserModel;
    using NPOI.HSSF.Util;
    using System.Text;
    using ThoughtWorks.QRCode.Codec;
    //using System.Web.Mvc.ControllerBase;

    public abstract class ReportBase : IReportBase
    {

        protected string barCodeFontName;
        protected short barCodeFontSize;

        protected HSSFWorkbook workbook;

        protected ISheet sheet;


        /*
         * protected IRow row;
        protected CellStyle style;

        protected Font font;
        */
        protected int rowCount = 0;

        // 设置cell编码解决中文高位字节截断
        //private static short XLS_ENCODING = HSSFWorkbook.ENCODING_UTF_16;

        //private static readonly String BARCODEFONT_CODE39 = "C39HrP24DhTt";
        //private static readonly String BARCODEFONT_3OF9 = "3 of 9 Barcode";

        public ReportBase()
        {
            this.workbook = new HSSFWorkbook();

            this.sheet = workbook.CreateSheet(Guid.NewGuid().ToString("N"));

            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "YFGM Company";

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "Report";
            si.Author = "tiansu";

            workbook.DocumentSummaryInformation = dsi;
            workbook.SummaryInformation = si;

            this.sheet.ForceFormulaRecalculation = true;

        }


        /**
        * 
        * Param templateFilePath 模版文件路径
        * 
        */
        protected virtual void init(String templateFilePath)
        {
            FileStream file = new FileStream(templateFilePath, FileMode.Open, FileAccess.Read);
            this.workbook = new HSSFWorkbook(file);
            this.sheet = workbook.GetSheetAt(0);

            this.sheet.ForceFormulaRecalculation = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateFilePath"></param>
        /// <param name="sheetIndex"></param>
        protected virtual void init(String templateFilePath, int sheetIndex, string bySheet)
        {
            if (sheetIndex == 0)
            {
                FileStream file = new FileStream(templateFilePath, FileMode.Open, FileAccess.Read);
                this.workbook = new HSSFWorkbook(file);
            }
            this.sheet = workbook.GetSheetAt(sheetIndex);
            this.sheet.ForceFormulaRecalculation = true;
        }

        /**
        * 
        * Param templateFilePath 模版文件路径
        * Param rowCount 行数
         * workbook.GetSheetAt(0)要控制打印第几个模板子加一个方法或者传参数进来不要再原基础上改
        */
        protected virtual void init(String templateFilePath, int rowCount)
        {
            this.rowCount = rowCount;
            FileStream file = new FileStream(templateFilePath, FileMode.Open, FileAccess.Read);
            this.workbook = new HSSFWorkbook(file);
            this.sheet = workbook.GetSheetAt(0);

            this.sheet.ForceFormulaRecalculation = true;
        }

        protected ISheet GetSheet()
        {
            return workbook.GetSheetAt(0);
        }

        protected ISheet GetSheet(int index)
        {
            return workbook.GetSheetAt(index);
        }

        /**
         * 页处理
         *      pageCount==1,隐藏模版中的第二页
         *      pageCount==2,不做处理
         *      pageCount>2,填充页
         * 
         * Param pageCount 页数
         * Param columnCount 列数
         */
        public int CopyPage(int pageCount, int columnCount)
        {
            return this.CopyPage(pageCount, columnCount, 2);
        }


        /**
         * 页处理
         *      pageCount==1,隐藏模版中的第二页
         *      pageCount==pageNum,不做处理
         *      pageCount>pageNum,填充页
         * 
         * Param pageCount 页数
         * Param columnCount 列数
         * Param pageNum 参照页
         */
        protected int CopyPage(int pageCount, int columnCount, int pageNum)
        {
            if (pageCount == 1 && pageNum == 2)
            {
                //删除行
                for (int i = this.rowCount; i < (this.rowCount * 2); i++)
                {
                    IRow rowT = this.GetRow(i);
                    //this.sheet.RemoveRow(rowT);
                    rowT.ZeroHeight = true;
                }
            }
            if (pageCount > pageNum)
            {

                for (int pageIndex = pageNum + 1; pageIndex <= pageCount; pageIndex++)
                {

                    if (this.GetColumnIndexAbsolute(pageIndex, columnCount) != columnCount)
                    {
                        //列宽读
                        for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
                        {
                            this.SetColumnWidth(pageIndex, columnIndex);
                        }
                    }

                    //此方法之类实现
                    this.CopyPageValues(pageIndex);
                    for (int rowNum = 0; rowNum < rowCount; rowNum++)
                    {
                        this.CopyRowStyle(this.GetRowIndexAbsolute(1, rowNum), this.GetRowIndexAbsolute(pageIndex, rowNum), this.GetColumnIndexAbsolute(1, 0), this.GetColumnIndexAbsolute(pageIndex - 1, columnCount));
                    }
                }
            }
            return pageCount;
        }


        /**
         * 页处理
         *      pageCount>pageNum,填充页
         * 
         * Param pageCount 页数
         * Param columnCount 行数
         * Param pageNum 参照页
         */
        protected int CopyPageCloumn(int pageCount, int columnCount, int pageNum)
        {
            if (pageCount > pageNum)
            {

                for (int pageIndex = pageNum + 1; pageIndex <= pageCount; pageIndex++)
                {
                    //此方法之类实现
                    this.CopyPageValues(pageIndex);
                    for (int rowNum = 0; rowNum < rowCount; rowNum++)
                    {
                        this.CopyRowStyleColumn(this.GetRowIndexAbsolute(1, rowNum), this.GetRowIndexAbsolute(pageIndex, rowNum), columnCount);
                    }
                }
            }
            return pageCount;
        }

        /*
         * 设置列的宽度
         * 
         * Param pageIndex 页码
         * Param columnIndex 列号(相对值)
         */
        protected virtual void SetColumnWidth(int pageIndex, int columnIndex)
        {
            sheet.SetColumnWidth(this.GetColumnIndexAbsolute(pageIndex, columnIndex), sheet.GetColumnWidth(columnIndex));
        }


        /*
         * 合并单元格
         * 
         * Param pageIndex 页码
         * Param row1 起始行号(相对值)
         * Param column1 起始列号
         * Param row2 终止行号(相对值)
         * Param colunm2 终止列号
         */
        protected virtual void SetMergedRegion(int pageIndex, int row1, int column1, int row2, int colunm2)
        {
            this.sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(this.GetRowIndexAbsolute(pageIndex, row1), this.GetRowIndexAbsolute(pageIndex, row2), this.GetColumnIndexAbsolute(pageIndex, column1), this.GetColumnIndexAbsolute(pageIndex, colunm2)));
        }

        //纵向打印
        protected virtual void SetMergedRegionColumn(int pageIndex, int row1, int column1, int row2, int colunm2)
        {
            this.sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(this.GetRowIndexAbsolute(pageIndex, row1), this.GetRowIndexAbsolute(pageIndex, row2), column1, colunm2));
        }

        /*
         * 合并单元格
         *
         * Param row1 起始行号
         * Param column1 起始列号
         * Param row2 终止行号
         * Param colunm2 终止列号
         */
        protected void SetMergedRegion(int row1, int column1, int row2, int colunm2)
        {
            this.sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(row1, row2, column1, colunm2));
        }

        /*
         * 拷贝单元格
         *
         * Param pageIndex 页码
         * Param rowIndexRelative 行号(相对值)
         * Param cellIndex 列号
         * Param formula 公式
         */
        protected virtual void CopyCell(int pageIndex, int rowIndexRelative, int cellIndex, string formula)
        {
            this.SetRowCellFormula(this.GetRowIndexAbsolute(pageIndex, rowIndexRelative), this.GetColumnIndexAbsolute(pageIndex, cellIndex), formula);
        }

        //纵向打印
        protected virtual void CopyCellColumn(int pageIndex, int rowIndexRelative, int cellIndex, string formula)
        {
            this.SetRowCellFormula(this.GetRowIndexAbsolute(pageIndex, rowIndexRelative), cellIndex, formula);
        }

        protected void SetRowCellBarCode(int pageIndex, int rowIndexRelative, int cellIndex)
        {
            this.SetRowCellStyle(pageIndex, rowIndexRelative, cellIndex, barCodeFontName, barCodeFontSize);
        }

        /*
        * 设置单元格样式
        *
        * Param pageIndex 页码
        * Param rowIndexRelative 相对行号
        * Param cellIndex 列号
        * Param fontName 字体名称
        * Param fontSize 字体大小
        * 
        * Return 生成文件的URL
        */
        protected void SetRowCellStyle(int pageIndex, int rowIndexRelative, int cellIndex, String fontName, short fontSize)
        {
            int rowIndexAbsolute = this.GetRowIndexAbsolute(pageIndex, rowIndexRelative);
            IRow row = this.GetRow(rowIndexAbsolute);

            int columIndexAbsolute = this.GetColumnIndexAbsolute(pageIndex, cellIndex);
            ICell cell = row.GetCell((short)columIndexAbsolute);
            if (cell == null) cell = row.CreateCell((short)columIndexAbsolute);
            ICellStyle style = workbook.CreateCellStyle();
            if (cell.CellStyle != null)
            {
                style.CloneStyleFrom(cell.CellStyle);
            }
            IFont font = workbook.CreateFont();
            font.FontName = fontName;
            font.FontHeightInPoints = fontSize;
            style.SetFont(font);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.CloneStyleFrom(style);
        }



        /*
         * 设置单元格值
         *
         * Param pageIndex 页码
         * Param rowIndexRelative 相对行号
         * Param cellIndex 列号
         * Param value 目标值
         * 
         * Return 生成文件的URL
         */
        protected virtual void SetRowCell(int pageIndex, int rowIndexRelative, int cellIndex, String value)
        {
            int rowIndexAbsolute = this.GetRowIndexAbsolute(pageIndex, rowIndexRelative);
            IRow row = this.GetRow(rowIndexAbsolute);

            int columIndexAbsolute = this.GetColumnIndexAbsolute(pageIndex, cellIndex);

            ICell cell = row.GetCell((short)columIndexAbsolute);
            if (cell == null) cell = row.CreateCell((short)columIndexAbsolute);
            cell.SetCellValue(value);
        }

        /*
         * 设置单元格值
         *
         * Param pageIndex 页码
         * Param rowIndexRelative 相对行号
         * Param cellIndex 列号
         * Param value 目标值
         */
        //纵向打印
        protected virtual void SetColumnCell(int pageIndex, int rowIndexRelative, int cellIndex, String value)
        {
            int rowIndexAbsolute = this.GetRowIndexAbsolute(pageIndex, rowIndexRelative);
            IRow row = this.GetRow(rowIndexAbsolute);
            ICell cell = row.GetCell((short)cellIndex);
            if (cell == null) cell = row.CreateCell((short)cellIndex);
            cell.SetCellValue(value);
        }

        /*
        * 设置单元格值
        *
        * Param pageIndex 页码
        * Param rowIndexRelative 相对行号
        * Param cellIndex 列号
        * Param value 目标值
        * 
        * Return 生成文件的URL
        */
        protected virtual void SetRowCell(int pageIndex, int rowIndexRelative, int cellIndex, double value)
        {
            int rowIndexAbsolute = this.GetRowIndexAbsolute(pageIndex, rowIndexRelative);
            IRow row = this.GetRow(rowIndexAbsolute);

            int columIndexAbsolute = this.GetColumnIndexAbsolute(pageIndex, cellIndex);

            ICell cell = row.GetCell((short)columIndexAbsolute);
            if (cell == null) cell = row.CreateCell((short)columIndexAbsolute);
            cell.SetCellValue(value);
        }

        /*
         * 根据相对位置行号获得绝对位置行号
         *
         * Param pageIndex 页码
         * Param rowIndexRelative 相对行号
         * 
         * Return 绝对位置行号
         */
        protected virtual int GetRowIndexAbsolute(int pageIndex, int rowIndexRelative)
        {
            int rowIndexAbsolute = this.rowCount * (pageIndex - 1) + rowIndexRelative;
            return rowIndexAbsolute;
        }

        /*
         * 根据相对位置列号获得绝对位置列号
         *
         * Param pageIndex 页码
         * Param rowIndexRelative 相对列号
         * 
         * Return 绝对位置列号
         */
        protected virtual int GetColumnIndexAbsolute(int pageIndex, int columnIndexRelative)
        {
            return columnIndexRelative;
        }


        /*
         * 设置单元格
         *
         * Param rowIndex 行号
         * Param cellIndex 列号
         * Param value 值
         * 
         * Return 绝对位置行号
         */
        /// <summary>
        /// 0开始 rowIndex 行号 cellIndex 列号 value 值
        /// </summary>
        protected void SetRowCell(int rowIndex, int cellIndex, String value)
        {
            IRow row = this.GetRow(rowIndex);
            ICell cell = row.GetCell((short)cellIndex);
            if (cell == null) cell = row.CreateCell((short)cellIndex);
            cell.SetCellValue(value);
        }

        /// <summary>
        /// 0开始 rowIndex 行号 cellIndex 列号 value 值
        /// </summary>
        protected void SetRowCell(int rowIndex, int cellIndex, DateTime value)
        {
            IRow row = this.GetRow(rowIndex);
            ICell cell = row.GetCell((short)cellIndex);
            if (cell == null) cell = row.CreateCell((short)cellIndex);
            cell.SetCellValue(value);
        }

        /// <summary>
        /// 0开始 rowIndex 行号 cellIndex 列号 value 值
        /// </summary>
        protected void SetRowCell(int rowIndex, int cellIndex, double value)
        {
            IRow row = this.GetRow(rowIndex);
            ICell cell = row.GetCell((short)cellIndex);
            if (cell == null) cell = row.CreateCell((short)cellIndex);
            cell.SetCellValue(value);
        }

        //public void AddPicture(int rowIndex, int cellIndex, byte[] bytes)
        //{
        //    int pictureIdx = this.workbook.AddPicture(bytes, HSSFWorkbook.PICTURE_TYPE_PNG);

        //    // Create the drawing patriarch.  This is the top level container for all shapes. 
        //    HSSFPatriarch patriarch = this.sheet.CreateDrawingPatriarch();

        //    //add a picture
        //    HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 1023, 0, rowIndex, cellIndex, rowIndex, cellIndex);
        //    HSSFPicture pict = patriarch.CreatePicture(anchor, pictureIdx);
        //    //pict.Resize();
        //}

        /**
         * 
         * 设置单元格公式
         * 
         * Param rowIndex 行号
         * Param cellIndex 列号
         * Param formula 公式
         * 
         */
        protected void SetRowCellFormula(int rowIndex, int cellIndex, string formula)
        {
            IRow row = this.GetRow(rowIndex);
            ICell cell = row.GetCell((short)cellIndex);
            if (cell == null) cell = row.CreateCell((short)cellIndex);
            cell.CellFormula = formula;
        }


        /**
         * 
         * 获得行对象
         * 
         * Param rowIndex 行号
         * Return 行对象
         */
        protected IRow GetRow(int rowIndex)
        {
            IRow row = null;
            row = this.sheet.GetRow(rowIndex);
            if (row == null) row = this.sheet.CreateRow(rowIndex);
            return row;
        }

        /**
         * 
         * 拷贝行样式
         * 
         * Param rowIndexFrom 源行号
         * Param rowIndexTo 目标行号
         * Param columnCount 列数
         * 
         * Return 行对象
         */
        private void CopyRowStyle(int rowIndexFrom, int rowIndexTo, int columnFrom, int columnTo)
        {
            this.CopyRowStyle(this.GetRow(rowIndexFrom), this.GetRow(rowIndexTo), columnFrom, columnTo);
        }

        //纵向打印
        private void CopyRowStyleColumn(int rowIndexFrom, int rowIndexTo, int columnCount)
        {
            this.CopyRowStyleColumn(this.GetRow(rowIndexFrom), this.GetRow(rowIndexTo), columnCount);
        }

        private void CopyRowStyleColumn(IRow rowFrom, IRow rowTo, int columnCount)
        {
            //rowTo.HeightInPoints = rowFrom.HeightInPoints;
            rowTo.Height = rowFrom.Height;
            for (int i = 0; i < columnCount; i++)
            {
                this.CopyCellStyle(this.GetCell(rowFrom.RowNum, i), this.GetCell(rowTo.RowNum, i));
            }
        }
        /**
        * 拷贝行
        * 
        * Param cellFrom 源单元格
        * Param cellTo 目标单元格
        * 
        */
        protected virtual void CopyRowStyle(IRow rowFrom, IRow rowTo, int columnFrom, int columnTo)
        {
            //rowTo.HeightInPoints = rowFrom.HeightInPoints;
            rowTo.Height = rowFrom.Height;
            for (int i = columnFrom; i < columnTo; i++)
            {
                this.CopyCellStyle(this.GetCell(rowFrom.RowNum, i), this.GetCell(rowTo.RowNum, i));
            }
        }

        /**
         * 拷贝单元格样式
         * 
         * Param cellFrom 源单元格
         * Param cellTo 目标单元格
         * 
         */
        protected void CopyCellStyle(ICell cellFrom, ICell cellTo)
        {
            //cellTo.CellStyle = workbook.CreateCellStyle();
            //cellTo.CellStyle.CloneStyleFrom(cellFrom.CellStyle);
            cellTo.CellStyle = cellFrom.CellStyle;
        }

        /**
         * 获得单元格对象
         * 
         * Param rowIndex 行号
         * Param cellIndex 列号
         * 
         * Return 单元格对象
         */
        protected ICell GetCell(int rowIndex, int cellIndex)
        {
            IRow row = this.GetRow(rowIndex);
            ICell cell = row.GetCell((short)cellIndex);
            if (cell == null) cell = row.CreateCell((short)cellIndex);
            return cell;
        }

        public HSSFWorkbook GetWorkbook()
        {
            return workbook;
        }


        protected string GetBarcodeFontName(int rowIndex, int cellIndex)
        {
            return this.GetCell(rowIndex, cellIndex).CellStyle.GetFont(this.workbook).FontName;
        }


        /**
         * 填充报表值
         * 
         * Param list 参数
         * 
         * Return 成功
         */
        public virtual bool FillValues(String templateFileFolder, String templateFileName, IList<object> list)
        {
            return true;
        }
        public virtual bool FillValues(String templateFileFolder, String templateFileName, IList<object> list, int sheetIndex)
        {
            return true;
        }

        //public bool FillValues(String templateFileName, string orderNo)
        //{
        //    return FillValues(templateFileName, GetDataList(orderNo));
        //}


        /**
         * 拷贝单元格
         * 
         * Param pageIndex 页码
         */
        public abstract void CopyPageValues(int pageIndex);



        /// <summary>
        /// 获取并填充二维条码图片
        /// </summary>
        /// <param name="barCode">条码</param>
        /// <param name="col1">图片填充起始列,从0开始</param>
        /// <param name="row1">图片填充起始行,从0开始</param>
        /// <param name="col2">图片填充终止列,从0开始</param>
        /// <param name="row2">图片填充终止行,从0开始</param>
        protected void Fill2DBarCodeImage(int pageIndex, int col1, int row1, int col2, int row2, string barCode, int qRCodeScale = 6)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
            qrCodeEncoder.QRCodeScale = qRCodeScale;
            qrCodeEncoder.QRCodeVersion = 1;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
            var image = qrCodeEncoder.Encode(barCode);

            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            var patriarch = this.sheet.CreateDrawingPatriarch();

            int _rowIndex = this.GetRowIndexAbsolute(pageIndex, 0);
            HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 0, 0, col1, row1 + _rowIndex, col2 + 1, row2 + _rowIndex + 1);
            anchor.AnchorType = 2;
            int index = this.workbook.AddPicture(ms.ToArray(), NPOI.SS.UserModel.PictureType.PNG);
            var signaturePicture = patriarch.CreatePicture(anchor, index);
            //signaturePicture.Resize();
        }

        protected string GetShiftName(string shift)
        {
            if (string.IsNullOrWhiteSpace(shift))
            {
                return string.Empty;
            }
            else if (shift.Contains("3-1"))
            {
                return "早班";
            }
            else if (shift.Contains("3-2"))
            {
                return "中班";
            }
            else if (shift.Contains("3-3"))
            {
                return "夜班";
            }
            else if (shift.Contains("2-1"))
            {
                return "早班";
            }
            else if (shift.Contains("2-2"))
            {
                return "夜班";
            }
            else
            {
                return shift;
            }
        }
    }
}

