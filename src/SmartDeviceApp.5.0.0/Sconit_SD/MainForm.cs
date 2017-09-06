using System;
using System.Windows.Forms;
using System.Drawing;
using com.Sconit.SmartDevice.SmartDeviceRef;

namespace com.Sconit.SmartDevice
{
    public partial class MainForm : Form
    {
        public delegate void ModuleSelectHandler(CodeMaster.TerminalPermission module);
        public delegate void LoginHandler(string userCode, string password);
        public delegate void ExitHandler();
        public delegate void ModuleSelectExitHandler();

        private UCLogin ucLogin;
        private UCModuleSelect ucModuleSelect;
        private int time, timeOut;

        private User user;
        //private UserPreference userPreference;

        SD_SmartDeviceService smartDeviceService;

        public MainForm()
        {
            InitializeComponent();
            LoadUCLogin();
            smartDeviceService = new SD_SmartDeviceService();
            smartDeviceService.Url = Utility.WEBSERVICE_URL;
            timeOut = 20;
        }

        private void LoadUCLogin()
        {
            try
            {
                if (this.plMain.Controls.Count > 0)
                {
                    this.plMain.Controls.RemoveAt(0);
                }
                this.ucLogin = new UCLogin();
                //
                this.ucLogin.LoginEvent += new LoginHandler(this.ProcessLoginEvent);
                this.ucLogin.ExitEvent += new ExitHandler(this.ProcessExitEvent);
                this.user = null;
                this.plMain.Controls.Add(this.ucLogin);

            }
            catch (Exception ex)
            {
                Utility.ShowMessageBox("网络故障!" + ex.Message);
                Application.Exit();
            }
        }

        private void ProcessLoginEvent(string userCode, string password)
        {
            try
            {
                smartDeviceService.UserAgent = string.Format("{0} {1}{2}",
                    System.Reflection.Assembly.GetCallingAssembly().GetName().Version,
                    Environment.OSVersion.Platform,
                    Environment.OSVersion.Version.ToString());
                this.user = smartDeviceService.GetUser(userCode, Utility.Md5(password));
                this.SwitchModule(CodeMaster.TerminalPermission.M_Switch);
            }
            catch (Exception ex)
            {
                Utility.ShowMessageBox(ex.Message);
                this.ucLogin.InitialLogin();
                this.user = null;
            }
        }

        private void ProcessExitEvent()
        {
            this.Dispose(true);
        }

        private void SwitchModule(CodeMaster.TerminalPermission module)
        {
            if (module == CodeMaster.TerminalPermission.M_Switch)
            {
                if (this.user != null)
                {
                    this.ucModuleSelect = new UCModuleSelect(this.user);
                    this.ucModuleSelect.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                    this.ucModuleSelect.ModuleSelectExitEvent += new ModuleSelectExitHandler(this.LoadUCLogin);
                    this.AddModule(this.ucModuleSelect);
                    this.Text = "模块选择_Sconit_SD";
                }
                else
                {
                    this.ucModuleSelect.ModuleSelectExitEvent += new ModuleSelectExitHandler(this.LoadUCLogin);
                    this.LoadUCLogin();
                }
            }
            else if (module == CodeMaster.TerminalPermission.Client_OrderShip)
            {
                UCShip ucShip = new UCShip(this.user);//.GetUCShip(user);
                ucShip.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucShip);
                ucShip.tbBarCode.Focus();
                this.Text = "发货";
            }
            else if (module == CodeMaster.TerminalPermission.Client_Receive)
            {
                UCReceive ucReceive = new UCReceive(this.user);//.GetUCReceive(this.user);
                ucReceive.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucReceive);
                ucReceive.tbBarCode.Focus();
                this.Text = "收货";
            }
            else if (module == CodeMaster.TerminalPermission.Client_PurchaseReturn)
            {
                UCPurchaseReturn ucPurchaseReturn = new UCPurchaseReturn(this.user);
                ucPurchaseReturn.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucPurchaseReturn);
                ucPurchaseReturn.tbBarCode.Focus();
                this.Text = "采购退货";
            }
            else if (module == CodeMaster.TerminalPermission.Client_Transfer)
            {
                UCTransfer ucTransfer = new UCTransfer(this.user);//.GetUCTransfer(this.user);
                ucTransfer.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucTransfer);
                ucTransfer.tbBarCode.Focus();
                this.Text = "移库";
            }
            else if (module == CodeMaster.TerminalPermission.Client_PickList)
            {
                UCPickList ucPickList = new UCPickList(this.user);
                ucPickList.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucPickList);
                ucPickList.tbBarCode.Focus();
                this.Text = "拣货";
            }
            else if (module == CodeMaster.TerminalPermission.Client_PickListShip)
            {
                UCPickListShip UCPickListShip = new UCPickListShip(this.user);//.GetUCPickListShip(this.user);
                UCPickListShip.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(UCPickListShip);
                UCPickListShip.tbBarCode.Focus();
                this.Text = "拣货发货";
            }
            else if (module == CodeMaster.TerminalPermission.Client_PutAway)
            {
                var ucBinOn = new UCBinOn(this.user);//.GetUCPickUp(this.user);
                ucBinOn.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucBinOn);
                ucBinOn.tbBarCode.Focus();
                this.Text = "上架";
            }
            else if (module == CodeMaster.TerminalPermission.Client_Pickup)
            {
                var ucPickup = new UCPickUp(this.user);//.GetUCPickUp(this.user);
                ucPickup.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucPickup);
                this.Text = "下架";
            }
            else if (module == CodeMaster.TerminalPermission.Client_AnDon)
            {
                UCAnDon ucAnDon = new UCAnDon(this.user);//.GetUCAnDon(this.user);
                ucAnDon.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                AddModule(ucAnDon);
                ucAnDon.tbBarCode.Focus();
                this.Text = "按灯";
                //this.ucDevanning.Height = height;
            }
            else if (module == CodeMaster.TerminalPermission.Client_StockTaking)
            {
                UCStockTaking ucStockTaking = new UCStockTaking(this.user);//.GetUCStockTaking(this.user);
                ucStockTaking.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucStockTaking);
                ucStockTaking.tbBarCode.Focus();
                this.Text = "盘点";
            }

            else if (module == CodeMaster.TerminalPermission.Client_MaterialIn)
            {
                UCMaterialIn ucMaterialIn = new UCMaterialIn(this.user, false);//.GetUCMaterialIn(this.user, false);
                ucMaterialIn.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucMaterialIn);
                ucMaterialIn.tbBarCode.Focus();
                this.Text = "投料";
            }
            else if (module == CodeMaster.TerminalPermission.Client_ForceMaterialIn)
            {
                var ucForceMaterialIn = new UCForceMaterialIn(this.user, false);
                ucForceMaterialIn.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucForceMaterialIn);
                ucForceMaterialIn.tbBarCode.Focus();
                this.Text = "强制投料";
            }
            else if (module == CodeMaster.TerminalPermission.Client_Qualify)
            {
                var ucJudgeInspect = new UCJudgeInspect(this.user);
                ucJudgeInspect.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucJudgeInspect);
                ucJudgeInspect.tbBarCode.Focus();
                this.Text = "合格";
            }
            else if (module == CodeMaster.TerminalPermission.Client_RePack)
            {
                var ucRePack = new UCRePack(this.user);
                ucRePack.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucRePack);
                ucRePack.tbBarCode.Focus();
                this.Text = "翻箱";
            }
            else if (module == CodeMaster.TerminalPermission.Client_UnPack)
            {
                var ucUnPack = new UCUnPack(this.user);
                ucUnPack.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucUnPack);
                ucUnPack.tbBarCode.Focus();
                this.Text = "拆箱";
            }
            else if (module == CodeMaster.TerminalPermission.Client_Pack)
            {
                var ucPack = new UCPack(this.user);
                ucPack.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucPack);
                ucPack.tbBarCode.Focus();
                this.Text = "装箱";
            }
            else if (module == CodeMaster.TerminalPermission.Client_Inspect)
            {
                var ucInspect = new UCInspect(this.user);
                ucInspect.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucInspect);
                ucInspect.tbBarCode.Focus();
                this.Text = "报验";
            }
            else if (module == CodeMaster.TerminalPermission.Client_WorkerWaste)
            {
                var ucWorkerWaste = new UCWorkerWaste(this.user);
                ucWorkerWaste.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucWorkerWaste);
                ucWorkerWaste.tbBarCode.Focus();
                this.Text = "工废";
            }
            else if (module == CodeMaster.TerminalPermission.Client_PickListOnline)
            {
                var ucPickListOnline = new UCPickListOnline(this.user);
                ucPickListOnline.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucPickListOnline);
                ucPickListOnline.tbBarCode.Focus();
                this.Text = "拣货单上线";
            }
            else if (module == CodeMaster.TerminalPermission.Client_HuStatus)
            {
                var ucHuStatus = new UCHuStatus(this.user);
                ucHuStatus.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucHuStatus);
                ucHuStatus.tbBarCode.Focus();
                this.Text = "条码状态";
            }
            else if (module == CodeMaster.TerminalPermission.Client_ProductionOnline)
            {
                UCProductOrderOnline ucProductOrderOnline = new UCProductOrderOnline(this.user);//.GetUCProductOrderOnline(this.user);
                ucProductOrderOnline.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucProductOrderOnline);
                ucProductOrderOnline.tbBarCode.Focus();
                this.Text = "上线";
            }
            else if (module == CodeMaster.TerminalPermission.Client_MiscInOut)
            {
                UCMisInOut ucMisInOut = new UCMisInOut(this.user);//.GetUCMisInOut(this.user);
                ucMisInOut.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucMisInOut);
                ucMisInOut.tbBarCode.Focus();
                this.Text = "计划外出入库";
            }
            else if (module == CodeMaster.TerminalPermission.Client_HuClone)
            {
                UCHuClone ucHuClone = new UCHuClone(this.user);//.GetUCHuClone(this.user);
                ucHuClone.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucHuClone);
                ucHuClone.tbBarCode.Focus();
                this.Text = "条码克隆";
            }
            else if (module == CodeMaster.TerminalPermission.Client_MaterialReturn)
            {
                UCMaterialIn ucMaterialIn = new UCMaterialIn(this.user, true);//.GetUCMaterialIn(this.user, true);
                ucMaterialIn.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucMaterialIn);
                ucMaterialIn.tbBarCode.Focus();
                this.Text = "退料";
            }
            else if (module == CodeMaster.TerminalPermission.Client_Freeze)
            {
                UCFreeze ucFreeze = new UCFreeze(this.user);//.GetUCFreeze(this.user);
                ucFreeze.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucFreeze);
                ucFreeze.tbBarCode.Focus();
                this.Text = "库存冻结";
            }
            else if (module == CodeMaster.TerminalPermission.Client_UnFreeze)
            {
                UCUnFreeze ucUnFreeze = new UCUnFreeze(this.user);//.GetUCUnFreeze(this.user);
                ucUnFreeze.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucUnFreeze);
                ucUnFreeze.tbBarCode.Focus();
                this.Text = "库存冻结";
            }
            else if (module == CodeMaster.TerminalPermission.Client_QuickReturn)
            {
                UCQuickReturn ucQuickReturn = new UCQuickReturn(this.user);
                ucQuickReturn.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucQuickReturn);
                ucQuickReturn.tbBarCode.Focus();
                this.Text = "领料退库";
            }

            else if (module == CodeMaster.TerminalPermission.Client_ProductionOffline)
            {
                UCReceiveProdOrder ucReceiptProdOrder = new UCReceiveProdOrder(this.user);
                ucReceiptProdOrder.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(ucReceiptProdOrder);
                ucReceiptProdOrder.tbBarCode.Focus();
                this.Text = "生产收货";
            }
            else if (module == CodeMaster.TerminalPermission.Client_FiReceipt)
            {
                UCFiReceipt uc = new UCFiReceipt(this.user);//.GetUCFiReceipt(this.user);
                uc.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(uc);
                uc.tbBarCode.Focus();
                this.Text = "后加工生产入库";
            }
            else if (module == CodeMaster.TerminalPermission.Client_StartAging)
            {
                UCHuAging uc = new UCHuAging(this.user, true);//.GetUCHuAging(this.user, true);
                uc.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(uc);
                uc.tbBarCode.Focus();
                this.Text = "老化开始";
            }
            else if (module == CodeMaster.TerminalPermission.Client_Aging)
            {
                UCHuAging uc = new UCHuAging(this.user, false);//.GetUCHuAging(this.user, false);
                uc.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(uc);
                uc.tbBarCode.Focus();
                this.Text = "老化结束";
            }
            else if (module == CodeMaster.TerminalPermission.Client_Filter)
            {
                UCHuFilter uc = new UCHuFilter(this.user);//.GetUCHuFilter(this.user);
                uc.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(uc);
                uc.tbBarCode.Focus();
                this.Text = "过滤";
            }
            else if (module == CodeMaster.TerminalPermission.Client_SparePartChk)
            {
                UCSpChk uc = new UCSpChk(this.user);//.GetUCHuFilter(this.user);
                uc.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(uc);
                uc.tbBarCode.Focus();
                this.Text = "Check";
            }
            else if (module == CodeMaster.TerminalPermission.Client_WMSPickGoods)
            {
                UCWMSPickGoods uc = new UCWMSPickGoods(this.user, true);//.GetUCHuFilter(this.user);
                uc.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(uc);
                uc.tbBarCode.Focus();
                this.Text = "条码拣货";
            }
            else if (module == CodeMaster.TerminalPermission.Client_WMSPickGoodsQty)
            {
                UCWMSPickGoods uc = new UCWMSPickGoods(this.user, false);//.GetUCHuFilter(this.user);
                uc.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(uc);
                uc.tbBarCode.Focus();
                this.Text = "批次拣货";
            }
            else if (module == CodeMaster.TerminalPermission.Client_WMSDeliverBarCode)
            {
                UCWMSDeliveryBarCode uc = new UCWMSDeliveryBarCode(this.user);
                uc.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(uc);
                uc.tbBarCode.Focus();
                this.Text = "配送标签扫描";
            }
            else if (module == CodeMaster.TerminalPermission.Client_WMSTransfer)
            {
                UCWMSTransfer uc = new UCWMSTransfer(this.user);
                uc.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(uc);
                uc.tbBarCode.Focus();
                this.Text = "移库";
            }
            else if (module == CodeMaster.TerminalPermission.Client_WMSShip)
            {
                UCWMSShip uc = new UCWMSShip(this.user);
                uc.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(uc);
                uc.tbBarCode.Focus();
                this.Text = "发货";
            }
            else if (module == CodeMaster.TerminalPermission.Client_BindContainerIn)
            {
                UCBindContainer uc = new UCBindContainer(this.user,true);
                uc.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(uc);
                uc.tbBarCode.Focus();
                this.Text = "托盘打包";
            }
            else if (module == CodeMaster.TerminalPermission.Client_BindContainerOut)
            {
                UCBindContainer uc = new UCBindContainer(this.user, false);
                uc.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(uc);
                uc.tbBarCode.Focus();
                this.Text = "托盘拆包";
            }
            else if (module == CodeMaster.TerminalPermission.Client_ProductionReceive)
            {
                UCProductionReceive uc = new UCProductionReceive(this.user);
                uc.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(uc);
                uc.tbBarCode.Focus();
                this.Text = "成品入库";
            }
            else if (module == CodeMaster.TerminalPermission.Client_QuickTransfer)
            {
                UCQuickTransfer uc = new UCQuickTransfer(this.user);
                uc.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(uc);
                uc.tbBarCode.Focus();
                this.Text = "车间领料";
            }
            else if (module == CodeMaster.TerminalPermission.Client_ProductionReturn)
            {
                UCProductionReturn uc = new UCProductionReturn(this.user);
                uc.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(uc);
                uc.tbBarCode.Focus();
                this.Text = "成品入库冲销";
            }
            else if (module == CodeMaster.TerminalPermission.Client_DistributionReturn)
            {
                UCDistributionReturn uc = new UCDistributionReturn(this.user);
                uc.ModuleSelectionEvent += new ModuleSelectHandler(this.SwitchModule);
                this.AddModule(uc);
                uc.tbBarCode.Focus();
                this.Text = "销售退货";
            }
        }

        private void AddModule(UserControl userControl)
        {
            userControl.Location = new System.Drawing.Point(0, 0);
            userControl.Size = new System.Drawing.Size(280, 320);
            this.plMain.Controls.RemoveAt(0);
            this.plMain.Controls.Add(userControl);
            this.Activate();
            userControl.Focus();
        }

        private void timerLogin_Tick(object sender, EventArgs e)
        {
            if (this.user != null)
            {
                time++;
                int countDown = timeOut - time;

                if (countDown < 0)
                {
                    this.LoadUCLogin();
                    time = 0;
                }
            }
        }

    }
}