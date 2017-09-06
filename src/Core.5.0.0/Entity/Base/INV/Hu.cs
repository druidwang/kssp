using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.INV
{
    [Serializable]
    public partial class Hu : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Export(ExportName = "Ageing", ExportSeq = 3)]
        [Export(ExportName = "ShelfLifeWarning", ExportSeq = 10)] 
        [Display(Name = "Hu_HuId", ResourceType = typeof(Resources.INV.Hu))]
        public string HuId { get; set; }
        [Export(ExportName = "Ageing", ExportSeq = 60, ExportTitle = "Hu_manufacture_date", ExportTitleResourceType = typeof(Resources.INV.Hu))]
        [Export(ExportName = "ShelfLifeWarning", ExportSeq = 50, ExportTitle = "LocationLotDetail_ManufactureDate", ExportTitleResourceType = typeof(Resources.INV.LocationLotDetail))]
        [Display(Name = "Hu_lotNo", ResourceType = typeof(Resources.INV.Hu))]
        public string LotNo { get; set; }
        [Export(ExportName = "Ageing", ExportSeq = 10)]
        [Export(ExportName = "AgeingSumByLocation", ExportSeq = 10)]
        [Export(ExportName = "ShelfLifeWarning", ExportSeq = 20)]
        [Export(ExportName = "OutOfExpireTimeWarning", ExportSeq = 10)]
        [Export(ExportName = "ShelfLifeWarningSummary", ExportSeq = 10)] 
        [Display(Name = "Hu_Item", ResourceType = typeof(Resources.INV.Hu))]
        public string Item { get; set; }
        [Export(ExportName = "Ageing", ExportSeq = 20)]
        [Export(ExportName = "AgeingSumByLocation", ExportSeq = 20)]
        [Export(ExportName = "ShelfLifeWarning", ExportSeq = 40)]
        [Export(ExportName = "OutOfExpireTimeWarning", ExportSeq = 20)]
        [Export(ExportName = "ShelfLifeWarningSummary", ExportSeq = 20)] 
        [Display(Name = "Hu_ItemDescription", ResourceType = typeof(Resources.INV.Hu))]
        public string ItemDescription { get; set; }
        [Export(ExportName = "Ageing", ExportSeq = 30)]
        [Export(ExportName = "ShelfLifeWarning", ExportSeq = 30)] 
        [Display(Name = "Hu_ReferenceItemCode", ResourceType = typeof(Resources.INV.Hu))]
        public string ReferenceItemCode { get; set; }
        [Export(ExportName = "Ageing", ExportSeq = 70)]
        [Export(ExportName = "ShelfLifeWarning", ExportSeq = 70)] 
        [Display(Name = "Hu_Uom", ResourceType = typeof(Resources.INV.Hu))]
        public string Uom { get; set; }
        public string BaseUom { get; set; }
        [Export(ExportName = "ShelfLifeWarning", ExportSeq = 60)] 
        [Display(Name = "Hu_UnitCount", ResourceType = typeof(Resources.INV.Hu))]
        public Decimal UnitCount { get; set; }
        [Export(ExportName = "Ageing", ExportSeq = 80)]
        [Export(ExportName = "ShelfLifeWarning", ExportSeq = 80)]
        [Export(ExportName = "OutOfExpireTimeWarning", ExportSeq = 30)] 
        [Display(Name = "Hu_Qty", ResourceType = typeof(Resources.INV.Hu))]
        public Decimal Qty { get; set; }

        /// <summary>
        /// 单位用量
        /// </summary>
        [Display(Name = "Hu_UnitQty", ResourceType = typeof(Resources.INV.Hu))]//
        public Decimal UnitQty { get; set; }
        
        [Display(Name = "Hu_manufacture_date", ResourceType = typeof(Resources.INV.Hu))]
        public DateTime ManufactureDate { get; set; }
        [Display(Name = "Hu_ManufactureParty", ResourceType = typeof(Resources.INV.Hu))]
        public string ManufactureParty { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        //[Export(ExportName = "ShelfLifeWarning", ExportSeq = 100)] 
        [Display(Name = "Hu_ExpireDate", ResourceType = typeof(Resources.INV.Hu))]
        public DateTime? ExpireDate { get; set; }
        //[Export(ExportName = "ShelfLifeWarning", ExportSeq = 90)] 
        [Display(Name = "Hu_RemindExpireDate", ResourceType = typeof(Resources.INV.Hu))]
        public DateTime? RemindExpireDate { get; set; }

        /// <summary>
        /// 打印次数
        /// </summary>
        [Display(Name = "Hu_PrintCount", ResourceType = typeof(Resources.INV.Hu))]
        public Int16 PrintCount { get; set; }

        [Display(Name = "Hu_FirstInventoryDate", ResourceType = typeof(Resources.INV.Hu))]
        public DateTime? FirstInventoryDate { get; set; }
        [Display(Name = "Hu_IsOdd", ResourceType = typeof(Resources.INV.Hu))]
        public Boolean IsOdd { get; set; }
        public Int32 CreateUserId { get; set; }
        [Display(Name = "Hu_CreateUserName", ResourceType = typeof(Resources.INV.Hu))]
        public string CreateUserName { get; set; }
        [Display(Name = "Hu_CreateDate", ResourceType = typeof(Resources.INV.Hu))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Hu_LastModifyUserName", ResourceType = typeof(Resources.INV.Hu))]
        public string LastModifyUserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Hu_LastModifyDate", ResourceType = typeof(Resources.INV.Hu))]
        public DateTime LastModifyDate { get; set; }
        //[Display(Name = "Version", ResourceType = typeof(Resources.INV.Hu))]
        //public Int32 Version { get; set; }
        /// <summary>
        /// 让步次数
        /// </summary>
        [Display(Name = "Hu_ConcessionCount", ResourceType = typeof(Resources.INV.Hu))]
        public Int16 ConcessionCount { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        [Display(Name = "Hu_OrderNo", ResourceType = typeof(Resources.INV.Hu))]
        public string OrderNo { get; set; }
        /// <summary>
        /// 收货单号
        /// </summary>
        [Display(Name = "Hu_ReceiptNo", ResourceType = typeof(Resources.INV.Hu))]
        public string ReceiptNo { get; set; }
        [Display(Name = "Hu_SupplierLotNo", ResourceType = typeof(Resources.INV.Hu))]
        public string SupplierLotNo { get; set; }
        /// <summary>
        /// 容器描述
        /// </summary>
        [Display(Name = "Hu_ContainerDesc", ResourceType = typeof(Resources.INV.Hu))]
        public string ContainerDesc { get; set; }

        [Display(Name = "Hu_LocationTo", ResourceType = typeof(Resources.INV.Hu))]
        public string LocationTo { get; set; }
        /// <summary>
        /// 是否允许修改包装数
        /// </summary>
        [Display(Name = "Hu_IsChangeUnitCount", ResourceType = typeof(Resources.INV.Hu))]
        public Boolean IsChangeUnitCount { get; set; }
        /// <summary>
        /// 包装描述
        /// </summary>
        [Display(Name = "Hu_UnitCountDescription", ResourceType = typeof(Resources.INV.Hu))]
        public string UnitCountDescription { get; set; }
        /// <summary>
        /// 库位
        /// </summary>
        public string LocTo { get; set; }
        //public Int32 Id { get; set; }
        [Display(Name = "Hu_HuTemplate", ResourceType = typeof(Resources.INV.Hu))]
        public string HuTemplate { get; set; }
        #endregion

        /// <summary>
        /// 外部条码号
        /// </summary>
        public string ExternalHuId { get; set; }


        /// <summary>
        /// 是否外部条码
        /// </summary>
        public Boolean IsExternal { get; set; }


        /// <summary>
        /// 是否托盘
        /// </summary>
        public Boolean IsPallet { get; set; }


        #region

        /// <summary>
        /// 老化属性:不需老化 0 / 未老化 1 / 已老化 2
        /// 条码选项
        /// </summary>
        [Display(Name = "Hu_HuOption", ResourceType = typeof(Resources.INV.Hu))]
        public CodeMaster.HuOption HuOption { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "Hu_Remark", ResourceType = typeof(Resources.INV.Hu))]
        public string Remark { get; set; }
        /// <summary>
        /// 去向
        /// </summary>
        [Display(Name = "Hu_Direction", ResourceType = typeof(Resources.INV.Hu))]
        public string Direction { get; set; }
        /// <summary>
        /// 路线
        /// </summary>
        [Display(Name = "Hu_Flow", ResourceType = typeof(Resources.INV.Hu))]
        public string Flow { get; set; }
        /// <summary>
        /// 班次
        /// </summary>
        [Display(Name = "Hu_Shift", ResourceType = typeof(Resources.INV.Hu))]
        public string Shift { get; set; }

        /// <summary>
        /// 送货单号
        /// </summary>
        [Display(Name = "Hu_IpNo", ResourceType = typeof(Resources.INV.Hu))]
        public string IpNo { get; set; }
        /// <summary>
        /// 试制
        /// </summary>
        [Display(Name = "Hu_ItemVersion", ResourceType = typeof(Resources.INV.Hu))]
        public string ItemVersion { get; set; }
        /// <summary>
        /// 原条码号
        /// </summary>
        [Display(Name = "Hu_RefHu", ResourceType = typeof(Resources.INV.Hu))]
        public string RefHu { get; set; }
        /// <summary>
        /// 物料组
        /// </summary>
        [Export(ExportName = "ShelfLifeWarning", ExportSeq = 43)]
        [Export(ExportName = "OutOfExpireTimeWarning", ExportSeq = 23)]
        [Export(ExportName = "ShelfLifeWarningSummary", ExportSeq = 30)] 
        [Display(Name = "Hu_MaterialsGroup", ResourceType = typeof(Resources.INV.Hu))]
        public string MaterialsGroup { get; set; }
        /// <summary>
        /// 开始老化时间
        /// </summary>
        [Export(ExportName = "Ageing", ExportSeq = 90)]
        [Display(Name = "Hu_AgingStartTime", ResourceType = typeof(Resources.INV.Hu))]
        public DateTime? AgingStartTime { get; set; }
        /// <summary>
        /// 结束老化时间
        /// </summary>
        [Export(ExportName = "Ageing", ExportSeq = 80)]
        [Display(Name = "Hu_AgingEndTime", ResourceType = typeof(Resources.INV.Hu))]
        public DateTime? AgingEndTime { get; set; }

        public Int32 RefId { get; set; }

        /// <summary>
        /// 托盘编号
        /// </summary>
        [Display(Name = "Hu_PalletCode", ResourceType = typeof(Resources.INV.Hu))]
        public string PalletCode { get; set; }



        [Display(Name = "Hu_ExternalOrderNo", ResourceType = typeof(Resources.INV.Hu))]
        public string ExternalOrderNo { get; set; }


        #endregion


        public override int GetHashCode()
        {
            if (HuId != null)
            {
                return HuId.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }
        //用来做checkbox的头
        public string CheckHuId { get; set; }

        public override bool Equals(object obj)
        {
            Hu another = obj as Hu;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.HuId == another.HuId);
            }
        }
    }

}
