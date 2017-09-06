using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.Sconit.Entity
{
    public class TMSConstants
    {

        public static readonly string CODE_PREFIX_TMS_BILL = "TBL";

        public static readonly string CODE_PREFIX_ROUTE = "RTE";

        public static readonly string CODE_PREFIX_FREIGHT = "FRT";

        public static readonly string PARTY_TYPE_CARRIER = "Carrier";

        public static readonly string CODE_MASTER_PARTY_TYPE_VALUE_CARRIER = "Carrier";

        public static readonly string CODE_MASTER_PERMISSION_CATEGORY_TYPE_VALUE_CARRIER = "Carrier";

        public static readonly string CODE_MASTER_TMS_PRICELIST = "TMSPriceList";
        /// <summary>
        /// 包车
        /// </summary>
        public static readonly string CODE_MASTER_TMS_PRICELIST_BUS = "Bus";
        /// <summary>
        /// 体积(立方米)
        /// </summary>
        public static readonly string CODE_MASTER_TMS_PRICELIST_STERE = "Stere";
        /// <summary>
        /// 单位
        /// </summary>
        public static readonly string CODE_MASTER_TMS_PRICELIST_UOM = "UOM";
        /// <summary>
        /// 阶梯
        /// </summary>
        public static readonly string CODE_MASTER_TMS_PRICELIST_LADDERSTERE = "LadderStere";
        /// <summary>
        /// 包车+里程
        /// </summary>
        public static readonly string CODE_MASTER_TMS_PRICELIST_BUS_KM = "BusKM";
        /// <summary>
        /// 包车+体积
        /// </summary>
        public static readonly string CODE_MASTER_TMS_PRICELIST_BUS_STERE = "BusStere";
        /// <summary>
        /// 包车+阶梯
        /// </summary>
        public static readonly string CODE_MASTER_TMS_PRICELIST_BUS_LADDER = "BusLadder";

        /// <summary>
        /// 重量(公斤)
        /// </summary>
        public static readonly string CODE_MASTER_TMS_PRICELIST_KG = "KG";
        /// <summary>
        /// 面积(平方米)
        /// </summary>
        public static readonly string CODE_MASTER_TMS_PRICELIST_SQM = "SQM";
        /// <summary>
        /// 托盘
        /// </summary>
        public static readonly string CODE_MASTER_TMS_PRICELIST_PALLET = "Pallet";
        /// <summary>
        /// 箱子
        /// </summary>
        public static readonly string CODE_MASTER_TMS_PRICELIST_UNITPACK = "UnitPack";
        /// <summary>
        /// 包月
        /// </summary>
        public static readonly string CODE_MASTER_TMS_PRICELIST_MONTHLY = "Monthly";

        public static readonly string CODE_MASTER_TMS_FREIGHTSTATUS = "TMSFreightStatus";
        public static readonly string CODE_MASTER_TMS_FREIGHTSTATUS_CREATE = "Create";
        public static readonly string CODE_MASTER_TMS_FREIGHTSTATUS_SUBMIT = "Submit";
        public static readonly string CODE_MASTER_TMS_FREIGHTSTATUS_CANCEL = "Cancel";
        public static readonly string CODE_MASTER_TMS_FREIGHTSTATUS_CLOSE = "Close";

        public static readonly string CODE_MASTER_TMS_STATUS = "TMSStatus";
        public static readonly string CODE_MASTER_TMS_STATUS_VALUE_CREATE = "Create";
        public static readonly string CODE_MASTER_TMS_STATUS_VALUE_SUBMIT = "Submit";
        public static readonly string CODE_MASTER_TMS_STATUS_VALUE_INPROCESS = "In-Process";
        public static readonly string CODE_MASTER_TMS_STATUS_VALUE_COMPLETE = "Complete";
        public static readonly string CODE_MASTER_TMS_STATUS_VALUE_CANCEL = "Cancel";
        public static readonly string CODE_MASTER_TMS_STATUS_VALUE_CLOSE = "Close";

        public static readonly string CODE_MASTER_TMS_BILLSTATUS = "TMSBillStatus";
        public static readonly string CODE_MASTER_TMS_BILLSTATUS_CREATE = "Create";
        public static readonly string CODE_MASTER_TMS_BILLSTATUS_SUBMIT = "Submit";
        public static readonly string CODE_MASTER_TMS_BILLSTATUS_CANCEL = "Cancel";
        public static readonly string CODE_MASTER_TMS_BILLSTATUS_CLOSE = "Close";

        public static readonly string PERMISSION_PAGE_VALUE_FREIGHTRELEASE = "FreightRelease";
        public static readonly string PERMISSION_PAGE_VALUE_FREIGHTVIEW = "FreightView";
        public static readonly string PERMISSION_PAGE_VALUE_TMS_VIEW = "TMSView";

        public static readonly string CODE_MASTER_TMS_TYPE = "TMSType";
        public static readonly string CODE_MASTER_TMS_TYPE_NML = "NML";
        public static readonly string CODE_MASTER_TMS_TYPE_COM = "COM";
        public static readonly string CODE_MASTER_TMS_TYPE_FMT = "FMT";
        public static readonly string CODE_MASTER_TMS_TYPE_RST = "RST";

        public static readonly string CODE_MASTER_TMS_SUBTYPE = "TMSSubType";
        public static readonly string CODE_MASTER_TMS_SUBTYPE_RETURN = "Return";

        public static readonly string PERMISSION_PAGE_VALUE_BILL_VIEW = "BillView";
        public static readonly string PERMISSION_PAGE_VALUE_BILL_EDIT = "BillEdit";


        public static readonly string PERMISSION_SETUP_REPORT_VALUE_TMSREP = "TMSRep";
    }
}
