using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.ORD;
using com.Sconit.PrintModel.INP;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepInspectOrderOperator : RepTemplate1
    {
        public RepInspectOrderOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 25;
            //列数   1起始
            this.columnCount = 12;
            //报表头的行数  1起始
            this.headRowCount = 9;
            //报表尾的行数  1起始
            this.bottomRowCount = 1;
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

                PrintInspectMaster inspectMaster = (PrintInspectMaster)list[0];
                IList<PrintInspectDetail> inspectDetailList = (IList<PrintInspectDetail>)list[1];

                if (inspectMaster == null
                    || inspectDetailList == null || inspectDetailList.Count == 0)
                {
                    return false;
                }
                //ASN号:
                //List<Transformer> transformerList = Utility.TransformerHelper.ConvertInProcessLocationDetailsToTransformers(ipDetailList);
                this.barCodeFontName = this.GetBarcodeFontName(0, 8);

                this.CopyPage(inspectDetailList.Count);

                this.FillHead(inspectMaster);

                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;

                var groupDetails = inspectDetailList.GroupBy(p => new
                {
                    p.Item,
                    p.ItemDescription,
                    p.ReferenceItemCode,
                    p.Uom,
                    p.UnitCount,
                    p.LocationFrom,
                    p.LotNo,
                    p.FailCode,
                    p.Note
                }, (k, g) => new PrintInspectDetail
                {
                    Item = k.Item,
                    ItemDescription = k.ItemDescription,
                    ReferenceItemCode = k.ReferenceItemCode,
                    Uom = k.Uom,
                    UnitCount = k.UnitCount,
                    LocationFrom = k.LocationFrom,
                    LotNo = k.LotNo,
                    FailCode = k.FailCode,
                    Note = k.Note,
                    InspectQty = g.Sum(q => q.InspectQty),
                    QualifyQty = g.Sum(q => q.QualifyQty),
                    RejectQty = g.Sum(q => q.RejectQty)
                });

                foreach (PrintInspectDetail inspectDetail in groupDetails)
                {
                    var seq = rowIndex + 1;
                    //序号
                    this.SetRowCell(pageIndex, rowIndex, 0, seq);
                    //物料号
                    this.SetRowCell(pageIndex, rowIndex, 1, inspectDetail.Item);
                    //物料描述[参考物料号]
                    string itemDescription = inspectDetail.ItemDescription;
                    if (!string.IsNullOrWhiteSpace(inspectDetail.ReferenceItemCode))
                    {
                        itemDescription = itemDescription + "[" + inspectDetail.ReferenceItemCode + "]";
                    }
                    this.SetRowCell(pageIndex, rowIndex, 2, itemDescription);
                    //单位
                    this.SetRowCell(pageIndex, rowIndex, 3, inspectDetail.Uom);
                    //包装
                    this.SetRowCell(pageIndex, rowIndex, 4, inspectDetail.UnitCount.ToString("0.###"));
                    //库位
                    this.SetRowCell(pageIndex, rowIndex, 5, inspectDetail.LocationFrom);
                    //批号
                    this.SetRowCell(pageIndex, rowIndex, 6, inspectDetail.LotNo);
                    //this.SetRowCell(pageIndex, rowIndex, 5, inspectDetail.CurrentLocation);

                    //this.SetRowCell(pageIndex, rowIndex, 6, inspectDetail.LotNo);
                    //待检数
                    this.SetRowCell(pageIndex, rowIndex, 7, inspectDetail.InspectQty.ToString("0.###"));
                    //合格数
                    this.SetRowCell(pageIndex, rowIndex, 8, inspectDetail.QualifyQty > 0 ? inspectDetail.QualifyQty.ToString("0.###") : "");
                    //不合格数
                    this.SetRowCell(pageIndex, rowIndex, 9, inspectDetail.RejectQty > 0 ? inspectDetail.RejectQty.ToString("0.###") : "");
                    //失效模式
                    this.SetRowCell(pageIndex, rowIndex, 10, inspectDetail.FailCode);
                    //备注
                    this.SetRowCell(pageIndex, rowIndex, 11, inspectDetail.Note);
                    //this.SetRowCell(pageIndex, rowIndex, 12, inspectDetail.IpDetailSequence.ToString());

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
        protected void FillHead(PrintInspectMaster inspectMaster)
        {

            //报验单号
            string inspectNoCode = Utility.BarcodeHelper.GetBarcodeStr(inspectMaster.InspectNo, this.barCodeFontName);
            this.SetRowCell(0, 8, inspectNoCode);
            this.SetRowCell(2, 8, inspectMaster.InspectNo);
            ////Order No.:
            //string ipNoCode = Utility.BarcodeHelper.GetBarcodeStr(inspectMaster.IpNo, this.barCodeFontName);
            //this.SetRowCell(2, 11, ipNoCode);
            //this.SetRowCell(3, 11, inspectMaster.IpNo);
            //制单时间
            this.SetRowCell(4, 2, inspectMaster.CreateDate.ToString("yyyy-MM-dd HH:mm"));
            //报验人(制单人)
            this.SetRowCell(4, 8, inspectMaster.CreateUserName);
            //送货单号???
            this.SetRowCell(5, 2, inspectMaster.Region);
            //收货单号
            this.SetRowCell(5, 8, inspectMaster.ReceiptNo);
            //区域
          //  this.SetRowCell(6, 2, inspectMaster.Region);
            //this.SetRowCell(5, 6, inspectMaster.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            //检验员
            this.CopyCell(pageIndex, 34, 1, "B35");
            //检验时间
            this.CopyCell(pageIndex, 34, 9, "J35");
        }
    }
}
