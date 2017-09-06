namespace com.Sconit.Service.SI.Impl
{
    using System.Collections.Generic;
    using AutoMapper;
    using Castle.Services.Transaction;
    using System;

    public class SD_MasterDataMgrImpl : BaseMgr, com.Sconit.Service.SI.ISD_MasterDataMgr
    {
        [Transaction(TransactionMode.Requires)]
        public Entity.SI.SD_MD.Bin GetBin(string binCode)
        {
            var locationBin = this.genericMgr.FindById<Entity.MD.LocationBin>(binCode);
            var bin = Mapper.Map<Entity.MD.LocationBin, Entity.SI.SD_MD.Bin>(locationBin);
            var location = this.genericMgr.FindById<Entity.MD.Location>(bin.Location);
            bin.Region = location.Region;
            return bin;
        }

        [Transaction(TransactionMode.Requires)]
        public Entity.SI.SD_MD.Location GetLocation(string locationCode)
        {
            var locationBase = this.genericMgr.FindById<Entity.MD.Location>(locationCode);
            return Mapper.Map<Entity.MD.Location, Entity.SI.SD_MD.Location>(locationBase);
        }

        [Transaction(TransactionMode.Requires)]
        public Entity.SI.SD_MD.Pallet GetPallet(string palletCode)
        {
            var palletBase = this.genericMgr.FindById<Entity.MD.Pallet>(palletCode);
            return Mapper.Map<Entity.MD.Pallet, Entity.SI.SD_MD.Pallet>(palletBase);
        }

        [Transaction(TransactionMode.Requires)]
        public string GetEntityPreference(Entity.SYS.EntityPreference.CodeEnum entityEnum)
        {
            return this.systemMgr.GetEntityPreferenceValue(entityEnum);
        }

        [Transaction(TransactionMode.Requires)]
        public Entity.SI.SD_MD.Item GetItem(string itemCode)
        {
            var itemBase = this.genericMgr.FindById<Entity.MD.Item>(itemCode);
            return Mapper.Map<Entity.MD.Item, Entity.SI.SD_MD.Item>(itemBase);
        }

        public DateTime GetEffDate(string date)
        {
            DateTime effDate = DateTime.Now;
            if (date.Length == 6)
            {
                string newStr = effDate.Year.ToString() + "-";//年
                newStr += date.Substring(0, 2) + "-";//月
                newStr += date.Substring(2, 2) + " ";//日
                newStr += date.Substring(4, 2) + ":00:00";//时:分:秒
                effDate = DateTime.Parse(newStr);
                if (effDate > DateTime.Now)
                {
                    if (effDate.Month == 1)
                    {
                        effDate.AddYears(-1);
                    }
                    else
                    {
                        throw new Entity.Exception.BusinessException("输入的时间不能大于当前时间");
                    }
                }
            }
            else
            {
                throw new Entity.Exception.BusinessException("输入正确的格式,2月3日14时输入..020314");
            }
            Entity.MD.FinanceCalendar financeCalendar = financeCalendarMgr.GetNowEffectiveFinanceCalendar();
            if (financeCalendar.StartDate.Value > effDate || financeCalendar.EndDate.Value <= effDate)
            {
                throw new Entity.Exception.BusinessException("不在开放的会计期间里面");
            }
            return effDate;
        }
    }
}
