using System.Windows;

namespace DirectXTest
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Hide();
            var devices = new DeviceWindow(DirectXControl_UserControl.DirectXDeviceManager.DirectXDevice.FactoryHelper);
            devices.Show();
            devices.Closed += devices_Closed;
        }

        void devices_Closed(object sender, System.EventArgs e)
        {
            Width = DirectXControl_UserControl.DirectXDeviceManager.DirectXDevice.FactoryHelper.Width;
            Height = DirectXControl_UserControl.DirectXDeviceManager.DirectXDevice.FactoryHelper.Height;
            Show();
            DirectXControl_UserControl.DirectXDeviceManager.DirectXDevice.InitDevice();
        }

        private void Layout_Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DirectXControl_UserControl.Width = e.NewSize.Width;
            DirectXControl_UserControl.Height = e.NewSize.Height;
        }
    }
}
