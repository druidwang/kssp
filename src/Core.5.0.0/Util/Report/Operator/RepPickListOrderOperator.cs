using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.ORD;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepPickListOrderOperator : RepTemplate1
    {
        public RepPickListOrderOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 20;
            //列数   1起始
            this.columnCount = 10;
            //报表头的行数  1起始
            this.headRowCount = 17;
            //报表尾的行数  1起始
            this.bottomRowCount = 1;
        }

        /**
         * 填充报表
         * 
         * Param list [0]PickList
         *            
         */
        protected override bool FillValuesImpl(String templateFileName, IList<object> list)
        {
            try
            {
                PrintPickListMaster pickListMaster = (PrintPickListMaster)list[0];
                IList<PrintPickListDetail> pickListDetails = pickListMaster.PickListDetails;

                if (pickListMaster == null || pickListDetails == null || pickListDetails.Count == 0)
                {
                    return false;
                }

                //this.barCodeFontName = this.GetBarcodeFontName(0, 7);
                //this.SetRowCellBarCode(0, 0, 7);
                this.CopyPage(pickListDetails.Count);
                //  this.barCodeFontName = this.GetBarcodeFontName(2, 8);
                this.barCodeFontName = this.GetBarcodeFontName(0, 6);
                pickListMaster.OrderNo = pickListDetails.ToList().First().OrderNo;
                pickListMaster.LocationFrom = pickListDetails.First().LocationFrom;
                pickListMaster.LocationFromName = pickListDetails.First().LocationFromName;
                pickListMaster.LocationTo = pickListDetails.First().LocationTo;
                pickListMaster.LocationToName = pickListDetails.First().LocationToName;
                this.FillHead(pickListMaster);


                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;
                int no = 1;
                foreach (PrintPickListDetail pickListDetail in pickListDetails)
                {
                    //序号
                    this.SetRowCell(pageIndex, rowIndex, 0, pickListDetail.Sequence);
                    //
                    //this.SetRowCell(pageIndex, rowIndex, 1, pickListDetail.OrderNo);
                    //物料号
                    this.SetRowCell(pageIndex, rowIndex, 1, pickListDetail.Item);
                    //参考号 Ref No.
                    string itemDescription = pickListDetail.ItemDescription;
                    if (!string.IsNullOrWhiteSpace(pickListDetail.ReferenceItemCode))
                    {
                        itemDescription = itemDescription + "[" + pickListDetail.ReferenceItemCode + "]";
                    }
                    this.SetRowCell(pageIndex, rowIndex, 2, itemDescription);


                    //this.SetRowCell(pageIndex, rowIndex, 3, pickListDetail.ItemDescription);

                    //this.SetRowCell(pageIndex, rowIndex, 4, pickListDetail.ManufactureParty);
                    //单位
                    this.SetRowCell(pageIndex, rowIndex, 3, pickListDetail.Uom);
                    //包装
                    this.SetRowCell(pageIndex, rowIndex, 4, pickListDetail.UnitCount.ToString("0.######"));

                    //this.SetRowCell(pageIndex, rowIndex, 7, pickListDetail.Bin);
                    //库格
                    this.SetRowCell(pageIndex, rowIndex, 5, pickListDetail.Bin);
                    //批号 Follow InspectOrder
                    string lotNo = pickListDetail.LotNo;
                    lotNo = string.IsNullOrWhiteSpace(lotNo) ? "没有库存" : lotNo;
                    this.SetRowCell(pageIndex, rowIndex, 6, lotNo);
                    //拣货数
                    this.SetRowCell(pageIndex, rowIndex, 7, pickListDetail.Qty.ToString("0.######"));
                    //箱数
                    this.SetRowCell(pageIndex, rowIndex, 8, ((int)Math.Ceiling(pickListDetail.Qty / pickListDetail.UnitCount)).ToString());

                    string remark = string.Format("{0} {1}", pickListDetail.Direction, pickListDetail.IsDevan ? "需要翻箱" : string.Empty);

                    //备注
                    this.SetRowCell(pageIndex, rowIndex, 9, remark);
                    //this.SetRowCell(pageIndex, rowIndex, 11, pickListDetail.LocationTo);

                    //if (pickListDetail.IsInventory == false)
                    //{
                    //    this.SetRowCell(pageIndex, rowIndex, 12, "库存不足");
                    //}

                    //if (pickListDetail.IsOdd == true)
                    //{
                    //    this.SetRowCell(pageIndex, rowIndex, 12, "零头");
                    //}

                    //if (pickListDetail.IsDevan == true)
                    //{
                    //    this.SetRowCell(pageIndex, rowIndex, 12, "拆箱");
                    //}

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
         * Param pickList 订单头对象
         */
        private void FillHead(PrintPickListMaster pickListMaster)
        {
            //拣货单号:
            string PickorderCode = Utility.BarcodeHelper.GetBarcodeStr(pickListMaster.PickListNo, this.barCodeFontName);
            this.SetRowCell(0, 6, PickorderCode);
            //Pick No.:
            this.SetRowCell(2, 6, pickListMaster.PickListNo);
            //订单 号
            this.SetRowCell(5, 2, pickListMaster.OrderNo);
            //制单时间
            this.SetRowCell(4, 2, pickListMaster.CreateDate.ToString("yyyy-MM-dd HH:mm"));
            //开始时间
            this.SetRowCell(4, 6, pickListMaster.StartTime.ToString("yyyy-MM-dd HH:mm"));
            //窗口时间
            this.SetRowCell(5, 6, pickListMaster.WindowTime.ToString("yyyy-MM-dd HH:mm"));
            //参考订单号
            //this.SetRowCell(5, 2, pickListMaster.ReferenceOrderNo);

            //*****收货方信息
            //目的(收货)区域名称 Region To:	
            this.SetRowCell(8, 2, string.Format("{0} {1}", pickListMaster.PartyTo, pickListMaster.PartyToName));

            //收货库位
            string LocTo = pickListMaster.LocationTo;
            if (!string.IsNullOrWhiteSpace(pickListMaster.LocationToName))
            {
                LocTo = LocTo + " " + pickListMaster.LocationToName;
            }
            this.SetRowCell(9, 2, LocTo);

            //供应商地址/道口 Address:	
            string AddressDock = string.Format("{0}/{1}", pickListMaster.ShipToAddress, pickListMaster.Dock);
            this.SetRowCell(10, 2, AddressDock);

            //收货方联系人 Contact:	
            this.SetRowCell(8, 6, pickListMaster.ShipToContact);

            //收货方电话 Telephone:		
            this.SetRowCell(9, 6, string.Format("{0} {1}", pickListMaster.ShipToTel, pickListMaster.ShipToCell));

            //收货方传真 Fax:	
            this.SetRowCell(10, 6, pickListMaster.ShipToFax);

            //*****发货方信息
            //供应商代码 Supplier Code:	
            this.SetRowCell(12, 2, string.Format("{0} {1}", pickListMaster.PartyFrom, pickListMaster.PartyFromName));

            //供应商名称 Supplier Name:		
            //this.SetRowCell(6, 1, pickListMaster.PartyFromName);

            //发货库位
            string LocFrom = pickListMaster.LocationFrom;
            if (!string.IsNullOrWhiteSpace(pickListMaster.LocationFromName))
            {
                LocFrom = LocFrom + " " + pickListMaster.LocationFromName;
            }
            this.SetRowCell(13, 2, LocFrom);

            //供应商地址 Address:	
            this.SetRowCell(14, 2, pickListMaster.ShipFromAddress);

            //目的区域地址
            //this.SetRowCell(11, 2, pickListMaster.ShipToAddress);

            //供应商联系人 Contact:	
            this.SetRowCell(12, 6, pickListMaster.ShipFromContact);
            //供应商电话
            this.SetRowCell(13, 6, string.Format("{0} {1}", pickListMaster.ShipFromTel, pickListMaster.ShipFromCell));
            //YFV传真 Fax:
            this.SetRowCell(14, 6, pickListMaster.ShipFromFax);
            //订单号:
            //string orderCode = Utility.BarcodeHelper.GetBarcodeStr(pickListMaster.PickListNo, this.barCodeFontName);
            //this.SetRowCell(2, 8, orderCode);
            //Order No.:
            //this.SetRowCell(3, 8, pickListMaster.PickListNo);

            //if (pickListMaster.PrintPickListDetails == null
            //        || pickListMaster.PrintPickListDetails[0] == null
            //        || pickListMaster.PrintPickListDetails[0]. == null
            //        || pickListMaster.PrintPickListDetails[0].o.OrderDetail == null
            //        || pickListMaster.PrintPickListDetails[0].OrderLocationTransaction.OrderDetail.OrderHead == null
            //        || "Normal".Equals(pickListMaster.PrintPickListDetails[0].OrderLocationTransaction.OrderDetail.OrderHead.Priority)) 
            //{
            //    this.SetRowCell(2, 4, "");
            //}
            //else
            //{
            //    this.SetRowCell(1, 4, "");
            //}

            //源超市：
            //if (pickListMaster != null )//&& pickListMaster.Flow.Trim() != string.Empty)
            //{
            //    //Flow flow = this.flowMgrE.LoadFlow(pickList.Flow);
            //    this.SetRowCell(2, 2, flow.LocationFrom == null ? string.Empty : flow.LocationFrom.Code);
            //    //目的超市：
            //    this.SetRowCell(3, 2, pickList.Flow);
            //    //领料地点：
            //    this.SetRowCell(4, 2, flow.LocationFrom == null ? string.Empty : flow.LocationFrom.Region.Code);
            //}

            //窗口时间
            //    this.SetRowCell(2, 8, pickListMaster.WindowTime.ToString("yyyy-MM-dd HH:mm"));
            //订单时间
            //  this.SetRowCell(4, 8, pickListMaster.CreateDate.ToString("yyyy-MM-dd HH:mm"));
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            //this.SetMergedRegion(pageIndex, 47, 3, 47, 4);
            //this.SetMergedRegion(pageIndex, 47, 7, 47, 8);

            this.CopyCell(pageIndex, 37, 1, "B38");
            this.CopyCell(pageIndex, 37, 6, "G38");

        }

    }
}
