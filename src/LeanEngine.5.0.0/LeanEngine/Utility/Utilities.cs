using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeanEngine.Entity;

namespace LeanEngine.Utility
{
    public static class Utilities
    {
        /// <summary>
        /// String comparer,ignore case
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool StringEq(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        public static DateTime GetStartTime(DateTime windowTime, double leadTime, List<RestTime> restTimeList)
        {
            return NestCalStartTime(windowTime.AddHours(-leadTime), windowTime, restTimeList);
        }

        public static DateTime GetNextRollingWindowTime(DateTime windowTime, double timeInterval, List<RestTime> restTimeList)
        {
            return NestCalWindowTime(windowTime, windowTime.AddHours(timeInterval), restTimeList);
        }

        private static DateTime NestCalStartTime(DateTime startTime, DateTime baseTime, List<RestTime> restTimeList)
        {
            if (startTime == baseTime)
            {
                return startTime;
            }
            if (restTimeList != null && restTimeList.Count > 0)
            {
                List<RestTime> restTimes = restTimeList.Where(rt => rt.StartTime < baseTime
                    && rt.EndTime > startTime).ToList();

                if (restTimes != null && restTimes.Count > 0)
                {
                    DateTime newStartTime = new DateTime(startTime.Ticks);
                    foreach (RestTime rt in restTimes)
                    {
                        newStartTime = newStartTime.Add(-(rt.EndTime > baseTime ? baseTime : rt.EndTime).Subtract((rt.StartTime > startTime ? rt.StartTime : startTime)));
                    }

                    startTime = NestCalStartTime(newStartTime, startTime, restTimeList);
                }
            }

            return startTime;
        }

        private static DateTime NestCalWindowTime(DateTime baseTime, DateTime windowTime, List<RestTime> restTimeList)
        {
            if (restTimeList != null && restTimeList.Count > 0)
            {
                List<RestTime> restTimes = restTimeList.Where(rt => rt.EndTime > baseTime
                    && rt.StartTime < windowTime).ToList();

                if (restTimes != null && restTimes.Count > 0)
                {
                    DateTime nextWindowTime = new DateTime(windowTime.Ticks);
                    foreach (RestTime rt in restTimes)
                    {
                        nextWindowTime = nextWindowTime.Add((rt.EndTime > windowTime ? windowTime : rt.EndTime).Subtract((rt.StartTime > baseTime ? rt.StartTime : baseTime)));
                    }

                    windowTime = NestCalWindowTime(windowTime, nextWindowTime, restTimeList);
                }
            }

            return windowTime;
        }
    }
}
