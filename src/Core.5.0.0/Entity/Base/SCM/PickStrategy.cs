using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.SCM
{
    [Serializable]
    public partial class PickStrategy : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "PickStrategy_Code", ResourceType = typeof(Resources.SCM.PickStrategy))]
        public string Code { get; set; }
        [Display(Name = "PickStrategy_IsPickFromBin", ResourceType = typeof(Resources.SCM.PickStrategy))]
        public Boolean IsPickFromBin { get; set; }
        [Display(Name = "PickStrategy_OddOption", ResourceType = typeof(Resources.SCM.PickStrategy))]
        public CodeMaster.PickOddOption OddOption { get; set; }
        [Display(Name = "PickStrategy_IsOddOccupy", ResourceType = typeof(Resources.SCM.PickStrategy))]
        public Boolean IsOddOccupy { get; set; }
        [Display(Name = "PickStrategy_IsDevan", ResourceType = typeof(Resources.SCM.PickStrategy))]
        public Boolean IsDevan { get; set; }

        [Display(Name = "PickStrategy_IsFulfillUC", ResourceType = typeof(Resources.SCM.PickStrategy))]
        public Boolean IsFulfillUC { get; set; }

        [Range(-10, 100, ErrorMessageResourceName = "Errors_Common_FieldEquals", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "PickStrategy_UcDeviation", ResourceType = typeof(Resources.SCM.PickStrategy))]
        public double UcDeviation { get; set; }

        [Display(Name = "PickStrategy_ShipStrategy", ResourceType = typeof(Resources.SCM.PickStrategy))]
        public CodeMaster.ShipStrategy ShipStrategy { get; set; }
        [Display(Name = "PickStrategy_IsSimple", ResourceType = typeof(Resources.SCM.PickStrategy))]
        public Boolean IsSimple { get; set; }
        [Display(Name = "PickStrategy_IsMatchDirection", ResourceType = typeof(Resources.SCM.PickStrategy))]
        public Boolean IsMatchDirection { get; set; }

        //[Display(Name = "CreateUserId", ResourceType = typeof(Resources.SCM.PickStrategy))]
        public Int32 CreateUserId { get; set; }
        //[Display(Name = "CreateUserName", ResourceType = typeof(Resources.SCM.PickStrategy))]
        public string CreateUserName { get; set; }
        //[Display(Name = "CreateDate", ResourceType = typeof(Resources.SCM.PickStrategy))]
        public DateTime CreateDate { get; set; }
        //[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.SCM.PickStrategy))]
        public Int32 LastModifyUserId { get; set; }
        //[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.SCM.PickStrategy))]
        public string LastModifyUserName { get; set; }
        //[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.SCM.PickStrategy))]
        public DateTime LastModifyDate { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (Code != null)
            {
                return Code.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            PickStrategy another = obj as PickStrategy;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Code == another.Code);
            }
        }
    }

}
