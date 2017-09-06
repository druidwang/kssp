namespace com.Sconit.SmartDevice
{
    partial class UCProductOrderOnline
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
            this.btnOrder = new System.Windows.Forms.Button();
            this.tbBarCode = new System.Windows.Forms.TextBox();
            this.lblBarCode = new System.Windows.Forms.Label();
            this.lblRefItemInfo = new System.Windows.Forms.Label();
            this.lblItemInfo = new System.Windows.Forms.Label();
            this.lblRefItem = new System.Windows.Forms.Label();
            this.lblItem = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblItemDescInfo = new System.Windows.Forms.Label();
            this.lblBarCodeInfo = new System.Windows.Forms.Label();
            this.lblBarCodeTitle = new System.Windows.Forms.Label();
            this.lblStartTimeInfo = new System.Windows.Forms.Label();
            this.lblStartTime = new System.Windows.Forms.Label();
            this.lblWoInfo = new System.Windows.Forms.Label();
            this.lblWo = new System.Windows.Forms.Label();
            this.lblVANInfo = new System.Windows.Forms.Label();
            this.lblVAN = new System.Windows.Forms.Label();
            this.lblFlowInfo = new System.Windows.Forms.Label();
            this.lblFlow = new System.Windows.Forms.Label();
            this.lblSeqInfo = new System.Windows.Forms.Label();
            this.lblSeq = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
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
            // lblBarCode
            // 
            this.lblBarCode.Location = new System.Drawing.Point(6, 1);
            this.lblBarCode.Name = "lblBarCode";
            this.lblBarCode.Size = new System.Drawing.Size(151, 23);
            this.lblBarCode.Text = "条码:";
            // 
            // lblRefItemInfo
            // 
            this.lblRefItemInfo.Location = new System.Drawing.Point(78, 275);
            this.lblRefItemInfo.Name = "lblRefItemInfo";
            this.lblRefItemInfo.Size = new System.Drawing.Size(162, 16);
            this.lblRefItemInfo.Text = "5000-17010D";
            // 
            // lblItemInfo
            // 
            this.lblItemInfo.Location = new System.Drawing.Point(78, 257);
            this.lblItemInfo.Name = "lblItemInfo";
            this.lblItemInfo.Size = new System.Drawing.Size(162, 16);
            this.lblItemInfo.Text = "5801333266_50000";
            // 
            // lblRefItem
            // 
            this.lblRefItem.Location = new System.Drawing.Point(2, 275);
            this.lblRefItem.Name = "lblRefItem";
            this.lblRefItem.Size = new System.Drawing.Size(70, 18);
            this.lblRefItem.Text = "旧图号:";
            // 
            // lblItem
            // 
            this.lblItem.Location = new System.Drawing.Point(2, 257);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(70, 18);
            this.lblItem.Text = "物料号:";
            // 
            // lblDescription
            // 
            this.lblDescription.Location = new System.Drawing.Point(2, 214);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(41, 18);
            this.lblDescription.Text = "描述:";
            // 
            // lblItemDescInfo
            // 
            this.lblItemDescInfo.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.lblItemDescInfo.Location = new System.Drawing.Point(43, 213);
            this.lblItemDescInfo.Name = "lblItemDescInfo";
            this.lblItemDescInfo.Size = new System.Drawing.Size(197, 43);
            this.lblItemDescInfo.Text = "新大康平顶驾驶室总成K10(上柴电喷,软轴)金属漆-玫瑰红色52449（老产品专用)";
            // 
            // lblBarCodeInfo
            // 
            this.lblBarCodeInfo.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular);
            this.lblBarCodeInfo.Location = new System.Drawing.Point(43, 193);
            this.lblBarCodeInfo.Name = "lblBarCodeInfo";
            this.lblBarCodeInfo.Size = new System.Drawing.Size(197, 16);
            this.lblBarCodeInfo.Text = "123456789012345678901234567890";
            // 
            // lblBarCodeTitle
            // 
            this.lblBarCodeTitle.Location = new System.Drawing.Point(2, 193);
            this.lblBarCodeTitle.Name = "lblBarCodeTitle";
            this.lblBarCodeTitle.Size = new System.Drawing.Size(41, 18);
            this.lblBarCodeTitle.Text = "条码:";
            // 
            // lblStartTimeInfo
            // 
            this.lblStartTimeInfo.Location = new System.Drawing.Point(77, 137);
            this.lblStartTimeInfo.Name = "lblStartTimeInfo";
            this.lblStartTimeInfo.Size = new System.Drawing.Size(162, 16);
            this.lblStartTimeInfo.Text = "2011-1-1 11:11";
            // 
            // lblStartTime
            // 
            this.lblStartTime.Location = new System.Drawing.Point(1, 137);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Size = new System.Drawing.Size(70, 18);
            this.lblStartTime.Text = "开始时间:";
            // 
            // lblWoInfo
            // 
            this.lblWoInfo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular);
            this.lblWoInfo.Location = new System.Drawing.Point(43, 100);
            this.lblWoInfo.Name = "lblWoInfo";
            this.lblWoInfo.Size = new System.Drawing.Size(179, 16);
            this.lblWoInfo.Text = "123456789012345678901234567890";
            // 
            // lblWo
            // 
            this.lblWo.Location = new System.Drawing.Point(2, 100);
            this.lblWo.Name = "lblWo";
            this.lblWo.Size = new System.Drawing.Size(41, 18);
            this.lblWo.Text = "工单:";
            // 
            // lblVANInfo
            // 
            this.lblVANInfo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular);
            this.lblVANInfo.Location = new System.Drawing.Point(43, 118);
            this.lblVANInfo.Name = "lblVANInfo";
            this.lblVANInfo.Size = new System.Drawing.Size(179, 16);
            this.lblVANInfo.Text = "123456789012345678901234567890";
            // 
            // lblVAN
            // 
            this.lblVAN.Location = new System.Drawing.Point(3, 118);
            this.lblVAN.Name = "lblVAN";
            this.lblVAN.Size = new System.Drawing.Size(40, 18);
            this.lblVAN.Text = "VAN :";
            // 
            // lblFlowInfo
            // 
            this.lblFlowInfo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular);
            this.lblFlowInfo.Location = new System.Drawing.Point(43, 80);
            this.lblFlowInfo.Name = "lblFlowInfo";
            this.lblFlowInfo.Size = new System.Drawing.Size(176, 18);
            this.lblFlowInfo.Text = "A";
            // 
            // lblFlow
            // 
            this.lblFlow.Location = new System.Drawing.Point(2, 80);
            this.lblFlow.Name = "lblFlow";
            this.lblFlow.Size = new System.Drawing.Size(41, 18);
            this.lblFlow.Text = "产线:";
            // 
            // lblSeqInfo
            // 
            this.lblSeqInfo.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular);
            this.lblSeqInfo.Location = new System.Drawing.Point(43, 61);
            this.lblSeqInfo.Name = "lblSeqInfo";
            this.lblSeqInfo.Size = new System.Drawing.Size(176, 18);
            this.lblSeqInfo.Text = "10";
            // 
            // lblSeq
            // 
            this.lblSeq.Location = new System.Drawing.Point(2, 61);
            this.lblSeq.Name = "lblSeq";
            this.lblSeq.Size = new System.Drawing.Size(41, 18);
            this.lblSeq.Text = "序号:";
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular);
            this.lblMessage.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.Location = new System.Drawing.Point(0, 24);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(234, 16);
            this.lblMessage.Text = "123456789012345678901234567890";
            // 
            // UCProductOrderOnline
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblRefItemInfo);
            this.Controls.Add(this.lblItemInfo);
            this.Controls.Add(this.lblRefItem);
            this.Controls.Add(this.lblItem);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblItemDescInfo);
            this.Controls.Add(this.lblBarCodeInfo);
            this.Controls.Add(this.lblBarCodeTitle);
            this.Controls.Add(this.lblStartTimeInfo);
            this.Controls.Add(this.lblStartTime);
            this.Controls.Add(this.lblWoInfo);
            this.Controls.Add(this.lblWo);
            this.Controls.Add(this.lblVANInfo);
            this.Controls.Add(this.lblVAN);
            this.Controls.Add(this.lblFlowInfo);
            this.Controls.Add(this.lblFlow);
            this.Controls.Add(this.lblSeqInfo);
            this.Controls.Add(this.lblSeq);
            this.Controls.Add(this.btnOrder);
            this.Controls.Add(this.tbBarCode);
            this.Controls.Add(this.lblBarCode);
            this.Name = "UCProductOrderOnline";
            this.Size = new System.Drawing.Size(240, 320);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOrder;
        public System.Windows.Forms.TextBox tbBarCode;
        private System.Windows.Forms.Label lblBarCode;
        private System.Windows.Forms.Label lblRefItemInfo;
        private System.Windows.Forms.Label lblItemInfo;
        private System.Windows.Forms.Label lblRefItem;
        private System.Windows.Forms.Label lblItem;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblItemDescInfo;
        private System.Windows.Forms.Label lblBarCodeInfo;
        private System.Windows.Forms.Label lblBarCodeTitle;
        private System.Windows.Forms.Label lblStartTimeInfo;
        private System.Windows.Forms.Label lblStartTime;
        private System.Windows.Forms.Label lblWoInfo;
        private System.Windows.Forms.Label lblWo;
        private System.Windows.Forms.Label lblVANInfo;
        private System.Windows.Forms.Label lblVAN;
        private System.Windows.Forms.Label lblFlowInfo;
        private System.Windows.Forms.Label lblFlow;
        private System.Windows.Forms.Label lblSeqInfo;
        private System.Windows.Forms.Label lblSeq;
        private System.Windows.Forms.Label lblMessage;
    }
}
