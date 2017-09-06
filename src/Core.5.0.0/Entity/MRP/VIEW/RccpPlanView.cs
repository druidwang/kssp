using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.VIEW
{
    #region
    public class PlanView
    {
        public PlanHead PlanHead { get; set; }
        public List<PlanBody> PlanBodyList { get; set; }
    }

    public class PlanHead
    {
        public string LocationOrFlowHead = "LocationOrFlow";
        public string ItemHead = "Item";
        public string ItemDescriptionHead = "ItemDescription";
        public string ReferenceItemCodeHead = "ReferenceItemCode";
        public string UomHead = "Uom";
        public List<PlanColumnCell> ColumnCellList { get; set; }
    }

    public class PlanBody
    {
        public string LocationOrFlow { get; set; }
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public string ItemReference { get; set; }
        public string Uom { get; set; }
        public List<PlanRowCell> RowCellList { get; set; }
    }

    public class PlanRowCell
    {
        public string LocationOrFlow { get; set; }
        public string Item { get; set; }
        public DateTime PlanDate { get; set; }
        public Double OrderQty { get; set; }
        public Double PlanQty { get; set; }
        public Double StockQty { get; set; }
        public string DisplayQty
        {
            get
            {
                if (this.OrderQty == 0 && this.PlanQty == 0 && this.StockQty == 0)
                {
                    return "-";
                }
                else if (this.OrderQty != 0 && this.PlanQty != 0 && this.StockQty == 0)
                {
                    return this.PlanQty.ToString("#,##0.##") + "/" + this.OrderQty.ToString("#,##0.##");
                }
                else if (this.OrderQty == 0 && this.PlanQty != 0 && this.StockQty == 0)
                {
                    return this.PlanQty.ToString("#,##0.##");
                }
                else
                {
                    return this.PlanQty.ToString("#,##0.##") + "/" + this.OrderQty.ToString("#,##0.##") + "/" + this.StockQty.ToString("#,##0.##");
                }
            }
        }

        public string TotalQty
        {
            get
            {
                return (this.OrderQty + this.PlanQty + this.StockQty).ToString("#,##0.##");
            }
        }
    }

    public class PlanColumnCell
    {
        public DateTime PlanDate { get; set; }

        public string DisplayHead
        {
            get
            {
                return PlanDate.ToString("MM-dd");
            }
        }
    }
    #endregion


    #region //////////////Rccp//////////////////
    public class RccpPlanView
    {
        public RccpPlanHead PlanHead { get; set; }
        public List<RccpPlanBody> PlanBodyList { get; set; }
    }

    public class RccpPlanHead
    {
        public string FlowHead = "Item";
        public string MachineHead = "Item";
        public string ItemHead = "Item";
        public string ItemDescriptionHead = "ItemDescription";
        public string ReferenceItemCodeHead = "ItemReference";
        public string UomHead = "Uom";
        public List<RccpPlanColumnCell> ColumnCellList { get; set; }
    }

    public class RccpPlanBody
    {
        public string Flow { get; set; }
        public string Machine { get; set; }
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public string ItemReference { get; set; }
        public string Uom { get; set; }
        public List<RccpPlanRowCell> RowCellList { get; set; }
    }

    public class RccpPlanRowCell
    {
        public string Flow { get; set; }
        public string Machine { get; set; }
        public string Item { get; set; }
        public string DateIndex { get; set; }
        public int PlanVersion { get; set; }
        //������
        public double PlanQty { get; set; }
        public string DisPlanQty
        {
            get
            {
                if (this.PlanQty == 0)
                {
                    return "-";
                }
                else
                {
                    return this.PlanQty.ToString("#,##0.##");
                }
            }
        }
    }

    public class RccpPlanColumnCell
    {
        public string DateIndex { get; set; }
    }

    #endregion

    #region //////////////RccpMi//////////////////
    public class RccpPlanMiView
    {
        public RccpPlanMiHead PlanHead { get; set; }
        public List<RccpPlanMiBody> PlanBodyList { get; set; }
    }

    public class RccpPlanMiHead
    {
        public string FlowHead = "Flow";
        public string ItemHead = "Item";
        public string ItemDescriptionHead = "ItemDescription";
        public string ReferenceItemCodeHead = "ItemReference";
        public string UomHead = "Uom";
        public List<RccpPlanMiColumnCell> ColumnCellList { get; set; }
    }

    public class RccpPlanMiBody
    {
        public string Flow { get; set; }
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public string ItemReference { get; set; }
        public string Uom { get; set; }
        public List<RccpPlanMiRowCell> RowCellList { get; set; }
    }

    public class RccpPlanMiRowCell
    {
        public string Flow { get; set; }
        public string Item { get; set; }
        public string WeekOfYear { get; set; }
        public int PlanVersion { get; set; }
        //������
        public double LoadBalance { get; set; }
        public string DisLoadBalance
        {
            get
            {
                if (this.LoadBalance == 0)
                {
                    return "-";
                }
                else
                {
                    return this.LoadBalance.ToString("#,##0.##");
                }
            }
        }
        //���ù�ʱ
        public double TotalWorkHour { get; set; }
        public string DisTotalWorkHour
        {
            get
            {
                if (this.TotalWorkHour == 0)
                {
                    return "-";
                }
                else
                {
                    return this.TotalWorkHour.ToString("#,##0.##");
                }
            }
        }
        //�ƻ���ʱ
        public double PlanWorkHour { get; set; }
        public string DisPlanWorkHour
        {
            get
            {
                if (this.PlanWorkHour == 0)
                {
                    return "-";
                }
                else
                {
                    return this.PlanWorkHour.ToString("#,##0.##");
                }
            }
        }
        //�ƻ�����
        public double PlanQty { get; set; }
        public string DisPlanQty
        {
            get
            {
                if (this.PlanQty == 0)
                {
                    return "-";
                }
                else
                {
                    return this.PlanQty.ToString("#,##0.##");
                }
            }
        }
        //��Э����
        public double SubQty { get; set; }
        public string DisSubQty
        {
            get
            {
                if (this.SubQty == 0)
                {
                    return "-";
                }
                else
                {
                    return this.SubQty.ToString("#,##0.##");
                }
            }
        }
    }

    public class RccpPlanMiColumnCell
    {
        public string WeekOfYear { get; set; }
    }

    #endregion

    #region //////////////RccpMiCapacity//////////////////
    public class RccpPlanMiCapacityView
    {
        public RccpPlanMiCapacityHead PlanHead { get; set; }
        public List<RccpPlanMiCapacityBody> PlanBodyList { get; set; }
    }

    public class RccpPlanMiCapacityHead
    {
        public string ProductLineHead = "ProductLine";
        public string WeekOfYearHead = "WeekOfYear";

        /// <summary>
        /// ���� ���ù�ʱ
        /// </summary>
        public string CapacityHead = "Capacity";
        /// <summary>
        /// ���� �ƻ���ʱ
        /// </summary>
        public string QtyHead = "Qty";
        /// <summary>
        /// ί������
        /// </summary>
        public string SubQtyHead = "SubQty";
        /// <summary>
        /// �ƻ�����
        /// </summary>
        public string PlanQtyHead = "PlanQty";

        /// <summary>
        /// ����
        /// </summary>
        public string LoadBalanceHead = "LoadBalance";
    }

    public class RccpPlanMiCapacityBody
    {
        //���ù�ʱ	�ƻ���ʱ	������	�ƻ�����	��Э����

        public string ProductLine { get; set; }
        public string WeekOfYear { get; set; }
        /// <summary>
        /// ���� ���ù�ʱ
        /// </summary>
        public Double Capacity { get; set; }
        /// <summary>
        /// ���� �ƻ���ʱ
        /// </summary>
        public Double Qty { get; set; }
        /// <summary>
        /// ί������
        /// </summary>
        public Double SubQty { get; set; }
        /// <summary>
        /// �ƻ�����
        /// </summary>
        public Double PlanQty { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public string LoadBalance
        {
            get
            {
                return (this.Qty / this.Capacity).ToString("0.##%");
            }
        }
    }


    #endregion

    #region //////////////RccpPr//////////////////
    public class RccpPlanPrView
    {
        public RccpPlanPrHead PlanHead { get; set; }
        public List<RccpPlanPrBody> PlanBodyList { get; set; }
    }

    public class RccpPlanPrHead
    {
        public string FlowHead = "Flow";
        public string MachineHead = "Machine";
        public string ItemHead = "Item";
        public string ItemDescriptionHead = "ItemDescription";
        public string ReferenceItemCodeHead = "ItemReference";
        public string UomHead = "Uom";
        public List<RccpPlanPrColumnCell> ColumnCellList { get; set; }
    }

    public class RccpPlanPrBody
    {
        public string Flow { get; set; }
        public string Machine { get; set; }
        public string Item { get; set; }
        public string ItemDescription { get; set; }
        public string ItemReference { get; set; }
        public string Uom { get; set; }
        public List<RccpPlanPrRowCell> RowCellList { get; set; }
    }

    public class RccpPlanPrRowCell
    {
        public string Flow { get; set; }
        public string Machine { get; set; }
        public string Item { get; set; }
        public string WeekOfYear { get; set; }
        public int PlanVersion { get; set; }


        public Double ShiftQuota { get; set; }
        public string DisShiftQuota
        {
            get
            {
                if (this.ShiftQuota == 0)
                {
                    return "-";
                }
                else
                {
                    return this.ShiftQuota.ToString("#,##0.##");
                }
            }
        }

        //������
        public double LoadBalance { get; set; }
        public string DisLoadBalance
        {
            get
            {
                if (this.LoadBalance == 0)
                {
                    return "-";
                }
                else
                {
                    return this.LoadBalance.ToString("#,##0.##");
                }
            }
        }

        //���ð��
        public double TotalShift { get; set; }
        public string DisTotalShift
        {
            get
            {
                if (this.TotalShift == 0)
                {
                    return "-";
                }
                else
                {
                    return this.TotalShift.ToString("#,##0.##");
                }
            }
        }

        //�ƻ����
        public double PlanShift { get; set; }
        public string DisPlanShift
        {
            get
            {
                if (this.PlanShift == 0)
                {
                    return "-";
                }
                else
                {
                    return this.PlanShift.ToString("#,##0.##");
                }
            }
        }

        //�ƻ����� 
        public double PlanQty { get; set; }
        //�����ײ���
        public double SubQty { get; set; }
        //����
        public string DisPlanQty
        {
            get
            {
                if (this.PlanQty == 0 && this.SubQty == 0)
                {
                    return "-";
                }
                else if (this.PlanQty > 0 && this.SubQty == 0)
                {
                    return this.PlanQty.ToString("#,##0.##");
                }
                else
                {
                    return this.PlanQty.ToString("#,##0.##") + "(" + this.SubQty.ToString("#,##0.##") + ")";
                }
            }
        }

    }

    public class RccpPlanPrColumnCell
    {
        public string WeekOfYear { get; set; }
    }
    #endregion

}
