using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class MrpExPlanItemRate : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        //<!--Id, Section, SectionDesc, Item, ItemDesc, ItemRate, CreateUser, CreateUserNm, CreateDate-->
        public Int32 Id { get; set; }
        [Display(Name = "MrpExPlanItemRate_Section", ResourceType = typeof(Resources.MRP.MrpExPlanItemRate))]
        public string Section { get; set; }

        [Display(Name = "MrpExPlanItemRate_Item", ResourceType = typeof(Resources.MRP.MrpExPlanItemRate))]
        public string Item { get; set; }

        [Display(Name = "MrpExPlanItemRate_ItemRate", ResourceType = typeof(Resources.MRP.MrpExPlanItemRate))]
        public decimal ItemRate { get; set; }

        public Int32 CreateUserId { get; set; }

        [Display(Name = "MrpExPlanItemRate_CreateUserName", ResourceType = typeof(Resources.MRP.MrpExPlanItemRate))]
        public string CreateUserName { get; set; }

        [Display(Name = "MrpExPlanItemRate_CreateDate", ResourceType = typeof(Resources.MRP.MrpExPlanItemRate))]
        public DateTime CreateDate { get; set; }

        public Int32 LastModifyUserId { get; set; }

        [Display(Name = "MrpExPlanItemRate_LastModifyUserName", ResourceType = typeof(Resources.MRP.MrpExPlanItemRate))]
        public string LastModifyUserName { get; set; }

        [Display(Name = "MrpExPlanItemRate_LastModifyDate", ResourceType = typeof(Resources.MRP.MrpExPlanItemRate))]
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
            MrpExPlanItemRate another = obj as MrpExPlanItemRate;

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
