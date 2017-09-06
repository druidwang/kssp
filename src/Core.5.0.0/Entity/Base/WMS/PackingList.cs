using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.CodeMaster;

namespace com.Sconit.Entity.WMS
{
    [Serializable]
    public partial class PackingList :  EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "PackingList_PackingListCode", ResourceType = typeof(Resources.WMS.PackingList))]
        public string PackingListCode { get; set; }

        [Display(Name = "PackingList_Flow", ResourceType = typeof(Resources.WMS.PackingList))]
        public string Flow { get; set; }

        public Int32 CreateUserId { get; set; }

        [Display(Name = "PackingList_IsActive", ResourceType = typeof(Resources.WMS.PackingList))]
        public Boolean IsActive { get; set; }

        [Display(Name = "PackingList_CreateUserName", ResourceType = typeof(Resources.WMS.PackingList))]
        public string CreateUserName { get; set; }

        [Display(Name = "PackingList_CreateDate", ResourceType = typeof(Resources.WMS.PackingList))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }

        [Display(Name = "PackingList_LastModifyUserName", ResourceType = typeof(Resources.WMS.PackingList))]
        public string LastModifyUserName { get; set; }

        [Display(Name = "PackingList_LastModifyDate", ResourceType = typeof(Resources.WMS.PackingList))]
        public DateTime LastModifyDate { get; set; }
    
        #endregion

        public override int GetHashCode()
        {
            if (PackingListCode != null)
            {
                return PackingListCode.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            PackingList another = obj as PackingList;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.PackingListCode == another.PackingListCode);
            }
        }
    }

}
