using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    [Serializable]
    public partial class DefectCode : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "DefectCode_Code", ResourceType = typeof(Resources.CUST.DefectCode))]
		public string Code { get; set; }
        [Display(Name = "DefectCode_Description", ResourceType = typeof(Resources.CUST.DefectCode))]
        public string Description { get; set; }
        [Display(Name = "DefectCode_ProductCode", ResourceType = typeof(Resources.CUST.DefectCode))]
		public string ProductCode { get; set; }
        [Display(Name = "DefectCode_Assemblies", ResourceType = typeof(Resources.CUST.DefectCode))]
		public string Assemblies { get; set; }
        [Display(Name = "DefectCode_ComponentDefectCode", ResourceType = typeof(Resources.CUST.DefectCode))]
		public string ComponentDefectCode { get; set; }
		//[Display(Name = "CreateUserId", ResourceType = typeof(Resources.CUST.DefectCode))]
		public Int32 CreateUserId { get; set; }
        [Display(Name = "DefectCode_CreateUserName", ResourceType = typeof(Resources.CUST.DefectCode))]
		public string CreateUserName { get; set; }
        [Display(Name = "DefectCode_CreateDate", ResourceType = typeof(Resources.CUST.DefectCode))]
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
            DefectCode another = obj as DefectCode;

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
