using System;

namespace com.Sconit.Entity.SI
{
    [Serializable]
    public partial class LogToUser : EntityBase
    {
        #region O/R Mapping Properties

        public int Id { get; set; }
        public string Descritpion { get; set; }
        public string Emails { get; set; }
        public string Mobiles { get; set; }
        public string Template { get; set; }

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
            LogToUser another = obj as LogToUser;

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
