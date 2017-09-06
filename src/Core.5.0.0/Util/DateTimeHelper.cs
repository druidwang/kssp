using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity;
using System.Globalization;
using com.Sconit.Entity.Exception;

namespace com.Sconit.Utility
{
    public static class DateTimeHelper
    {
        private static readonly double _weekStartDay = 1;

        public static DateTime GetStartTime(CodeMaster.TimeUnit timePeriodType, DateTime startTime)
        {
            DateTime time = startTime;
            //天起始
            if (timePeriodType == CodeMaster.TimeUnit.Day || timePeriodType == CodeMaster.TimeUnit.Hour)
            {
                time = startTime.Date;
            }
            //周起始
            else if (timePeriodType == CodeMaster.TimeUnit.Week)
            {
                int dayOfWeek = (int)(startTime.AddDays(-_weekStartDay).DayOfWeek);
                time = startTime.Date.AddDays(-(double)dayOfWeek);
            }
            //月起始
            else if (timePeriodType == CodeMaster.TimeUnit.Month)
            {
                time = new DateTime(startTime.Date.Year, startTime.Date.Month, 1);
            }
            //季起始
            else if (timePeriodType == CodeMaster.TimeUnit.Quarter)
            {
                int month = startTime.Date.Month;
                if (month < 4)
                {
                    time = new DateTime(startTime.Date.Year, 1, 1);
                }
                else if (month < 7)
                {
                    time = new DateTime(startTime.Date.Year, 4, 1);
                }
                else if (month < 10)
                {
                    time = new DateTime(startTime.Date.Year, 7, 1);
                }
                else
                {
                    time = new DateTime(startTime.Date.Year, 10, 1);
                }
            }
            //年起始
            else if (timePeriodType == CodeMaster.TimeUnit.Year)
            {
                time = new DateTime(startTime.Date.Year, 1, 1);
            }

            return time;
        }

        public static DateTime GetEndStartTime(CodeMaster.TimeUnit timePeriodType, DateTime endTime)
        {
            if (timePeriodType == CodeMaster.TimeUnit.Hour)
            {
                return endTime.Date.AddDays(1).AddHours(-1);
            }
            else
            {
                return GetStartTime(timePeriodType, endTime);
            }
        }

        public static DateTime GetEndTime(CodeMaster.TimeUnit timePeriodType, DateTime startTime)
        {
            DateTime nextStartTime = startTime;
            //天结束
            if (timePeriodType == CodeMaster.TimeUnit.Hour || timePeriodType == CodeMaster.TimeUnit.Day)
            {
                nextStartTime = startTime.Date.AddDays(1);
            }
            //周结束
            else if (timePeriodType == CodeMaster.TimeUnit.Week)
            {
                nextStartTime = GetStartTime(timePeriodType, startTime).AddDays(7);
            }
            //月结束
            else if (timePeriodType == CodeMaster.TimeUnit.Month)
            {
                nextStartTime = GetStartTime(timePeriodType, startTime).AddMonths(1);
            }
            //季结束
            else if (timePeriodType == CodeMaster.TimeUnit.Quarter)
            {
                int month = startTime.Date.Month;
                if (month < 4)
                {
                    nextStartTime = new DateTime(startTime.Date.Year, 3, 1);
                }
                else if (month < 7)
                {
                    nextStartTime = new DateTime(startTime.Date.Year, 6, 1);
                }
                else if (month < 10)
                {
                    nextStartTime = new DateTime(startTime.Date.Year, 9, 1);
                }
                else
                {
                    nextStartTime = new DateTime(startTime.Date.Year, 12, 1);
                }

                nextStartTime = nextStartTime.AddMonths(1);
            }
            else if (timePeriodType == CodeMaster.TimeUnit.Year)
            {
                nextStartTime = GetStartTime(timePeriodType, startTime).AddYears(1);
            }

            DateTime time = nextStartTime.AddMilliseconds(-1);
            return time;
        }

        public static DateTime GetNextStartTime(CodeMaster.TimeUnit timePeriodType, DateTime startTime)
        {
            DateTime nextStartTime = startTime;
            //下一天
            if (timePeriodType == CodeMaster.TimeUnit.Day)
            {
                nextStartTime = startTime.Date.AddDays(1);
            }
            //下一周
            else if (timePeriodType == CodeMaster.TimeUnit.Week)
            {
                nextStartTime = GetStartTime(timePeriodType, startTime).AddDays(7);
            }
            //下一月
            else if (timePeriodType == CodeMaster.TimeUnit.Month)
            {
                nextStartTime = GetStartTime(timePeriodType, startTime).AddMonths(1);
            }
            //下一季
            else if (timePeriodType == CodeMaster.TimeUnit.Quarter)
            {
                int month = startTime.Date.Month;
                if (month < 4)
                {
                    nextStartTime = new DateTime(startTime.Date.Year, 3, 1);
                }
                else if (month < 7)
                {
                    nextStartTime = new DateTime(startTime.Date.Year, 6, 1);
                }
                else if (month < 10)
                {
                    nextStartTime = new DateTime(startTime.Date.Year, 9, 1);
                }
                else
                {
                    nextStartTime = new DateTime(startTime.Date.Year, 12, 1);
                }

                nextStartTime = nextStartTime.AddMonths(1);
            }
            //下一年
            else if (timePeriodType == CodeMaster.TimeUnit.Year)
            {
                nextStartTime = GetStartTime(timePeriodType, startTime).AddYears(1);
            }
            //下一小时
            else if (timePeriodType == CodeMaster.TimeUnit.Hour)
            {
                nextStartTime = startTime.AddHours(1);
            }

            return nextStartTime;
        }

        public static DateTime GetWeekStart(DateTime date)
        {
            int dayOfWeek = (int)(date.AddDays(-_weekStartDay).DayOfWeek);

            return date.Date.AddDays(-(double)dayOfWeek);
        }

        public static string GetQuarter(DateTime date)
        {
            int month = date.Month;
            if (month < 4)
                return "Q1";
            else if (month < 7)
                return "Q2";
            else if (month < 10)
                return "Q3";
            else
                return "Q4";
        }

        public static int GetQuarterIndex(DateTime date)
        {
            int month = date.Month;
            if (month < 4)
                return 1;
            else if (month < 7)
                return 2;
            else if (month < 10)
                return 3;
            else
                return 4;
        }

        public static int GetWeekIndex(DateTime date)
        {
            //找到第一周的最后一天
            //一.第一周周末是几号
            DateTime firstWeekend = GetEndTime(CodeMaster.TimeUnit.Week, DateTime.Parse(date.Year + "-1-1"));

            //二.获取当前时间和第一周周末相差多少天
            double diffDay = (date - firstWeekend).TotalDays;

            //三.今天 减去 第一周周末）/7 等于 距第一周有多少周 再加上第一周的1 就是今天是今年的第几周了
            return Convert.ToInt32(Math.Ceiling(diffDay / 7.0)) + 1;
        }

        public static bool CompareDateTime(DateTime dateTime1, DateTime dateTime2, CodeMaster.TimeUnit periodType)
        {
            dateTime1 = GetStartTime(periodType, dateTime1);

            dateTime2 = GetStartTime(periodType, dateTime2);

            if (dateTime1 == dateTime2)
            {
                return true;
            }
            return false;
        }

        /// <summary>    
        ///当前月有多少天 
        /// </summary> 
        /// <param name="y"></param> 
        /// <param name="m"></param> 
        /// <returns></returns> 
        public static int HowMonthDay(int y, int m)
        {
            int mnext;
            int ynext;
            if (m < 12)
            {
                mnext = m + 1;
                ynext = y;
            }
            else
            {
                mnext = 1;
                ynext = y + 1;
            }
            DateTime dt1 = System.Convert.ToDateTime(y + "-" + m + "-1");
            DateTime dt2 = System.Convert.ToDateTime(ynext + "-" + mnext + "-1");
            TimeSpan diff = dt2 - dt1;
            return diff.Days;
        }


        private static string GetWeekOfYear(DateTime dateTime, int weekIndex)
        {
            if (weekIndex > 52)
            {
                weekIndex = weekIndex - 52;
                return dateTime.AddYears(1).ToString("yyyy") + "-" + (weekIndex).ToString("D2");
            }
            else
            {
                return dateTime.ToString("yyyy") + "-" + weekIndex.ToString("D2");
            }
        }
        /// <summary>
        /// return 2012-25
        /// </summary>
        public static string GetWeekOfYear(DateTime curDay)
        {
            int weekIndex = GetWeekIndex(curDay);
            return GetWeekOfYear(curDay, weekIndex);
        }

        public static DateTime GetWeekIndexDateFrom(string weekOfYear)
        {
            string[] wk = weekOfYear.Split('-');
            DateTime dateTime = new DateTime(int.Parse(wk[0]), 1, 1);
            dateTime = GetStartTime(CodeMaster.TimeUnit.Week, dateTime);
            CultureInfo ci = new System.Globalization.CultureInfo("zh-CN");
            dateTime = ci.Calendar.AddWeeks(dateTime, int.Parse(wk[1]) - 1);
            return dateTime;
        }


        /// <summary>
        /// 获取指定周的开始时间 
        /// </summary>
        /// <param name="weekOfYear">如：2012-45,2012年第45周</param>
        /// <returns>时间</returns>
        public static DateTime GetWeekIndexDateTo(string weekOfYear)
        {
            return GetWeekIndexDateFrom(weekOfYear).AddDays(7).AddMilliseconds(-1);
        }
        
        public static double TimeTranfer(decimal sourceTime, CodeMaster.TimeUnit sourceTimeUnit, CodeMaster.TimeUnit targetTimeUnit)
        {
            if (sourceTimeUnit == targetTimeUnit)
            {
                return (double)sourceTime;
            }

            switch (sourceTimeUnit)
            {
                case CodeMaster.TimeUnit.Second:
                    break;
                case CodeMaster.TimeUnit.Minute:
                    sourceTime = sourceTime * 60;
                    break;
                case CodeMaster.TimeUnit.Hour:
                    sourceTime = sourceTime * 60 * 60;
                    break;
                case CodeMaster.TimeUnit.Day:
                    sourceTime = sourceTime * 60 * 60 * 24;
                    break;
                case CodeMaster.TimeUnit.Week:
                    sourceTime = sourceTime * 60 * 60 * 24 * 7;
                    break;
                case CodeMaster.TimeUnit.Month:
                    sourceTime = sourceTime * 60 * 60 * 24 * (DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    break;
                case CodeMaster.TimeUnit.Quarter:
                    sourceTime = sourceTime * 60 * 60 * 24 * 91;
                    break;
                case CodeMaster.TimeUnit.Year:
                    sourceTime = sourceTime * 60 * 60 * 24 * 365;
                    break;
                default:
                    throw new TechnicalException("not supported time unit");
            }

            switch (targetTimeUnit)
            {
                case CodeMaster.TimeUnit.Second:
                    return (double)sourceTime;
                case CodeMaster.TimeUnit.Minute:
                    return (double)sourceTime / 60;
                case CodeMaster.TimeUnit.Hour:
                    return (double)sourceTime / 60 / 60;
                case CodeMaster.TimeUnit.Day:
                    return (double)sourceTime / 60 / 60 / 24;
                case CodeMaster.TimeUnit.Week:
                    return (double)sourceTime / 60 / 60 / 24 / 7;
                case CodeMaster.TimeUnit.Month:
                    return (double)sourceTime / 60 / 60 / 24 / 30;
                case CodeMaster.TimeUnit.Quarter:
                    return (double)sourceTime / 60 / 60 / 24 / 91;
                case CodeMaster.TimeUnit.Year:
                    return (double)sourceTime / 60 / 60 / 24 / 365;
                default:
                    throw new TechnicalException("not supported time unit.");
            }
        }

    }
}
