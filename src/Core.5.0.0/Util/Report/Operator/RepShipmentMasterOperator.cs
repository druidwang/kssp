using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.ORD;
using com.Sconit.Entity.ORD;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepShipmentMasterOperator : RepTemplate1
    {

       
        public RepShipmentMasterOperator()
        {

            //明细部分的行数
            this.pageDetailRowCount = 17;
            //列数   1起始
            this.columnCount = 6;
            //报表头的行数  1起始
            this.headRowCount = 10;
            //报表尾的行数  1起始
            this.bottomRowCount = 0;

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

                ShipmentMaster ShipmentMaster = (ShipmentMaster)list[0];
                IList<IpMaster> IpMasterList = (IList<IpMaster>)list[1];

                if (ShipmentMaster == null
                    || IpMasterList == null || IpMasterList.Count == 0)
                {
                    return false;
                }
                //ASN号:
                //List<Transformer> transformerList = Utility.TransformerHelper.ConvertInProcessLocationDetailsToTransformers(ipDetailList);
                this.barCodeFontName = this.GetBarcodeFontName(2, 7);

                this.CopyPage(IpMasterList.Count);

                this.FillHead(ShipmentMaster);

                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;

                foreach (IpMaster ipMaster in IpMasterList)
                {
                    this.SetRowCell(pageIndex, rowIndex, 0, ipMaster.IpNo);

                    //合同号
                    this.SetRowCell(pageIndex, rowIndex, 1, ipMaster.Flow);

                    //零件号
                    this.SetRowCell(pageIndex, rowIndex, 2, ipMaster.PartyFromName);

                    //旧零件号
                    this.SetRowCell(pageIndex, rowIndex, 4, ipMaster.PartyToName);

                  

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

                //this.sheet.DisplayGridlines = false;
                //this.sheet.IsPrintGridlines = false;

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
        protected void FillHead(ShipmentMaster ShipmentMaster)
        {
            //运单号:
            string orderCode = Utility.BarcodeHelper.GetBarcodeStr(ShipmentMaster.ShipmentNo, this.barCodeFontName);
            this.SetRowCell(0, 4, orderCode);
            //ShipmentNo No.:
            this.SetRowCell(1, 4, ShipmentMaster.ShipmentNo);

            //车牌:
            this.SetRowCell(2, 1, ShipmentMaster.VehicleNo);

            //驾驶员 Supplier Code:	
            this.SetRowCell(3, 1, ShipmentMaster.Driver);
            //目的地
            this.SetRowCell(3, 4, ShipmentMaster.AddressTo);


            //承运商:		
            this.SetRowCell(4, 1, ShipmentMaster.Shipper);
            //数量
            this.SetRowCell(4, 4, ShipmentMaster.CaseQty);

            //创建用户:	
            this.SetRowCell(5, 1, ShipmentMaster.CreateUserName);
            //创建时间
            this.SetRowCell(5, 4, ShipmentMaster.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"));

            //供应商联系人 Contact:	
            this.SetRowCell(7, 1, ShipmentMaster.PassPerson);
            //目的库位
            this.SetRowCell(7, 4, ShipmentMaster.PassDate.HasValue? ShipmentMaster.PassDate.Value.ToString("yyyy-MM-dd HH:mm:ss"):"" );

       

        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            ////客户代码
            //this.CopyCell(pageIndex, 50, 2, "C51");
            ////开单人
            //this.CopyCell(pageIndex, 50, 4, "F51");
            ////开单日期
            //this.CopyCell(pageIndex, 50, 9, "J51");
        }

    }
}
