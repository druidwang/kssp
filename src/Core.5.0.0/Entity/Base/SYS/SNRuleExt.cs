using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.SYS
{
    [Serializable]
    public partial class SNRuleExt : EntityBase
    {
        #region O/R Mapping Properties
		
		//[Display(Name = "Id", ResourceType = typeof(Resources.SYS.SNRuleExt))]
		public Int32 Id { get; set; }
		//[Display(Name = "Code", ResourceType = typeof(Resources.SYS.SNRuleExt))]
		public Int32 Code { get; set; }
		//[Display(Name = "Field", ResourceType = typeof(Resources.SYS.SNRuleExt))]
		public string Field { get; set; }
		//[Display(Name = "FieldSeq", ResourceType = typeof(Resources.SYS.SNRuleExt))]
		public Int32 FieldSeq { get; set; }

        public Boolean IsChoosed { get; set; }
        
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
            SNRuleExt another = obj as SNRuleExt;

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
