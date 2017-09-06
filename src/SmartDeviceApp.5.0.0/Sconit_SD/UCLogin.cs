using System;
using System.Windows.Forms;

namespace com.Sconit.SmartDevice
{
    public partial class UCLogin : UserControl
    {
        public event com.Sconit.SmartDevice.MainForm.LoginHandler LoginEvent;
        public event com.Sconit.SmartDevice.MainForm.ExitHandler ExitEvent;

        public UCLogin()
        {
            InitializeComponent();
            InitialLogin();
            this.lblVersion.Text += string.Format("{0} {1}{2}",
                System.Reflection.Assembly.GetCallingAssembly().GetName().Version,
                Environment.OSVersion.Platform,
                Environment.OSVersion.Version.ToString());
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string userCode = this.tbUserCode.Text;
                string password = this.tbPassword.Text;
                this.LoginEvent(userCode, password);
            }
            catch (Exception ex)
            {
                this.tbUserCode.Focus();
                Utility.ShowMessageBox("未知错误!请与管理员联系!" + ex.Message);
                InitialLogin();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (true)
            {
                this.ExitEvent();
            }
            else
            {
                //this.tbUserCode.Text = string.Empty;
                //this.tbPassword.Text = string.Empty;
                //this.tbUserCode.Focus();
            }
        }

        private void UCLogin_Load(object sender, EventArgs e)
        {

        }

        public void InitialLogin()
        {
            this.tbUserCode.Focus();
            this.tbUserCode.Text = string.Empty;
            this.tbPassword.Text = string.Empty;
            this.lblMessage.Text = string.Empty;
        }

        private void tbUserCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (((e.KeyData & Keys.KeyCode) == Keys.Enter) && this.tbUserCode.Text.Trim() != string.Empty)
            {
                this.tbPassword.Focus();
            }
            else if (true)
            {

            }
        }

        private void tbPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (((e.KeyData & Keys.KeyCode) == Keys.Enter) && this.tbPassword.Text.Trim() != string.Empty)
            {
                btnLogin_Click(null, null);
            }
        }
    }
}
