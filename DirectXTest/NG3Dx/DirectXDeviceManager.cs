using System.Windows.Controls;

namespace NG3Dx
{
    public class DirectXDeviceManager
    {
        private DirectXDevice _directXDevice;

        public DirectXDeviceManager(Control control)
        {
            _directXDevice = new DirectXDevice(control);
        }
  
        public DirectXDevice DirectXDevice
        {
            get
            {
                return _directXDevice;
            }
        }
    }
}
