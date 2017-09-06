namespace com.Sconit.Entity.SI.SD_SCM
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class FlowDetail
    {
        #region O/R Mapping Properties

        public Int32 Id { get; set; }

        public string Flow { get; set; }

        //public com.Sconit.CodeMaster.FlowStrategy Strategy { get; set; }

        public Int32 Sequence { get; set; }

        public string Item { get; set; }

        public string ReferenceItemCode { get; set; }

        public string Uom { get; set; }

        public Decimal? UnitCount { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string LocationFrom { get; set; }

        public string LocationTo { get; set; }
 
        #endregion

        #region �����ֶ�
        public List<FlowDetailInput> FlowDetailInputs { get; set; }
        public decimal CurrentQty { get; set; }
        public int Carton { get; set; }
        #endregion
    }
    public class FlowDetailInput
    {
        public string HuId { get; set; }
        public decimal Qty { get; set; }
        public string LotNo { get; set; }
        public string Direction { get; set; }
        public string Item { get; set; }
        public string Uom { get; set; }
        public CodeMaster.QualityType QualityType { get; set; }


        //Ϊ�˿��������䷢��֧�ֶ��λ
        public string LocFrom { get; set; }
    }

}
