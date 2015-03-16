using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NG3Dx.Devices;
using SlimDX.Direct3D11;

namespace NG3Dx.Managers
{
    public class DeviceManager
    {
        private static DirectXDevice directXDevice;

        public DeviceManager(Control control)
        {
            directXDevice = new DirectXDevice(control);
        }
  
        public static DirectXDevice DirectXDevice
        {
            get
            {
                return directXDevice;
            }
        }
    }
}
