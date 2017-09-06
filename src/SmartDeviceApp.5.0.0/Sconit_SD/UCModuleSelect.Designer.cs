using System.Linq;
namespace com.Sconit.SmartDevice
{
    partial class UCModuleSelect
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnExit = new System.Windows.Forms.Button();
            this.btnLogOff = new System.Windows.Forms.Button();
            this.tbKeyCode = new System.Windows.Forms.TextBox();
            this.lblUserStatus = new System.Windows.Forms.Label();
            this.tabInventory = new System.Windows.Forms.TabPage();
            this.btnBindContainerOut = new System.Windows.Forms.Button();
            this.btnBindContainerIn = new System.Windows.Forms.Button();
            this.btnBinning = new System.Windows.Forms.Button();
            this.btnDevanning = new System.Windows.Forms.Button();
            this.btnHuStatus = new System.Windows.Forms.Button();
            this.btnHuClone = new System.Windows.Forms.Button();
            this.btnMiscInOut = new System.Windows.Forms.Button();
            this.btnStockTaking = new System.Windows.Forms.Button();
            this.btnReBinning = new System.Windows.Forms.Button();
            this.btnPutAway = new System.Windows.Forms.Button();
            this.btnPickUp = new System.Windows.Forms.Button();
            this.btnTransfer = new System.Windows.Forms.Button();
            this.tabModuleSelect = new System.Windows.Forms.TabControl();
            this.tabProcurement = new System.Windows.Forms.TabPage();
            this.btnProductionReturn = new System.Windows.Forms.Button();
            this.btnQuickTransfer = new System.Windows.Forms.Button();
            this.btnDistributionReturn = new System.Windows.Forms.Button();
            this.btnProductionReceive = new System.Windows.Forms.Button();
            this.btnPurchaseReturn = new System.Windows.Forms.Button();
            this.btnQuickReturn = new System.Windows.Forms.Button();
            this.btnOrderShip = new System.Windows.Forms.Button();
            this.btnReceive = new System.Windows.Forms.Button();
            this.tabQuality = new System.Windows.Forms.TabPage();
            this.btnUnfreeze = new System.Windows.Forms.Button();
            this.btnFreeze = new System.Windows.Forms.Button();
            this.btnQualify = new System.Windows.Forms.Button();
            this.btnInspect = new System.Windows.Forms.Button();
            this.tabInventory.SuspendLayout();
            this.tabModuleSelect.SuspendLayout();
            this.tabProcurement.SuspendLayout();
            this.tabQuality.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(169, 247);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(72, 20);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "退出";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnLogOff
            // 
            this.btnLogOff.Location = new System.Drawing.Point(94, 247);
            this.btnLogOff.Name = "btnLogOff";
            this.btnLogOff.Size = new System.Drawing.Size(72, 20);
            this.btnLogOff.TabIndex = 1;
            this.btnLogOff.Text = "注销";
            this.btnLogOff.Click += new System.EventHandler(this.btnLogOff_Click);
            // 
            // tbKeyCode
            // 
            this.tbKeyCode.Location = new System.Drawing.Point(7, 244);
            this.tbKeyCode.Name = "tbKeyCode";
            this.tbKeyCode.Size = new System.Drawing.Size(81, 23);
            this.tbKeyCode.TabIndex = 3;
            this.tbKeyCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbKeyCode_KeyUp);
            // 
            // lblUserStatus
            // 
            this.lblUserStatus.Location = new System.Drawing.Point(12, 227);
            this.lblUserStatus.Name = "lblUserStatus";
            this.lblUserStatus.Size = new System.Drawing.Size(215, 17);
            this.lblUserStatus.Text = "当前用户:";
            // 
            // tabInventory
            // 
            this.tabInventory.Controls.Add(this.btnBindContainerOut);
            this.tabInventory.Controls.Add(this.btnBindContainerIn);
            this.tabInventory.Controls.Add(this.btnBinning);
            this.tabInventory.Controls.Add(this.btnDevanning);
            this.tabInventory.Controls.Add(this.btnHuStatus);
            this.tabInventory.Controls.Add(this.btnHuClone);
            this.tabInventory.Controls.Add(this.btnMiscInOut);
            this.tabInventory.Controls.Add(this.btnStockTaking);
            this.tabInventory.Controls.Add(this.btnReBinning);
            this.tabInventory.Controls.Add(this.btnPutAway);
            this.tabInventory.Controls.Add(this.btnPickUp);
            this.tabInventory.Controls.Add(this.btnTransfer);
            this.tabInventory.Location = new System.Drawing.Point(4, 25);
            this.tabInventory.Name = "tabInventory";
            this.tabInventory.Size = new System.Drawing.Size(245, 183);
            this.tabInventory.Text = "仓库";
            // 
            // btnBindContainerOut
            // 
            this.btnBindContainerOut.Enabled = false;
            this.btnBindContainerOut.Location = new System.Drawing.Point(128, 150);
            this.btnBindContainerOut.Name = "btnBindContainerOut";
            this.btnBindContainerOut.Size = new System.Drawing.Size(98, 20);
            this.btnBindContainerOut.TabIndex = 12;
            this.btnBindContainerOut.Text = "12.托盘拆包";
            this.btnBindContainerOut.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnBindContainerOut.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnBindContainerIn
            // 
            this.btnBindContainerIn.Enabled = false;
            this.btnBindContainerIn.Location = new System.Drawing.Point(15, 150);
            this.btnBindContainerIn.Name = "btnBindContainerIn";
            this.btnBindContainerIn.Size = new System.Drawing.Size(100, 20);
            this.btnBindContainerIn.TabIndex = 11;
            this.btnBindContainerIn.Text = "11.托盘打包";
            this.btnBindContainerIn.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnBindContainerIn.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnBinning
            // 
            this.btnBinning.Enabled = false;
            this.btnBinning.Location = new System.Drawing.Point(15, 70);
            this.btnBinning.Name = "btnBinning";
            this.btnBinning.Size = new System.Drawing.Size(100, 20);
            this.btnBinning.TabIndex = 5;
            this.btnBinning.Text = "5.装箱";
            this.btnBinning.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnBinning.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnDevanning
            // 
            this.btnDevanning.Enabled = false;
            this.btnDevanning.Location = new System.Drawing.Point(128, 70);
            this.btnDevanning.Name = "btnDevanning";
            this.btnDevanning.Size = new System.Drawing.Size(98, 20);
            this.btnDevanning.TabIndex = 6;
            this.btnDevanning.Text = "6.拆箱";
            this.btnDevanning.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnDevanning.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnHuStatus
            // 
            this.btnHuStatus.Enabled = false;
            this.btnHuStatus.Location = new System.Drawing.Point(128, 96);
            this.btnHuStatus.Name = "btnHuStatus";
            this.btnHuStatus.Size = new System.Drawing.Size(98, 20);
            this.btnHuStatus.TabIndex = 8;
            this.btnHuStatus.Text = "8.条码状态";
            this.btnHuStatus.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnHuStatus.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnHuClone
            // 
            this.btnHuClone.Enabled = false;
            this.btnHuClone.Location = new System.Drawing.Point(128, 122);
            this.btnHuClone.Name = "btnHuClone";
            this.btnHuClone.Size = new System.Drawing.Size(98, 20);
            this.btnHuClone.TabIndex = 10;
            this.btnHuClone.Text = "10.条码克隆";
            this.btnHuClone.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnHuClone.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnMiscInOut
            // 
            this.btnMiscInOut.Enabled = false;
            this.btnMiscInOut.Location = new System.Drawing.Point(15, 122);
            this.btnMiscInOut.Name = "btnMiscInOut";
            this.btnMiscInOut.Size = new System.Drawing.Size(100, 20);
            this.btnMiscInOut.TabIndex = 9;
            this.btnMiscInOut.Text = "9.计划外出入";
            this.btnMiscInOut.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnMiscInOut.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnStockTaking
            // 
            this.btnStockTaking.Enabled = false;
            this.btnStockTaking.Location = new System.Drawing.Point(15, 96);
            this.btnStockTaking.Name = "btnStockTaking";
            this.btnStockTaking.Size = new System.Drawing.Size(100, 20);
            this.btnStockTaking.TabIndex = 7;
            this.btnStockTaking.Text = "7.盘点";
            this.btnStockTaking.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnStockTaking.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnReBinning
            // 
            this.btnReBinning.Enabled = false;
            this.btnReBinning.Location = new System.Drawing.Point(128, 18);
            this.btnReBinning.Name = "btnReBinning";
            this.btnReBinning.Size = new System.Drawing.Size(98, 20);
            this.btnReBinning.TabIndex = 2;
            this.btnReBinning.Text = "2.翻箱";
            this.btnReBinning.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnReBinning.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnPutAway
            // 
            this.btnPutAway.Enabled = false;
            this.btnPutAway.Location = new System.Drawing.Point(15, 44);
            this.btnPutAway.Name = "btnPutAway";
            this.btnPutAway.Size = new System.Drawing.Size(100, 20);
            this.btnPutAway.TabIndex = 3;
            this.btnPutAway.Text = "3.上架";
            this.btnPutAway.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnPutAway.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnPickUp
            // 
            this.btnPickUp.Enabled = false;
            this.btnPickUp.Location = new System.Drawing.Point(128, 44);
            this.btnPickUp.Name = "btnPickUp";
            this.btnPickUp.Size = new System.Drawing.Size(99, 20);
            this.btnPickUp.TabIndex = 4;
            this.btnPickUp.Text = "4.下架";
            this.btnPickUp.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnPickUp.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnTransfer
            // 
            this.btnTransfer.Enabled = false;
            this.btnTransfer.Location = new System.Drawing.Point(15, 18);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(100, 20);
            this.btnTransfer.TabIndex = 1;
            this.btnTransfer.Text = "1.移库";
            this.btnTransfer.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnTransfer.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // tabModuleSelect
            // 
            this.tabModuleSelect.Controls.Add(this.tabProcurement);
            this.tabModuleSelect.Controls.Add(this.tabInventory);
            this.tabModuleSelect.Controls.Add(this.tabQuality);
            this.tabModuleSelect.Location = new System.Drawing.Point(3, 13);
            this.tabModuleSelect.Name = "tabModuleSelect";
            this.tabModuleSelect.SelectedIndex = 0;
            this.tabModuleSelect.Size = new System.Drawing.Size(253, 212);
            this.tabModuleSelect.TabIndex = 0;
            this.tabModuleSelect.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            this.tabModuleSelect.SelectedIndexChanged += new System.EventHandler(this.tabModuleSelect_SelectedIndexChanged);
            // 
            // tabProcurement
            // 
            this.tabProcurement.Controls.Add(this.btnProductionReturn);
            this.tabProcurement.Controls.Add(this.btnQuickTransfer);
            this.tabProcurement.Controls.Add(this.btnDistributionReturn);
            this.tabProcurement.Controls.Add(this.btnProductionReceive);
            this.tabProcurement.Controls.Add(this.btnPurchaseReturn);
            this.tabProcurement.Controls.Add(this.btnQuickReturn);
            this.tabProcurement.Controls.Add(this.btnOrderShip);
            this.tabProcurement.Controls.Add(this.btnReceive);
            this.tabProcurement.Location = new System.Drawing.Point(4, 25);
            this.tabProcurement.Name = "tabProcurement";
            this.tabProcurement.Size = new System.Drawing.Size(245, 183);
            this.tabProcurement.Text = "收发";
            // 
            // btnProductionReturn
            // 
            this.btnProductionReturn.Enabled = false;
            this.btnProductionReturn.Location = new System.Drawing.Point(119, 87);
            this.btnProductionReturn.Name = "btnProductionReturn";
            this.btnProductionReturn.Size = new System.Drawing.Size(110, 20);
            this.btnProductionReturn.TabIndex = 13;
            this.btnProductionReturn.Text = "6.成品入库冲销";
            this.btnProductionReturn.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnProductionReturn.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnQuickTransfer
            // 
            this.btnQuickTransfer.Enabled = false;
            this.btnQuickTransfer.Location = new System.Drawing.Point(5, 50);
            this.btnQuickTransfer.Name = "btnQuickTransfer";
            this.btnQuickTransfer.Size = new System.Drawing.Size(104, 20);
            this.btnQuickTransfer.TabIndex = 12;
            this.btnQuickTransfer.Text = "3.车间领料";
            this.btnQuickTransfer.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnQuickTransfer.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnDistributionReturn
            // 
            this.btnDistributionReturn.Enabled = false;
            this.btnDistributionReturn.Location = new System.Drawing.Point(119, 123);
            this.btnDistributionReturn.Name = "btnDistributionReturn";
            this.btnDistributionReturn.Size = new System.Drawing.Size(110, 20);
            this.btnDistributionReturn.TabIndex = 11;
            this.btnDistributionReturn.Text = "8.销售退货";
            this.btnDistributionReturn.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnDistributionReturn.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnProductionReceive
            // 
            this.btnProductionReceive.Enabled = false;
            this.btnProductionReceive.Location = new System.Drawing.Point(5, 87);
            this.btnProductionReceive.Name = "btnProductionReceive";
            this.btnProductionReceive.Size = new System.Drawing.Size(104, 20);
            this.btnProductionReceive.TabIndex = 10;
            this.btnProductionReceive.Text = "5.成品入库";
            this.btnProductionReceive.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnProductionReceive.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnPurchaseReturn
            // 
            this.btnPurchaseReturn.Enabled = false;
            this.btnPurchaseReturn.Location = new System.Drawing.Point(119, 15);
            this.btnPurchaseReturn.Name = "btnPurchaseReturn";
            this.btnPurchaseReturn.Size = new System.Drawing.Size(111, 20);
            this.btnPurchaseReturn.TabIndex = 7;
            this.btnPurchaseReturn.Text = "2.采购退货";
            this.btnPurchaseReturn.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnPurchaseReturn.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnQuickReturn
            // 
            this.btnQuickReturn.Enabled = false;
            this.btnQuickReturn.Location = new System.Drawing.Point(119, 50);
            this.btnQuickReturn.Name = "btnQuickReturn";
            this.btnQuickReturn.Size = new System.Drawing.Size(110, 20);
            this.btnQuickReturn.TabIndex = 6;
            this.btnQuickReturn.Text = "4.领料退库";
            this.btnQuickReturn.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnQuickReturn.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnOrderShip
            // 
            this.btnOrderShip.Enabled = false;
            this.btnOrderShip.Location = new System.Drawing.Point(5, 123);
            this.btnOrderShip.Name = "btnOrderShip";
            this.btnOrderShip.Size = new System.Drawing.Size(104, 20);
            this.btnOrderShip.TabIndex = 4;
            this.btnOrderShip.Text = "7.销售发货";
            this.btnOrderShip.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnOrderShip.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnReceive
            // 
            this.btnReceive.Enabled = false;
            this.btnReceive.Location = new System.Drawing.Point(5, 15);
            this.btnReceive.Name = "btnReceive";
            this.btnReceive.Size = new System.Drawing.Size(104, 20);
            this.btnReceive.TabIndex = 5;
            this.btnReceive.Text = "1.采购收货";
            this.btnReceive.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnReceive.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // tabQuality
            // 
            this.tabQuality.Controls.Add(this.btnUnfreeze);
            this.tabQuality.Controls.Add(this.btnFreeze);
            this.tabQuality.Controls.Add(this.btnQualify);
            this.tabQuality.Controls.Add(this.btnInspect);
            this.tabQuality.Location = new System.Drawing.Point(4, 25);
            this.tabQuality.Name = "tabQuality";
            this.tabQuality.Size = new System.Drawing.Size(245, 183);
            this.tabQuality.Text = "质量";
            // 
            // btnUnfreeze
            // 
            this.btnUnfreeze.Location = new System.Drawing.Point(130, 69);
            this.btnUnfreeze.Name = "btnUnfreeze";
            this.btnUnfreeze.Size = new System.Drawing.Size(90, 20);
            this.btnUnfreeze.TabIndex = 5;
            this.btnUnfreeze.Text = "4.解冻";
            this.btnUnfreeze.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnUnfreeze.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnFreeze
            // 
            this.btnFreeze.Location = new System.Drawing.Point(22, 69);
            this.btnFreeze.Name = "btnFreeze";
            this.btnFreeze.Size = new System.Drawing.Size(90, 20);
            this.btnFreeze.TabIndex = 4;
            this.btnFreeze.Text = "3.冻结";
            this.btnFreeze.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnFreeze.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnQualify
            // 
            this.btnQualify.Location = new System.Drawing.Point(130, 26);
            this.btnQualify.Name = "btnQualify";
            this.btnQualify.Size = new System.Drawing.Size(90, 20);
            this.btnQualify.TabIndex = 3;
            this.btnQualify.Text = "2.判定";
            this.btnQualify.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnQualify.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // btnInspect
            // 
            this.btnInspect.Location = new System.Drawing.Point(22, 26);
            this.btnInspect.Name = "btnInspect";
            this.btnInspect.Size = new System.Drawing.Size(90, 20);
            this.btnInspect.TabIndex = 2;
            this.btnInspect.Text = "1.报验";
            this.btnInspect.Click += new System.EventHandler(this.UCModuleSelect_Click);
            this.btnInspect.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            // 
            // UCModuleSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.lblUserStatus);
            this.Controls.Add(this.tbKeyCode);
            this.Controls.Add(this.btnLogOff);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.tabModuleSelect);
            this.Name = "UCModuleSelect";
            this.Size = new System.Drawing.Size(280, 275);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UCModuleSelect_KeyUp);
            this.tabInventory.ResumeLayout(false);
            this.tabModuleSelect.ResumeLayout(false);
            this.tabProcurement.ResumeLayout(false);
            this.tabQuality.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnLogOff;
        private System.Windows.Forms.TextBox tbKeyCode;
        private System.Windows.Forms.Label lblUserStatus;
        private System.Windows.Forms.TabPage tabInventory;
        private System.Windows.Forms.Button btnBinning;
        private System.Windows.Forms.Button btnHuStatus;
        private System.Windows.Forms.Button btnHuClone;
        private System.Windows.Forms.Button btnMiscInOut;
        private System.Windows.Forms.Button btnStockTaking;
        private System.Windows.Forms.Button btnReBinning;
        private System.Windows.Forms.Button btnPutAway;
        private System.Windows.Forms.Button btnPickUp;
        private System.Windows.Forms.Button btnTransfer;
        private System.Windows.Forms.TabControl tabModuleSelect;
        private System.Windows.Forms.TabPage tabProcurement;
        private System.Windows.Forms.Button btnPurchaseReturn;
        private System.Windows.Forms.Button btnQuickReturn;
        private System.Windows.Forms.Button btnOrderShip;
        private System.Windows.Forms.Button btnReceive;
        private System.Windows.Forms.Button btnBindContainerOut;
        private System.Windows.Forms.Button btnBindContainerIn;
        private System.Windows.Forms.Button btnDevanning;
        private System.Windows.Forms.Button btnProductionReceive;
        private System.Windows.Forms.Button btnProductionReturn;
        private System.Windows.Forms.Button btnQuickTransfer;
        private System.Windows.Forms.Button btnDistributionReturn;
        private System.Windows.Forms.TabPage tabQuality;
        private System.Windows.Forms.Button btnUnfreeze;
        private System.Windows.Forms.Button btnFreeze;
        private System.Windows.Forms.Button btnQualify;
        private System.Windows.Forms.Button btnInspect;
    }
}
