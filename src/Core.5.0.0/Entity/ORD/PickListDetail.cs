using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using com.Sconit.Entity.INV;
using System;
using com.Sconit.Entity.SYS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ORD
{
    public partial class PickListDetail
    {
        #region Non O/R Mapping Properties
        [Display(Name = "PickListDetail_ItemDescription", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public string ItemFullDescription
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.ReferenceItemCode))
                {
                    return this.ItemDescription + " [" + this.ReferenceItemCode + "]";
                }
                else
                {
                    return this.ItemDescription;
                }
            }
        }

        #endregion
        public decimal CurrentPickQty { get; set; }
        [Export(ExportName = "PickListDetail", ExportSeq = 5)]
        [Display(Name = "PickListDetail_ItemCount", ResourceType = typeof(Resources.ORD.PickListDetail))]
        public Int32 ItemCount { get; set; }
        public IList<PickListDetailInput> PickListDetailInputs { get; set; }

        public void AddPickListDetailInput(PickListDetailInput pickListDetailInput)
        {
            if (PickListDetailInputs == null)
            {
                PickListDetailInputs = new List<PickListDetailInput>();
            }
            PickListDetailInputs.Add(pickListDetailInput);
        }

        public IList<string> GetPickedHuList()
        {
            if (PickListDetailInputs != null)
            {
                return PickListDetailInputs.Select(i => i.HuId).ToList();
            }

            return null;
        }

        public int BinSeq { get; set; }
    }

    public class PickLocationDetail
    {
        public string Item { get; set; }
        public decimal UnitCount { get; set; }
        public string Uom { get; set; }
        public string ManufactureParty { get; set; }
        public string LotNo { get; set; }
        public string Area { get; set; }
        public string Bin { get; set; }
        public int BinSeq { get; set; }
        public decimal Qty { get; set; }
        public bool IsOdd { get; set; }
        public bool IsDevan { get; set; }

        public OrderDetail OrderDetail { get; set; }
        public string PickStrategy { get; set; }
        public bool IsInventory { get; set; }  //ÊÇ·ñ¿â´æ£¬false´ú±í¿â´æ²»×ã

        public string Direction { get; set; }
        public bool IsMatchDirection { get; set; }
        public decimal UcDeviation { get; set; }
        //public CodeMaster.HuOption HuOption { get; set; }
        //public string AgingLocation { get; set; }
    }

    public class PickListDetailInput
    {
        public string HuId { get; set; }

        //¸¨Öú×Ö¶Î
    }
}