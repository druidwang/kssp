using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.FMS
{
    [Serializable]
    public partial class CheckListMaster : EntityBase, IAuditable
    {
        [Display(Name = "CheckList_Code", ResourceType = typeof(Resources.FMS.CheckList))]
        public string Code { get; set; }

        [Display(Name = "CheckList_Name", ResourceType = typeof(Resources.FMS.CheckList))]
        public string Name { get; set; }

        [Display(Name = "CheckList_Region", ResourceType = typeof(Resources.FMS.CheckList))]
        public string Region { get; set; }

        [Display(Name = "CheckList_FacilityID", ResourceType = typeof(Resources.FMS.CheckList))]
        public string FacilityID { get; set; }
        public string FacilityName { get; set; }

        [Display(Name = "CheckList_Description", ResourceType = typeof(Resources.FMS.CheckList))]
        public string Description { get; set; }

        public bool NeekCreateTask { get; set; }

        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        public List<CheckListDetail> CheckListDetailList { get; set; }

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
            CheckListMaster another = obj as CheckListMaster;

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
