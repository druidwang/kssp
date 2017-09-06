using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using com.Sconit.PrintMonitor.PUB_PrintOrderService;

namespace com.Sconit.PrintMonitor
{
    public partial class Main : Form
    {
        public static object locker = new object();
        private PUB_PrintOrderService.PUB_PrintOrderServiceSoapClient ThePubPrintOrderClient;
        private List<PubPrintOrder> cachePubPrintOrderList;
        private string clientCode;
        public Main()
        {
            InitializeComponent();
            this.ThePubPrintOrderClient = new PUB_PrintOrderService.PUB_PrintOrderServiceSoapClient();
            cachePubPrintOrderList = new List<PubPrintOrder>();
            clientCode = System.Configuration.ConfigurationSettings.AppSettings["ClientCode"];
            this.lblMessage.Text = string.Empty;
            this.dataGridView1.AutoGenerateColumns = false;
        }


        private delegate void lblMessageDelegate(string message);
        private void SetlblMessage(string message)
        {
            this.lblMessage.Text = message;
        }
        private void SetlblMessage(lblMessageDelegate myDelegate, string message)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(myDelegate, message);
            }
            else
            {
                myDelegate(message);
            }
        }

        private void Print_Tick(object sender, EventArgs e)
        {
            try
            {
                PrintAndBind();
            }
            catch(Exception ex)
            {
                Utility.Log("Error:UCPrintMonitor:" + ex.Message);
            }
        }

        private delegate void Async();
        private void AsyncPrint()
        {
            Async async = new Async(PrintAndBind);
            async.BeginInvoke(null, null);
        }

        private void PrintAndBind()
        {
            try
            {
                var pubPrintOrders = ThePubPrintOrderClient.GetPubPrintOrderList(clientCode);
                if(pubPrintOrders != null)
                {
                    foreach(PubPrintOrder pubPrintOrder in pubPrintOrders)
                    {
                        DoPrint(pubPrintOrder.PrintUrl, pubPrintOrder.Printer);
                    }
                }
                cachePubPrintOrderList.AddRange(pubPrintOrders);
                this.dataGridView1.DataSource = new BindingList<PubPrintOrder>
                    (cachePubPrintOrderList.OrderByDescending(p => p.CreateDate).Take(100).ToList());
            }
            catch(Exception ex)
            {
                SetlblMessage(SetlblMessage, ex.Message);
                Utility.Log(ex.Message);
                Utility.Log(ex.StackTrace);
            }
        }

        public static object lockerPrint = new object();
        private void DoPrint(string printUrl, string printer)
        {
            lock(lockerPrint)
            {
                var message = Utility.PrintOrder(printUrl, this, printer);
                if(!string.IsNullOrEmpty(message))
                {
                    SetlblMessage(SetlblMessage, message);
                }
            }
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            //点击后启动,启动后不可以可以修改间隔时间
            if(this.tbInterval.Enabled)
            {
                this.tbInterval.Enabled = false;
                this.BtnStart1.Text = "暂停";

                if(this.tbInterval.Text.Trim() != string.Empty && int.Parse(this.tbInterval.Text) != 0)
                {
                    this.Print_Timer.Interval = int.Parse(this.tbInterval.Text) * 1000;
                }
                else
                {
                    this.tbInterval.Text = (this.Print_Timer.Interval / 1000).ToString();
                }
                this.Print_Timer.Enabled = true;
                Print_Tick(this, null);
            }
            //点击后暂停,暂停后可以修改间隔时间
            else
            {
                this.tbInterval.Enabled = true;
                this.Print_Timer.Enabled = false;
                this.BtnStart1.Text = "开始";
            }
        }

        private void tbInterval_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utility.TextBoxIntFilter(sender, e);
            try
            {
                int.Parse(this.tbInterval.Text);
            }
            catch(Exception)
            {
                //this.tbInterval.Text = (this.timer1.Interval / 1000).ToString();
                //throw;
            }
        }
    }
}
