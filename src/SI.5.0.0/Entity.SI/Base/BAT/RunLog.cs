using System;
using com.Sconit.Entity;

namespace com.Sconit.Entity.SI.BAT
{
    [Serializable]
    public partial class RunLog : EntityBase
    {
        #region O/R Mapping Properties
		
		public Decimal Id { get; set; }
        public string JobCode { get; set; }
		public string TriggerName { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime? EndTime { get; set; }
        public CodeMaster.JobRunStatus Status { get; set; }
		public string Message { get; set; }
        
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
            RunLog another = obj as RunLog;

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
