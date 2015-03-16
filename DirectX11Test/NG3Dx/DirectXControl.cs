using System.Windows.Forms;
using NG3Dx.Managers;
using SlimDX.Direct3D11;
using SlimDX;
using SlimDX.DXGI;
using NG3Dx.Geometrics;
using System;
using NG3Dx.Imports;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace NG3Dx
{
    public partial class DirectXControl : UserControl
    {
        private DeviceManager deviceManager;
        private CameraManager cameraManager;

        private Grid gridData;
        private List<Model> listModels;
        private Model plane;
        private Model triangle;

        public DirectXControl()
        {
            InitializeComponent();

            NG3Dx.Debug.ConsoleDebug.Show();

            deviceManager = new DeviceManager(this);
            cameraManager = new CameraManager();

            MouseWheel += new MouseEventHandler(DirectXControl_MouseWheel);
            MouseDown += new MouseEventHandler(DirectXControl_MouseDown);
            MouseMove += new MouseEventHandler(DirectXControl_MouseMove);
            MouseUp += new MouseEventHandler(DirectXControl_MouseUp);

            Resize += new EventHandler(DirectXControl_Resize);

            CameraManager.OrbitPanCamera.eye = new Vector3(200, 150, 0);
            CameraManager.OrbitPanCamera.target = new Vector3(0, 0, 0);
            CameraManager.OrbitPanCamera.up = new Vector3(0, 1, 0);

            CameraManager.OrbitPanCamera.SetView();

            gridData = new Grid(50, 100.0f);
            gridData.color = new Color4(1, 1, 1 );
            gridData.modelType = eModelType.None;

            listModels = ObjLoader.LoadFile(Application.StartupPath + "\\Terrian\\Beach\\", "Beach.obj");
            
            foreach (Model model in listModels)
            {
                model.Transform = Matrix.Scaling(10, 10, 10) * Matrix.Reflection(new Plane(new Vector3(1, 0, 0), -1));
            }

            List<Vector3> listVertices = new List<Vector3>();
            //listVertices.Add(new Vector3(-2.3f, -10, 0));
            //listVertices.Add(new Vector3(-1.1f, 10, 0));
            //listVertices.Add(new Vector3(6.2f, -10, 0));
            //listVertices.Add(new Vector3(2.8f, 10, 0));
            //listVertices.Add(new Vector3(10f, -9.7f, 0));
            //listVertices.Add(new Vector3(6.7f, 10f, 0));
            
            listVertices.Add(new Vector3(-10f, -10, 0));
            listVertices.Add(new Vector3( 10f, -10, 0));
            listVertices.Add(new Vector3( 10f,  10, 0));
            listVertices.Add(new Vector3(-10f,  10, 0));
            plane = new Model(listVertices);
            plane.Transform = Matrix.Scaling(10, 10, 10);
            plane.color = new Color4(1, 1, 1);
            plane.ComputeNormalsAndTextCoords();

            Triangle tria = new Triangle(
                new SlimDX.Vector3(0, 500.5f, 500.5f),
                new SlimDX.Vector3(500.5f, -500.5f, 500.5f),
                new SlimDX.Vector3(-500.5f, -500.5f, 500.5f));
            triangle = new Model(new List<Triangle> { tria });
            
            List<Triangle> listtrie = new List<Triangle>();
            listtrie.Add(tria);
            triangle = new Model(listtrie);
            triangle.color = new Color4(1, 0, 0);
        }

        private void DirectXControl_MouseUp(object sender, MouseEventArgs e)
        {
            CameraManager.OrbitPanCamera.MouseUp(sender, e);
        }

        private void DirectXControl_MouseDown(object sender, MouseEventArgs e)
        {
            CameraManager.OrbitPanCamera.MouseDown(sender, e);
        }

        private void DirectXControl_MouseMove(object sender, MouseEventArgs e)
        {
            CameraManager.OrbitPanCamera.MouseMove(sender, e);
            Render();  
        }

        private void DirectXControl_MouseWheel(object sender, MouseEventArgs e)
        {
            CameraManager.OrbitPanCamera.MouseWheel(sender, e);
            Render();  
        }

        private void DirectXControl_Paint(object sender, PaintEventArgs e)
        {
            Render();   
        }

        private void Render()
        {
            DeviceManager.DirectXDevice.context.ClearDepthStencilView(DeviceManager.DirectXDevice.depthStencil,
                        DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil,
                        1.0f,
                        0);
            
            DeviceManager.DirectXDevice.context.ClearRenderTargetView(DeviceManager.DirectXDevice.renderTarget, new Color4(System.Drawing.Color.Black));
            
            gridData.Render();
 
            foreach (Model model in listModels)
            {
                model.Render();
            }

            plane.Render();
            //triangle.Render();
            DeviceManager.DirectXDevice.swapChain.Present(0, PresentFlags.None);
        }

        private void DirectXControl_Resize(object sender, EventArgs e)
        {
            try
            {
                if (DeviceManager.DirectXDevice.device == null)
                    return;

                CameraManager.OrbitPanCamera.SetPerspective((float)Math.PI / 4, (float)ClientSize.Width / (float)ClientSize.Height, 1f, 10000f);
                CameraManager.OrbitPanCamera.SetOrthographic((float)ClientSize.Width, (float)ClientSize.Height, -10000f, 10000.0f);

                if (DeviceManager.DirectXDevice.renderTarget != null)
                    DeviceManager.DirectXDevice.renderTarget.Dispose();

                if (DeviceManager.DirectXDevice.resource != null)
                    DeviceManager.DirectXDevice.resource.Dispose();

                if (DeviceManager.DirectXDevice.depthStencil != null)
                    DeviceManager.DirectXDevice.depthStencil.Dispose();

                DeviceManager.DirectXDevice.CreateDepthStencilView(this);

                DeviceManager.DirectXDevice.swapChain.ResizeBuffers(1,
                  ClientSize.Width,
                  ClientSize.Height,
                  Format.R8G8B8A8_UNorm,
                  SwapChainFlags.AllowModeSwitch);

                DeviceManager.DirectXDevice.resource = Texture2D.FromSwapChain<Texture2D>(DeviceManager.DirectXDevice.swapChain, 0);
                DeviceManager.DirectXDevice.renderTarget = new RenderTargetView(DeviceManager.DirectXDevice.device, DeviceManager.DirectXDevice.resource);

                DeviceManager.DirectXDevice.viewport = new Viewport(0.0f, 0.0f, ClientSize.Width, ClientSize.Height);
                DeviceManager.DirectXDevice.context.Rasterizer.SetViewports(DeviceManager.DirectXDevice.viewport);
                DeviceManager.DirectXDevice.context.OutputMerger.SetTargets(DeviceManager.DirectXDevice.depthStencil, DeviceManager.DirectXDevice.renderTarget);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
