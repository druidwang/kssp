using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using com.Sconit.SmartDevice.SmartDeviceRef;
using System.Web.Services.Protocols;

namespace com.Sconit.SmartDevice
{
    public partial class UCWMSDeliveryBarCode111 : UCBase
    {
        //public event MainForm.ModuleSelectHandler ModuleSelectionEvent;

        private List<PickTask> pickTasks;

        private string binCode;
        private DateTime? effDate;
        private bool isPickByHu;
        //private List<IpDetailInput> ipDetailProcess;
        private static UCWMSDeliveryBarCode111 ucWMSDeliveryBarCode;
        private static object obj = new object();

        public UCWMSDeliveryBarCode111(User user)
            : base(user)
        {
            this.InitializeComponent();
            base.btnOrder.Text = "配送标签扫描";
        }

        public static UCWMSDeliveryBarCode111 GetUCWMSDeliveryBarCode(User user)
        {
            if (ucWMSDeliveryBarCode == null)
            {
                lock (obj)
                {
                    if (ucWMSDeliveryBarCode == null)
                    {
                        ucWMSDeliveryBarCode = new UCWMSDeliveryBarCode111(user);
                    }
                }
            }
            ucWMSDeliveryBarCode.user = user;
            ucWMSDeliveryBarCode.Reset();
            ucWMSDeliveryBarCode.lblMessage.Text = "请扫描条码\\配送标签";
            return ucWMSDeliveryBarCode;
        }

        #region Event

        protected override void ScanBarCode()
        {
            base.ScanBarCode();


            if (base.op == CodeMaster.BarCodeType.HU.ToString())
            {

                Hu hu = smartDeviceService.GetHu(barCode);
                MatchPickTask(hu);

            }
            else
            {
                throw new BusinessException("条码格式不合法");
            }

        }

        #endregion


        private void MatchPickTask(Hu hu)
        {
            if (hu == null)
            {
                throw new BusinessException("条码不存在");
            }
        }


        #region DataBind


        protected override void gvListDataBind()
        {
            this.tbBarCode.Text = string.Empty;
            this.ts = new DataGridTableStyle();

            this.ts.GridColumnStyles.Add(columnItemCode);
            this.ts.GridColumnStyles.Add(columnItemDescription);
            this.ts.GridColumnStyles.Add(columnLotNo);
            if (isPickByHu == true)
            {
                this.ts.GridColumnStyles.Add(columnHuId);
            }
            //this.ts.GridColumnStyles.Add(columnIsOdd);
            this.ts.GridColumnStyles.Add(columnBin);
            this.ts.GridColumnStyles.Add(columnUom);
            this.ts.GridColumnStyles.Add(columnReferenceItemCode);

            this.dgList.TableStyles.Clear();
            this.dgList.TableStyles.Add(this.ts);

            this.ResumeLayout();
            this.isMasterBind = true;
        }

        #endregion

        #region Init Reset
        protected override void Reset()
        {
            base.Reset();
            this.effDate = null;
        }
        #endregion

        protected override void DoSubmit()
        {

            this.Reset();
            //base.lblMessage.Text = string.Format("收货成功,收货单号:{0}", receiptNo);
            this.isMark = true;
        }

      
        protected override Hu DoCancel()
        {
            Hu firstHu = base.DoCancel();
            this.CancelHu(firstHu);
            return firstHu;
        }

        private void VerifyPermission(string partyTo)
        {
            if (!this.user.Permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Region)
                .Select(t => t.PermissionCode).Contains(partyTo))
            {
                throw new BusinessException("没有目的区域权限不能收货。");
            }
        }

        private void CancelHu(Hu hu)
        {
            //if (this.ipMaster == null && (this.orderMasters == null || this.orderMasters.Count() == 0))
            if (this.hus == null)
            {
                //this.ModuleSelectionEvent(CodeMaster.TerminalPermission.M_Switch);
                this.Reset();
                return;
            }

            if (hu != null)
            {
                base.hus = base.hus.Where(h => !h.HuId.Equals(hu.HuId, StringComparison.OrdinalIgnoreCase)).ToList();
                this.gvHuListDataBind();
            }
        }



    }
}
