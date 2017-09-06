using System;

using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace com.Sconit.SmartDevice
{
    static class Program
    {

        const string appName = "SmartDevice";
        const int ALREADY_EXISTS = 183;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            try
            {
                startTicks = Environment.TickCount;

                using (AppExecutionManager execMgr = new AppExecutionManager(appName))
                {
                    if (execMgr.IsFirstInstance)
                    {
                        Application.Run(new MainForm());
                    }
                }
            }
            catch (Exception)
            {
                Application.Run(new MainForm());
            }
            if (Environment.OSVersion.Platform.ToString().ToUpper().Contains("WINCE"))
            {
                Utility.SetKeyCodeDiff(0);
            }
            else
            {
                Utility.SetKeyCodeDiff(0);
            }
        }
        internal static int startTicks;
    }
}