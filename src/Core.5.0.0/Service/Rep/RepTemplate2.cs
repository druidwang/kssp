
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Services.Transaction;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace com.Sconit.Service.Report
{
    /*
     * 用于带重复左列头的excel报表
     * 
     * 
     */
    public abstract class RepTemplate2 : ReportBaseMgr
    {

        //明细部分的行数
        protected int pageDetailRowCount = 0;
        //列数  1起始
        protected int columnCount = 0;
        //左侧列头 1起始
        protected int leftColumnHeadCount = 0;
        //报表尾的行数  1起始
        protected int bottomRowCount = 0;

        //报表头的行数  1起始
        protected int headRowCount = 0;

        //记录数
        protected int resultCount = 0;


        public override bool FillValues(String templateFileName, IList<object> list)
        {
            if (list == null || list.Count < 1 || templateFileName == null || templateFileName.Length < 0) return false;
            this.rowCount = pageDetailRowCount + bottomRowCount;
            this.init(templateFileName);
            return this.FillValuesImpl(templateFileName, list);
        }


        protected bool isPageBottom(int rowIndex)
        {
            return (rowIndex == (this.resultCount - 1) || ((rowIndex + 1) % this.pageDetailRowCount == 0));
        }

        /*
         * 计算页数
         * 
         * Param resultCount 记录明细数
         * 
         */
        private int PageCount(int resultCount)
        {

            return (int)Math.Ceiling(resultCount / 1.0); ;
        }

        protected int CopyPage(int resultCount)
        {
            this.resultCount = resultCount;
            int pageCount = PageCount(resultCount);
            return this.CopyPage(pageCount, this.columnCount, 1);
        }


        /*
         * 根据相对位置行号获得绝对位置行号
         *
         * Param pageIndex 页码
         * Param rowIndexRelative 相对行号
         * 
         * Return 绝对位置行号
         */
        protected override int GetRowIndexAbsolute(int pageIndex, int rowIndexRelative)
        {
            return rowIndexRelative;
        }


        /*
         * 根据相对位置列号获得绝对位置列号
         *
         * Param pageIndex 页码
         * Param rowIndexRelative 相对列号
         * 
         * Return 绝对位置列号
         */
        protected override int GetColumnIndexAbsolute(int pageIndex, int columnIndexRelative)
        {
            if (pageIndex <= 1)
            {
                return columnIndexRelative;
            }
            else
            {
                return columnIndexRelative + (pageIndex - 1) * (columnCount - leftColumnHeadCount);
            }
        }

        /*
         * 设置列的宽度
         * 
         * Param pageIndex 页码
         * Param columnIndex 列号(相对值)
         */
        protected override void SetColumnWidth(int pageIndex, int columnIndex)
        {
            if (columnIndex != (this.leftColumnHeadCount - 1))
            {
                sheet.SetColumnWidth(this.GetColumnIndexAbsolute(pageIndex, columnIndex), sheet.GetColumnWidth(columnIndex));
            }
        }


        /**
        * 拷贝行
        * 
        * Param cellFrom 源单元格
        * Param cellTo 目标单元格
        * 
        */
        protected override void CopyRowStyle(IRow rowFrom, IRow rowTo, int columnFrom, int columnTo)
        {
            //rowTo.HeightInPoints = rowFrom.HeightInPoints;
            rowTo.Height = rowFrom.Height;
            for (int columnIndex = columnFrom; columnIndex < this.columnCount; columnIndex++)
            {
                if (columnIndex != (this.leftColumnHeadCount - 1))
                {
                    this.CopyCellStyle(this.GetCell(rowFrom.RowNum, columnIndex), this.GetCell(rowTo.RowNum, columnTo - this.leftColumnHeadCount + columnIndex));
                }
            }
        }


        protected abstract bool FillValuesImpl(String templateFileName, IList<object> list);
    }
}

