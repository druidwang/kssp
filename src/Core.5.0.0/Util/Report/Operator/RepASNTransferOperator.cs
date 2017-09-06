using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.ORD;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepASNTransferOperator : RepTemplate1
    {
        public RepASNTransferOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 20;
            //列数   1起始
            this.columnCount = 13;
            //报表头的行数  1起始
            this.headRowCount = 15;
            //报表尾的行数  1起始
            this.bottomRowCount = 2;
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
                this.barCodeFontName = this.GetBarcodeFontName(0, 6);
                this.CopyPage(ipDetailList.Count);

                ipMaster.LocationFrom = ipDetailList.First().LocationFrom;
                ipMaster.LocationFromName = ipDetailList.First().LocationFromName;
                ipMaster.LocationTo = ipDetailList.First().LocationTo;
                ipMaster.LocationToName = ipDetailList.First().LocationToName;
                ipMaster.WindowTime = (DateTime)ipDetailList.First().WindowTime;
                this.FillHead(ipMaster);

                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;

                foreach (PrintIpDetail ipDetail in ipDetailList)
                {
                    // this.SetRowCell(pageIndex, rowIndex, 0, ipDetail.WindowTime.HasValue? ipDetail.WindowTime.Value.ToString("yyyy-MM-dd"):string.Empty);

                    //合同号
                    this.SetRowCell(pageIndex, rowIndex, 0, ipDetail.OrderNo);

                    //序号
                    this.SetRowCell(pageIndex, rowIndex, 1, ipDetail.Sequence);

                    //零件号
                    this.SetRowCell(pageIndex, rowIndex, 2, ipDetail.Item);

                    //旧零件号
                    // this.SetRowCell(pageIndex, rowIndex, 3, ipDetail.ReferenceItemCode);
                    string itemDescription = ipDetail.ItemDescription;
                    if (!string.IsNullOrWhiteSpace(ipDetail.ReferenceItemCode))
                    {
                        itemDescription = itemDescription + "[" + ipDetail.ReferenceItemCode + "]";
                    }
                    this.SetRowCell(pageIndex, rowIndex, 3, itemDescription);
                    //单位
                    this.SetRowCell(pageIndex, rowIndex, 5, ipDetail.Uom);

                    //单包装
                    this.SetRowCell(pageIndex, rowIndex, 6, ipDetail.UnitCount.ToString("0.########"));

                    //订单数
                    this.SetRowCell(pageIndex, rowIndex, 7, ipDetail.OrderQty.ToString("0.########"));
                    /* //Mark these codes ,no need to transform the variable because we can get corresponding filed fron view VIEW_IpDet//实送数量
                     this.SetRowCell(pageIndex, rowIndex, 6, (ipDetail.Qty /ipDetail.UnitCount).ToString("0.########"));

                     //实送数量
                     this.SetRowCell(pageIndex, rowIndex, 7, ipDetail.Qty.ToString("0.########"));

                     //是否报验
                     if (ipDetail.IsInspect == true)
                     {
                         this.SetRowCell(pageIndex, rowIndex, 8, "√");
                     }*/

                    //是否寄售
                    //if (ipDetail.BillTerm == (Int32)CodeMaster.OrderBillTerm.ReceivingSettlement)
                    //{
                    //    this.SetRowCell(pageIndex, rowIndex, 9, "√");
                    //}

                    //if (ipDetail.Tax.ToUpper() == "ZCK")
                    //{
                    //    this.SetRowCell(pageIndex, rowIndex, 11, "出");
                    //}
                    //else if (ipDetail.Tax.ToUpper() == "ZSP")
                    //{
                    //    this.SetRowCell(pageIndex, rowIndex, 11, "军");
                    //}
                    //if (this.isPageBottom(rowIndex, rowTotal))//页的最后一行
                    //{
                    //    //客户代码
                    //    this.SetRowCell(pageIndex, this.pageDetailRowCount + 4, 10, ipMaster.PartyTo == null ? string.Empty : ipMaster.PartyTo);
                    //    //开单人
                    //    this.SetRowCell(pageIndex, this.pageDetailRowCount + 6, 1, ipMaster.CreateUserName);
                    //    //开单日期
                    //    this.SetRowCell(pageIndex, this.pageDetailRowCount + 6, 3, ipMaster.CreateDate.ToString("yyyy    MM    dd"));

                    //    this.sheet.SetRowBreak(this.GetRowIndexAbsolute(pageIndex, this.pageDetailRowCount + this.bottomRowCount - 1));
                    //发货数  Qty就是ShippedQty
                    if (ipDetail.Qty == 0)
                    {
                        this.SetRowCell(pageIndex, rowIndex, 8, "");
                    }
                    else
                    {
                        this.SetRowCell(pageIndex, rowIndex, 8, ipDetail.Qty.ToString("0.####"));
                    }
                    //箱数
                    if (ipDetail.BoxQty == 0)
                    {
                        this.SetRowCell(pageIndex, rowIndex, 9, "");
                    }
                    else
                    {
                        this.SetRowCell(pageIndex, rowIndex, 9, ipDetail.BoxQty.ToString("0.####"));
                    }
                    //实收 Received	零件数
                    if (ipDetail.ReceivedQty == 0)
                    {
                        this.SetRowCell(pageIndex, rowIndex, 10, "");
                    }
                    else
                    {
                        this.SetRowCell(pageIndex, rowIndex, 10, ipDetail.ReceivedQty.ToString("0.####"));
                    }
                    //差异数
                    if (ipDetail.GapQty == 0)
                    {
                        this.SetRowCell(pageIndex, rowIndex, 11, "");
                    }
                    else
                    {
                        this.SetRowCell(pageIndex, rowIndex, 11, ipDetail.GapQty.ToString("0.####"));
                    }

                    // 批号/备注
                    this.SetRowCell(pageIndex, rowIndex, 12, ipDetail.Remark); 
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
        protected void FillHead(PrintIpMaster ipMaster)
        {
            if (ipMaster.OrderSubType == 1)
            {
                this.SetRowCell(2, 5, "退货");
            }
            //订单号:
            string orderCode = Utility.BarcodeHelper.GetBarcodeStr(ipMaster.IpNo, this.barCodeFontName);
            this.SetRowCell(0, 6, orderCode);
            //Order No.:
            this.SetRowCell(2, 6, ipMaster.IpNo);

            //制单时间 Create Time:
            this.SetRowCell(4, 2, ipMaster.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"));

            //窗口时间 Window Time:
            this.SetRowCell(4, 7, ipMaster.WindowTime.ToString("yyyy-MM-dd HH:mm:ss"));

            //*****收货方信息
            //目的(收货)区域名称 Region To:	
            this.SetRowCell(6, 1, string.Format("{0} {1}", ipMaster.PartyTo, ipMaster.PartyToName));

            //收货库位
            string LocTo = ipMaster.LocationTo;
            if (!string.IsNullOrWhiteSpace(ipMaster.LocationToName))
            {
                LocTo = LocTo + " " + ipMaster.LocationToName;
            }
            this.SetRowCell(7, 1, LocTo);

            //供应商地址/道口 Address:	
            string AddressDock = string.Format("{0}/{1}", ipMaster.ShipToAddress, ipMaster.Dock);
            this.SetRowCell(8, 1, AddressDock);

            //收货方联系人 Contact:	
            this.SetRowCell(6, 7, ipMaster.ShipToContact);

            //收货方电话 Telephone:		
            this.SetRowCell(7, 7, string.Format("{0} {1}", ipMaster.ShipToTel, ipMaster.ShipToCell));

            //收货方传真 Fax:	
            this.SetRowCell(8, 7, ipMaster.ShipToFax);

            //*****发货方信息
            //供应商代码 Supplier Code:	
            //this.SetRowCell(10, 1, ipMaster.PartyFrom);

            //供应商名称 Supplier Name:		
            this.SetRowCell(10, 1, string.Format("{0} {1}", ipMaster.PartyFrom, ipMaster.PartyFromName));

            //发货库位
            string LocFrom = ipMaster.LocationFrom;
            if (!string.IsNullOrWhiteSpace(ipMaster.LocationFromName))
            {
                LocFrom = LocFrom + " " + ipMaster.LocationFromName;
            }
            this.SetRowCell(11, 1, LocFrom);

            //供应商地址 Address:	
            this.SetRowCell(12, 1, ipMaster.ShipFromAddress);

            //目的区域地址
            //this.SetRowCell(11, 2, ipMaster.ShipToAddress);

            //供应商联系人 Contact:	
            this.SetRowCell(10, 7, ipMaster.ShipFromContact);
            //供应商电话
            this.SetRowCell(11, 7, string.Format("{0} {1}", ipMaster.ShipFromTel, ipMaster.ShipFromCell));
            //YFV传真 Fax:
            this.SetRowCell(12, 7, ipMaster.ShipFromFax);

            //this.SetRowCell(9, 2, LocTo);
 
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            //合并整页的明细
            int i;
            for (i = 0; i < this.pageDetailRowCount; i++)
            {
                this.SetMergedRegionColumn(pageIndex, i, 3, i, 4);
            }
            //Supplier sinature
            this.CopyCell(pageIndex, 35, 1, "B36");
            //Supplier sinature date time
            this.CopyCell(pageIndex, 35, 6, "G36");
            //receiver signature
            this.CopyCell(pageIndex, 36, 1, "B37");
            //receiver signature date time
            this.CopyCell(pageIndex, 36, 6, "G37");
 
        }

    }
}
