using System;
using System.Collections;
using System.Collections.Generic;

namespace com.Sconit.Entity.MRP.VIEW
{
    public class RccpExView
    {
        public string Region { get; set; }
        public string ProductLine { get; set; }
        public string Item { get; set; }
        public string DateIndex { get; set; }
        public CodeMaster.TimeUnit DateType { get; set; }

        public Double Speed { get; set; }
        public CodeMaster.ApsPriorityType ApsPriority { get; set; }
        public double Quota { get; set; }
        public Int32 SpeedTimes { get; set; }

        public Double SwitchTime { get; set; }
        public double EconomicLotSize { get; set; }

        public Double ConvSpeed { get; set; }
        public Double ScrapPercentage { get; set; }

        //public double MinLotSize { get; set; }
        //public double MaxLotSize { get; set; }

        //public double ScrapPercent { get; set; }
        //public Int32 TurnQty { get; set; }
        //public double Correction { get; set; }

        public Double Qty { get; set; }
        //总可用时间
        public Double UpTime { get; set; }

        public Int32 TotalAps { get; set; }

        //public DateTime DateFrom { get; set; }
        //public DateTime DateTo { get; set; }

        public Double RequireTime { get; set; }

        //辅助字段
        public Double Correction { get; set; }
    
    }
}
