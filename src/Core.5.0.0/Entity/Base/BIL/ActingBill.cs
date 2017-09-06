using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.BIL
{
    [Serializable]
    public partial class ActingBill : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Display(Name = "ActingBill_Flow", ResourceType = typeof(Resources.BIL.ActingBill))]
        public string Flow { get; set; }

		public Int32 Id { get; set; }
        public Int32 PlanBill { get; set; }
        /// <summary>
        /// ������
        /// </summary>
		public string OrderNo { get; set; }
        /// <summary>
        /// �ͻ�����
        /// </summary>
		public string IpNo { get; set; }
        /// <summary>
        /// �ⲿ�ͻ�����
        /// </summary>
		public string ExternalIpNo { get; set; }
        /// <summary>
        /// �ջ�����
        /// </summary>
        [Export(ExportName = "SalesActBill", ExportSeq = 10)]
        [Export(ExportName = "ProcurementActBill", ExportSeq = 10)]
        [Display(Name = "ActingBill_ReceiptNo", ResourceType = typeof(Resources.BIL.ActingBill))]
		public string ReceiptNo { get; set; }
        /// <summary>
        /// �ⲿ�ջ�����
        /// </summary>
        [Export(ExportName = "SalesActBill", ExportSeq = 20)]
        [Display(Name = "ActingBill_ExternalReceiptNo", ResourceType = typeof(Resources.BIL.ActingBill))]
		public string ExternalReceiptNo { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public com.Sconit.CodeMaster.BillType Type { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [Export(ExportName = "SalesActBill", ExportSeq = 30)]
        [Export(ExportName = "ProcurementNoInvoice", ExportSeq = 20)]
        [Export(ExportName = "SalesNoInvoiceBill", ExportSeq = 15)]
        [Export(ExportName = "ProcurementActBill", ExportSeq = 20)]
        [Display(Name = "ActingBill_Item", ResourceType = typeof(Resources.BIL.ActingBill))]
        public string Item { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        [Export(ExportName = "SalesActBill", ExportSeq = 40)]
        [Export(ExportName = "ProcurementNoInvoice", ExportSeq = 30)]
        [Export(ExportName = "SalesNoInvoiceBill", ExportSeq = 30)]
        [Export(ExportName = "ProcurementActBill", ExportSeq = 30)]
        [Display(Name = "ActingBill_ItemDescription", ResourceType = typeof(Resources.BIL.ActingBill))]
        public string ItemDescription { get; set; }
        /// <summary>
        /// ��λ
        /// </summary>
        [Export(ExportName = "SalesActBill", ExportSeq = 60)]
        [Export(ExportName = "ProcurementNoInvoice", ExportSeq = 60)]
        [Export(ExportName = "SalesNoInvoiceBill", ExportSeq = 60)]
        [Export(ExportName = "ProcurementActBill", ExportSeq = 40)]
        [Display(Name = "ActingBill_Uom", ResourceType = typeof(Resources.BIL.ActingBill))]
		public string Uom { get; set; }
        /// <summary>
        /// ����װ
        /// </summary>
		public Decimal UnitCount { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public CodeMaster.OrderBillTerm BillTerm { get; set; }
        /// <summary>
        /// ��Ʊ��ַ
        /// </summary>
		public string BillAddress { get; set; }
        /// <summary>
        /// ��Ʊ��ַ����
        /// </summary>
		public string BillAddressDescription { get; set; }
        /// <summary>
        /// ��Ӧ��/�ͻ�
        /// </summary> 
        [Export(ExportName = "ProcurementNoInvoice", ExportSeq = 10, ExportTitle = "CodeDetail_PermissionCategoryType_Supplier", ExportTitleResourceType = typeof(Resources.SYS.CodeDetail))]
        [Export(ExportName = "SalesNoInvoiceBill", ExportSeq = 10, ExportTitle = "CodeDetail_PermissionCategoryType_Customer", ExportTitleResourceType = typeof(Resources.SYS.CodeDetail))]
        [Display(Name = "ActingBill_PartyName", ResourceType = typeof(Resources.BIL.ActingBill))]
		public string Party { get; set; }
        /// <summary>
        /// ��Ӧ��/�ͻ�����
        /// </summary>
        [Display(Name = "ActingBill_PartyName", ResourceType = typeof(Resources.BIL.ActingBill))]
		public string PartyName { get; set; }
        /// <summary>
        /// �۸�
        /// </summary>
		public string PriceList { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [Export(ExportName = "ProcurementActBill", ExportSeq = 70)]
        [Export(ExportName = "SalesActBill", ExportSeq = 90)]
        [Export(ExportName = "ProcurementNoInvoice", ExportSeq = 50)]
        [Export(ExportName = "SalesNoInvoiceBill", ExportSeq = 50)]
        [Display(Name = "ActingBill_Currency", ResourceType = typeof(Resources.BIL.ActingBill))]
		public string Currency { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [Export(ExportName = "ProcurementActBill", ExportSeq = 60)]
        [Export(ExportName = "SalesActBill", ExportSeq = 80)]
        [Display(Name = "ActingBill_UnitPrice", ResourceType = typeof(Resources.BIL.ActingBill))]
		public Decimal UnitPrice { get; set; }
        /// <summary>
        /// �Ƿ��ݹ�
        /// </summary>
		public Boolean IsProvisionalEstimate { get; set; }
        /// <summary>
        /// ˰��
        /// </summary>
		public string Tax { get; set; }
        /// <summary>
        /// �Ƿ�˰
        /// </summary>
		public Boolean IsIncludeTax { get; set; }
        /// <summary>
        /// ���
        /// </summary>
        [Export(ExportName = "SalesActBill", ExportSeq = 110)]
        [Export(ExportName = "ProcurementActBill", ExportSeq = 90)]
        [Display(Name = "ActingBill_BillAmount", ResourceType = typeof(Resources.BIL.ActingBill))]
		public Decimal BillAmount { get; set; }
        /// <summary>
        /// ��Ʊ�н��
        /// </summary>
        [Export(ExportName = "SalesActBill", ExportSeq = 130)]
        [Export(ExportName = "ProcurementActBill", ExportSeq = 110)]
        [Display(Name = "ActingBill_BillingAmount", ResourceType = typeof(Resources.BIL.ActingBill))]
		public Decimal BillingAmount { get; set; }
        /// <summary>
        /// �ѿ�Ʊ���
        /// </summary>
        [Display(Name = "ActingBill_BilledAmount", ResourceType = typeof(Resources.BIL.ActingBill))]
        public Decimal BilledAmount { get; set; }
        /// <summary>
        /// �������
        /// </summary>
        public Decimal VoidAmount { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [Export(ExportName = "ProcurementActBill", ExportSeq = 80)]
        [Export(ExportName = "SalesActBill", ExportSeq = 100)]
        [Display(Name = "ActingBill_BillQty", ResourceType = typeof(Resources.BIL.ActingBill))]
		public Decimal BillQty { get; set; }
        /// <summary>
        /// ��Ʊ������
        /// </summary>
        [Export(ExportName = "ProcurementActBill", ExportSeq = 100)]
        [Export(ExportName = "SalesActBill", ExportSeq = 120)]
        [Display(Name = "ActingBill_BillingQty", ResourceType = typeof(Resources.BIL.ActingBill))]
        public Decimal BillingQty { get; set; }
        /// <summary>
        /// �ѿ�Ʊ����
        /// </summary>
		public Decimal BilledQty { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public Decimal VoidQty { get; set; }
        /// <summary>
        /// ��λת����
        /// </summary>
		public Decimal UnitQty { get; set; }
        /// <summary>
        /// ��Դ��λ
        /// </summary>
		public string LocationFrom { get; set; }
        /// <summary>
        /// ��Чʱ��
        /// </summary>
        [Export(ExportName = "SalesActBill", ExportSeq = 65)]
        [Export(ExportName = "ProcurementActBill", ExportSeq = 45)]
        [Display(Name = "ActingBill_EffectiveDate", ResourceType = typeof(Resources.BIL.ActingBill))]
		public DateTime EffectiveDate { get; set; }
        public Boolean IsClose { get; set; }
		public Int32 CreateUserId { get; set; }
		public string CreateUserName { get; set; }
		public DateTime CreateDate { get; set; }
		public Int32 LastModifyUserId { get; set; }
		public string LastModifyUserName { get; set; }
		public DateTime LastModifyDate { get; set; }
		public Int32 Version { get; set; }
        [Display(Name = "ActingBill_RecPrice", ResourceType = typeof(Resources.BIL.ActingBill))]
        public decimal RecPrice { get; set; }

        [Export(ExportName = "SalesActBill", ExportSeq = 45)]
        [Display(Name = "ActingBill_ReferenceItemCode", ResourceType = typeof(Resources.BIL.ActingBill))]
        public string ReferenceItemCode { get; set; }
        #endregion

		public override int GetHashCode()
        {
			if (Id != 0)
            {
                return Id.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            ActingBill another = obj as ActingBill;

            if (another == null)
            {
                return false;
            }
            else
            {
            	return (this.Id == another.Id);
            }
        } 
    }
	
}
