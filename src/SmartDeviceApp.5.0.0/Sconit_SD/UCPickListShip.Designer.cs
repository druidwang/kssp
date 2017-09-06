namespace com.Sconit.SmartDevice
{
    partial class UCPickListShip
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
            this.btnBack = new System.Windows.Forms.Button();
            this.btnOrder = new System.Windows.Forms.Button();
            this.tbBarCode = new System.Windows.Forms.TextBox();
            this.lblWindowTime = new System.Windows.Forms.Label();
            this.lblStartTime = new System.Windows.Forms.Label();
            this.lblEffectTime = new System.Windows.Forms.Label();
            this.labelEffectTime = new System.Windows.Forms.Label();
            this.labelWindowTime = new System.Windows.Forms.Label();
            this.labelStartTime = new System.Windows.Forms.Label();
            this.labelPartyTo = new System.Windows.Forms.Label();
            this.lblPartyTo = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lblPartyFrom = new System.Windows.Forms.Label();
            this.labelPartyFrom = new System.Windows.Forms.Label();
            this.lblBarCode = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(190, 270);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(48, 20);
            this.btnBack.TabIndex = 117;
            this.btnBack.Text = "返回";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnOrder
            // 
            this.btnOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOrder.Location = new System.Drawing.Point(200, 1);
            this.btnOrder.Name = "btnOrder";
            this.btnOrder.Size = new System.Drawing.Size(40, 20);
            this.btnOrder.TabIndex = 116;
            this.btnOrder.Text = "确定";
            this.btnOrder.Click += new System.EventHandler(this.btnOrder_Click);
            this.btnOrder.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbBarCode_KeyUp);
            // 
            // tbBarCode
            // 
            this.tbBarCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBarCode.Location = new System.Drawing.Point(43, 0);
            this.tbBarCode.MaxLength = 50;
            this.tbBarCode.Name = "tbBarCode";
            this.tbBarCode.Size = new System.Drawing.Size(151, 23);
            this.tbBarCode.TabIndex = 115;
            this.tbBarCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbBarCode_KeyUp);
            // 
            // lblWindowTime
            // 
            this.lblWindowTime.Location = new System.Drawing.Point(78, 119);
            this.lblWindowTime.Name = "lblWindowTime";
            this.lblWindowTime.Size = new System.Drawing.Size(162, 16);
            this.lblWindowTime.Text = "2012-1-1 11:12";
            // 
            // lblStartTime
            // 
            this.lblStartTime.Location = new System.Drawing.Point(78, 101);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Size = new System.Drawing.Size(162, 16);
            this.lblStartTime.Text = "2011-1-1 11:11";
            // 
            // lblEffectTime
            // 
            this.lblEffectTime.Location = new System.Drawing.Point(78, 137);
            this.lblEffectTime.Name = "lblEffectTime";
            this.lblEffectTime.Size = new System.Drawing.Size(162, 16);
            this.lblEffectTime.Text = "2010-02-11";
            // 
            // labelEffectTime
            // 
            this.labelEffectTime.Location = new System.Drawing.Point(2, 137);
            this.labelEffectTime.Name = "labelEffectTime";
            this.labelEffectTime.Size = new System.Drawing.Size(70, 18);
            this.labelEffectTime.Text = "生效时间:";
            // 
            // labelWindowTime
            // 
            this.labelWindowTime.Location = new System.Drawing.Point(2, 119);
            this.labelWindowTime.Name = "labelWindowTime";
            this.labelWindowTime.Size = new System.Drawing.Size(70, 18);
            this.labelWindowTime.Text = "窗口时间:";
            // 
            // labelStartTime
            // 
            this.labelStartTime.Location = new System.Drawing.Point(2, 101);
            this.labelStartTime.Name = "labelStartTime";
            this.labelStartTime.Size = new System.Drawing.Size(70, 18);
            this.labelStartTime.Text = "开始时间:";
            // 
            // labelPartyTo
            // 
            this.labelPartyTo.Location = new System.Drawing.Point(2, 82);
            this.labelPartyTo.Name = "labelPartyTo";
            this.labelPartyTo.Size = new System.Drawing.Size(41, 18);
            this.labelPartyTo.Text = "目的:";
            // 
            // lblPartyTo
            // 
            this.lblPartyTo.Location = new System.Drawing.Point(78, 81);
            this.lblPartyTo.Name = "lblPartyTo";
            this.lblPartyTo.Size = new System.Drawing.Size(162, 19);
            this.lblPartyTo.Text = "BBB";
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular);
            this.lblMessage.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.Location = new System.Drawing.Point(4, 35);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(234, 16);
            this.lblMessage.Text = "拣货单发货";
            // 
            // lblPartyFrom
            // 
            this.lblPartyFrom.Location = new System.Drawing.Point(78, 61);
            this.lblPartyFrom.Name = "lblPartyFrom";
            this.lblPartyFrom.Size = new System.Drawing.Size(162, 16);
            this.lblPartyFrom.Text = "AAA";
            // 
            // labelPartyFrom
            // 
            this.labelPartyFrom.Location = new System.Drawing.Point(2, 61);
            this.labelPartyFrom.Name = "labelPartyFrom";
            this.labelPartyFrom.Size = new System.Drawing.Size(41, 18);
            this.labelPartyFrom.Text = "来源:";
            // 
            // lblBarCode
            // 
            this.lblBarCode.Location = new System.Drawing.Point(6, 1);
            this.lblBarCode.Name = "lblBarCode";
            this.lblBarCode.Size = new System.Drawing.Size(151, 23);
            this.lblBarCode.Text = "条码:";
            // 
            // UCPickListShip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnOrder);
            this.Controls.Add(this.tbBarCode);
            this.Controls.Add(this.lblWindowTime);
            this.Controls.Add(this.lblStartTime);
            this.Controls.Add(this.lblEffectTime);
            this.Controls.Add(this.labelEffectTime);
            this.Controls.Add(this.labelWindowTime);
            this.Controls.Add(this.labelStartTime);
            this.Controls.Add(this.labelPartyTo);
            this.Controls.Add(this.lblPartyTo);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblPartyFrom);
            this.Controls.Add(this.labelPartyFrom);
            this.Controls.Add(this.lblBarCode);
            this.Name = "UCPickListShip";
            this.Size = new System.Drawing.Size(240, 320);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnOrder;
        public System.Windows.Forms.TextBox tbBarCode;
        private System.Windows.Forms.Label lblWindowTime;
        private System.Windows.Forms.Label lblStartTime;
        private System.Windows.Forms.Label lblEffectTime;
        private System.Windows.Forms.Label labelEffectTime;
        private System.Windows.Forms.Label labelWindowTime;
        private System.Windows.Forms.Label labelStartTime;
        private System.Windows.Forms.Label labelPartyTo;
        private System.Windows.Forms.Label lblPartyTo;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblPartyFrom;
        private System.Windows.Forms.Label labelPartyFrom;
        private System.Windows.Forms.Label lblBarCode;
    }
}
