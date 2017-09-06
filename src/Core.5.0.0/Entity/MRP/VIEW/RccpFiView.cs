using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity.MRP.TRANS;

//TODO: Add other using statements here

namespace com.Sconit.Entity.MRP.VIEW
{
    public class RccpFiView
    {
        #region ��������RccpFiPlan��Key
        public string ProductLine { get; set; }
        public string DateIndex { get; set; }
        public CodeMaster.TimeUnit DateType { get; set; }
        //ģ��
        public string Machine { get; set; }

        //���� ����
        public Double Qty { get; set; }
        //�����
        public Double RequiredShiftQty { get; set; }
        #endregion

        #region ��������MachineInstance
        public int Seq { get; set; }
        //ģ������
        public string Description { get; set; }
        //����
        public string Island { get; set; }
        //��������
        public string IslandDescription { get; set; }
        //��������
        public Double IslandQty { get; set; }
        //ģ������
        public Double MachineQty { get; set; }
        //8Сʱ�������
        public Double ShiftQuota { get; set; }
        
        //���� 2��/3��
        public com.Sconit.CodeMaster.ShiftType ShiftType { get; set; }
        //������������
        public Double NormalWorkDay { get; set; }
        //���������
        public Double MaxWorkDay { get; set; }

        //���/��
        public int ShiftPerDay { get; set; }
        //ģ������
        public com.Sconit.CodeMaster.MachineType MachineType { get; set; }
        //ͣ��ʱ��
        public double HaltTime { get; set; }
        //����ʱ��
        public double TrialProduceTime { get; set; }
        //�ڼ���
        public double Holiday { get; set; }
        #endregion
        //���������
        public double NormalShiftQty { get; set; }
        //����ΰ����
        public double MaxShiftQty { get; set; }
        //��������
        public double NormalQty { get; set; }
        //ģ��������
        public double MaxQty { get; set; }
        //��ǰ���������
        public double CurrentNormalShiftQty { get; set; }
        //��ǰ����ΰ����
        public double CurrentMaxShiftQty { get; set; }
        //��ǰģ��������  
        public double CurrentMaxQty { get; set; }
        public double CurrentNormalQty { get; set; }

        //ģ��/���� ������
        public double RequiredFactQty { get; set; }
        public double CurrentRequiredFactQty { get; set; }

        //��������
        public double DiffQty { get; set; }
        public double CurrentDiffQty { get; set; }
        //��Ҫ�����
        public double RequiredShiftPerDay { get; set; }

        //һ������������
        public double ItemCount { get; set; }
        //����
        public double KitQty { get; set; }
        public double KitShiftQuota { get; set; }

        //�����齨
        public double ModelRate { get; set; }

        public List<RccpFiPlan> RccpFiPlanList { get; set; }

        public List<RccpFiView> RccpFiViewList { get; set; }

    }
}
