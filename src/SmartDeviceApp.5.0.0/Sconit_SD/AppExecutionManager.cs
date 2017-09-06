using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace com.Sconit.SmartDevice
{

    public class AppExecutionManager : IDisposable
    {
        public AppExecutionManager(string appName)
        {
            _eventHandle = CreateEvent(IntPtr.Zero, true, false, appName + "Event");
            _isFirstInstance = Marshal.GetLastWin32Error() == 0;
        }

        public bool IsFirstInstance
        {
            get { return _isFirstInstance; }
        }

        public void Dispose()
        {
            if (_eventHandle != IntPtr.Zero)
                CloseHandle(_eventHandle);
        }

        private bool _isFirstInstance;
        private IntPtr _eventHandle = IntPtr.Zero;

        #region Imports

        [DllImport("Coredll.dll", SetLastError = true)]
        static extern IntPtr CreateEvent(IntPtr alwaysZero, bool manualReset, bool initialState, string name);
        [DllImport("Coredll.dll", SetLastError = true)]
        static extern int CloseHandle(IntPtr handle);

        #endregion

    }
}
