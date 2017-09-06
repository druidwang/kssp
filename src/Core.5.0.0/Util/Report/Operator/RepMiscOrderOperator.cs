using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.ORD;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepMiscOrderOperator : RepTemplate1
    {
        public RepMiscOrderOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 20;
            //列数   1起始
            this.columnCount = 8;
            //报表头的行数  1起始
            this.headRowCount = 12;
            //报表尾的行数  1起始
            this.bottomRowCount = 1;
        }

        /**
         * 填充报表
         */
        protected override bool FillValuesImpl(String templateFileName, IList<object> list)
        {
            try
            {
                if (list == null || list.Count < 2) return false;

                PrintMiscOrderMaster miscOrderMaster = (PrintMiscOrderMaster)list[0];
                IList<PrintMiscOrderDetail> miscOrderDetailList = (IList<PrintMiscOrderDetail>)list[1];

                if (miscOrderMaster == null
                    && (miscOrderDetailList == null || miscOrderDetailList.Count == 0))
                {
                    return false;
                }
                this.barCodeFontName = this.GetBarcodeFontName(1, 4);
                this.CopyPage(miscOrderDetailList.Count);
                this.FillHead(miscOrderMaster);
                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;

                foreach (PrintMiscOrderDetail miscOrderDetail in miscOrderDetailList)
                {
                    //序号
                    this.SetRowCell(pageIndex, rowIndex, 0, miscOrderDetail.Sequence);

                    string location = miscOrderDetail.Location;
                    location = string.IsNullOrWhiteSpace(location) ? miscOrderMaster.Location : location;
                    //库位
                    this.SetRowCell(pageIndex, rowIndex, 1, location);

                    //零件号
                    this.SetRowCell(pageIndex, rowIndex, 2, miscOrderDetail.Item);

                    //零件描述
                    this.SetRowCell(pageIndex, rowIndex, 3, miscOrderDetail.ItemDescription);

                    //参考物料号
                    this.SetRowCell(pageIndex, rowIndex, 4, miscOrderDetail.ReferenceItemCode);

                    //单位
                    this.SetRowCell(pageIndex, rowIndex, 5, miscOrderDetail.Uom);

                    //数量
                    this.SetRowCell(pageIndex, rowIndex, 6, miscOrderDetail.Qty.ToString("0.###"));

                    if (miscOrderMaster.MoveType == "281")
                    {
                        this.SetRowCell(pageIndex, rowIndex, 7, miscOrderDetail.ReserveLine + "-" + miscOrderDetail.ReserveNo);
                    }

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
         * Param pageIndex 页号
         * Param orderHead 订单头对象
         * Param orderDetails 订单明细对象
         */
        protected void FillHead(PrintMiscOrderMaster miscOrderMaster)
        {
            //抬头
            this.SetRowCell(0, 4, miscOrderMaster.Title);

            //计划外出入库单号:
            string orderCode = Utility.BarcodeHelper.GetBarcodeStr(miscOrderMaster.MiscOrderNo, this.barCodeFontName);
            this.SetRowCell(1, 4, orderCode);

            //Order No.:
            this.SetRowCell(2, 4, miscOrderMaster.MiscOrderNo);

            if (miscOrderMaster.Status == 2)
            {
                this.SetRowCell(2, 2, "☑已冲销");
            }
            else
            {
                this.SetRowCell(2, 2, string.Empty);
            }

            //创建日期:	
            this.SetRowCell(4, 2, miscOrderMaster.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"));
            //创建者: 
            this.SetRowCell(4, 5, miscOrderMaster.CreateUserName);

            //区域:
            this.SetRowCell(5, 2, miscOrderMaster.Region);

            //库位:
            this.SetRowCell(5, 5, miscOrderMaster.Location);

            //质量类型:
            this.SetRowCell(6, 2, miscOrderMaster.QualityTypeDescription);

            //参考订单号:
            this.SetRowCell(6, 5, miscOrderMaster.ReferenceNo);

            //移库类型:
            this.SetRowCell(7, 2, miscOrderMaster.MoveType);

            //移动原因:
            this.SetRowCell(7, 5, miscOrderMaster.Note);

            //生效日期:	
            this.SetRowCell(8, 2, miscOrderMaster.EffectiveDate.ToString("yyyy-MM-dd HH:mm:ss"));

            //成本中心:
            if (miscOrderMaster.MoveType == "201" || miscOrderMaster.MoveType == "202")
            {
       
                this.SetRowCell(8, 5, miscOrderMaster.CostCenter);
            }
            else
            {
                this.SetRowCell(8, 4, string.Empty);
                this.SetRowCell(8, 5, string.Empty);
            }
            //路线:
            if (miscOrderMaster.MoveType == "261" || miscOrderMaster.MoveType == "262"
                || miscOrderMaster.MoveType == "281" || miscOrderMaster.MoveType == "581")
            {

                if (!string.IsNullOrWhiteSpace(miscOrderMaster.Flow))
                {
                    this.SetRowCell(9, 2, miscOrderMaster.Flow);
                }
                else
                {
                    this.SetRowCell(9, 0, string.Empty);
                    this.SetRowCell(9, 2, string.Empty);
                }
            }
            else
            {
                this.SetRowCell(9, 0, string.Empty);
                this.SetRowCell(9, 2, string.Empty);
            }
            //WBS:
            if (miscOrderMaster.MoveType == "581")
            {

                this.SetRowCell(9, 5, miscOrderMaster.WBS);
            }
            else
            {
                this.SetRowCell(9, 4, string.Empty);
                this.SetRowCell(9, 5, string.Empty);
            }

            ////科目代码:	
            //this.SetRowCell(6, 2, "");

            ////分账号代码:	
            //this.SetRowCell(7, 2, "");


            ////项目代码:
            //this.SetRowCell(7, 5, "");
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            //签字
            this.CopyCell(pageIndex, 32, 3, "D33");
        }
    }
}
