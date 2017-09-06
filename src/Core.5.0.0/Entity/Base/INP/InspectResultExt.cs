using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.INP
{
    [Serializable]
    public partial class InspectResultExt : EntityBase, IAuditable
    {
        //[Display(Name = "InspectResultExt_Id", ResourceType = typeof(Resources.INP.InspectResultExt))]
        public Int32 Id { get; set; }
        // [Display(Name = "InspectResultExt_InspectResultId", ResourceType = typeof(Resources.INP.InspectResultExt))]
        public Int32 InspectResultId { get; set; }
        [Display(Name = "InspectResultExt_QualityDiscription", ResourceType = typeof(Resources.INP.InspectResultExt))]
        public string QualityDiscription { get; set; }
        [Display(Name = "InspectResultExt_QualityType", ResourceType = typeof(Resources.INP.InspectResultExt))]
        public string QualityType { get; set; }
        [Display(Name = "InspectResultExt_ProductType", ResourceType = typeof(Resources.INP.InspectResultExt))]
        public string ProductType { get; set; }
        [Display(Name = "InspectResultExt_Supplier", ResourceType = typeof(Resources.INP.InspectResultExt))]
        public string Supplier { get; set; }
        [Display(Name = "InspectResultExt_UnitCode", ResourceType = typeof(Resources.INP.InspectResultExt))]
        public string UnitCode { get; set; }
        [Display(Name = "InspectResultExt_RationalizeInspect", ResourceType = typeof(Resources.INP.InspectResultExt))]
        public string RationalizeInspect { get; set; }
        [Display(Name = "InspectResultExt_PurDepartment", ResourceType = typeof(Resources.INP.InspectResultExt))]
        public string PurDepartment { get; set; }
        [Display(Name = "InspectResultExt_Picture", ResourceType = typeof(Resources.INP.InspectResultExt))]
        public Byte[] Picture { get; set; }
        //public virtual byte[] Picture { get; set; }
        [Display(Name = "InspectResultExt_Checker", ResourceType = typeof(Resources.INP.InspectResultExt))]
        public string Checker { get; set; }
        [Display(Name = "InspectResultExt_StartLotNo", ResourceType = typeof(Resources.INP.InspectResultExt))]
        public string StartLotNo { get; set; }
        [Display(Name = "InspectResultExt_EndLotNo", ResourceType = typeof(Resources.INP.InspectResultExt))]
        public string EndLotNo { get; set; }
        //[Display(Name = "CreateUserId", ResourceType = typeof(Resources.INP.InspectResultExt))]
        public Int32 CreateUserId { get; set; }
        //[Display(Name = "InspectDetail_CreateUserName", ResourceType = typeof(Resources.INP.InspectResultExt))]
        public string CreateUserName { get; set; }
        //[Display(Name = "InspectDetail_CreateDate", ResourceType = typeof(Resources.INP.InspectResultExt))]
        public DateTime CreateDate { get; set; }
        //[Display(Name = "LastModifyUserId", ResourceType = typeof(Resources.INP.InspectResultExt))]
        public Int32 LastModifyUserId { get; set; }
        //[Display(Name = "LastModifyUserName", ResourceType = typeof(Resources.INP.InspectResultExt))]
        public string LastModifyUserName { get; set; }
        //[Display(Name = "LastModifyDate", ResourceType = typeof(Resources.INP.InspectResultExt))]
        public DateTime LastModifyDate { get; set; }


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
            InspectResultExt another = obj as InspectResultExt;

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
