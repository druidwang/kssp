using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MRP.ORD
{
    [Serializable]
    public partial class MrpExScrap : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MrpExScrap_Flow", ResourceType = typeof(Resources.MRP.MrpExScrap))]
        public string Flow { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MrpExScrap_Item", ResourceType = typeof(Resources.MRP.MrpExScrap))]
        public string Item { get; set; }
        [Display(Name = "MrpExScrap_ItemDescription", ResourceType = typeof(Resources.MRP.MrpExScrap))]
        public string ItemDescription { get; set; }

        [Display(Name = "MrpExScrap_ScrapQty", ResourceType = typeof(Resources.MRP.MrpExScrap))]
        public double ScrapQty { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MrpExScrap_ScrapType", ResourceType = typeof(Resources.MRP.MrpExScrap))]
        public CodeMaster.ScheduleType ScrapType { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MrpExScrap_EffectDate", ResourceType = typeof(Resources.MRP.MrpExScrap))]
        public DateTime EffectDate { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "MrpExScrap_Shift", ResourceType = typeof(Resources.MRP.MrpExScrap))]
        public string Shift { get; set; }
        [Display(Name = "MrpExScrap_IsVoid", ResourceType = typeof(Resources.MRP.MrpExScrap))]
        public Boolean IsVoid { get; set; }

        [Display(Name = "MrpExScrap_OrderNo", ResourceType = typeof(Resources.MRP.MrpExScrap))]
        public string OrderNo { get; set; }

        [Display(Name = "MrpExScrap_RefOrderNo", ResourceType = typeof(Resources.MRP.MrpExScrap))]
        public string RefOrderNo { get; set; }

        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }

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
            MrpExScrap another = obj as MrpExScrap;

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
