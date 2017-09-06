using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.CodeMaster;

namespace com.Sconit.Entity.WMS
{
    [Serializable]
    public partial class PickGroup :  EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "PickGroup_PickGroupCode", ResourceType = typeof(Resources.WMS.PickGroup))]
        public string PickGroupCode { get; set; }


        public PickGroupType Type { get; set; }

        [Display(Name = "PickGroup_Description", ResourceType = typeof(Resources.WMS.PickGroup))]
        public string Description { get; set; }

        [Display(Name = "PickGroup_IsActive", ResourceType = typeof(Resources.WMS.PickGroup))]
        public Boolean IsActive { get; set; }
        public Int32 CreateUserId { get; set; }

        [Display(Name = "PickGroup_CreateUserName", ResourceType = typeof(Resources.WMS.PickGroup))]
        public string CreateUserName { get; set; }

        [Display(Name = "PickGroup_CreateDate", ResourceType = typeof(Resources.WMS.PickGroup))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }

        [Display(Name = "PickGroup_LastModifyUserName", ResourceType = typeof(Resources.WMS.PickGroup))]
        public string LastModifyUserName { get; set; }

        [Display(Name = "PickGroup_LastModifyDate", ResourceType = typeof(Resources.WMS.PickGroup))]
        public DateTime LastModifyDate { get; set; }
        public Int32 Version { get; set; }

        [Display(Name = "PickGroup_IsAutoAssign", ResourceType = typeof(Resources.WMS.PickGroup))]
        public Boolean IsAutoAssign { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (PickGroupCode != null)
            {
                return PickGroupCode.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            PickGroup another = obj as PickGroup;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.PickGroupCode == another.PickGroupCode);
            }
        }
    }

}
