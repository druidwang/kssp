using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.SCM
{
    [Serializable]
    public partial class ProductLineFacility : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Display(Name = "Code", ResourceType = typeof(Resources.SCM.ProductLineFacility))]
		public string Code { get; set; }
		public string ProductLine { get; set; }
        [Display(Name = "IsActive", ResourceType = typeof(Resources.SCM.ProductLineFacility))]
		public Boolean IsActive { get; set; }
		public string LocationFrom { get; set; }
		public string LocationTo { get; set; }
		public string InspectLocation { get; set; }
		public string RejectLocation { get; set; }
		public Int32 CreateUserId { get; set; }
		public string CreateUserName { get; set; }
		public DateTime CreateDate { get; set; }
		public Int32 LastModifyUserId { get; set; }
		public string LastModifyUserName { get; set; }
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
            ProductLineFacility another = obj as ProductLineFacility;

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
