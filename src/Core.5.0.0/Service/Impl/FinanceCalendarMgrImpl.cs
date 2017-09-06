using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.MD;
using com.Sconit.Entity.Exception;
using com.Sconit.Persistence;
using System.Data.SqlClient;
using System.Data;

namespace com.Sconit.Service.Impl
{
    public class FinanceCalendarMgrImpl : BaseMgr, IFinanceCalendarMgr
    {
        public IGenericMgr genericMgr { get; set; }
        public ISqlQueryDao sqlQueryDao { get; set; }

        private static string FinanceCalendarCloseLock = string.Empty;
        private static FinanceCalendar NowEffectiveFinanceCalendar;
        private static string selectEffectiveFinanceCalendar = @"from FinanceCalendar where IsClose = false order by EndDate asc";
        private static string selectCloseFinanceCalendar = @"from FinanceCalendar where IsClose = true order by EndDate desc";


        public FinanceCalendar GetNowEffectiveFinanceCalendar()
        {
            if (NowEffectiveFinanceCalendar == null)
            {
                //查询有效财政日历
                IList<FinanceCalendar> financeCalendarList = genericMgr.FindAll<FinanceCalendar>(selectEffectiveFinanceCalendar, 0, 1);
                if (financeCalendarList != null && financeCalendarList.Count > 0)
                {
                    NowEffectiveFinanceCalendar = financeCalendarList[0];
                }
                else
                {
                    throw new BusinessException(Resources.EXT.ServiceLan.TheFiscalMonthNotMaintained);
                }
            }

            return NowEffectiveFinanceCalendar;
        }

        public void CloseFinanceCalendar()
        {
            lock (FinanceCalendarCloseLock)
            {
                try
                {
                    //查询有效财政日历
                    IList<FinanceCalendar> effectiveFinanceCalendarList = genericMgr.FindAll<FinanceCalendar>(selectEffectiveFinanceCalendar, 0, 2);
                    IList<FinanceCalendar> closeFinanceCalendarList = genericMgr.FindAll<FinanceCalendar>(selectCloseFinanceCalendar, 0, 1);

                    if (effectiveFinanceCalendarList != null && effectiveFinanceCalendarList.Count > 0)
                    {
                        #region 判断月结日期是否连续
                        if (closeFinanceCalendarList != null && closeFinanceCalendarList.Count > 0)
                        {
                            if (closeFinanceCalendarList[0].EndDate != effectiveFinanceCalendarList[0].StartDate)
                            {
                                throw new BusinessException(Resources.EXT.ServiceLan.LastMBNotMatchEffDate);
                            }
                        }

                        if (effectiveFinanceCalendarList.Count != 2)
                        {
                            throw new BusinessException(Resources.EXT.ServiceLan.TheNextFiscalMonthNotMaintained);
                        }
                        else if (effectiveFinanceCalendarList[0].EndDate != effectiveFinanceCalendarList[1].StartDate)
                        {
                            throw new BusinessException(Resources.EXT.ServiceLan.CurrentFiscalMonthNotMatchNextStart);
                        }
                        #endregion

                        #region 判断当前日期是否满足月结结束日期
                        if (effectiveFinanceCalendarList[0].EndDate > DateTime.Now)
                        {
                            throw new BusinessException(Resources.EXT.ServiceLan.CouldMBNotEnd);
                        }
                        #endregion

                        //当前有效财政月改为下个月，不能补当前有效财政月的库存
                        NowEffectiveFinanceCalendar = effectiveFinanceCalendarList[1];

                        //调用存储过程月结
                        SqlParameter[] paras=new SqlParameter[4];
                        paras[0]=new SqlParameter("@FinanceYear", SqlDbType.Int);
                        paras[0].Value = effectiveFinanceCalendarList[0].FinanceYear;

                        paras[1]=new SqlParameter("@FinanceMonth", SqlDbType.Int);
                        paras[1].Value = effectiveFinanceCalendarList[0].FinanceMonth;

                        paras[2]=new SqlParameter("@UserId", SqlDbType.Int);
                        paras[2].Value = effectiveFinanceCalendarList[0].CreateUserId;

                        paras[3]=new SqlParameter("@UserNm", SqlDbType.VarChar, 100);
                        paras[3].Value = effectiveFinanceCalendarList[0].CreateUserName;
                        this.sqlQueryDao.ExecuteStoredProcedure("USP_Busi_SetMonthInv_All", paras);
                        //当前有效财政月关闭
                        effectiveFinanceCalendarList[0].IsClose = true;
                        this.genericMgr.Update(effectiveFinanceCalendarList[0]);
                    }
                    else
                    {
                        throw new BusinessException(Resources.EXT.ServiceLan.NotMaintainEffFiscalMonth);
                    }
                }
                finally
                {
                    //不管3721，把当前有效财政月置空，重新取。
                    NowEffectiveFinanceCalendar = null;
                }
            }
        }
    }
}
