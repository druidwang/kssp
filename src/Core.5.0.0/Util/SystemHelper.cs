using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace com.Sconit.Utility
{
    public static class SystemHelper
    {
        public static bool IsInterate(decimal dl)
        {
            return Math.Ceiling(dl) == dl && Math.Floor(dl) == dl;
        }

        public static bool IsInterate(double db)
        {
            return Math.Ceiling(db) == db && Math.Floor(db) == db;
        }

        static object lockGetSystemUniqueCode = new object();
        public static string GetSystemUniqueCode()
        {
            lock (lockGetSystemUniqueCode)
            {
                Thread.Sleep(100);
                return DateTime.Now.ToString("yyyyMMddHHmmssff");
            }
        }

        //public static string ConvertSequence(string sequence)
        //{
        //    int inti = 55;
        //    if (sequence.Length > 2)
        //    {
        //        string tobeConvert = sequence.Substring(0, 2);
        //        return (char)(inti + int.Parse(tobeConvert)) + sequence.Substring(2, sequence.Length - 2);
        //    }
        //    else
        //    {
        //        return sequence;
        //    }
        //}

        //public static string ResolveSequence(string sequence)
        //{
        //    int inti = 55;
        //    if (sequence.Length > 2)
        //    {
        //        string tobeConvert = sequence.Substring(0, 2);
        //        return (char)(inti + int.Parse(tobeConvert)) + sequence.Substring(2, sequence.Length - 2);
        //    }
        //    else
        //    {
        //        return sequence;
        //    }
        //}

    }
}
