using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//TODO: Add other using statements here

namespace com.Sconit.Entity.FMS
{
    [Serializable]
    public partial class CheckListOrderDetail : EntityBase, IAuditable
    {
        public int Id { get; set; }
        public String OrderNo { get; set; }
        public String CheckListCode { get; set; }
        public String CheckListDetailCode { get; set; }
        public String Description { get; set; }
        public int Seq { get; set; }
        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }

        public Boolean IsNormal { get; set; }
        public String Remark { get; set; }

        //
        public bool IsRequired { get; set; }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
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
            CheckListOrderDetail another = obj as CheckListOrderDetail;

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
