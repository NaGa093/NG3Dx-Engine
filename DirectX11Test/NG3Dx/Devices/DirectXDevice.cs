using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NG3Dx.Cameras;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D11.Device;

namespace NG3Dx.Devices
{
    public enum HELPER_STATES
    {
        CULL_BACK,
        CULL_FRONT,
        NO_CULLING,
        CULL_BACK_MULTISAMPLING,
        CULL_FRONT_MULTISAMPLING,
        NO_CULLING_MULTISAMPLING,
    };

    public class DirectXDevice
    {
        public Device device;
        public SwapChain swapChain;
        public Viewport viewport;
        public Texture2D resource;
        public RenderTargetView renderTarget;
        public DepthStencilView depthStencil;
        public DeviceContext context;
        
        private int MSAACount;
        private Texture2D DSTexture;
        private DepthStencilState dss;
        private DepthStencilState depthStencilStateNormal;
        
        public DirectXDevice(Control control)
        {
            FeatureLevel[] featureLevel = new[]
            {
               FeatureLevel.Level_11_0
            };

            Factory factory = new Factory();
            SlimDX.DXGI.Adapter MyAdapter = factory.GetAdapter(factory.GetAdapterCount() - 2);
            device = new Device(MyAdapter, DeviceCreationFlags.None, featureLevel);
            MSAACount = device.CheckMultisampleQualityLevels(Format.R8G8B8A8_UNorm, 1);

            var description = new SwapChainDescription()
            {
                BufferCount = 1,
                Usage = Usage.RenderTargetOutput,
                OutputHandle = control.Handle,
                IsWindowed = true,
                ModeDescription = new ModeDescription(control.Width, control.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(MSAACount, 0),
                Flags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard
            };

            swapChain = new SwapChain(factory, device, description);

            //Device.CreateWithSwapChain(SlimDX.Direct3D11.DriverType.Hardware, DeviceCreationFlags.BgraSupport, featureLevel, description, out device, out swapChain);
           
            resource = SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(swapChain, 0);
            renderTarget = new RenderTargetView(device, resource);

            context = device.ImmediateContext;
            viewport = new Viewport(0.0f, 0.0f, control.Width, control.Height);
            context.OutputMerger.SetTargets(renderTarget);
            context.Rasterizer.SetViewports(viewport);
        }

        public void CreateDepthStencilView(Control control)
        {
            if (DSTexture != null)
                DSTexture.Dispose();

            DSTexture = new Texture2D(
                device,
                new Texture2DDescription()
                {
                    ArraySize = 1,
                    MipLevels = 1,
                    Format = Format.D32_Float,
                    Width = control.Width,
                    Height = control.Height,
                    BindFlags = BindFlags.DepthStencil,
                    CpuAccessFlags = CpuAccessFlags.None,
                    SampleDescription = new SampleDescription(MSAACount, 0),
                    Usage = ResourceUsage.Default
                }
            );

            if (depthStencil != null)
                depthStencil.Dispose();

            depthStencil = new DepthStencilView(
               device,
               DSTexture,
               new DepthStencilViewDescription()
               {
                   ArraySize = 0,
                   FirstArraySlice = 0,
                   MipSlice = 0,
                   Format = Format.D32_Float,
                   Dimension = (DSTexture.Description.SampleDescription.Count > 1 ? DepthStencilViewDimension.Texture2DMultisampled : DepthStencilViewDimension.Texture2D)
               }
           );

            if (dss != null)
                dss.Dispose();

            dss = DepthStencilState.FromDescription(
               device,
               new DepthStencilStateDescription()
               {
                   DepthComparison = Comparison.Always,
                   DepthWriteMask = DepthWriteMask.All,
                   IsDepthEnabled = true,
                   IsStencilEnabled = false
               }
           );

            context.OutputMerger.DepthStencilState = dss;
            context.OutputMerger.SetTargets(depthStencil, renderTarget);

            DepthStencilStateDescription dssd = new DepthStencilStateDescription
            {
                IsDepthEnabled = true,
                IsStencilEnabled = false,
                DepthWriteMask = DepthWriteMask.All,
                DepthComparison = Comparison.Less,
            };

            depthStencilStateNormal = DepthStencilState.FromDescription(device, dssd);
            context.OutputMerger.DepthStencilState = depthStencilStateNormal;
        }
    }
}
