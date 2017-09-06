using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.SCM
{
    [Serializable]
    public partial class FlowBinding : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        [Display(Name = "FlowBinding_MasterFlow", ResourceType = typeof(Resources.SCM.FlowBinding))]
        public FlowMaster MasterFlow { get; set; }
        [Display(Name = "FlowBinding_BindedFlow", ResourceType = typeof(Resources.SCM.FlowBinding))]
        public FlowMaster BindedFlow { get; set; }
        [Display(Name = "FlowBinding_BindType", ResourceType = typeof(Resources.SCM.FlowBinding))]
        public com.Sconit.CodeMaster.BindType BindType { get; set; }
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
            FlowBinding another = obj as FlowBinding;

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
