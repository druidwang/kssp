using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.ORD;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepSequenceOrderOperator: RepTemplate1
    {
        public RepSequenceOrderOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 30;
            //列数   1起始
            this.columnCount = 11;
            //报表头的行数  1起始
            this.headRowCount = 7;
            //报表尾的行数  1起始
            this.bottomRowCount = 1;
        }

        /**
         * 填充报表
         * 
         * Param list [0]OrderHead
         * Param list [0]IList<OrderDetail>           
         */
        protected override bool FillValuesImpl(String templateFileName, IList<object> list)
        {
            try
            {
                if (list == null || list.Count < 2) return false;

                PrintSequenceMaster sequenceMaster = (PrintSequenceMaster)(list[0]);
                IList<PrintSequenceDetail> sequenceDetails = (IList<PrintSequenceDetail>)(list[1]);

                sequenceDetails = sequenceDetails.OrderBy(o => o.Sequence).ThenBy(o => o.Item).ToList();

                if (sequenceMaster == null
                    || sequenceDetails == null || sequenceDetails.Count == 0)
                {
                    return false;
                }


                //this.SetRowCellBarCode(0, 2, 8);
                this.barCodeFontName = this.GetBarcodeFontName(1,8);
                this.CopyPage(sequenceDetails.Count);

                this.FillHead(sequenceMaster);


                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;
                foreach (PrintSequenceDetail sequenceDetail in sequenceDetails)
                {
                    // 顺序号.	
                    this.SetRowCell(pageIndex, rowIndex, 0, "" + sequenceDetail.Sequence);
                    
                    //位置
                    this.SetRowCell(pageIndex, rowIndex, 1,"");

                    //零件号 Item Code
                    this.SetRowCell(pageIndex, rowIndex, 2, sequenceDetail.Item);

                    //描述Description
                    this.SetRowCell(pageIndex, rowIndex, 3, sequenceDetail.ItemDescription);

                    //供应商
                    this.SetRowCell(pageIndex, rowIndex, 5, sequenceDetail.ManufactureParty);

                    //单位Unit
                    this.SetRowCell(pageIndex, rowIndex, 7, sequenceDetail.Uom);

                    //订单数
                    this.SetRowCell(pageIndex, rowIndex, 8, sequenceDetail.Qty.ToString("0.########"));

                    //发货数
                    this.SetRowCell(pageIndex, rowIndex, 9, "");

                    //VAN
                    this.SetRowCell(pageIndex, rowIndex, 10, sequenceDetail.TraceCode);

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
         * 填充报表头
         * 
         * Param repack 报验单头对象
         */
        private void FillHead(PrintSequenceMaster sequenceMaster)
        {
            //排序单号:
            string seqCode = Utility.BarcodeHelper.GetBarcodeStr(sequenceMaster.SequenceNo, this.barCodeFontName);
            this.SetRowCell(1, 8, seqCode);
            //排序单号 No.:
            this.SetRowCell(2, 8, sequenceMaster.SequenceNo);

            //来源组织
            this.SetRowCell(3, 2, sequenceMaster.PartyFrom);

            //来源库位
            this.SetRowCell(3, 4, sequenceMaster.LocationFrom);

            //发出时间 Create Time:
            this.SetRowCell(3, 8, sequenceMaster.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"));

            //目的组织
            this.SetRowCell(4, 2, sequenceMaster.PartyTo);

            //目的库位
            this.SetRowCell(4, 4, sequenceMaster.LocationTo);

            //窗口时间 
            this.SetRowCell(4, 8, sequenceMaster.WindowTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            this.CopyCell(pageIndex, 37, 0, "A38");
            this.CopyCell(pageIndex, 37, 4, "E38");
            this.CopyCell(pageIndex, 37, 8, "I38");
        }


    }
}
