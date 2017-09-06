using System;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.WMS
{
    [Serializable]
    public partial class PickResult : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }

        [Display(Name = "PickResult_PickTaskId", ResourceType = typeof(Resources.WMS.PickResult))]
        public Int32 PickTaskId { get; set; }

        public string PickTaskUUID { get; set; }

        [Display(Name = "PickResult_Item", ResourceType = typeof(Resources.WMS.PickResult))]
        public string Item { get; set; }


        [Display(Name = "PickResult_ItemDescription", ResourceType = typeof(Resources.WMS.PickResult))]
        public string ItemDescription { get; set; }

        [Display(Name = "PickResult_ReferenceItemCode", ResourceType = typeof(Resources.WMS.PickResult))]
        public string ReferenceItemCode { get; set; }

        [Display(Name = "PickResult_Uom", ResourceType = typeof(Resources.WMS.PickResult))]
        public string Uom { get; set; }

        [Display(Name = "PickResult_BaseUom", ResourceType = typeof(Resources.WMS.PickResult))]
        public string BaseUom { get; set; }

        [Display(Name = "PickResult_UnitQty", ResourceType = typeof(Resources.WMS.PickResult))]
        public Decimal UnitQty { get; set; }

        [Display(Name = "PickResult_UnitCount", ResourceType = typeof(Resources.WMS.PickResult))]
        public Decimal UnitCount { get; set; }

        [Display(Name = "PickResult_UCDescription", ResourceType = typeof(Resources.WMS.PickResult))]
        public string UCDescription { get; set; }

        [Display(Name = "PickResult_PickQty", ResourceType = typeof(Resources.WMS.PickResult))]
        public Decimal PickQty { get; set; }

        [Display(Name = "PickResult_Location", ResourceType = typeof(Resources.WMS.PickResult))]
        public string Location { get; set; }

        [Display(Name = "PickResult_Area", ResourceType = typeof(Resources.WMS.PickResult))]
        public string Area { get; set; }

        [Display(Name = "PickResult_Bin", ResourceType = typeof(Resources.WMS.PickResult))]
        public string Bin { get; set; }

        [Display(Name = "PickResult_LotNo", ResourceType = typeof(Resources.WMS.PickResult))]
        public string LotNo { get; set; }

        [Display(Name = "PickResult_HuId", ResourceType = typeof(Resources.WMS.PickResult))]
        public string HuId { get; set; }
        public Int32 PickUserId { get; set; }

        [Display(Name = "PickResult_PickUserName", ResourceType = typeof(Resources.WMS.PickResult))]
        public string PickUserName { get; set; }

        [Display(Name = "PickResult_PickDate", ResourceType = typeof(Resources.WMS.PickResult))]
        public DateTime PickDate { get; set; }
        public Int32 CreateUserId { get; set; }

        [Display(Name = "PickResult_CreateUserName", ResourceType = typeof(Resources.WMS.PickResult))]
        public string CreateUserName { get; set; }

        [Display(Name = "PickResult_CreateDate", ResourceType = typeof(Resources.WMS.PickResult))]
        public DateTime CreateDate { get; set; }
        public Boolean IsCancel { get; set; }

        public Int32 CancelUser { get; set; }

        [Display(Name = "PickResult_CancelUserName", ResourceType = typeof(Resources.WMS.PickResult))]
        public string CancelUserName { get; set; }

        [Display(Name = "PickResult_CancelDate", ResourceType = typeof(Resources.WMS.PickResult))]
        public DateTime CancelDate { get; set; }

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
            PickResult another = obj as PickResult;

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
