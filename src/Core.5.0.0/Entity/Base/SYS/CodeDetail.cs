using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.SYS
{
    [Serializable]
    public partial class CodeDetail : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
		public string Code { get; set; }
        [Display(Name = "CodeDetail_Value", ResourceType = typeof(Resources.SYS.CodeDetail))]
		public string Value { get; set; }
        [Display(Name = "CodeDetail_Description", ResourceType = typeof(Resources.SYS.CodeDetail))]
		public string Description { get; set; }
        [Display(Name = "CodeDetail_IsDefault", ResourceType = typeof(Resources.SYS.CodeDetail))]
		public Boolean IsDefault { get; set; }
        [Display(Name = "CodeDetail_Sequence", ResourceType = typeof(Resources.SYS.CodeDetail))]
		public Int32 Sequence { get; set; }
        
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
            CodeDetail another = obj as CodeDetail;

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
