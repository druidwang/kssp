using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.MD
{
    [Serializable]
    public partial class ProductType : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        [Display(Name = "ProductType_Code", ResourceType = typeof(Resources.MRP.ProductType))]
        public string Code { get; set; }
        [Display(Name = "ProductType_Description", ResourceType = typeof(Resources.MRP.ProductType))]
        public string Description { get; set; }

        [Display(Name = "OrderMaster_SubType", ResourceType = typeof(Resources.ORD.OrderMaster))]
        public com.Sconit.CodeMaster.ScheduleType SubType { get; set; }
        //需要冻结
        [Display(Name = "ProductType_NeedFreeze", ResourceType = typeof(Resources.MRP.ProductType))]
        public bool NeedFreeze { get; set; }
        //需要轮番
        [Display(Name = "ProductType_NeedTurn", ResourceType = typeof(Resources.MRP.ProductType))]
        public bool NeedTurn { get; set; }
        //是否有效
        [Display(Name = "ProductType_IsActive", ResourceType = typeof(Resources.MRP.ProductType))]
        public bool IsActive { get; set; }

        public Int32 CreateUserId { get; set; }

        [Display(Name = "ProductType_CreateUserName", ResourceType = typeof(Resources.MRP.ProductType))]
        public string CreateUserName { get; set; }

        [Display(Name = "ProductType_CreateDate", ResourceType = typeof(Resources.MRP.ProductType))]
        public DateTime CreateDate { get; set; }

        public Int32 LastModifyUserId { get; set; }

        [Display(Name = "ProductType_LastModifyUserName", ResourceType = typeof(Resources.MRP.ProductType))]
        public string LastModifyUserName { get; set; }

        [Display(Name = "ProductType_LastModifyDate", ResourceType = typeof(Resources.MRP.ProductType))]
        public DateTime LastModifyDate { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (Code != null)
            {
                return Code.GetHashCode();

            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            ProductType another = obj as ProductType;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Code == another.Code);
            }
        }
    }

}
