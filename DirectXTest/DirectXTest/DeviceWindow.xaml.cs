using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using NG3Dx.Helpers;

namespace DirectXTest
{
    public partial class DeviceWindow
    {
        private readonly DeviceHelper _factoryHelper;

        public DeviceWindow(DeviceHelper factoryHelper)
        {
            InitializeComponent();
            _factoryHelper = factoryHelper;
            Devices_ComboBox.ItemsSource = factoryHelper.GetAdapters().Keys;
            Devices_ComboBox.SelectedIndex = 0;
            Resolution_ComboBox.ItemsSource = factoryHelper.Resolutions;
            Resolution_ComboBox.SelectedIndex = 0;
        }

        private void Devices_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Memory_TextBlock.Text = _factoryHelper.GetAdapters().ElementAt(((ComboBox)sender).SelectedIndex).Value.ToString(CultureInfo.InvariantCulture);
        }

        private void OK_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _factoryHelper.SetAdapter(Devices_ComboBox.SelectedItem.ToString());
            _factoryHelper.SetResolution(Resolution_ComboBox.SelectedItem.ToString());
            Close();
        }
    }
}
