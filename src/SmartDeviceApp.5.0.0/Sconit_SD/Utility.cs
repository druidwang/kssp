using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using com.Sconit.SmartDevice.Properties;
using System.Reflection;
using System.Web.Services.Protocols;
using com.Sconit.SmartDevice.SmartDeviceRef;
namespace com.Sconit.SmartDevice
{
    public class Utility
    {
       public static string WEBSERVICE_URL = "http://10.136.3.28/WebService/SD/SmartDeviceService.asmx";


       // public static string WEBSERVICE_URL = "http://localhost:2015/WebService/SD/SmartDeviceService.asmx";

        public static string GetBarCodeType(BarCodeType[] barCodeTypes, string barCode)
        {
            if (string.IsNullOrEmpty(barCode) || barCode.Length < 2)
            {
                return null;
            }
            else if (barCode.StartsWith("$"))
            {
                return barCode.Substring(1, 1);
            }
            else if (barCode.StartsWith(".."))
            {
                return CodeMaster.BarCodeType.DATE.ToString();
            }
            else if (barCode.StartsWith("W"))
            {
                return CodeMaster.BarCodeType.W.ToString();
            }
            else if (barCode.StartsWith("SP"))
            {
                return CodeMaster.BarCodeType.SP.ToString();
            }
            else if (barCode.StartsWith("HU"))
            {
                return CodeMaster.BarCodeType.HU.ToString();
            }
            else if (barCode.StartsWith("DC"))
            {
                return CodeMaster.BarCodeType.DC.ToString();
            }
            else if (barCode.StartsWith("COT"))
            {
                return CodeMaster.BarCodeType.COT.ToString();
            }
            else if (barCode.StartsWith("TP") || (barCode.StartsWith("UN")))
            {
                return CodeMaster.BarCodeType.TP.ToString();
            }
            else
            {
                foreach (var codeType in barCodeTypes)
                {
                    if (barCode.ToUpper().StartsWith(codeType.PreFixed.ToUpper()))
                    {
                        return codeType.Type.ToString();
                    }
                }
            }
            return null;
        }

        public static bool HasPermission(IpMaster ipMaster, User user)
        {
            return HasPermission(user.Permissions, ipMaster.OrderType, ipMaster.IsCheckPartyFromAuthority, ipMaster.IsCheckPartyToAuthority, ipMaster.PartyFrom, ipMaster.PartyTo);
        }

        public static bool HasPermission(OrderMaster orderMaster, User user)
        {
            return HasPermission(user.Permissions, orderMaster.Type, orderMaster.IsCheckPartyFromAuthority, orderMaster.IsCheckPartyToAuthority, orderMaster.PartyFrom, orderMaster.PartyTo, orderMaster.SubType == OrderSubType.Return ? true : false); ;
        }

        public static bool HasPermission(FlowMaster flowMaster, User user)
        {
            return HasPermission(user.Permissions, flowMaster.Type, flowMaster.IsCheckPartyFromAuthority, flowMaster.IsCheckPartyToAuthority, flowMaster.PartyFrom, flowMaster.PartyTo);
        }

        public static bool HasPermission(PickListMaster pickListMaster, User user)
        {
            return HasPermission(user.Permissions, pickListMaster.OrderType, pickListMaster.IsCheckPartyFromAuthority, pickListMaster.IsCheckPartyToAuthority, pickListMaster.PartyFrom, pickListMaster.PartyTo);
        }

        public static bool HasPermission(Permission[] permissions, OrderType? orderType, bool isCheckPartyFromAuthority, bool isCheckPartyToAuthority, string partyFrom, string partyTo)
        {
            return HasPermission(permissions, orderType, isCheckPartyFromAuthority, isCheckPartyToAuthority, partyFrom, partyTo, false);
        }

        public static bool HasPermission(Permission[] permissions, OrderType? orderType, bool isCheckPartyFromAuthority, bool isCheckPartyToAuthority, string partyFrom, string partyTo, bool isReturn)
        {
            bool hasPermission = true;
            if (orderType == null || orderType == OrderType.Transfer || orderType == OrderType.Production || orderType == OrderType.SubContractTransfer)
            {
                if (orderType == null && partyFrom == null)
                {
                    hasPermission = true;
                }
                else
                {
                    if (isCheckPartyFromAuthority && isCheckPartyToAuthority)
                    {
                        hasPermission = permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Region).Select(t => t.PermissionCode).Contains(partyFrom)
                            && permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Region).Select(t => t.PermissionCode).Contains(partyTo);
                    }
                    else if (isCheckPartyFromAuthority && !isCheckPartyToAuthority)
                    {
                        hasPermission = permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Region).Select(t => t.PermissionCode).Contains(partyFrom);
                    }
                    else if (!isCheckPartyFromAuthority && isCheckPartyToAuthority)
                    {
                        hasPermission = permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Region).Select(t => t.PermissionCode).Contains(partyTo);
                    }
                    else if (!isCheckPartyFromAuthority && !isCheckPartyToAuthority)
                    {
                        hasPermission = permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Region).Select(t => t.PermissionCode).Contains(partyFrom)
                           || permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Region).Select(t => t.PermissionCode).Contains(partyTo);
                    }
                }
            }
            else if (orderType == OrderType.Procurement || orderType == OrderType.SubContract || orderType == OrderType.ScheduleLine)
            {
                if (!isReturn)
                {
                    if (isCheckPartyToAuthority)
                    {
                        hasPermission = permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Supplier).Select(t => t.PermissionCode).Contains(partyFrom)
                            && permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Region).Select(t => t.PermissionCode).Contains(partyTo);
                    }
                    else
                    {
                        hasPermission = permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Supplier).Select(t => t.PermissionCode).Contains(partyFrom);
                    }
                }
                else
                {
                    if (isCheckPartyToAuthority)
                    {
                        hasPermission = permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Supplier).Select(t => t.PermissionCode).Contains(partyTo)
                            && permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Region).Select(t => t.PermissionCode).Contains(partyFrom);
                    }
                    else
                    {
                        hasPermission = permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Supplier).Select(t => t.PermissionCode).Contains(partyTo);
                    }
                }
            }
            else if (orderType == OrderType.Distribution || orderType == OrderType.CustomerGoods)
            {
                if (!isReturn)
                {
                    if (isCheckPartyFromAuthority)
                    {
                        hasPermission = permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Region).Select(t => t.PermissionCode).Contains(partyFrom)
                            && permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Customer).Select(t => t.PermissionCode).Contains(partyTo);
                    }
                    else
                    {
                        hasPermission = permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Customer).Select(t => t.PermissionCode).Contains(partyTo);
                    }
                }
                else
                {
                    if (isCheckPartyFromAuthority)
                    {
                        hasPermission = permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Region).Select(t => t.PermissionCode).Contains(partyTo)
                            && permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Customer).Select(t => t.PermissionCode).Contains(partyFrom);
                    }
                    else
                    {
                        hasPermission = permissions.Where(t => t.PermissionCategoryType == PermissionCategoryType.Customer).Select(t => t.PermissionCode).Contains(partyFrom);
                    }
                }
            }
            return hasPermission;
        }


        #region
        public static void ShowMessageBox(string message)
        {
            message = FormatExMessage(message);
            Sound sound = new Sound(Resources.Error);
            sound.Play();
            DialogResult dr = MessageBox.Show(message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2);

        }

        public static void ShowMessageBox(BusinessException ex)
        {
            string message = FormatExMessage(ex.Message);
            if (ex.MessageParams != null)
            {
                message = string.Format(message, ex.MessageParams);
            }
            ShowMessageBox(message);
        }

        public static void ShowMessageBox(Exception ex)
        {
            string message = FormatExMessage(ex.Message);
            ShowMessageBox(message);
        }

        public static void ShowMessageBox(SoapException ex)
        {
            string message = FormatExMessage(ex.Message);
            ShowMessageBox(message);
        }

        public static string FormatExMessage(string message)
        {
            try
            {
                if (message.StartsWith("System.Web.Services.Protocols.SoapException"))
                {
                    message = message.Remove(0, 44);
                    int index = message.IndexOf("\n");
                    if (index > 0)
                    {
                        message = message.Remove(index, message.Length - index);
                    }
                    index = message.IndexOf("\r\n");
                    if (index > 0)
                    {
                        message = message.Remove(index, message.Length - index);
                    }
                }
                if (message.Contains("NHibernate.ObjectNotFoundException"))
                {
                    message = "输入错误,没有找到此对象";
                }
                message = message.Replace("\\n", "\r\n");
                return message;
            }
            catch (Exception)
            {
                return message;
            }
        }

        public static string Md5(string originalPassword)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(originalPassword);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }

        public static void ValidatorDecimal(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.ToString() == "\b" || e.KeyChar.ToString() == "." || e.KeyChar.ToString() == "-")
            {
                string str = string.Empty;
                if (e.KeyChar.ToString() == "\b")
                {
                    e.Handled = false;
                    return;
                }
                else
                {
                    str = ((TextBox)sender).Text.Trim() + e.KeyChar.ToString();
                }

                if (Utility.IsDecimal(str) || str == "-")
                {
                    e.Handled = false;
                    return;
                }
            }

            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
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
        #endregion

        public static void SetKeyCodeDiff(int diff)
        {
            Microsoft.Win32.RegistryKey subKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SconitKeyCodeDiff", true);
            if (subKey == null)
            {
                subKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SconitKeyCodeDiff");
            }
            subKey.SetValue("KeyCodeDiff", diff);
            subKey.Close();
        }

        public static int GetKeyCodeDiff()
        {
            Microsoft.Win32.RegistryKey subKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SconitKeyCodeDiff");
            if (subKey == null)
            {
                return 0;
            }
            int selectIndex = (int)(subKey.GetValue("KeyCodeDiff"));
            subKey.Close();
            return selectIndex;
        }
    }

    public class Sound
    {
        private byte[] m_soundBytes;

        private string m_fileName;

        private enum Flags
        {
            SND_SYNC = 0x0000,  /* play synchronously (default) */
            SND_ASYNC = 0x0001,  /* play asynchronously */
            SND_NODEFAULT = 0x0002,  /* silence (!default) if sound not found */
            SND_MEMORY = 0x0004,  /* pszSound points to a memory file */
            SND_LOOP = 0x0008,  /* loop the sound until next sndPlaySound */
            SND_NOSTOP = 0x0010,  /* don't stop any currently playing sound */
            SND_NOWAIT = 0x00002000, /* don't wait if the driver is busy */
            SND_ALIAS = 0x00010000, /* name is a registry alias */
            SND_ALIAS_ID = 0x00110000, /* alias is a predefined ID */
            SND_FILENAME = 0x00020000, /* name is file name */
            SND_RESOURCE = 0x00040004  /* name is resource name or atom */
        }

        [DllImport("CoreDll.DLL", EntryPoint = "PlaySound", SetLastError = true)]
        private extern static int WCE_PlaySound(string szSound, IntPtr hMod, int flags);

        [DllImport("CoreDll.DLL", EntryPoint = "PlaySound", SetLastError = true)]
        private extern static int WCE_PlaySoundBytes(byte[] szSound, IntPtr hMod, int flags);

        /// <summary>
        /// Construct the Sound object to play sound data from the specified file.
        /// </summary>
        public Sound(string fileName)
        {
            m_fileName = fileName;
        }

        /// <summary>
        /// Construct the Sound object to play sound data from the specified stream.
        /// </summary>
        public Sound(Stream stream)
        {
            // read the data from the stream
            m_soundBytes = new byte[stream.Length];
            stream.Read(m_soundBytes, 0, (int)stream.Length);
        }

        /// <summary>
        /// Construct the Sound object to play sound data from the specified byte.
        /// </summary>
        public Sound(byte[] m_soundBytes)
        {
            this.m_soundBytes = m_soundBytes;
        }

        /// <summary>
        /// Play the sound
        /// </summary>
        public void Play()
        {
            // if a file name has been registered, call WCE_PlaySound,
            //  otherwise call WCE_PlaySoundBytes
            try
            {
                if (m_fileName != null)
                {
                    WCE_PlaySound(m_fileName, IntPtr.Zero, (int)(Flags.SND_ASYNC | Flags.SND_FILENAME));
                }
                else
                {
                    WCE_PlaySoundBytes(m_soundBytes, IntPtr.Zero, (int)(Flags.SND_ASYNC | Flags.SND_MEMORY));
                }

            }
            catch (Exception)
            {
                //throw;
            }
        }


    }
}
