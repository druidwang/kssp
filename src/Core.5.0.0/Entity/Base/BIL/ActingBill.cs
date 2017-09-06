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
        /// 订单号
        /// </summary>
		public string OrderNo { get; set; }
        /// <summary>
        /// 送货单号
        /// </summary>
		public string IpNo { get; set; }
        /// <summary>
        /// 外部送货单号
        /// </summary>
		public string ExternalIpNo { get; set; }
        /// <summary>
        /// 收货单号
        /// </summary>
        [Export(ExportName = "SalesActBill", ExportSeq = 10)]
        [Export(ExportName = "ProcurementActBill", ExportSeq = 10)]
        [Display(Name = "ActingBill_ReceiptNo", ResourceType = typeof(Resources.BIL.ActingBill))]
		public string ReceiptNo { get; set; }
        /// <summary>
        /// 外部收货单号
        /// </summary>
        [Export(ExportName = "SalesActBill", ExportSeq = 20)]
        [Display(Name = "ActingBill_ExternalReceiptNo", ResourceType = typeof(Resources.BIL.ActingBill))]
		public string ExternalReceiptNo { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public com.Sconit.CodeMaster.BillType Type { get; set; }
        /// <summary>
        /// 物料
        /// </summary>
        [Export(ExportName = "SalesActBill", ExportSeq = 30)]
        [Export(ExportName = "ProcurementNoInvoice", ExportSeq = 20)]
        [Export(ExportName = "SalesNoInvoiceBill", ExportSeq = 15)]
        [Export(ExportName = "ProcurementActBill", ExportSeq = 20)]
        [Display(Name = "ActingBill_Item", ResourceType = typeof(Resources.BIL.ActingBill))]
        public string Item { get; set; }
        /// <summary>
        /// 物料描述
        /// </summary>
        [Export(ExportName = "SalesActBill", ExportSeq = 40)]
        [Export(ExportName = "ProcurementNoInvoice", ExportSeq = 30)]
        [Export(ExportName = "SalesNoInvoiceBill", ExportSeq = 30)]
        [Export(ExportName = "ProcurementActBill", ExportSeq = 30)]
        [Display(Name = "ActingBill_ItemDescription", ResourceType = typeof(Resources.BIL.ActingBill))]
        public string ItemDescription { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        [Export(ExportName = "SalesActBill", ExportSeq = 60)]
        [Export(ExportName = "ProcurementNoInvoice", ExportSeq = 60)]
        [Export(ExportName = "SalesNoInvoiceBill", ExportSeq = 60)]
        [Export(ExportName = "ProcurementActBill", ExportSeq = 40)]
        [Display(Name = "ActingBill_Uom", ResourceType = typeof(Resources.BIL.ActingBill))]
		public string Uom { get; set; }
        /// <summary>
        /// 单包装
        /// </summary>
		public Decimal UnitCount { get; set; }
        /// <summary>
        /// 寄售类型
        /// </summary>
        public CodeMaster.OrderBillTerm BillTerm { get; set; }
        /// <summary>
        /// 开票地址
        /// </summary>
		public string BillAddress { get; set; }
        /// <summary>
        /// 开票地址描述
        /// </summary>
		public string BillAddressDescription { get; set; }
        /// <summary>
        /// 供应商/客户
        /// </summary> 
        [Export(ExportName = "ProcurementNoInvoice", ExportSeq = 10, ExportTitle = "CodeDetail_PermissionCategoryType_Supplier", ExportTitleResourceType = typeof(Resources.SYS.CodeDetail))]
        [Export(ExportName = "SalesNoInvoiceBill", ExportSeq = 10, ExportTitle = "CodeDetail_PermissionCategoryType_Customer", ExportTitleResourceType = typeof(Resources.SYS.CodeDetail))]
        [Display(Name = "ActingBill_PartyName", ResourceType = typeof(Resources.BIL.ActingBill))]
		public string Party { get; set; }
        /// <summary>
        /// 供应商/客户名称
        /// </summary>
        [Display(Name = "ActingBill_PartyName", ResourceType = typeof(Resources.BIL.ActingBill))]
		public string PartyName { get; set; }
        /// <summary>
        /// 价格单
        /// </summary>
		public string PriceList { get; set; }
        /// <summary>
        /// 货币
        /// </summary>
        [Export(ExportName = "ProcurementActBill", ExportSeq = 70)]
        [Export(ExportName = "SalesActBill", ExportSeq = 90)]
        [Export(ExportName = "ProcurementNoInvoice", ExportSeq = 50)]
        [Export(ExportName = "SalesNoInvoiceBill", ExportSeq = 50)]
        [Display(Name = "ActingBill_Currency", ResourceType = typeof(Resources.BIL.ActingBill))]
		public string Currency { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        [Export(ExportName = "ProcurementActBill", ExportSeq = 60)]
        [Export(ExportName = "SalesActBill", ExportSeq = 80)]
        [Display(Name = "ActingBill_UnitPrice", ResourceType = typeof(Resources.BIL.ActingBill))]
		public Decimal UnitPrice { get; set; }
        /// <summary>
        /// 是否暂估
        /// </summary>
		public Boolean IsProvisionalEstimate { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
		public string Tax { get; set; }
        /// <summary>
        /// 是否含税
        /// </summary>
		public Boolean IsIncludeTax { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        [Export(ExportName = "SalesActBill", ExportSeq = 110)]
        [Export(ExportName = "ProcurementActBill", ExportSeq = 90)]
        [Display(Name = "ActingBill_BillAmount", ResourceType = typeof(Resources.BIL.ActingBill))]
		public Decimal BillAmount { get; set; }
        /// <summary>
        /// 开票中金额
        /// </summary>
        [Export(ExportName = "SalesActBill", ExportSeq = 130)]
        [Export(ExportName = "ProcurementActBill", ExportSeq = 110)]
        [Display(Name = "ActingBill_BillingAmount", ResourceType = typeof(Resources.BIL.ActingBill))]
		public Decimal BillingAmount { get; set; }
        /// <summary>
        /// 已开票金额
        /// </summary>
        [Display(Name = "ActingBill_BilledAmount", ResourceType = typeof(Resources.BIL.ActingBill))]
        public Decimal BilledAmount { get; set; }
        /// <summary>
        /// 冲销金额
        /// </summary>
        public Decimal VoidAmount { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [Export(ExportName = "ProcurementActBill", ExportSeq = 80)]
        [Export(ExportName = "SalesActBill", ExportSeq = 100)]
        [Display(Name = "ActingBill_BillQty", ResourceType = typeof(Resources.BIL.ActingBill))]
		public Decimal BillQty { get; set; }
        /// <summary>
        /// 开票中数量
        /// </summary>
        [Export(ExportName = "ProcurementActBill", ExportSeq = 100)]
        [Export(ExportName = "SalesActBill", ExportSeq = 120)]
        [Display(Name = "ActingBill_BillingQty", ResourceType = typeof(Resources.BIL.ActingBill))]
        public Decimal BillingQty { get; set; }
        /// <summary>
        /// 已开票数量
        /// </summary>
		public Decimal BilledQty { get; set; }
        /// <summary>
        /// 冲销数量
        /// </summary>
        public Decimal VoidQty { get; set; }
        /// <summary>
        /// 单位转换率
        /// </summary>
		public Decimal UnitQty { get; set; }
        /// <summary>
        /// 来源库位
        /// </summary>
		public string LocationFrom { get; set; }
        /// <summary>
        /// 生效时间
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
