using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.INP;
using com.Sconit.PrintModel.INP;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepRejectOrderOperator : RepTemplate1
    {
        public RepRejectOrderOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 14;
            //列数   1起始
            this.columnCount = 13;
            //报表头的行数  1起始
            this.headRowCount = 9;
            //报表尾的行数  1起始
            this.bottomRowCount = 18;
        }

        /**
        * 填充报表
        * 
        * Param list [0]InspectOrder
        * Param list [0]IList<InspectOrderDetail>           
        */
        protected override bool FillValuesImpl(String templateFileName, IList<object> list)
        {
            try
            {

                if (list == null || list.Count < 2) return false;

                PrintRejectMaster printRejectMaster = (PrintRejectMaster)(list[0]);
                IList<PrintRejectDetail> PrintRejectDetailList = (IList<PrintRejectDetail>)(list[1]);


                if (printRejectMaster == null
                    || PrintRejectDetailList == null || PrintRejectDetailList.Count == 0)
                {
                    return false;
                }


                int count = 0;
                foreach (PrintRejectDetail rejectDetail in PrintRejectDetailList)
                {
                    //if (inspectResult.RejectedQty > 0)
                    //{
                        count++;
                    //}
                }
                if (count == 0) return false;
                this.barCodeFontName = this.GetBarcodeFontName(1, 7);
                this.CopyPage(count);

                this.FillHead(printRejectMaster);

                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;
                foreach (PrintRejectDetail printRejectDetail in PrintRejectDetailList)
                {

                    //    零件号 Part Name	
                    this.SetRowCell(pageIndex, rowIndex, 0, printRejectDetail.Item);
                    //"旧图号"
                    this.SetRowCell(pageIndex, rowIndex, 1, printRejectDetail.ReferenceItemCode);
                    //零件名称      Sta. No.
                    this.SetRowCell(pageIndex, rowIndex, 2, printRejectDetail.ItemDescription);
                    //"处理数量."
                    this.SetRowCell(pageIndex, rowIndex, 3, printRejectDetail.HandleQty.ToString("0.########"));
                    //"已处理数量."
                   // this.SetRowCell(pageIndex, rowIndex, 4, printRejectDetail.HandledQty.ToString());
                    //库位
                    this.SetRowCell(pageIndex, rowIndex, 4, printRejectDetail.LocationFrom);
                   // 处理方法  
                    this.SetRowCell(pageIndex, rowIndex, 5, string.Empty); 
                     //失效模式
                    this.SetRowCell(pageIndex, rowIndex, 6, printRejectDetail.FailCode);
                    //送货单号
                    this.SetRowCell(pageIndex, rowIndex, 7, printRejectDetail.IpNo);
                    //送货单行号
                    this.SetRowCell(pageIndex, rowIndex, 8, string.Empty);

                    if (this.isPageBottom(rowIndex, rowTotal))//页的最后一行
                    {
                        pageIndex++;
                        rowIndex = 0;
                    }
                    else
                    {
                        rowIndex++;
                    }
                    rowTotal++;

                }

                this.sheet.DisplayGridlines = false;
                this.sheet.IsPrintGridlines = false;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        /*
        * 填充不合格品头
        * 
        * Param repack 不合格品头对象
        */
        private void FillHead(PrintRejectMaster printRejectMaster)
        {
            //单号
            string RejectNo = Utility.BarcodeHelper.GetBarcodeStr(printRejectMaster.RejectNo, this.barCodeFontName);
            this.SetRowCell(1, 7, RejectNo);
            this.SetRowCell(3, 7, printRejectMaster.RejectNo);
            //部门/小组
            //this.SetRowCell(5, 1,  );
            //班次
            //this.SetRowCell(5, 3, inspectOrder );
            //填写人
            this.SetRowCell(5, 4, printRejectMaster.CreateUserName);
            //日期
            this.SetRowCell(5, 7, printRejectMaster.CreateDate.ToString("yyyy-MM-dd HH:mm"));
        }

        public override void CopyPageValues(int pageIndex)
        {
            this.SetMergedRegion(pageIndex, 27, 5, 27, 6);

            this.CopyCell(pageIndex, 22, 0, "A23");
            this.CopyCell(pageIndex, 25, 0, "A26");
            this.CopyCell(pageIndex, 28, 0, "A29");
            this.CopyCell(pageIndex, 28, 3, "D29");
            this.CopyCell(pageIndex, 28, 6, "G29");

            this.CopyCell(pageIndex, 31, 0, "A32");
            this.CopyCell(pageIndex, 33, 6, "G34");
            this.CopyCell(pageIndex, 34, 0, "A35");
            this.CopyCell(pageIndex, 36, 0, "A37");
            this.CopyCell(pageIndex, 38, 0, "A39");
           
        }
    }
}
