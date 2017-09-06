using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using com.Sconit.SmartDevice.SmartDeviceRef;

namespace com.Sconit.SmartDevice
{
    public partial class UCModuleSelect : UserControl
    {
        public event com.Sconit.SmartDevice.MainForm.ModuleSelectHandler ModuleSelectionEvent;
        public event com.Sconit.SmartDevice.MainForm.ModuleSelectExitHandler ModuleSelectExitEvent;
        private User user;
        private Dictionary<CodeMaster.TerminalPermission, List<object>> dicModule = new Dictionary<CodeMaster.TerminalPermission, List<object>>();

        public UCModuleSelect(User user)
        {
            this.user = user;
            InitializeComponent();
            this.lblUserStatus.Text = string.Format("当前用户:{0}[{1}{2}]", user.Code, user.FirstName, user.LastName);

            this.CheckAccessPermission();
            this.tabModuleSelect.SelectedIndex = this.GetSelectedIndex();
            if (this.user.Code != "su")
            {
                this.tbKeyCode.Visible = false;
            }
        }

        private void CheckAccessPermission()
        {
            #region Object Dictionary
            Dictionary<CodeMaster.TerminalPermission, List<object>> dicObject = new Dictionary<CodeMaster.TerminalPermission, List<object>>();
            //收发

            dicObject.Add(CodeMaster.TerminalPermission.Client_Receive, new List<object> { this.btnReceive, 0, Keys.D1, Keys.NumPad1 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_PurchaseReturn, new List<object> { this.btnPurchaseReturn, 0, Keys.D2, Keys.NumPad2 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_QuickTransfer, new List<object> { this.btnQuickTransfer, 0, Keys.D3, Keys.NumPad3 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_QuickReturn, new List<object> { this.btnQuickReturn, 0, Keys.D4, Keys.NumPad4 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_ProductionReceive, new List<object> { this.btnProductionReceive, 0, Keys.D5, Keys.NumPad5 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_ProductionReturn, new List<object> { this.btnProductionReturn, 0, Keys.D6, Keys.NumPad6 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_OrderShip, new List<object> { this.btnOrderShip, 0, Keys.D7, Keys.NumPad7 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_DistributionReturn, new List<object> { this.btnDistributionReturn, 0, Keys.D8, Keys.NumPad8 });

            //dicObject.Add(CodeMaster.TerminalPermission.Client_PickListOnline, new List<object> { this.btnPickListOnline, 0, Keys.D6, Keys.NumPad6 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_PickList, new List<object> { this.btnPickList, 0, Keys.D7, Keys.NumPad7 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_PickListShip, new List<object> { this.btnPickListShip, 0, Keys.D8, Keys.NumPad8 });


            //生产
            //dicObject.Add(CodeMaster.TerminalPermission.Client_Filter, new List<object> { this.btnFilter, 1, Keys.D1, Keys.NumPad1 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_StartAging, new List<object> { this.btnStartAging, 1, Keys.D2, Keys.NumPad2 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_Aging, new List<object> { this.btnAging, 1, Keys.D3, Keys.NumPad3 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_ProductionOnline, new List<object> { this.btnProductionOnline, 1, Keys.D1, Keys.NumPad1 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_ProductionOffline, new List<object> { this.btnProdutionOffline, 1, Keys.D2, Keys.NumPad2 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_MaterialIn, new List<object> { this.btnMaterialIn, 1, Keys.D3, Keys.NumPad3 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_MaterialReturn, new List<object> { this.btnMaterialReturn, 1, Keys.D4, Keys.NumPad4 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_AnDon, new List<object> { this.btnAnDon, 1, Keys.D3, Keys.NumPad3 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_ForceMaterialIn, new List<object> { this.btnForceMaterialIn, 1, Keys.D5, Keys.NumPad5 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_SeqPack, new List<object> { this.btnSeqPack, 1, Keys.D6, Keys.NumPad6 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_FiReceipt, new List<object> { this.btnFiReceipt, 1, Keys.D0, Keys.NumPad0 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_SparePartChk, new List<object> { this.btnSpChk, 1, Keys.D4, Keys.NumPad4 });

            //仓库
            dicObject.Add(CodeMaster.TerminalPermission.Client_Transfer, new List<object> { this.btnTransfer, 2, Keys.D1, Keys.NumPad1 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_RePack, new List<object> { this.btnReBinning, 2, Keys.D2, Keys.NumPad2 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_PutAway, new List<object> { this.btnPutAway, 2, Keys.D3, Keys.NumPad3 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_Pickup, new List<object> { this.btnPickUp, 2, Keys.D4, Keys.NumPad4 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_Pack, new List<object> { this.btnBinning, 2, Keys.D5, Keys.NumPad5 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_UnPack, new List<object> { this.btnDevanning, 2, Keys.D6, Keys.NumPad6 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_StockTaking, new List<object> { this.btnStockTaking, 2, Keys.D7, Keys.NumPad7 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_HuStatus, new List<object> { this.btnHuStatus, 2, Keys.D8, Keys.NumPad8 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_MiscInOut, new List<object> { this.btnMiscInOut, 2, Keys.D9, Keys.NumPad9 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_HuClone, new List<object> { this.btnHuClone, 2, Keys.D0, Keys.NumPad0 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_BindContainerIn, new List<object> { this.btnBindContainerIn, 2, Keys.D0, Keys.NumPad0 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_BindContainerOut, new List<object> { this.btnBindContainerOut, 2, Keys.D0, Keys.NumPad0 });

            //质量
            dicObject.Add(CodeMaster.TerminalPermission.Client_Inspect, new List<object> { this.btnInspect, 3, Keys.D1, Keys.NumPad1 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_Qualify, new List<object> { this.btnQualify, 3, Keys.D2, Keys.NumPad2 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_Freeze, new List<object> { this.btnFreeze, 3, Keys.D3, Keys.NumPad3 });
            dicObject.Add(CodeMaster.TerminalPermission.Client_UnFreeze, new List<object> { this.btnUnfreeze, 3, Keys.D3, Keys.NumPad3 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_WorkerWaste, new List<object> { this.btnWorkerWaste, 3, Keys.D6, Keys.NumPad6 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_Reject, new List<object> { this.btnReject, 3, Keys.D3, Keys.NumPad3 });

            //高级仓库
            //dicObject.Add(CodeMaster.TerminalPermission.Client_WMSPickGoods, new List<object> { this.btnWMSPickGoods, 4, Keys.D1, Keys.NumPad1 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_WMSDeliverBarCode, new List<object> { this.btnWMSDeliverBarCode, 4, Keys.D2, Keys.NumPad2 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_WMSPickGoodsQty, new List<object> { this.btnWMSPickGoodsQty, 4, Keys.D3, Keys.NumPad3 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_WMSTransfer, new List<object> { this.btnWMSTransfer, 4, Keys.D4, Keys.NumPad4 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_WMSShip, new List<object> { this.btnWMSShip, 4, Keys.D5, Keys.NumPad5 });
            //dicObject.Add(CodeMaster.TerminalPermission.Client_WMSRepack, new List<object> { this.btnWMSRepack, 4, Keys.D6, Keys.NumPad6 });

            #endregion

            var permissionList = this.user.Permissions.Where(p => p.PermissionCategoryType == PermissionCategoryType.Terminal)
                .Select(p => p.PermissionCode).ToList();

            foreach (var keyValuePair in dicObject)
            {
                //dicModule.Add(keyValuePair.Key, keyValuePair.Value);
                //((Button)((keyValuePair.Value)[0])).Enabled = false;
                if (permissionList != null)
                {
                    foreach (string permission in permissionList)
                    {
                        if (permission == keyValuePair.Key.ToString())
                        {
                            if (!dicModule.ContainsKey(keyValuePair.Key))
                            {
                                ((Button)((keyValuePair.Value)[0])).Enabled = true;
                                dicModule.Add(keyValuePair.Key, keyValuePair.Value);
                                break;
                            }
                        }
                        //if (permission == CodeMaster.TerminalPermission.Client_Qualify.ToString() && dicModule.All(d => d.Key.ToString() != "Client_Reject"))
                        //{
                        //    this.btnReject.Enabled = true;
                        //    dicModule.Add(CodeMaster.TerminalPermission.Client_Reject, new List<object> { this.btnReject, 3, Keys.D3, Keys.NumPad3 });
                        //    break;
                        //}
                    }
                }
            }
        }

        private void UCModuleSelect_Click(object sender, EventArgs e)
        {
            Button moduleButton = (Button)sender;
            foreach (var keyValuePair in dicModule)
            {
                if (moduleButton == (Button)((keyValuePair.Value)[0]))
                {
                    this.ModuleSelectionEvent(keyValuePair.Key);
                    break;
                }
            }
        }

        private void UCModuleSelect_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                Keys keyEventArgs = e.KeyData & Keys.KeyCode;

                if (this.tabModuleSelect.Focused && keyEventArgs == Keys.Enter)
                {
                    if (this.tabModuleSelect.SelectedIndex == 0)
                    {
                        this.tabModuleSelect.SelectedIndex = 1;
                    }
                    else if (this.tabModuleSelect.SelectedIndex == 1)
                    {
                        this.tabModuleSelect.SelectedIndex = 2;
                    }
                    else if (this.tabModuleSelect.SelectedIndex == 2)
                    {
                        this.tabModuleSelect.SelectedIndex = 3;
                    }
                    else if (this.tabModuleSelect.SelectedIndex == 3)
                    {
                        this.tabModuleSelect.SelectedIndex = 0;
                        //this.tabModuleSelect.SelectedIndex = 4;
                    }
                    else if (this.tabModuleSelect.SelectedIndex == 4)
                    {
                        this.tabModuleSelect.SelectedIndex = 0;
                    }
                    this.tabModuleSelect.Focus();
                }
                else
                {
                    foreach (var keyValuePair in dicModule)
                    {
                        Button dicButton = (Button)((keyValuePair.Value)[0]);
                        int tabPageIndex = (int)((keyValuePair.Value)[1]);
                        Keys keyD = (Keys)((keyValuePair.Value)[2]);
                        Keys keyNumPad = (Keys)((keyValuePair.Value)[3]);

                        if ((keyEventArgs == keyD || keyEventArgs == keyNumPad) && (tabPageIndex == this.tabModuleSelect.SelectedIndex)
                            || (!this.tabModuleSelect.Focused && (Button)sender == dicButton && keyEventArgs == Keys.Enter))
                        {
                            this.ModuleSelectionEvent(keyValuePair.Key);
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            { }
        }

        private void SetSelectedIndex(int selectedIndex)
        {
            Microsoft.Win32.RegistryKey subKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SconitModuleSelectedIndex", true);
            if (subKey == null)
            {
                subKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SconitModuleSelectedIndex");
            }
            subKey.SetValue("SelectedIndex", selectedIndex);
            subKey.Close();
        }

        private int GetSelectedIndex()
        {
            Microsoft.Win32.RegistryKey subKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SconitModuleSelectedIndex");
            if (subKey == null)
            {
                return 0;
            }
            int selectIndex = (int)(subKey.GetValue("SelectedIndex"));
            subKey.Close();
            return selectIndex;
        }

        private void tabModuleSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.SetSelectedIndex(this.tabModuleSelect.SelectedIndex);
            }
            catch (Exception)
            { }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.ModuleSelectExitEvent();
        }

        private void btnLogOff_Click(object sender, EventArgs e)
        {
            this.ModuleSelectExitEvent();
        }

        private void tbKeyCode_KeyUp(object sender, KeyEventArgs e)
        {
            tbKeyCode.Text = e.KeyValue.ToString();
        }
    }
}
