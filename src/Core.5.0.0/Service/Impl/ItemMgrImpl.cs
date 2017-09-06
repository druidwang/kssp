using System;
using System.Collections.Generic;
using System.Linq;
using com.Sconit.Entity.BIL;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MD;
using NHibernate.Criterion;
using Castle.Services.Transaction;
using com.Sconit.Entity.CUST;
using NHibernate.Type;
using NHibernate;
using com.Sconit.Utility;
using System.IO;
using NPOI.HSSF.UserModel;
using System.Collections;
using NPOI.SS.UserModel;

namespace com.Sconit.Service.Impl
{
    [Transactional]
    public class ItemMgrImpl : BaseMgr, IItemMgr
    {
        #region 变量
        public IQueryMgr queryMgr { get; set; }
        public IGenericMgr genericMgr { get; set; }
        #endregion

        private static IList<UomConversion> cachedAllUomConversion;
        private static Dictionary<string, Item> cachedAllItem;
        private static DateTime cacheDateTime;

        #region public methods

        private static object cacheUomLock = new object();
        public IList<UomConversion> GetCacheAllUomConversionList()
        {
            lock (cacheUomLock)
            {
                if (cachedAllUomConversion == null || cacheDateTime < DateTime.Now.AddMinutes(-10))
                {
                    cacheDateTime = DateTime.Now;
                    cachedAllUomConversion = this.genericMgr.FindAll<UomConversion>();
                }
                return cachedAllUomConversion;
            }
        }

        private UomConversion LoadUomConversion(string itemCode, string sourceUomCode, string targetUomCode)
        {
            return GetCacheAllUomConversionList()
                .FirstOrDefault(p => p.Item.Code == itemCode && p.BaseUom == sourceUomCode && p.AlterUom == targetUomCode);
        }

        private static object cacheLock = new object();
        public Dictionary<string, Item> GetCacheAllItem()
        {
            lock (cacheLock)
            {
                if (cachedAllItem == null || cacheDateTime < DateTime.Now.AddMinutes(-10))
                {
                    cacheDateTime = DateTime.Now;
                    var allItem = this.genericMgr.FindAll<Item>();
                    cachedAllItem = allItem.ToDictionary(d => d.Code, d => d);
                }
                return cachedAllItem;
            }
        }

        public Item GetCacheItem(string itemCode)
        {
            return GetCacheAllItem().ValueOrDefault(itemCode);
        }

        public void ResetItemCache()
        {
            cacheDateTime = DateTime.MinValue;
        }

        public IList<ItemKit> GetKitItemChildren(string kitCode)
        {
            return GetKitItemChildren(kitCode, false);
        }

        public IList<ItemKit> GetKitItemChildren(string kitItem, bool includeInActive)
        {
            DetachedCriteria criteria = DetachedCriteria.For<ItemKit>();

            criteria.Add(Expression.Eq("KitItem", kitItem));
            criteria.Add(Expression.Eq("IsActive", includeInActive));

            return queryMgr.FindAll<ItemKit>(criteria);
        }

        public decimal ConvertItemUomQty(string itemCode, string sourceUomCode, decimal sourceQty, string targetUomCode)
        {
            if (itemCode == null || sourceUomCode == null || targetUomCode == null)
            {
                throw new BusinessException("UomConversion Error:itemCode Or sourceUomCode Or targetUomCode is null");
            }

            if (sourceUomCode == targetUomCode || sourceQty == 0)
            {
                return sourceQty;
            }

            IList<UomConversion> allUomConversionList = GetCacheAllUomConversionList()
                .Where(p => p.Item == null || string.Equals(p.Item.Code, itemCode)).ToList();
            if (allUomConversionList != null)
            {
                List<UomConversion> uomConversionList = allUomConversionList.Where(u => u.Item != null).ToList();
                foreach (UomConversion y in allUomConversionList)
                {
                    if (uomConversionList.Where(x =>
                        (string.Equals(x.AlterUom, y.AlterUom) && string.Equals(x.BaseUom, y.BaseUom))
                        || (string.Equals(x.AlterUom, y.BaseUom) && string.Equals(x.BaseUom, y.AlterUom)))
                        .Count() == 0)
                    {
                        uomConversionList.Add(y);
                    }
                }
                foreach (UomConversion u in uomConversionList)
                {
                    //顺
                    if (string.Equals(u.BaseUom, sourceUomCode))
                    {
                        u.Qty = sourceQty * u.AlterQty / u.BaseQty;
                        u.IsAsc = true;
                        if (string.Equals(u.AlterUom, targetUomCode))
                        {
                            return u.Qty.Value;
                        }
                    }
                    //反
                    else if (string.Equals(u.AlterUom, sourceUomCode))
                    {
                        u.Qty = sourceQty * u.BaseQty / u.AlterQty;
                        u.IsAsc = false;
                        if (string.Equals(u.BaseUom, targetUomCode))
                        {
                            return u.Qty.Value;
                        }
                    }
                }

                for (int i = 1; i < uomConversionList.Count; i++)
                {
                    foreach (UomConversion uomConversion1 in uomConversionList)
                    {
                        if (uomConversion1.Qty.HasValue)
                        {
                            foreach (UomConversion uomConversion2 in uomConversionList)
                            {
                                //顺
                                if (uomConversion1.IsAsc)
                                {
                                    //顺
                                    if (string.Equals(uomConversion2.BaseUom, uomConversion1.AlterUom) && !uomConversion2.Qty.HasValue)
                                    {
                                        uomConversion2.Qty = uomConversion1.Qty.Value * uomConversion2.AlterQty / uomConversion2.BaseQty;
                                        uomConversion2.IsAsc = true;
                                        if (string.Equals(uomConversion2.AlterUom, targetUomCode))
                                        {
                                            return uomConversion2.Qty.Value;
                                        }
                                    }
                                    //反
                                    else if (string.Equals(uomConversion2.AlterUom, uomConversion1.AlterUom) && !uomConversion2.Qty.HasValue)
                                    {
                                        uomConversion2.Qty = uomConversion1.Qty.Value * uomConversion2.BaseQty / uomConversion2.AlterQty;
                                        uomConversion2.IsAsc = false;
                                        if (string.Equals(uomConversion2.BaseUom, targetUomCode))
                                        {
                                            return uomConversion2.Qty.Value;
                                        }
                                    }
                                }
                                //反
                                else
                                {
                                    //顺
                                    if (string.Equals(uomConversion2.BaseUom, uomConversion1.BaseUom) && !uomConversion2.Qty.HasValue)
                                    {
                                        uomConversion2.Qty = uomConversion1.Qty.Value * uomConversion2.AlterQty / uomConversion2.BaseQty;
                                        uomConversion2.IsAsc = true;
                                        if (string.Equals(uomConversion2.AlterUom, targetUomCode))
                                        {
                                            return uomConversion2.Qty.Value;
                                        }
                                    }
                                    //反
                                    else if (string.Equals(uomConversion2.AlterUom, uomConversion1.BaseUom) && !uomConversion2.Qty.HasValue)
                                    {
                                        uomConversion2.Qty = uomConversion1.Qty.Value * uomConversion2.BaseQty / uomConversion2.AlterQty;
                                        uomConversion2.IsAsc = false;
                                        if (string.Equals(uomConversion2.BaseUom, targetUomCode))
                                        {
                                            return uomConversion2.Qty.Value;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            throw new BusinessException(string.Format("单位转换没有找到:物料{0},原单位{1},转换后单位{2}", itemCode, sourceUomCode, targetUomCode));
        }

        /*
        public decimal ConvertItemUomQty(string itemCode, string sourceUomCode, decimal sourceQty, string targetUomCode)
        {
            if (sourceUomCode == targetUomCode || sourceQty == 0)
            {
                return sourceQty;
            }

            #region 第一次转换单位，使用Item定义的单位转换
            DetachedCriteria criteria = DetachedCriteria.For<UomConversion>();
            criteria.Add(Expression.Eq("Item.Code", itemCode));

            IList<UomConversion> SpecifiedItemUomConversionList = queryMgr.FindAll<UomConversion>(criteria);

            UomConversion uomConversion = (from conv in SpecifiedItemUomConversionList
                                           where (conv.BaseUom == sourceUomCode && conv.AlterUom == targetUomCode)
                                           || (conv.BaseUom == targetUomCode && conv.AlterUom == sourceUomCode)
                                           select conv).FirstOrDefault();

            if (uomConversion != null)
            {
                if (uomConversion.BaseUom == sourceUomCode)
                {
                    return (sourceQty * uomConversion.AlterQty / uomConversion.BaseQty);
                }
                else
                {
                    return (sourceQty * uomConversion.BaseQty / uomConversion.AlterQty);
                }
            }
            #endregion

            #region 第二次转换单位，使用通用的单位转换
            criteria = DetachedCriteria.For<UomConversion>();
            criteria.Add(Expression.IsNull("Item"));

            IList<UomConversion> genericItemUomConversionList = queryMgr.FindAll<UomConversion>(criteria);

            uomConversion = (from conv in genericItemUomConversionList
                             where (conv.BaseUom == sourceUomCode && conv.AlterUom == targetUomCode)
                             || (conv.BaseUom == targetUomCode && conv.AlterUom == sourceUomCode)
                             select conv).FirstOrDefault();

            if (uomConversion != null)
            {
                if (uomConversion.BaseUom == sourceUomCode)
                {
                    return (sourceQty * uomConversion.AlterQty / uomConversion.BaseQty);
                }
                else
                {
                    return (sourceQty * uomConversion.BaseQty / uomConversion.AlterQty);
                }
            }
            #endregion

            #region 无级单位转换
            List<UomConversion> mergedItemUomConversionList = new List<UomConversion>();
            mergedItemUomConversionList.AddRange(SpecifiedItemUomConversionList);
            mergedItemUomConversionList.AddRange(genericItemUomConversionList);

            //思路：用sourceUomCode、targetUomCode分别和uomConversion.BaseUom、uomConversion.AlterUom匹配
            //总共4种情况，循环嵌套往下查找。
            //每次嵌套需要过滤掉已经用过得uomConversion来防止死循环。
            decimal? targetQty = NestConvertItemUomQty(mergedItemUomConversionList, sourceUomCode, targetUomCode, sourceQty, false);

            if (!targetQty.HasValue)
            {
                targetQty = NestConvertItemUomQty(mergedItemUomConversionList, targetUomCode, sourceUomCode, sourceQty, true);
            }

            if (targetQty.HasValue)
            {
                return targetQty.Value;
            }
            else
            {
                throw new BusinessException("Errors.Item.UomConvertionNotFound", itemCode, sourceUomCode, targetUomCode);
            }
            #endregion
        }
        */

        public PriceListDetail GetItemPrice(string itemCode, string uom, string priceList, string currency, DateTime? effectiveDate)
        {
            if (!effectiveDate.HasValue)
            {
                effectiveDate = DateTime.Now;
            }

            DetachedCriteria criteria = DetachedCriteria.For<PriceListDetail>();

            criteria.CreateAlias("PriceList", "pl");

            criteria.Add(Expression.Eq("pl.Code", priceList));
            criteria.Add(Expression.Eq("Item", itemCode));
            criteria.Add(Expression.Eq("Uom", uom));
            criteria.Add(Expression.Eq("pl.Currency", currency));
            criteria.Add(Expression.Le("StartDate", effectiveDate));
            criteria.Add(Expression.Or(Expression.Ge("EndDate", effectiveDate), Expression.IsNull("EndDate")));

            criteria.AddOrder(Order.Desc("StartDate"));

            IList<PriceListDetail> resultList = queryMgr.FindAll<PriceListDetail>(criteria, 0, 1);
            if (resultList != null && resultList.Count() > 0)
            {
                return resultList[0];
            }
            else
            {
                return null;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateItem(Item item)
        {
            ItemPackage itemPackage = new ItemPackage();
            itemPackage.Item = item.Code;
            itemPackage.IsDefault = true;
            itemPackage.UnitCount = item.UnitCount;
            itemPackage.Description = string.Empty;

            genericMgr.Create(item);
            genericMgr.Create(itemPackage);
            this.ResetItemCache();
            GetCacheAllItem();
            //if (!cachedAllItem.ContainsKey(item.Code))
            //{
            //    cachedAllItem.Add(item.Code, item);
            //}
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateItem(Item item)
        {
            string hql = "from ItemPackage where Item = ? and UnitCount = ?";
            IList<ItemPackage> itemPackageList = queryMgr.FindAll<ItemPackage>(hql, new object[] { item.Code, item.UnitCount });
            if (itemPackageList != null && itemPackageList.Count > 0)
            {
                ItemPackage itemPackage = itemPackageList[0];
                if (!itemPackage.IsDefault)
                {
                    itemPackage.IsDefault = true;
                    genericMgr.Update(itemPackage);
                }
            }
            else
            {
                #region 原默认包装至False
                hql = "from ItemPackage as i where i.Item = ? and i.IsDefault= ?";
                IList<ItemPackage> defualtItemPackageList = queryMgr.FindAll<ItemPackage>(hql, new object[] { item.Code, true });
                for (int i = 0; i < defualtItemPackageList.Count; i++)
                {
                    defualtItemPackageList[i].IsDefault = false;
                    this.genericMgr.Update(defualtItemPackageList[i]);
                }
                #endregion

                #region 没有找到包装，新增包装
                ItemPackage itemPackage = new ItemPackage();
                itemPackage.Item = item.Code;
                itemPackage.IsDefault = true;
                itemPackage.UnitCount = item.UnitCount;
                itemPackage.Description = string.Empty;
                this.genericMgr.Create(itemPackage);
                #endregion
            }
            //GetCacheAllItem();
            //if (cachedAllItem.ContainsKey(item.Code))
            //{
            //    cachedAllItem[item.Code] = item;
            //}
            genericMgr.Update(item);
            this.ResetItemCache();
        }

        public IList<ItemDiscontinue> GetItemDiscontinues(string itemCode, DateTime effectiveDate)
        {
            string hql = "from ItemDiscontinue where Item = ? and StartDate < ? and (EndDate is null or EndDate >= ?) order by Priority ";
            return this.genericMgr.FindAll<ItemDiscontinue>(hql, new object[] { itemCode, effectiveDate, effectiveDate });
        }

        public IList<ItemDiscontinue> GetParentItemDiscontinues(string itemCode, DateTime effectiveDate)
        {
            string hql = "from ItemDiscontinue where DiscontinueItem = ? and StartDate < ? and (EndDate is null or EndDate >= ?)";
            return this.genericMgr.FindAll<ItemDiscontinue>(hql, new object[] { itemCode, effectiveDate, effectiveDate });
        }

        public IList<Item> GetItems(IList<string> itemCodeList)
        {
            if (itemCodeList != null && itemCodeList.Count > 0)
            {
                //IList<Item> itemList = this.genericMgr.FindAllIn<Item>("from Item where code in(?", itemCodeList);
                IList<Item> itemList = new List<Item>();
                var allItems = GetCacheAllItem();
                foreach (var itemCode in itemCodeList)
                {
                    itemList.Add(allItems[itemCode]);
                }

                return itemList;
            }

            return null;
        }

        #region 保管员
        [Transaction(TransactionMode.Requires)]
        public void CreateCustodian(Custodian custodian)
        {
            string ItemCodeStr = custodian.ItemCodes.Replace("\r\n", ",");
            string[] ItemCodeArray = ItemCodeStr.Split(',');
            BusinessException businessException = new BusinessException();
            #region 判定Item是否有效
            foreach (string itemCode in ItemCodeArray.Distinct())
            {
                if (itemCode != null && itemCode != "")
                {
                    IList<Item> items = this.genericMgr.FindAll<Item>("from Item where Code='" + itemCode + "'");
                    if (items == null || items.Count == 0)
                    {
                        businessException.AddMessage(itemCode.ToString() + ",");
                    }
                }
            }
            if (businessException.HasMessage)
            {
                businessException.AddMessage("不存在。");
                throw businessException;
            }

            #endregion

            #region 判定物料是否已经分配保管员
            string hql = string.Empty;
            IList<object> parm = new List<object>();
            foreach (string itemCode in ItemCodeArray.Distinct())
            {
                if (hql == string.Empty)
                {
                    hql = "from Custodian where Location=? and UserCode=? and item in (?";
                    parm.Add(custodian.Location);
                    parm.Add(custodian.UserCode);
                }
                else
                {
                    hql += ", ?";
                }
                parm.Add(itemCode);

            }

            hql += ")";
            IList<Custodian> CustodianList = this.queryMgr.FindAll<Custodian>(hql, parm.ToArray());

            if (CustodianList != null && CustodianList.Count > 0)
            {

                foreach (Custodian cd in CustodianList)
                {
                    businessException.AddMessage(cd.Item.ToString() + ",");
                }
                if (businessException.HasMessage)
                {
                    businessException.AddMessage("已经分配保管员。");
                    throw businessException;
                }
            }
            #endregion

            foreach (string itemCode in ItemCodeArray.Distinct())
            {
                if (itemCode != "")
                {
                    Custodian cus = new Custodian();
                    cus.Item = itemCode;
                    cus.Location = custodian.Location;
                    cus.UserCode = custodian.UserCode;

                    this.genericMgr.Create(cus);
                }
            }


        }
        #endregion

        public void DeleteItem(string itemCode)
        {
            this.genericMgr.Delete("from ItemPackage where Item = ? ", new object[] { itemCode }, new IType[] { NHibernateUtil.String });
            this.genericMgr.DeleteById<Item>(itemCode);
            //GetCacheAllItem();
            //if (cachedAllItem.ContainsKey(itemCode))
            //{
            //    cachedAllItem.Remove(itemCode);
            //}
            this.ResetItemCache();
        }

        public IList<Uom> GetItemUoms(string itemCode)
        {
            IList<Uom> uomList = new List<Uom>();
            itemCode = itemCode == null ? string.Empty : itemCode;
            var _items = this.genericMgr.FindAll<Item>("from Item where Code = ? ", itemCode);
            if (_items != null && _items.Count() == 1)
            {
                var _item = _items[0];
                List<string> uomCodes = new List<string>();

                GetUomCodes(_item.Uom, uomCodes);

                //IList<UomConversion> uomConvList = genericMgr.FindAll<UomConversion>
                //    ("from UomConversion as u where u.Item.Code = ? and (u.AlterUom = ? or u.BaseUom = ?)",
                //    new object[] { itemCode, _item.Uom, _item.Uom });
                var uomConvList = GetCacheAllUomConversionList().Where(u => u.Item != null && u.Item.Code == itemCode
                     && (u.AlterUom == _item.Uom || u.BaseUom == _item.Uom));

                if (uomConvList != null && uomConvList.Count() > 0)
                {
                    foreach (var uomConv in uomConvList)
                    {
                        if (uomConv.BaseUom == _item.Uom && !uomCodes.Contains(uomConv.AlterUom))
                        {
                            GetUomCodes(uomConv.AlterUom, uomCodes);
                        }
                        else if (uomConv.AlterUom == _item.Uom && !uomCodes.Contains(uomConv.BaseUom))
                        {
                            GetUomCodes(uomConv.BaseUom, uomCodes);
                        }
                    }
                }

                foreach (var uomCode in uomCodes.Distinct())
                {
                    uomList.Add(queryMgr.FindById<Uom>(uomCode));
                }
            }
            return uomList;
        }


        public Dictionary<string, Item> GetRefItemCode(string flowCode, List<string> refItemCodeList)
        {
            Dictionary<string, Item> itemDic = new Dictionary<string, Item>();
            if (!string.IsNullOrWhiteSpace(flowCode))
            {
                string sql = @"select f.Item, case when ltrim(isnull(f.RefItemCode,''))='' then i.RefCode else f.RefItemCode end as RefCode into #item 
                        from SCM_FlowDet f join MD_Item i on f.Item = i.Code where f.Flow = '9306-500026'
                        select * from #item where RefCode in(?";
                var itemCodes = this.genericMgr.FindAllWithNativeSqlIn<object[]>(sql, refItemCodeList)
                    .GroupBy(p => p[0], (k, g) => new { Item = (string)k, RefCode = (string)(g.First()[1]) });
                foreach (var itemCode in itemCodes)
                {
                    itemDic.Add(itemCode.RefCode, GetCacheItem(itemCode.Item));
                }
            }
            else
            {
                itemDic = refItemCodeList.GroupJoin(GetCacheAllItem().Select(p => p.Value), r => r, i => i.ReferenceCode, (r, i) => new { r, i })
                    .Where(p => p.i != null && p.i.Count() > 0).ToDictionary(d => d.r, d => d.i.First());
            }
            return itemDic;
        }

        #endregion

        #region private methods
        private decimal? NestConvertItemUomQty(IList<UomConversion> itemUomConversionList, string sourceUomCode, string targetUomCode, decimal convQty, bool isUomReversed)
        {
            IList<UomConversion> matchedUomConversionList = (from conv in itemUomConversionList
                                                             where (conv.BaseUom == sourceUomCode)   //用Source Uom匹配
                                                             || (conv.AlterUom == sourceUomCode)
                                                             orderby conv.Item descending            //把Item有值的放前面
                                                             select conv).ToList();

            foreach (UomConversion matchedUomConversion in matchedUomConversionList)
            {
                if (matchedUomConversion.BaseUom == sourceUomCode)
                {
                    sourceUomCode = matchedUomConversion.AlterUom;
                    if (isUomReversed)
                    {
                        convQty = (convQty * matchedUomConversion.AlterQty / matchedUomConversion.BaseQty);
                    }
                    else
                    {
                        convQty = (convQty * matchedUomConversion.BaseQty / matchedUomConversion.AlterQty);
                    }
                }
                else
                {
                    sourceUomCode = matchedUomConversion.BaseUom;
                    if (isUomReversed)
                    {
                        convQty = (convQty * matchedUomConversion.BaseQty / matchedUomConversion.AlterQty);
                    }
                    else
                    {
                        convQty = (convQty * matchedUomConversion.AlterQty / matchedUomConversion.BaseQty);
                    }
                }

                if (sourceUomCode == targetUomCode)
                {
                    //新的Source Uom等于Target Uom, 找到目标转换单位
                    return convQty;
                }
                else
                {
                    //没有找到目标转换单位
                    //先过滤掉已经匹配过的转换
                    IList<UomConversion> filteredUomConversionList = itemUomConversionList.Where(m => !m.Equals(matchedUomConversion)).ToList();

                    //再嵌套往下查找
                    decimal? targetQty = NestConvertItemUomQty(filteredUomConversionList, sourceUomCode, targetUomCode, convQty, false);
                    if (targetQty.HasValue)
                    {
                        return targetQty;
                    }
                    else
                    {
                        targetQty = NestConvertItemUomQty(filteredUomConversionList, targetUomCode, sourceUomCode, convQty, true);
                    }
                }
            }

            return null;
        }

        private void GetUomCodes(string uom, List<string> uomCodes)
        {
            uomCodes.Add(uom);
            //IList<UomConversion> uomConvList = queryMgr.FindAll<UomConversion>
            //    ("from UomConversion as u where (u.BaseUom=? or u.AlterUom=? ) ",
            //    new object[] { uom, uom });
            var uomConvList = GetCacheAllUomConversionList().Where(
                u => u.Item == null && (u.BaseUom == uom || u.AlterUom == uom));
            if (uomConvList != null && uomConvList.Count() > 0)
            {
                foreach (var uomConv in uomConvList)
                {
                    if (uomConv.BaseUom == uom && !uomCodes.Contains(uomConv.AlterUom))
                    {
                        uomCodes.Add(uomConv.AlterUom);
                        GetUomCodes(uomConv.AlterUom, uomCodes);
                    }
                    else if (uomConv.AlterUom == uom && !uomCodes.Contains(uomConv.BaseUom))
                    {
                        uomCodes.Add(uomConv.BaseUom);
                        GetUomCodes(uomConv.BaseUom, uomCodes);
                    }
                }
            }
        }

        #endregion

        [Transaction(TransactionMode.Requires)]
        public void ImportItem(Stream inputStream)
        {
            if (inputStream.Length == 0)
            {
                throw new BusinessException("Import.Stream.Empty");
            }

            HSSFWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);
            IEnumerator rows = sheet.GetRowEnumerator();

            ImportHelper.JumpRows(rows, 1);

            #region 列定义
            //物料	物料描述	旧物料号	物料类型	基本计量单位	物料组 	虚拟物料
            int colItem = 0;//物料	
            int colItemDesc = 1;//物料描述	
            int colItemRef = 2;//旧物料号
            int colItemCategory = 3;//物料类型		
            int colUom = 4;//基本计量单位	
            int colUc = 5;//基本计量单位	
            int colItemGroup = 6;//物料组	
            int colIsVirtual = 7;//虚拟物料	
            int colDivision = 8;//产品组	
            #endregion

            var errorMessage = new BusinessException();
            int colCount = 0;
            List<Item> rowDataList = new List<Item>();
            #region 读取数据
            while (rows.MoveNext())
            {
                HSSFRow row = (HSSFRow)rows.Current;
                if (!ImportHelper.CheckValidDataRow(row, 0, 7))
                {
                    break;//边界
                }
                colCount++;

                bool hasError = false;
                var rowData = new Item();

                #region
                rowData.Code = ImportHelper.GetCellStringValue(row.GetCell(colItem));
                if (rowData.Code == null)
                {
                    errorMessage.AddMessage("第{0}行物料代码不能为空", colCount.ToString());
                    hasError = true;
                }

                rowData.Description = ImportHelper.GetCellStringValue(row.GetCell(colItemDesc));
                if (rowData.Description == null)
                {
                    errorMessage.AddMessage("第{0}行物料描述不能为空", colCount.ToString());
                    hasError = true;
                }

                rowData.ReferenceCode = ImportHelper.GetCellStringValue(row.GetCell(colItemRef));

                rowData.ItemCategory = ImportHelper.GetCellStringValue(row.GetCell(colItemCategory));
                if (rowData.ItemCategory == null)
                {
                    errorMessage.AddMessage("第{0}行物料类型不能为空", colCount.ToString());
                    hasError = true;
                }

                rowData.Uom = ImportHelper.GetCellStringValue(row.GetCell(colUom));
                if (rowData.Uom == null)
                {
                    errorMessage.AddMessage("第{0}行基本计量单位不能为空", colCount.ToString());
                    hasError = true;
                }
                else
                {
                    rowData.Uom = rowData.Uom.ToUpper();
                }

                string uc = ImportHelper.GetCellStringValue(row.GetCell(colUc));
                decimal _uc = 1;
                decimal.TryParse(uc, out _uc);
                rowData.UnitCount = _uc;

                rowData.MaterialsGroup = ImportHelper.GetCellStringValue(row.GetCell(colItemGroup));
                if (rowData.MaterialsGroup == null)
                {
                    errorMessage.AddMessage("第{0}行物料组不能为空", colCount.ToString());
                    hasError = true;
                }
                else
                {
                    rowData.MaterialsGroup = rowData.MaterialsGroup.ToUpper();
                }

                if (ImportHelper.GetCellStringValue(row.GetCell(colIsVirtual)) == "X")
                {
                    rowData.IsVirtual = true;
                }
                rowData.Division = ImportHelper.GetCellStringValue(row.GetCell(colDivision));
                #endregion
                if (!hasError)
                {
                    rowData.IsActive = true;
                    rowData.UnitCount = 1;

                    rowDataList.Add(rowData);
                }
            }
            #endregion

            if (rowDataList.Count == 0)
            {
                errorMessage.AddMessage("没有找到有效的数据.");
                throw errorMessage;
            }

            #region 验证重复明细
            var dataRowGroup = rowDataList.GroupBy(p => p.Code).Where(p => p.Count() > 1).Select(p => new { p.Key, p.First().Description });
            foreach (var dataRow in dataRowGroup)
            {
                errorMessage.AddMessage("存在重复的明细:物料{0}[{1}]", dataRow.Key, dataRow.Description);
            }
            #endregion

            if (!errorMessage.HasMessage)
            {
                var allItems = GetCacheAllItem();
                foreach (var rowData in rowDataList)
                {
                    if (allItems.ContainsKey(rowData.Code))
                    {
                        var item = allItems[rowData.Code];
                        if (item.Description != rowData.Description ||
                            item.ReferenceCode != rowData.ReferenceCode ||
                            item.MaterialsGroup != rowData.MaterialsGroup ||
                            item.ItemCategory != rowData.ItemCategory ||
                            item.UnitCount != rowData.UnitCount)
                        {
                            item.Description = rowData.Description;
                            item.ReferenceCode = rowData.ReferenceCode;
                            item.MaterialsGroup = rowData.MaterialsGroup;
                            item.ItemCategory = rowData.ItemCategory;
                            item.UnitCount = rowData.UnitCount;
                            this.genericMgr.Update(item);
                        }
                    }
                    else
                    {
                        this.genericMgr.Create(rowData);
                    }
                }
            }
            else
            {
                throw errorMessage;
            }
        }

    }
}
