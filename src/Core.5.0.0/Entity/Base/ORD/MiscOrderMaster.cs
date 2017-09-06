using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.ORD
{
    [Serializable]
    public partial class MiscOrderMaster : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        /// <summary>
        /// //�ƻ������ⵥ��
        /// </summary>
        [Export(ExportName = "ProductionAdjustMiscOrder", ExportSeq = 10)]
        [Export(ExportName = "ProductionReworkOrderMaster", ExportSeq = 10)]
        [Export(ExportName = "ProductionTrailMiscOrderMaster", ExportSeq = 10)]
        [Export(ExportName = "Master", ExportSeq = 10)]
        [Display(Name = "MiscOrderMstr_MiscOrderNo", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string MiscOrderNo { get; set; }

        /// <summary>
        /// //�ƻ�����������
        /// </summary>
        [Display(Name = "MiscOrderMstr_Type", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public com.Sconit.CodeMaster.MiscOrderType Type { get; set; }

        public com.Sconit.CodeMaster.MiscOrderSubType SubType { get; set; }


        /// <summary>
        /// //״̬
        /// </summary>
        [Display(Name = "MiscOrderMstr_Status", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public com.Sconit.CodeMaster.MiscOrderStatus Status { get; set; }

        /// <summary>
        /// //�Ƿ�ɨ������
        /// </summary>
        [Display(Name = "MiscOrderMstr_IsScanHu", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public Boolean IsScanHu { get; set; }

        /// <summary>
        /// //����״̬
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MiscOrderMstr_QualityType", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public CodeMaster.QualityType QualityType { get; set; }

        /// <summary>
        /// //�ƶ�����
        /// </summary>
        [Export(ExportName = "ProductionAdjustMiscOrder", ExportSeq = 50)]
        [Export(ExportName = "ProductionReworkOrderMaster", ExportSeq = 40)]
        [Export(ExportName = "ProductionTrailMiscOrderMaster", ExportSeq = 40)]
        [Export(ExportName = "Master", ExportSeq = 40)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MiscOrderMstr_MoveType", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string MoveType { get; set; }

        /// <summary>
        /// //����ʱ���ƶ�����
        /// </summary>
        public string CancelMoveType { get; set; }

        /// <summary>
        /// //����������Ӧ�����򣬿繤���ƿ�ʱ�����������
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MiscOrderMstr_Region", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string Region { get; set; }

        /// <summary>
        ///  //���ص��Ӧ�Ŀ�λ
        /// </summary>
        [Export(ExportName = "ProductionAdjustMiscOrder", ExportSeq = 40)]
        [Export(ExportName = "ProductionReworkOrderMaster", ExportSeq = 30)]
        [Export(ExportName = "ProductionTrailMiscOrderMaster", ExportSeq = 30)]
        [Export(ExportName = "Master", ExportSeq = 30)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MiscOrderMstr_Location", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]              
        public string Location { get; set; }

        /// <summary>
        /// //�ջ��ص㣬ΪSAP��λ���÷���
        /// </summary>
        [Display(Name = "MiscOrderMstr_ReceiveLocation", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string ReceiveLocation { get; set; }

        /// <summary>
        /// //�ƶ�ԭ��
        /// </summary>
        [Export(ExportName = "ProductionAdjustMiscOrder", ExportSeq = 70)]
        [Export(ExportName = "ProductionReworkOrderMaster", ExportSeq = 60)]
        [Export(ExportName = "ProductionTrailMiscOrderMaster", ExportSeq = 70)]
        [Export(ExportName = "Master", ExportSeq = 80)]
        [Display(Name = "MiscOrderMstr_Note", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string Note { get; set; }

        [Display(Name = "MiscOrderMstr_GeneralLedger", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string GeneralLedger { get; set; }

        /// <summary>
        /// //�ɱ�����
        /// </summary>
        [Export(ExportName = "Master", ExportSeq = 60)]
        [Display(Name = "MiscOrderMstr_CostCenter", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string CostCenter { get; set; }

        /// <summary>
        /// //����֪ͨ
        /// </summary>
        [Display(Name = "MiscOrderMstr_Asn", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string Asn { get; set; }

        /// <summary>
        /// //�ڲ�������
        /// </summary>
        [Display(Name = "MiscOrderMstr_ReferenceNo", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string ReferenceNo { get; set; }

        /// <summary>
        /// �繤���ƿ�ʱ,�����ջ�����
        /// �������룬ΪSAP���룬�ǿ繤���ƿ�ʱ�����Region�ֶ��ҵ���Ӧ��Plant����ֶ�����
        /// </summary>
        [Display(Name = "MiscOrderMstr_DeliverRegion", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string DeliverRegion { get; set; }

        /// <summary>
        /// WBSԪ��
        /// </summary>
        [Display(Name = "MiscOrderMstr_WBS", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string WBS { get; set; }
        public string WBSRow { get; set; }
        [Export(ExportName = "ProductionAdjustMiscOrder", ExportSeq = 20)]
        [Export(ExportName = "ProductionReworkOrderMaster", ExportSeq = 20)]
        [Export(ExportName = "ProductionTrailMiscOrderMaster", ExportSeq = 20)]
        [Export(ExportName = "Master", ExportSeq = 20)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MiscOrderMstr_EffectiveDate", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public DateTime EffectiveDate { get; set; }

        //[Display(Name = "CreateUserId", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public Int32 CreateUserId { get; set; }
        [Export(ExportName = "ProductionAdjustMiscOrder", ExportSeq = 80)]
        [Export(ExportName = "ProductionReworkOrderMaster", ExportSeq = 70)]
        [Export(ExportName = "ProductionTrailMiscOrderMaster", ExportSeq = 80)]
        [Export(ExportName = "Master", ExportSeq = 90)]
        [Display(Name = "MiscOrderMstr_CreateUserName", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string CreateUserName { get; set; }
        [Export(ExportName = "ProductionAdjustMiscOrder", ExportSeq = 90)]
        [Export(ExportName = "ProductionReworkOrderMaster", ExportSeq = 80)]
        [Export(ExportName = "ProductionTrailMiscOrderMaster", ExportSeq = 90)]
        [Export(ExportName = "Master", ExportSeq = 100)]
        [Display(Name = "MiscOrderMstr_CreateDate", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public DateTime CreateDate { get; set; }

        //[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public Int32 LastModifyUserId { get; set; }

        //[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string LastModifyUserName { get; set; }

        //[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public DateTime LastModifyDate { get; set; }

        public DateTime? CloseDate { get; set; }

        public Int32? CloseUserId { get; set; }

        public string CloseUserName { get; set; }

        public DateTime? CancelDate { get; set; }

        public Int32? CancelUserId { get; set; }

        public string CancelUserName { get; set; }

        //[Display(Name = "Version", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public Int32 Version { get; set; }
        public string WMSNo { get; set; }
        [Export(ExportName = "ProductionTrailMiscOrderMaster", ExportSeq = 60, ExportTitle = "OrderMaster_Flow_Production", ExportTitleResourceType = typeof(@Resources.ORD.OrderMaster))]
        [Export(ExportName = "ProductionReworkOrderMaster", ExportSeq = 50,ExportTitle = "OrderMaster_Flow_Production", ExportTitleResourceType = typeof(@Resources.ORD.OrderMaster))]
        [Display(Name = "MiscOrderMstr_Flow", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public string Flow { get; set; }
        [Export(ExportName = "Master", ExportSeq = 70)]
        [Display(Name = "MiscOrderMstr_IsCs", ResourceType = typeof(Resources.ORD.MiscOrderMstr))]
        public bool IsCs { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (MiscOrderNo != null)
            {
                return MiscOrderNo.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            MiscOrderMaster another = obj as MiscOrderMaster;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.MiscOrderNo == another.MiscOrderNo);
            }
        }
    }

}
