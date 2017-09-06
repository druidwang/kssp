using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.MD
{
    [Serializable]
    public partial class PartyAddress : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "PartyAddress_Id", ResourceType = typeof(Resources.MD.PartyAddress))]
		public Int32 Id { get; set; }
		public string Party { get; set; }
        public Address Address { get; set; }
        [Display(Name = "Address_Type", ResourceType = typeof(Resources.MD.Address))]
        public com.Sconit.CodeMaster.AddressType Type { get; set; }
        [Display(Name = "PartyAddress_IsPrimary", ResourceType = typeof(Resources.MD.PartyAddress))]
		public Boolean IsPrimary { get; set; }

        [Display(Name = "PartyAddress_Sequence", ResourceType = typeof(Resources.MD.PartyAddress))]
		public Int32 Sequence { get; set; }
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
            PartyAddress another = obj as PartyAddress;

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
