using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.WMS
{
    [Serializable]
    public partial class PickRule : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }

        [Display(Name = "PickRule_PickGroupCode", ResourceType = typeof(Resources.WMS.PickRule))]
        public string PickGroupCode { get; set; }

        [Display(Name = "PickRule_Location", ResourceType = typeof(Resources.WMS.PickRule))]
        public string Location { get; set; }

        [Display(Name = "PickRule_LocationArea", ResourceType = typeof(Resources.WMS.PickRule))]
        public string Area { get; set; }

        public Int32 CreateUserId { get; set; }

        [Display(Name = "PickRule_CreateUserName", ResourceType = typeof(Resources.WMS.PickRule))]
        public string CreateUserName { get; set; }

        [Display(Name = "PickRule_CreateDate", ResourceType = typeof(Resources.WMS.PickRule))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }

        [Display(Name = "PickRule_LastModifyUserName", ResourceType = typeof(Resources.WMS.PickRule))]
        public string LastModifyUserName { get; set; }

        [Display(Name = "PickRule_LastModifyDate", ResourceType = typeof(Resources.WMS.PickRule))]
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
            PickRule another = obj as PickRule;

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
