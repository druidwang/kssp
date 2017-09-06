using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel;
using com.Sconit.PrintModel.INV;
using NPOI.HSSF.UserModel;
//using NPOI.Util.IO;
using ThoughtWorks.QRCode.Codec;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepBarCode2DSpecialOperator : RepTemplate1
    {
        public RepBarCode2DSpecialOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 6;
            //列数   1起始
            this.columnCount = 4;
            //报表头的行数  1起始
            this.headRowCount = 0;
            //报表尾的行数  1起始
            this.bottomRowCount = 0;
        }

        /**
         * 需要拷贝的数据与合并单元格操作
         * 
         * Param pageIndex 页号
         */
        public override void CopyPageValues(int pageIndex)
        {
            this.SetMergedRegionColumn(pageIndex, 4, 0, 4, 3);
            this.SetMergedRegionColumn(pageIndex, 5, 0, 5, 3);
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
                var specialBarCodeList = (List<SpecialBarCode>)list[0];
                this.sheet.DisplayGridlines = false;
                this.sheet.IsPrintGridlines = false;
                int pageIndex = 1;

                this.CopyPage(pageDetailRowCount * specialBarCodeList.Count);
                foreach (var specialBarCode in specialBarCodeList)
                {
                    this.Fill2DBarCodeImage(pageIndex, 1, 1, 2, 2, specialBarCode.Code, 8);
                    this.SetColumnCell(pageIndex, 4, 0, specialBarCode.Desc1);
                    this.SetColumnCell(pageIndex, 5, 0, specialBarCode.Desc2);
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
