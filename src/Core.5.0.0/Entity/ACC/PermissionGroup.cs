using System;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ACC
{
    public partial class PermissionGroup
    {
        #region Non O/R Mapping Properties

        [Display(Name = "PermissionGroup_Desc", ResourceType = typeof(Resources.ACC.PermissionGroup))]
         public string CodeDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Description))
                {
                    return this.Code;
                }
                else
                {
                    return this.Code + " [" + this.Description + "]";
                }
            }
        }

        public PermissionGroup()
        {
        }

        public PermissionGroup(int id)
        {
            this.Id = id;
        }

        #endregion
    }
}