using System;
using System.ComponentModel.DataAnnotations;
namespace com.Sconit.Entity.MD
{
    [Serializable]
    public partial class SwitchTrading : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }

        [Display(Name = "SwitchTrading_Flow", ResourceType = typeof(Resources.MD.SwitchTrading))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        public string Flow { get; set; }


        [Display(Name = "SwitchTrading_Supplier", ResourceType = typeof(Resources.MD.SwitchTrading))]
        public string Supplier { get; set; }

        [Display(Name = "SwitchTrading_Customer", ResourceType = typeof(Resources.MD.SwitchTrading))]
        public string Customer { get; set; }

        [Display(Name = "SwitchTrading_PurchaseGroup", ResourceType = typeof(Resources.MD.SwitchTrading))]
        public string PurchaseGroup { get; set; }

        [Display(Name = "FlowMaster_SalesOrg", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string SalesOrg { get; set; }

        [Display(Name = "FlowMaster_DistrChan", ResourceType = typeof(Resources.SCM.FlowMaster))]
        public string DistrChan { get; set; }
        public string DIVISION { get; set; }
        public Int32 CreateUserId { get; set; }
        [Display(Name = "SwitchTrading_CreateUserName", ResourceType = typeof(Resources.MD.SwitchTrading))]
        public string CreateUserName { get; set; }
        [Display(Name = "SwitchTrading_CreateDate", ResourceType = typeof(Resources.MD.SwitchTrading))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        [Display(Name = "SwitchTrading_LastModifyUserName", ResourceType = typeof(Resources.MD.SwitchTrading))]
        public string LastModifyUserName { get; set; }
        [Display(Name = "SwitchTrading_LastModifyDate", ResourceType = typeof(Resources.MD.SwitchTrading))]
        public DateTime LastModifyDate { get; set; }
        [Display(Name = "PriceListDetail_PriceList", ResourceType = typeof(Resources.BIL.PriceListDetail))]
        public string PriceList { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (Id != null)
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
            SwitchTrading another = obj as SwitchTrading;

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
