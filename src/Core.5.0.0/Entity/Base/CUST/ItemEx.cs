using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    [Serializable]
    public partial class ItemEx : EntityBase, IAuditable
    {
        //物料号
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemEx_Code", ResourceType = typeof(Resources.CUST.ItemEx))]
        public string Code { get; set; }
        //客户
        [Display(Name = "ItemEx_Customer", ResourceType = typeof(Resources.CUST.ItemEx))]
        public string Customer { get; set; }
        /// <summary>
        /// 分类1
        /// </summary>
        [Display(Name = "ItemEx_ItemType1", ResourceType = typeof(Resources.CUST.ItemEx))]
        public string ItemType1 { get; set; }
        /// <summary>
        /// 分类2
        /// </summary>
        [Display(Name = "ItemEx_ItemType2", ResourceType = typeof(Resources.CUST.ItemEx))]
        public string ItemType2 { get; set; }
        /// <summary>
        /// 分类3
        /// </summary>
        [Display(Name = "ItemEx_ItemType3", ResourceType = typeof(Resources.CUST.ItemEx))]
        public string ItemType3 { get; set; }
        /// <summary>
        /// 分类4
        /// </summary>
        [Display(Name = "ItemEx_ItemType4", ResourceType = typeof(Resources.CUST.ItemEx))]
        public string ItemType4 { get; set; }
        /// <summary>
        /// 包装
        /// </summary>
        [Display(Name = "ItemEx_Package", ResourceType = typeof(Resources.CUST.ItemEx))]
        public string Package { get; set; }
        /// <summary>
        /// 老化时间
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemEx_AgeingTime", ResourceType = typeof(Resources.CUST.ItemEx))]
        public Double AgeingTime { get; set; }
        /// <summary>
        /// 老化方法
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ItemEx_AgeingMethod", ResourceType = typeof(Resources.CUST.ItemEx))]
        public string AgeingMethod { get; set; }
        /// <summary>
        /// 断面号
        /// </summary>
        [Display(Name = "ItemEx_Section", ResourceType = typeof(Resources.CUST.ItemEx))]
        public string Section { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        [Display(Name = "ItemEx_PartLength", ResourceType = typeof(Resources.CUST.ItemEx))]
        public Double PartLength { get; set; }
        /// <summary>
        /// 规格单位
        /// </summary>
        [Display(Name = "ItemEx_PartLengthUom", ResourceType = typeof(Resources.CUST.ItemEx))]
        public string PartLengthUom { get; set; }

        public Double OrderLotSize { get; set; }

        public Int32 CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
        public DateTime LastModifyDate { get; set; }
        

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
            ItemEx another = obj as ItemEx;

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
