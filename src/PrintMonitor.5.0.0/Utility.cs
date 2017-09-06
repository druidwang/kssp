using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace com.Sconit.PrintMonitor
{
    public class Utility
    {
        public static string GetBarcodeCode128BByStr(string str)
        {
            int total = 104;
            int a = 0;
            int endAsc = 0;
            char endChar = new char();
            for(int i = 0; i < str.Length; i++)
            {
                //转换ASCII数值
                a = Convert.ToInt32(Convert.ToChar(str.Substring(i, 1)));

                //Code 128 SET B 字符集
                if(a >= 32)
                {
                    total += (a - 32) * (i + 1);
                }
                else
                {
                    total += (a + 64) * (i + 1);
                }
            }
            endAsc = total % 103;
            //字符集大于95直接赋值，其它转换后获得
            if(endAsc >= 95)
            {
                switch(endAsc)
                {
                    case 95:
                        endChar = Convert.ToChar("Ã");
                        break;
                    case 96:
                        endChar = Convert.ToChar("Ä");
                        break;
                    case 97:
                        endChar = Convert.ToChar("Å");
                        break;
                    case 98:
                        endChar = Convert.ToChar("Æ");
                        break;
                    case 99:
                        endChar = Convert.ToChar("Ç");
                        break;
                    case 100:
                        endChar = Convert.ToChar("È");
                        break;
                    case 101:
                        endChar = Convert.ToChar("É");
                        break;
                    case 102:
                        endChar = Convert.ToChar("Ê");
                        break;
                    default:
                        endChar = Convert.ToChar("");
                        break;
                }
            }
            else
            {
                endAsc += 32;
                endChar = Convert.ToChar(endAsc);
            }
            //生成Code 128B条码字符串
            string result = "Ì" + str + endChar.ToString() + "Î";
            return result;
        }

        public static string Md5(string originalPassword)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(originalPassword);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach(byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }

        public static bool IsDecimal(string str)
        {
            try
            {
                Convert.ToDecimal(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string FormatExMessage(string message)
        {
            try
            {
                if(message.StartsWith("System.Web.Services.Protocols.SoapException"))
                {
                    message = message.Remove(0, 44);
                    message = message.Remove(message.IndexOf("\n"), message.Length - message.IndexOf("\n"));
                }
                message = message.Replace("\\n", "\n\n");
            }
            catch(Exception ex)
            {
                return message;
            }
            return message;
        }


        public static DataGridView RenderDataGridViewBackColor(DataGridView dataGrid)
        {
            foreach(DataGridViewRow row in dataGrid.Rows)
            {
                decimal CurrentQty = Convert.ToDecimal(row.Cells["CurrentQty"].Value.ToString());
                decimal CurrentRejectQty = 0;
                try { CurrentRejectQty = Convert.ToDecimal(row.Cells["CurrentRejectQty"].Value.ToString()); }
                catch(Exception) { }
                decimal Qty = Convert.ToDecimal(row.Cells["Qty"].Value.ToString());
                if(CurrentQty + CurrentRejectQty == Qty)
                {
                    row.DefaultCellStyle.ForeColor = Color.Green;
                }
                else if(CurrentQty + CurrentRejectQty > Qty)
                {
                    row.DefaultCellStyle.ForeColor = Color.OrangeRed;
                }
                else if(CurrentQty + CurrentRejectQty < Qty)
                {
                    //row.DefaultCellStyle.BackColor = Color.Yellow;
                }
            }
            return dataGrid;
        }

        public static string PrintOrder(string fileUrl, IWin32Window win32Window, string printer)
        {
            string message = null;
            //KillProcess("EXCEL");
            Microsoft.Office.Interop.Excel.Application myExcel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbooks myBooks = null;
            Microsoft.Office.Interop.Excel.Workbook myBook = null;
            Microsoft.Office.Interop.Excel.Worksheet mySheet1 = null;

            Object missing = System.Reflection.Missing.Value;
            Object defaultPrint = missing;

            //string print = Settings.Default.DefaultPrintName1;
            if(printer != null && printer != string.Empty)
            {
                defaultPrint = printer;
            }

            try
            {
                myBooks = myExcel.Workbooks; // <-- the important part
                myBook = myBooks.Open(fileUrl, missing, missing, missing, missing,
                    missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);
                //handle sheets
                mySheet1 = (Microsoft.Office.Interop.Excel.Worksheet)myBook.Sheets[1];
                mySheet1.PrintOut(missing, missing, missing, missing, defaultPrint, missing, missing, missing);
            }
            catch(Exception e)
            {
                message = "打印失败,错误信息:" + e.Message;
                //MessageBox.Show(win32Window, errorMsg, "打印失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if(myBook != null)
                {
                    myBook.Save();
                    myBook.Close();
                }
                if(myExcel != null)
                {
                    myExcel.Quit();
                }
                ReleaseObject(mySheet1);
                ReleaseObject(myBooks);
                ReleaseObject(myBook);
                ReleaseObject(myExcel);
            }
            return message;
        }

        private static void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch(Exception)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }



        #region 杀死进程
        private static void KillProcess(string processName)
        {
            //获得进程对象，以用来操作   
            System.Diagnostics.Process myproc = new System.Diagnostics.Process();
            //得到所有打开的进程    
            try
            {
                //获得需要杀死的进程名   
                foreach(Process thisproc in Process.GetProcessesByName(processName))
                {
                    //立即杀死进程   
                    thisproc.Kill();
                }
            }
            catch(Exception Exc)
            {
                //throw new Exception("", Exc);
            }
        }
        #endregion

        public static void DataGridViewDecimalFilter(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar.ToString() == "\b" || e.KeyChar.ToString() == "." || e.KeyChar.ToString() == "-")
            {
                string str;
                if(e.KeyChar.ToString() == "\b")
                {
                    e.Handled = false;
                    return;
                }
                else
                {
                    str = ((DataGridViewTextBoxEditingControl)sender).Text + e.KeyChar.ToString();
                }

                if(Utility.IsDecimal(str) || str == "-")
                {
                    e.Handled = false;
                    return;
                }
            }
            if(!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        public static void DataGridViewIntFilter(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar.ToString() == "\b" || e.KeyChar.ToString() == "-")
            {
                string str;
                if(e.KeyChar.ToString() == "\b")
                {
                    e.Handled = false;
                    return;
                }
                else
                {
                    str = ((DataGridViewTextBoxEditingControl)sender).Text + e.KeyChar.ToString();
                }

                if(Utility.IsDecimal(str) || str == "-")
                {
                    e.Handled = false;
                    return;
                }
            }
            if(!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        public static void TextBoxDecimalFilter(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar.ToString() == "\b" || e.KeyChar.ToString() == "." || e.KeyChar.ToString() == "-")
            {
                string str;
                if(e.KeyChar.ToString() == "\b")
                {
                    e.Handled = false;
                    return;
                }
                else
                {
                    str = ((TextBox)sender).Text.Trim() + e.KeyChar.ToString();
                }
                if(Utility.IsDecimal(str) || str == "-")
                {
                    e.Handled = false;
                    return;
                }
            }
            if(!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        public static void TextBoxIntFilter(object sender, KeyPressEventArgs e)
        {
            if(!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        public static void Log(string logstr)
        {
            if(!Directory.Exists("C:\\Logs\\SconitPrintMonitor\\"))
            {
                Directory.CreateDirectory("C:\\Logs\\SconitPrintMonitor\\");
            }
            FileStream fs = new FileStream(string.Format("C:\\Logs\\SconitPrintMonitor\\Sconit_CSLog.{0}.txt", DateTime.Now.ToString("yyyyMMdd")),
                FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter m_streamWriter = new StreamWriter(fs);
            m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
            m_streamWriter.WriteLine(DateTime.Now.ToString() + " " + logstr + "\n");
            m_streamWriter.Flush();
            m_streamWriter.Close();
            fs.Close();
        }


        #region 私有方法

        private static void CopyProperty(object sourceObj, object targetObj)
        {
            PropertyInfo[] sourcePropertyInfoAry = sourceObj.GetType().GetProperties();
            PropertyInfo[] targetPropertyInfoAry = targetObj.GetType().GetProperties();

            foreach(PropertyInfo sourcePropertyInfo in sourcePropertyInfoAry)
            {
                foreach(PropertyInfo targetPropertyInfo in targetPropertyInfoAry)
                {
                    if(sourcePropertyInfo.Name == targetPropertyInfo.Name)
                    {
                        if(targetPropertyInfo.CanWrite && sourcePropertyInfo.CanRead)
                        {
                            targetPropertyInfo.SetValue(targetObj, sourcePropertyInfo.GetValue(sourceObj, null), null);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
