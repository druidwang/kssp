using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.TRANS
{
    [Serializable]
    public partial class RccpTrans : EntityBase
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }
        [Display(Name = "RccpTrans_DateIndex", ResourceType = typeof(Resources.MRP.RccpTrans))]
        public string DateIndex { get; set; }
        [Display(Name = "RccpTrans_Item", ResourceType = typeof(Resources.MRP.RccpTrans))]
        public string Item { get; set; }
        [Display(Name = "RccpTrans_DateType", ResourceType = typeof(Resources.MRP.RccpTrans))]
        public com.Sconit.CodeMaster.TimeUnit DateType { get; set; }
        [Display(Name = "RccpTrans_PlanVersion", ResourceType = typeof(Resources.MRP.RccpTrans))]
        public DateTime PlanVersion { get; set; }
        [Display(Name = "RccpTrans_Qty", ResourceType = typeof(Resources.MRP.RccpTrans))]
        public double Qty { get; set; }
        [Display(Name = "RccpTrans_SourcePlanVersion", ResourceType = typeof(Resources.MRP.RccpTrans))]
        public Int32 SourcePlanVersion { get; set; }
        [Display(Name = "RccpTrans_Bom", ResourceType = typeof(Resources.MRP.RccpTrans))]
        public string Bom { get; set; }
        [Display(Name = "RccpTrans_ScrapPercentage", ResourceType = typeof(Resources.MRP.RccpTrans))]
        public Double ScrapPercentage { get; set; }
        [Display(Name = "RccpTrans_IsLastLevel", ResourceType = typeof(Resources.MRP.RccpTrans))]
        public bool IsLastLevel { get; set; }

        public string Model { get; set; }
        public Double ModelRate { get; set; }

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
            RccpTrans another = obj as RccpTrans;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.Id == another.Id);
            }
        }

        //public override int GetHashCode()
        //{
        //    if (DateIndex != null && Item != null && (int)DateType != 0 && PlanVersion != DateTime.MinValue)
        //    {
        //        return DateIndex.GetHashCode()
        //            ^ Item.GetHashCode()
        //            ^ DateType.GetHashCode()
        //            ^ PlanVersion.GetHashCode();
        //    }
        //    else
        //    {
        //        return base.GetHashCode();
        //    }
        //}

        //public override bool Equals(object obj)
        //{
        //    RccpTrans another = obj as RccpTrans;

        //    if (another == null)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return (this.DateIndex == another.DateIndex)
        //            && (this.Item == another.Item)
        //            && (this.DateType == another.DateType)
        //            && (this.PlanVersion == another.PlanVersion);
        //    }
        //}
    }

}
