using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SlimDX.DXGI;

namespace NG3Dx.Helpers
{
    public class DeviceHelper
    {
        public Adapter SelectedAdapter { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Factory Factory { get; private set; }
        public List<string> Resolutions { get; private set; }

        private readonly Collection<Adapter> _adapters;

        public DeviceHelper()
        {
            Factory = new Factory();
            _adapters = new Collection<Adapter>();
            Resolutions = new List<string>();

            for (var i = 0; i < Factory.GetAdapterCount(); i++)
            {
                _adapters.Add(Factory.GetAdapter(i));
            }

            Resolutions.Add("800x600");
            Resolutions.Add("1024x768");
            Resolutions.Add("1024x819");
            Resolutions.Add("1093x614");
            Resolutions.Add("1152x864");
            Resolutions.Add("1164x931");
            Resolutions.Add("1170x936");
            Resolutions.Add("1200x750");
            Resolutions.Add("1202x751");
            Resolutions.Add("1280x1024");
            Resolutions.Add("1280x720");
            Resolutions.Add("1280x752");
            Resolutions.Add("1280x768");
            Resolutions.Add("1280x800");
            Resolutions.Add("1280x960");
            Resolutions.Add("1311x737");
            Resolutions.Add("1336x751");
            Resolutions.Add("1344x840");
            Resolutions.Add("1347x1078");
            Resolutions.Add("1360x768");
            Resolutions.Add("1366x768");
            Resolutions.Add("1400x1050");
            Resolutions.Add("1440x900");
            Resolutions.Add("1536x864");
            Resolutions.Add("1600x900");
            Resolutions.Add("1680x1050");
            Resolutions.Add("1745x982");
            Resolutions.Add("1920x1080");
            Resolutions.Add("1920x1200");
        }

        public Dictionary<string, long> GetAdapters()
        {
            return _adapters.ToDictionary(adapter => adapter.Description.Description, adapter => adapter.Description.DedicatedVideoMemory);
        }

        public void SetAdapter(string DeviceDescription)
        {
            SelectedAdapter = _adapters.FirstOrDefault(x => x.Description.Description == DeviceDescription);
        }

        public void SetResolution(string resolution)
        {
            Width = int.Parse(resolution.Split('x')[0]);
            Height = int.Parse(resolution.Split('x')[1]);
        }
    }
}
