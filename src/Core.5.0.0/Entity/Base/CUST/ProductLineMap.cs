using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.CUST
{
    [Serializable]
    public partial class ProductLineMap : EntityBase
    {
        #region O/R Mapping Properties
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProductLineMap_SAPProductLine", ResourceType = typeof(Resources.CUST.ProductLineMap))]
        public string SAPProductLine { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Errors_Common_FieldRequired", ErrorMessageResourceType = typeof(Resources.SYS.ErrorMessage))]
        [Display(Name = "ProductLineMap_ProductLine", ResourceType = typeof(Resources.CUST.ProductLineMap))]
        public string ProductLine { get; set; }
        [Display(Name = "ProductLineMap_CabLocation", ResourceType = typeof(Resources.CUST.ProductLineMap))]
        public string CabLocation { get; set; }
        [Display(Name = "ProductLineMap_ChassisLocation", ResourceType = typeof(Resources.CUST.ProductLineMap))]
        public string ChassisLocation { get; set; }
        [Display(Name = "ProductLineMap_VanLocation", ResourceType = typeof(Resources.CUST.ProductLineMap))]
        public string VanLocation { get; set; }
        [Display(Name = "ProductLineMap_CabFlow", ResourceType = typeof(Resources.CUST.ProductLineMap))]
        public string CabFlow { get; set; }
        [Display(Name = "ProductLineMap_ChassisFlow", ResourceType = typeof(Resources.CUST.ProductLineMap))]
        public string ChassisFlow { get; set; }
        [Display(Name = "ProductLineMap_ChassisSapLocation", ResourceType = typeof(Resources.CUST.ProductLineMap))]
        public string ChassisSapLocation { get; set; }
        [Display(Name = "ProductLineMap_VanSapLocation", ResourceType = typeof(Resources.CUST.ProductLineMap))]
        public string VanSapLocation { get; set; }
        [Display(Name = "ProductLineMap_PowerFlow", ResourceType = typeof(Resources.CUST.ProductLineMap))]
        public string PowerFlow { get; set; }
        [Display(Name = "ProductLineMap_TransmissionFlow", ResourceType = typeof(Resources.CUST.ProductLineMap))]
        public string TransmissionFlow { get; set; }
        [Display(Name = "ProductLineMap_TireFlow", ResourceType = typeof(Resources.CUST.ProductLineMap))]
        public string TireFlow { get; set; }
        [Display(Name = "ProductLineMap_SaddleFlow", ResourceType = typeof(Resources.CUST.ProductLineMap))]
        public string SaddleFlow { get; set; }
        #endregion

        public override int GetHashCode()
        {
            if (SAPProductLine != null)
            {
                return SAPProductLine.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            ProductLineMap another = obj as ProductLineMap;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.SAPProductLine == another.SAPProductLine);
            }
        }
    }

}
