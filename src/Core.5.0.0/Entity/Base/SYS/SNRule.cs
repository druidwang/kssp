using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.SYS
{
    [Serializable]
    public partial class SNRule : EntityBase
    {
        #region O/R Mapping Properties

        [Display(Name = "SNRule_Code", ResourceType = typeof(Resources.SYS.SNRule))]
        public Int32 Code { get; set; }
        [Display(Name = "SNRule_DocumentsType", ResourceType = typeof(Resources.SYS.SNRule))]
        public string Description { get; set; }
        [Display(Name = "SNRule_PreFixed", ResourceType = typeof(Resources.SYS.SNRule))]
		public string PreFixed { get; set; }
        [Display(Name = "SNRule_YearCode", ResourceType = typeof(Resources.SYS.SNRule))]
		public string YearCode { get; set; }
        [Display(Name = "SNRule_MonthCode", ResourceType = typeof(Resources.SYS.SNRule))]
		public string MonthCode { get; set; }
        [Display(Name = "SNRule_DayCode", ResourceType = typeof(Resources.SYS.SNRule))]
		public string DayCode { get; set; }
        [Display(Name = "SNRule_BlockSeq", ResourceType = typeof(Resources.SYS.SNRule))]
		public string BlockSeq { get; set; }
        [Display(Name = "SNRule_SeqLength", ResourceType = typeof(Resources.SYS.SNRule))]
		public Int16 SeqLength { get; set; }
        [Display(Name = "SNRule_SeqBaseType", ResourceType = typeof(Resources.SYS.SNRule))]
		public string SeqBaseType { get; set; }
        [Display(Name = "SNRule_SeqMin", ResourceType = typeof(Resources.SYS.SNRule))]
		public Int32 SeqMin { get; set; }
        
        #endregion

		public override int GetHashCode()
        {
			if (Code != 0)
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
            SNRule another = obj as SNRule;

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
