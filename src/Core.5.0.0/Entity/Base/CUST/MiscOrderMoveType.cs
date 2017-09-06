using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    [Serializable]
    public partial class MiscOrderMoveType : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        public com.Sconit.CodeMaster.MiscOrderType IOType { get; set; }
        public com.Sconit.CodeMaster.MiscOrderSubType SubType { get; set; }
        //[Display(Name = "MoveType", ResourceType = typeof(Resources.CUST.MiscOrderMoveType))]
		public string MoveType { get; set; }
        public string CancelMoveType { get; set; }
		//[Display(Name = "Description", ResourceType = typeof(Resources.CUST.MiscOrderMoveType))]
		public string Description { get; set; }
		//[Display(Name = "CheckRecLoc", ResourceType = typeof(Resources.CUST.MiscOrderMoveType))]
		public Boolean CheckRecLoc { get; set; }
		//[Display(Name = "CheckNote", ResourceType = typeof(Resources.CUST.MiscOrderMoveType))]
		public Boolean CheckNote { get; set; }
		//[Display(Name = "CheckCostCenter", ResourceType = typeof(Resources.CUST.MiscOrderMoveType))]
		public Boolean CheckCostCenter { get; set; }
		//[Display(Name = "CheckAsn", ResourceType = typeof(Resources.CUST.MiscOrderMoveType))]
		public Boolean CheckAsn { get; set; }
		//[Display(Name = "CheckReserveNo", ResourceType = typeof(Resources.CUST.MiscOrderMoveType))]
		public Boolean CheckReserveNo { get; set; }
		//[Display(Name = "CheckReserveLine", ResourceType = typeof(Resources.CUST.MiscOrderMoveType))]
		public Boolean CheckReserveLine { get; set; }
		//[Display(Name = "CheckRefNo", ResourceType = typeof(Resources.CUST.MiscOrderMoveType))]
		public Boolean CheckRefNo { get; set; }
		//[Display(Name = "CheckDeliverRegion", ResourceType = typeof(Resources.CUST.MiscOrderMoveType))]
		public Boolean CheckDeliverRegion { get; set; }
		//[Display(Name = "CheckWBS", ResourceType = typeof(Resources.CUST.MiscOrderMoveType))]
		public Boolean CheckWBS { get; set; }
        //要货单号
        public Boolean CheckEBELN { get; set; }
        //要货单行号
        public Boolean CheckEBELP { get; set; }

        public com.Sconit.CodeMaster.QualityType? CheckQualityType { get; set; }

        public Boolean CheckFlow { get; set; }
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
            MiscOrderMoveType another = obj as MiscOrderMoveType;

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
