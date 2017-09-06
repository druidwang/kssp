using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.VIEW
{
    public class LocationDetailIOB
    {
        //LOC_INI = 100,          //库存初始化
        //ISS_SO = 101,           //销售出库
        //ISS_SO_VOID = 102,      //销售出库冲销
        //RCT_SO = 103,           //销售退货入库
        //RCT_SO_VOID = 104,      //销售退货入库冲销        
        //RCT_PO = 201,           //采购入库
        //RCT_PO_VOID = 202,      //采购入库冲销
        //ISS_PO = 203,           //采购退货
        //ISS_PO_VOID = 204,      //采购退货冲销
        //RCT_SL = 205,           //计划协议入库
        //RCT_SL_VOID = 206,      //计划协议入库冲销
        //ISS_SL = 207,           //计划协议退货
        //ISS_SL_VOID = 208,      //计划协议退货冲销
        //ISS_TR = 301,           //移库出库
        //ISS_TR_VOID = 302,      //移库出库冲销
        //RCT_TR = 303,           //移库入库
        //RCT_TR_VOID = 304,      //移库入库冲销
        //ISS_TR_RTN = 311,       //移库退货出库
        //ISS_TR_RTN_VOID = 312,  //移库退货出库冲销
        //RCT_TR_RTN = 313,       //移库退货入库
        //RCT_TR_RTN_VOID = 314,  //移库退货入库冲销
        //ISS_STR = 305,          //委外移库出库
        //ISS_STR_VOID = 306,     //委外移库出库冲销
        //RCT_STR = 307,          //委外移库入库
        //RCT_STR_VOID = 308,     //委外移库入库冲销
        //ISS_STR_RTN = 315,      //委外移库退货出库
        //ISS_STR_RTN_VOID = 316, //委外移库退货出库冲销
        //RCT_STR_RTN = 317,      //委外移库退货入库
        //RCT_STR_RTN_VOID = 318, //委外移库退货入库冲销
        //ISS_WO = 401,           //生产出库/原材料
        //ISS_WO_VOID = 402,      //生产出库/原材料冲销
        //ISS_WO_BF = 403,        //生产投料回冲出库/出生产线
        //ISS_WO_BF_VOID = 404,   //生产投料回冲出库/出生产线冲销
        //RCT_WO = 405,           //生产入库/成品
        //RCT_WO_VOID = 406,      //生产入库/成品冲销
        //ISS_MIN = 407,          //生产投料出库
        //ISS_MIN_RTN = 408,      //生产投料退库出库
        //RCT_MIN = 409,          //生产投料入库/入生产线
        //RCT_MIN_RTN = 410,      //生产投料出库/出生产线
        //ISS_SWO = 411,          //委外生产出库/原材料
        //ISS_SWO_VOID = 412,     //委外生产出库/原材料冲销
        //ISS_SWO_BF = 413,       //委外生产投料回冲出库/出生产线
        //ISS_SWO_BF_VOID = 414,  //委外生产投料回冲出库/出生产线冲销
        //RCT_SWO = 415,          //委外生产入库/成品
        //RCT_SWO_VOID = 416,     //委外生产入库/成品冲销
        //ISS_INP = 501,          //报验出库
        //RCT_INP = 502,          //报验入库
        //ISS_ISL = 503,          //隔离出库
        //RCT_ISL = 504,          //隔离入库
        //ISS_INP_QDII = 505,     //检验合格出库 
        //RCT_INP_QDII = 506,     //检验合格入库 
        //ISS_INP_REJ = 507,      //检验不合格出库 
        //RCT_INP_REJ = 508,      //检验不合格入库 
        //ISS_INP_CCS = 509,      //让步使用出库
        //RCT_INP_CCS = 510,      //让步使用入库
        //CYC_CNT = 601,          //盘点差异出入库
        //CYC_CNT_VOID = 602,     //盘点差异出入库
        //ISS_UNP = 603,          //计划外出库
        //ISS_UNP_VOID = 604,     //计划外出库冲销
        //RCT_UNP = 605,          //计划外入库
        //RCT_UNP_VOID = 606,     //计划外入库冲销
        //ISS_REP = 607,          //翻箱出库
        //RCT_REP = 608,          //翻箱入库
        //ISS_PUT = 609,          //上架出库
        //RCT_PUT = 610,          //上架入库
        //ISS_PIK = 611,          //下架出库
        //RCT_PIK = 612,          //下架入库
        //ISS_IIC = 613,          //库存物料替换出库
        //ISS_IIC_VOID = 614,     //库存物料替换出库冲销
        //RCT_IIC = 615,          //库存物料替换入库
        //RCT_IIC_VOID = 616,     //库存物料替换入库冲销

        ////客户化
        //ISS_AGE = 901,          //老化出库
        //ISS_AGE_VOID = 902,     //老化出库冲销
        //RCT_AGE = 903,          //老化入库
        //RCT_AGE_VOID = 904,     //老化入库冲销
        //ISS_FLT = 905,          //过滤出库
        //ISS_FLT_VOID = 906,     //过滤出库冲销
        //RCT_FLT = 907,          //过滤入库
        //RCT_FLT_VOID = 908,     //过滤入库冲销
        [Display(Name = "LocationDetailIOB_Item", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public string Item { get; set; }
        [Display(Name = "LocationDetailIOB_ItemDescription", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public string ItemDescription { get; set; }
        [Display(Name = "LocationDetailIOB_Location", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public string Location { get; set; }
        [Display(Name = "LocationDetailIOB_LocationName", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public string LocationName { get; set; }
        [Display(Name = "LocationDetailIOB_Uom", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public string Uom { get; set; }
        [Display(Name = "Item_MaterialsGroup", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroup { get; set; }
        [Display(Name = "Item_MaterialsGroupDesc", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroupDesc { get; set; }
        /// <summary>
        /// 期初
        /// </summary>
        [Display(Name = "LocationDetailIOB_Start", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal Start { get; set; }//期初总数量

        /// <summary>
        /// 期初寄售
        /// </summary>
        [Display(Name = "LocationDetailIOB_StartCs", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal StartCs { get; set; } 
        /// <summary>
        /// 合格
        /// </summary>
        [Display(Name = "LocationDetailIOB_StartNml", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal StartNml { get; set; }//期初正常

        /// <summary>
        /// 待验
        /// </summary>
        [Display(Name = "LocationDetailIOB_StartInp", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal StartInp { get; set; }//期初待验

        /// <summary>
        /// 不合格
        /// </summary>
        [Display(Name = "LocationDetailIOB_StartRej", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal StartRej { get; set; }//期初不合格

        /// <summary>
        /// 采购入
        /// </summary>
        [Display(Name = "LocationDetailIOB_RctPo", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal RctPo { get; set; }

        /// <summary>
        /// 生产入
        /// </summary>
        [Display(Name = "LocationDetailIOB_RctWo", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal RctWo { get; set; }

        /// <summary>
        /// 委外入
        /// </summary>
        [Display(Name = "LocationDetailIOB_RctSwo", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal RctSwo { get; set; }

        /// <summary>
        /// 移库入
        /// </summary>
        [Display(Name = "LocationDetailIOB_RctTr", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal RctTr { get; set; }

        /// <summary>
        /// 委外移库入
        /// </summary>
        [Display(Name = "LocationDetailIOB_RctStr", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal RctStr { get; set; }

        /// <summary>
        /// 计划外入
        /// </summary>
        [Display(Name = "LocationDetailIOB_RctUnp", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal RctUnp { get; set; }

        /// <summary>
        /// 销售退货入
        /// </summary>
        [Display(Name = "LocationDetailIOB_RctSo", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal RctSo { get; set; }

        /// <summary>
        /// 物料替换入
        /// </summary>
        [Display(Name = "LocationDetailIOB_RctIic", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal RctIic { get; set; }//物料替换,过滤


        /// <summary>
        /// 销售出
        /// </summary>
        [Display(Name = "LocationDetailIOB_IssSo", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal IssSo { get; set; }

        /// <summary>
        /// 生产出
        /// </summary>
        [Display(Name = "LocationDetailIOB_IssWo", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal IssWo { get; set; }

        /// <summary>
        /// 委外生产出
        /// </summary>
        [Display(Name = "LocationDetailIOB_IssSwo", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal IssSwo { get; set; }

        /// <summary>
        /// 移库出
        /// </summary>
        [Display(Name = "LocationDetailIOB_IssTr", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal IssTr { get; set; }

        /// <summary>
        /// 委外移库出
        /// </summary>
        [Display(Name = "LocationDetailIOB_IssStr", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal IssStr { get; set; }

        /// <summary>
        /// 计划外出
        /// </summary>
        [Display(Name = "LocationDetailIOB_IssUnp", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal IssUnp { get; set; }

        /// <summary>
        /// 采购退货出
        /// </summary>
        [Display(Name = "LocationDetailIOB_IssPo", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal IssPo { get; set; }

        /// <summary>
        /// 物料替换出
        /// </summary>
        [Display(Name = "LocationDetailIOB_IssIic", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal IssIic { get; set; }//物料替换,过滤

        //[Display(Name = "LocationDetailIOB_Inp", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        ////质量
        ///// <summary>
        ///// 报验
        ///// </summary>
        //public decimal Inp { get; set; }//报验
        //[Display(Name = "LocationDetailIOB_Qdii", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        ///// <summary>
        ///// 合格
        ///// </summary>
        //public decimal Qdii { get; set; }//合格
        //[Display(Name = "LocationDetailIOB_Rej", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        ///// <summary>
        ///// 不合格
        ///// </summary>
        //public decimal Rej { get; set; }//不合格
        //[Display(Name = "LocationDetailIOB_Ccs", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        ///// <summary>
        ///// 让步
        ///// </summary>
        //public decimal Ccs { get; set; }//让步

        /// <summary>
        /// 盘点
        /// </summary>
        [Display(Name = "LocationDetailIOB_CycCnt", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal CycCnt { get; set; }

        /// <summary>
        /// 其他
        /// </summary>
        [Display(Name = "LocationDetailIOB_Other", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal Other { get; set; }//未统计:库存初始化等

        /// <summary>
        /// 期末
        /// </summary>
        [Display(Name = "LocationDetailIOB_End", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal End { get; set; }

        /// <summary>
        /// 期末寄售
        /// </summary>
        [Display(Name = "LocationDetailIOB_EndCs", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal EndCs { get; set; }

        /// <summary>
        /// 期末合格
        /// </summary>
        [Display(Name = "LocationDetailIOB_EndNml", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal EndNml { get; set; }//期末正常

        /// <summary>
        /// 期末待验
        /// </summary>
        [Display(Name = "LocationDetailIOB_EndInp", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal EndInp { get; set; }//期末待验

        /// <summary>
        /// 期末不合格
        /// </summary>
        [Display(Name = "LocationDetailIOB_EndRej", ResourceType = typeof(Resources.Report.LocationDetailIOB))]
        public decimal EndRej { get; set; }//期末不合格
    }
}
