using System;
using System.Windows;
using System.Windows.Input;

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
            Cursor = Cursors.None;
        }

        void DirectXControl_Loaded(object sender, RoutedEventArgs e)
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
            const double sensitivity = 10.0;

            if (_currentPosition.X == 0 && _currentPosition.Y == 0)
            {
                _currentPosition = Mouse.GetPosition(this);
            }

            var y = -(_currentPosition.Y - Mouse.GetPosition(this).Y);
            var x = -(_currentPosition.X - Mouse.GetPosition(this).X);

            if (Mouse.GetPosition(this).X  == 0)
            {
                (DirectXDeviceManager.DirectXDevice.Camera).Yaw(-10 / sensitivity);
            }
            else if (Mouse.GetPosition(this).X == DesiredSize.Width - 1)
            {
                (DirectXDeviceManager.DirectXDevice.Camera).Yaw(10 / sensitivity);
            }
            else
            {
                (DirectXDeviceManager.DirectXDevice.Camera).Yaw(x / sensitivity);
            }

            (DirectXDeviceManager.DirectXDevice.Camera).Pitch(y / sensitivity);
            
            _currentPosition = Mouse.GetPosition(this);
        }

        void DirectXControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DirectXDeviceManager.DirectXDevice.Resize();
        }
    }
}
