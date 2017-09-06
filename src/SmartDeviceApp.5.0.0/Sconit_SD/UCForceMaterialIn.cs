using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using com.Sconit.SmartDevice.SmartDeviceRef;

namespace com.Sconit.SmartDevice
{
    public partial class UCForceMaterialIn : UCMaterialIn
    {
        public UCForceMaterialIn(User user, bool isReturn)
            : base(user, false)
        {
            InitializeComponent();
            base.isForceFeed = true;
        }
    }
}
