using System;

namespace com.Sconit.Entity.PRD
{
    [Serializable]
    public partial class WorkingShift : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        public ShiftMaster Shift { get; set; }
        public Int32 WorkingCalendar { get; set; }
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
            WorkingShift another = obj as WorkingShift;

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
