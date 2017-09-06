using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using com.Sconit.Entity.INV;

namespace com.Sconit.Web.Models.INV
{
    public class LocationTransactionView : LocationTransaction
    {
        [Display(Name = "LocationTransaction_Location", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public string GroupLocation { get; set; }

        [Display(Name = "LocationTransaction_ProcurementInQty", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public decimal SumProcurementInQty { get; set; }

        [Display(Name = "LocationTransaction_DistributionOutQty", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public decimal SumDistributionOutQty { get; set; }

        [Display(Name = "LocationTransaction_TransferInQty", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public decimal SumTransferInQty { get; set; }

        [Display(Name = "LocationTransaction_TransferOutQty", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public decimal SumTransferOutQty { get; set; }

        [Display(Name = "LocationTransaction_ProductionInQty", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public decimal SumProductionInQty { get; set; }

        [Display(Name = "LocationTransaction_ProductionOutQty", ResourceType = typeof(Resources.INV.LocationTransaction))]
        public decimal SumProductionOutQty { get; set; }

    }
}
