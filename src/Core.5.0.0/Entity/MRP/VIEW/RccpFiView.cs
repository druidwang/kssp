using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity.MRP.TRANS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.VIEW
{
    public class RccpFiView
    {
        #region 数据来自RccpFiPlan的Key
        public string ProductLine { get; set; }
        public string DateIndex { get; set; }
        public CodeMaster.TimeUnit DateType { get; set; }
        //模具
        public string Machine { get; set; }

        //产量 需求
        public Double Qty { get; set; }
        //班次数
        public Double RequiredShiftQty { get; set; }
        #endregion

        #region 数据来自MachineInstance
        public int Seq { get; set; }
        //模具描述
        public string Description { get; set; }
        //岛区
        public string Island { get; set; }
        //岛区描述
        public string IslandDescription { get; set; }
        //岛区数量
        public Double IslandQty { get; set; }
        //模具数量
        public Double MachineQty { get; set; }
        //8小时班产定额
        public Double ShiftQuota { get; set; }
        
        //班制 2班/3班
        public com.Sconit.CodeMaster.ShiftType ShiftType { get; set; }
        //正常工作天数
        public Double NormalWorkDay { get; set; }
        //最大工作天数
        public Double MaxWorkDay { get; set; }

        //班次/天
        public int ShiftPerDay { get; set; }
        //模具类型
        public com.Sconit.CodeMaster.MachineType MachineType { get; set; }
        //停机时间
        public double HaltTime { get; set; }
        //试制时间
        public double TrialProduceTime { get; set; }
        //节假日
        public double Holiday { get; set; }
        #endregion
        //正常班次数
        public double NormalShiftQty { get; set; }
        //最大班次班次数
        public double MaxShiftQty { get; set; }
        //正常产量
        public double NormalQty { get; set; }
        //模具最大产量
        public double MaxQty { get; set; }
        //当前正常班次数
        public double CurrentNormalShiftQty { get; set; }
        //当前最大班次班次数
        public double CurrentMaxShiftQty { get; set; }
        //当前模具最大产量  
        public double CurrentMaxQty { get; set; }
        public double CurrentNormalQty { get; set; }

        //模具/岛区 需求数
        public double RequiredFactQty { get; set; }
        public double CurrentRequiredFactQty { get; set; }

        //差异数套
        public double DiffQty { get; set; }
        public double CurrentDiffQty { get; set; }
        //需要班次数
        public double RequiredShiftPerDay { get; set; }

        //一次生产的套数
        public double ItemCount { get; set; }
        //套数
        public double KitQty { get; set; }
        public double KitShiftQuota { get; set; }

        //车型组建
        public double ModelRate { get; set; }

        public List<RccpFiPlan> RccpFiPlanList { get; set; }

        public List<RccpFiView> RccpFiViewList { get; set; }

    }
}
