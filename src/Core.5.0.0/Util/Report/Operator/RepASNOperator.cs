using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.ORD;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepASNOperator : RepTemplate1
    {
        public RepASNOperator()
        {

            //明细部分的行数
            this.pageDetailRowCount = 12;
            //列数   1起始
            this.columnCount = 17;
            //报表头的行数  1起始
            this.headRowCount = 9;
            //报表尾的行数  1起始
            this.bottomRowCount = 7;

        }

        /**
         * 填充报表
         * 
         * Param list [0]InProcessLocation
         *            [1]inProcessLocationDetailList
         */
        protected override bool FillValuesImpl(String templateFileName, IList<object> list)
        {
            try
            {

                if (list == null || list.Count < 2) return false;

                PrintIpMaster ipMaster = (PrintIpMaster)list[0];
                IList<PrintIpDetail> ipDetailList = (IList<PrintIpDetail>)list[1];

                if (ipMaster == null
                    || ipDetailList == null || ipDetailList.Count == 0)
                {
                    return false;
                }
                //ASN号:
                //List<Transformer> transformerList = Utility.TransformerHelper.ConvertInProcessLocationDetailsToTransformers(ipDetailList);

                this.CopyPage(ipDetailList.Count);

                this.FillHead(ipMaster);

                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;

                foreach (PrintIpDetail ipDetail in ipDetailList)
                {
                    //零件号
                    this.SetRowCell(pageIndex, rowIndex, 1, ipDetail.Item);
                    //客户零件号
                    //IList<ItemReference> referenceItemList = itemReferenceMgrE.GetItemReference(transformer.ItemCode);
                    //if (referenceItemList != null && referenceItemList.Count > 0)
                    //{
                    //    this.SetRowCell(pageIndex, rowIndex, 2, referenceItemList[0].ReferenceCode);
                    //}
                    this.SetRowCell(pageIndex, rowIndex, 2, ipDetail.ReferenceItemCode);

                    //零件名称
                    this.SetRowCell(pageIndex, rowIndex, 3, ipDetail.ItemDescription);
                    //库位
                    this.SetRowCell(pageIndex, rowIndex, 4, ipDetail.LocationFrom);
                    //包装箱 类型
                    //this.SetRowCell(pageIndex, rowIndex, 5, transformer.Cartons.ToString("0.########"));
                    //长
                    //宽
                    //高
                    //单位
                    this.SetRowCell(pageIndex, rowIndex, 9, ipDetail.Uom);
                    //实送数量
                    this.SetRowCell(pageIndex, rowIndex, 10, ipDetail.Qty.ToString("0.########"));
                    //实收数量
                    //装箱数(transformer.Qty / transformer.UnitCount).ToString("0.########")
                    this.SetRowCell(pageIndex, rowIndex, 12, (ipDetail.Qty /ipDetail.UnitCount).ToString("0.########"));
                    //件箱数
                    //单价
                    //总金额
                    //备注

                    if (this.isPageBottom(rowIndex, rowTotal))//页的最后一行
                    {
                        //承运经办人
                        //发货人
                        //收货经办人
                        //送货日期
                        //送货日期
                        //出门日期
                        //执勤门卫
                        //客户代码
                        this.SetRowCell(pageIndex, this.pageDetailRowCount + 4, 10, ipMaster.PartyTo == null ? string.Empty : ipMaster.PartyTo);
                        //开单人
                        this.SetRowCell(pageIndex, this.pageDetailRowCount + 6, 1, ipMaster.CreateUserName);
                        //开单日期
                        this.SetRowCell(pageIndex, this.pageDetailRowCount + 6, 3, ipMaster.CreateDate.ToString("yyyy    MM    dd"));

                        this.sheet.SetRowBreak(this.GetRowIndexAbsolute(pageIndex, this.pageDetailRowCount + this.bottomRowCount - 1));

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
        protected void FillHead(PrintIpMaster ipMaster)
        {

            //首部
            //No.
            this.SetRowCell(3, 14, ipMaster.IpNo);
            //收货单位
            this.SetRowCell(5, 2, ipMaster.PartyToName);
            //地址
            this.SetRowCell(5, 9, ipMaster.ShipTo == null ? string.Empty : ipMaster.ShipToAddress);

            //卸货地点
            //运输方法
            //车号
            //介绍信编号
            //货物出门性质
            //CodeMaster codeMaster = codeMasterMgrE.LoadCodeMaster(BusinessConstants.CODE_MASTER_ORDER_TYPE, ipMaster.OrderType);
            //this.SetRowCell(6, 16, codeMaster.Description);

        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            //客户代码
            this.CopyCell(pageIndex, 25, 10, "K26");
            //开单人
            this.CopyCell(pageIndex, 27, 1, "B28");
            //开单日期
            this.CopyCell(pageIndex, 27, 3, "D28");
        }

    }
}
