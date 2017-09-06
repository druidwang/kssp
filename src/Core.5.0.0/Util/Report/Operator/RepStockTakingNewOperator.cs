using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.PrintModel.INV;
using com.Sconit.Entity.INV;

namespace com.Sconit.Utility.Report.Operator
{
    public class RepStockTakingNewOperator : RepTemplate1
    {
        public RepStockTakingNewOperator()
        {
            //明细部分的行数
            this.pageDetailRowCount = 35;
            //列数   1起始
            this.columnCount = 7;
            //报表头的行数  1起始
            this.headRowCount = 11;
            //报表尾的行数  1起始
            this.bottomRowCount = 1;
        }

        /**
         * 填充报表
         * 
         *
         */
        protected override bool FillValuesImpl(String templateFileName, IList<object> list)
        {
            try
            {
                if (list == null || list.Count < 2) return false;

                PrintStockTakeMaster printStockTakeMaster = (PrintStockTakeMaster)list[0];
                IList<StockTakeLocation> printStockTakeLocation = (IList<StockTakeLocation>)list[1];
                IList<StockTakeItem> printStockTakeItemList = (List<StockTakeItem>)list[2];
                //IList<StockTakeInv> printStockTakeInvList = (List<StockTakeInv>)list[3];

                this.FillHead(printStockTakeMaster, printStockTakeLocation);

                var bins = printStockTakeLocation.Where(p => !string.IsNullOrWhiteSpace(p.Bins))
                   .SelectMany(p => p.Bins.Split(',')).ToList();
                if (bins.Count > 0)
                {
                    int pageIndex = 1;
                    int rowIndex = 0;
                    int rowTotal = 0;
                    int seq = 1;
                    foreach (var bin in bins)
                    {
                        //序号	物料代码	描述	单位	库位	数量	备注
                        this.SetRowCell(pageIndex, rowIndex, 6, bin);
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
                        seq++;
                    }
                }

                bool filllocdata = printStockTakeLocation.Count == 1 ? true : false;
                string location = filllocdata ? printStockTakeLocation.FirstOrDefault().Location : string.Empty;

                //如果是全盘则不显示明细
                if (printStockTakeMaster.Type == 0 /*&& !printStockTakeMaster.IsScanHu*/)
                {
                    int pageIndex = 1;
                    int rowIndex = 0;
                    int rowTotal = 0;
                    int seq = 1;
                    foreach (StockTakeItem printStockTakeItem in printStockTakeItemList)
                    {
                        //序号	物料代码	描述	单位	库位	数量	备注
                        //序号
                        //this.SetRowCell(pageIndex, rowIndex, 0, seq);
                        //物料代码
                        this.SetRowCell(pageIndex, rowIndex, 1, printStockTakeItem.Item);
                        //描述
                        this.SetRowCell(pageIndex, rowIndex, 2, printStockTakeItem.ItemDescription);
                        //库位
                        if (filllocdata)
                        {
                            this.SetRowCell(pageIndex, rowIndex, 3, location);
                        }
                        //数量
                        //var stockTakeInv = printStockTakeInvList.FirstOrDefault(p => p.Item == printStockTakeItem.Item && p.Location == location);
                        //this.SetRowCell(pageIndex, rowIndex, 4, stockTakeInv == null ? "0" : stockTakeInv.Qty.ToString());
                        //单位
                        //this.SetRowCell(pageIndex, rowIndex, 5, printStockTakeItem.Uom);
                        if (this.isPageBottom(rowIndex, rowTotal))//页的最后一行
                        {
                            //实际到货时间:
                            //this.SetRowCell(pageIndex, rowIndex, , "");

                            pageIndex++;
                            rowIndex = 0;
                        }
                        else
                        {
                            rowIndex++;
                        }
                        rowTotal++;
                        seq++;
                    }
                }
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
        private void FillHead(PrintStockTakeMaster printStockTakeMaster, IList<StockTakeLocation> printStockTakeLocation)
        {
            printStockTakeLocation = printStockTakeLocation.Take(6).ToList();
            this.sheet.DisplayGridlines = false;
            this.sheet.IsPrintGridlines = false;

            this.barCodeFontName = this.GetBarcodeFontName(0, 5);
            int pageIndex = 1;

            string barCode = Utility.BarcodeHelper.GetBarcodeStr(printStockTakeMaster.StNo, this.barCodeFontName);
            //条形码
            this.SetRowCell(0, 5, barCode);
            //StNo
            this.SetRowCell(2, 5, printStockTakeMaster.StNo);
            //区域
            this.SetRowCell(3, 1, printStockTakeMaster.Region);
            //类型
            this.SetRowCell(4, 1, printStockTakeMaster.Type == 0 ? "抽盘" : "全盘");
            //生效日期
            this.SetRowCell(3, 5, printStockTakeMaster.EffectiveDate.HasValue ? printStockTakeMaster.EffectiveDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty);
            //是否扫条码
            this.SetRowCell(4, 5, printStockTakeMaster.IsScanHu ? "条码盘点" : "数量盘点");
            //创建日期
            this.SetRowCell(5, 5, printStockTakeMaster.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"));
            //创建人
            this.SetRowCell(5, 1, printStockTakeMaster.CreateUserName);
            int l = 1;
            int rowadd = 1;
            foreach (StockTakeLocation StockTakeLocation in printStockTakeLocation)
            {
                if (l % 2 == 1)
                {
                    this.SetRowCell(5 + rowadd, 1, StockTakeLocation.Location + (string.IsNullOrWhiteSpace(StockTakeLocation.LocationName) ? "" : "[" + StockTakeLocation.LocationName + "]"));
                }
                else
                {
                    this.SetRowCell(5 + rowadd, 4, StockTakeLocation.Location + (string.IsNullOrWhiteSpace(StockTakeLocation.LocationName) ? "" : "[" + StockTakeLocation.LocationName + "]"));
                    rowadd++;
                }
                l++;
            }
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            ////合并整页的明细
            //int i;
            //for (i = 0; i < this.pageDetailRowCount; i++)
            //{
            //    this.SetMergedRegionColumn(pageIndex, i, 3, i, 4);
            //}
            //盘点人:
            this.CopyCell(pageIndex, 46, 0, "A47");
            //完成时间:
            this.CopyCell(pageIndex, 46, 3, "D47");
        }
    }
}
