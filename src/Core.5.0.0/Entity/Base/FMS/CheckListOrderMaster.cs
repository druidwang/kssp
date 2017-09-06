using System;
using System.Collections;
using System.Collections.Generic;

//TODO: Add other using statements here

namespace com.Sconit.Entity.FMS
{
    [Serializable]
    public partial class CheckListOrderMaster : EntityBase, IAuditable
    {

        public string Code { get; set; }
        public string CheckListCode { get; set; }
        public string CheckListName { get; set; }
        public string Region { get; set; }
        public string FacilityID { get; set; }
        public string FacilityName { get; set; }
        public string Description { get; set; }

        public string Remark { get; set; }
        public CodeMaster.CheckListOrderStatus Status { get; set; }

        public string CheckUser { get; set; }
        public DateTime CheckDate { get; set; }
        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        public List<CheckListOrderDetail> CheckListOrderDetailList { get; set; }

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
            CheckListOrderMaster another = obj as CheckListOrderMaster;

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
