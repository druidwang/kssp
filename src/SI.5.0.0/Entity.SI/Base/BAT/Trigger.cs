using System;
using com.Sconit.Entity;

namespace com.Sconit.Entity.SI.BAT
{
    [Serializable]
    public partial class Trigger : EntityBase
    {
        #region O/R Mapping Properties
		
		public string Name { get; set; }
        public JobDetail JobDetail { get; set; }
		public string Description { get; set; }
		public DateTime? PreviousFireTime { get; set; }
		public DateTime? NextFireTime { get; set; }
		public Int32 RepeatCount { get; set; }
		public Int32 Interval { get; set; }
        public CodeMaster.TimeUnit IntervalType { get; set; }
		public Int32 TimesTriggered { get; set; }
        public CodeMaster.TriggerStatus Status { get; set; }
        
        #endregion

		public override int GetHashCode()
        {
            if (Name != null)
            {
                return Name.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            Trigger another = obj as Trigger;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Name == another.Name);
            }
        }

    }
	
}
