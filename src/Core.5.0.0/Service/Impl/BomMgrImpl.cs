using System;
using System.Collections.Generic;
using System.Linq;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.PRD;
using com.Sconit.Entity.SCM;
using Castle.Services.Transaction;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections;
using com.Sconit.Utility;
using com.Sconit.Entity;

namespace com.Sconit.Service.Impl
{
    public class BomMgrImpl : BaseMgr, IBomMgr
    {
        #region 变量
        public IQueryMgr queryMgr { get; set; }
        public IItemMgr itemMgr { get; set; }
        public IFlowMgr flowMgr { get; set; }
        public IGenericMgr genericMgr { get; set; }
        #endregion

        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.Import");
        #region 缓存

        private static Dictionary<string, List<BomDetail>> cachedAllBomDetail;
        private static DateTime cacheDateTime;
        private static Dictionary<string, BomMaster> cachedAllBomMaster;

        #endregion

        #region public methods
        private static object bomDetailLock = new object();
        public Dictionary<string, List<BomDetail>> GetCacheAllBomDetail()
        {
            lock(bomDetailLock)
            {
                if(cachedAllBomDetail == null || cacheDateTime < DateTime.Now.AddMinutes(-10))
                {
                    cacheDateTime = DateTime.Now;

                    var allBomDetails = this.genericMgr.FindAll<BomDetail>();
                    var allItems = this.itemMgr.GetCacheAllItem();

                    foreach(var bomDetail in allBomDetails)
                    {
                        bomDetail.CurrentItem = allItems[bomDetail.Item];
                    }

                    cachedAllBomDetail = (from p in allBomDetails
                                          group p by p.Bom into g
                                          select new
                                          {
                                              Bom = g.Key,
                                              List = g
                                          }).ToDictionary(c => c.Bom, c => c.List.ToList());
                }
                return cachedAllBomDetail;
            }
        }

        private static object bomMasterLock = new object();
        public Dictionary<string, BomMaster> GetCacheAllBomMaster()
        {
            lock(bomMasterLock)
            {
                if(cachedAllBomMaster == null || cacheDateTime < DateTime.Now.AddMinutes(-60 * 24))
                {
                    cacheDateTime = DateTime.Now;
                    var allItem = this.genericMgr.FindAll<BomMaster>();
                    cachedAllBomMaster = allItem.ToDictionary(d => d.Code, d => d);
                }
                return cachedAllBomMaster;
            }
        }

        public void ResetBomCache()
        {
            cacheDateTime = DateTime.MinValue;
        }

        public IList<BomDetail> GetCacheBomDetails(string bomCode)
        {
            return GetCacheAllBomDetail().ValueOrDefault(bomCode);
        }

        public BomMaster GetCacheBomMaster(string bomCode)
        {
            BomMaster bomMaster = null;
            var bomMasterDic = GetCacheAllBomMaster();
            bomMasterDic.TryGetValue(bomCode, out bomMaster);
            return bomMaster;
        }

        public IList<BomDetail> GetOnlyNextLevelBomDetail(string bomCode, DateTime? effectiveDate, bool isMrp = true)
        {
            if(effectiveDate == null)
            {
                effectiveDate = DateTime.Now;
            }

            IList<BomDetail> bomDetailList = (GetCacheBomDetails(bomCode) ?? new List<BomDetail>())
            .Where(p => p.StartDate <= effectiveDate && (!p.EndDate.HasValue || p.EndDate >= effectiveDate)).ToList();

            if(isMrp)
            {
                bomDetailList = bomDetailList.Where(p => p.BomMrpOption == CodeMaster.BomMrpOption.All
                    || p.BomMrpOption == CodeMaster.BomMrpOption.MrpOnly).ToList();
            }
            else
            {
                bomDetailList = bomDetailList.Where(p => p.BomMrpOption == CodeMaster.BomMrpOption.All
                    || p.BomMrpOption == CodeMaster.BomMrpOption.ProductionOnly).ToList();
            }

            this.GetNoOverloadBomDetail(bomDetailList);

            var bom = GetCacheBomMaster(bomCode);
            foreach(var bomDetail in bomDetailList)
            {
                bomDetail.UnitBomQty = bomDetail.RateQty / bom.Qty;
                bomDetail.CalculatedQty = bomDetail.UnitBomQty * (1 + bomDetail.ScrapPercentage);
            }
            return bomDetailList;
        }

        public IList<BomDetail> GetFlatBomDetail(string bomCode, DateTime? effectiveDate, bool isMrp = false)
        {
            var bom = GetCacheBomMaster(bomCode);

            return GetFlatBomDetail(bom, effectiveDate, isMrp);
        }

        public IList<BomDetail> GetFlatBomDetail(BomMaster bom, DateTime? effectiveDate, bool isMrp = false)
        {
            if(effectiveDate == null)
            {
                effectiveDate = DateTime.Now;
            }

            IList<BomDetail> flatBomDetailList = new List<BomDetail>();
            IList<BomDetail> nextBomDetailList = this.GetNextLevelBomDetail(bom.Code, effectiveDate, isMrp);

            foreach(BomDetail nextBomDetail in nextBomDetailList)
            {
                nextBomDetail.UnitBomQty = nextBomDetail.RateQty / bom.Qty;
                nextBomDetail.CalculatedQty = nextBomDetail.UnitBomQty * (1 + nextBomDetail.ScrapPercentage);
                ProcessCurrentBomDetailStructrue(flatBomDetailList, nextBomDetail, effectiveDate.Value, isMrp);
            }
            return flatBomDetailList;
        }

        public string FindItemBom(Item item)
        {
            //默认用Item上的BomCode，如果Item上面没有设置Bom，直接用ItemCode作为BomCode去找
            return (!string.IsNullOrWhiteSpace(item.Bom) ? item.Bom : item.Code);
        }

        public string FindItemBom(string itemCode)
        {
            Item item = this.itemMgr.GetCacheItem(itemCode);//this.genericMgr.FindById<Item>(itemCode);
            return FindItemBom(item);
        }

        public IList<BomDetail> GetProductLineWeightAverageBomDetail(string flow)
        {
            IList<FlowDetail> flowDetailList = this.flowMgr.GetFlowDetailList(flow);

            if(flowDetailList != null && flowDetailList.Count > 0)
            {
                DateTime dateTimeNow = DateTime.Now;
                FlowMaster flowMaster = this.genericMgr.FindById<FlowMaster>(flow);

                IList<BomDetail> batchFeedBomDetailList = new List<BomDetail>();
                foreach(FlowDetail flowDetail in flowDetailList)
                {
                    //先获取flowdetail上的bom，如果为null再以flowdetail的Item对象去找
                    string bomCode = flowDetail.Bom != null ? flowDetail.Bom : FindItemBom(flowDetail.Item);
                    IList<BomDetail> bomDetailList = GetFlatBomDetail(bomCode, dateTimeNow);

                    if(bomDetailList != null && bomDetailList.Count > 0)
                    {
                        foreach(BomDetail bomDetail in bomDetailList)
                        {
                            if(bomDetail.BackFlushMethod == com.Sconit.CodeMaster.BackFlushMethod.WeightAverage)
                            {
                                bomDetail.FeedLocation = !string.IsNullOrWhiteSpace(flowDetail.LocationFrom) ? flowDetail.LocationFrom : flowMaster.LocationFrom;
                                batchFeedBomDetailList.Add(bomDetail);
                            }
                        }
                    }
                }

                return batchFeedBomDetailList;
            }
            return null;
        }
        #endregion

        #region private methods
        private IList<BomDetail> GetNoOverloadBomDetail(IList<BomDetail> bomDetailList)
        {
            //过滤BomCode，ItemCode，Operation，Reference相同的BomDetail，只取StartDate最大的。
            var groupedDetList = from det in bomDetailList
                                 group det by new
                                 {
                                     Bom = det.Bom,
                                     Item = det.Item,
                                     Ref = det.OpReference,
                                     Op = det.Operation
                                 } into Result
                                 select new
                                 {
                                     Bom = Result.Key.Bom,
                                     Item = Result.Key.Item,
                                     Ref = Result.Key.Ref,
                                     Op = Result.Key.Op,
                                     StartDate = Result.Max(det => det.StartDate),
                                     MaxId = Result.Max(det => det.Id)
                                 };

            IList<BomDetail> noOverloadDetailList = (from det in bomDetailList
                                                     join groupedDet in groupedDetList
                                                     on new
                                                     {
                                                         Bom = det.Bom,
                                                         Item = det.Item,
                                                         Ref = det.OpReference,
                                                         Op = det.Operation,
                                                         StartDate = det.StartDate
                                                     }
                                                     equals new
                                                     {
                                                         Bom = groupedDet.Bom,
                                                         Item = groupedDet.Item,
                                                         Ref = groupedDet.Ref,
                                                         Op = groupedDet.Op,
                                                         StartDate = groupedDet.StartDate
                                                     }
                                                     select det).ToList();

            #region 检查Bom + Item + Op + Ref + StartDate是否重复
            #endregion

            return noOverloadDetailList;

            #region 旧算法
            /*
            IList<BomDetail> noOverloadBomDetailList = new List<BomDetail>();
            foreach (BomDetail bomDetail in bomDetailList)
            {
                int overloadIndex = -1;
                for (int i = 0; i < noOverloadBomDetailList.Count; i++)
                {
                    //判断BomCode，ItemCode，Operation，Reference是否相同
                    if (noOverloadBomDetailList[i].Bom == bomDetail.Bom
                        && noOverloadBomDetailList[i].Item == bomDetail.Item
                        && noOverloadBomDetailList[i].Operation == bomDetail.Operation
                        && noOverloadBomDetailList[i].Reference == bomDetail.Reference)
                    {
                        //存在相同的，记录位置。
                        overloadIndex = i;
                        break;
                    }
                }

                if (overloadIndex == -1)
                {
                    //没有相同的记录，直接把BomDetail加入返回列表
                    noOverloadBomDetailList.Add(bomDetail);
                }
                else
                {
                    //有相同的记录，判断bomDetail.StartDate和结果集中的大。
                    if (noOverloadBomDetailList[overloadIndex].StartDate < bomDetail.StartDate)
                    {
                        //bomDetail.StartDate大于结果集中的，替换结果集
                        noOverloadBomDetailList[overloadIndex] = bomDetail;
                    }
                }
            }
            return noOverloadBomDetailList;
                */
            #endregion
        }

        private void ProcessCurrentBomDetailStructrue(IList<BomDetail> flatBomDetailList, BomDetail currentBomDetail, DateTime efftiveDate, bool isMrp)
        {
            if(currentBomDetail.StructureType == com.Sconit.CodeMaster.BomStructureType.Normal) //普通结构
            {
                ProcessCurrentBomDetailItem(flatBomDetailList, currentBomDetail, efftiveDate, isMrp);
            }
            else if(currentBomDetail.StructureType == com.Sconit.CodeMaster.BomStructureType.Virtual) //虚结构
            {
                //如果是虚结构(X)，不把自己加到返回表里，继续向下分解
                NestingGetNextLevelBomDetail(flatBomDetailList, currentBomDetail, efftiveDate, isMrp);
            }
        }

        private void ProcessCurrentBomDetailItem(IList<BomDetail> flatBomDetailList, BomDetail currentBomDetail, DateTime efftiveDate, bool isMrp)
        {
            TryLoadBomItem(currentBomDetail);
            if(currentBomDetail.CurrentItem.IsVirtual)
            {
                //如果是虚零件(X)，继续向下分解
                NestingGetNextLevelBomDetail(flatBomDetailList, currentBomDetail, efftiveDate, isMrp);
            }
            else if(currentBomDetail.CurrentItem.IsKit)
            {
                //组件，先拆分组件再继续向下分解
                //考虑组件的比例
                IList<ItemKit> itemKitList = itemMgr.GetKitItemChildren(currentBomDetail.Item);

                if(itemKitList != null && itemKitList.Count() > 0)
                {
                    foreach(ItemKit itemKit in itemKitList)
                    {
                        NestingGetNextLevelBomDetail(flatBomDetailList, itemKit.ChildItem.Code, currentBomDetail.Uom, (currentBomDetail.UnitBomQty * itemKit.Qty), (currentBomDetail.CalculatedQty * itemKit.Qty), efftiveDate, isMrp);
                    }
                }
                else
                {
                    throw new BusinessException("Errors.ItemKit.ChildrenItemNotFound", currentBomDetail.Item);
                }
            }
            else
            {
                //thinking:是否需要考虑某种零件不能作为BomDetail.Item

                //直接加入到flatBomDetailList
                flatBomDetailList.Add(currentBomDetail);
            }
        }

        private void NestingGetNextLevelBomDetail(IList<BomDetail> flatBomDetailList, BomDetail currentBomDetail, DateTime efftiveDate, bool isMrp)
        {
            NestingGetNextLevelBomDetail(flatBomDetailList, currentBomDetail.Item, currentBomDetail.Uom, currentBomDetail.UnitBomQty, currentBomDetail.CalculatedQty, efftiveDate, isMrp);
        }

        private void NestingGetNextLevelBomDetail(IList<BomDetail> flatBomDetailList, string currentBomItem, string currentBomItemUom, decimal unitQty, decimal calculatedQty, DateTime efftiveDate, bool isMrp)
        {
            string nextLevelBomCode = this.FindItemBom(currentBomItem);
            IList<BomDetail> nextBomDetailList = this.GetNextLevelBomDetail(nextLevelBomCode, efftiveDate, isMrp);

            foreach(BomDetail nextBomDetail in nextBomDetailList)
            {
                //当前子件的Uom和下层Bom的Uom不匹配，需要做单位转换
                BomMaster nextLevelBom = this.GetCacheBomMaster(nextBomDetail.Bom);//queryMgr.FindById<BomMaster>(nextBomDetail.Bom);

                //单位换算
                decimal rateQty = itemMgr.ConvertItemUomQty(currentBomItem, currentBomItemUom, nextBomDetail.RateQty, nextLevelBom.Uom) / nextLevelBom.Qty;
                nextBomDetail.UnitBomQty = rateQty * unitQty;
                nextBomDetail.CalculatedQty = rateQty * (1 + nextBomDetail.ScrapPercentage) * calculatedQty;

                ProcessCurrentBomDetailStructrue(flatBomDetailList, nextBomDetail, efftiveDate, isMrp);
            }
        }

        private void TryLoadBomItem(BomDetail bomDetail)
        {
            if(bomDetail.CurrentItem == null)
            {
                bomDetail.CurrentItem = this.itemMgr.GetCacheItem(bomDetail.Item);//this.genericMgr.FindById<Item>(bomDetail.Item);
            }
        }

        private IList<BomDetail> GetNextLevelBomDetail(string bomCode, DateTime? effectiveDate, bool isMrp)
        {
            if(effectiveDate == null)
            {
                effectiveDate = DateTime.Now;
            }

            //string hql = @"select bd from BomDetail as bd,Item as i where bd.Item = i.Code
            //            and bd.Bom = ? and i.IsActive = ? and bd.StartDate <= ? 
            //                    and (bd.EndDate is null or bd.EndDate >= ?)";

            //IList<BomDetail> bomDetailList = queryMgr.FindAll<BomDetail>(hql, new object[] { bomCode, true, effectiveDate, effectiveDate });

            var bomDetails = (GetCacheBomDetails(bomCode) ?? new List<BomDetail>())
            .Where(p => p.StartDate <= effectiveDate && (!p.EndDate.HasValue || p.EndDate >= effectiveDate));
            if(isMrp)
            {
                bomDetails = bomDetails.Where(p => p.BomMrpOption == CodeMaster.BomMrpOption.All
                    || p.BomMrpOption == CodeMaster.BomMrpOption.MrpOnly);
            }
            else
            {
                bomDetails = bomDetails.Where(p => p.BomMrpOption == CodeMaster.BomMrpOption.All
                    || p.BomMrpOption == CodeMaster.BomMrpOption.ProductionOnly);
            }
            //没有bom明细不报错
            return this.GetNoOverloadBomDetail(bomDetails.ToList());
            //if (bomDetails != null && bomDetails.Count() > 0)
            //{
            //    return this.GetNoOverloadBomDetail(bomDetails.ToList());
            //}
            //else
            //{
            //    throw new BusinessException("Errors.Bom.BomDetailNotFound:" + bomCode);
            //}
        }
        #endregion


        [Transaction(TransactionMode.Requires)]
        public void ImportBom(Stream inputStream)
        {
            if(inputStream.Length == 0)
            {
                throw new BusinessException("Import.Stream.Empty");
            }

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();

            ImportHelper.JumpRows(rows, 1);

            #region 列定义
            int colBom = 0;//物料	
            int colBomDesc = 1;//物料描述	
            int colBomQty = 2;//基本数量	
            int colBomUom = 3;//Bom单位	
            int colOp = 4;//项目	
            int colItem = 5;//组件	
            int colItemDesc = 6;//描述	
            int colRateQty = 7;//数量	
            int colItemUom = 8;//组件单位	
            int colScrap = 9;//工序废品
            #endregion

            var errorMessage = new BusinessException();
            int colCount = 0;
            List<RowData> rowDataList = new List<RowData>();
            #region 读取数据
            while(rows.MoveNext())
            {
                HSSFRow row = (HSSFRow)rows.Current;
                if(!ImportHelper.CheckValidDataRow(row, 0, 9))
                {
                    break;//边界
                }
                colCount++;

                var rowData = new RowData();

                #region
                rowData.Bom = ImportHelper.GetCellStringValue(row.GetCell(colBom));
                if(rowData.Bom == null)
                {
                    errorMessage.AddMessage(Resources.EXT.ServiceLan.TheSpecialLineBomShouldNotEmpty, colCount.ToString());
                    rowData.HasError = true;
                }

                rowData.BomDesc = ImportHelper.GetCellStringValue(row.GetCell(colBomDesc));
                if(rowData.BomDesc == null)
                {
                    errorMessage.AddMessage(Resources.EXT.ServiceLan.TheSpecialLineBomDescShouldNotEmpty, colCount.ToString());
                    rowData.HasError = true;
                }

                string BomQty = ImportHelper.GetCellStringValue(row.GetCell(colBomQty));
                if(BomQty == null)
                {
                    errorMessage.AddMessage(Resources.EXT.ServiceLan.TheSpecialLineBomQtyShouldNotEmpty, colCount.ToString());
                    rowData.HasError = true;
                }
                else
                {
                    decimal _BomQty = 0m;
                    if(!decimal.TryParse(BomQty, out _BomQty))
                    {
                        rowData.HasError = true;
                        errorMessage.AddMessage(Resources.EXT.ServiceLan.TheSpecialLineBomQtyIsNotNum, colCount.ToString());
                    }
                    rowData.BomQty = _BomQty;
                }

                rowData.BomUom = ImportHelper.GetCellStringValue(row.GetCell(colBomUom));
                if(rowData.BomUom == null)
                {
                    errorMessage.AddMessage(Resources.EXT.ServiceLan.TheSpecialLineBomUomShouldNotEmpty, colCount.ToString());
                    rowData.HasError = true;
                }

                string Op = ImportHelper.GetCellStringValue(row.GetCell(colOp));
                if(Op == null)
                {
                    errorMessage.AddMessage(Resources.EXT.ServiceLan.TheSpecialLineOPShouldNotEmpty, colCount.ToString());
                    rowData.HasError = true;
                }
                else
                {
                    int _Op = 0;
                    if(!int.TryParse(Op, out _Op))
                    {
                        rowData.HasError = true;
                        errorMessage.AddMessage(Resources.EXT.ServiceLan.TheSpecialLineOPIsNotNum, colCount.ToString());
                    }
                    rowData.Op = _Op;
                }

                rowData.Item = ImportHelper.GetCellStringValue(row.GetCell(colItem));
                if(rowData.Item == null)
                {
                    errorMessage.AddMessage(Resources.EXT.ServiceLan.TheSpecialLineItemShouldNotEmpty, colCount.ToString());
                    rowData.HasError = true;
                }
                if(itemMgr.GetCacheItem(rowData.Item) == null)
                {
                    errorMessage.AddMessage(Resources.EXT.ServiceLan.TheSpecialLineItemIsNotExists, colCount.ToString(), rowData.Item);
                    rowData.HasError = true;
                }

                string rateQty = ImportHelper.GetCellStringValue(row.GetCell(colRateQty));
                if(rateQty == null)
                {
                    errorMessage.AddMessage(Resources.EXT.ServiceLan.TheSpecialLineRateQtyShouldNotEmpty, colCount.ToString());
                    rowData.HasError = true;
                }
                else
                {
                    decimal _rateQty = 0m;
                    if(!decimal.TryParse(rateQty, out _rateQty))
                    {
                        rowData.HasError = true;
                        errorMessage.AddMessage(Resources.EXT.ServiceLan.TheSpecialLineRateQtyIsNotNum, colCount.ToString());
                    }
                    rowData.RateQty = _rateQty;
                }

                rowData.ItemUom = ImportHelper.GetCellStringValue(row.GetCell(colItemUom));
                if(rowData.ItemUom == null)
                {
                    errorMessage.AddMessage(Resources.EXT.ServiceLan.TheSpecialLineItemUomShouldNotEmpty, colCount.ToString());
                    rowData.HasError = true;
                }
                else
                {
                    rowData.ItemUom = rowData.ItemUom.ToUpper();
                }

                string scrap = ImportHelper.GetCellStringValue(row.GetCell(colScrap));
                if(scrap == null)
                {
                    errorMessage.AddMessage(Resources.EXT.ServiceLan.TheSpecialLineScrapShouldNotEmpty, colCount.ToString());
                    rowData.HasError = true;
                }
                else
                {
                    decimal _Scrap = 0m;
                    if(!decimal.TryParse(scrap, out _Scrap))
                    {
                        rowData.HasError = true;
                        errorMessage.AddMessage(Resources.EXT.ServiceLan.TheSpecialLineScrapIsNotNum, colCount.ToString());
                    }
                    rowData.Scrap = _Scrap;
                }

                #endregion
                rowDataList.Add(rowData);
            }
            #endregion

            if(rowDataList.Count == 0)
            {
                errorMessage.AddMessage(Resources.EXT.ServiceLan.NotFoundEffectData);
                throw errorMessage;
            }

            #region 验证
            var dataRowGroup = rowDataList.Where(p => !p.HasError)
                .GroupBy(p => new { p.Bom, p.Item, p.Op }, (k, g) => new { k, Count = g.Count() })
                .Where(p => p.Count > 1).Select(p => new { p.k.Bom, p.k.Item, p.k.Op });
            foreach(var dataRow in dataRowGroup)
            {
                errorMessage.AddMessage(Resources.EXT.ServiceLan.BomImportDuplicate, dataRow.Bom, dataRow.Item, dataRow.Op.ToString());
            }
            #endregion

            if(!errorMessage.HasMessage)
            {
                var distinctDataRows = rowDataList.Where(p => !p.HasError)
                    .GroupBy(p => new { p.Bom, p.Item, p.Op }, (k, g) => new { k, j = g.First() })
                    .Select(p => p.j);

                var excelBomMasterList = distinctDataRows.GroupBy(p => p.Bom, (k, g) => new
                {
                    Bom = k,
                    List = g
                });

                var bomMasterDic = this.genericMgr.FindAll<BomMaster>().ToDictionary(d => d.Code, d => d);
                var bomDetailDic = this.genericMgr.FindAll<BomDetail>().GroupBy(p => p.Bom, (k, g) => new { k, g })
                    .ToDictionary(d => d.k, d => d.g.ToList());

                foreach(var excelBomMaster in excelBomMasterList)
                {
                    var firstDataRow = excelBomMaster.List.First();
                    var bomMaster = bomMasterDic.ValueOrDefault(excelBomMaster.Bom);

                    var bomDetails = bomDetailDic.ValueOrDefault(excelBomMaster.Bom) ?? new List<BomDetail>();
                    #region bomMaster
                    if(bomMaster == null)
                    {
                        try
                        {
                            var newBomMaster = new BomMaster();
                            newBomMaster.Code = excelBomMaster.Bom;
                            newBomMaster.Description = firstDataRow.BomDesc;
                            newBomMaster.IsActive = true;
                            newBomMaster.Qty = firstDataRow.BomQty;
                            newBomMaster.Uom = firstDataRow.BomUom;
                            this.genericMgr.Create(newBomMaster);
                        }
                        catch(Exception ex)
                        {
                            errorMessage.AddMessage(string.Format(Resources.EXT.ServiceLan.FaildCreateBomMaster, excelBomMaster.Bom, ex.Message));
                            break;
                        }
                    }
                    else
                    {
                        if(true || bomMaster.Description != firstDataRow.BomDesc ||
                            bomMaster.Qty != firstDataRow.BomQty ||
                            bomMaster.Uom != firstDataRow.BomUom)
                        {
                            bomMaster.Description = firstDataRow.BomDesc;
                            bomMaster.Qty = firstDataRow.BomQty;
                            bomMaster.Uom = firstDataRow.BomUom;
                            this.genericMgr.Update(bomMaster);
                        }
                    }
                    #endregion

                    #region BomDetail
                    foreach(var dataRow in excelBomMaster.List)
                    {
                        try
                        {
                            var bomDetail = bomDetails.FirstOrDefault(p => p.Item == dataRow.Item && p.Operation == dataRow.Op);
                            if(bomDetail == null)
                            {
                                //Id	Bom	Item	Op	Uom	StartDate	EndDate	RateQty	ScrapPct
                                var newBomDetail = new BomDetail();
                                newBomDetail.Bom = dataRow.Bom;
                                newBomDetail.Item = dataRow.Item;
                                newBomDetail.Operation = dataRow.Op;
                                newBomDetail.OpReference = "10";
                                newBomDetail.Uom = dataRow.ItemUom;
                                newBomDetail.StartDate = DateTime.Parse("2004-1-1");
                                newBomDetail.EndDate = DateTime.Parse("9999-1-1");
                                newBomDetail.RateQty = dataRow.RateQty;
                                newBomDetail.ScrapPercentage = dataRow.Scrap;
                                this.genericMgr.Create(newBomDetail);
                            }
                            else
                            {
                                if(true || bomDetail.Uom != dataRow.ItemUom ||
                                    bomDetail.RateQty != dataRow.RateQty ||
                                    bomDetail.ScrapPercentage != dataRow.Scrap)
                                {
                                    bomDetail.Uom = dataRow.ItemUom;
                                    bomDetail.RateQty = dataRow.RateQty;
                                    bomDetail.Operation = dataRow.Op;
                                    bomDetail.ScrapPercentage = dataRow.Scrap;
                                    bomDetail.StartDate = DateTime.Parse("2004-1-1");
                                    bomDetail.EndDate = DateTime.Parse("9999-1-1");
                                    this.genericMgr.Update(bomDetail);
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            errorMessage.AddMessage(string.Format(Resources.EXT.ServiceLan.FaildCreateBomDetail, dataRow.Item, ex.Message));
                            goto theEnd;
                        }
                    }

                    foreach(var bomDetail in bomDetails)
                    {
                        var dataRow = excelBomMaster.List.FirstOrDefault(p => p.Item == bomDetail.Item && p.Op == bomDetail.Operation);
                        if(dataRow == null)
                        {
                            bomDetail.EndDate = DateTime.Today;
                            this.genericMgr.Update(bomDetail);
                        }
                    }
                    #endregion
                }
                ResetBomCache();
            }
        theEnd:
            if(errorMessage.HasMessage)
            {
                throw errorMessage;
            }
        }

        #region 断面bom客户化

        public string GetSection(string itemCode)
        {
            var bomDetail = this.GetCacheAllBomDetail().SelectMany(p => p.Value).Where(p => p.Bom == itemCode
                 && p.StartDate <= DateTime.Now && (!p.EndDate.HasValue || p.EndDate >= DateTime.Now) && p.Item.StartsWith("29"))
                 .FirstOrDefault();
            if(bomDetail != null)
            {
                return bomDetail.Item;
            }
            else
            {
                return null;
            }
        }

        public void ProcessSectionBom()
        {
            log.Info("------------------------------------------------------------------");
            var bomMasterDic = this.genericMgr.FindAll<BomMaster>().ToDictionary(d => d.Code, d => d);
            var bomDetailDic = this.genericMgr.FindAll<BomDetail>().GroupBy(p => p.Bom, (k, g) => new { k, g })
                .ToDictionary(d => d.k, d => d.g.ToList());

            foreach(var bom in bomMasterDic)
            {
                var bomMaster = bom.Value;
                var bomDetails = bomDetailDic.ValueOrDefault(bom.Key) ?? new List<BomDetail>();
                var bomDetail29s = bomDetails.Where(p => p.Item.StartsWith("29"));

                //如果下级物料是断面,分母就是1000
                if(bomDetail29s.Count() > 0)
                {
                    #region bomMaster
                    decimal masterQty = bomMaster.Qty; //30的原来的分母
                    bomMaster.Qty = 1000;
                    this.genericMgr.Update(bomMaster);
                    #endregion

                    #region BomDetail
                    var bomDetail29First = bomDetail29s.First();
                    bomDetail29First.EndDate = DateTime.Parse("9999-1-1");
                    this.genericMgr.Update(bomDetail29First);
                    var bomMaster29 = bomMasterDic.ValueOrDefault(bomDetail29First.Bom);
                    var bomDetail2_s = bomDetails.Where(p => !p.Item.StartsWith("29"));
                    var bomDetails29 = bomDetailDic.ValueOrDefault(bomDetail29First.Item) ?? new List<BomDetail>();
                    foreach(var bomDetail2_ in bomDetail2_s)
                    {
                        var bomDetail = bomDetails29.FirstOrDefault(p => p.Item == bomDetail2_.Item && p.Operation == bomDetail2_.Operation);

                        decimal rateQty = (bomDetail2_.RateQty / masterQty)
                                        * (bomMaster.Qty / bomDetail29First.RateQty)
                                        * bomMaster29.Qty;
                        if(bomDetail == null)
                        {
                            //Id	Bom	Item	Op	Uom	StartDate	EndDate	RateQty	ScrapPct
                            var newBomDetail = new BomDetail();
                            newBomDetail.Bom = bomDetail2_.Bom;
                            newBomDetail.Item = bomDetail2_.Item;
                            newBomDetail.Operation = bomDetail2_.Operation;
                            newBomDetail.Uom = bomDetail2_.Uom;
                            newBomDetail.StartDate = DateTime.Parse("2004-1-1");
                            newBomDetail.EndDate = DateTime.Parse("9999-1-1");
                            newBomDetail.RateQty = rateQty;
                            newBomDetail.ScrapPercentage = bomDetail2_.ScrapPercentage;
                            this.genericMgr.Create(newBomDetail);
                        }
                        else
                        {
                            if(bomDetail.Uom != bomDetail2_.Uom ||
                                bomDetail.RateQty != rateQty ||
                                bomDetail.ScrapPercentage != bomDetail2_.ScrapPercentage)
                            {
                                bomDetail.Uom = bomDetail2_.Uom;
                                bomDetail.RateQty = rateQty;
                                bomDetail.ScrapPercentage = bomDetail2_.ScrapPercentage;
                                bomDetail.StartDate = DateTime.Parse("2004-1-1");
                                bomDetail.EndDate = DateTime.Parse("9999-1-1");
                                this.genericMgr.Update(bomDetail);
                            }
                        }
                        bomDetail2_.EndDate = DateTime.Parse("2000-1-1");
                        this.genericMgr.Update(bomDetail2_);
                    }

                    foreach(var bomDetail in bomDetails29)
                    {
                        var dataRow = bomDetail2_s.FirstOrDefault(p => p.Item == bomDetail.Item && p.Operation == bomDetail.Operation);
                        if(dataRow == null)
                        {
                            bomDetail.EndDate = DateTime.Today;
                            this.genericMgr.Update(bomDetail);
                        }
                    }
                    #endregion
                }
                else if(bomDetail29s.Count() == 0 && bom.Key.StartsWith("30"))
                {
                    log.Error(string.Format(Resources.EXT.ServiceLan.BomNotHaveMatchedSection, bom.Key));
                }
            }
        }
        #endregion
        class RowData
        {
            public string Bom { get; set; }//物料	
            public string BomDesc { get; set; }//物料描述	
            public decimal BomQty { get; set; }//基本数量	
            public string BomUom { get; set; }//Bom单位	
            public int Op { get; set; }//项目	
            public string Item { get; set; }//组件	
            public string ItemDesc { get; set; }//描述	
            public decimal RateQty { get; set; }//数量	
            public string ItemUom { get; set; }//组件单位	
            public decimal Scrap { get; set; }//工序废品
            public bool HasError { get; set; }
        }
    }
}
