using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.PRD
{
    [Serializable]
    public partial class SmallSparePartChk : EntityBase, IAuditable
    {
        #region O/R Mapping Properties
        public Int32 Id { get; set; }
        [Export(ExportName = "SmallMatchSparePartCheck", ExportSeq = 10)]
        [Display(Name = "SmallSparePartChk_Huid", ResourceType = typeof(Resources.PRD.SmallSparePartChk))]
        public string Huid { get; set; }
        [Export(ExportName = "SmallMatchSparePartCheck", ExportSeq = 20)]
        [Display(Name = "SmallSparePartChk_HuItem", ResourceType = typeof(Resources.PRD.SmallSparePartChk))]
        public string HuItem { get; set; }
        [Export(ExportName = "SmallMatchSparePartCheck", ExportSeq = 30)]
        [Display(Name = "SmallSparePartChk_HuQty", ResourceType = typeof(Resources.PRD.SmallSparePartChk))]
        public decimal HuQty { get; set; }
        [Export(ExportName = "SmallMatchSparePartCheck", ExportSeq = 40)]
        [Display(Name = "SmallSparePartChk_HuItemDesc", ResourceType = typeof(Resources.PRD.SmallSparePartChk))]
        public string HuItemDesc { get; set; }
        [Export(ExportName = "SmallMatchSparePartCheck", ExportSeq = 50)]
        [Display(Name = "SmallSparePartChk_SpareItem", ResourceType = typeof(Resources.PRD.SmallSparePartChk))]
        public string SpareItem { get; set; }
        [Export(ExportName = "SmallMatchSparePartCheck", ExportSeq = 60)]
        [Display(Name = "SmallSparePartChk_SpareItemDesc", ResourceType = typeof(Resources.PRD.SmallSparePartChk))]
        public string SpareItemDesc { get; set; }

        public Int32 CreateUserId { get; set; }

        [Display(Name = "IpMaster_CreateUserName", ResourceType = typeof(Resources.ORD.IpMaster))]
        public string CreateUserName { get; set; }

        [Display(Name = "IpMaster_CreateDate", ResourceType = typeof(Resources.ORD.IpMaster))]
        public DateTime CreateDate { get; set; }

        public Int32 LastModifyUserId { get; set; }
        public string LastModifyUserName { get; set; }
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
            SmallSparePartChk another = obj as SmallSparePartChk;

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
