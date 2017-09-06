using System;

namespace com.Sconit.Entity.SI.EDI_Ford
{
    [Serializable]
    public partial class PlanningScheduleMaster : EntityBase
    {
        #region O/R Mapping Properties
		
		public Int32 Id { get; set; }
		public string FileType { get; set; }
		public string SenderID { get; set; }
        public string ReceiverID { get; set; }
		public string FileName { get; set; }
		public DateTime CreateDate { get; set; }
        public bool IsImported { get; set; }
        
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
            PlanningScheduleMaster another = obj as PlanningScheduleMaster;

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
