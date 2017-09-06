using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace com.Sconit.Utility.Report.Operator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using com.Sconit.Entity.ORD;
    using com.Sconit.Entity;
    using com.Sconit.PrintModel.ORD;

    public class RepORDProductEXOrderOperator : RepTemplate1
    {
        public RepORDProductEXOrderOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 33;
            //列数   1起始
            this.columnCount = 9;
            //报表头的行数  1起始
            this.headRowCount = 12;
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
                if (list == null || list.Count < 3) return false;

                PrintOrderMaster orderHead = (PrintOrderMaster)(list[0]);
                IList<PrintOrderDetail> orderDetails = (IList<PrintOrderDetail>)(list[1]);
                List<PrintOrderBomDetail> OrderBomDetails = (List<PrintOrderBomDetail>)(list[2]);
                //物料信息
                ////物料号	物料描述	单位	单用量	计划数	实消耗	其他	
                orderDetails = orderDetails.OrderBy(o => o.Sequence).ThenBy(o => o.Item).ToList();

                if (orderHead == null
                    || orderDetails == null || orderDetails.Count == 0)
                {
                    return false;
                }

                this.barCodeFontName = this.GetBarcodeFontName(0, 6);
                this.CopyPage(OrderBomDetails.Count);
                //Get Remark for head area
                orderHead.remark = orderDetails.ToList().First().Remark;
                this.FillHead(orderHead);


                int pageIndex = 1;
                int rowIndex = 0;
                int rowIndex1 = 7;//用于成品明细
                int rowTotal = 0;
                //****成品明细  成品	成品描述	单位	包装	计划数	收货数	废品数	备注	
                foreach (PrintOrderDetail orderDetail in orderDetails)
                {
                    //不分页 ，取出两行资料直接结束循环
                    if (rowIndex1 == 9)
                    {
                        break;
                    }
                    //"成品物料号 FG Item Code"	
                    this.SetRowCell(rowIndex1, 0, orderDetail.Item);
                    //"描述Description"	
                    this.SetRowCell(rowIndex1, 1, orderDetail.ItemDescription);
                    //"单位Unit"	
                    this.SetRowCell(rowIndex1, 2, orderDetail.Uom);
                    //"包装UC"	
                    this.SetRowCell(rowIndex1, 3, orderDetail.UnitCount.ToString("0.########"));
                    //"计划数Dmd Qty"	
                    this.SetRowCell(rowIndex1, 4, orderDetail.OrderedQty.ToString("0.########"));
                    ////车数
                    //this.SetRowCell(pageIndex, rowIndex, 5, orderDetail.UnitCount == 0 ? orderDetail.OrderedQty.ToString() : ((int)(orderDetail.OrderedQty / orderDetail.UnitCount)).ToString());
                    //收货数---Close,Complete,Cancel显示
                    if (orderHead.Status == 3 || orderHead.Status == 4 || orderHead.Status == 5)
                    {
                        this.SetRowCell(rowIndex1, 5, orderDetail.ReceivedQty.ToString("0.##"));
                    }
                    //"废品数Scrap Qty"	
                    this.SetRowCell(rowIndex1, 6, orderDetail.ScrapQty.ToString("0.##"));
                    ////方向
                    //this.SetRowCell(pageIndex, rowIndex, 7, orderDetail.Direction);
                    //备注	
                    this.SetRowCell(rowIndex1, 7, orderDetail.Remark);
                    rowIndex1++;
                }
                //****材料明细  物料号(材料号)	物料描述	单位	单用量	计划数	实消耗	其他	
                foreach (PrintOrderBomDetail OrderBomDetail in OrderBomDetails.Take(33))
                {

                    //物料号
                    this.SetRowCell(pageIndex, rowIndex, 0, OrderBomDetail.Item);
                    //"物料描述
                    this.SetRowCell(pageIndex, rowIndex, 1, OrderBomDetail.ItemDescription);
                    //单位
                    this.SetRowCell(pageIndex, rowIndex, 2, OrderBomDetail.Uom);
                    //单用量
                    this.SetRowCell(pageIndex, rowIndex, 3, OrderBomDetail.BomUnitQty.ToString("0.########"));
                    //计划数
                    this.SetRowCell(pageIndex, rowIndex, 4, OrderBomDetail.OrderedQty.ToString("0.########"));
                    //实消耗	取相反数
                    this.SetRowCell(pageIndex, rowIndex, 5, (-OrderBomDetail.BackflushedQty).ToString("0.########"));
                    //其他???
                    //this.SetRowCell(pageIndex, rowIndex, 3, OrderBomDetail.)

                    //if (this.isPageBottom(rowIndex, rowTotal))//页的最后一行不需要分页
                    //{
                    //    pageIndex++;
                    //    rowIndex = 0;
                    //}
                    //else
                    //{
                    rowIndex++;
                    //}
                    //rowTotal++;
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
         * Param repack
         */
        private void FillHead(PrintOrderMaster orderHead)
        {
            #region 报表头
            if (orderHead.SubType == 1)
            {
                this.SetRowCell(1, 5, "退货");
            }
            //this.SetRowCell(pageIndex, 0, 4, orderHead.Sequence.ToString());
            //注意事项
            this.SetRowCell(4, 1, orderHead.Dock);
            //工单号码Order code
            string orderCode = Utility.BarcodeHelper.GetBarcodeStr(orderHead.OrderNo, this.barCodeFontName);
            this.SetRowCell(0, 6, orderCode);
            this.SetRowCell(1, 6, orderHead.OrderNo);
            // "生产线：Prodline："
            //Flow flow = this.flowMgr.LoadFlow(orderHead.Flow);
            this.SetRowCell(2, 1, orderHead.FlowDescription + "(" + orderHead.Flow + ")");
            //"生产班组：Shift："
            this.SetRowCell(2, 3, orderHead.Shift == null ? string.Empty : orderHead.Shift);
            //"发单人：Issuer："
            this.SetRowCell(2, 7, orderHead.CreateUserName);
            ////"交货地点：Shipto："
            //this.SetRowCell(3, 7, orderHead.PartyToName);
            //"参考订单号：Shipto："
            this.SetRowCell(3, 7, orderHead.ReferenceOrderNo);
            //开始时间
            this.SetRowCell(3, 1, orderHead.StartTime.ToString("yyyy-MM-dd HH:mm"));
            //结束时间
            this.SetRowCell(3, 3, orderHead.WindowTime.ToString("yyyy-MM-dd HH:mm"));
            //备注
            //this.SetRowCell(4, 1, orderHead.remark);
            //"注意事项：Remarks："
            // this.SetRowCell(7, 1, headremark);
            //"联系电话：Tel："
            //this.SetRowCell(pageIndex, 6, 1, string.Empty);

            #endregion
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            //this.SetMergedRegionColumn(pageIndex, 0, 2, 0, 3);
            //this.SetMergedRegionColumn(pageIndex, 1, 2, 1, 3);
            ////合并整页的明细
            //int i;
            //for (i = 0; i < this.pageDetailRowCount; i++)
            //{
            //    this.SetMergedRegionColumn(pageIndex, i, 2, i, 3);
            //}
            this.CopyCell(pageIndex, 45, 0, "A46");
            this.CopyCell(pageIndex, 45, 5, "F46");

        }
    }
}
