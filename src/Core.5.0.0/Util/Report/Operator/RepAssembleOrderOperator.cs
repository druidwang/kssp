using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.HPSF;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using com.Sconit.PrintModel.ORD;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepAssembleOrderOperator : ReportBase
    {
        private static readonly int ROW_COUNT = 10;
        //列数   1起始
        private static readonly int COLUMN_COUNT = 5;


        public RepAssembleOrderOperator() 
        {

        }

        /**
         * 需要拷贝的数据与合并单元格操作
         * 
         * Param pageIndex 页号
         */
        public override void CopyPageValues(int pageIndex)
        {
            
        }

        /**
         * 填充报表
         * 
         * Param list [0]huDetailList
         */
        public override bool FillValues(String templateFileFolder, String templateFileName, IList<object> list)
        {
            try
            {
                this.init(templateFileFolder.Replace("\r\n\t\t", "") + templateFileName, ROW_COUNT);

                if (list == null || list.Count < 2) return false;

                PrintOrderMaster orderMaster = (PrintOrderMaster)(list[0]);
                IList<PrintOrderDetail> orderDetails = (IList<PrintOrderDetail>)(list[1]);
 
                this.sheet.DisplayGridlines = false;
                this.sheet.IsPrintGridlines = false;

                this.barCodeFontName = this.GetBarcodeFontName(0, 0);

                ICellStyle cellStyleT = workbook.CreateCellStyle();
                IFont fontT = workbook.CreateFont();
                fontT.FontHeightInPoints = (short)9;
                fontT.FontName = "宋体";
                fontT.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.BOLD;
                cellStyleT.SetFont(fontT);

                int pageIndex = 1;

                string barCode = Utility.BarcodeHelper.GetBarcodeStr(orderMaster.OrderNo, this.barCodeFontName);
                this.SetRowCell(pageIndex, 0, 0, barCode);

                this.SetRowCell(pageIndex, 1, 0, orderMaster.OrderNo);

                //生产线编号	
                this.SetRowCell(pageIndex, 3, 0, orderMaster.Flow + "[" + orderMaster.FlowDescription + "]");

                //物料代码+描述
                this.SetRowCell(pageIndex, 4, 0, orderDetails[0].Item+"["+orderDetails[0].ItemDescription+"]");

                //Van号
                barCode = Utility.BarcodeHelper.GetBarcodeStr(orderMaster.TraceCode, this.barCodeFontName);
                this.SetRowCell(pageIndex, 5, 1, barCode);

                this.SetRowCell(pageIndex, 7, 1, orderMaster.TraceCode);

                //开始时间	
                this.SetRowCell(pageIndex, 8, 1, orderMaster.WindowTime.ToString("yyyy-MM-dd HH:mm"));

                //Printed Date:
                this.SetRowCell(pageIndex, 9, 3, DateTime.Now.ToString("MM/dd/yy"));

                this.sheet.SetRowBreak(this.GetRowIndexAbsolute(pageIndex, ROW_COUNT - 1));
                
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

    }
}
