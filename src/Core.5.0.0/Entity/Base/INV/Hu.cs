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
        /// ��λ����
        /// </summary>
        [Display(Name = "Hu_UnitQty", ResourceType = typeof(Resources.INV.Hu))]//
        public Decimal UnitQty { get; set; }
        
        [Display(Name = "Hu_manufacture_date", ResourceType = typeof(Resources.INV.Hu))]
        public DateTime ManufactureDate { get; set; }
        [Display(Name = "Hu_ManufactureParty", ResourceType = typeof(Resources.INV.Hu))]
        public string ManufactureParty { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        //[Export(ExportName = "ShelfLifeWarning", ExportSeq = 100)] 
        [Display(Name = "Hu_ExpireDate", ResourceType = typeof(Resources.INV.Hu))]
        public DateTime? ExpireDate { get; set; }
        //[Export(ExportName = "ShelfLifeWarning", ExportSeq = 90)] 
        [Display(Name = "Hu_RemindExpireDate", ResourceType = typeof(Resources.INV.Hu))]
        public DateTime? RemindExpireDate { get; set; }

        /// <summary>
        /// ��ӡ����
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
        /// �ò�����
        /// </summary>
        [Display(Name = "Hu_ConcessionCount", ResourceType = typeof(Resources.INV.Hu))]
        public Int16 ConcessionCount { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        [Display(Name = "Hu_OrderNo", ResourceType = typeof(Resources.INV.Hu))]
        public string OrderNo { get; set; }
        /// <summary>
        /// �ջ�����
        /// </summary>
        [Display(Name = "Hu_ReceiptNo", ResourceType = typeof(Resources.INV.Hu))]
        public string ReceiptNo { get; set; }
        [Display(Name = "Hu_SupplierLotNo", ResourceType = typeof(Resources.INV.Hu))]
        public string SupplierLotNo { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [Display(Name = "Hu_ContainerDesc", ResourceType = typeof(Resources.INV.Hu))]
        public string ContainerDesc { get; set; }

        [Display(Name = "Hu_LocationTo", ResourceType = typeof(Resources.INV.Hu))]
        public string LocationTo { get; set; }
        /// <summary>
        /// �Ƿ������޸İ�װ��
        /// </summary>
        [Display(Name = "Hu_IsChangeUnitCount", ResourceType = typeof(Resources.INV.Hu))]
        public Boolean IsChangeUnitCount { get; set; }
        /// <summary>
        /// ��װ����
        /// </summary>
        [Display(Name = "Hu_UnitCountDescription", ResourceType = typeof(Resources.INV.Hu))]
        public string UnitCountDescription { get; set; }
        /// <summary>
        /// ��λ
        /// </summary>
        public string LocTo { get; set; }
        //public Int32 Id { get; set; }
        [Display(Name = "Hu_HuTemplate", ResourceType = typeof(Resources.INV.Hu))]
        public string HuTemplate { get; set; }
        #endregion

        /// <summary>
        /// �ⲿ�����
        /// </summary>
        public string ExternalHuId { get; set; }


        /// <summary>
        /// �Ƿ��ⲿ����
        /// </summary>
        public Boolean IsExternal { get; set; }


        /// <summary>
        /// �Ƿ�����
        /// </summary>
        public Boolean IsPallet { get; set; }


        #region

        /// <summary>
        /// �ϻ�����:�����ϻ� 0 / δ�ϻ� 1 / ���ϻ� 2
        /// ����ѡ��
        /// </summary>
        [Display(Name = "Hu_HuOption", ResourceType = typeof(Resources.INV.Hu))]
        public CodeMaster.HuOption HuOption { get; set; }
        /// <summary>
        /// ��ע
        /// </summary>
        [Display(Name = "Hu_Remark", ResourceType = typeof(Resources.INV.Hu))]
        public string Remark { get; set; }
        /// <summary>
        /// ȥ��
        /// </summary>
        [Display(Name = "Hu_Direction", ResourceType = typeof(Resources.INV.Hu))]
        public string Direction { get; set; }
        /// <summary>
        /// ·��
        /// </summary>
        [Display(Name = "Hu_Flow", ResourceType = typeof(Resources.INV.Hu))]
        public string Flow { get; set; }
        /// <summary>
        /// ���
        /// </summary>
        [Display(Name = "Hu_Shift", ResourceType = typeof(Resources.INV.Hu))]
        public string Shift { get; set; }

        /// <summary>
        /// �ͻ�����
        /// </summary>
        [Display(Name = "Hu_IpNo", ResourceType = typeof(Resources.INV.Hu))]
        public string IpNo { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [Display(Name = "Hu_ItemVersion", ResourceType = typeof(Resources.INV.Hu))]
        public string ItemVersion { get; set; }
        /// <summary>
        /// ԭ�����
        /// </summary>
        [Display(Name = "Hu_RefHu", ResourceType = typeof(Resources.INV.Hu))]
        public string RefHu { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        [Export(ExportName = "ShelfLifeWarning", ExportSeq = 43)]
        [Export(ExportName = "OutOfExpireTimeWarning", ExportSeq = 23)]
        [Export(ExportName = "ShelfLifeWarningSummary", ExportSeq = 30)] 
        [Display(Name = "Hu_MaterialsGroup", ResourceType = typeof(Resources.INV.Hu))]
        public string MaterialsGroup { get; set; }
        /// <summary>
        /// ��ʼ�ϻ�ʱ��
        /// </summary>
        [Export(ExportName = "Ageing", ExportSeq = 90)]
        [Display(Name = "Hu_AgingStartTime", ResourceType = typeof(Resources.INV.Hu))]
        public DateTime? AgingStartTime { get; set; }
        /// <summary>
        /// �����ϻ�ʱ��
        /// </summary>
        [Export(ExportName = "Ageing", ExportSeq = 80)]
        [Display(Name = "Hu_AgingEndTime", ResourceType = typeof(Resources.INV.Hu))]
        public DateTime? AgingEndTime { get; set; }

        public Int32 RefId { get; set; }

        /// <summary>
        /// ���̱��
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
        //������checkbox��ͷ
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
