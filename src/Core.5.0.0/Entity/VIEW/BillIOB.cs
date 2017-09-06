using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace com.Sconit.Entity.VIEW
{
    public class BillIOB
    {
        [Display(Name = "BillIOB_Party", ResourceType = typeof(Resources.Report.BillIOB))]
        public string Party { get; set; }
        [Display(Name = "BillIOB_PartyName", ResourceType = typeof(Resources.Report.BillIOB))]
        public string PartyName { get; set; }
        [Display(Name = "BillIOB_Location", ResourceType = typeof(Resources.Report.BillIOB))]
        public string Location { get; set; }
        [Display(Name = "BillIOB_LocationName", ResourceType = typeof(Resources.Report.BillIOB))]
        public string LocationName { get; set; }
        [Display(Name = "BillIOB_Item", ResourceType = typeof(Resources.Report.BillIOB))]
        public string Item { get; set; }
        [Display(Name = "BillIOB_ItemDescription", ResourceType = typeof(Resources.Report.BillIOB))]
        public string ItemDescription { get; set; }
        [Display(Name = "BillIOB_Uom", ResourceType = typeof(Resources.Report.BillIOB))]
        public string Uom { get; set; }

        [Display(Name = "Item_MaterialsGroup", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroup { get; set; }

        [Display(Name = "Item_MaterialsGroupDesc", ResourceType = typeof(Resources.MD.Item))]
        public string MaterialsGroupDesc { get; set; }

        [Display(Name = "BillIOB_StartQty", ResourceType = typeof(Resources.Report.BillIOB))]
        /// <summary>
        /// 期初数量
        /// </summary>
        public decimal StartQty { get; set; }
        [Display(Name = "BillIOB_StartAmount", ResourceType = typeof(Resources.Report.BillIOB))]
        /// <summary>
        /// 期初金额
        /// </summary>
        public decimal StartAmount { get; set; }
        [Display(Name = "BillIOB_InQty", ResourceType = typeof(Resources.Report.BillIOB))]
        /// <summary>
        /// 入数量
        /// </summary>
        public decimal InQty { get; set; }
        [Display(Name = "BillIOB_InAmount", ResourceType = typeof(Resources.Report.BillIOB))]
        /// <summary>
        /// 入金额
        /// </summary>
        public decimal InAmount { get; set; }
        [Display(Name = "BillIOB_OutQty", ResourceType = typeof(Resources.Report.BillIOB))]
        /// <summary>
        /// 出数量
        /// </summary>
        public decimal OutQty { get; set; }
        [Display(Name = "BillIOB_OutAmount", ResourceType = typeof(Resources.Report.BillIOB))]
        /// <summary>
        /// 出金额
        /// </summary>
        public decimal OutAmount { get; set; }
        [Display(Name = "BillIOB_EndQty", ResourceType = typeof(Resources.Report.BillIOB))]
        /// <summary>
        /// 期末数量
        /// </summary>
        public decimal EndQty { get; set; }
        [Display(Name = "BillIOB_EndAmount", ResourceType = typeof(Resources.Report.BillIOB))]
        /// <summary>
        /// 期末金额
        /// </summary>
        public decimal EndAmount { get; set; }
    }

  
}
