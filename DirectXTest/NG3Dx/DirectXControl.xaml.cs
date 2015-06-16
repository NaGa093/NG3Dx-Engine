using System.Drawing;
using System.Windows.Input;
using NG3Dx.Helpers;

namespace NG3Dx
{
    public partial class DirectXControl  
    {
        public DirectXDeviceManager DirectXDeviceManager { get; set; }

        private Point _currentPosition;

        public DirectXControl()
        {
            InitializeComponent();
            DirectXDeviceManager = new DirectXDeviceManager(this);
            SizeChanged += DirectXControl_SizeChanged;
            MouseMove += DirectXControl_MouseMove;
            PreviewKeyDown += DirectXControl_PreviewKeyDown;
            Loaded += DirectXControl_Loaded;
            Focusable = true;
            _currentPosition = new Point(0,0);
        }

        void DirectXControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Focus();
        }

        void DirectXControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                (DirectXDeviceManager.DirectXDevice.Camera).Strafe(100);
            }
            else if (e.Key == Key.D)
            {
                (DirectXDeviceManager.DirectXDevice.Camera).Strafe(-100);
            }
            else if (e.Key == Key.W)
            {
                (DirectXDeviceManager.DirectXDevice.Camera).Move(100);
            }
            else if (e.Key == Key.S)
            {
                (DirectXDeviceManager.DirectXDevice.Camera).Move(-100);
            }
        }

        void DirectXControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_currentPosition.X == 0 && _currentPosition.Y == 0)
            {
                _currentPosition = MouseHelper.GetMousePosition();
            }
            
            (DirectXDeviceManager.DirectXDevice.Camera).Pitch(-(_currentPosition.Y - MouseHelper.GetMousePosition().Y));
            (DirectXDeviceManager.DirectXDevice.Camera).Yaw(-(_currentPosition.X - MouseHelper.GetMousePosition().X));
            _currentPosition = MouseHelper.GetMousePosition();
        }

        void DirectXControl_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            DirectXDeviceManager.DirectXDevice.Resize();
        }
    }
}
