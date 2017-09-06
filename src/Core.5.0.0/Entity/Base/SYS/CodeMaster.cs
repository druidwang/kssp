using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.SYS
{
    [Serializable]
    public partial class CodeMaster : EntityBase
    {
        #region O/R Mapping Properties
        [Display(Name = "CodeMaster_Code", ResourceType = typeof(Resources.SYS.CodeMaster))]
        public string Code { get; set; }

       [Display(Name = "CodeMaster_Description", ResourceType = typeof(Resources.SYS.CodeMaster))]
		public string Description { get; set; }

        public com.Sconit.CodeMaster.CodeMasterType Type { get; set; }
        
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
            CodeMaster another = obj as CodeMaster;

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
