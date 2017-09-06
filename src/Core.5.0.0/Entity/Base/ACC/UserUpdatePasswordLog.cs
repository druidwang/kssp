using System;
using System.ComponentModel.DataAnnotations;
namespace com.Sconit.Entity.ACC
{
    [Serializable]
    public partial class UserUpdatePasswordLog : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public DateTime UpdateTime { get; set; }

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
            UserUpdatePasswordLog another = obj as UserUpdatePasswordLog;

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