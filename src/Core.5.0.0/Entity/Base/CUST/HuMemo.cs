using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    [Serializable]
    public partial class HuMemo : EntityBase, IAuditable
    {
        [Display(Name = "HuMemo_Code", ResourceType = typeof(Resources.CUST.HuMemo))]
        public string Code { get; set; }
        [Display(Name = "HuMemo_Description", ResourceType = typeof(Resources.CUST.HuMemo))]
        public string Description { get; set; }
        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        [Display(Name = "HuMemo_ResourceGroup", ResourceType = typeof(Resources.CUST.HuMemo))]
        public com.Sconit.CodeMaster.ResourceGroup ResourceGroup { get; set; }
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
            HuMemo another = obj as HuMemo;

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
