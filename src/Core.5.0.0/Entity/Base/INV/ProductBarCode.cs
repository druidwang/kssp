using System;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

namespace com.Sconit.Entity.INV
{
    [Serializable]
    public partial class ProductBarCode : EntityBase, IAuditable
    {
        #region O/R Mapping Properties

        [Display(Name = "ProductBarCode_Code", ResourceType = typeof(Resources.INV.ProductBarCode))]
        public string Code { get; set; }

        [Display(Name = "ProductBarCode_HuId", ResourceType = typeof(Resources.INV.ProductBarCode))]
        public string HuId { get; set; }

        [Display(Name = "ProductBarCode_LotNo", ResourceType = typeof(Resources.INV.ProductBarCode))]
        public string LotNo { get; set; }
     
        [Display(Name = "ProductBarCode_Item", ResourceType = typeof(Resources.INV.ProductBarCode))]
        public string Item { get; set; }
      
        [Display(Name = "ProductBarCode_ItemDescription", ResourceType = typeof(Resources.INV.ProductBarCode))]
        public string ItemDescription { get; set; }
     
        [Display(Name = "ProductBarCode_ReferenceItemCode", ResourceType = typeof(Resources.INV.ProductBarCode))]
        public string ReferenceItemCode { get; set; }
     
        [Display(Name = "ProductBarCode_Uom", ResourceType = typeof(Resources.INV.ProductBarCode))]
        public string Uom { get; set; }
        public string BaseUom { get; set; }
      
        [Display(Name = "ProductBarCode_UnitCount", ResourceType = typeof(Resources.INV.ProductBarCode))]
        public Decimal UnitCount { get; set; }
   
        [Display(Name = "ProductBarCode_Qty", ResourceType = typeof(Resources.INV.ProductBarCode))]
        public Decimal Qty { get; set; }


        [Display(Name = "ProductBarCode_ManufactureDate", ResourceType = typeof(Resources.INV.ProductBarCode))]
        public DateTime ManufactureDate { get; set; }

        [Display(Name = "ProductBarCode_ManufactureParty", ResourceType = typeof(Resources.INV.ProductBarCode))]
        public string ManufactureParty { get; set; }


        [Display(Name = "ProductBarCode_OrderNo", ResourceType = typeof(Resources.INV.ProductBarCode))]
        public string OrderNo { get; set; }

        public Int32 CreateUserId { get; set; }
        [Display(Name = "ProductBarCode_CreateUserName", ResourceType = typeof(Resources.INV.ProductBarCode))]
        public string CreateUserName { get; set; }
        [Display(Name = "ProductBarCode_CreateDate", ResourceType = typeof(Resources.INV.ProductBarCode))]
        public DateTime CreateDate { get; set; }
        public Int32 LastModifyUserId { get; set; }
      
        [Display(Name = "ProductBarCode_LastModifyUserName", ResourceType = typeof(Resources.INV.ProductBarCode))]
        public string LastModifyUserName { get; set; }
      
        [Display(Name = "ProductBarCode_LastModifyDate", ResourceType = typeof(Resources.INV.ProductBarCode))]
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
            ProductBarCode another = obj as ProductBarCode;

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
