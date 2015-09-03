using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using NG3Dx.Cameras;
using NG3Dx.Helpers;
using NG3Dx.Models;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D11.Device;
using Grid = NG3Dx.Models.Grid;
using Resource = SlimDX.Direct3D11.Resource;

namespace NG3Dx
{
    public class DirectXDevice
    {
        private Device _device;
        private SwapChain _swapChain;
        private Viewport _viewport;
        private RenderTargetView _renderTargetView;
        private DepthStencilView _depthStencil;
        private DeviceContext _context;

        private readonly Control _control;
        private int _msaaCount;
        private Texture2D _dsTexture;

        public DeviceHelper FactoryHelper { get; private set; }
        public CameraBase Camera { get; private set; }

        private Grid _grid;
        private List<Model> _models;

        public DirectXDevice(Control control)
        {
            _control = control; 
            FactoryHelper = new DeviceHelper();
            Camera = new CameraFps
            {
                Eye = new Vector3(-200, 150, -100),
                Target = new Vector3(0, 0, 0),
                Up = new Vector3(0, 1, 0),
                Look = new Vector3(1, 0, 0)
            };

            Camera.SetView();
            Camera.SetPerspective((float)Math.PI / 4, (float)_control.Width / (float)_control.Height, 1f, 100000f);
            Camera.SetOrthographic((float)_control.Width, (float)_control.Height, -100000f, 100000.0f);
        }

        public void InitDevice()
        {
            _device = new Device(FactoryHelper.SelectedAdapter, DeviceCreationFlags.None);
            _msaaCount = _device.CheckMultisampleQualityLevels(Format.R8G8B8A8_UNorm, Device.MultisampleCountMaximum);
            _context = _device.ImmediateContext;

            var source = (HwndSource)PresentationSource.FromVisual(_control);

            if (source != null)
            {
                var description = new SwapChainDescription
                {
                    BufferCount = 1,
                    Usage = Usage.RenderTargetOutput,
                    OutputHandle = source.Handle,
                    IsWindowed = true,
                    ModeDescription = new ModeDescription((int)_control.Width, (int)_control.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm)
                    {
                        ScanlineOrdering = DisplayModeScanlineOrdering.Unspecified,
                        Scaling = DisplayModeScaling.Unspecified
                    },
                    SampleDescription = new SampleDescription(4, _msaaCount - 1),
                    Flags = SwapChainFlags.AllowModeSwitch,
                    SwapEffect = SwapEffect.Discard
                };

                _swapChain = new SwapChain(_device.Factory, _device, description);
            }

            Resize();

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        Application.Current.Dispatcher.Invoke(Render);
                    }
                    catch (Exception)
                    {

                    }
                }
            });
        }

        public void Render()
        {
            if (_device == null)
                return;

            if (_grid == null)
            {
                _grid = new Grid(_device,50,100.0f);
            }

            if (_models == null)
            {
                //_models = Model.CreateModels(AppDomain.CurrentDomain.BaseDirectory + @"MetroModel\l00_intro\l00_intro.fbx", AppDomain.CurrentDomain.BaseDirectory + @"MetroModel\", _device);
                _models = Model.CreateModels(AppDomain.CurrentDomain.BaseDirectory + @"MetroModel\duck\duck.dae", AppDomain.CurrentDomain.BaseDirectory + @"MetroModel\duck", _device);
                _models[0].Transform = Matrix.Translation(100, 50, 200);
                _models.AddRange(Model.CreateModels(AppDomain.CurrentDomain.BaseDirectory + @"MetroModel\Small Tropical Island\Small Tropical Island.obj", AppDomain.CurrentDomain.BaseDirectory + @"MetroModel\Small Tropical Island", _device));
                //_models = Model.CreateModels(AppDomain.CurrentDomain.BaseDirectory + @"MetroModel\riga\riga.fbx", AppDomain.CurrentDomain.BaseDirectory + @"MetroModel\riga\,", _device);
            }

            _context.ClearRenderTargetView(_renderTargetView, new Color4(Color.Black));
            _context.ClearDepthStencilView(_depthStencil, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
            
            _grid.Render(_context, Camera);

            foreach (var model in _models)
            {
                model.Render(_context, Camera);
            }
            _swapChain.Present(0, PresentFlags.None);
        }

        public void Resize()
        {
            if (_device == null)
                return;

            if (_renderTargetView != null) _renderTargetView.Dispose();
            if (_dsTexture != null) _dsTexture.Dispose();
            if (_depthStencil != null) _depthStencil.Dispose();
           
            _swapChain.ResizeBuffers(1, (int)_control.Width, (int)_control.Height, Format.R8G8B8A8_UNorm, SwapChainFlags.None);

            using (var resource = Resource.FromSwapChain<Texture2D>(_swapChain, 0))
            {
                _renderTargetView = new RenderTargetView(_device, resource);
            }

            _dsTexture = new Texture2D(
                _device,
                new Texture2DDescription
                {
                    ArraySize = 1,
                    MipLevels = 1,
                    Format = Format.D24_UNorm_S8_UInt,
                    Width = (int)_control.Width,
                    Height = (int)_control.Height,
                    BindFlags = BindFlags.DepthStencil,
                    CpuAccessFlags = CpuAccessFlags.None,
                    SampleDescription = new SampleDescription(4, _msaaCount - 1),
                    Usage = ResourceUsage.Default,
                    OptionFlags = ResourceOptionFlags.None
                }
            );
            
            _depthStencil = new DepthStencilView(_device, _dsTexture);
            _context.OutputMerger.SetTargets(_depthStencil, _renderTargetView);
            _viewport = new Viewport(0.0f, 0.0f, (int)_control.Width, (int)_control.Height, 0.0f, 1.0f);
            _context.Rasterizer.SetViewports(_viewport);           
        }
    }
}
